using HotTabFunction;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

namespace led
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Definition
        private int checkCount = 0;         // 目前測了幾個LED燈號
        private int checkSuccessCount = 0;  // 目前有有幾個LED燈號測試成功
        private int TotalTestLEDCount = 0;  // 總共有幾個LED燈號需要測試(在LED_Load中初始化)
        // 用來控制LED測試燈號的ON /OFF切換
        private int IsLedBatteryEnabled = 1;
        private int IsLedRfEnabled = 1;
        private int IsLedGpsEnabled = 1;
        private bool SwitchBT = true;
        private bool SwitchWiFi = true;
        private bool SwitchStop = true;

        // 用來控制Battery / RF LED 亂數的亮暗行為
        private Random RandomGenerator = new Random();
        private int MinValue = 0;
        private int MaxValue = 2;

        // WINDY BOX LED USE
        private int SwitchLED1R = 0;
        private int SwitchLED1G = 0;
        private int SwitchLED1B = 0;
        private int SwitchLED2R = 0;
        private int SwitchLED2G = 0;
        private int SwitchLED2B = 0;
        private int SwitchLED3R = 0;
        private int SwitchLED3G = 0;
        private int SwitchLED3B = 0;
        bool IsDebugMode = true;
        string TestProduct = "IB80";
        bool IsBiosTestModeEnabled = false;
        bool EnableGPS = false;
        public int MCU_Comport = 34;
        bool EnablePower = true;
        bool EnableHDD = true;
        bool EnableRF = true;
        int EnableBattery = 1;
        bool IsAutoModeBatteryLED = false;
        bool IsFixtureExisted = true;
        System.Windows.Forms.Timer timer;
        JObject result = new JObject();
        #endregion


        public MainWindow()
        {
            InitializeComponent();
            Loaded += led_Load;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (PublicFunction.IsCharging)
            {
                buttonBatteryLEDPass.IsEnabled = true;
                timer.Enabled = false;
            }
        }

        string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(exepath, path);
        }

        private void led_Load(object sender, RoutedEventArgs routedEventArgs)
        {
            result["result"] = "FAIL";
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Application.Current.Shutdown();
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            TestProduct = jobject.TestProduct.ToString();
            IsAutoModeBatteryLED = (bool)jobject.IsAutoModeBatteryLED;
            EnablePower = (bool)jobject.EnablePower;
            EnableHDD = (bool)jobject.EnableHDD;
            EnableRF = (bool)jobject.EnableRF;
            EnableBattery = (int)jobject.EnableBattery;
            IsBiosTestModeEnabled = (bool)jobject.IsBiosTestModeEnabled;

            if (IsDebugMode) Trace.WriteLine("LED_Load");

            timer = new System.Windows.Forms.Timer();
            timer.Interval = 750;
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();

            // 當MCU測項有打開就連到MCU_Comport
            if (TestProduct.Equals("IBWD"))
            {
                if (HotTabDLL.DeviceConnect(9600, MCU_Comport) == 0)
                {
                    if (IsDebugMode) Trace.WriteLine("MCU_Comport : " + MCU_Comport);
                    //groupBoxWindyBox.Visible = true;
                    //groupBoxWindyBox.Location = new Point(652, 304);
                }
                else
                {
                    if (IsDebugMode) Trace.WriteLine("Can't connect to MCU_Comport : " + MCU_Comport);
                    ShowDialogMessageBox("Can't connect to MCU comport : " + MCU_Comport, "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                    checkSuccessCount = -1;
                    checkTestStatus("CheckIt");
                }
            }
            else
            {
                if (EnablePower)
                {
                    TotalTestLEDCount++;
                    groupPower.Visibility = Visibility.Visible;
                }

                if (EnableBattery.Equals(1))
                {
                    TotalTestLEDCount++;
                    BatteryLED.Visibility = Visibility.Visible;
                    if (IsFixtureExisted && IsAutoModeBatteryLED)
                    {
                        buttonBatteryLEDPass.IsEnabled = false;
                        buttonBatteryLEDFail.IsEnabled = false;
                        buttonBatteryLEDTest.Visibility = Visibility.Visible;
                        TotalTestLEDCount++; // Battery LED 需要各偵測ON/OFF狀態各1次
                    }
                    else
                    {
                        //buttonBatteryLEDPass.IsEnabled = true;
                        labelTestBattery.Visibility = Visibility.Visible;
                    }
                }
                else if (EnableBattery.Equals(2))
                {
                    TotalTestLEDCount += 2;
                    BatteryLED.Visibility = Visibility.Visible;
                    //groupBattery2.Visible = true;
                }

                if (EnableHDD)
                {
                    TotalTestLEDCount++;
                    groupHDD.Visibility = Visibility.Visible;
                }

                if (EnableGPS) // 除ID82 / IB10X 外, 其他機種不顯示GPS LED項目
                {
                    TotalTestLEDCount++;
                    //groupGPS.Visible = true;
                }

                // SupportModelName, LED
                if (TestProduct.Equals("FMB8"))
                {
                    TotalTestLEDCount += 3;
                    groupRF.Visibility = Visibility.Visible;
                    //groupBoxBluetooth.Visible = true;
                    //groupBoxWiFi.Visible = true;
                    //groupBoxStop.Visible = true;
                    // 預設將 BT / WiFi / Stop LED 先開啟
                    HotTabDLL.WinIO_WriteToECSpace(0x46, 0x01); // BTLED
                    HotTabDLL.WinIO_WriteToECSpace(0x3D, 0x01); // WIFILED
                    HotTabDLL.WinIO_WriteToECSpace(0x47, 0x01); // STOPLED
                }
                else if (EnableRF)
                {
                    if (TestProduct.Equals("IBWH")) // HandHeld 8"
                    {
                        groupRF.Content = "Custom LED";
                    }
                    TotalTestLEDCount += 2; // RF 需要各偵測ON/OFF狀態各1次
                    groupRF.Visibility = Visibility.Visible;
                    buttonRFLEDPass.IsEnabled = false;
                    buttonRFLEDFail.IsEnabled = false;
                }

                if (IsBiosTestModeEnabled)
                {
                    // TotalTestLEDCount ++; // Battery LED 需要各偵測ON/OFF狀態各1次
                    TestMode.SetTestModeLED(0x07);
                    buttonBatteryLEDTest.Visibility = Visibility.Visible;
                    buttonBatteryLEDTest.IsEnabled = true;
                    buttonBatteryLEDTest.Content = "Test";
                    buttonBatteryLEDTest.Foreground = Brushes.Blue;
                    buttonRFLEDTest.Content = "Test";
                    buttonRFLEDTest.Foreground = Brushes.Blue;
                    buttonBatteryLEDPass.IsEnabled = false;
                    buttonBatteryLEDFail.IsEnabled = false;
                    // buttonRFLEDPass.Enabled = false;
                    // buttonRFLEDFail.Enabled = false;
                }
            }
        }

        private void checkTestStatus(string testResult)
        {
            checkCount++;
            if (IsDebugMode) Trace.WriteLine("Total : " + TotalTestLEDCount + "\tNow : " + checkCount + "\tSuccess : " + checkSuccessCount);

            if ((checkSuccessCount == TotalTestLEDCount) || (TestProduct.Equals("IBWD") && checkSuccessCount.Equals(10)))
            {
                Trace.WriteLine("PASS");
                result["result"] = "PASS";
                File.WriteAllText(GetFullPath("result.json"), result.ToString());
                Thread.Sleep(200);
                File.Create(GetFullPath("completed"));
                Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Send);
            }
            else if ((checkCount == TotalTestLEDCount) || (TestProduct.Equals("IBWD") && checkSuccessCount.Equals(-1)))
            {
                Trace.WriteLine("FAIL");
                result["result"] = "FAIL";
                File.WriteAllText(GetFullPath("result.json"), result.ToString());
                Thread.Sleep(200);
                File.Create(GetFullPath("completed"));
                Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Send);
            }
        }

        private void buttonPowerLED_Click(object sender, RoutedEventArgs e)
        {
            Button mButton = (Button)sender;
            if (mButton.Name.Equals(buttonPowerLEDPass.Name))
            {
                checkSuccessCount++;
            }

            groupPower.IsEnabled = false;
            checkTestStatus("CheckIt");
        }

        private void buttonHDDLED_Click(object sender, RoutedEventArgs e)
        {
            Button temp = (Button)sender;
            if (temp.Name.Equals(buttonHDDLEDPass.Name))
            {
                checkSuccessCount++;
            }

            groupHDD.IsEnabled = false;
            checkTestStatus("CheckIt");
        }

        private void buttonRFLED_Click(object sender, RoutedEventArgs e)
        {
            Button temp = (Button)sender;

            if (temp.Name.Equals(buttonRFLEDPass.Name))
                checkSuccessCount++;

            groupRF.IsEnabled = false;
            checkTestStatus("CheckIt");
        }

        private void buttonHDDLEDTest_Click(object sender, RoutedEventArgs e)
        {
            buttonHDDLEDTest.IsEnabled = false;
            buttonHDDLEDPass.IsEnabled = false;
            buttonHDDLEDFail.IsEnabled = false;
            try
            {
                // Process.Start("cmd.exe", "/c defrag c: -a -u -v");
                Process.Start(@"C:\Program Files\Windows Defender\MSASCui.exe", " -QuickScan");
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                ShowDialogMessageBox(ex.Message, "Attention", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ex.Message);
            }
            buttonHDDLEDTest.IsEnabled = true;
            buttonHDDLEDPass.IsEnabled = true;
            buttonHDDLEDFail.IsEnabled = true;
        }

        private void buttonRFLEDTest_Click(object sender, RoutedEventArgs e)
        {
            IsLedRfEnabled = RandomGenerator.Next(MinValue, MaxValue);
            Trace.WriteLine("IsLedRfEnabled : " + IsLedRfEnabled);
            try
            {
                // if (IsBiosTestModeEnabled)
                // {
                //     buttonTestRF.Enabled = false;
                //     // 因為要亂數測試 ON / OFF 狀態, TestRfLedOnOff()要呼叫兩次
                //     TestLedOnOffRF();
                // 
                //     if (IsLedRfEnabled.Equals(1)) IsLedRfEnabled = 0;
                //     else IsLedRfEnabled = 1;
                // 
                //     TestLedOnOffRF();
                // }
                // else
                // {
                //     if (TestProduct.Equals("IBWH")) SwitchLedCustom();
                //     else SwitchLedRF();
                //     if (buttonTestRF.Text.Equals("ON") && !buttonRFLEDPass.Enabled)
                //     {
                //         buttonRFLEDPass.Enabled = true;
                //     }
                // }

                buttonRFLEDTest.IsEnabled = false;
                // 因為要亂數測試 ON / OFF 狀態, TestRfLedOnOff()要呼叫兩次
                TestLedOnOffRF();

                if (IsLedRfEnabled.Equals(1)) IsLedRfEnabled = 0;
                else IsLedRfEnabled = 1;

                TestLedOnOffRF();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void SwitchLedCustom()
        {
            if (IsLedRfEnabled % 2 == 1)
            {
                buttonRFLEDTest.Foreground = Brushes.Green;
                buttonRFLEDTest.Content = "ON";
                HotTabDLL.WinIO_SetDeviceState(0x77);
                HotTabDLL.WinIO_SetDevice2State(0x4E);
                if (IsDebugMode) Trace.WriteLine("LED - SetDeviceState(0x77), SetDeviceState(0x4E)");
            }
            else if (IsLedRfEnabled % 2 == 0)
            {
                buttonRFLEDTest.Foreground = Brushes.Red;
                buttonRFLEDTest.Content = "ON";
                HotTabDLL.WinIO_SetDeviceState(0x37);
                HotTabDLL.WinIO_SetDevice2State(0x0E);
                if (IsDebugMode) Trace.WriteLine("LED - SetDeviceState(0x37), SetDeviceState(0x0E)");
            }
        }

        private void TestLedOnOffRF()
        {
            if (IsBiosTestModeEnabled)
            {
                if (IsLedRfEnabled.Equals(1)) // 沒有 GPS 獨立燈號
                {
                    TestMode.SetTestModeLED((byte)TestMode.ListLED.RF);
                    if (IsDebugMode) Trace.WriteLine("BTZ1 RF LED ON");
                }
                else if (IsLedRfEnabled.Equals(0)) // 沒有 GPS 獨立燈號
                {
                    TestMode.SetTestModeLED(0x00);
                    if (IsDebugMode) Trace.WriteLine("BTZ1 RF LED OFF");
                }
            }
            else
            {
                if (TestProduct.Equals("IBWH")) SwitchLedCustom();
                else SwitchLedRF();
            }

            MessageBoxResult Result = ShowDialogMessageBox("Please confirm RF LED Status?\r\n( ON = Yes, OFF = No )", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if ((Result.Equals(MessageBoxResult.Yes) && IsLedRfEnabled.Equals(1)) ||
                (Result.Equals(MessageBoxResult.No) && IsLedRfEnabled.Equals(0)))
            {
                if (IsDebugMode) Trace.WriteLine("RF LED PASS");
                checkSuccessCount++;
            }
            else
            {
                if (IsDebugMode) Trace.WriteLine("RF LED FAIL");
            }

            HotTabDLL.WinIO_SetDeviceState(0xB7);
            if (IsDebugMode) Trace.WriteLine("LED - SetDeviceState(0xB7)");
            checkTestStatus("CheckIt");
        }

        private void SwitchLedRF()
        {
            if (EnableGPS) // 有 GPS 獨立燈號
            {
                if (IsLedRfEnabled.Equals(1) && IsLedGpsEnabled % 2 == 1)
                {
                    buttonRFLEDTest.Foreground = Brushes.Green;
                    buttonRFLEDTest.Content = "ON";
                    HotTabDLL.WinIO_SetDeviceState(0xFF);
                    if (IsDebugMode) Trace.WriteLine("LED - SetDeviceState(0xFF)");
                }
                else if (IsLedRfEnabled.Equals(1) && IsLedGpsEnabled % 2 == 0)
                {
                    buttonRFLEDTest.Foreground = Brushes.Green;
                    buttonRFLEDTest.Content = "ON";
                    HotTabDLL.WinIO_SetDeviceState(0x7B);
                    if (IsDebugMode) Trace.WriteLine("LED - SetDeviceState(0x7B)");
                }
                else if (IsLedRfEnabled.Equals(0) && IsLedGpsEnabled % 2 == 1)
                {
                    buttonRFLEDTest.Foreground = Brushes.Red;
                    buttonRFLEDTest.Content = "OFF";
                    HotTabDLL.WinIO_SetDeviceState(0xAC);
                    if (IsDebugMode) Trace.WriteLine("LED - SetDeviceState(0xAC)");
                }
                else
                {
                    buttonRFLEDTest.Foreground = Brushes.Red;
                    buttonRFLEDTest.Content = "OFF";
                    HotTabDLL.WinIO_SetDeviceState(0x00);
                    if (IsDebugMode) Trace.WriteLine("LED - SetDeviceState(0x00)");
                }
            }
            else if (IsLedRfEnabled.Equals(1)) // 沒有 GPS 獨立燈號
            {
                buttonRFLEDTest.Foreground = Brushes.Green;
                buttonRFLEDTest.Content = "ON";
                HotTabDLL.WinIO_SetDeviceState(0xB7);
                if (IsDebugMode) Trace.WriteLine("LED - SetDeviceState(0xB7)");
            }
            else if (IsLedRfEnabled.Equals(0)) // 沒有 GPS 獨立燈號
            {
                buttonRFLEDTest.Foreground = Brushes.Red;
                buttonRFLEDTest.Content = "OFF";
                HotTabDLL.WinIO_SetDeviceState(0x37);
                if (IsDebugMode) Trace.WriteLine("LED - SetDeviceState(0x37)");
            }
        }

        private void TestLedOnOffBattery()
        {
            if (IsLedBatteryEnabled.Equals(1))
            {
                if (IsBiosTestModeEnabled) TestMode.SetTestModeLED((byte)TestMode.ListLED.Red | (byte)TestMode.ListLED.Green);
                else if (!PublicFunction.DisableRTS())
                {
                    checkTestStatus("Disable RTS FAIL");
                    return;
                }
                if (IsDebugMode) Trace.WriteLine("Battery LED ON");
            }
            else if (IsLedBatteryEnabled.Equals(0))
            {
                if (IsBiosTestModeEnabled) TestMode.SetTestModeLED(0x00);
                else if (!PublicFunction.EnableRTS())
                {
                    checkTestStatus("Enable RTS FAIL");
                    return;
                }
                if (IsDebugMode) Trace.WriteLine("Battery LED OFF");
            }

            MessageBoxResult Result = ShowDialogMessageBox("Please confirm Battery LED Status?\r\n( ON = Yes, OFF = No )", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if ((Result.Equals(MessageBoxResult.Yes) && IsLedBatteryEnabled.Equals(1)) ||
                (Result.Equals(MessageBoxResult.No) && IsLedBatteryEnabled.Equals(0)))
            {
                if (IsDebugMode) Trace.WriteLine("Battery LED PASS");
                checkSuccessCount++;
            }
            else
            {
                if (IsDebugMode) Trace.WriteLine("Battery LED FAIL");
            }
            checkTestStatus("CheckIt");
        }

        private void buttonBatteryLEDTest_Click(object sender, RoutedEventArgs e)
        {
            buttonBatteryLEDTest.IsEnabled = false;
            buttonBatteryLEDPass.IsEnabled = false;
            buttonBatteryLEDFail.IsEnabled = false;

            IsLedBatteryEnabled = RandomGenerator.Next(MinValue, MaxValue);
            Trace.WriteLine("IsLedBatteryEnabled : " + IsLedBatteryEnabled);
            TestLedOnOffBattery();

            if (IsLedBatteryEnabled.Equals(1)) IsLedBatteryEnabled = 0;
            else IsLedBatteryEnabled = 1;

            TestLedOnOffBattery();
        }

        private void buttonBatteryLED_Click(object sender, RoutedEventArgs e)
        {
            Button mButton = (Button)sender;
            if (mButton.Name.Equals(buttonBatteryLEDPass.Name))
            {
                checkSuccessCount++;
            }

            BatteryLED.IsEnabled = false;
            checkTestStatus("CheckIt");
        }

        public MessageBoxResult ShowDialogMessageBox(string text, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            return MessageBox.Show(text, title, buttons, icon);
        }
    }
}
