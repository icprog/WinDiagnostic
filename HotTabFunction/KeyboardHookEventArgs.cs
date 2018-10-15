using System;
using System.Collections.Generic;
using System.Text;

namespace HotTabFunction
{
    public class KeyboardHookEventArgs : EventArgs
    {
        #region Const Event Number Declare

        public const int WM_APP = 0x8000;
        public const int ECEvent_OpenMenu = WM_APP + 0x10;
        public const int ECEvent_CloseAllForm = WM_APP + 0x11;
        public const int ECEvent_Brightness2 = WM_APP + 0x12;
        public const int ECEvent_Brightness3 = WM_APP + 0x13;
        public const int ECEvent_Brightness4 = WM_APP + 0x14;
        public const int ECEvent_Brightness5 = WM_APP + 0x15;
        public const int ECEvent_Brightness6 = WM_APP + 0x16;
        public const int ECEvent_Brightness7 = WM_APP + 0x17;
        public const int ECEvent_Brightness8 = WM_APP + 0x18;
        public const int ECEvent_APUnlock = WM_APP + 0x19;
        public const int ECEvent_ShowVolume = WM_APP + 0x20;
        public const int ECEvent_WLOnBTOn = WM_APP + 0x21;
        public const int ECEvent_WLOnBTOff = WM_APP + 0x22;
        public const int ECEvent_WLOffBTOn = WM_APP + 0x23;
        public const int ECEvent_WLOffBTOff = WM_APP + 0x24;
        public const int ECEvent_VolumeUp = WM_APP + 0x25;
        public const int ECEvent_VolumeDown = WM_APP + 0x26;
        public const int ECEvent_Up = WM_APP + 0x28;
        public const int ECEvent_Down = WM_APP + 0x29;
        public const int ECEvent_Left = WM_APP + 0x2a;
        public const int ECEvent_Right = WM_APP + 0x2b;
        public const int ECEvent_Enter = WM_APP + 0x2c;
        public const int ECEvent_HomeKeyEvent = WM_APP + 0x2f;
        public const int ECEvent_F1Short = WM_APP + 0x31;
        public const int ECEvent_F1Long = WM_APP + 0x32;
        public const int ECEvent_F2Short = WM_APP + 0x33;
        public const int ECEvent_F2Long = WM_APP + 0x34;
        public const int ECEvent_F3Short = WM_APP + 0x35;
        public const int ECEvent_F3Long = WM_APP + 0x36;
        public const int ECEvent_S3 = WM_APP + 0x37;
        public const int ECEvent_BatteryLow = WM_APP + 0x38;
        public const int ECEvent_BatteryWithWarmSwap = WM_APP + 0x39;
        public const int ECEvent_SmallBatteryLow = WM_APP + 0x3a;
        public const int ECEvent_ShowBattery = WM_APP + 0x3b;
        public const int ECEvent_HotSwapSupport = WM_APP + 0x3c;
        public const int ECEvent_SwitchSecondBattery = WM_APP + 0x3d;
        public const int ECEvent_SwitchBatteryError = WM_APP + 0x3e;
        public const int ECEvent_FirmwareVersion = WM_APP + 0x3f;
        public const int ECEvent_CloseMenu = WM_APP + 0x40;
        public const int ECEvent_MenuLock = WM_APP + 0x41;
        public const int ECEvent_MenuUnlock = WM_APP + 0x42;
        public const int ECEvent_BatteryCurrentOverload = WM_APP + 0x43;
        public const int ECEvent_RotationLock = WM_APP + 0x44;// brian add
        public const int ECEvent_FM08BrightnessAuto = WM_APP + 0x45;
        public const int ECEvent_FM08BrightnessUp = WM_APP + 0x46;
        public const int ECEvent_FM08BrightnessDown = WM_APP + 0x47;

        // Kenkun add on 2013/12/20 for DAP7 Keyboard
        public const int ECEvent_Q = WM_APP + 0x48;
        public const int ECEvent_W = WM_APP + 0x49;
        public const int ECEvent_E = WM_APP + 0x4a;
        public const int ECEvent_R = WM_APP + 0x4b;
        public const int ECEvent_T = WM_APP + 0x4c;
        public const int ECEvent_Y = WM_APP + 0x4d;
        public const int ECEvent_U = WM_APP + 0x4e;
        public const int ECEvent_I = WM_APP + 0x4f;
        public const int ECEvent_O = WM_APP + 0x50;
        public const int ECEvent_P = WM_APP + 0x51;
        public const int ECEvent_A = WM_APP + 0x52;
        public const int ECEvent_S = WM_APP + 0x53;
        public const int ECEvent_D = WM_APP + 0x54;
        public const int ECEvent_F = WM_APP + 0x55;
        public const int ECEvent_G = WM_APP + 0x56;
        public const int ECEvent_H = WM_APP + 0x57;
        public const int ECEvent_J = WM_APP + 0x58;
        public const int ECEvent_K = WM_APP + 0x59;
        public const int ECEvent_L = WM_APP + 0x5a;
        public const int ECEvent_Z = WM_APP + 0x5b;
        public const int ECEvent_X = WM_APP + 0x5c;
        public const int ECEvent_C = WM_APP + 0x5d;
        public const int ECEvent_V = WM_APP + 0x5e;
        public const int ECEvent_B = WM_APP + 0x5f;
        public const int ECEvent_N = WM_APP + 0x60;
        public const int ECEvent_M = WM_APP + 0x61;
        public const int ECEvent_BACKSPACE = WM_APP + 0x62;
        public const int ECEvent_COMMA = WM_APP + 0x63;
        public const int ECEvent_PERIOD = WM_APP + 0x64;
        public const int ECEvent_FN = WM_APP + 0x65;
        public const int ECEvent_0 = WM_APP + 0x66;
        public const int ECEvent_1 = WM_APP + 0x67;
        public const int ECEvent_2 = WM_APP + 0x68;
        public const int ECEvent_3 = WM_APP + 0x69;
        public const int ECEvent_4 = WM_APP + 0x6a;
        public const int ECEvent_5 = WM_APP + 0x6b;
        public const int ECEvent_6 = WM_APP + 0x6c;
        public const int ECEvent_7 = WM_APP + 0x6d;
        public const int ECEvent_8 = WM_APP + 0x6e;
        public const int ECEvent_9 = WM_APP + 0x6f;
        public const int ECEvent_F4Short = WM_APP + 0x73;
        public const int ECEvent_F5Short = WM_APP + 0x74;
        public const int ECEvent_CAMERA = WM_APP + 0x75;
        public const int ECEvent_BARCODE = WM_APP + 0x76;
        public const int ECEvent_MENU = WM_APP + 0x77; // 有跟ECEvent_OpenMenu重複的可能性
        public const int ECEvent_ESC = WM_APP + 0x78;
        public const int ECEvent_TAB = WM_APP + 0x79;
        public const int ECEvent_SHIFT_Left = WM_APP + 0x7a;
        public const int ECEvent_SHIFT_Right = WM_APP + 0x9E;
        public const int ECEvent_DELETE = WM_APP + 0x7b;
        // public const int ECEvent_ENTER = WM_APP + 0x7c;
        public const int ECEvent_SPACE = WM_APP + 0x7d;
        public const int ECEvent_CTRL = WM_APP + 0x7e;
        public const int ECEvent_ALT = WM_APP + 0x7f;
        // Kenkun add on 2013/12/20 End
        // Kenkun add on 2014/04/25 for Handheld NB
        public const int ECEvent_F1 = WM_APP + 0x80;
        public const int ECEvent_F2 = WM_APP + 0x81;
        public const int ECEvent_F3 = WM_APP + 0x82;
        public const int ECEvent_F4 = WM_APP + 0x83;
        public const int ECEvent_F5 = WM_APP + 0x84;
        public const int ECEvent_F6 = WM_APP + 0x85;
        public const int ECEvent_F7 = WM_APP + 0x86;
        public const int ECEvent_F8 = WM_APP + 0x87;
        public const int ECEvent_F9 = WM_APP + 0x88;
        public const int ECEvent_F10 = WM_APP + 0x89;
        public const int ECEvent_F11 = WM_APP + 0x8a;
        public const int ECEvent_F12 = WM_APP + 0x8b;
        public const int ECEvent_F13 = WM_APP + 0x9B;
        public const int ECEvent_F15 = WM_APP + 0x9C;
        public const int ECEvent_OemMinus = WM_APP + 0x8c; // - _
        public const int ECEvent_OemPlus = WM_APP + 0x8d; // = +
        public const int ECEvent_OemOpenBrackets = WM_APP + 0x8e; // [ {
        public const int ECEvent_Oem6 = WM_APP + 0x8f; // ] }
        public const int ECEvent_Oem5 = WM_APP + 0x90; // \ |
        public const int ECEvent_Oem1 = WM_APP + 0x91; // ; :
        public const int ECEvent_Oem7 = WM_APP + 0x92; // ' "
        public const int ECEvent_OemQuestion = WM_APP + 0x93; //
        public const int ECEvent_Pause = WM_APP + 0x94;
        public const int ECEvent_PrtScr = WM_APP + 0x95;
        public const int ECEvent_Insert = WM_APP + 0x96;
        // public const int ECEvent_Delete = WM_APP + 0x97;
        public const int ECEvent_Apps = WM_APP + 0x98;
        public const int ECEvent_BrightnessUp = WM_APP + 0x99;
        public const int ECEvent_BrightnessDown = WM_APP + 0x9A;
        // Kenkun add on 2014/04/25 for Handheld NB End
        public const int ECEvent_CapsLock = WM_APP + 0x9D; // M101BK
        #endregion
    }
}
