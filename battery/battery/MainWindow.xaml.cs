using HotTabFunction;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace battery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region DLL Import
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        #endregion

        #region Definition
        private int NowTestCount = 0;           // 目前電池的測試次數
        private int CorrectCount = 0;           // 目前電池的正確測試次數
        private static int ReadCount = 0;       // 每一輪的測試, 目前重讀電池資訊的嘗試次數
        private static int RetryThreshold = 3;  // 每一輪的測試, 當沒有讀取到電池資訊或是讀取到錯誤的電池資訊(電流、電壓...etc)時, 要立刻要重讀的嘗試次數(預設為3次)
        private static int BatteryErrorCode = 0;// 用來判斷是否有電池一直讀取錯誤的錯誤代碼, 代碼的不同代表不同的欄位錯誤顯示對應的錯誤訊息
        private static int BatteryErrorCode2 = 0;
        private bool IsSecondBatteryTesting = false;
        private readonly string AC_Mode_Test = "AC Mode Test";
        private readonly string Battery_Mode_Test = "Battery Mode Test";
        private readonly string SBAT_Charge_Test = "SBAT Charge Test";
        private readonly string SBAT_Discharge_Test = "SBAT Discharge Test";

        public static bool ACModeSuccess = false;
        public static bool MainBatterySuccess = false;
        public static bool SecondBatterySuccess = false;

        private const int ExtBatChargeThresholdWithInternalBatOver90 = 90;
        private const int TwoBatteryChargeThreshold = 15;
        private int RetryCount = 0;

        private string TempFirmwareVersionBattery1 = "";
        private string TempFirmwareVersionBattery2 = "";
        static bool IsDebugMode = true;
        static string TestProduct = "IB80";
        System.Windows.Forms.Timer TimerBattery;
        static bool IsBiosTestModeEnabled = false;
        static bool IsBTZ1 = false;
        static bool IsFixtureExisted = true; // 看是否支援治具測試, 例如: RTS 控制AC(測試Battery)跟LED燈(測試LightSensor)
        JObject result = new JObject();
        #endregion

        #region Battery Definition
        private int batteryLifePrecent = -1; // 電池目前的剩餘%數
        private int BatteryLifePrecent
        {
            get
            {
                // BatteryLifePercent = 電池完成充電的剩餘百分比. 可以是介於 0 到 100 範圍內的值, 如果狀態未知則為 255. 所有其他值都已保留. 
                HotTabDLL.BatteryLifePrecentGet(ref batteryLifePrecent);
                if (IsDebugMode)
                {
                    Trace.WriteLine("Battery Life = " + batteryLifePrecent);
                }
                return batteryLifePrecent;
            }
        }

        private static int b1RemainingCapacity = 0;
        public static int Battery1RemainingCapacity
        {
            get
            {
                if (!HotTabDLL.GetBattery1RemainingCapacity(out b1RemainingCapacity)) b1RemainingCapacity = -1;
                ReadCount = 0;
                while (b1RemainingCapacity.Equals(-1) && ReadCount <= RetryThreshold)
                {
                    HotTabDLL.GetBattery1RemainingCapacity(out b1RemainingCapacity);
                    ReadCount++;
                    if (IsDebugMode) Trace.WriteLine("Count : " + ReadCount + "\tBattery1RemainingCapacity".PadRight(30, ' ') + b1RemainingCapacity);
                }
                if (ReadCount >= RetryThreshold) BatteryErrorCode += 1;

                if (IsDebugMode) Trace.WriteLine("Battery1RemainingCapacity".PadRight(30, ' ') + b1RemainingCapacity);
                return b1RemainingCapacity;
            }
        }

        private static int b2RemainingCapacity = 0;
        public static int Battery2RemainingCapacity
        {
            get
            {
                if (!HotTabDLL.GetBattery2RemainingCapacity(out b2RemainingCapacity)) b2RemainingCapacity = -1;
                ReadCount = 0;
                while (b2RemainingCapacity.Equals(-1) && ReadCount <= RetryThreshold)
                {
                    HotTabDLL.GetBattery2RemainingCapacity(out b2RemainingCapacity);
                    ReadCount++;
                    if (IsDebugMode) Trace.WriteLine("Count : " + ReadCount + "\tBattery2RemainingCapacity".PadRight(30, ' ') + b1RemainingCapacity);
                }
                if (ReadCount >= RetryThreshold) BatteryErrorCode2 += 1;

                if (IsDebugMode) Trace.WriteLine("Battery2RemainingCapacity".PadRight(30, ' ') + b2RemainingCapacity);
                return b2RemainingCapacity;
            }
        }

        private static int b1FullChargeCapacity = 0;
        public static int Battery1FullChargeCapacity
        {
            get
            {
                if (!HotTabDLL.GetBattery1FullChargeCapacity(out b1FullChargeCapacity)) b1FullChargeCapacity = -1;
                ReadCount = 0;
                while (b1FullChargeCapacity.Equals(-1) && ReadCount <= RetryThreshold)
                {
                    HotTabDLL.GetBattery1FullChargeCapacity(out b1FullChargeCapacity);
                    ReadCount++;
                }
                if (ReadCount >= RetryThreshold) BatteryErrorCode += 2;

                if (IsDebugMode) Trace.WriteLine("Battery1FullChargeCapacity".PadRight(30, ' ') + b1FullChargeCapacity);
                return b1FullChargeCapacity;
            }
        }
        private static int b2FullChargeCapacity = 0;
        public static int Battery2FullChargeCapacity
        {
            get
            {
                if (!HotTabDLL.GetBattery2FullChargeCapacity(out b2FullChargeCapacity)) b2FullChargeCapacity = -1;
                ReadCount = 0;
                while (b2FullChargeCapacity.Equals(-1) && ReadCount <= RetryThreshold)
                {
                    HotTabDLL.GetBattery2FullChargeCapacity(out b2FullChargeCapacity);
                    ReadCount++;
                }
                if (ReadCount >= RetryThreshold) BatteryErrorCode2 += 2;

                if (IsDebugMode) Trace.WriteLine("Battery2FullChargeCapacity".PadRight(30, ' ') + b2FullChargeCapacity);
                return b2FullChargeCapacity;
            }
        }

        private static int b1ChargingVoltage = 0;
        public static int Battery1ChargingVoltage
        {
            get
            {
                if (!HotTabDLL.GetBattery1ChargingVoltage(out b1ChargingVoltage)) b1ChargingVoltage = -1;
                ReadCount = 0;
                while (b1ChargingVoltage.Equals(-1) && ReadCount <= RetryThreshold)
                {
                    HotTabDLL.GetBattery1ChargingVoltage(out b1ChargingVoltage);
                    ReadCount++;
                }
                if (ReadCount >= RetryThreshold) BatteryErrorCode += 4;

                if (IsDebugMode) Trace.WriteLine("Battery1ChargingVoltage".PadRight(30, ' ') + b1ChargingVoltage);
                return b1ChargingVoltage;
            }
        }
        private static int b2ChargingVoltage = 0;
        public static int Battery2ChargingVoltage
        {
            get
            {
                if (!HotTabDLL.GetBattery2ChargingVoltage(out b2ChargingVoltage)) b2ChargingVoltage = -1;
                ReadCount = 0;
                while (b2ChargingVoltage.Equals(-1) && ReadCount <= RetryThreshold)
                {
                    HotTabDLL.GetBattery2ChargingVoltage(out b2ChargingVoltage);
                    ReadCount++;
                }
                if (ReadCount >= RetryThreshold) BatteryErrorCode2 += 4;

                if (IsDebugMode) Trace.WriteLine("Battery2ChargingVoltage".PadRight(30, ' ') + b2ChargingVoltage);
                return b2ChargingVoltage;
            }
        }

        private static int b1Percentage = -1;
        public static int Battery1Percentage
        {
            get
            {
                b1Percentage = -1;
                HotTabDLL.GetBattery1Percentage(out b1Percentage);
                if (IsDebugMode) Trace.WriteLine("Battery1RemainingCapacity".PadRight(30, ' ') + b1Percentage);
                return b1Percentage;
            }
        }

        private static int b2Percentage = -1;
        public static int Battery2Percentage
        {
            get
            {
                b2Percentage = -1;
                HotTabDLL.GetBattery2Percentage(out b2Percentage);
                if (IsDebugMode) Trace.WriteLine("Battery2RemainingCapacity".PadRight(30, ' ') + b2Percentage);
                return b2Percentage;
            }
        }

        private static int b1Status = -1;
        public static int Battery1Status
        {
            get
            {
                b1Status = -1;
                HotTabDLL.GetBattery1RemainingCapacity(out b1Status);
                if (IsDebugMode) Trace.WriteLine("Battery1RemainingCapacity".PadRight(30, ' ') + b1Status);
                return b1Status;
            }
        }

        private static int b2Status = -1;
        public static int Battery2Status
        {
            get
            {
                b2Status = -1;
                HotTabDLL.GetBattery2RemainingCapacity(out b2Status);
                if (IsDebugMode) Trace.WriteLine("Battery2RemainingCapacity".PadRight(30, ' ') + b2Status);
                return b2Status;
            }
        }

        // CycleCount 0x17
        // DesignCapacity 0x18 
        // ManufactureDate 0x1b 
        // ManufacturerName 0x20
        private static uint b1DesignCapacity = 0;
        public static uint Battery1DesignCapacity
        {
            get
            {
                b1DesignCapacity = 0;
                HotTabDLL.GetBattery1SpecificInfo(0x18, out b1DesignCapacity);
                if (IsDebugMode) Trace.WriteLine("Battery1DesignCapacity".PadRight(30, ' ') + b1DesignCapacity);
                return b1DesignCapacity;
            }
        }

        private static uint b2DesignCapacity = 0;
        public static uint Battery2DesignCapacity
        {
            get
            {
                b2DesignCapacity = 0;
                HotTabDLL.GetBattery2SpecificInfo(0x18, out b2DesignCapacity);
                if (IsDebugMode) Trace.WriteLine("Battery2DesignCapacity".PadRight(30, ' ') + b2DesignCapacity);
                return b2DesignCapacity;
            }
        }

        private static uint b1CycleCount = 0;
        public static uint Battery1CycleCount
        {
            get
            {
                b1CycleCount = 0;
                HotTabDLL.GetBattery1SpecificInfo(0x17, out b1CycleCount);
                if (IsDebugMode) Trace.WriteLine("Battery1CycleCount".PadRight(30, ' ') + b1CycleCount);
                return b1CycleCount;
            }
        }

        private static uint b2CycleCount = 0;
        public static uint Battery2CycleCount
        {
            get
            {
                b2CycleCount = 0;
                HotTabDLL.GetBattery2SpecificInfo(0x17, out b2CycleCount);
                if (IsDebugMode) Trace.WriteLine("Battery2CycleCount".PadRight(30, ' ') + b2CycleCount);
                return b2CycleCount;
            }
        }

        // Day : bit4 - bit0
        // Month : bit8 - bit5
        // Year : bit15 - bit9 (by 1980)
        private static string ManufactureDate = "19800101";
        private static uint ManufactureDate_Day = 1;
        private static uint ManufactureDate_Month = 1;
        private static uint ManufactureDate_Year = 1980;
        private static uint b1ManufactureDate = 0;
        public static uint Battery1ManufactureDate
        {
            get
            {
                ManufactureDate_Day = 0;
                ManufactureDate_Month = 0;
                ManufactureDate_Year = 1980;
                b1ManufactureDate = 0;
                HotTabDLL.GetBattery1SpecificInfo(0x1b, out b1ManufactureDate);
                ManufactureDate_Day = (uint)(b1ManufactureDate & 0x1F);
                ManufactureDate_Month = (uint)((b1ManufactureDate >> 5) & 0x0F);
                ManufactureDate_Year += (uint)((b1ManufactureDate >> 9) & 0x7F);
                ManufactureDate = ManufactureDate_Year.ToString() + ManufactureDate_Month.ToString("D2") + ManufactureDate_Day.ToString("D2");
                if (!uint.TryParse(ManufactureDate, out b1ManufactureDate)) b1ManufactureDate = 19800101;  // b1ManufactureDate = Convert.ToUInt32(ManufactureDate);
                if (IsDebugMode) Trace.WriteLine("Battery1ManufactureDate".PadRight(30, ' ') + b1ManufactureDate);
                return b1ManufactureDate;
            }
        }

        private static uint b2ManufactureDate = 0;
        public static uint Battery2ManufactureDate
        {
            get
            {
                ManufactureDate_Day = 0;
                ManufactureDate_Month = 0;
                ManufactureDate_Year = 1980;
                b2ManufactureDate = 0;
                HotTabDLL.GetBattery2SpecificInfo(0x1b, out b2ManufactureDate);
                ManufactureDate_Day = (uint)(b2ManufactureDate & 0x1F);
                ManufactureDate_Month = (uint)((b2ManufactureDate >> 5) & 0x0F);
                ManufactureDate_Year += (uint)((b2ManufactureDate >> 9) & 0x7F);
                ManufactureDate = ManufactureDate_Year.ToString() + ManufactureDate_Month.ToString("D2") + ManufactureDate_Day.ToString("D2");
                if (!uint.TryParse(ManufactureDate, out b2ManufactureDate)) b2ManufactureDate = 19800101;  // b2ManufactureDate = Convert.ToUInt32(ManufactureDate);
                if (IsDebugMode) Trace.WriteLine("Battery2ManufactureDate".PadRight(30, ' ') + b2ManufactureDate);
                return b2ManufactureDate;
            }
        }

        private static string b1ManufacturerName = "Null";
        public static string Battery1ManufacturerName
        {
            get
            {
                return b1ManufacturerName;
            }
        }

        private static string b2ManufacturerName = "Null";
        public static string Battery2ManufacturerName
        {
            get
            {
                return b2ManufacturerName;
            }
        }
        #endregion

        #region Battery
        public static int BatteryTotalCount = 5;            // 總共要測試幾次
        public static double CorrectTolerance = 0.8;          // 成功率要幾%才是PASS
        public static int checkBatteryTimerInterval = 1550; // 間隔多久MS去讀取電池資訊
        public static int BatteryAmount = 2;                  // 電池數量. 1：只有主電池, 2：有主副兩顆電池. 

        public static int B1DesignFullCapacity = 5200;      // 電池設計充滿電量
        public static int B2DesignFullCapacity = 0;
        public static int B1DesignVoltage = 8400;           // 電池設計電壓
        public static int B2DesignVoltage = 400;

        public static double FullCapacityTolerance = 0.95;  // 電池設計充滿電量最低容忍值（換算成百分比）
        public static double RemainingTolerance = 0.05;     // 電池目剩餘電量最低容忍值
        public static double VoltageTolerance = 0.9;        // 電池電壓最低容忍值
        public static double OutlierLevel = 0.8;            // 設定要大於或小於某個百分比後, 必須重新讀取電池資訊. 例：Outlier等於0.8, 則電量低於(1-0.8)或是高於(1+0.8)就是代表有問題需要重新讀取

        public static double BatteryTime = 0;               // 用以計算單一測項的測試時間
        public static bool NoGaugeIC = false;                // 小電池是否有Gauge IC

        public static bool IsSmartBatterySpecSupport = true;
        public static bool IsVerifyBatteryFirmware = true;

        private static double b2ChangeThreshold = 0.8;       // SBAT要小於幾%才開始充電
        static double SBATChangeThreshold
        {
            get
            {
                return (b2ChangeThreshold > 0) ? b2ChangeThreshold : 0.8;
            }
            set
            {
                b2ChangeThreshold = value;
            }
        }

        static string Battery1FirmwareAddress = "ManufactureDate";
        static string Battery1FirmwareVersion = "20160615";


        static string Battery2FirmwareAddress = "ManufactureDate";
        static string Battery2FirmwareVersion = "20170908";

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Loaded += battery_Load;
        }

        string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(exepath, path);
        }

        private void battery_Load(object sender, RoutedEventArgs routedEventArgs)
        {
            result["result"] = "FAIL";
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Application.Current.Shutdown();
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            PublicFunction.PortAddressRTS = jobject.PortAddressRTS.ToString();
            TestProduct = jobject.TestProduct.ToString();
            IsBiosTestModeEnabled = (bool)jobject.IsBiosTestModeEnabled;
            IsBTZ1 = (bool)jobject.IsBTZ1;
            IsFixtureExisted = (bool)jobject.IsFixtureExisted;
            IsVerifyBatteryFirmware = (bool)jobject.IsVerifyBatteryFirmware;
            NoGaugeIC = (bool)jobject.NoGaugeIC;
            BatteryTotalCount = (int)jobject.BatteryTotalCount;
            CorrectTolerance = (double)jobject.CorrectTolerance;
            BatteryAmount = (int)jobject.BatteryAmount;
            SBATChangeThreshold = (double)jobject.SBATChangeThreshold;

            Trace.WriteLine("Battery_Load");

            TimerBattery = new System.Windows.Forms.Timer();
            //設定計時器的速度

            TimerBattery.Interval = checkBatteryTimerInterval;
            TimerBattery.Tick += new EventHandler(TimerBattery_Tick);

            // 當大電池抓不到會嘗試去抓小電池的資訊或是當有第二顆電池時才顯示第二顆的相關資訊
            if (BatteryAmount.Equals(2) ||
                (BatteryAmount.Equals(1) && NoGaugeIC))
            {
                groupSecondBattery.Visibility = Visibility.Visible;
            }
            else
            {
                groupSecondBattery.Visibility = Visibility.Hidden;
            }

            if (IsSmartBatterySpecSupport)
            {
                if (IsVerifyBatteryFirmware)
                {
                    #region 讀取韌體版本
                    Trace.WriteLine("IsVerifyBatteryFirmware" + IsVerifyBatteryFirmware);

                    if (Battery1FirmwareAddress.Equals("ManufacturerName") && !Battery1ManufacturerName.Equals("Null")) TempFirmwareVersionBattery1 = Battery1ManufacturerName;
                    else if (Battery1ManufactureDate.ToString().Length.Equals(8)) TempFirmwareVersionBattery1 = Battery1ManufactureDate.ToString();
                    else TempFirmwareVersionBattery1 = "Unknown";

                    Trace.WriteLine("TempFirmwareVersionBattery1: " + TempFirmwareVersionBattery1);

                    labelBat1Firmware.Text = TempFirmwareVersionBattery1;

                    if (BatteryAmount.Equals(2))
                    {
                        if (Battery2FirmwareAddress.Equals("ManufacturerName") && !Battery2ManufacturerName.Equals("Null")) TempFirmwareVersionBattery2 = Battery2ManufacturerName;
                        else if (Battery2ManufactureDate.ToString().Length.Equals(8)) TempFirmwareVersionBattery2 = Battery2ManufactureDate.ToString();
                        else TempFirmwareVersionBattery2 = "Unknown";

                        labelBat2Firmware.Text = TempFirmwareVersionBattery2;

                    }
                    #endregion

                    #region 確認韌體版本是最新一版
                    if (!Battery1FirmwareVersion.Equals(TempFirmwareVersionBattery1))
                    {
                        checkTestStatus("Battery1 Firmware Version is incorrect : " + labelBat1Firmware.Text);
                        return;
                    }
                    else if (BatteryAmount.Equals(2) && !Battery2FirmwareVersion.Equals(TempFirmwareVersionBattery2))
                    {
                        checkTestStatus("Battery2 Firmware Version is incorrect : " + labelBat2Firmware.Text);
                        return;
                    }
                    #endregion
                }
            }

            if (IsTwoMainBatteryProduct())
            {
                groupMainBattery.Content = "Internal Battery";
                groupSecondBattery.Content = "Extend Battery";
            }

            if (PublicFunction.SerialPortRTS == null)
                IsFixtureExisted = false;

            CheckBatteryInfo();
        }

        private bool IsAcAdapterExist()
        {
            while (!PublicFunction.IsCharging)
            {
                MessageBoxResult Result = ShowDialogMessageBox("Please plug into an AC adapter.", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (Result.Equals(MessageBoxResult.No))
                {
                    // checkTestStatus("FAIL");
                    return false;
                }
            }
            return true;
        }

        private void CheckBatteryInfo()
        {
            if (IsTwoMainBatteryProduct() && !GetBTZ1RemainingCapacity())
            {
                checkTestStatus("FAIL");
                return;
            }

            if (IsAcAdapterExist())
            {
                Title.Content = AC_Mode_Test;
                TimerBattery.Start();
            }
            else
            {
                checkTestStatus("FAIL");
            }
        }

        private bool GetBTZ1RemainingCapacity()
        {
            if (IsBiosTestModeEnabled)
            {
                TestMode.SetTestModeCharging((byte)TestMode.ListBattery.External);
                RetryCount = 0;
                while (Battery2Percentage.Equals(0))
                {
                    RetryCount++;
                    if (RetryCount > 20)
                    {
                        return false;
                    }
                    MessageBoxResult ExtendBatResult = ShowDialogMessageBox("Please plug into an Extend Battery.\r\n", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (ExtendBatResult == MessageBoxResult.Yes)
                    {
                        Thread.Sleep(200);
                        TestMode.SetTestModeCharging((byte)TestMode.ListBattery.External);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                while (Battery2Percentage.Equals(0) || Battery2Percentage > TwoBatteryChargeThreshold)
                {
                    MessageBoxResult ExtendBatResult = ShowDialogMessageBox("Please plug into an Extend Battery.\r\nAnd make sure the Extend Battery is less than 15%", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (ExtendBatResult == MessageBoxResult.Yes)
                    {
                        if (Battery1Percentage < ExtBatChargeThresholdWithInternalBatOver90)
                        {
                            ShowDialogMessageBox("Please make sure the Internal Battery is above 90%", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool IsTwoMainBatteryProduct()
        {   // 為兩顆大電池架構的機種
            return IsBTZ1 || TestProduct.Equals("9020") || TestProduct.Equals("IH83");
        }

        private string ckeckBatteryErrorMessage(int ErrorCode)
        {
            switch (ErrorCode)
            {
                case 0:
                    return "OK";
                case 1:
                    return "Remaining Capacity read error";
                case 2:
                    return "Full Capacity read error";
                case 3:
                    return "Remaining + Full Capacity read error";
                case 4:
                    return "Voltage read error";
                case 5:
                    return "Remaining + Voltage read error";
                case 6:
                    return "Full Capacity + Voltage read error";
                case 7:
                    return "All information read error";
                default:
                    return "Unknown Error";
            }
        }


        private void UpdatingBatteryInfo()
        {
            try
            {
                // 每次更新電池資訊前, 先將錯誤代碼歸零
                BatteryErrorCode = 0;
                BatteryErrorCode2 = 0;

                if ((Title.Content.Equals(AC_Mode_Test)) && (!PublicFunction.IsCharging))
                {
                    Trace.WriteLine("Lost AC Adapter in AC Mode Test.");
                    checkTestStatus("Lost AC Adapter in AC Mode Test");
                }
                else if ((Title.Content.Equals(Battery_Mode_Test) ||
                         (!IsTwoMainBatteryProduct() && Title.Content.Equals(SBAT_Charge_Test)) ||
                         Title.Content.Equals(SBAT_Discharge_Test)) &&
                         PublicFunction.IsCharging)
                {
                    Trace.WriteLine("AC IN in Battery Mode Test.");
                    checkTestStatus("AC IN in Battery Mode Test");
                }

                #region Battery 1 即時更新電池的讀取狀態
                B1RealTimeInfo.Content = ckeckBatteryErrorMessage(BatteryErrorCode);
                if (B1RealTimeInfo.Content.Equals("OK")) B1RealTimeInfo.Foreground = Brushes.Green;
                else B1RealTimeInfo.Foreground = Brushes.Red;
                #endregion

                #region Battery 2 即時更新電池的讀取狀態
                B2RealTimeInfo.Content = ckeckBatteryErrorMessage(BatteryErrorCode2);
                if (B2RealTimeInfo.Content.Equals("OK")) B2RealTimeInfo.Foreground = Brushes.Green;
                else B2RealTimeInfo.Foreground = Brushes.Red;
                #endregion

                #region Update UI
                BatteryLife.Text = BatteryLifePrecent + " %";

                if (PublicFunction.IsCharging) AC_Status.Text = "IN";
                else AC_Status.Text = "OUT";

                labelBat1Percentage.Text = Battery1Percentage + " %";
                labelBat1NowCurrent.Text = PublicFunction.Battery1Current + " mA";
                labelBat1Voltage.Text = Battery1ChargingVoltage + " mV";
                labelBat1Remaining.Text = Battery1RemainingCapacity + " mAh";
                labelBat1FullCapacity.Text = Battery1FullChargeCapacity + " mAh";

                labelBat2Percentage.Text = Battery2Percentage + " %";
                labelBat2NowCurrent.Text = PublicFunction.Battery2Current + " mA";
                labelBat2Voltage.Text = Battery2ChargingVoltage + " mV";
                labelBat2Remaining.Text = Battery2RemainingCapacity + " mAh";
                labelBat2FullCapacity.Text = Battery2FullChargeCapacity + " mAh";
                #endregion
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                TimerBattery.Stop();
            }
        }

        private bool IsBTZ1BatteryCorrect()
        {
            if (((Title.Content.Equals(AC_Mode_Test) || Title.Content.Equals(Battery_Mode_Test)) && b2RemainingCapacity > 0 && PublicFunction.Battery2Current > 0) ||
                ((Title.Content.Equals(SBAT_Discharge_Test) || Title.Content.Equals(SBAT_Charge_Test)) && b1RemainingCapacity > 0 && PublicFunction.Battery1Current > 0 && PublicFunction.Battery2Current.Equals(0))
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool IsTwoMainBatteryCorrect()
        {
            if (!IsTwoMainBatteryProduct()) return false;

            if (IsBiosTestModeEnabled)
            {
                return IsBTZ1BatteryCorrect();
            }
            else
            {
                if (((Battery1Percentage >= ExtBatChargeThresholdWithInternalBatOver90 && Battery2Percentage < ExtBatChargeThresholdWithInternalBatOver90) ||
                     (Battery1Percentage.Equals(100) && Battery2Percentage > 0) ||
                     (Battery1Percentage < TwoBatteryChargeThreshold && Battery2Percentage < TwoBatteryChargeThreshold)))
                {
                    return IsBTZ1BatteryCorrect();
                }
                else
                {
                    return false;
                }
            }
        }

        private bool IsMainBatteryCorrect()
        {
            Trace.WriteLine("Main");
            Trace.WriteLine("BatteryAmount: " + BatteryAmount);
            Trace.WriteLine("IsSecondBatteryTesting: " + IsSecondBatteryTesting);
            Trace.WriteLine("b1RemainingCapacity: " + b1RemainingCapacity);
            Trace.WriteLine("PublicFunction.Battery1Current: " + PublicFunction.Battery1Current);
            Trace.WriteLine("Battery1Percentage: " + Battery1Percentage);
            Trace.WriteLine("NoGaugeIC: " + NoGaugeIC);
            Trace.WriteLine("b2RemainingCapacity: " + b2RemainingCapacity);
            Trace.WriteLine("PublicFunction.Battery2Current: " + PublicFunction.Battery2Current);
            Trace.WriteLine("b2ChargingVoltage: " + b2ChargingVoltage);

            return BatteryAmount.Equals(2) && !IsSecondBatteryTesting
                    &&
                    (
                    // MBAT 的剩餘電量要 > 0 & ( 充電電流 > 0 || 當充電電流 = 0 時確認電池已經充到100%
                    ((b1RemainingCapacity > 0 && (PublicFunction.Battery1Current > 0 || (Battery1Percentage.Equals(100) && PublicFunction.Battery1Current.Equals(0)))) &&
                    // SBAT
                    ((!NoGaugeIC && b2RemainingCapacity > 0 && PublicFunction.Battery2Current >= 0) || (NoGaugeIC && b2ChargingVoltage > 133 && b2ChargingVoltage < 200)))
                    ||
                    (!NoGaugeIC && !PublicFunction.IsCharging && b1RemainingCapacity > 0 && PublicFunction.Battery1Current.Equals(0) && b2RemainingCapacity > 0 && PublicFunction.Battery2Current > 0)
                    );
        }

        private bool IsSingleBatteryCorrect()
        {
            return BatteryAmount.Equals(1) && !IsSecondBatteryTesting && b1RemainingCapacity > 0 && PublicFunction.Battery1Current > 0;
        }

        private bool IsSecondBatteryCorrect()
        {
            Trace.WriteLine("Second");
            Trace.WriteLine("BatteryAmount: " + BatteryAmount);
            Trace.WriteLine("IsSecondBatteryTesting: " + IsSecondBatteryTesting);
            Trace.WriteLine("NoGaugeIC: " + NoGaugeIC);
            Trace.WriteLine("b2RemainingCapacity: " + b2RemainingCapacity);
            Trace.WriteLine("PublicFunction.Battery2Current: " + PublicFunction.Battery2Current);
            Trace.WriteLine("b2ChargingVoltage: " + b2ChargingVoltage);

            return BatteryAmount.Equals(2) && IsSecondBatteryTesting &&
                    (// SBAT 的剩餘電量要 > 0 & ( 充電電流 > 0 || 當充電電流 = 0 時確認小電池的剩餘電量%已經 > SBATChangeThreshold
                    (!NoGaugeIC && b2RemainingCapacity > 0 && (PublicFunction.Battery2Current > 0 || Battery2Percentage >= (100 * SBATChangeThreshold)))
                    ||
                    (NoGaugeIC && b2ChargingVoltage > 133 && b2ChargingVoltage < 200));
        }

        private void NowTestBAT1()
        {
            groupMainBattery.Background = Brushes.Gold;
            groupSecondBattery.Background = SystemColors.ControlBrush;
        }

        private void NowTestBAT2()
        {
            groupSecondBattery.Background = Brushes.Gold;
            groupMainBattery.Background = SystemColors.ControlBrush;
        }

        private void IdentifyTestingBatteryInUI()
        {
            if (Title.Content.Equals(AC_Mode_Test) ||
                Title.Content.Equals(Battery_Mode_Test)
                )
            {
                if (IsTwoMainBatteryProduct())
                {
                    NowTestBAT2();
                }
                else
                {
                    NowTestBAT1();
                }
            }
            else if (Title.Content.Equals(SBAT_Discharge_Test) ||
                Title.Content.Equals(SBAT_Charge_Test))
            {
                if (IsTwoMainBatteryProduct())
                {
                    NowTestBAT1();
                }
                else
                {
                    NowTestBAT2();
                }
            }
        }

        private bool IsAcAdapterNotExist()
        {
            while (PublicFunction.IsCharging)
            {
                if (IsFixtureExisted)
                {
                    if (!PublicFunction.EnableRTS()) return false;
                }
                else
                {
                    MessageBoxResult Result = ShowDialogMessageBox("Please un-plug an AC adapter.", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (Result.Equals(MessageBoxResult.No))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static void WaitBatteryInit()
        {
            if (TestProduct.Equals("101P"))
            {
                Trace.WriteLine("Please wait 3 seconds for M101P Battery Initial.");
                Thread.Sleep(3000);
            }
            else if (TestProduct.Equals("IBWH"))
            {
                ShowDialogMessageBox("Please wait 10 seconds for Battery Init.", "Attention", MessageBoxButton.OK, MessageBoxImage.Information);
                Trace.WriteLine("Please wait 10 seconds for IBWH Battery Initial.");
                Thread.Sleep(12000);
            }
        }

        private bool SetInternalBatteryDischarge()
        {
            if (IsBiosTestModeEnabled)
            {
                TestMode.SetTestModeDischarging((byte)TestMode.ListBattery.Internal);
                Thread.Sleep(300);
                RetryCount = 0;
                while (!PublicFunction.Battery2Current.Equals(0))
                {
                    RetryCount++;
                    if (RetryCount > 20)
                    {
                        // checkTestStatus("Can't switch to Internal Battery");
                        return false;
                    }
                    else
                    {
                        Thread.Sleep(300);
                        Trace.WriteLine("Can't switch to Internal Battery");
                        TestMode.SetTestModeDischarging((byte)TestMode.ListBattery.Internal);
                    }
                }
                return true;

            }
            else
            {
                PublicFunction.SwitchToInternalBattery();
                MessageBoxResult Result = ShowDialogMessageBox("Please remove Extend Battery.", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (Result.Equals(MessageBoxResult.No))
                {
                    return false;
                }
                return true;
            }
        }

        private bool IsMainBatteryInserted()
        {
            while (Battery1Status <= 0)
            {
                MessageBoxResult Result = ShowDialogMessageBox("Please plug into Main Battery.", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (Result.Equals(MessageBoxResult.No))
                {
                    checkTestStatus("IsMainBatteryNotPlugIn");
                    return false;
                }

                WaitBatteryInit();
            }
            return true;
        }

        private static bool IsEcPollingSlowProduct()
        {   // EC 更新速度慢, 當切換BAT跟AC狀態時需要初始化時間
            return TestProduct.Equals("IBWH") || TestProduct.Equals("101P");
        }

        private void TimerBattery_Tick(object sender, EventArgs e)
        {
            //Thread Timer = Thread.CurrentThread; // 取得目前執行的執行緒
            //Timer.IsBackground = true; // 將thread設定為背景執行緒 (如果設定為false, 按[X]關閉Form, 執行緒仍在背景繼續執行, 故要設定為true. )

            NowTestCount++;
            UpdatingBatteryInfo();
            Trace.WriteLine("IsTwoMainBatteryCorrect: " + IsTwoMainBatteryCorrect());
            Trace.WriteLine("IsSingleBatteryCorrect: " + IsSingleBatteryCorrect());
            Trace.WriteLine("IsMainBatteryCorrect: " + IsMainBatteryCorrect());
            Trace.WriteLine("IsSecondBatteryCorrect: " + IsSecondBatteryCorrect());
            if (IsTwoMainBatteryCorrect() || IsSingleBatteryCorrect() || IsMainBatteryCorrect() || IsSecondBatteryCorrect())
            {
                CorrectCount++;
            }

            IdentifyTestingBatteryInUI();

            TestCount.Content = "Total : " + NowTestCount.ToString();
            ReadCorrectCount.Content = "Correct : " + CorrectCount.ToString();

            if (NowTestCount.Equals(BatteryTotalCount) && CorrectCount >= BatteryTotalCount * CorrectTolerance)
            {
                TimerBattery.Stop();
                // 測完 AC Mode 改測 Battery Mode
                if (Title.Content.Equals(AC_Mode_Test)) // 測完 MBAT 充電 ->  MBAT 放電 
                {
                    if (!IsAcAdapterNotExist())
                    {
                        checkTestStatus("IsAcAdapterExist - Can't switch AC In/Out.");
                        return;
                    }

                    WaitBatteryInit();
                }
                else if (BatteryAmount.Equals(2) && Title.Content.Equals(Battery_Mode_Test)) // 測完 MBAT 放電 -> SBAT 放電
                {
                    if (!IsSecondBatteryTesting)
                    {
                        IsSecondBatteryTesting = true;
                    }

                    if (IsTwoMainBatteryProduct())
                    {
                        SetInternalBatteryDischarge();
                    }
                    else
                    {
                        while (Battery1Status > 0)
                        {
                            PublicFunction.CloseRTSPort();
                            MessageBoxResult Result = ShowDialogMessageBox("Please remove Main Battery.", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);

                            WaitBatteryInit();

                            if (Result.Equals(MessageBoxResult.No))
                            {
                                checkTestStatus("Is Main Battery Exist");
                                return;
                            }
                        }
                    }
                }
                else if (BatteryAmount.Equals(2) && Title.Content.Equals(SBAT_Discharge_Test)) // 測完 SBAT 放電 -> SBAT 充電
                {
                    if (IsTwoMainBatteryProduct())
                    {
                        if (IsFixtureExisted)
                        {
                            if (!PublicFunction.DisableRTS())
                            {
                                checkTestStatus("Can't control RTS Pin.");
                                return;
                            }
                        }
                        else
                        {
                            IsAcAdapterExist();
                        }
                    }
                    else
                    {
                        // SBAT 的剩餘容量大於充電Threshold直接當作小電池可以充電, 不多做確認
                        if (/*!GetBatteryChangeThreshold() || */!IsMainBatteryInserted())
                        {
                            return;
                        }
                    }
                }
                else if (Title.Content.Equals(SBAT_Charge_Test) && IsTwoMainBatteryProduct())
                {
                    if (!IsAcAdapterNotExist())
                    {
                        checkTestStatus("IsAcAdapterExist");
                        return;
                    }

                    if (!SetInternalBatteryDischarge())
                    {
                        checkTestStatus("IsInternalBatteryDischarge");
                        return;
                    }

                    while (Battery2Status > 0)
                    {
                        MessageBoxResult Result = ShowDialogMessageBox("Please remove Extend Battery.", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (Result.Equals(MessageBoxResult.No))
                        {
                            checkTestStatus("Is Extend Battery Exist");
                            return;
                        }
                    }
                }

                CorrectCount = 0;
                NowTestCount = 0;

                if ((BatteryAmount.Equals(1) && Title.Content.Equals(Battery_Mode_Test)) ||
                    Title.Content.Equals(SBAT_Charge_Test))
                {
                    checkTestStatus("PASS");
                }
                else
                {
                    if (!IsSecondBatteryTesting)
                    {
                        Title.Content = Battery_Mode_Test;
                        Trace.WriteLine(Battery_Mode_Test.PadRight(40, '='));
                    }
                    else if (Title.Content.Equals(Battery_Mode_Test))
                    {
                        Title.Content = SBAT_Discharge_Test;
                        Trace.WriteLine(SBAT_Discharge_Test.PadRight(40, '='));
                    }
                    else if (Title.Content.Equals(SBAT_Discharge_Test))
                    {
                        Title.Content = SBAT_Charge_Test;
                        Trace.WriteLine(SBAT_Charge_Test.PadRight(40, '='));
                    }

                    if (IsEcPollingSlowProduct()) WaitBatteryInit();

                    TimerBattery.Start();
                }
            }
            else if (NowTestCount >= BatteryTotalCount)
            {
                if (IsDebugMode) Trace.WriteLine("Battery : " + NowTestCount + " , TotalCount : " + BatteryTotalCount);
                checkTestStatus("FAIL");
            }
        }

        public static MessageBoxResult ShowDialogMessageBox(string text, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            return MessageBox.Show(text, title, buttons, icon);
        }

        public void checkTestStatus(string testResult)
        {
            Trace.WriteLine("testResult: " + testResult);

            if (TimerBattery.Enabled) TimerBattery.Stop();

            if (testResult.Equals("PASS"))
            {
                Trace.WriteLine("PASS");
                result["result"] = "PASS";
                result["EIPLog"] = new JObject
                {
                    { "Battery", "PASS" },
                    { "Battery_Info", "PASS"}
                };
            }
            else
            {
                Trace.WriteLine("FAIL");
                result["result"] = "FAIL";
                result["EIPLog"] = new JObject
                {
                    { "Battery", "FAIL" },
                    { "Battery_Info", testResult}
                };
            }

            // 完成Battery測試, 將計數器重新歸零
            NowTestCount = 0;
            CorrectCount = 0;
            BatteryErrorCode = 0;
            IsSecondBatteryTesting = false;

            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Send);
        }
    }
}
