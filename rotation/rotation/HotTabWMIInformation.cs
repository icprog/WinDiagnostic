using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace rotation
{
    // 主要用途為Rotation和Information
    class HotTabWMIInformation
    {
        private static ConnectionOptions connectionOptions;
        private static ManagementScope managementScope;
        bool IsDebugMode = true;
        string SmbiosProductName = "";

        public HotTabWMIInformation()
        {
            InitializeConnectionOptions();
            InitializeManagementScope();
        }

        private void InitializeConnectionOptions()
        {
            connectionOptions = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.Default,
                EnablePrivileges = true
            };
        }

        private void InitializeManagementScope()
        {
            managementScope = new ManagementScope
            {
                Path = new ManagementPath(@"\\" + Environment.MachineName + @"\root\CIMV2"),
                Options = connectionOptions
            };
        }

        #region #BIOS Informarion# Win32_BIOS class
        /// <summary>
        /// https:// msdn.microsoft.com/en-us/library/windows/desktop/aa394077(v=vs.85).aspx
        /// </summary>
        /// <returns></returns>
        // Winmate Kenkun modify on 2016/03/22 >>
        public string GetWMI_BIOSVersion()
        {
            bool bflag = false;
            int iCount = 0;
            String SMBIOSBIOSVersion = "";
            String BIOSReleaseDate = "";
            String BIOManufacturer = "";

            try
            {
                while ((!bflag) && (iCount < 15))
                {
                    iCount++;

                    SelectQuery selectQuery = new SelectQuery("SELECT * FROM Win32_BIOS");
                    ManagementObjectSearcher managementObjectSearch = new ManagementObjectSearcher(managementScope, selectQuery);
                    ManagementObjectCollection managementObjectCollection = managementObjectSearch.Get();

                    foreach (ManagementObject managementObject in managementObjectCollection)
                    {
                        BIOSReleaseDate = managementObject["ReleaseDate"] == null ? string.Empty : ManagementDateTimeConverter.ToDateTime(managementObject["ReleaseDate"].ToString()).ToString();  // 時間
                        BIOManufacturer = managementObject["Manufacturer"].ToString();  // Manufacturer
                        SMBIOSBIOSVersion = (string)managementObject["SMBIOSBIOSVersion"];
                    }

                    if (IsDebugMode) Trace.WriteLine("Win32_BIOS ReleaseDate : " + BIOSReleaseDate + " , Manufacturer : " + BIOManufacturer + " , Version : " + SMBIOSBIOSVersion + "\r\n");
                    bflag = true;
                }

                // 當PPC系列的SMBIOS走的規範是舊的2.x (EX: IV70), 就需要將BIOS版本的欄位值改去抓BaseBoard的Version
                // BIOS燒錄工具燒錄SMBIOS_BIOSVersion, 格式不統一, M101B為3碼(EX:137), ID82/IB10X為4碼(EX:V206)
                if (!SMBIOSBIOSVersion.Length.Equals(5) && !SMBIOSBIOSVersion.Length.Equals(4) && !SMBIOSBIOSVersion.Length.Equals(3))
                {
                    SelectQuery selectQuery = new SelectQuery("SELECT * FROM Win32_BaseBoard");
                    ManagementObjectSearcher managementObjectSearch = new ManagementObjectSearcher(managementScope, selectQuery);
                    ManagementObjectCollection moCollection = managementObjectSearch.Get();

                    foreach (ManagementObject mObject in moCollection)
                    {
                        SMBIOSBIOSVersion = mObject["Version"].ToString();
                    }

                    if (IsDebugMode)
                    {
                        Trace.WriteLine("Product is not support SMBIOS 3.0 spec. Read BaseBoard Version.");
                    }
                }
            }
            catch (Exception ex)
            {
                SMBIOSBIOSVersion = "Unknown";
                throw ex;
            }

            SMBIOSBIOSVersion = Regex.Replace(SMBIOSBIOSVersion, @"[\W_a-zA-Z]+", ""); // 過濾特殊字元跟字母, 只留數字

            return SMBIOSBIOSVersion;
        }
        // Winmate Kenkun modify on 2016/03/22 <<

        public static String GetWMI_BIOSSerialNumber()
        {
            String tempBIOSSerialNumber = "";

            SelectQuery selectQuery = new SelectQuery("SELECT * FROM Win32_BIOS");
            ManagementObjectSearcher managementObjectSearch = new ManagementObjectSearcher(managementScope, selectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearch.Get();

            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                tempBIOSSerialNumber = (string)managementObject["SerialNumber"];
            }

            return tempBIOSSerialNumber;
        }
        #endregion

        #region #System Information# Win32_ComputerSystemProduct class
        /// <summary>
        /// https:// msdn.microsoft.com/en-us/library/windows/desktop/aa394105(v=vs.85).aspx
        /// </summary>
        /// <returns></returns>
        public string GetWMI_BIOSMainBoard()
        {
            SelectQuery selectQuery = new SelectQuery("SELECT * FROM Win32_ComputerSystemProduct");
            ManagementObjectSearcher managementObjectSearch = new ManagementObjectSearcher(managementScope, selectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearch.Get();

            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                SmbiosProductName = (string)managementObject["Name"];
                if (IsDebugMode)
                {
                    /*
                    Trace.WriteLine("ComputerSystemProduct Caption : " + managementObject["Caption"].ToString());
                    Trace.WriteLine("ComputerSystemProduct Description : " + managementObject["Description"].ToString());
                    Trace.WriteLine("ComputerSystemProduct IdentifyingNumber : " + managementObject["IdentifyingNumber"].ToString());
                    Trace.WriteLine("ComputerSystemProduct Vendor : " + managementObject["Vendor"].ToString());
                    Trace.WriteLine("ComputerSystemProduct Version : " + managementObject["Version"].ToString());
                    */
                    // Trace.WriteLine("Win32_ComputerSystemProduct SKUNumber : " + managementObject["SKUNumber"].ToString()); // Not Support
                }
            }

            return SmbiosProductName;
        }

        // Winmate Kenkun add on 2014/05/15 >>
        public String GetWMI_BIOSUuid()
        {
            String BIOSUuid = string.Empty;

            SelectQuery selectQuery = new SelectQuery("SELECT * FROM Win32_ComputerSystemProduct");
            ManagementObjectSearcher managementObjectSearch = new ManagementObjectSearcher(managementScope, selectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearch.Get();

            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                BIOSUuid = (string)managementObject["UUID"];
            }

            return BIOSUuid;
        }
        // Winmate Kenkun add on 2014/05/15 <<
        #endregion

        // winmate brian modify not use wmi get processor name ++
        public String Get_ProcessorName()
        {
            String ProcessorName = "";

            ProcessorName = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");

            if (ProcessorName.ToUpper().IndexOf("INTEL") != -1) ProcessorName = "INTEL";
            else if (ProcessorName.ToUpper().IndexOf("VIA") != -1) ProcessorName = "VIA";

            return ProcessorName;
        }
        // winmate brian modify not use wmi get processor name --

        public bool DetectIf64bit()
        {
            bool Is64Bit = false;
            String ProcessorName = "";

            SelectQuery selectQuery = new SelectQuery("SELECT * FROM Win32_Processor");
            ManagementObjectSearcher managementObjectSearch = new ManagementObjectSearcher(managementScope, selectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearch.Get();

            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                ProcessorName = (string)managementObject["AddressWidth"].ToString().Trim();
            }

            if (ProcessorName.ToUpper().IndexOf("64") != -1)
            {
                Is64Bit = true;
            }
            else
            {
                Is64Bit = false;
            }

            return (Is64Bit);
        }

        public static bool IsWindows7 // OS version: 6.1.9200 build 9200
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 1);
            }
        }

        /* Windows 10	10.0
         * Windows 8.1	6.3
         * For applications that have been manifested for Windows 8.1 or Windows 10. 
         * Applications not manifested for Windows 8.1 or Windows 10 will return the Windows 8 OS version value (6.2). 
         */
        public static bool IsWindows8or10 // OS version: 6.2.9200 build 9200
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= 6) && (Environment.OSVersion.Version.Minor >= 2);
            }
        }

        // Winmate Kenkun add on 2016/03/21 >>
        public void GetPageFileInfo()
        {
            try
            {
                ManagementObjectCollection moCollection = null;

                ObjectQuery Query = new ObjectQuery("select * from Win32_PageFile"); // 設定通過WMI要查詢的內容
                // WQL語句, 設定的WMI查詢內容和WMI的操作範圍, 檢索WMI對象集合
                ManagementObjectSearcher Searcher = new ManagementObjectSearcher(managementScope, Query);

                moCollection = Searcher.Get(); // 異步調用WMI查詢

                if (moCollection != null) // 循環
                {
                    foreach (ManagementObject mObject in moCollection)
                    {
                        long FileSize = mObject["FileSize"] == null ? 0 : long.Parse(mObject["FileSize"].ToString()); // 分頁檔大小
                        if (IsDebugMode)
                        {
                            Trace.WriteLine("Win32_PageFile FileSize : " + (FileSize / 1024 / 1024).ToString("#0.00") + "G");
                            Trace.WriteLine("Win32_PageFile Name : " + mObject["Name"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Winmate Kenkun add on 2016/03/21 <<

        public List<String> GetWMI_BatteryInformation()
        {
            List<String> batteryInformation = new List<String>();
            String sTemp = "";

            SelectQuery selectQuery = new SelectQuery("SELECT * FROM Win32_Battery");
            ManagementObjectSearcher managementObjectSearch = new ManagementObjectSearcher(managementScope, selectQuery);
            ManagementObjectCollection managementObjectCollection = managementObjectSearch.Get();

            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                try
                {
                    sTemp = managementObject["EstimatedChargeRemaining"].ToString();
                    batteryInformation.Add(sTemp);
                }
                catch (Exception ex)
                {
                    throw ex;
                    // ShowDialogMessageBox("Error");
                }
            }
            return batteryInformation;
        }

        public static bool SetWmiBrightness(byte targetBrightness)
        {
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
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return bResult;
        }

        public IEnumerable<ManagementObject> moCollection { get; set; }
    }

    #region get / set function
    public class BIOSInfoEntity
    {
        // BIOS版本
        private string strBIOSVersion = string.Empty;
        /// <summary>
        /// BIOS版本
        /// </summary>
        public string BIOSVersion
        {
            get { return strBIOSVersion; }
            set { strBIOSVersion = value; }
        }
        // 日期
        private string strBIOSReleaseDate = string.Empty;
        /// <summary>
        /// 日期
        /// </summary>
        public string BIOSReleaseDate
        {
            get { return strBIOSReleaseDate; }
            set { strBIOSReleaseDate = value; }
        }
        // SMBIOS
        private string strSMBIOS = string.Empty;
        /// <summary>
        /// SMBIOS
        /// </summary>
        public string SMBIOS
        {
            get { return strSMBIOS; }
            set { strSMBIOS = value; }
        }
    }

    public class ComputerInfoEntity
    {
        // 系統名稱:Name
        private string strName = string.Empty;
        /// <summary>
        /// 系統名稱
        /// </summary>
        public string ComputerSystemName
        {
            get { return strName; }
            set { strName = value; }
        }
        // 系統製造商:Manufacturer
        private string strManufacturer = string.Empty;
        /// <summary>
        /// 系統製造商
        /// </summary>
        public string ComputerManufacturer
        {
            get { return strManufacturer; }
            set { strManufacturer = value; }
        }

        // 系統模式:Model
        private string strModel = string.Empty;
        /// <summary>
        /// 系統模式
        /// </summary>
        public string ComputerSystemModel
        {
            get { return strModel; }
            set { strModel = value; }
        }

        // 系統類型:SystemType
        private string strType = string.Empty;
        /// <summary>
        /// 系統類型
        /// </summary>
        public string ComputerSystemType
        {
            get { return strType; }
            set { strType = value; }
        }
    }

    public class CPUInfoEntity
    {
        #region 屬性
        #region CPU名稱
        string strCPUName = string.Empty;
        /// <summary>
        /// CPU名稱
        /// </summary>
        public string CPUName
        {
            get { return strCPUName; }
            set { strCPUName = value; }
        }
        #endregion

        #region CPU序列號
        string strCPUID = string.Empty;
        /// <summary>
        /// CPU序列號
        /// </summary>
        public string CPUID
        {
            get { return strCPUID; }
            set { strCPUID = value; }
        }
        #endregion

        #region CPU個數
        int nCPUCount = 0;
        /// <summary>
        /// CPU個數
        /// </summary>
        public int CPUCount
        {
            get { return nCPUCount; }
            set { nCPUCount = value; }
        }
        #endregion

        #region CPU製造商
        string strCPUManufacturer = string.Empty;
        /// <summary>
        /// CPU製造商
        /// </summary>
        public string CPUManufacturer
        {
            get { return strCPUManufacturer; }
            set { strCPUManufacturer = value; }
        }
        #endregion

        #region 當前時脈速度
        string strCPUCurrentClockSpeed = string.Empty;
        /// <summary>
        /// 當前時脈速度
        /// </summary>
        public string CPUCurrentClockSpeed
        {
            get { return strCPUCurrentClockSpeed; }
            set { strCPUCurrentClockSpeed = value; }
        }
        #endregion

        #region 最大時脈速度
        string strCPUMaxClockSpeed = string.Empty;
        /// <summary>
        /// 最大時脈速度
        /// </summary>
        public string CPUMaxClockSpeed
        {
            get { return strCPUMaxClockSpeed; }
            set { strCPUMaxClockSpeed = value; }
        }
        #endregion

        #region 外部頻率
        string strCPUExtClock = string.Empty;
        /// <summary>
        /// 外部頻率
        /// </summary>
        public string CPUExtClock
        {
            get { return strCPUExtClock; }
            set { strCPUExtClock = value; }
        }
        #endregion

        #region 當前電壓
        string strCPUCurrentVoltage = string.Empty;
        /// <summary>
        /// 當前電壓
        /// </summary>
        public string CPUCurrentVoltage
        {
            get { return strCPUCurrentVoltage; }
            set { strCPUCurrentVoltage = value; }
        }
        #endregion

        #region 二級暫存器
        string strCPUL2CacheSize = string.Empty;
        /// <summary>
        /// 二級暫存器
        /// </summary>
        public string CPUL2CacheSize
        {
            get { return strCPUL2CacheSize; }
            set { strCPUL2CacheSize = value; }
        }
        #endregion

        #region DataWidth
        string strCPUDataWidth = string.Empty;
        /// <summary>
        /// 數據帶寬
        /// </summary>
        public string CPUDataWidth
        {
            get { return strCPUDataWidth; }
            set { strCPUDataWidth = value; }
        }
        #endregion

        #region AddressWidth
        string strCPUAddressWidth = string.Empty;
        /// <summary>
        /// 地址帶寬
        /// </summary>
        public string CPUAddressWidth
        {
            get { return strCPUAddressWidth; }
            set { strCPUAddressWidth = value; }
        }
        #endregion

        #region 使用百分比
        float fCPUUsedPercent;
        /// <summary>
        /// 使用百分比
        /// </summary>
        public float CPUUsedPercent
        {
            get { return fCPUUsedPercent; }
            set { fCPUUsedPercent = value; }
        }
        #endregion

        #region CPU溫度
        double strCPUTemperature;
        /// <summary>
        /// CPU溫度
        /// </summary>
        public double CPUTemperature
        {
            get { return strCPUTemperature; }
            set { strCPUTemperature = value; }
        }
        #endregion

        #region CPU內核
        string strNumberOfCores = "";
        /// <summary>
        /// CPU內核
        /// </summary>
        public string CPUNumberOfCores
        {
            get { return strNumberOfCores; }
            set { strNumberOfCores = value; }
        }
        #endregion

        #region CPU邏輯處理器
        string strNumberOfLogicalProcessors = "";
        /// <summary>
        /// CPU邏輯處理器
        /// </summary>
        public string CPUNumberOfLogicalProcessors
        {
            get { return strNumberOfLogicalProcessors; }
            set { strNumberOfLogicalProcessors = value; }
        }
        #endregion
        #endregion
    }

    public class MemoryInfoEntity
    {
        #region 總實體記憶體
        string strTotalVisibleMemorySize = string.Empty;  // 總實體記憶體
        public string TotalVisibleMemorySize
        {
            get { return strTotalVisibleMemorySize; }
            set { strTotalVisibleMemorySize = value; }
        }
        #endregion

        #region 可用實體記憶體
        string strFreePhysicalMemory = string.Empty;  // 可用實體記憶體

        public string FreePhysicalMemory
        {
            get { return strFreePhysicalMemory; }
            set { strFreePhysicalMemory = value; }
        }
        #endregion

        #region 總虛擬記憶體
        string strTotalVirtualMemorySize = string.Empty;  // 總虛擬記憶體

        public string TotalVirtualMemorySize
        {
            get { return strTotalVirtualMemorySize; }
            set { strTotalVirtualMemorySize = value; }
        }
        #endregion

        #region 可用虛擬記憶體
        string strFreeVirtualMemory = string.Empty;  // 可用虛擬記憶體

        public string FreeVirtualMemory
        {
            get { return strFreeVirtualMemory; }
            set { strFreeVirtualMemory = value; }
        }
        #endregion

        #region 分頁檔大小
        string strSizeStoredInPagingFiles = string.Empty;  // 分頁檔大小

        public string SizeStoredInPagingFiles
        {
            get { return strSizeStoredInPagingFiles; }
            set { strSizeStoredInPagingFiles = value; }
        }
        #endregion

        #region 可用分頁檔大小
        string strFreeSpaceInPagingFiles = string.Empty;

        public string FreeSpaceInPagingFiles
        {
            get { return strFreeSpaceInPagingFiles; }
            set { strFreeSpaceInPagingFiles = value; }
        }
        #endregion
    }

    public class SystemInfoEntity
    {
        #region 屬性
        #region OS名稱
        string strOSName = string.Empty;  // OS名稱
        /// <summary>
        /// OS名稱
        /// </summary>
        public string OSName
        {
            get { return strOSName; }
            set { strOSName = value; }
        }
        #endregion
        #region OS版本
        string strOSVersion = string.Empty;  // OS版本
        /// <summary>
        /// OS版本
        /// </summary>
        public string OSVersion
        {
            get { return strOSVersion; }
            set { strOSVersion = value; }
        }
        #endregion
        #region OS製造商
        string strOSManufacturer = string.Empty;  // OS製造商
        /// <summary>
        /// OS製造商
        /// </summary>
        public string OSManufacturer
        {
            get { return strOSManufacturer; }
            set { strOSManufacturer = value; }
        }
        #endregion

        #region SP版本
        /// <summary>
        /// SP版本
        /// </summary>
        string strOSCSDVersion = string.Empty;
        public string OSCSDVersion
        {
            get { return strOSCSDVersion; }
            set { strOSCSDVersion = value; }
        }
        #endregion

        #region Build版本
        string str0SBuildNumber = string.Empty;
        public string OSBuildNumber
        {
            get { return str0SBuildNumber; }
            set { str0SBuildNumber = value; }
        }
        #endregion

        #region Windows 目錄
        string strWindowsDirectory = string.Empty;
        /// <summary>
        /// Windows 目錄
        /// </summary>
        public string WindowsDirectory
        {
            get { return strWindowsDirectory; }
            set { strWindowsDirectory = value; }
        }
        #endregion
        #region 系統目錄
        string strSystemDirectory = string.Empty;  // 系統目錄
        /// <summary>
        /// 系統目錄
        /// </summary>
        public string SystemDirectory
        {
            get { return strSystemDirectory; }
            set { strSystemDirectory = value; }
        }
        #endregion
        #region 啟動設備
        string strBootDevice = string.Empty;  // 啟動設備
        /// <summary>
        //// /啟動設備
        /// </summary>
        public string BootDevice
        {
            get { return strBootDevice; }
            set { strBootDevice = value; }
        }
        #endregion
        #region 地區
        string strCountry = string.Empty;  // 地區
        /// <summary>
        /// 地區
        /// </summary>
        public string Country
        {
            get { return strCountry; }
            set { strCountry = value; }
        }
        #endregion
        #region 時區
        string strTimeZone = string.Empty;  // 時區
        /// <summary>
        /// 時區
        /// </summary>
        public string TimeZone
        {
            get { return strTimeZone; }
            set { strTimeZone = value; }
        }
        #endregion
        #region 總實體記憶體
        string strTotalVisibleMemorySize = string.Empty;  // 總實體記憶體
        /// <summary>
        /// 總實體記憶體
        /// </summary>
        public string TotalVisibleMemorySize
        {
            get { return strTotalVisibleMemorySize; }
            set { strTotalVisibleMemorySize = value; }
        }
        #endregion
        #region 可用實體記憶體
        string strFreePhysicalMemory = string.Empty;  // 可用實體記憶體
        /// <summary>
        /// 可用實體記憶體
        /// </summary>
        public string FreePhysicalMemory
        {
            get { return strFreePhysicalMemory; }
            set { strFreePhysicalMemory = value; }
        }
        #endregion
        #region 總虛擬記憶體
        string strTotalVirtualMemorySize = string.Empty;  // 總虛擬記憶體
        /// <summary>
        /// 總虛擬記憶體
        /// </summary>
        public string TotalVirtualMemorySize
        {
            get { return strTotalVirtualMemorySize; }
            set { strTotalVirtualMemorySize = value; }
        }
        #endregion
        #region 可用虛擬記憶體
        string strFreeVirtualMemory = string.Empty;  // 可用虛擬記憶體
        /// <summary>
        /// 可用虛擬記憶體
        /// </summary>
        public string FreeVirtualMemory
        {
            get { return strFreeVirtualMemory; }
            set { strFreeVirtualMemory = value; }
        }
        #endregion
        #region 分頁檔大小
        string strSizeStoredInPagingFiles = string.Empty;  // 分頁檔大小
        /// <summary>
        /// 分頁檔大小
        /// </summary>
        public string SizeStoredInPagingFiles
        {
            get { return strSizeStoredInPagingFiles; }
            set { strSizeStoredInPagingFiles = value; }
        }
        #endregion

        #region 可用分頁檔大小
        string strFreeSpaceInPagingFiles = string.Empty;
        /// <summary>
        /// 可用分頁檔大小
        /// </summary>
        public string FreeSpaceInPagingFiles
        {
            get { return strFreeSpaceInPagingFiles; }
            set { strFreeSpaceInPagingFiles = value; }
        }
        #endregion

        #region 分頁檔大小
        string strFileSize = string.Empty;
        /// <summary>
        /// 分頁檔大小
        /// </summary>
        public string FileSize
        {
            get { return strFileSize; }
            set { strFileSize = value; }
        }
        #endregion

        #region 分頁檔
        string strFileName = string.Empty;
        /// <summary>
        /// 分頁檔大小
        /// </summary>
        public string FileName
        {
            get { return strFileName; }
            set { strFileName = value; }
        }
        #endregion
        #endregion
    }
    #endregion

}
