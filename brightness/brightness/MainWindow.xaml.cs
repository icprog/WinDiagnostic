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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HotTabFunction;
using Newtonsoft.Json.Linq;

namespace brightness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Winmate WinIO DLL Declare
        [DllImport(@"WMIO2.dll")]
        public static extern bool SetLightSensorOnOff(uint uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetLightSensorChannel0(out uint uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetLightVariable(out uint uiValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetAccelerometer(out double uiXValue, out double uiYValue, out double uiZValue);

        [DllImport(@"WMIO2.dll")]
        public static extern bool SetAutoBrightnessStatus(bool bValue, uint iValue);
        #endregion

        #region Definition
        int TempBrightnessLevel = 0;
        bool IsBrightnessLevelOnceMinimum = false;
        bool IsBrightnessLevelOnceMaximum = false;
        uint ReadBatteryCurrentCount = 0;
        uint CountdownForUI = 0;
        const uint ReadBatteryCurrentTime = 10; // Timer Interval 為 500 ms
        const uint ReadLightSensorValueTime = 5; // Timer Interval 為 1000 ms
        decimal BrightnessLevelMaxAverageCurrent = 0;
        decimal BrightnessLevelMinAverageCurrent = 0;
        bool BrightnessCheck = false;
        bool IsDetectPanelCurrent = false;
        int BrightnessMidValue;
        bool IsTablet = true;
        bool IsAutoModeBrightness = true;
        bool IsSensorHubExist = true;
        bool IsLightSensorExist = false;       // 判斷是否有LightSensor, 並顯示隱藏LightSensor的欄位
        int TimerIntervalBrightness = 250;     // 判斷自動判斷Brightness階數時, 要多久跳一階(MS)
        int TimerIntervalPanelCurrentDetect = 750;  // 判斷自動判斷Brightness階數時, 要多久跳一階(MS)
        decimal PanelBrightnessCurrentThreshold = 100; // 當在相同Power Option下，Panel最亮跟最暗的電流至少要差多少才算是PASS
        System.Windows.Forms.Timer TimerBrightness;
        JObject result = new JObject();
        string TestProduct = string.Empty;
        int BrightnessMaximum;
        int BrightnessMinimum;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Loaded += brightness_Load;
        }

        public IntPtr GetHandle(Window w)
        {
            WindowInteropHelper h = new WindowInteropHelper(w);
            return h.Handle;
        }

        private void brightness_Load(object sender, RoutedEventArgs routedEventArgs)
        {
            result["result"] = "FAIL";
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Application.Current.Shutdown();
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            IsAutoModeBrightness = (bool)jobject.IsAutoModeBrightness;
            TestProduct = jobject.TestProduct.ToString();
            TimerIntervalBrightness = (int)jobject.TimerIntervalBrightness;
            IsSensorHubExist = (bool)jobject.IsSensorHubExist;
            BrightnessMaximum = (int)jobject.BrightnessMaximum;
            BrightnessMinimum = (int)jobject.BrightnessMinimum;
            BrightnessMidValue = BrightnessMaximum / 2;

            Trace.WriteLine("Brightness_Load");

            HotTabDLL.SetTestProduct(TestProduct);
            HotTabDLL.InitBrightness(GetHandle(this), 0);

            if (!IsTablet)
            {
                checkTestStatus("Non-TPC models cannot test Brightness.");
                return;
            }

            if (IsTablet) HotTabDLL.SetBrightness(0);

            slValue.Maximum = BrightnessMaximum;


            // 用Timer自動切換Brightness階數, 最後需要OP人工確認Panel是否正常
            if (IsAutoModeBrightness)
            {
                slValue.Value = 0;
                TimerBrightness = new System.Windows.Forms.Timer();
                //設定計時器的速度
                TimerBrightness.Interval = TimerIntervalBrightness;
                TimerBrightness.Tick += new EventHandler(TimerBrightness_Tick);
                TimerBrightness.Start();
            }
            // 純手動移動TrackBar(或是Button), 先調到最暗再調到最亮, 最後需要OP人工確認Panel是否正常
            else
            {

            }
        }

        static string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(exepath, path);
        }

        private void TimerBrightness_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!IsBrightnessLevelOnceMaximum)
                {
                    TempBrightnessLevel++;
                }
                else
                {
                    TempBrightnessLevel--;
                }

                slValue.Value = TempBrightnessLevel;

                if (TempBrightnessLevel.Equals(BrightnessMaximum))
                {
                    IsBrightnessLevelOnceMaximum = true;
                }
                else if (TempBrightnessLevel.Equals(0))
                {
                    IsBrightnessLevelOnceMaximum = false;
                    if (!BrightnessCheck) CheckBrightnessChangeNormally();
                }
            }
            catch (Exception ex)
            {
                checkTestStatus(ex.Message);
            }
        }

        private void FAIL_Click(object sender, RoutedEventArgs e)
        {
            checkTestStatus("FAIL");
        }

        public MessageBoxResult ShowDialogMessageBox(string text, string title, MessageBoxButton buttons, MessageBoxImage icon)
        {
            return MessageBox.Show(text, title, buttons, icon);            
        }

        private void DisableLightSensorDetect()
        {
            Trace.WriteLine("Disable LightSensorDetect");

            slValue.Value = BrightnessMidValue; // 4;

            if (IsSensorHubExist && IsTablet)
            {
                SetAutoBrightnessStatus(false, 1);
                SetAutoBrightnessStatus(false, 0);
            }
            else if (IsTablet) SetLightSensorOnOff(1);
        }

        private void checkTestStatus(String testResult)
        {
            if (IsAutoModeBrightness) TimerBrightness.Stop();

            DisableLightSensorDetect();

            if (testResult.Equals("PASS"))
            {
                result["result"] = "PASS";
                result["EIPLog"] = new JObject
                {
                    { "Brightness", "PASS" },
                    { "Brightness_Info", "PASS"}
                };
            }
            else
            {
                result["result"] = "FAIL";
                result["EIPLog"] = new JObject
                {
                    { "Brightness", "FAIL" },
                    { "Brightness_Info", testResult}
                };
            }

            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Send);
        }

        private void CheckBrightnessChangeNormally()
        {
            BrightnessCheck = true;
            MessageBoxResult Result;
            if (IsDetectPanelCurrent)
            {
                Result = MessageBoxResult.Yes;
            }
            else
            {
                Result = ShowDialogMessageBox("Does the panel brightness change normally?", "Attention", MessageBoxButton.YesNo, MessageBoxImage.Information);
            }

            if (Result.Equals(MessageBoxResult.Yes))
            {
                checkTestStatus("PASS");
            }
            else if (Result.Equals(MessageBoxResult.No))
            {
                checkTestStatus("Please retry or check Panel settings.");
            }
        }

        private void slValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsTablet) HotTabDLL.SetBrightness((byte)Convert.ToInt16(slValue.Value));
        }
    }
}
