using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace barcode
{
    // Intermec EA30/EA11 專用API
    class IsdcRsApi
    {
        #region ISDC_RS DLL Declare For Barcode
        [DllImport("ISDC_RS.dll")]
        public static extern byte Disconnect();

        [DllImport("ISDC_RS.dll")]
        public static extern byte Connect();

        [DllImport("ISDC_RS.dll")]
        public static extern byte Initialize(char[] pszRegistryKey, byte[] pStatus);

        [DllImport("ISDC_RS.dll")]
        public static extern byte Deinitialize();

        [DllImport("ISDC_RS.dll")]
        public static extern byte ConfigurationDialog();

        [DllImport("ISDC_RS.dll")]
        public static extern byte GetDllVersion(byte[] pOutputBuffer, UInt32 nSizeOfOutputBuffer, UInt32[] pnBytesReturned);

        [DllImport("ISDC_RS.dll")]
        public static extern byte ControlCommand(byte[] pInputBuffer, UInt32 nBytesInInputBuffer, byte[] pOutputBuffer, UInt32 nSizeOfOutputBuffer, UInt32[] pnBytesReturned);

        [DllImport("ISDC_RS.dll")]
        public static extern byte DeviceIOControl(UInt32 ioControlCode, byte[] pInputBuffer, UInt32 nBytesInInputBuffer, byte[] pOutputBuffer, UInt32 nSizeOfOutputBuffer, UInt32[] pnBytesReturned);

        [DllImport("ISDC_RS.dll")]
        public static extern byte MessageIdentify(Int32 handle, byte ID);

        [DllImport("ISDC_RS.dll")]
        public static extern byte GetBarcodeData(byte[] pOutputBuffer, UInt32 nSizeOfOutputBuffer, UInt32[] pnBytesReturned);

        [DllImport("ISDC_RS.dll")]
        public static extern byte GetRawData(byte[] pOutputBuffer, UInt32 nSizeOfOutputBuffer, UInt32[] pnBytesReturned);

        [DllImport("ISDC_RS.dll")]
        public static extern byte StatusRead(byte[] pInputBuffer, UInt32 nBytesInInputBuffer, byte[] pOutputBuffer, UInt32 nSizeOfOutputBuffer, UInt32[] pnBytesReturned);
        #endregion

        public UInt32 iInputOutputBufferSize = 512;

        private byte[] InputBuffer;
        private byte[] OutputBuffer;
        private UInt32 nBytesInInputBuffer;
        private UInt32[] pnBytesReturned;
        private UInt32 OutputBufferSize;
        private UInt32 InputBufferSize;
        bool IsDebugMode = true;

        public IsdcRsApi()
        {
            InputBufferSize = iInputOutputBufferSize;
            OutputBufferSize = iInputOutputBufferSize;

            InputBuffer = new byte[InputBufferSize];
            OutputBuffer = new byte[OutputBufferSize];

            nBytesInInputBuffer = 0;
            pnBytesReturned = new UInt32[1];
        }

        public byte InitializeIsdcRs(string registerString)
        {
            char[] myString = new char[iInputOutputBufferSize];
            byte[] pRomVersion = new byte[1];
            byte status;

            for (int i = 0; i < registerString.Length; i++)
            {
                myString[i] = registerString[i];
            }
#if IsInstallHotTab
            status = Initialize(myString, pRomVersion);
#else
            status = 0;
#endif
            return status;
        }

        public byte DeinitializeIsdcRs()
        {
            byte status;

            status = Deinitialize();

            return status;
        }

        public byte ConnectIsdcRs()
        {
            byte status;

            status = Connect();

            return status;
        }

        public byte DisconnectIsdcRs()
        {
            byte status;

            status = Disconnect();

            return status;
        }

        public byte ConfigurationDialogIsdcRs()
        {
            byte status;

            status = ConfigurationDialog();

            return status;
        }

        public byte SetPortNumber(byte num)
        {
            byte status;

            InputBuffer[0] = (byte)num;
            InputBuffer[1] = 0x00;
            InputBuffer[2] = 0x00;
            InputBuffer[3] = 0x00;

            nBytesInInputBuffer = 4;

            status = DeviceIOControl(0x020D, InputBuffer, nBytesInInputBuffer, OutputBuffer, OutputBufferSize, pnBytesReturned);

            return status;
        }

        public byte SetBaudRate(UInt32 baudrate)
        {
            byte status;

            InputBuffer[0] = (byte)(baudrate % 0x100);
            InputBuffer[1] = (byte)(baudrate / 0x100);
            InputBuffer[2] = (byte)(baudrate / 0x10000);
            InputBuffer[3] = (byte)(baudrate / 0x1000000);

            nBytesInInputBuffer = 4;

            status = DeviceIOControl(0x020E, InputBuffer, nBytesInInputBuffer, OutputBuffer, OutputBufferSize, pnBytesReturned);

            return status;
        }

        public byte ScanStart()
        {
            byte status;

            InputBuffer[0] = 0x20;
            InputBuffer[1] = 0x40;
            InputBuffer[2] = 0x01;

            nBytesInInputBuffer = 3;

            if (IsDebugMode) Trace.WriteLine("Send Intermec Trigger : 0x20, 0x40, 0x01");
            status = ControlCommand(InputBuffer, nBytesInInputBuffer, OutputBuffer, OutputBufferSize, pnBytesReturned);

            return status;
        }

        public byte ScanStop()
        {
            byte status;

            InputBuffer[0] = 0x20;
            InputBuffer[1] = 0x40;
            InputBuffer[2] = 0x00;

            nBytesInInputBuffer = 3;

            status = ControlCommand(InputBuffer, nBytesInInputBuffer, OutputBuffer, OutputBufferSize, pnBytesReturned);

            return status;
        }

        public byte GetDllVersion(out string version)
        {
            byte status;

            char[] OutputBufferChar = new char[OutputBufferSize];

            version = "";

            status = GetDllVersion(OutputBuffer, OutputBufferSize, pnBytesReturned);

            if (pnBytesReturned[0] <= 0)
            {
                version = "N/A";
            }
            else
            {
                for (int i = 0; i < pnBytesReturned[0]; i++)
                    version += (char)OutputBuffer[i];
            }
            return status;
        }

        public byte MessageIdentifyIsdcRs(Int32 HWND, byte ID)
        {
            byte status;

            status = MessageIdentify(HWND, ID);

            return status;
        }

        //// Intermec EA30/EA11除了RAW DATA模式還有一個PACKET模式
        public byte GetBarcodeDataIsdcRs(byte[] pOutputBuffer, UInt32 nSizeOfOutputBuffer, UInt32[] pnBytesReturned)
        {
            byte status;

            status = GetBarcodeData(pOutputBuffer, nSizeOfOutputBuffer, pnBytesReturned);

            return status;
        }

        // Intermec EA30/EA11 預設是使用RAW DATA模式去接收資料
        public byte GetRawDataIsdcRs(byte[] pOutputBuffer, UInt32 nSizeOfOutputBuffer, UInt32[] pnBytesReturned)
        {
            byte status;

            status = GetRawData(pOutputBuffer, nSizeOfOutputBuffer, pnBytesReturned);

            return status;
        }

        public byte GetVersion(out string retString)
        {
            byte status;
            UInt32 wBarCodeSize;

            InputBuffer[0] = 0x30;
            InputBuffer[1] = 0xC0;

            nBytesInInputBuffer = 2;

            status = StatusRead(InputBuffer, nBytesInInputBuffer, OutputBuffer, OutputBufferSize, pnBytesReturned);

            retString = "";

            if ((status == 0) && (pnBytesReturned[0] != 0))
            {
                wBarCodeSize = pnBytesReturned[0];

                // refresh barcode data display
                string s_tmp = "";

                for (int wCount = 4; wCount < wBarCodeSize; wCount++)
                {
                    s_tmp += Convert.ToChar(OutputBuffer[wCount]);
                }
                retString = s_tmp;
            }
            else
            {
                retString = "";
            }

            return status;
        }

        public byte GetHardwareId(out string retString)
        {
            byte status;
            UInt32 wBarCodeSize;

            InputBuffer[0] = 0x30;
            InputBuffer[1] = 0x82;

            nBytesInInputBuffer = 2;

            status = StatusRead(InputBuffer, nBytesInInputBuffer, OutputBuffer, OutputBufferSize, pnBytesReturned);

            retString = "";

            if ((status == 0) && (pnBytesReturned[0] != 0))
            {
                wBarCodeSize = pnBytesReturned[0];

                // refresh barcode data display
                string s_tmp = "0x";

                for (int wCount = 2; wCount < wBarCodeSize; wCount++)
                {
                    s_tmp += String.Format("{0:X2}", OutputBuffer[wCount]);
                }

                retString = s_tmp;
            }
            else
            {
                retString = "";
            }

            return status;
        }
    }
}
