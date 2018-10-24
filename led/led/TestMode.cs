using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace led
{
    class TestMode
    {
        #region Winmate WinIO DLL Declare
        [DllImport(@"WMIO2.dll")]
        private static extern bool WinIO_WriteCommand(byte uiCommand, byte uiData);
        #endregion

        enum ListCommand : byte
        {
            LED = 0x1,
            Charging = 0x2,
            Discharging = 0x3,
            Unlock = 0x87,
            Lock = 0xAA
        }

        public enum ListLED : byte
        {
            Red = 0x01,
            Green = 0x02,
            RF = 0x04
        }

        public enum ListBattery : byte
        {
            Internal = 0x01,
            External = 0x02
        }

        private static bool SetTestModeCommand(byte cmd)
        {
            return WinIO_WriteCommand(0x1B, cmd);
        }

        public static bool UnlockTestMode()
        {
            bool bRet = false;

            if (SetTestModeCommand((byte)ListCommand.Unlock))
            {
                if (SetTestModeCommand((byte)ListCommand.Unlock))
                {
                    bRet = true;
                }
            }

            return bRet;
        }

        public static bool LockTestMode()
        {
            return SetTestModeCommand((byte)ListCommand.Lock);
        }

        public static bool SetTestModeLED(byte data)
        {
            bool bRet = false;

            if (SetTestModeCommand((byte)ListCommand.LED))
            {
                if (SetTestModeCommand(data))
                {
                    bRet = true;
                }
            }

            return bRet;
        }

        public static bool SetTestModeCharging(byte data)
        {
            bool bRet = false;

            if (SetTestModeCommand((byte)ListCommand.Charging))
            {
                if (SetTestModeCommand(data))
                {
                    bRet = true;
                }
            }

            return bRet;
        }

        public static bool SetTestModeDischarging(byte data)
        {
            bool bRet = false;

            if (SetTestModeCommand((byte)ListCommand.Discharging))
            {
                if (SetTestModeCommand(data))
                {
                    bRet = true;
                }
            }

            return bRet;
        }
    }
}
