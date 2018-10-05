using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace functionbutton
{
    public class KeyboardHook
    {
        #region User32 DLL Declare For All

        [DllImport("user32.dll")]
        private static extern Int32 PostMessage(Int32 hWnd, Int32 wMsg, Int32 wParam, Int32 lParam);

        #endregion

        #region Windows structure definitions

        [StructLayout(LayoutKind.Sequential)]
        private class POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class MouseLLHookStruct
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private class KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        #endregion

        #region Windows function imports

        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("user32")]
        private static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        #endregion

        #region Windows constants

        private const int WH_MOUSE_LL = 14;
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE = 7;
        private const int WH_KEYBOARD = 2;

        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;
        private const int WM_MOUSEWHEEL = 0x020A;

        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        private const byte VK_SHIFT = 0x10;
        private const byte VK_CAPITAL = 0x14;
        private const byte VK_NUMLOCK = 0x90;

        #endregion

        #region Hook Original Event Handler

        public event KeyPressEventHandler KeyPressForHook;
        public event KeyEventHandler KeyUpForHook, KeyDownForHook;

        #endregion

        #region Hook Original Attribute Declare

        private int hKeyboardHook = 0;
        private HookProc KeyboardHookProcedure;

        #endregion

        #region HotTab Keyboard Hook Const & Enumeration Declare

        private enum KeyboardInputState
        {
            State_None,
            State_StartWithAlt,
            State_StartWithCtrl,
            State_StartWithAltCtrl,
            State_StartWithCtrlAlt,
            State_StartWithAltCtrlShift,
            State_StartWithCtrlAltShift,
        }

        #endregion

        #region HotTab Attribute Declare

        private Int32 hwndMainForm;
        private KeyboardInputState keyboardInputState;
        private DateTime beforeKeyboardInputStateDateTime;
        private TimeSpan KeyboardInputDifferenceTime;

        #endregion

        bool IsDebugMode = true;
        public static int KEY_MenuToHome = 0;       // 用來判斷是否把Menu鍵當作Home鍵使用, 對應EC中的_KEY_MenuToHome Token

        // 原本為FormHotTabHook.cs(含WinForm介面)
        // public KeyboardHook(bool InstallKeyboardHook) 
        public KeyboardHook(int handle, bool InstallKeyboardHook)
        {
            // InitializeComponent(); // 取消System.Windows.Forms
            hwndMainForm = handle;
            Start(InstallKeyboardHook);
        }

        ~KeyboardHook()
        {
            Stop(true, false);
        }


        public void Start(bool InstallKeyboardHook)
        {
            if (IsDebugMode) Trace.WriteLine("KeyboardHook() Start");
            if (hKeyboardHook == 0 && InstallKeyboardHook)
            {
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProcedure, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
                if (IsDebugMode) Trace.WriteLine("ModuleName : " + Process.GetCurrentProcess().MainModule.ModuleName + ", Handle : " + GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName));
                if (hKeyboardHook == 0)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Stop(true, false);
                    throw new Win32Exception(errorCode);
                }
                else
                {
                    beforeKeyboardInputStateDateTime = DateTime.Now;
                    keyboardInputState = KeyboardInputState.State_None;
                    if (IsDebugMode) Trace.WriteLine("Add KeyUpForHook()");
                    this.KeyUpForHook += new KeyEventHandler(this.KeyboradUpHandler);
                    // this.KeyDownForHook += new KeyEventHandler(this.KeyboradDownHandler);
                }
            }
        }

        public void Stop(bool UninstallKeyboardHook, bool ThrowExceptions)
        {
            if (IsDebugMode) Trace.WriteLine("KeyboardHook() Stop");
            if (hKeyboardHook != 0 && UninstallKeyboardHook)
            {
                int retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                hKeyboardHook = 0;
                if (retKeyboard == 0 && ThrowExceptions)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            bool handled = false;
            KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
            if ((nCode >= 0) && (KeyDownForHook != null || KeyUpForHook != null || KeyPressForHook != null))
            {
                if (KeyDownForHook != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.scanCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    KeyDownForHook(this, e);
                    handled = handled || e.Handled;
                }

                if (KeyPressForHook != null && wParam == WM_KEYDOWN)
                {
                    bool isDownShift = ((GetKeyState(VK_SHIFT) & 0x80) == 0x80 ? true : false);
                    bool isDownCapslock = (GetKeyState(VK_CAPITAL) != 0 ? true : false);

                    byte[] keyState = new byte[256];
                    GetKeyboardState(keyState);
                    byte[] inBuffer = new byte[2];
                    if (ToAscii(MyKeyboardHookStruct.vkCode, MyKeyboardHookStruct.scanCode, keyState, inBuffer, MyKeyboardHookStruct.flags) == 1)
                    {
                        char key = (char)inBuffer[0];
                        if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                        KeyPressEventArgs e = new KeyPressEventArgs(key);
                        KeyPressForHook(this, e);
                        handled = handled || e.Handled;
                    }
                }

                if (KeyUpForHook != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.scanCode;
                    Keys keyDataVk = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    if (keyDataVk == Keys.VolumeUp || keyDataVk == Keys.VolumeDown) e = new KeyEventArgs(keyDataVk);

                    KeyUpForHook(this, e);
                    handled = handled || e.Handled;
                }
            }

            if (handled)
                return 1;
            else
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }

        private void KeyboradUpHandler(object Asender, KeyEventArgs e)
        {
            // ShowDialogMessageBox("KeyboradUpHandler , KeyEventArgs = " + e.KeyValue + " , keyboardInputState = " + keyboardInputState);
            if (e.KeyValue == 541) return; // French Mode, Because the alt key will send the code.

            KeyboardInputDifferenceTime = DateTime.Now - beforeKeyboardInputStateDateTime;
            beforeKeyboardInputStateDateTime = DateTime.Now;
            if (KeyboardInputDifferenceTime.TotalMilliseconds > 200)
            {
#if IsInstallHotTab
                keyboardInputState = KeyboardInputState.State_None;
#endif
                beforeKeyboardInputStateDateTime = DateTime.Now;
            }
            else
            {
                beforeKeyboardInputStateDateTime = DateTime.Now;
            }

            Trace.WriteLine(e.KeyValue.ToString());
            switch (keyboardInputState)
            {
                #region KeyboardInputState.State_None
                case KeyboardInputState.State_None:
                    // Trace.WriteLine("KeyboardInputState.State_None : " + e.KeyValue);
                    switch (e.KeyValue) // (Translate.pdf => PS/2 Set 1 Make) 轉換表是16進位, 這邊要改成10進位的KeyValue
                    {
                        case 1:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_ESC, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 2:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_1, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 3:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_2, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 4:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_3, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 5:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_4, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 6:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_5, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 7:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_6, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 8:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_7, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 9:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_8, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 10:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_9, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 11:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_0, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 12: // 0C(- _)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_OemMinus, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 13: // 0D(= +)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_OemPlus, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 14:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_BACKSPACE, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 15:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_TAB, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 16:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Q, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 17:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_W, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 18:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_E, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 19:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_R, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 20:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_T, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 21:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Y, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 22:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_U, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 23:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_I, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 24:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_O, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 25:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_P, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 26:// 1A([ {)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_OemOpenBrackets, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 27:// 1B(] })
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Oem6, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 28: // ENTER
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Enter, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 29: // CTRL
                            // if (IsDebugMode) Trace.WriteLine("KeyboardHook : Ctrl");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_CTRL, 0, 0); // kenkun add for DAP7
                            keyboardInputState = KeyboardInputState.State_StartWithCtrl;
                            break;
                        case 30:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_A, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 31:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_S, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 32:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_D, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 33:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 34:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_G, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 35:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_H, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 36:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_J, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 37:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_K, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 38:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_L, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 39: // 27(; :)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Oem1, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 40: // 28(' ")
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Oem7, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 42: // Left Shift
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_SHIFT_Left, 0, 0);
                            // PostMessage(hwndMainForm, HotTabECEventArgs.ECEvent_FN, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 43: // 2B(\ |)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Oem5, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 44:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Z, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 45:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_X, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 46:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_C, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 47:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_V, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 48:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_B, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 49:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_N, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 50:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_M, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 51:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_COMMA, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 52:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_PERIOD, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 53: // 35(/ ?)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_OemQuestion, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 54: // Right Shift
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_SHIFT_Right, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 55: // E0 37 (Print Screen)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_PrtScr, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 56: // ALT
                            // if (IsDebugMode) Trace.WriteLine("KeyboardHook : Alt");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_ALT, 0, 0); // kenkun add for DAP7
                            keyboardInputState = KeyboardInputState.State_StartWithAlt;
                            break;
                        case 58:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_CapsLock, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 57:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_SPACE, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 59: // 3B(HEX)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F1, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 60: // 3C
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F2, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 61: // 3D
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F3, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 62: // 3E
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F4, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 63: // 3F
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F5, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 64: // 40
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F6, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 65: // 41
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F7, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 66: // 42
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F8, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 67: // 43
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F9, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 68: // 44
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F10, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 69: // E1 1D 45 / E1 9D C5 
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Pause, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 72: // Up
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Up, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 75: // Left
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Left, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 77: // Right
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Right, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 80: // Down
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Down, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 82: // E0 52(Insert)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Insert, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 83: // E0 53(Delete)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_DELETE, 0, 0); // DAP7
                            // PostMessage(hwndMainForm, HotTabECEventArgs.ECEvent_Delete, 0, 0); // Handheld NB
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 87: // 57
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F11, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 88: // 58
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F12, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 91:
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_MENU, 0, 0);
                            // PostMessage(hwndMainForm, HotTabECEventArgs.ECEvent_OpenMenu, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 93: // E0 5D(Apps)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Apps, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 100: // F13
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F13, 0, 0); // For FM08(FMB80) Use, Winmate kenkun add on 2015/11/25
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 102: // F15
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F15, 0, 0); // For FM08(FMB80) Use, Winmate kenkun add on 2015/11/25
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 115: // 0x73
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_FM08BrightnessAuto, 0, 0); // For FM08(FMB80) Use, Winmate kenkun add on 2015/11/25
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 116: // 0x74 // fn + F10 Brightness Up
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_BrightnessUp, 0, 0);
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_FM08BrightnessDown, 0, 0); // For FM08(FMB80) Use, Winmate kenkun add on 2015/11/25
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 117: // 0x75 // fn + F11 Brightness Down
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_BrightnessDown, 0, 0);
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_FM08BrightnessUp, 0, 0); // For FM08(FMB80) Use, Winmate kenkun add on 2015/11/25
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 174: // fn + F8 Volume Down
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_VolumeDown, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 175: // fn + F7 Volume Up
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_VolumeUp, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        default:
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                    }
                    break;
                #endregion

                #region KeyboardInputState.State_StartWithAlt
                case KeyboardInputState.State_StartWithAlt:
                    switch (e.KeyValue)
                    {
                        case 29: // ctrl
                            // if (IsDebugMode) Trace.WriteLine("KeyboardHook : AltCtrl");
                            // 加了會讓複合鍵直接將CTRL跟ALT覆蓋掉, 不加按了CTRL後直接按ALT也會變複合鍵, 抓不到Keycode
                            // PostMessage(hwndMainForm, HotTabECEventArgs.ECEvent_CTRL, 0, 0); // kenkun add for DAP7
                            // PostMessage(hwndMainForm, HotTabECEventArgs.ECEvent_ALT, 0, 0); // kenkun add for DAP7
                            keyboardInputState = KeyboardInputState.State_StartWithAltCtrl;
                            break;
                        default:
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                    }
                    break;
                #endregion

                #region KeyboardInputState.State_StartWithCtrl
                case KeyboardInputState.State_StartWithCtrl:
                    switch (e.KeyValue)
                    {
                        case 56: // alt
                            // if (IsDebugMode) Trace.WriteLine("KeyboardHook : CtrlAlt");
                            // PostMessage(hwndMainForm, HotTabECEventArgs.ECEvent_CTRL, 0, 0); // kenkun add for DAP7
                            // PostMessage(hwndMainForm, HotTabECEventArgs.ECEvent_ALT, 0, 0); // kenkun add for DAP7
                            keyboardInputState = KeyboardInputState.State_StartWithCtrlAlt;
                            break;
                        default:
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                    }
                    break;
                #endregion

                #region KeyboardInputState.State_StartWithAltCtrl
                case KeyboardInputState.State_StartWithAltCtrl:
                    switch (e.KeyValue)
                    {
                        case 54: // shift
                            // if (IsDebugMode) Trace.WriteLine("KeyboardHook : AltCtrlShift");
                            keyboardInputState = KeyboardInputState.State_StartWithAltCtrlShift;
                            break;
                        default:
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                    }
                    break;
                #endregion

                #region KeyboardInputState.State_StartWithCtrlAlt
                case KeyboardInputState.State_StartWithCtrlAlt:
                    switch (e.KeyValue)
                    {
                        case 54: // shift
                            // if (IsDebugMode) Trace.WriteLine("KeyboardHook : CtrlAltShift");
                            keyboardInputState = KeyboardInputState.State_StartWithCtrlAltShift;
                            break;
                        case 87:// F11
                            // 賓士客製版 Menu key 當 Scan key 用, Keycode : Ctrl + Alt + F11, Winmate Kenkun add on 2015/12/02
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_BARCODE, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        default:
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                    }
                    break;
                #endregion

                #region KeyboardInputState.State_StartWithAltCtrlShift
                case KeyboardInputState.State_StartWithAltCtrlShift:
                    // if (IsDebugMode) Trace.WriteLine("KeyboardInputState KeyValue : " + e.KeyValue); // Winmate add on 2016/05/26
                    switch (e.KeyValue)
                    {
                        case 11: // 0 (PS2 Set:B)
                            // if (IsDebugMode) Trace.WriteLine("KeyboardInputState tempMenu : PASS");
                            // if (IsDebugMode) Trace.WriteLine("KeyboardHook : ECEvent_OpenMenu");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_OpenMenu, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            Trace.WriteLine("11 Hook Menu");
                            break;
                        case 2: // 1
                            // if (IsDebugMode) Trace.WriteLine("KeyboardInputState tempUp : PASS");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_VolumeUp, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 3: // 2
                            // if (IsDebugMode) Trace.WriteLine("KeyboardInputState tempDown : PASS");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_VolumeDown, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 4: // 3
                            // if (IsDebugMode) Trace.WriteLine("KeyboardHook : ECEvent_F1Short");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F1Short, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 5: // 4
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F1Long, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 6: // 5
                            // if (IsDebugMode) Trace.WriteLine("KeyboardHook : ECEvent_F2Short");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F2Short, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 7: // 6
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F2Long, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 8: // 7
                            // if (IsDebugMode) Trace.WriteLine("KeyboardInputState tempF3 : PASS");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F3Short, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 9: // 8 (PS2 Set:8)
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F3Long, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 10: // 9 (PS2 Set:A)
                            // if (IsDebugMode) Trace.WriteLine("KeyboardInputState tempLeft : PASS");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Left, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 18: // e (PS2 Set:12)
                            // if (IsDebugMode) Trace.WriteLine("KeyboardHook : ECEvent_Enter");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Enter, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 23: // kenkun add
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_CAMERA, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 30: // a (PS2 Set:1E)
                            // if (IsDebugMode) Trace.WriteLine("KeyboardInputState tempRight : PASS");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_Right, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 32: // d (PS2 Set:20)
                            if (KEY_MenuToHome.Equals(1)) // For IBCMC Keycode Use, Winmate Kenkun add on 2015/10/06
                            {
                                PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_OpenMenu, 0, 0);
                                Trace.WriteLine("32 Hook Menu");
                            }
                            else if (false/*TestProduct.Equals("IBWH") || TestProduct.Equals("IB10")*/) // For HandHeld 8" / 10", Winmate kenkun modify on 2016/08/16
                            {
                                PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_VolumeUp, 0, 0);
                            }
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 34: // kenkun add
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F5Short, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 37: // kenkun add
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_BARCODE, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 46: // c (PS2 Set:2E)
                            break;
                        case 48: // b (PS2 Set:30)
                            break;
                        case 49: // kenkun add
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_F4Short, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 50: // m (PS2 Set:32)
                            // if (IsDebugMode) Trace.WriteLine("KeyboardHook : ECEvent_HomeKeyEvent");
                            PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_HomeKeyEvent, 0, 0);
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                    }
                    break;
                #endregion

                #region KeyboardInputState.State_StartWithCtrlAltShift
                case KeyboardInputState.State_StartWithCtrlAltShift:
                    switch (e.KeyValue)
                    {
                        case 23: // i
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 24: // o
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        case 18:// e
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                        default:
                            keyboardInputState = KeyboardInputState.State_None;
                            break;
                    }
                    #endregion
                    break;
            }
        }
    }
}
