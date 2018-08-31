using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace rotation
{
    class MonitorControl
    {
        #region Windows API
        [DllImport("user32.dll")]
        static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "ChangeDisplaySettingsEx")]
        static extern int ChangeDisplaySettingsEx2(object lpszDeviceName, object lpDevMode, object hwnd, uint dwflags, object lParam);

        [DllImport("user32.dll", EntryPoint = "EnumDisplaySettings")]
        static extern bool EnumDisplaySettings(string lpszDeviceName, Int32 iModeNum, ref DEVMODE lpDevMode);

        [DllImport("user32.dll")]
        static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [DllImport("user32.dll")]
        static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetVersionEx(ref OSVERSIONFOEX lpVersionInfo);


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hFile);



        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFile(
                                           [MarshalAs(UnmanagedType.LPStr)]
                                            string strName,
                                            uint nAccess,
                                            uint nShareMode,
                                            IntPtr lpSecurity,
                                            uint nCreationFlags,
                                            uint nAttributes,
                                            IntPtr lpTemplate
                                        );

        [DllImport("kernel32.dll", SetLastError = true)] // Marshal.GetLastWin32Error().ToString();
        static extern uint GetLastError();



        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(
                                    IntPtr hDevice,
                                    uint dwIoControlCode,
                                    byte[] lpInBuffer,
                                    uint nInBufferSize,
                                    [Out] byte[] lpOutBuffer,
                                    uint nOutBufferSize,
                                    out uint lpBytesReturned,
                                    IntPtr lpOverlapped
                                );

        #endregion

        #region Windows Structure
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct OSVERSIONFOEX
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public short wServicePackMajor;
            public short wServicePackMinor;
            public short wSuitMask;
            public byte wProductType;
            public byte wReserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DISPLAY_BRIGHTNESS
        {
            public byte DisplayPolicy;
            public byte ACBRightness;
            public byte DCBrightness;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct OVERLAPPED
        {
            public UIntPtr Internal;
            public UIntPtr InternalHigh;
            public uint Offset;
            public uint OffsetHigh;
            public IntPtr EventHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;

            public POINTL dmPosition;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;

            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;    // Declared wrong in the full framework
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;

            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTL
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DISPLAY_DEVICE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cb;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;

            [MarshalAs(UnmanagedType.U4)]
            public uint StateFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        #region DISP_CHANGE

        const long DISP_CHANGE_SUCCESSFUL = 0;
        const long DISP_CHANGE_RESTART = 1;
        const long DISP_CHANGE_FAILED = -1;
        const long DISP_CHANGE_BADMODE = -2;
        const long DISP_CHANGE_NOTUPDATED = -3;
        const long DISP_CHANGE_BADFLAGS = -4;
        const long DISP_CHANGE_BADPARAM = -5;
        const long DISP_CHANGE_BADDUALVIEW = -6;


        #endregion

        #region DM

        const int DM_ORIENTATION = 0x00000001;
        const int DM_PAPERSIZE = 0x00000002;
        const int DM_PAPERLENGTH = 0x00000004;
        const int DM_PAPERWIDTH = 0x00000008;
        const int DM_SCALE = 0x00000010;
        const int DM_POSITION = 0x00000020;
        const int DM_NUP = 0x00000040;
        const int DM_DISPLAYORIENTATION = 0x00000080;
        const int DM_COPIES = 0x00000100;
        const int DM_DEFAULTSOURCE = 0x00000200;
        const int DM_PRINTQUALITY = 0x00000400;
        const int DM_COLOR = 0x00000800;
        const int DM_DUPLEX = 0x00001000;
        const int DM_YRESOLUTION = 0x00002000;
        const int DM_TTOPTION = 0x00004000;
        const int DM_COLLATE = 0x00008000;
        const int DM_FORMNAME = 0x00010000;
        const int DM_LOGPIXELS = 0x00020000;
        const int DM_BITSPERPEL = 0x00040000;
        const int DM_PELSWIDTH = 0x00080000;
        const int DM_PELSHEIGHT = 0x00100000;
        const int DM_DISPLAYFLAGS = 0x00200000;
        const int DM_DISPLAYFREQUENCY = 0x00400000;
        const int DM_ICMMETHOD = 0x00800000;
        const int DM_ICMINTENT = 0x01000000;
        const int DM_MEDIATYPE = 0x02000000;
        const int DM_DITHERTYPE = 0x04000000;
        const int DM_PANNINGWIDTH = 0x08000000;
        const int DM_PANNINGHEIGHT = 0x10000000;
        const int DM_DISPLAYFIXEDOUTPUT = 0x20000000;

        #endregion

        #endregion

        #region Windows Constant

        const uint FILE_SHARE_WRITE = 0x2;
        /// <summary>CreateFile : file share for read</summary>
        const uint FILE_SHARE_READ = 0x1;
        /// <summary>CreateFile : Open handle for overlapped operations</summary>
        const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        const uint GENERIC_READ = 0x80000000;
        /// <summary>CreateFile : Open file for write</summary>
        const uint GENERIC_WRITE = 0x40000000;
        /// <summary>CreateFile : Resource to be "created" must exist</summary>
        const uint OPEN_EXISTING = 3;
        const int CCHDEVICENAME = 32;
        const int CCHFORMNAME = 32;
        const uint CDS_UPDATEREGISTRY = 0x00000001;
        const uint CDS_TEST = 0x00000002;
        const uint CDS_SET_PRIMARY = 0x00000010;
        const uint CDS_NORESET = 0x10000000;
        const uint CDS_GLOBAL = 0x00000008;
        const uint CDS_RESET = 0x40000000;

        const int ENUM_CURRENT_SETTINGS = -1;

        const short DMDUP_HORIZONTAL = 3;
        const short DMDUP_SIMPLEX = 1;
        const short DMDUP_VERTICAL = 2;

        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MONITORPOWER = 0xF170;

        const uint FILE_DEVICE_FILE_SYSTEM = 0x00000009;
        const uint FILE_DEVICE_VIDEO = 0x00000023;
        const uint FILE_ANY_ACCESS = 0;
        const uint FILE_SPECIAL_ACCESS = FILE_ANY_ACCESS;
        const uint METHOD_BUFFERED = 0;
        const uint METHOD_NEITHER = 3;

        static uint IOCTL_VIDEO_QUERY_SUPPORTED_BRIGHTNESS =
                            CTL_CODE(FILE_DEVICE_VIDEO, 293, METHOD_BUFFERED, FILE_ANY_ACCESS);

        static uint IOCTL_VIDEO_QUERY_DISPLAY_BRIGHTNESS =
                            CTL_CODE(FILE_DEVICE_VIDEO, 294, METHOD_BUFFERED, FILE_ANY_ACCESS);

        static uint IOCTL_VIDEO_SET_DISPLAY_BRIGHTNESS =
                            CTL_CODE(FILE_DEVICE_VIDEO, 295, METHOD_BUFFERED, FILE_ANY_ACCESS);

        static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        { return ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method); }
        #endregion

        #region Monitor Control Enum
        enum DisplayMode : int
        {
            ExtendedDisplay = 2,
            SingleDisplay = 1
        }
        public enum RotationDisplay : int
        {
            DMDO_DEFAULT = 0,
            DMDO_90 = 1,
            DMDO_180 = 2,
            DMDO_270 = 3
        }
        enum MonitorStateEnum : int
        {
            MONITOR_ON = -1,    // the display is powering on
            MONITOR_OFF = 2,    // the display is being shut off
            MONITOR_STANBY = 1  // the display is going to low power
        }
        #endregion

        #region Monitor Control Variable
        IntPtr hDevice;
        byte[] BacklightIndex;
        DISPLAY_BRIGHTNESS NowBrightness;

        private string DeviceList;
        private string BrightnessList;
        // public string[] DeviceArray;

        public int BrightnessLength;
        public byte[] BrightnessListArray = new byte[101];

        public byte Level0;
        public byte Level1;
        public byte Level2;
        public byte Level3;
        public byte Level4;
        public byte Level5;
        public byte Level6;
        public byte Level7;
        public byte Level8;
        public byte Level9;
        public byte Level10;
        #endregion

        public static uint bDebug = 0;
        bool IsDebugMode = true;

        public void DebugMessage(string token, string msg)
        {
            if (bDebug == 1)
            {
                if (IsDebugMode) Trace.WriteLine(token + " ==> " + msg);
            }
        }

        public bool MonitorControl_Load()
        {
            if (IsDebugMode) Trace.WriteLine("MonitorControlLoad()");// Winmate Kenkun comment on 2014/12/08

            uint DisplayIndex = 0;

            DISPLAY_DEVICE DisplayDevice;
            OSVERSIONFOEX OSVer;

            while (GetDisplayDevice(DisplayIndex, out DisplayDevice))
            {
                // if (DisplayDevice.DeviceID != "") { DisplayList.Items.Add(DisplayDevice.DeviceString); }
                if (DisplayDevice.DeviceID != "") DeviceList = String.Concat(DeviceList, DisplayDevice.DeviceString, ";");
                DisplayIndex++;
            }

            if (DisplayIndex == 0) return false;
            // if (IsDebugMode) Trace.WriteLine("MonitorControl_Load false DisplayIndex");// Winmate Kenkun comment on 2014/12/08

            DisplayIndex = 0;

            DeviceList = DeviceList.Remove(DeviceList.Length - 1, 1);// brian
            UpdateDeviceTable(DeviceList);// brian

            GetWindowsVersion(out OSVer);

            if (!InitalBraklightFunction(OSVer.dwMajorVersion, DisplayIndex)) return false;
            // if (IsDebugMode) Trace.WriteLine("MonitorControl_Load InitalBraklightFunction");// Winmate Kenkun comment on 2014/12/08

            // if (IsDebugMode) Trace.WriteLine("MonitorControl_Load() End");// Winmate Kenkun comment on 2014/12/08
            return true;
        }


        private bool GetWindowsVersion(out OSVERSIONFOEX OSVersion)
        {
            if (IsDebugMode) Trace.WriteLine("Get Windows Version");
            OSVersion = new OSVERSIONFOEX();
            OSVersion.dwOSVersionInfoSize = Marshal.SizeOf(OSVersion);
            bool Ret = GetVersionEx(ref OSVersion);
            return Ret;
        }

        private IntPtr OpenLCDDevice()
        {
            IntPtr hDevice;
            hDevice = CreateFile(
                            "\\\\.\\LCD",                           // open LCD device   @\\.\LCD or \\\\.\LCD                      
                            GENERIC_READ | GENERIC_WRITE,           // no access to the drive
                                                                    // FILE_SHARE_READ | FILE_SHARE_WRITE,     // share mode
                            FILE_ANY_ACCESS,
                            IntPtr.Zero,                            // default security attributes
                            OPEN_EXISTING,                          // disposition
                            0,                                      // file attributes                         
                            IntPtr.Zero
                        );
            return hDevice;
        }

        private bool QueryDisplayState(IntPtr hDevice, out byte[] StateBuffer, out uint lpByte)
        {
            bool Ret;
            lpByte = 0;
            StateBuffer = new byte[512];
            Ret = DeviceIoControl(hDevice, IOCTL_VIDEO_QUERY_SUPPORTED_BRIGHTNESS, null, 0, StateBuffer, (uint)sizeof(byte) * 512, out lpByte, IntPtr.Zero);

            return Ret;
        }

        private static bool GetDisplayDevice(uint DisplayIndex, out DISPLAY_DEVICE DisplayDevice)
        {
            bool Ret;
            DisplayDevice = new DISPLAY_DEVICE();
            DisplayDevice.cb = Marshal.SizeOf(DisplayDevice);
            Ret = EnumDisplayDevices(null, DisplayIndex, ref DisplayDevice, 0);
            return Ret;
        }

        private bool InitalBraklightFunction(int oSVer, uint DisplayIndex)
        {
            DebugMessage("winmate", "InitalBraklightFunction");

            bool Ret;
            int BacklightPercentage = 0;
            uint lpByte;

            if ((int)(hDevice = OpenLCDDevice()) == -1)
            {
                // ShowDialogMessageBox("找不到LCD介面!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DebugMessage("winmate", "LCD not found");
                return false;
            }
            else
            {
                if (oSVer == 5)     // Windows XP 
                {
                    NowBrightness = new DISPLAY_BRIGHTNESS();
                    Ret = GetDisplayBrightness(hDevice, ref NowBrightness);
                    if (Ret == false)
                    {
                        // ShowDialogMessageBox("Error Code:" + Marshal.GetLastWin32Error().ToString(), "Get Display Brightness!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (NowBrightness.DisplayPolicy == 0x01) BacklightPercentage = NowBrightness.ACBRightness;
                    else BacklightPercentage = NowBrightness.DCBrightness;

                    DebugMessage("winmate", "xp");
                    return false;
                }
                else if (oSVer >= 6) // Windows Vista 以上
                {
                    ManagementClass mc = new ManagementClass(string.Format(@"\\{0}\ROOT\wmi:WmiMonitorBrightness", Environment.MachineName));
                    foreach (ManagementObject mo in mc.GetInstances())
                    {
                        PropertyDataCollection property = mo.Properties;
                        BacklightPercentage = (byte)property["CurrentBrightness"].Value;
                    }
                    mc.Dispose();
                }

                Ret = QueryDisplayState(hDevice, out BacklightIndex, out lpByte);
                if (Ret == false)
                {
                    // ShowDialogMessageBox("Error Code:" + Marshal.GetLastWin32Error().ToString(), "Query Display State!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    for (int i = 0; i < lpByte; i++)
                    {
                        if (BacklightIndex[i] == BacklightPercentage) { DisplayIndex = (uint)i; }
                        // this.BacklightLevel.Items.Add("Level " + (i + 1).ToString() + ":" + BacklightIndex[i].ToString());
                        BrightnessList = String.Concat(BrightnessList, BacklightIndex[i].ToString(), ";");
                    }

                    // BrightnessList = "0;10;20;30;40;50;60;70;80;90;100;";

                    if (BrightnessList == null)
                    {
                        DebugMessage("winmate", "BrightnessList 0");
                        return false;
                    }
                    DebugMessage("winmate", "BrightnessList = " + BrightnessList);
                    BrightnessList = BrightnessList.Remove(BrightnessList.Length - 1, 1);
                    UpdateBrightnessTable(BrightnessList);

                    if (lpByte > 0)
                    {
                        // BacklightLevel.SelectedIndex = (int)DisplayIndex;
                        // this.BacklightLevel.Enabled = true;
                        // this.Backlight_Setting.Enabled = true;
                    }
                }
            }

            DebugMessage("winmate", "InitalBraklightFunction end");
            return true;
        }
        private bool UpdateBrightnessTable(string BrightnessTableStirng)// brian
        {
            // ShowDialogMessageBox(BrightnessTableStirng);

            string[] buf;
            int i, len;

            buf = BrightnessTableStirng.Split(';');
            len = buf.Length;
            BrightnessLength = len;

            if (BrightnessLength < 9)
                return false;

            byte[] BrightnessArray = new byte[len];

            // ShowDialogMessageBox("Len:" + len);

            for (i = 0; i < len; i++)
            {
                BrightnessArray[i] = Convert.ToByte(buf[i]);
                // ShowDialogMessageBox(i.ToString() + ":" + BrightnessArray[i]);
            }

            for (i = 0; i < len; i++)
            {
                BrightnessListArray[i] = BrightnessArray[i];
            }

            if (len < 12)
            {

                if (len >= 11)
                    Level10 = BrightnessArray[10];

                if (len >= 10)
                    Level9 = BrightnessArray[9];

                if (len >= 9)
                    Level8 = BrightnessArray[8];

                if (len >= 8)
                    Level7 = BrightnessArray[7];

                if (len >= 7)
                    Level6 = BrightnessArray[6];

                if (len >= 6)
                    Level5 = BrightnessArray[5];

                if (len >= 5)
                    Level4 = BrightnessArray[4];

                if (len >= 4)
                    Level3 = BrightnessArray[3];

                if (len >= 3)
                    Level2 = BrightnessArray[2];

                if (len >= 2)
                    Level1 = BrightnessArray[1];

                if (len >= 1)
                    Level0 = BrightnessArray[0];
            }
            else
            {
                len = 11;
                BrightnessLength = len;
                BrightnessListArray[0] = 0;
                BrightnessListArray[1] = 10;
                BrightnessListArray[2] = 20;
                BrightnessListArray[3] = 30;
                BrightnessListArray[4] = 40;
                BrightnessListArray[5] = 50;
                BrightnessListArray[6] = 60;
                BrightnessListArray[7] = 70;
                BrightnessListArray[8] = 80;
                BrightnessListArray[9] = 90;
                BrightnessListArray[10] = 100;
            }

            return true;
        }
        private bool UpdateDeviceTable(string DeviceStirng)// brian
        {
            // ShowDialogMessageBox(DeviceStirng);
            string[] buf;
            int i, len;

            buf = DeviceStirng.Split(';');
            len = buf.Length;

            string[] DeviceArray = new string[len];

            for (i = 0; i < len; i++) DeviceArray[i] = buf[i];

            return true;
        }

        private bool GetDisplayBrightness(IntPtr hDevice, ref DISPLAY_BRIGHTNESS DB)
        {
            if (IsDebugMode) Trace.WriteLine("GetDisplayBrightness");
            IntPtr DBBuffer = IntPtr.Zero;
            uint lpByte;
            byte[] DataBuffer = new byte[4];
            bool Ret = DeviceIoControl(hDevice, IOCTL_VIDEO_QUERY_DISPLAY_BRIGHTNESS, null, 0, DataBuffer, (uint)sizeof(byte) * 3, out lpByte, IntPtr.Zero);

            DB.DisplayPolicy = DataBuffer[0];
            DB.ACBRightness = DataBuffer[1];
            DB.DCBrightness = DataBuffer[2];
            return Ret;
        }

        public static bool RotationDisplayMode(uint RotationMonitor, RotationDisplay RotationMode, bool ShowMessage)
        {

            DISPLAY_DEVICE DisplayDevice;
            DEVMODE RotationDM = new DEVMODE();
            if (!GetDisplayDevice(RotationMonitor, out DisplayDevice)) { return false; }

            RotationDM.dmSize = (short)Marshal.SizeOf(RotationDM);
            if (EnumDisplaySettings(DisplayDevice.DeviceName, ENUM_CURRENT_SETTINGS, ref RotationDM))
            {
                int temp;
                RotationDM.dmDisplayOrientation = (int)RotationMode;
                if (RotationMode == RotationDisplay.DMDO_DEFAULT || RotationMode == RotationDisplay.DMDO_180)
                {
                    if (RotationDM.dmPelsHeight > RotationDM.dmPelsWidth)
                    {
                        temp = RotationDM.dmPelsHeight;
                        RotationDM.dmPelsHeight = RotationDM.dmPelsWidth;
                        RotationDM.dmPelsWidth = temp;
                    }
                }
                else if (RotationMode == RotationDisplay.DMDO_90 || RotationMode == RotationDisplay.DMDO_270)
                {
                    if (RotationDM.dmPelsHeight < RotationDM.dmPelsWidth)
                    {
                        temp = RotationDM.dmPelsHeight;
                        RotationDM.dmPelsHeight = RotationDM.dmPelsWidth;
                        RotationDM.dmPelsWidth = temp;
                    }
                }

                RotationDM.dmFields |= DM_DISPLAYORIENTATION;
                int Return_Value;
                if (DISP_CHANGE_SUCCESSFUL == (Return_Value = ChangeDisplaySettingsEx(DisplayDevice.DeviceName, ref RotationDM, IntPtr.Zero, CDS_UPDATEREGISTRY | CDS_RESET, IntPtr.Zero)))
                {
                    if (ShowMessage == true)
                    {
                        if (DialogResult.Cancel == MessageBox.Show("Do you keep this state?", "Change Display orientation", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
                        {
                            RotationDM.dmDisplayOrientation = 0;
                            if (RotationDM.dmPelsHeight > RotationDM.dmPelsWidth)
                            {
                                temp = RotationDM.dmPelsHeight;
                                RotationDM.dmPelsHeight = RotationDM.dmPelsWidth;
                                RotationDM.dmPelsWidth = temp;
                            }
                            ChangeDisplaySettingsEx(DisplayDevice.DeviceName, ref RotationDM, IntPtr.Zero, CDS_UPDATEREGISTRY | CDS_RESET, IntPtr.Zero);
                        }
                    }
                    return true;
                }
                else
                {
                    // ShowChangeDisplayError(Return_Value);
                }
            }
            return false;
        }

    }
}
