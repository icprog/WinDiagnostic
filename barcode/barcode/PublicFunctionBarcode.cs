using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace barcode
{
    public class PublicFunctionBarcode
    {

        #region Moto SE4500DL

        private const byte SSI_SOURCE_HOST = 0x04;
        private const int SSI_MAX_PACKET_LENGTH = 258;
        private const byte SSI_HEADER_LEN = 0x04;

        private const byte REQUEST_REVISION = 0xA3;
        private const byte REPLY_REVISION = 0xA4;
        private const byte PARAM_SEND = 0xC6;
        private const byte CMD_ACK = 0xD0;
        private const byte CMD_NAK = 0xD1;
        private const byte START_SESSION = 0xE4;
        private const byte STOP_SESSION = 0xE5;

        #endregion

        #region Definition
        private SerialPort mSerialPort;
        private int iPortNumber;
        private bool isSupportBarCode = false;
        private bool firstScan = false;
        private string msg2 = "";
        private byte[] buffer;
        private int count;
        private string message;
        private byte DeviceResponse;
        private static string IsdcRsVersion = "";

        private IntPtr ihw;
        private static string se4500dlVersion = "No device found.";
        public bool isOpenSE4500DL = false;

        public static int type = 0; // 0:Opticon-Default, 1:BS-523, 2:Opticon M3, 3:Opticon-MDI3100, 4:Intermec EA30/EA11, 5:Motorola SE4500DL
        // 會依照使用者設定的值, 在程式中自動判斷TYPE( 4 = initinalIsdcRsBarCode / 1~3 = serialComport_DataReceived )
        public static uint BarcodeType = 2;             // 0:Normal(BS523/MDI3100/M3), 1:MDL-1000, 2:Intermec EA30/EA11, 3:Motorola SE4500DL (經由使用者在CONFIG設定)
        public static uint BarcodeIdentifierCode = 0;   // 0:removed, 1:reserved
        public static uint BarcodeVisible = 1;          // 0:not dispaly, 1:display

        private static IsdcRsApi m_IsdcRsApi;
        public const byte BCD = 0x60; // BARCODE DATA
        public static uint WM_ISCP_FRAME = RegisterWindowMessage("WM_ISCP_FRAME");
        public static uint WM_RAW_DATA = RegisterWindowMessage("WM_RAW_DATA");
        static bool IsDebugMode = true;
        static string PortAddressBarcode = "COM15";
        int BarcodeReadTime = 2000;

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint RegisterWindowMessage(string lpString);
        #endregion

        public PublicFunctionBarcode(string COMLocation, IntPtr hw)
        {
            BarcodeType = Convert.ToUInt16(BarcodeType);
            ihw = hw;
            iPortNumber = Convert.ToInt32(COMLocation.Remove(0, 3));

            mSerialPort = new SerialPort(COMLocation);
            mSerialPort.BaudRate = 9600;
            mSerialPort.DataBits = 8;
            mSerialPort.Parity = Parity.None;
            mSerialPort.StopBits = StopBits.One;
            mSerialPort.Handshake = Handshake.None; // Winmate kenkun modify on 2017/07/28
            mSerialPort.DataReceived += new SerialDataReceivedEventHandler(mSerialPort_DataReceived);

            if (BarcodeType == 2)
            {
                if (!initinalIsdcRsBarCode())
                {
                    m_IsdcRsApi.DisconnectIsdcRs();
                    m_IsdcRsApi.DeinitializeIsdcRs();
                    initinalBarCode();
                }
            }
            else if (BarcodeType == 3)
            {
                initinalSE4500DLBarCode();
            }
            else
            {
                initinalBarCode();
            }
        }

        public bool IsSupportBarCode()
        {
            return isSupportBarCode;
        }

        public void IsForceOpticonMDI3100()
        {
            isSupportBarCode = true;
            firstScan = true;
            msg2 = "";
            type = 0;
        }

        public String GetBarcodeName2()
        {
            return GetBarcodeName();
        }

        public static String GetBarcodeName()
        {
            if (type == 0)
                return "Opticon-Default";
            else if (type == 1)
                return "BS-523";
            else if (type == 2)
                return "Opticon M3";
            else if (type == 3)
                return "Opticon-MDI3100";
            else if (type == 4)
                return IsdcRsVersion;
            else if (type == 5)
                return se4500dlVersion;
            else
                return "none";
        }

        public bool IsComportReady()
        {
            string[] ports = SerialPort.GetPortNames();

            if (Array.Exists(ports, CheckSaurus)) return true;
            else return false;
        }

        private static bool CheckSaurus(String s)
        {
            if (s == PortAddressBarcode) return true;
            else return false;
        }

        public void initinalBarCode()
        {
            if ((BarcodeType == 0) || (BarcodeType == 1))
            {
                if (IsComportReady() && (!IsSupportBarCode()))
                {
                    if (IsDebugMode) Trace.WriteLine("initinalBarCode()");
                    // GlobalVariable.DebugMessage("winmate", "initinalBarCode", GlobalVariable.bDebug);
                    openPort();

                    firstScan = false;
                    isSupportBarCode = false;
                    DeviceResponse = 0x00;

                    initinalOpticonBarCode();

                    initinalBS523BarCode();

                    initinalOpticonM3BarCode();

                    closePort();
                    // GlobalVariable.DebugMessage("winmate", "initinalBarCode end", GlobalVariable.bDebug);
                }
            }
        }

        public void initinalBS523BarCode()
        {
            if (IsDebugMode) Trace.WriteLine("initinalBS523BarCode()");
            byte[] SendDataForBS523 = { 0x1B };

            mSerialPort.Write(SendDataForBS523, 0, SendDataForBS523.Length);
            Thread.Sleep(150);
        }

        public void initinalOpticonBarCode()
        {
            if (IsDebugMode) Trace.WriteLine("initinalOpticonBarCode()");
            byte[] SendData = { 0x1B, 0x54, 0x35, 0x0D }; // command T5: Indicator duration: 0.2 s
            byte[] SendDataReadTime = { 0x1B, 0x59, 0x32, 0x5A, 0x32, 0x0D };// command Y3: Read time 2s

            mSerialPort.Write(SendData, 0, SendData.Length);
            Thread.Sleep(150);

            mSerialPort.Write(SendDataReadTime, 0, SendDataReadTime.Length);
            Thread.Sleep(150);
        }

        public void initinalOpticonM3BarCode()
        {
            if (IsDebugMode) Trace.WriteLine("initinalOpticonM3BarCode()");
            byte[] SendData = { 0x55, 0xA0, 0xD1, 0xAA };

            mSerialPort.Write(SendData, 0, SendData.Length);
            Thread.Sleep(150);
        }

        public bool initinalIsdcRsBarCode()
        {
            if (IsDebugMode) Trace.WriteLine("PublicFunctionBarcode - initinalIsdcRsBarCode()");
            byte status = 0;
            string s = "";
            byte port = 15;
            int num = PortAddressBarcode.IndexOf("COM", 0); // gCOMLocation.IndexOf("COM", 0);

            if (num >= 0)
            {
                s = PortAddressBarcode.Remove(num, 3); // gCOMLocation.Remove(num, 3);
                port = Convert.ToByte(s);
            }

            m_IsdcRsApi = new IsdcRsApi();

            status = m_IsdcRsApi.InitializeIsdcRs("HKCU\\SOFTWARE\\HotTab\\Intermec\n"); // for windws 7 or windows 8

            if (status != 0) return false;

            status += m_IsdcRsApi.SetPortNumber(port);
            status += m_IsdcRsApi.SetBaudRate(57600);
            if (status != 0)
            {
                m_IsdcRsApi.DeinitializeIsdcRs();
                return false;
            }

            status += m_IsdcRsApi.ConnectIsdcRs(); // 出現搜尋Comport的ProcessBar視窗
            if (status != 0)
            {
                m_IsdcRsApi.DeinitializeIsdcRs();
                return false;
            }

            status += m_IsdcRsApi.GetVersion(out IsdcRsVersion);
            Trace.WriteLine("status: " + status);
            if (status == 0)
            {
                isSupportBarCode = true;
                firstScan = true;
                msg2 = "";
                type = 4;
                Trace.WriteLine("type set to 4");
                return true;
            }
            return false;
        }

        public void initinalSE4500DLBarCode()
        {
            if (IsComportReady() && (!IsSupportBarCode()))
            {
                if (IsDebugMode) Trace.WriteLine("initinalSE4500DLBarCode()");
                openPort();

                firstScan = false;
                isSupportBarCode = false;
                DeviceResponse = 0x00;

                byte[] PowerModeAlwaysOn = { 0xFF, 0x80, 0x00 };
                byte[] Data = { 0xFF, 0xEE, 0x01 };

                Thread.Sleep(400);

                for (int i = 0; i < 10; i++)
                {
                    // Trace.WriteLine("initinalSE4500DLBarCode="+ i.ToString());
                    Thread.Sleep(50);

                    moto_write(PARAM_SEND, PowerModeAlwaysOn, PowerModeAlwaysOn.Length);
                    Thread.Sleep(50);

                    moto_write(PARAM_SEND, PowerModeAlwaysOn, PowerModeAlwaysOn.Length);
                    Thread.Sleep(50);

                    moto_write(PARAM_SEND, PowerModeAlwaysOn, PowerModeAlwaysOn.Length);
                    Thread.Sleep(50);

                    moto_write(PARAM_SEND, PowerModeAlwaysOn, PowerModeAlwaysOn.Length);
                    Thread.Sleep(50);

                    moto_write(PARAM_SEND, Data, Data.Length);
                    Thread.Sleep(50);

                    if (isSupportBarCode)
                    {
                        if (IsDebugMode) Trace.WriteLine("initinalSE4500DLBarCode() success!");
                        break;
                    }
                }
                closePort();

                if ((isSupportBarCode) && (type == 5))
                {
                    openPortSSI();

                    Thread.Sleep(200);
                    SsiDllApi.TransmitVersion(iPortNumber);
                    SsiDllApi.SetVersionBuffer(iPortNumber, SsiDllApi.DecodeData, SsiDllApi.MAX_LEN);
                    Thread.Sleep(200);
                    WriteTriggerTimeout((byte)(BarcodeReadTime / 1000));
                    Thread.Sleep(200);
                }
            }
        }

        public bool isOpen()
        {
            if (mSerialPort.IsOpen) return true;
            else return false;
        }

        public void openPort()
        {
            try
            {
                if (IsDebugMode) Trace.WriteLine("PublicFunctionBarcode - Try to open Barcode Port.");
                if (BarcodeType == 1) mSerialPort.RtsEnable = true;
                mSerialPort.Open();
            }
            catch (IOException)
            {
                Thread.Sleep(1000);
                mSerialPort.Open();
            }
        }

        public void closePort()
        {
            try
            {
                if (IsDebugMode) Trace.WriteLine("PublicFunctionBarcode - Close Barcode Port.");
                if (BarcodeType == 1) mSerialPort.RtsEnable = false;
                mSerialPort.Close();
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void free()
        {
            if (IsDebugMode) Trace.WriteLine("PublicFunctionBarcode - free() , Type : " + BarcodeType + " , Port.IsOpen : " + mSerialPort.IsOpen);
            if (BarcodeType == 2)
            {
                if (m_IsdcRsApi != null)
                {
                    try
                    {
                        m_IsdcRsApi.DisconnectIsdcRs();
                        m_IsdcRsApi.DeinitializeIsdcRs();
                        m_IsdcRsApi = null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            else if (BarcodeType == 3)
            {
                closePortSSI();
            }

            if (mSerialPort.IsOpen) mSerialPort.Close();
            mSerialPort.Dispose();
            mSerialPort = null;
        }

        private void mSerialPort_DataReceived(Object sender, SerialDataReceivedEventArgs e)
        {
            if ((sender as SerialPort).BytesToRead > 0)
            {
                buffer = new byte[mSerialPort.ReadBufferSize];

                count = mSerialPort.Read(buffer, 0, mSerialPort.ReadBufferSize - 1);

                char[] tmp = new char[count];

                for (int i = 0; i < count; i++)
                {
                    tmp[i] = (char)buffer[i];

                    if (buffer[0] == 0x8F) DeviceResponse = 0x8F;
                    else if (buffer[0] == 0x15) DeviceResponse = 0x15;
                    else if (buffer[1] == CMD_ACK) DeviceResponse = CMD_ACK;
                }

                message = new String(tmp, 0, count);

                if (DeviceResponse == CMD_ACK)// SE4500DL
                {
                    if (buffer[1] == CMD_ACK)
                    {
                        se4500dlVersion = "SE4500DL";
                        // Trace.WriteLine("CMD_ACK");
                    }
                    else if ((buffer[1] != CMD_ACK) && (buffer[1] != CMD_NAK))
                    {
                        msg2 += message;
                    }
                }
                else
                {
                    msg2 += message;
                }

                // if (IsDebugMode) Trace.WriteLine("SerialPort DataReceived : " + message + " , Count : " + count);
            }

            if (!firstScan && msg2.Contains("OK"))
            {
                isSupportBarCode = true;
                firstScan = true;
                msg2 = "";
                type = 3;
            }
            else if (!firstScan && (DeviceResponse == 0x15))
            {
                isSupportBarCode = true;
                firstScan = true;
                msg2 = "";
                type = 1;
            }
            else if (!firstScan && (DeviceResponse == 0x8F))
            {
                isSupportBarCode = true;
                firstScan = true;
                msg2 = "";
                type = 2;
            }
            else if (!firstScan && (DeviceResponse == CMD_ACK))
            {
                isSupportBarCode = true;
                firstScan = true;
                msg2 = "";
                type = 5;
            }

            if (isSupportBarCode)
            {
                if ((type == 0) || (type == 3)) // Opticon 
                {
                    if ((int)buffer[count - 1] == 0x0D) // ASCII control unint 0x0D=CR (enter)
                    {
                        if (IsDebugMode) Trace.WriteLine("Get MDI3100 Data");
                        PublicFunction.OutputKeyboardMessage(msg2);// Winmate brian modify on 2018/04/18
                        msg2 = "";
                    }
                }
                else if (type == 1)// BS523
                {
                    if (IsDebugMode) Trace.WriteLine("Get BS523 Data");

                    if ((msg2.Length == 1) && ((msg2[0] == 0x06) || (msg2[0] == 0x15)))
                        msg2 = "";
                    else
                    {
                        if (msg2.Length != 0)
                            PublicFunction.OutputKeyboardMessage(msg2);// Winmate brian modify on 2018/04/18
                    }
                    msg2 = "";
                }
                else if (type == 2)// Opticon M3 (1D)
                {
                    if ((int)buffer[count - 1] == 0x0A)
                    {
                        if (IsDebugMode) Trace.WriteLine("Get M3 Data");
                        PublicFunction.OutputKeyboardMessage(msg2);// Winmate brian modify on 2018/04/18
                        msg2 = "";
                    }
                }
            }
        }

        public void BarcodeScan()
        {
            byte[] TriggerMDI3100 = { 0x1B, 0x5A, 0x0D };   // command Z: trigger the reader
            byte[] TriggerBS523 = { 0x1B, 0x31 };           // command Z: trigger the reader
            byte[] TriggerM3 = { 0x55, 0x30, 0x41, 0xAA };

            try
            {
                if (type == 5)
                {
                    openPortSSI();
                    if (isOpenSE4500DL)
                    {
                        SsiDllApi.PullTrigger(iPortNumber);
                        SsiDllApi.SetDecodeBuffer(iPortNumber, SsiDllApi.DecodeData, SsiDllApi.MAX_LEN);
                    }
                }
                else if (type != 4)
                {
                    if (isOpen())
                    {
                        if ((type == 0) || (type == 3))// Opticon
                        {
                            if (IsDebugMode) Trace.WriteLine("Send MDI3100 Trigger : 0x1B, 0x5A, 0x0D");
                            mSerialPort.Write(TriggerMDI3100, 0, TriggerMDI3100.Length);
                        }
                        else if (type == 1)// BS523
                        {
                            if (IsDebugMode) Trace.WriteLine("Send BS523 Trigger : 0x1B, 0x31");
                            mSerialPort.Write(TriggerBS523, 0, TriggerBS523.Length);
                        }
                        else if (type == 2)// Opticon M3
                        {
                            if (IsDebugMode) Trace.WriteLine("Send M3 Trigger : 0x55, 0x30, 0x41, 0xAA");
                            mSerialPort.Write(TriggerM3, 0, TriggerM3.Length);
                        }
                    }
                }
                else
                {
                    m_IsdcRsApi.ScanStart();
                }
            }
            catch (Exception ex)
            {
                if (IsDebugMode) Trace.WriteLine(ex.Message);
            }
        }

        public void BarcodeScanStop()
        {
            if (type == 4) m_IsdcRsApi.ScanStop();
        }

        public void DisconnectIsdcRs()
        {
            m_IsdcRsApi.DisconnectIsdcRs();
        }

        public void DeinitializeIsdcRs()
        {
            m_IsdcRsApi.DeinitializeIsdcRs();
        }

        public byte GetVersionIsdcRs(out string retString)
        {
            byte status = 0;
            status += m_IsdcRsApi.GetVersion(out retString);
            if (status != 0) retString = "";
            return status;
        }

        public byte GetHardwareIdIsdcRs(out string retString)
        {
            byte status = 0;
            status += m_IsdcRsApi.GetHardwareId(out retString);
            if (status != 0) retString = "";
            return status;
        }

        public static byte GetBarcodeDataIsdcRs()
        {
            byte[] data = new byte[m_IsdcRsApi.iInputOutputBufferSize];
            UInt32[] pnBytesReturned = new UInt32[1];

            byte status;
            Int32 wBarCodeSize;
            string msg = "";

            status = m_IsdcRsApi.GetBarcodeDataIsdcRs(data, m_IsdcRsApi.iInputOutputBufferSize, pnBytesReturned);
            wBarCodeSize = (Int32)(data[1] | (data[0] << 8));

            if (status == 0)
            {
                // refresh barcode data display
                string s_tmp = "";
                bool bIdentifier = false;

                for (int wCount = 2; wCount < wBarCodeSize + 2; wCount++)
                {
                    if ((wCount == 2) && (data[wCount] == ']')) bIdentifier = true;
                    if ((wCount == (wBarCodeSize)) && (data[wCount] == 0x0D)) data[wCount] = 0x00;
                    if ((wCount == (wBarCodeSize + 1)) && (data[wCount] == 0x0A)) data[wCount] = 0x00;
                    s_tmp += Convert.ToChar(data[wCount]);
                }

                msg = s_tmp;

                if ((bIdentifier) && (BarcodeIdentifierCode == 0)) msg = s_tmp.Remove(0, 3);

                if (IsDebugMode) Trace.WriteLine("Get Intermec Packet Data");

                PublicFunction.OutputKeyboardMessage(msg);// Winmate brian modify on 2018/04/18
            }
            return status;
        }

        // Intermec 預設的資料讀取模式
        public static byte GetRawDataIsdcRs()
        {
            byte[] data = new byte[m_IsdcRsApi.iInputOutputBufferSize];
            UInt32[] pnBytesReturned = new UInt32[1];

            byte status;
            UInt32 wBarCodeSize;
            string msg = "";
            bool bIdentifier = false;

            status = m_IsdcRsApi.GetRawDataIsdcRs(data, m_IsdcRsApi.iInputOutputBufferSize, pnBytesReturned);
            wBarCodeSize = pnBytesReturned[0];

            if (status == 0)
            {
                // refresh barcode data display
                string s_tmp = "";
                for (int wCount = 0; wCount < wBarCodeSize; wCount++)
                {
                    if ((wCount == 0) && (data[wCount] == ']')) bIdentifier = true;
                    if ((wCount == (wBarCodeSize - 2)) && (data[wCount] == 0x0D)) data[wCount] = 0x00;
                    if ((wCount == (wBarCodeSize - 1)) && (data[wCount] == 0x0A)) data[wCount] = 0x00;
                    s_tmp += Convert.ToChar(data[wCount]);
                }
                msg = s_tmp;

                if ((bIdentifier) && (BarcodeIdentifierCode == 0)) msg = s_tmp.Remove(0, 3);

                if (IsDebugMode) Trace.WriteLine("Get Intermec RAW Data");

                PublicFunction.OutputKeyboardMessage(msg);// Winmate brian modify on 2018/04/18
            }
            return status;
        }

        public static byte IsdcRsApiInit()
        {
            byte status = 0;
            BarcodeType = Convert.ToUInt16(BarcodeType);
            if (IsDebugMode) Trace.WriteLine("IsdcRsApiInit() - Set BarcodeType : " + BarcodeType);

            if (BarcodeType == 2)
            {
                m_IsdcRsApi = new IsdcRsApi();

                // for windws 7 or windows 8
                status = m_IsdcRsApi.InitializeIsdcRs("HKCU\\SOFTWARE\\HotTab\\Intermec\n");
            }
            return status;
        }

        public void openPortSSI()
        {
            // Trace.WriteLine("openPortSSI");
            if (type == 5)
            {
                if (!isOpenSE4500DL)
                {
                    if (SsiDllApi.SSIConnect(ihw, 9600, iPortNumber) == 0)
                    {
                        isOpenSE4500DL = true;
                    }
                }
            }
        }

        public void closePortSSI()
        {
            // Trace.WriteLine("closePortSSI");
            if (type == 5)
            {
                if (isOpenSE4500DL)
                {
                    if (SsiDllApi.SSIDisconnect(ihw, iPortNumber) == 0)
                    {
                        isOpenSE4500DL = false;
                    }
                }
            }
        }

        public bool moto_write(byte CmdCode, byte[] Params, int ParamBytes)
        {
            byte[] SSIBuffer = new byte[SSI_MAX_PACKET_LENGTH];
            int Checksum;
            int i;

            SSIBuffer[0] = 0;
            SSIBuffer[1] = CmdCode;
            SSIBuffer[2] = SSI_SOURCE_HOST;
            SSIBuffer[3] = 0x08;	// 儲存設定值在模組上(0x08)

            i = 4;
            if (ParamBytes > 0)
            {
                for (int x = 0; x < ParamBytes; x++)
                {
                    SSIBuffer[i] = Params[x];
                    i++;
                }
            }

            SSIBuffer[0] = (byte)(SSI_HEADER_LEN + ParamBytes);

            Checksum = 0;
            for (i = 0; i < SSIBuffer[0]; i++)
                Checksum += SSIBuffer[i];

            i = SSI_HEADER_LEN + ParamBytes;
            SSIBuffer[i++] = (byte)(((-Checksum) >> 8) & 0xFF);
            SSIBuffer[i++] = (byte)((-Checksum) & 0xFF);

            mSerialPort.Write(SSIBuffer, 0, i);

            return true;
        }

        public static void GetDecodeData()
        {
            try
            {
                for (int i = 0; i < (SsiDllApi.MAX_LEN - 1); i++)
                    SsiDllApi.DecodeData[i] = SsiDllApi.DecodeData[i + 1];

                if (GetByteLen(SsiDllApi.DecodeData) > 0)
                {
                    SsiDllApi.DecodeBuffer = System.Text.Encoding.ASCII.GetString(SsiDllApi.DecodeData, 0, GetByteLen(SsiDllApi.DecodeData));

                    SsiDllApi.DecodeData = new byte[SsiDllApi.MAX_LEN];

                    string msg = SsiDllApi.DecodeBuffer;

                    if (IsDebugMode) Trace.WriteLine("Get Moto Decode Data");

                    PublicFunction.OutputKeyboardMessage(msg);// Winmate brian modify on 2018/04/18
                }
                else
                {
                    // Trace.WriteLine("winmate GetDecodeData len error");
                }
            }
            catch
            {
                // Trace.WriteLine("winmate GetDecodeData error.");
            }
        }

        public static void GetVersionData()
        {
            try
            {
                // for (int i = 0; i < (SsiDllApi.MAX_LEN - 1); i++)
                //    SsiDllApi.DecodeData[i] = SsiDllApi.DecodeData[i + 1];

                if (GetByteLen(SsiDllApi.DecodeData) > 0)
                {
                    SsiDllApi.DecodeBuffer = System.Text.ASCIIEncoding.ASCII.GetString(SsiDllApi.DecodeData, 0, GetByteLen(SsiDllApi.DecodeData));

                    SsiDllApi.DecodeData = new byte[SsiDllApi.MAX_LEN];

                    char[] delimiterChars = { ' ' };
                    string[] words = SsiDllApi.DecodeBuffer.Split(delimiterChars);

                    se4500dlVersion = words[0];

                }
                else
                {
                    // Trace.WriteLine("winmate GetVersionData len error");
                }
            }
            catch
            {
                // Trace.WriteLine("winmate GetVersionData error.");
            }
        }

        public byte WriteTriggerTimeout(byte time)
        {
            // Trace.WriteLine("WriteTriggerTimeout=" + time.ToString());

            if (type == 5)
            {
                openPortSSI();
                if (isOpenSE4500DL)
                {
                    int iRet = -1;

                    if (time > 9)
                        time = 9;

                    time = (byte)(time * 10);

                    byte[] data = { 0x88, time };

                    iRet = SsiDllApi.SetParameters(data, data.Length, iPortNumber);

                    // Trace.WriteLine("WriteTriggerTimeout Ret=" + iRet.ToString());

                    Thread.Sleep(100);
                }
            }

            return 0;
        }

        private static int GetByteLen(byte[] array)
        {
            int iCount = 0;
            int iLen = 0;

            for (iCount = 0; iCount < array.Length; iCount++)
            {
                if (array[iCount] == 0)
                    return iLen;
                else
                    iLen++;

            }

            return iLen;
        }
    }
}
