using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace gps
{
    enum GSensor_Orientation : int
    {
        Orientation_Left = 0,
        Orientation_Right,
        Orientation_Up,
        Orientation_Down,
        Orientation_Top,
        Orientation_Bottom
    }

    class HotTabDLL
    {
        #region Const&Enumeration Declare

        // Resolution
        public const int DM_DISPLAYFREQUENCY = 0x400000;
        public const int DM_PELSWIDTH = 0x80000;
        public const int DM_PELSHEIGHT = 0x100000;

        public static int[] Resolution_Width = new int[] { 640, 800, 800, 1024, 1024, 1280, 1920 };
        public static int[] Resolution_Height = new int[] { 480, 480, 600, 600, 768, 1024, 1080 };

        #endregion

        #region Struct Declare

        // Resolution
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            public short dmLogPixels;
            public short dmBitsPerPel;
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
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public uint DataLength;
            public uint Data;
        };


        #endregion

        #region Winmate WinIO DLL Declare
        [DllImport(@"WMIO2.dll")]
        public static extern bool ModeOpen(uint uiMode);

        [DllImport(@"WMIO2.dll")]
        public static extern bool SetDevice(byte uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool SetDevice2(byte uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetDevice(out byte uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetDevice2(out byte uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool SetAutoScreenRotationLock(uint uiMode);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetAccelerometer(out double uiXValue, out double uiYValue, out double uiZValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery1Percentage(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery2Percentage(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery1Current(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery2Current(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery1FullChargeCapacity(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery1RemainingCapacity(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery2FullChargeCapacity(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery2RemainingCapacity(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery1ChargingVoltage(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery2ChargingVoltage(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery1BatteryStatus(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery2BatteryStatus(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery1Temperature(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery2Temperature(out int uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool WinIO_ReadFromECSpace(uint uiAddress, out uint uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool WinIO_WriteToECSpace(uint uiAddress, uint uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool WinIO_WriteCommand(uint uiCommand, uint bValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool WinIO_ReadCommand(uint uiCommand, out uint uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetACStatus(out uint uiValue);

        // CycleCount       0x17
        // DesignCapacity   0x18 
        // ManufactureDate  0x1b 
        // ManufacturerName 0x20
        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery1SpecificInfo(uint uiCommand, out uint uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetBattery2SpecificInfo(uint uiCommand, out uint uiValue);
        #endregion

        [DllImport("FangtecRotateDLL.dll")]
        public static extern uint FangtecRotate090();

        #region Attribute Declare

        public static String OSName = "VISTA";
        public static int VIAScreenWidth = 0;
        public static int VIAScreenHeight = 0;
        public static uint GSensorOrientation = 0;

        public static bool bIsScreenRotating = false;
        public static bool bIsShowBrightness;
        private static uint uiECBrightness;
        private static uint uiPMBrightness;
        private static int iSkipNotifyECBrightness;
        private static int iSkipNotifyPMBrightness;

        public static bool bIsReadingEC = false;
        public static bool bIsBrightnessReadingEC = false;

        static bool IsDebugMode = true;
        #endregion

        #region User32 DLL Declare For All

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern Int32 PostMessage(Int32 hWnd, Int32 wMsg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(int lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE lpDevMode, int dwflags);

        #endregion

        #region Kernel32 DLL Declare For All

        [DllImport("kernel32.dll")]
        public static extern void Sleep(int dwMilliseconds);

        #endregion

        #region WinIO DLL Declare For Windows Vista

        #region G-Sensor (WINDLLEX1.DLL)
        [DllImport(@"WINDLLEX1.dll")]
        public static extern bool GetAngleDirection(out int pdwPortVal);
        #endregion

        #endregion

        #region Volume DLL Declare For Windows Vista

        [DllImport("HotTabDLLV6.dll")]
        private static extern bool VolumeSetV(int iVolume);

        [DllImport("HotTabDLLV6.dll")]
        private static extern bool VolumeGetV(ref int iVolume);

        [DllImport("HotTabDLLV6.dll")]
        private static extern bool VolumeGetMuteV(ref bool iValue);

        [DllImport("HotTabDLLV6.dll")]
        private static extern bool VolumeSetMuteV(bool iValue);

        #endregion

        #region PowerManager DLL Declare For Windows Vista

        [DllImport("HotTabDLLV6.dll")]
        public static extern bool IsDCCharging();

        [DllImport("HotTabDLLV6.dll")]
        public static extern bool BatteryLifePrecentGet(ref int iPercent);

        [DllImport("HotTabDLLV6.dll")]
        public static extern bool BatteryLifeTimeGet(ref int iTime);

        [DllImport("HotTabDLLV6.dll")]
        private static extern bool SetHotTabSchemaTurnOffACPanelV(uint ulTimeoutTurnOffPanel);

        [DllImport("HotTabDLLV6.dll")]
        private static extern bool SetHotTabSchemaTurnOffBATPanelV(uint ulTimeoutTurnOffPanel);

        [DllImport("HotTabDLLV6.dll")]
        private static extern bool RegisterDisplayBrightnessEventV(IntPtr handle);

        [DllImport("HotTabDLLV6.dll")]
        private static extern bool UnregisterDisplayBrightnessEventV();

        [DllImport("HotTabDLLV6.dll")]
        private static extern bool SetDisplayBrightnessV(uint dwBrightness);

        [DllImport("HotTabDLLV6.dll")]
        private static extern bool SetDisplayBrightnessValue(uint dwBrightness, uint iMode);

        [DllImport("HotTabDLLV6.dll")]
        private static extern bool SetDisplayBrightnessRangeV();

        #endregion

        #region Winmate MCU DLL Declare by oliver
        [DllImport(@"DaimlerMCU.dll", EntryPoint = "MCUConnect")]
        public static extern int DeviceConnect(int baud, int port);

        [DllImport(@"DaimlerMCU.dll", EntryPoint = "MCUDisconnect")]
        public static extern int DeviceDisconnect(int port);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Get_BIOS_BOM(byte[] version);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Get_BIOS_Name(byte[] version);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Set_LoadDefault();

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Set_EventClear();

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Get_Event(out byte OutStatus);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Set_DeviceWake(byte type, bool bEnable);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Get_DeviceWake(byte type, out byte OutStatus);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Set_DeviceClear(byte type);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Get_DeviceCount(byte type, out byte OutCount);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Get_DeviceTimer(byte type, uint[] OutTime);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Get_DeviceTable(byte type, out byte OutCount, uint[] OutTime);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Get_IRTable(byte type, out byte OutCount, uint[] OutTime, out byte OutId1, out byte OutId2);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int Get_IRCode(byte type, out byte OutId1, out byte OutId2);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int rtcSetDateAndTimeSync();

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int rtcReadDateAndTime(uint[] OutTime);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int rtcSetAlarmTime(byte month, byte day, byte hour, byte minute, byte second, byte repeatMode);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int rtcReadAlarmTime(uint[] OutTime);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int gsensorInit();

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int gsensorSetSensitivity(byte value);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int gsensorReadSensitivity(out byte value);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int ledSetStatus(byte device, byte color, byte type, byte level);

        [DllImport(@"DaimlerMCU.dll")]
        public static extern int ledSetBlinkMode(byte mode);

        public enum MB_RET
        {
            RET_SUCCESS = 0,
            RET_ERROR_CODE = -6,
            RET_UNKNOWN = -8
        };

        public enum DEVICE_WAKE_TYPE
        {
            RTC = 0x00,
            GSENSOR = 0x01,
            IR = 0x02,
            DOCKING_IN = 0x03,
            DOCKING_OUT = 0x04,
            BUTTON = 0x05,
            OBD_IN = 0x06,
            OBD_OUT = 0x07
        }

        public enum DEVICE_CONTROL_TYPE
        {
            RTC = 0x00,
            GSENSOR = 0x01,
            IR1 = 0x12,
            IR2 = 0x22,
            IR3 = 0x32,
            DOCKING_IN = 0x03,
            DOCKING_OUT = 0x04,
            BUTTON = 0x05,
            OBD_IN = 0x06,
            OBD_OUT = 0x07
        }
        #endregion

        #region WinIO Function For All
        // public static FormDebug formDebug = new FormDebug();


        #region Battery - BTZ1 / IH83 / M9020B 專用 ( 雙MBAT )
        public static bool GetBatteryCurrentDischarge(out byte bValue)
        {
            // 判斷當前放電的電池是哪顆 0x02 = 外部  0x01 = 內部
            uint data = 0;
            bool bResult = false;
            int iCount = 0;

            do
            {
                bResult = WinIO_ReadCommand(0x15, out data);
                if (bResult)
                    break;
                ++iCount;
                System.Threading.Thread.Sleep(100);
            } while (bResult == false && iCount < 4);

            if (bResult)
                bValue = (byte)data;
            else
                bValue = 0;

            return bResult;
        }

        public static bool WinIO_SetHotSwap()
        {
            // 進行電池放電切換
            if (bIsBrightnessReadingEC)
            {
                // if (IsDebugMode) Trace.WriteLine("perry bIsBrightnessReadingEC = true");
                return false;
            }
            bool bResult = false;
            int iCount = 0;
            do
            {
                bResult = WinIO_WriteCommand(0x51, 1);
                ++iCount;
                if (!bResult)
                    Sleep(100);
            } while (bResult == false && iCount < 4);

            return bResult;
        }
        #endregion


        public static bool WinIO_MenuButtonLock()
        {
            bool bResult = false;
            int iCount = 0;
            do
            {
                bResult = WinIO_WriteCommand(0x5A, 1);
                ++iCount;
                if (!bResult)
                    Sleep(100);
            } while (bResult == false);

            return bResult;
        }

        public static bool WinIO_MenuButtonUnlock()
        {
            bool bResult = false;
            int iCount = 0;
            do
            {
                bResult = WinIO_WriteCommand(0x5A, 0);
                ++iCount;
                if (!bResult)
                    Sleep(100);
            } while (bResult == false);

            return bResult;
        }

        public static bool WinIO_FunctionButtonUnlock()
        {
            bool bResult = false;
            int iCount = 0;
            do
            {
                bResult = WinIO_WriteCommand(0x5C, 0);
                ++iCount;
                if (!bResult)
                    Sleep(100);
            } while (bResult == false);

            return bResult;
        }

        public static bool WinIO_FunctionButtonLock()
        {
            bool bResult = false;

#if Debug
            // Winmate Kenkun remove on 2015/09/09
            // if (IsOldHottabVersion) 
            // {
            //    if (IsDebugMode) Trace.WriteLine("Old Hottab Version, Hottab Mode(0).");
            //    bResult = ModeOpen(0); // Hottab Mode(Old Hottab : Only Hottab Mode)
            // }
            if (TestProduct.Equals("FMB8"))
            {
                if (IsDebugMode) Trace.WriteLine("Keyboard Mode - Standard Mode(1).");
                bResult = ModeOpen(1); // Standard Mode
            }
            else
            {
                if (IsDebugMode) Trace.WriteLine("Keyboard Mode - Consumer Mode(2).");
                bResult = ModeOpen(2); // Consumer Mode
            }

#else
            bResult = true;
#endif
            /*
            int iCount = 0;
            do
            {
                bResult = WinIO_WriteCommand(0x5C, 2);
                ++iCount;
                if (!bResult) Sleep(100);
            } while (bResult == false);
            */
            return bResult;
        }

        // Winmate Kenkun modify on 2014/12/19
        // Get EC All String
        /* public static bool WinIO_GetEC(out string version)
        {
            uint bValue;
            version = "";

            // BIOS Version
            WinIO_ReadFromECSpace(0x00, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x01, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x02, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x03, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();
            
            // V, C 標準品, 客製品
            WinIO_ReadFromECSpace(0x04, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            // EC Version
            WinIO_ReadFromECSpace(0x05, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x06, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x07, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            // 當EC define MINOR_VERSION (1-4個字元), 如一些客製版本或是FMB80, Winmate kenknu modfiy on 2016/11/23
            // if (TestProduct.Equals("FMB8") || TestProduct.Equals("IH83"))
            // {
                WinIO_ReadFromECSpace(0x08, out bValue);
                if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
                version += Convert.ToChar(bValue).ToString();

                WinIO_ReadFromECSpace(0x09, out bValue);
                if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
                version += Convert.ToChar(bValue).ToString();

                WinIO_ReadFromECSpace(0x0A, out bValue);
                if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
                version += Convert.ToChar(bValue).ToString();

                WinIO_ReadFromECSpace(0x0B, out bValue);
                if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
                version += Convert.ToChar(bValue).ToString();
            // }

            version.Trim();

            if (IsDebugMode) Trace.WriteLine("WinIO_GetEC() : " + version + " , Length : " + version.Length);
            return true;
        } */

        // Get MB Version
        public static bool WinIO_GetECMBVersion(out string version)
        {
            uint bValue;
            version = "";

            WinIO_ReadFromECSpace(0x00, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x01, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x02, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x03, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            if (IsDebugMode) Trace.WriteLine("WinIO_GetECMBVersion() : " + version);
            return true;
        }

        // Get EC Version
        public static bool WinIO_GetECVersion(out string version)
        {
            uint bValue;
            version = "";

            WinIO_ReadFromECSpace(0x05, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x06, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x07, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A)) bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            // if (IsDebugMode) Trace.WriteLine("WinIO_GetECVersion() : " + version);
            return true;
        }

        // Get SUB Version
        public static bool WinIO_GetECSubVersion(out string version)
        {
            uint bValue;

            version = "";

            WinIO_ReadFromECSpace(0x08, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A))
                bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x09, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A))
                bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x0A, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A))
                bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            WinIO_ReadFromECSpace(0x0B, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A))
                bValue = 0x5F;
            version += Convert.ToChar(bValue).ToString();

            if (IsDebugMode) Trace.WriteLine("WinIO_GetECSubVersion() : " + version);
            return true;
        }

        // Get EC Panel dimming type
        public static bool WinIO_GetECPanelDimmingType(out string type)
        {
            uint bValue;

            type = "";

            WinIO_ReadFromECSpace(0x04, out bValue);
            if ((bValue < 0x20) || (bValue > 0x7A))
                bValue = 0x5F;
            type = Convert.ToChar(bValue).ToString();

            return true;
        }

        // 0 ~ 3
        public static bool WinIO_SetWLBT(uint bValue)
        {
            bool bResult = false;
            int iCount = 0;
            do
            {
                bResult = WinIO_WriteCommand(0x43, bValue + 1); // Wireless on/off value=1,2,3,4
                ++iCount;
                if (!bResult)
                    Sleep(100);
            } while (bResult == false && iCount < 4);

            return bResult;
        }

        public static bool WinIO_IsSupportWarmSwap()
        {
            uint bValue = 0;
            char cFlag = new Char();

            WinIO_ReadFromECSpace(0x08, out bValue);

            try
            {
                cFlag = Convert.ToChar(bValue);
            }
            catch
            {
                // Do Nothing
            }

            if (cFlag == 'w' || cFlag == 'W') return true;

            return false;
        }


        // Volume State
        public static bool WinIO_ShowVolume()
        {
            // GlobalVariable.DebugMessage("winmate", "WinIO_ShowVolume", // GlobalVariable.bDebug);

            bool bResult = false;
            int iCount = 0;
            do
            {
                // GlobalVariable.DebugMessage("winmate", "start [" + iCount.ToString() + "]", // GlobalVariable.bDebug);
                bResult = WinIO_WriteCommand(0x5D, 1);
                ++iCount;
                if (!bResult)
                    Sleep(100);
                // GlobalVariable.DebugMessage("winmate", "end [" + iCount.ToString() + "][" + bResult.ToString() + "]", // GlobalVariable.bDebug);
            } while (bResult == false && iCount < 4);

            // GlobalVariable.DebugMessage("winmate", "WinIO_ShowVolume end [" + bResult.ToString() + "]", // GlobalVariable.bDebug);

            return bResult;
            // return WinIO_WriteToECSpace(0x90, (uint)0x20);
        }

        // Battery Information
        public static List<String> WinIO_GetBatteryInformation()
        {
            bool isPlugedFirstBattery, isPlugedSecondBatter;
            int bitNumber, registerInt;
            uint registerValue;
            List<String> batteryInformation;

            isPlugedFirstBattery = false;
            isPlugedSecondBatter = false;

            batteryInformation = new List<string>();

            if (!WinIO_ReadFromECSpace((uint)0x1a, out registerValue)) return batteryInformation;

            registerInt = (int)registerValue;

            bitNumber = 1 << 4;
            if ((registerInt & bitNumber) > 0) isPlugedFirstBattery = true;
            bitNumber = 1 << 6;
            if ((registerInt & bitNumber) > 0) isPlugedSecondBatter = true;

            if (isPlugedFirstBattery && WinIO_ReadFromECSpace((uint)0x1c, out registerValue))   /*0x1C:   Offset: 1Ch    Battery 1 remaining battery capacity : Low byte*/
            /*0x1D,   Offset: 1Dh    Battery 1 remaining battery capacity : Height byte */
            {
                batteryInformation.Add(registerValue.ToString());
            }

            if (isPlugedSecondBatter && WinIO_ReadFromECSpace((uint)0x2c, out registerValue))   /*0x2C:   Offset: 2Ch    Battery 2 remaining battery capacity : Low byte*/
            /*0x2D,   Offset: 2Dh    Battery 2 remaining battery capacity : Height byte */
            {
                batteryInformation.Add(registerValue.ToString());
            }

            return batteryInformation;
        }

        // Get EC ASL Brightness Length
        public static bool WinIO_GetEC_BrightnessLength(out uint bValue)
        {
            WinIO_WriteToECSpace(0x94, 0x80);
            WinIO_ReadFromECSpace(0x94, out bValue);
            return true;
        }

        // 0 ~ 8
        public static bool WinIO_SetBrightness(uint bValue)
        {
            // GlobalVariable.DebugMessage("winmate", "WinIO_SetBrightness [" + bValue.ToString() + "]", // GlobalVariable.bDebug);

            bool bResult = false;
            int iCount = 0;
            do
            {
                // GlobalVariable.DebugMessage("winmate", "start [" + iCount.ToString() + "]", // GlobalVariable.bDebug);
                bResult = WinIO_WriteCommand(0x4B, bValue);
                ++iCount;
                if (!bResult) Sleep(100);
                // GlobalVariable.DebugMessage("winmate", "end [" + iCount.ToString() + "][" + bResult.ToString() + "]", // GlobalVariable.bDebug);
            } while (bResult == false && iCount < 4);

            // GlobalVariable.DebugMessage("winmate", "WinIO_SetBrightness end [" + bResult.ToString() + "]", // GlobalVariable.bDebug);

            return bResult;
        }

        // Wireless State // brian add
        public static bool WinIO_SetDevice2State(uint bValue)
        {
            bool ret;

#if IsInstallHotTab
            ret = SetDevice2((byte)bValue);
#else
            ret = true;
#endif
            return ret;
        }

        public static bool WinIO_GetDeviceState(out byte bValue)
        {
            bool ret = false;

            ret = GetDevice(out bValue);

            return true;
        }

        public static bool WinIO_GetDevice2State(out byte bValue)
        {
            bool ret = false;

            ret = GetDevice2(out bValue);

            return true;
        }

        // Wireless State
        // public static bool WinIO_SetWirelessState(uint bValue)

        public static bool WinIO_SetDeviceState(uint bValue) // 不應該只是控制Wireless而是整個Device Winmate Kenkun modify on 2014/11/04
        {
            bool ret = false;
#if IsInstallHotTab
            ret = SetDevice((byte)bValue);
#else
            ret = true;
#endif
            return ret;
        }

        public static bool WinIO_SetRotationLock(uint bValue)
        {
            bool ret;
            // GlobalVariable.DebugMessage("winmate", "WinIO_SetRotationLock" + bValue.ToString(), // GlobalVariable.bDebug);
            // ret = WinIO_WriteCommand(0x4E, bValue);
            // GlobalVariable.DebugMessage("winmate", "WinIO_SetRotationLock end [" + ret.ToString() + "]", // GlobalVariable.bDebug);
            ret = SetAutoScreenRotationLock(bValue);
            return ret;
        }
        public static bool WinIO_GetRotationOSD(uint bValue)
        {
            bool ret;
            // GlobalVariable.DebugMessage("winmate", "WinIO_GetRotationOSD" + bValue.ToString(), // GlobalVariable.bDebug);
            ret = WinIO_ReadFromECSpace(0x4F, out bValue);
            // GlobalVariable.DebugMessage("winmate", "WinIO_GetRotationOSD end [" + ret.ToString() + "]", // GlobalVariable.bDebug);
            return ret;
        }
        #endregion

        #region Keyboard Function For All

        public static void KeyboardF11UpButton_Click()
        {
            keybd_event(0x7A, 0, 0x02, 0); // F11 up
        }

        #endregion

        #region Volume Function For All

        // 0 ~ 8
        public static int Volume_GetVolume()
        {
            int volume = 0;
            int value = 0;

            VolumeGetV(ref volume);

            if (volume <= 0) return 0;
            else if (volume <= 12) value = 1;
            else if (volume <= 24) value = 2;
            else if (volume <= 36) value = 3;
            else if (volume <= 36) value = 3;
            else if (volume <= 48) value = 4;
            else if (volume <= 60) value = 5;
            else if (volume <= 72) value = 6;
            else if (volume <= 84) value = 7;
            else value = 8;

            return value;
        }

        // 0 ~ 8
        public static void Volume_SetVolume(int value)
        {
            int volume;

            if (value < 0 || value > 8) return;

            if (value == 8) volume = 100;
            else volume = value * 12;
            VolumeSetMuteV(false);
            VolumeSetV(volume);
        }

        #endregion

        #region Camera DLL Declare For All

        [DllImport("CameraDll.dll")]
        public static extern uint InitFilterGraph(IntPtr hWnd, uint uiMessage, uint uiDevice);
        // public static extern uint InitFilterGraph(IntPtr hWnd, uint uiMessage);

        [DllImport("CameraDll.dll")]
        public static extern void DeinitFilterGraph();

        [DllImport("CameraDll.dll")]
        public static extern uint Snapshot([MarshalAs(UnmanagedType.LPWStr)] string szFilePath);

        [DllImport("CameraDll.dll")]
        public static extern uint VideoRecordStart([MarshalAs(UnmanagedType.LPWStr)]string szFilePath);

        [DllImport("CameraDll.dll")]
        public static extern uint VideoRecordStop();

        #endregion


        public static bool CheckComExist(string stCOM)
        {
            bool bRet = false; // Comport是否存在
            bool bPortOpen = false; // Comport是否被占住
            SerialPort serialComport = new SerialPort();

            string[] ports = SerialPort.GetPortNames();

            if (ports != null)
            {
                foreach (string sName in ports)
                {
                    if (stCOM == sName)
                    {
                        if (IsDebugMode) Trace.WriteLine("CheckComExist() - BarcodePort : " + sName);
                        bRet = true;
                    }
                }
            }

            if (bRet)
            {
                try
                {
                    serialComport.PortName = stCOM;
                    serialComport.Open();
                    bPortOpen = true;
                }
                catch (Exception ex)
                {
                    if (IsDebugMode) Trace.WriteLine("CheckComExist() - Open Exception : " + ex.Message);
                    bPortOpen = false;
                }

                if (bPortOpen)
                {
                    try
                    {
                        if (serialComport.IsOpen) serialComport.Close();
                        bPortOpen = true;
                    }
                    catch (Exception ex)
                    {
                        if (IsDebugMode) Trace.WriteLine("CheckComExist() - Close Exception : " + ex.Message);
                        bPortOpen = false;
                    }
                }
            }
            return bPortOpen;
        }

        #region PowerManager Function For All

        public static bool SetHotTabScreenOffACTime(uint ulTimeout)
        {
            // GlobalVariable.DebugMessage("winmate", "SetHotTabScreenOffTime", // GlobalVariable.bDebug);// brian add
            bool bResult = false;
            switch (OSName)
            {
                case "VISTA":
                    bResult = SetHotTabSchemaTurnOffACPanelV(ulTimeout);
                    break;
            }
            // GlobalVariable.DebugMessage("winmate", "SetHotTabScreenOffTime end [" + bResult.ToString() + "]", // GlobalVariable.bDebug);// brian add
            return bResult;
        }

        public static bool SetHotTabScreenOffBATTime(uint ulTimeout)
        {
            // GlobalVariable.DebugMessage("winmate", "SetHotTabScreenOffTime", // GlobalVariable.bDebug);// brian add
            bool bResult = false;
            switch (OSName)
            {
                case "VISTA":
                    bResult = SetHotTabSchemaTurnOffBATPanelV(ulTimeout);
                    break;
            }
            // GlobalVariable.DebugMessage("winmate", "SetHotTabScreenOffTime end [" + bResult.ToString() + "]", // GlobalVariable.bDebug);// brian add
            return bResult;
        }

        public static byte[] BrightnessListArray = new byte[11];
        public static int BrightnessLength = 0;
        public static int iECBrightnessLength = 0;

        public static int BrightnessPMLevelConvertECLevel(byte Value)
        {
            int iRet = -1;
            /*
                        for (int i = 0; i < BrightnessLength; i++)
                        {
                            if (BrightnessListArray[i] == Value)
                            {
                                iRet = i;
                                break;
                            }
                        }

                        if (iRet >= 8)
                            iRet = 8;
            */

            for (int i = 0; i < (BrightnessLength - 1); i++)
            {
                if (BrightnessListArray[i] == Value)
                {
                    iRet = i;
                    break;
                }
                else if (BrightnessListArray[i + 1] == Value)
                {
                    iRet = i + 1;
                    break;
                }
                else
                {
                    int iMidValue = (BrightnessListArray[i + 1] - BrightnessListArray[i]) / 2;
                    int a = BrightnessListArray[i] * 10;
                    int b = BrightnessListArray[i + 1] * 10;
                    int c = (b - a) / 2;
                    int d = c % 10;

                    if (d >= 5)
                    {
                        iMidValue++;
                    }

                    if ((Value >= iMidValue) && (Value < BrightnessListArray[i + 1]))
                    {
                        iRet = i + 1;
                        break;
                    }
                    else if ((Value < iMidValue) && (Value > BrightnessListArray[i]))
                    {
                        iRet = i;
                        break;
                    }
                }
            }

            if (iRet >= 10)
                iRet = 10;

            return iRet;

        }

        public static bool SetBrightness(uint bValue)
        {
            if (IsDebugMode) Trace.WriteLine("SetBrightness() : " + bValue); // Winmate Kenkun comment
            // GlobalVariable.DebugMessage("winmate", "SetBrightness [" + bValue.ToString() + "]", // GlobalVariable.bDebug);

            bool bResult = false;

            ++iSkipNotifyPMBrightness;

            if (iECBrightnessLength == 0)
                WinIO_SetBrightness(bValue);

            if (uiPMBrightness != bValue)
            {
                ++iSkipNotifyECBrightness;
                // if (IsDebugMode) Trace.WriteLine("uiPMBrightness != bValue , BrightnessListArray[bValue] = " + BrightnessListArray[bValue]); // Winmate Kenkun comment
                // GlobalVariable.DebugMessage("winmate", "SetBrightness SetDisplayBrightnessV [" + bValue.ToString() + "]", // GlobalVariable.bDebug);
#if IsInstallHotTab
                bResult = SetDisplayBrightnessV(BrightnessListArray[bValue]);
#endif
                if (!bResult)
                {
                    if (IsDebugMode) Trace.WriteLine("SetWmiBrightness"); // Winmate Kenkun comment
                    bResult = SetWmiBrightness((byte)(BrightnessListArray[bValue]));
                }
                // GlobalVariable.DebugMessage("winmate", "SetBrightness SetDisplayBrightnessV [" + bValue.ToString() + "]", // GlobalVariable.bDebug);
            }
            bResult = true;

            // GlobalVariable.DebugMessage("winmate", "SetBrightness end ==>" + bResult.ToString(), // GlobalVariable.bDebug);
            // if (IsDebugMode) Trace.WriteLine("SetBrightness() End"); // Winmate Kenkun comment
            return bResult;
        }

        public static bool SetBrightness(uint bValue, uint iMode)
        {
            // GlobalVariable.DebugMessage("winmate", "SetBrightness [" + bValue.ToString() + "]", // GlobalVariable.bDebug);

            bool bResult = false;

            ++iSkipNotifyPMBrightness;

            if (iECBrightnessLength == 0)
                WinIO_SetBrightness(bValue);

            if (uiPMBrightness != bValue)
            {
                ++iSkipNotifyECBrightness;
                // GlobalVariable.DebugMessage("winmate", "SetBrightness SetDisplayBrightnessV [" + bValue.ToString() + "]", // GlobalVariable.bDebug);
                bResult = SetDisplayBrightnessValue(BrightnessListArray[bValue], iMode);
                if (!bResult)
                {
                    bResult = SetWmiBrightness((byte)(BrightnessListArray[bValue]));
                }
                // GlobalVariable.DebugMessage("winmate", "SetBrightness SetDisplayBrightnessV [" + bValue.ToString() + "]", // GlobalVariable.bDebug);
            }
            bResult = true;


            // GlobalVariable.DebugMessage("winmate", "SetBrightness end ==>" + bResult.ToString(), // GlobalVariable.bDebug);
            return bResult;
        }

        public static bool SetBrightnessBetweenBatteryAndAC(uint bValue)
        {
            // GlobalVariable.DebugMessage("winmate", "SetBrightnessBetweenBatteryAndAC [" + bValue.ToString() + "]", // GlobalVariable.bDebug);

            bool bResult = false;
            switch (OSName)
            {
                case "VISTA":
                    bResult = true;
                    break;
            }
            // GlobalVariable.DebugMessage("winmate", "SetBrightnessBetweenBatteryAndAC end [" + bResult.ToString() + "]", // GlobalVariable.bDebug);
            return bResult;
        }

        public static bool SetWmiBrightness(byte targetBrightness)
        {
            // GlobalVariable.DebugMessage("winmate", "SetWmiBrightness ==>", // GlobalVariable.bDebug);

            bool bResult = false;

            ManagementScope scope = new ManagementScope("root\\WMI");
            SelectQuery query = new SelectQuery("WmiMonitorBrightnessMethods");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                using (ManagementObjectCollection objectCollection = searcher.Get())
                {
                    try
                    {
                        foreach (ManagementObject mObj in objectCollection)
                        {
                            mObj.InvokeMethod("WmiSetBrightness", new Object[] { UInt32.MaxValue, targetBrightness });
                            bResult = true;
                            // GlobalVariable.DebugMessage("winmate", "SetWmiBrightness [true]", // GlobalVariable.bDebug);
                            break;
                        }
                    }
                    catch
                    {
                        // GlobalVariable.DebugMessage("winmate", "SetWmiBrightness [false]", // GlobalVariable.bDebug);
                    }
                }
            }

            // GlobalVariable.DebugMessage("winmate", "SetWmiBrightness ==> " + bResult.ToString(), // GlobalVariable.bDebug);

            return bResult;
        }

        public static bool BrightnessFromWinIO(uint bValue)
        {
            bool bResult = false;

            switch (OSName)
            {
                case "VISTA":
                    // formDebug.writeLine("From WN :\t" +iSkipNotifyPMBrightness.ToString()+"\t"+ bValue.ToString() + "\t" + uiPMBrightness.ToString());
                    if (iSkipNotifyPMBrightness > 0)
                    {
                        --iSkipNotifyPMBrightness;
                        bIsShowBrightness = false;
                        bResult = true;
                    }
                    else
                    {
                        uiECBrightness = bValue;
                        if (uiPMBrightness != bValue)
                        {
                            uiPMBrightness = bValue;
                            ++iSkipNotifyECBrightness;
                            // GlobalVariable.DebugMessage("winmate", "BrightnessFromWinIO [" + bValue.ToString() + "]", // GlobalVariable.bDebug);
                            bResult = SetDisplayBrightnessV(BrightnessListArray[bValue]);
                            if (!bResult)
                            {
                                bResult = SetWmiBrightness((byte)(BrightnessListArray[bValue]));
                            }
                            // GlobalVariable.DebugMessage("winmate", "BrightnessFromWinIO [" + bValue.ToString() + "]", // GlobalVariable.bDebug);
                            break;
                        }
                        bResult = true;
                    }
                    break;
            }

            // GlobalVariable.DebugMessage("winmate", "BrightnessFromWinIO end ==>" + bResult.ToString(), // GlobalVariable.bDebug);
            return bResult;
        }
        public static bool BrightnessFromPowerManagement(uint bValue)
        {
            // GlobalVariable.DebugMessage("winmate", "BrightnessFromPowerManagement [" + bValue.ToString() + "]", // GlobalVariable.bDebug);

            bool bResult = false;

            switch (OSName)
            {
                case "VISTA":
                    // GlobalVariable.DebugMessage("winmate", "From PM :\t" + iSkipNotifyECBrightness.ToString() + "\t" + bValue.ToString() + "\t" + uiECBrightness.ToString(), // GlobalVariable.bDebug);
                    // formDebug.writeLine("From PM :\t" +iSkipNotifyECBrightness.ToString()+"\t"+ bValue.ToString() + "\t" + uiECBrightness.ToString());

                    if (iECBrightnessLength != 0)
                    {
                        uiPMBrightness = bValue;
                        uiECBrightness = bValue;
                        return true;
                    }

                    if (iSkipNotifyECBrightness > 0)
                    {
                        --iSkipNotifyECBrightness;
                        bResult = true;
                    }
                    else
                    {
                        uiPMBrightness = bValue;
                        if (uiECBrightness != bValue)
                        {
                            uiECBrightness = bValue;

                            if (iECBrightnessLength == 0)
                                bResult = WinIO_SetBrightness(bValue);
                            else
                                bResult = true;

                            if (bResult == true) ++iSkipNotifyPMBrightness;
                        }
                    }
                    break;
            }
            // GlobalVariable.DebugMessage("winmate", "BrightnessFromPowerManagement end [" + bResult.ToString() + "]", // GlobalVariable.bDebug);
            return bResult;
        }
        public static bool BrightnessClearCount()
        {
            bool bResult = false;

            switch (OSName)
            {
                case "VISTA":
                    iSkipNotifyECBrightness = 0;
                    iSkipNotifyPMBrightness = 0;
                    bResult = true;
                    break;
            }
            return bResult;
        }

        #endregion

        #region BatteryInfo

        public static bool GetACInOutStatus(out int Value)
        {
            bool ret = false;
            uint data = 0;
            int tmp1 = 0;

            data = 0;
            ret = false;
            ret = WinIO_ReadFromECSpace(0x1A, out data); /*0x1A:   Offset: 1A    AC Status */
            if (!ret == false)
            {
                Value = 0;
                return false;
            }
            else
            {
                tmp1 = Convert.ToInt32(data);
            }

            Value = tmp1;

            return !ret;
        }


        // For No Changer / Gauge IC Product, Winmate kenkun add on 2016/05/26 >>
        public static bool GetBattery2ChargingVoltage_NoGaugeIC(out int Value)
        {
            bool ret = false;
            uint data = 0;
            ret = WinIO_ReadFromECSpace(0x3B, out data); /*0x3B:   Offset: 3Bh    Battery 2 Voltage : Low byte*/
            if (!ret == false)
            {
                Value = 0;
                return false;
            }
            else
            {
                Trace.WriteLine("Battery 2 => ECSpace (0x3B) = " + data.ToString("X"));
                Value = Convert.ToInt32(data);
            }
            return !ret;
        }
        // For No Changer IC Product, Winmate kenkun add on 2016/05/26 <<

        #endregion

        public static void Process_KillDuplicate(String prcName)
        {
            int count = 0;
            foreach (Process prc in Process.GetProcesses())
            {
                if (prc.ProcessName.Equals(prcName))
                {
                    count++;
                    if (count >= 1)
                    {
                        prc.Kill();
                    }
                }
            }
        }

        public static bool Process_Execute(String APName)
        {
            ProcessStartInfo processInfo;
            Process process;
            String fileName = null;

            if (APName != null)
            {
                fileName = APName;
            }

            if (fileName == null || fileName.Equals("")) return false;

            try
            {
                processInfo = new ProcessStartInfo();
                processInfo.FileName = fileName;

                process = new Process();
                process.StartInfo = processInfo;
                process.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // public static bool SetLightSensorOnOff(uint on)
        // {

        //    bool bResult = false;
        //    int iCount = 0;
        //    uint bValue;

        //    WinIO_ReadFromECSpace(0x3A, out bValue);

        //    do
        //    {
        //        if (on == 1)
        //            bResult = WinIO_WriteToECSpace(0x3A, (bValue | 0x20));
        //        else
        //            bResult = WinIO_WriteToECSpace(0x3A, (bValue & 0xD0));

        //        ++iCount;
        //        Thread.Sleep(100);

        //    } while (bResult == false && iCount < 4);

        //    return bResult;

        // }

        // public static bool GetLightSensorValue(out int Value)
        // {
        //    bool ret = false;
        //    uint data = 0;
        //    int tmp1 = 0;
        //    int tmp2 = 0;

        //    // Value = 0;

        //    data = 0;
        //    ret = false;
        //    ret = WinIO_ReadFromECSpace(0x36, out data);
        //    if (!ret == false)
        //    {
        //        Value = 0;
        //        return false;
        //    }
        //    else
        //        tmp1 = Convert.ToInt32(data);

        //    data = 0;
        //    ret = false;
        //    ret = WinIO_ReadFromECSpace(0x37, out data);
        //    if (!ret == false)
        //    {
        //        Value = 0;
        //        return false;
        //    }
        //    else
        //        tmp2 = Convert.ToInt32(data);

        //    Value = (tmp2 << 8) + tmp1;

        //    return ret;
        // }
    }
}
