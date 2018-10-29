using HotTabFunction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace barcode
{
    public class PublicFunction : IDisposable
    {
        #region Definition
        private IntPtr handle;                          // Pointer to an external unmanaged resource.
        private Component component = new Component();  // Other managed resource this class uses.
        private bool disposed = false;                  // Track whether Dispose has been called.

        internal static string BaseBoardInfo = "";
        internal static string ProcessorInformation = "";
        internal static string OSInformation = "";
        static bool IsDebugMode = true;
        public static string PortAddressRTS = "COM11";
        static string PortAddressExtend = "COM20";
        static string scannerAlertPath = Path.Combine(Application.StartupPath, "WinmateScannerAlert.wav");


        private ManagementScope managementScope = new ManagementScope
        {
            Path = new ManagementPath(@"\\" + Environment.MachineName + @"\root\CIMV2"),
        };

        private static readonly string USB_RTS_Comport_SN = "RTS"; // 0BTDKRTS / 0HHVDRTS

        private static SelectQuery PnPEntityQuery = new SelectQuery("SELECT * FROM Win32_PnPEntity");
        // private static SelectQuery mPnPEntityQuery = new SelectQuery("SELECT * FROM Win32_PnPEntity");
        // private static SelectQuery PnPEntityQuery
        // {
        //     get
        //     {
        //         try
        //         {
        //             mPnPEntityQuery = null;
        //             if (mPnPEntityQuery == null) mPnPEntityQuery = new SelectQuery("SELECT * FROM Win32_PnPEntity");
        //             return mPnPEntityQuery;
        //         }
        //         catch (Exception ex)
        //         {
        //             Trace.WriteLine("PnPEntityQuery Exception : " + ex.Message);
        //             return new SelectQuery("SELECT * FROM Win32_PnPEntity");
        //         }
        //     }
        // }

        internal static ManagementObjectSearcher PnPEntitySearcher = new ManagementObjectSearcher(PnPEntityQuery);
        // internal static ManagementObjectSearcher mPnPEntitySearcher = new ManagementObjectSearcher(PnPEntityQuery);
        // internal static ManagementObjectSearcher PnPEntitySearcher
        // {
        //     get
        //     {
        //         try
        //         {
        //             mPnPEntitySearcher = null;
        //             if (mPnPEntitySearcher == null) mPnPEntitySearcher = new ManagementObjectSearcher(PnPEntityQuery);
        //             return mPnPEntitySearcher;
        //         }
        //         catch (Exception ex)
        //         {
        //             Trace.WriteLine("PnPEntitySearcher Exception : " + ex.Message);
        //             return new ManagementObjectSearcher(PnPEntityQuery);
        //         }
        //     }
        // }

        private static SerialPort serialPortRTS; // 用來控制SerialPort.RtsEnable的Enable / Disable 狀態, 得以控制AC狀態跟LED燈
        internal static SerialPort SerialPortRTS
        {
            get
            {
                try
                {
                    if (serialPortRTS == null)
                    {
                        // if (IsDebugMode) Trace.WriteLine("serialPortRTS == null");
                        serialPortRTS = new SerialPort();
                    }

                    if (!serialPortRTS.IsOpen)
                    {
                        serialPortRTS.PortName = PortAddressRTS;

                        // Trace.WriteLine("PnPEntitySearcher : " + (PnPEntitySearcher == null));
                        foreach (ManagementObject currentDevice in PnPEntitySearcher.Get())
                        {
                            if (currentDevice == null || currentDevice["Name"] == null || currentDevice["PNPDeviceID"] == null) continue;
                            if (currentDevice["Name"].ToString().Contains("USB Serial Port")) Trace.WriteLine("FTDI SerialPort : " + currentDevice["Name"].ToString() + "\tPNPDeviceID : " + currentDevice["PNPDeviceID"].ToString());
                            if (currentDevice["PNPDeviceID"].ToString().Contains(USB_RTS_Comport_SN) && currentDevice["Name"].ToString().Contains("USB Serial Port"))
                            {
                                string DeviceName = currentDevice["Name"].ToString();
                                Trace.WriteLine("Set RTS SerialPort Name : " + DeviceName/* + "\tLength : " + DeviceName.Length*/);
                                // Trace.WriteLine(DeviceName.IndexOf("COM") + "" + DeviceName.IndexOf(")"));
                                serialPortRTS.PortName = DeviceName.Substring(DeviceName.IndexOf("COM"), (DeviceName.IndexOf(")") - DeviceName.IndexOf("COM")));
                                PortAddressRTS = serialPortRTS.PortName;
                            }
                        }

                        Trace.WriteLine("Open RTS SerialPort : " + serialPortRTS.PortName);
                        serialPortRTS.Open();
                    }
                    return serialPortRTS;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("RTS Port Exception : " + ex.Message);
                    return null;
                }
            }
        }

        private static readonly string DeviceID_M6E = "VID_2008";
        private static SerialPort serialPortExtend;
        internal static SerialPort SerialPortExtend
        {
            get
            {
                try
                {
                    if (serialPortExtend == null)
                    {
                        serialPortExtend = new SerialPort();
                    }

                    if (!serialPortExtend.IsOpen)
                    {
                        serialPortExtend.PortName = PortAddressExtend;

                        foreach (ManagementObject currentDevice in PnPEntitySearcher.Get())
                        {
                            if (currentDevice["PNPDeviceID"].ToString().Contains(DeviceID_M6E) && currentDevice["Name"].ToString().Contains("USB Serial Device"))
                            {
                                string DeviceName = currentDevice["Name"].ToString();
                                Trace.WriteLine("Set Extend SerialPort Name : " + DeviceName + "\tLength : (" + DeviceName.Length + ")");
                                serialPortExtend.PortName = DeviceName.Substring(DeviceName.IndexOf("COM"), (DeviceName.IndexOf(")") - DeviceName.IndexOf("COM")));
                                PortAddressExtend = serialPortExtend.PortName;
                            }
                        }

                        serialPortExtend.Open();
                        Trace.WriteLine("Open Extend SerialPort : " + PortAddressExtend);
                    }
                    return serialPortExtend;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ExtendPort Exception : " + ex.Message);
                    return null;
                }
            }
        }

        private const string VID_Honeywell = "VID_0C2E";
        private const string DeviceID_Honeywell = "VID_0C2E&PID_0ECF";


        private static int RetryCount;
        #endregion

        #region DllImport
        // Use interop to call the method necessary to clean up the unmanaged resource.
        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);
        //取得flag，以判斷是否在 WOW64 之下執行的處理程序。
        [DllImport("kernel32")]
        private extern static bool IsWow64Process(IntPtr hProcess, out bool isWow64);
        [DllImport("kernel32")]
        private extern static IntPtr GetCurrentProcess();
        [DllImport("kernel32")]
        private extern static IntPtr GetModuleHandle(string moduleName);
        [DllImport("kernel32")]
        private extern static IntPtr GetProcAddress(IntPtr hModule, string methodName);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        #endregion

        #region Constructor
        // The class constructor.
        public PublicFunction()
        {
        }

        // The class constructor.
        public PublicFunction(IntPtr handle)
        {
            this.handle = handle;
        }
        #endregion

        #region Battery
        private static int isChargingNow = -1;
        public static bool IsCharging
        {
            get
            {
                // AC_IN=119, AC_OUT=82
                // HotTabDLL.GetACInOutStatus(out isChargingNow);

                //winmate brian 20180515 modify >>>
                uint ivalue = 0;
                if (HotTabDLL.GetACStatus(out ivalue))
                {
                    //get ac status success
                    isChargingNow = (int)ivalue;
                }
                else
                {
                    //get ac status fail
                    ivalue = 0;
                    isChargingNow = (int)ivalue;
                }
                //winmate brian 20180515 modify <<<

                if (IsDebugMode) Trace.WriteLine("AC In/Out Status : " + (0x01 & isChargingNow));
                // 跟0x01做AND運算, 等於只會取出最後一個bit的值
                return (0x01 & isChargingNow).Equals(1);
            }
        }

        private static int battery1Current = -1;
        public static int Battery1Current
        {
            get
            {
                HotTabDLL.GetBattery1Current(out battery1Current);
                if (IsDebugMode) Trace.WriteLine("Main(Inner) BAT Current".PadRight(30, ' ') + battery1Current);
                return battery1Current;
            }
        }

        private static int battery2Current = -1;
        public static int Battery2Current
        {
            get
            {
                HotTabDLL.GetBattery2Current(out battery2Current);
                if (IsDebugMode) Trace.WriteLine("Second(Extend) BAT Current".PadRight(30, ' ') + battery1Current);
                return battery2Current;
            }
        }

        private static byte BatteryCurrentDischarge = 0x00;
        public static bool IsExtendBatteryUsed
        {
            get
            {
                HotTabDLL.GetBatteryCurrentDischarge(out BatteryCurrentDischarge);
                if (IsDebugMode)
                {
                    Trace.WriteLine("IsExtendBatteryUsed : " + BatteryCurrentDischarge);
                }
                return BatteryCurrentDischarge.Equals(0x02);
            }
        }

        public static void SwitchToInternalBattery()
        {
            if (IsExtendBatteryUsed)
            {
                HotTabDLL.WinIO_SetHotSwap();
                if (IsDebugMode)
                {
                    Trace.WriteLine("Hotswap to Inner battery.");
                }
            }
        }

        public static void SwitchToExtendBattery()
        {
            if (!IsExtendBatteryUsed)
            {
                HotTabDLL.WinIO_SetHotSwap();
                if (IsDebugMode)
                {
                    Trace.WriteLine("Hotswap to Extend battery.");
                }
            }
        }
        #endregion

        /// <summary> 
        /// 產生不重複的亂數 
        /// </summary> 
        /// <param name="intLower"></param>產生亂數的範圍下限 
        /// <param name="intUpper"></param>產生亂數的範圍上限 
        /// <param name="intNum"></param>產生亂數的數量 
        /// <returns></returns> 
        internal static List<int> mRandom = null; // 用來儲存亂數序列, 給隨機讀取測試音效檔的時候用
        internal static List<int> MakeRand(int intLower, int intUpper, int intNum)
        {
            List<int> listRand = new List<int>();
            Random random = new Random((int)DateTime.Now.Ticks);
            int intRnd;
            while (listRand.Count < intNum)
            {
                intRnd = random.Next(intLower, intUpper + 1);
                if (!listRand.Contains(intRnd))
                {
                    listRand.Add(intRnd);
                }
            }
            return listRand;
        }

        internal static int SelectedAudioName = 0;

        internal static void CloseRTSPort()
        {
            if (SerialPortRTS == null)
                return;

            if (SerialPortRTS.IsOpen)
                SerialPortRTS.Close();
        }

        internal static bool EnableRTS()
        {
            if (SerialPortRTS == null)
            {
                Trace.WriteLine("EnableRTS SerialPortRTS == null");
                return false;
            }

            if (!SerialPortRTS.IsOpen)
                SerialPortRTS.Open();

            if (!SerialPortRTS.RtsEnable)
            {
                SerialPortRTS.RtsEnable = true; // RTS enable 斷電
                Trace.WriteLine("Set RTS Enable.");
                Thread.Sleep(300);
            }
            else return true;

            RetryCount = 0;
            while (IsCharging)
            {
                RetryCount++;
                if (RetryCount > 20)
                {
                    return false;
                }
                else
                {
                    Thread.Sleep(300);
                    Trace.WriteLine("Can't switch RTS Pin to Enable (AC Out).");
                    SerialPortRTS.RtsEnable = true; // RTS enable 斷電
                }
            }
            return true;
        }

        internal static bool DisableRTS()
        {
            if (SerialPortRTS == null)
            {
                Trace.WriteLine("DisableRTS SerialPortRTS == null");
                return false;
            }

            if (!SerialPortRTS.IsOpen)
                SerialPortRTS.Open();

            if (SerialPortRTS.RtsEnable)
            {
                SerialPortRTS.RtsEnable = false; // RTS disable 供電
                Trace.WriteLine("Set RTS Disable.");
                Thread.Sleep(300);
            }
            else
            {
                CloseRTSPort();
                return true;
            }

            RetryCount = 0;
            while (!IsCharging)
            {
                RetryCount++;
                if (RetryCount > 20)
                {
                    CloseRTSPort();
                    return false;
                }
                else
                {
                    Thread.Sleep(300);
                    Trace.WriteLine("Can't switch RTS Pin to Disable (AC In).");
                    SerialPortRTS.RtsEnable = false; // RTS disable 供電
                }
            }
            CloseRTSPort();
            return true;
        }

        internal static IntPtr MessageBoxPointer; // 用來找有沒有測試程式叫起來的MessageBox
        internal static bool IsMessageBoxExisted()
        {
            // 依MessageBox的標題, 找出MessageBox的視窗
            MessageBoxPointer = FindWindow(null, "Attention");

            //if (IsDebugMode) Trace.WriteLine("IsMessageBoxExisted : " + MessageBoxPointer);

            // 只要 ptr 不為 0 的時候, 就代表有測試程式叫起來的MessageBox
            return !MessageBoxPointer.Equals(IntPtr.Zero);
        }

        internal static bool CloseSpecificProcess(string specificProcess)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(specificProcess))
                { 
                    clsProcess.Kill();
                    return true;
                }
            }
            return false;
        }

        internal static uint GetGPSModuleType()
        {
            foreach (ManagementObject currenAdapter in PnPEntitySearcher.Get())
            {
                // Trace.WriteLine(currenAdapter["DeviceID"].ToString());
                if (currenAdapter["DeviceID"].ToString().Contains("VID_1546&PID_01A6"))
                {
                    Trace.WriteLine("GPS : u-blox 6");
                    return 1;
                }
                else if (currenAdapter["DeviceID"].ToString().Contains("VID_1546&PID_01A8"))
                {
                    Trace.WriteLine("GPS : u-blox M8N");
                    return 2;
                }
            }
            Trace.WriteLine("GPS : No GPS module");
            return 0;
        }

        internal static int TimeoutThresholdFactor(int TimerInterval)
        {
            return ((1000 / TimerInterval) > 0) ? (1000 / TimerInterval) : 1;
        }

        #region Scanner
        public static int SingleTriggerCount = 0;   // 目前Scanner掃描次數
        public static int TriggerThreshold = 5;     // 單次掃描掃不到Tag的retry上限次數
        public static void PlayScannerAlertAudio()
        {
            if (File.Exists(scannerAlertPath))
            {
                using (SoundPlayer mSoundPlayer = new SoundPlayer(scannerAlertPath))
                {
                    if (IsDebugMode)
                    {
                        Trace.WriteLine("Play Alert Sound.");
                    }
                    mSoundPlayer.Play();
                }
            }
            else if (IsDebugMode)
            {
                Trace.WriteLine("Alert Path (" + scannerAlertPath + ") is NOT exist!");
            }
        }
        #endregion

        // Winmate brian add on 2018/04/18 Start
        internal static void OutputKeyboardMessage(string msg)
        {
            PlayScannerAlertAudio();

            SendTagString(msg);
        }

        #region P/Invokes
        struct INPUT
        {
            public INPUTType type;
            public INPUTUnion Event;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct INPUTUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public KEYEVENTF dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        enum INPUTType : uint
        {
            INPUT_KEYBOARD = 1
        }

        [Flags]
        enum KEYEVENTF : uint
        {
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt32 SendInput(int numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

        public static void SendTagString(string s)
        {
            foreach (var c in s)
            {
                SendTagChars(c);
            }
        }

        private static void SendTagChars(char c)
        {
            SendKeyInternal((short)c);
        }

        private static void SendKeyInternal(short key)
        {
            // create input events as unicode with first down, then up
            INPUT[] inputs = new INPUT[2];
            inputs[0].type = inputs[1].type = INPUTType.INPUT_KEYBOARD;
            inputs[0].Event.ki.dwFlags = inputs[1].Event.ki.dwFlags = KEYEVENTF.UNICODE;
            inputs[0].Event.ki.wScan = inputs[1].Event.ki.wScan = key;
            inputs[1].Event.ki.dwFlags |= KEYEVENTF.KEYUP;

            uint cSuccess = SendInput(inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
            if (cSuccess != inputs.Length)
            {
                throw new Win32Exception();
            }
        }
        #endregion
        // Winmate brian add on 2018/04/18 End

        #region Implement IDisposable
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.
            // SupressFinalize to take this object off the finalization queue and prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
        // If disposing equals false, the method has been called by the runtime from inside the finalizer and you should not reference other objects. 
        // Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing) component.Dispose(); // Dispose managed resources.

                // Call the appropriate methods to clean up unmanaged resources here.
                // If disposing is false, only the following code is executed.
                CloseHandle(handle);
                handle = IntPtr.Zero;

                disposed = true; // Note disposing has been done.
            }
        }

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~PublicFunction()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of readability and maintainability.
            Dispose(false);
        }
        #endregion

        #region Serial Number
        public static String AutoTestItemPass = ""; // 儲存自動測項中PASS的測項名稱
        public static String AutoTestItemFail = ""; // 儲存自動測項中FAIL的測項名稱
        public static void AddTestNameToCurrentAlarmReport(bool IsPass, string name)
        {
            if (IsPass && (AutoTestItemPass.IndexOf(name) < 0))
            {
                AutoTestItemPass += name + " , ";
                if (AutoTestItemFail.IndexOf(name) > 0) AutoTestItemFail.Replace(name + " , ", string.Empty);
            }
            else if (!IsPass && (AutoTestItemFail.IndexOf(name) < 0))
            {
                AutoTestItemFail += name + " , ";
                if (AutoTestItemPass.IndexOf(name) > 0) AutoTestItemPass.Replace(name + " , ", string.Empty);
            }
            else
            {
                Trace.WriteLine(name + " still test failed or passed."); // 測項本次的測試結果跟上次的結果相同(一樣都是FAIL或是PASS)
            }
        }
        #endregion

        #region WMI
        private static string Product;
        // private string Manufacturer;
        // private string Name;
        // private string SerialNumber;
        // private string Version;
        public static string GetMotherBoardInfo()
        {
            try
            {
                // ManagementObjectSearcher mBaseboard = new ManagementObjectSearcher("select * from Win32_baseboard");
                // foreach (ManagementObject BaseboardInfo in mBaseboard.Get())
                // {
                //     if (IsDebugMode) Trace.Write("ProductName : " + BaseboardInfo["Product"]);
                // }

                ManagementClass mc = new ManagementClass("Win32_BaseBoard");
                ManagementObjectCollection mBaseboard = mc.GetInstances();
                foreach (ManagementObject mo in mBaseboard)
                {
                    Product = mo.Properties["Product"].Value.ToString();
                    if (IsDebugMode)
                    {
                        // Trace.Write("CreationClassName : " + mo.Properties["CreationClassName"].Value.ToString() + "\r\n"); // Win32_BaseBoard
                        // Trace.Write("Caption : " + mo.Properties["Caption"].Value.ToString() + "\r\n"); // Base Board
                        // Trace.Write("Name : " + mo.Properties["Name"].Value.ToString() + "\r\n"); // Base Board
                        // Trace.Write("Description : " + mo.Properties["Description"].Value.ToString() + "\r\n"); // Base Board
                        // Trace.Write("Tag : " + mo.Properties["Tag"].Value.ToString() + "\r\n"); // Base Board
                        Trace.Write("Product : " + mo.Properties["Product"].Value.ToString() + "\r\n"); // H81M-DS2
                        Trace.Write("SerialNumber : " + mo.Properties["SerialNumber"].Value.ToString() + "\r\n"); // To be filled by O.E.M.
                        Trace.Write("Manufacturer : " + mo.Properties["Manufacturer"].Value.ToString() + "\r\n"); // Gigabyte Technology Co., Ltd.
                        // Trace.Write("Status : " + mo.Properties["Status"].Value.ToString() + "\r\n"); // OK
                        // Trace.Write("Version : " + mo.Properties["Version"].Value.ToString() + "\r\n"); // x.x
                    }
                }
                return Product;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Unexpected Error : " + ex.Message);
                return "Unknown\r\n";
            }
        }

        #region #Base Board Information# Win32_ComputerSystemProduct class
        /// <summary>
        /// https:// msdn.microsoft.com/en-us/library/windows/desktop/aa394072(v=vs.85).aspx
        /// </summary>
        /// <returns></returns>
        public void GetBaseBoardInfo()
        {
            SelectQuery selectQuery = new SelectQuery("SELECT * FROM Win32_BaseBoard");
            ManagementObjectSearcher managementObjectSearch = new ManagementObjectSearcher(managementScope, selectQuery);
            ManagementObjectCollection moCollection = managementObjectSearch.Get();

            foreach (ManagementObject mObject in moCollection)
            {
                if (IsDebugMode)
                {
                    // Trace.WriteLine("Win32_BaseBoard CreationClassName : " + mObject["CreationClassName"].ToString()); // Win32_BaseBoard
                    // Trace.WriteLine("Win32_BaseBoard Caption : " + mObject["Caption"].ToString()); // Base Board
                    // Trace.WriteLine("Win32_BaseBoard Description : " + mObject["Description"].ToString()); // Base Board
                    // Trace.WriteLine("Win32_BaseBoard Tag : " + mObject["Tag"].ToString()); // Base Board

                    BaseBoardInfo += "#Base Board Information#" + "\r\n";
                    BaseBoardInfo += "Manufacturer : " + mObject["Manufacturer"].ToString() + "\r\n"; // Gigabyte Technology Co., Ltd.
                    BaseBoardInfo += "Name : " + mObject["Name"].ToString() + "\r\n"; // Base Board
                    BaseBoardInfo += "Product : " + mObject["Product"].ToString() + "\r\n"; // H81M-DS2
                    BaseBoardInfo += "SerialNumber : " + mObject["SerialNumber"].ToString() + "\r\n"; // To be filled by O.E.M.
                    BaseBoardInfo += "Status : " + mObject["Status"].ToString() + "\r\n"; // OK
                    BaseBoardInfo += "Version : " + mObject["Version"].ToString() + "\r\n"; // x.x

                    // Not Support
                    // Trace.WriteLine("Win32_BaseBoard Model : " + mObject["Model"].ToString());
                    // Trace.WriteLine("Win32_BaseBoard OtherIdentifyingInfo : " + mObject["OtherIdentifyingInfo"].ToString());
                    // Trace.WriteLine("Win32_BaseBoard PartNumber : " + mObject["PartNumber"].ToString());
                    // Trace.WriteLine("Win32_BaseBoard RequirementsDescription : " + mObject["RequirementsDescription"].ToString());
                    // Trace.WriteLine("Win32_BaseBoard SKU : " + mObject["SKU"].ToString());
                    // Trace.WriteLine("Win32_BaseBoard SlotLayout : " + mObject["SlotLayout"].ToString());
                }
            }
        }
        #endregion

        #region #CPU Information# Win32_Processor class
        // Winmate Kenkun modify on 2016/03/21 >>
        public String GetProcessorInformation()
        {
            // ManagementObjectSearcher 類別 : 根據指定的查詢擷取管理物件的集合
            // 透過查詢語法 SELECT * FROM Win32_Processor 取得所有 Win32_Processor 類別資料
            // 可參考 http://msdn.microsoft.com/en-us/library/aa394373(VS.85).aspx
            // 其中 CPU 型號為 ProcessorName

            List<CPUInfoEntity> cpuInfoList = new List<CPUInfoEntity>();
            ManagementObjectCollection moCollection = null;

            String ProcessorName = "";

            // 設定通過WMI要查詢的內容
            ObjectQuery selectQuery = new ObjectQuery("select * from Win32_Processor");
            // WQL語句, 設定的WMI查詢內容和WMI的操作範圍, 檢索WMI對象集合
            ManagementObjectSearcher managementObjectSearch = new ManagementObjectSearcher(managementScope, selectQuery);
            // 非同步調用WMI查詢
            moCollection = managementObjectSearch.Get();

            foreach (ManagementObject mObject in moCollection)
            {
                ProcessorName = (string)mObject["Name"];
                Trace.WriteLine("Win32_Processor Name : " + ProcessorName);
                if (IsDebugMode)
                {
                    ProcessorInformation += "#CPU Information#" + "\r\n";
                    // ProcessorInformation += "SystemName : " + mObject["SystemName"].ToString() + "\r\n";
                    ProcessorInformation += "Name : " + mObject["Name"].ToString() + "\r\n";
                    ProcessorInformation += "Manufacturer : " + mObject["Manufacturer"].ToString() + "\r\n";
                    // ProcessorInformation += "CreationClassName : " + mObject["CreationClassName"].ToString() + "\r\n";
                    // ProcessorInformation += "Caption : " + mObject["Caption"].ToString() + "\r\n";
                    // ProcessorInformation += "Description : " + mObject["Description"].ToString() + "\r\n";
                    ProcessorInformation += "ProcessorId : " + mObject["ProcessorId"].ToString() + "\r\n";
                    // ProcessorInformation += "DeviceID : " + mObject["DeviceID"].ToString() + "\r\n";
                    // ProcessorInformation += "SocketDesignation : " + mObject["SocketDesignation"].ToString() + "\r\n";
                    // ProcessorInformation += "Role : " + mObject["Role"].ToString() + "\r\n";
                    // ProcessorInformation += "Status : " + mObject["Status"].ToString() + "\r\n";
                    // ProcessorInformation += "SystemCreationClassName : " + mObject["SystemCreationClassName"].ToString() + "\r\n";
                    // ProcessorInformation += "Version : " + mObject["Version"].ToString() + "\r\n";
                    ProcessorInformation += "CurrentClockSpeed : " + mObject["CurrentClockSpeed"].ToString() + "\r\n";

                    /*
                    Trace.WriteLine("#CPU Information# Win32_Processor class : ");
                    Trace.WriteLine("SystemName : " + mObject["SystemName"].ToString()); // WE0930
                    Trace.WriteLine("Name : " + mObject["Name"].ToString());// Intel(R) Core(TM) i5-4460  CPU @ 3.20GHz
                    Trace.WriteLine("Manufacturer : " + mObject["Manufacturer"].ToString());// GenuineIntel
                    Trace.WriteLine("CreationClassName : " + mObject["CreationClassName"].ToString());// Win32_Processor
                    Trace.WriteLine("Caption : " + mObject["Caption"].ToString());// Intel64 Family 6 Model 60 Stepping 3
                    Trace.WriteLine("Description : " + mObject["Description"].ToString());// Intel64 Family 6 Model 60 Stepping 3
                    Trace.WriteLine("ProcessorId : " + mObject["ProcessorId"].ToString());// BFEBFBFF000306C3
                    Trace.WriteLine("DeviceID : " + mObject["DeviceID"].ToString());// CPU0
                    Trace.WriteLine("SocketDesignation : " + mObject["SocketDesignation"].ToString());// SOCKET 0
                    Trace.WriteLine("Role : " + mObject["Role"].ToString());// CPU
                    Trace.WriteLine("Status : " + mObject["Status"].ToString());// OK
                    Trace.WriteLine("SystemCreationClassName : " + mObject["SystemCreationClassName"].ToString());// Win32_ComputerSystem
                    Trace.WriteLine("Version : " + mObject["Version"].ToString());// 
                    Trace.WriteLine("CurrentClockSpeed : " + mObject["CurrentClockSpeed"].ToString());// 3201
                    */

                    // Not Support
                    // ProcessorInformation += "AssetTag : " + mObject["AssetTag"].ToString() + "\r\n";
                    // ProcessorInformation += "ErrorDescription : " + mObject["ErrorDescription"].ToString() + "\r\n";
                    // ProcessorInformation += "OtherFamilyDescription : " + mObject["OtherFamilyDescription"].ToString() + "\r\n";
                    // ProcessorInformation += "PartNumber : " + mObject["PartNumber"].ToString() + "\r\n";
                    // ProcessorInformation += "PNPDeviceID : " + mObject["PNPDeviceID"].ToString() + "\r\n";
                    // ProcessorInformation += "SerialNumber : " + mObject["SerialNumber"].ToString() + "\r\n";
                    // ProcessorInformation += "Stepping : " + mObject["Stepping"].ToString() + "\r\n";
                    // ProcessorInformation += "UniqueId : " + mObject["UniqueId"].ToString() + "\r\n";

                    CPUInfoEntity cpuInfo = new CPUInfoEntity
                    {
                        CPUCount = moCollection.Count,
                        CPUMaxClockSpeed = mObject["MaxClockSpeed"] == null ? string.Empty : mObject["MaxClockSpeed"].ToString(), // 獲取最大時鐘頻率
                        CPUExtClock = mObject["ExtClock"] == null ? string.Empty : mObject["ExtClock"].ToString(), // 獲取外部頻率
                        CPUCurrentVoltage = mObject["CurrentVoltage"] == null ? string.Empty : mObject["CurrentVoltage"].ToString(), // 獲取當前電壓
                        CPUL2CacheSize = mObject["L2CacheSize"] == null ? string.Empty : mObject["L2CacheSize"].ToString(), // 獲取二級Cache
                        CPUDataWidth = mObject["DataWidth"] == null ? string.Empty : mObject["DataWidth"].ToString(), // 獲取DataWidth
                        CPUAddressWidth = mObject["AddressWidth"] == null ? string.Empty : mObject["AddressWidth"].ToString(), // 獲取AddressWidth
                        CPUNumberOfCores = mObject["NumberOfCores"] == null ? string.Empty : mObject["NumberOfCores"].ToString(), // 核心數
                        CPUNumberOfLogicalProcessors = mObject["NumberOfLogicalProcessors"] == null ? string.Empty : mObject["NumberOfLogicalProcessors"].ToString(),// 邏輯處理器
                        CPUUsedPercent = mObject["LoadPercentage"] == null ? 0 : float.Parse(mObject["LoadPercentage"].ToString())
                    };
                    cpuInfoList.Add(cpuInfo); // 加入進去
                }
            }

            #region 獲取 CPU 溫度
            Double CPUtprt = 0;
            System.Management.ManagementObjectSearcher mos = new System.Management.ManagementObjectSearcher(@"root\WMI", "Select * From MSAcpi_ThermalZoneTemperature");

            foreach (System.Management.ManagementObject mo in mos.Get())
            {
                CPUtprt = Convert.ToDouble(Convert.ToDouble(mo.GetPropertyValue("CurrentTemperature").ToString()) - 2732) / 10;
                if (IsDebugMode)
                {
                    Trace.WriteLine("CPU Temperature : " + CPUtprt.ToString() + " °C");
                }
            }
            #endregion

            if (ProcessorName.ToUpper().IndexOf("INTEL") != -1) ProcessorName = "INTEL";
            else if (ProcessorName.ToUpper().IndexOf("VIA") != -1) ProcessorName = "VIA";

            return ProcessorName;
        }
        #endregion

        #region #OS Informarion# Win32_OperatingSystem class
        public String GetOSInformation()
        {
            String OSName = "";

            SelectQuery selectQuery = new SelectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher managementObjectSearch = new ManagementObjectSearcher(managementScope, selectQuery);
            ManagementObjectCollection moCollection = managementObjectSearch.Get();

            foreach (ManagementObject mObject in moCollection)
            {
                OSName = (string)mObject["Name"];
                if (IsDebugMode)
                {
                    OSInformation += "#OS Informarion#" + "\r\n";
                    OSInformation += "Caption : " + mObject["Caption"].ToString() + "\r\n";
                    OSInformation += "Manufacturer : " + mObject["Manufacturer"].ToString() + "\r\n";
                    OSInformation += "CountryCode : " + mObject["CountryCode"].ToString() + "\r\n";
                    OSInformation += "CSName : " + mObject["CSName"].ToString() + "\r\n";
                    OSInformation += "WindowsDirectory : " + mObject["WindowsDirectory"].ToString() + "\r\n";
                    OSInformation += "SystemDirectory : " + mObject["SystemDirectory"].ToString() + "\r\n";
                    OSInformation += "BootDevice : " + mObject["BootDevice"].ToString() + "\r\n";
                    OSInformation += "Version : " + mObject["Version"].ToString() + "\r\n";
                    // OSInformation += "CSDVersion : " + mObject["CSDVersion"].ToString() + "\r\n";
                    OSInformation += "BuildNumber : " + mObject["BuildNumber"].ToString() + "\r\n";

                    OSInformation += "TotalVisibleMemorySize : " + ((ulong)mObject["TotalVisibleMemorySize"] / 1024.0 / 1024).ToString("#0.00") + "G" + "\r\n";
                    OSInformation += "FreePhysicalMemory : " + ((ulong)mObject["FreePhysicalMemory"] / 1024.0 / 1024).ToString("#0.00") + "G" + "\r\n";
                    OSInformation += "TotalVirtualMemorySize : " + ((ulong)mObject["TotalVirtualMemorySize"] / 1024.0 / 1024).ToString("#0.00") + "G" + "\r\n";
                    OSInformation += "FreeVirtualMemory : " + ((ulong)mObject["FreeVirtualMemory"] / 1024.0 / 1024).ToString("#0.00") + "G" + "\r\n";
                    OSInformation += "SizeStoredInPagingFiles : " + ((ulong)mObject["SizeStoredInPagingFiles"] / 1024.0 / 1024).ToString("#0.00") + "G" + "\r\n";

                    /*
                    Trace.WriteLine("#OS Informarion# Win32_OperatingSystem class : ");
                    Trace.WriteLine("Caption : " + mObject["Caption"].ToString()); // Microsoft Windows 7 專業版 
                    Trace.WriteLine("Manufacturer : " + mObject["Manufacturer"].ToString()); // Microsoft Corporation
                    Trace.WriteLine("CountryCode : " + mObject["CountryCode"].ToString()); // 886
                    Trace.WriteLine("CSName : " + mObject["CSName"].ToString()); // WE0930
                    Trace.WriteLine("WindowsDirectory : " + mObject["WindowsDirectory"].ToString()); // C:\Windows
                    Trace.WriteLine("SystemDirectory : " + mObject["SystemDirectory"].ToString()); // C:\Windows\system32
                    Trace.WriteLine("BootDevice : " + mObject["BootDevice"].ToString()); // \Device\HarddiskVolume1
                    Trace.WriteLine("Version : " + mObject["Version"].ToString()); // 6.1.7601
                    Trace.WriteLine("CSDVersion : " + mObject["CSDVersion"].ToString()); // Service Pack 1
                    Trace.WriteLine("BuildNumber : " + mObject["BuildNumber"].ToString()); // 7601

                    Trace.WriteLine("TotalVisibleMemorySize : " + ((ulong)mObject["TotalVisibleMemorySize"] / 1024.0 / 1024).ToString("#0.00") + "G"); // 獲取總實體記憶體
                    Trace.WriteLine("FreePhysicalMemory : " + ((ulong)mObject["FreePhysicalMemory"] / 1024.0 / 1024).ToString("#0.00") + "G"); // 獲取可用實體記憶體
                    Trace.WriteLine("TotalVirtualMemorySize : " + ((ulong)mObject["TotalVirtualMemorySize"] / 1024.0 / 1024).ToString("#0.00") + "G"); // 獲取總虛擬記憶體
                    Trace.WriteLine("FreeVirtualMemory : " + ((ulong)mObject["FreeVirtualMemory"] / 1024.0 / 1024).ToString("#0.00") + "G"); // 獲取可用虛擬記憶體
                    Trace.WriteLine("SizeStoredInPagingFiles : " + ((ulong)mObject["SizeStoredInPagingFiles"] / 1024.0 / 1024).ToString("#0.00") + "G"); // 獲取分頁檔大小
                    */
                }
            }
            return OSName;
        }
        #endregion
        #endregion

        #region 判斷作業系統是否為 64 位元
        // 判斷作業系統是否為 64 位元
        public static bool Is64BitOperatingSystem
        {
            get
            {
                // 當目前處理程序為 64 位元, 則可以判定作業系統一定是 64 位元
                if (IsDebugMode) Trace.WriteLine("Is64BitProcess : " + Is64BitProcess);
                if (Is64BitProcess) return true;

                // 當目前kernel32有提供IsWow64Process函數與目前處理程序處於Wow64的狀態下, 則判定作業系統為 64 位元. 
                bool isWow64 = false;
                IsWow64Process(GetCurrentProcess(), out isWow64);
                if (IsDebugMode) Trace.WriteLine("Is64BitOperatingSystem : " + isWow64);
                return ModuleContainsFunction("kernel32.dll", "IsWow64Process") && IsWow64Process(GetCurrentProcess(), out isWow64) && isWow64;
            }
        }

        // 判斷目前處理程序是否為 64 位元
        public static bool Is64BitProcess
        {
            get
            {
                // 透過系統的 Intptr 型別來進行簡易的判斷. 4 = 32 位元, 8 = 64 位元. 
                return IntPtr.Size == 8;
            }
        }

        private static bool ModuleContainsFunction(string moduleName, string methodName)
        {
            IntPtr hModule = GetModuleHandle(moduleName);
            if (hModule != IntPtr.Zero) return GetProcAddress(hModule, methodName) != IntPtr.Zero;
            return false;
        }
        #endregion

        #region 清除沒有使用的元件, 釋放記憶體
        private static void DisposeCurrentTest()
        {
            // Dispose()是明確地在「程式員呼叫」與「using語句區塊結束時」二種情況下被呼叫的
            // 不可預期Finalize()究竟會在程式中的何處被呼叫, 只能被動地等待GC去呼叫它
            // 對於unmanaged resource 的釋放不要依賴 GC 來處理, 特別是 file handle 或者 database connection, 才用 Dispose/Finalize

            // Winmate Kenkun comment on 2014/07/18
            // Determine the maximum number of generations the system garbage collector currently supports.
            if (IsDebugMode) Trace.WriteLine("The highest generation is : " + GC.MaxGeneration);
            // Console.WriteLine("The highest generation is {0}", GC.MaxGeneration);
            // Determine the best available approximation of the number of bytes currently allocated in managed memory.
            if (IsDebugMode) Trace.WriteLine("Memory used before collection : " + GC.GetTotalMemory(false));
            // Console.WriteLine("Memory used before collection: {0:N0}", GC.GetTotalMemory(false));
            // Collect all generations of memory. 
            GC.Collect();
            // Perform a collection of generation 0 only.
            // GC.Collect(0);
            if (IsDebugMode) Trace.WriteLine("Memory used after full collection : " + GC.GetTotalMemory(true));
            // Console.WriteLine("Memory used after full collection: {0:N0}", GC.GetTotalMemory(true));
        }
        #endregion

        #region 針對可移除裝置進行新增檔案後刪除的讀寫測試
        static string[] USBFlash = new string[255];
        static String SourceFileName = "";
        public static int IsRemovableTransferred = 0;

        public static int RemovableDeviceTest(String path)
        {
            String str = "WinmateUSBTest";
            String sLine = "";
            FileStream fs = null;

            fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.WriteLine(str);
            sw.Close();
            fs.Close();

            fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            while ((sLine = sr.ReadLine()) != null)
            {
                if (sLine.Equals(str))
                {
                    Trace.WriteLine("File transfer success.");
                    // UpdateUSBDeviceDetails("File transfer success.");
                    IsRemovableTransferred++;
                }
                else
                {
                    Trace.WriteLine("An error occured during file transfer.");
                    // UpdateUSBDeviceDetails("An error occured during file transfer.");
                }
            }
            sr.Close();
            fs.Close();

            if (File.Exists(SourceFileName)) File.Delete(SourceFileName);
            return IsRemovableTransferred;
        }
        #endregion
    }
}