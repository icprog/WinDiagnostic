using HotTabFunction;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace rotation
{
    public partial class rotation : Form
    {
        #region Definition
        Rectangle theScreenRect;

        string RotationRegistry = "";           // 用來儲存從Registry回傳的Rotation Value
        int RotationModeNow = -1;               // 目前的Rotation方向, 0:0 deg, 1:90 deg, 2:180 deg, 3: 270 deg
        int tempRotationState = 4;
        bool IsAccelerometerExist = false;      // 是否能抓到G-sensor的資料
        double AxisX = 0, AxisY = 0, AxisZ = 0; // G-sensor的三軸資料

        bool IsAccelerometerStable = false;     // 抓出來的G-sensor的三軸資料是否都在各自的上限值內
        uint AccelerometerDetectCount = 0;
        uint AccelerometerStableCount = 0;
        int RotationCount = 0;                  // 用以判斷目前Rotation目前轉了幾次

        bool RotationMode0 = false;
        bool RotationMode90 = false;
        bool RotationMode180 = false;
        bool RotationMode270 = false;
        bool IsDebugMode = true;
        double MaxAxisX = 0.05;
        double MinAxisX = -1;
        double MaxAxisY = 0.05;
        double MinAxisY = -1.05;
        double MaxAxisZ = 0.01;
        double MinAxisZ = -0.45;
        bool isRotationSupport = true;
        bool IsAutoModeRotation = true;
        HotTabRegistry registryHandler = new HotTabRegistry();
        bool isSensorHubExist = true;
        bool isNotGraphicsDriverSupport = false;
        bool IsTablet = false;
        string Win8RotationFlag = "";
        double OS_DPI = 1;
        uint ThresholdSensorStable = 3;
        int ThresholdTimeoutAutoMode = 10;
        uint StartOrientation = 0;
        bool ShowWindow = false;
        JObject result = new JObject();
        #endregion

        public rotation()
        {
            InitializeComponent();
        }

        string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(exepath, path);
        }

        void Exit()
        {
            if (Application.MessageLoop)
            {
                Application.Exit();
            }
            else
            {
                Environment.Exit(1);
            }
        }

        void rotation_Load(object sender, EventArgs e)
        {
            result["result"] = "FAIL";
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Exit();
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            isRotationSupport = (bool)jobject.RotationSupport;
            IsAutoModeRotation = (bool)jobject.IsAutoModeRotation;
            MinAxisX = (double)jobject.MinAxisX;
            MinAxisY = (double)jobject.MinAxisY;
            MinAxisZ = (double)jobject.MinAxisZ;
            MaxAxisX = (double)jobject.MaxAxisX;
            MaxAxisY = (double)jobject.MaxAxisY;
            MaxAxisZ = (double)jobject.MaxAxisZ;
            ShowWindow = (bool)jobject.ShowWindow;

            if (ShowWindow)
            {
                this.Opacity = 100;
                this.ShowInTaskbar = true;
            }

            if (IsDebugMode) Trace.WriteLine("Rotation_Load");

            if (isRotationSupport && isSensorHubExist)
            {
                if (isNotGraphicsDriverSupport && HotTabWMIInformation.IsWindows8or10)
                {

                    TimerRotationWin8.Start();
                }
                else
                {
                    groupBoxRotationMode.Visible = true;
                    TimerRotation.Start();
                    AccelerometerStableCount = 0;
                }
            }
            else
            {
                checkTestStatus("Not support Rotation Item.");
            }

            if (!IsAutoModeRotation)
            {
                Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(DisplaySettingsChanged);
                IsRotationLocked(false);
            }

            DisplaySettingsChangedEvent();

        }

        void IsRotationLocked(bool lockStatus)
        {
            if (IsDebugMode) Trace.WriteLine("IsRotationLocked - Status : " + lockStatus);
            if (!isSensorHubExist && isRotationSupport && HotTabWMIInformation.IsWindows7)
            {
                if (IsTablet) return;

                if (lockStatus)
                {
                    HotTabDLL.WinIO_SetRotationLock(1);
                }
                else
                {
                    HotTabDLL.WinIO_SetRotationLock(0);
                }
            }
            else if (isRotationSupport && HotTabWMIInformation.IsWindows8or10)
            {
                HotTabRegistry.RegistryRead("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AutoRotation", "Enable", ref Win8RotationFlag);
                if (IsDebugMode) Trace.WriteLine("Rotation Flag = " + Win8RotationFlag);

                if ((lockStatus && Win8RotationFlag.Equals("1")) ||
                    (!lockStatus && Win8RotationFlag.Equals("0"))
                   )
                {
                    HotTabDLL.keybd_event(0x5B, 0x00, 0x00, 0); // home key down
                    HotTabDLL.keybd_event(0x4F, 0x00, 0x00, 0); // home key down
                    HotTabDLL.keybd_event(0x4F, 0x00, 0x02, 0); // home key up
                    HotTabDLL.keybd_event(0x5B, 0x00, 0x02, 0); // home key up
                }
            }
            else
            {
                if (IsDebugMode) Trace.WriteLine("Screen Rotation Unlock Error. The system version is lower than the minimum OS version");
            }
        }

        void DisplaySettingsChanged(object sender, EventArgs e)
        {
            DisplaySettingsChangedEvent();
        }

        void DisplaySettingsChangedEvent()
        {
            RotationCount++;

            // 自動化的新方法只看IsAccelerometerStable, RotationCount跟4個方向Token都是要OP實際手去轉
            if (IsAccelerometerStable ||
                (RotationCount >= 4 &&
                (isNotGraphicsDriverSupport ||
                (!isNotGraphicsDriverSupport && RotationMode0 && RotationMode90 && RotationMode180 && RotationMode270))))
            {
                checkTestStatus("PASS");

                IsAccelerometerStable = false;
                RotationCount = 0;
                RotationMode0 = false;
                RotationMode90 = false;
                RotationMode180 = false;
                RotationMode270 = false;
            }

            theScreenRect = Screen.GetBounds(this);
            if (IsDebugMode)
            {
                Trace.WriteLine("Height : " + theScreenRect.Height + " , Width : " + theScreenRect.Width);
            }
            ScreenHeight.Text = (theScreenRect.Height * OS_DPI).ToString();
            ScreenWidth.Text = (theScreenRect.Width * OS_DPI).ToString();

            if (theScreenRect.Height > theScreenRect.Width)
            {
                DisplayMode.Text = "Portrait";
            }
            else
            {
                DisplayMode.Text = "Landscape";
            }
        }

        void buttonPASS_Click(object sender, EventArgs e)
        {
            checkTestStatus("PASS");
        }

        void buttonFAIL_Click(object sender, EventArgs e)
        {
            checkTestStatus("User canceled the test.");
        }

        void checkTestStatus(String testResult)
        {
            if (!IsAutoModeRotation) IsRotationLocked(true);

            // if (TimerRotationWin8.Enabled)
            TimerRotationWin8.Stop();
            // if (TimerRotation.Enabled)
            TimerRotation.Stop();

            buttonPASS.Enabled = false;
            buttonFAIL.Enabled = false;

            if (testResult.Equals("PASS"))
            {
                labelResult.Text = "PASS";
                labelResult.ForeColor = Color.Green;
                result["result"] = "PASS";
                result["EIPLog"] = new JObject
                {
                    { "Rotation", "PASS" },
                    { "Rotation_Info", "PASS"}
                };
            }
            else
            {
                labelResult.Text = "FAIL";
                labelResult.ForeColor = Color.Red;
                result["result"] = "FAIL";
                result["EIPLog"] = new JObject
                {
                    { "Rotation", "FAIL" },
                    { "Rotation_Info", testResult}
                };
            }

            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            if (!ShowWindow)
                Exit();
        }

        #region For SensorHub Exist (IB80/IH80)
        void TimerRotation_Tick(object sender, EventArgs e)
        {
            IsAccelerometerExist = HotTabDLL.GetAccelerometer(out AxisX, out AxisY, out AxisZ);
            if (IsDebugMode)
            {
                Trace.WriteLine("Is Accelerometer Exist? = " + IsAccelerometerExist + " , x = " + AxisX + " , y = " + AxisY + " , z = " + AxisZ);
            }
            SensorX.Text = AxisX.ToString();
            SensorY.Text = AxisY.ToString();
            SensorZ.Text = AxisZ.ToString();
            RotationMode.Text = ShowRotationState();

            if (IsAutoModeRotation)
            {
                AccelerometerDetectCount++;
                if ((MinAxisX < AxisX) && (AxisX < MaxAxisX) &&
                    (MinAxisY < AxisY) && (AxisY < MaxAxisY) &&
                    (MinAxisZ < AxisZ) && (AxisZ < MaxAxisZ))
                {
                    if (AccelerometerStableCount < ThresholdSensorStable)
                    {
                        AccelerometerStableCount++;
                    }
                    else
                    {
                        IsAccelerometerStable = true;
                        DisplaySettingsChangedEvent();
                    }
                }
                else if (AccelerometerDetectCount > ThresholdTimeoutAutoMode)
                {
                    checkTestStatus(string.Format("AccelerometerDetectCount: {0} > ThresholdTimeoutAutoMode: {1}", AccelerometerDetectCount, ThresholdTimeoutAutoMode));
                }
            }

            if (IsAccelerometerExist)
            {
                if ((AxisY < -0.55)) RotationModeNow = 0;
                else if ((AxisY > 0.55)) RotationModeNow = 2;
                else if ((AxisX < -0.55)) RotationModeNow = 3;
                else if ((AxisX > 0.55)) RotationModeNow = 1;

                if (tempRotationState != RotationModeNow)
                {
                    try
                    {
                        Int32 hwndMainForm;
                        hwndMainForm = HotTabDLL.FindWindow(null, "MainForm");
                        HotTabDLL.PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_CloseAllForm, 0, 0);
                    }
                    catch (Exception ex)
                    {
                        if (IsDebugMode)
                        {
                            Trace.WriteLine(ex.Message);
                        }
                    }

                    tempRotationState = RotationModeNow;
                    switch (RotationModeNow)
                    {
                        case 0:
                            RotationMode0 = true;
                            HotTabDLL.HotKey_ScreenRotation(0);
                            registryHandler.RegistryWrite("RotationState", "0");
                            break;
                        case 1:
                            RotationMode90 = true;
                            HotTabDLL.HotKey_ScreenRotation(90);
                            registryHandler.RegistryWrite("RotationState", "1");
                            break;
                        case 2:
                            RotationMode180 = true;
                            HotTabDLL.HotKey_ScreenRotation(180);
                            registryHandler.RegistryWrite("RotationState", "2");
                            break;
                        case 3:
                            RotationMode270 = true;
                            HotTabDLL.HotKey_ScreenRotation(270);
                            registryHandler.RegistryWrite("RotationState", "3");
                            break;
                    }
                }
            }
        }

        string ShowRotationState()
        {
            registryHandler.RegistryRead("RotationState", ref RotationRegistry);
            return RotationRegistry;
        }
        #endregion

        #region For CederTrail(M700D/ID90) with Windows 8 OS
        void TimerRotationWin8_Tick(object sender, EventArgs e)
        {
            // HotTabDLL.GetPortValExV(out RotationModeNow); // old method
            HotTabDLL.GetAngleDirection(out RotationModeNow);
            RotationMode.Text = ShowRotationState();

            if (tempRotationState != RotationModeNow)
            {
                try
                {
                    Int32 hwndMainForm;
                    hwndMainForm = HotTabDLL.FindWindow(null, "MainForm");
                    HotTabDLL.PostMessage(hwndMainForm, KeyboardHookEventArgs.ECEvent_CloseAllForm, 0, 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                tempRotationState = RotationModeNow;

                #region StartOrientation Switch Case
                // 依照StartOrientation分成4個模式, 是因為G-SENSOR擺的位置可能是不一樣的
                switch (StartOrientation)
                {
                    case 0: // ID90-250 
                        switch (RotationModeNow)
                        {
                            case 0:
                                HotTabDLL.HotKey_ScreenRotation(180);
                                registryHandler.RegistryWrite("RotationState", "2");
                                break;
                            case 1:
                                HotTabDLL.HotKey_ScreenRotation(270);
                                registryHandler.RegistryWrite("RotationState", "3");
                                break;
                            case 2:
                                HotTabDLL.HotKey_ScreenRotation(0);
                                registryHandler.RegistryWrite("RotationState", "0");
                                break;
                            case 3:
                                HotTabDLL.HotKey_ScreenRotation(90);
                                registryHandler.RegistryWrite("RotationState", "1");
                                break;
                        }
                        break;
                    case 1:
                        switch (RotationModeNow)
                        {
                            case 0:
                                HotTabDLL.HotKey_ScreenRotation(90);
                                registryHandler.RegistryWrite("RotationState", "1");
                                break;
                            case 1:
                                HotTabDLL.HotKey_ScreenRotation(180);
                                registryHandler.RegistryWrite("RotationState", "2");
                                break;
                            case 2:
                                HotTabDLL.HotKey_ScreenRotation(270);
                                registryHandler.RegistryWrite("RotationState", "3");
                                break;
                            case 3:
                                HotTabDLL.HotKey_ScreenRotation(0);
                                registryHandler.RegistryWrite("RotationState", "0");
                                break;
                        }
                        break;
                    case 2:
                        switch (RotationModeNow)
                        {
                            case 0:
                                HotTabDLL.HotKey_ScreenRotation(0);
                                registryHandler.RegistryWrite("RotationState", "0");
                                break;
                            case 1:
                                HotTabDLL.HotKey_ScreenRotation(90);
                                registryHandler.RegistryWrite("RotationState", "1");
                                break;
                            case 2:
                                HotTabDLL.HotKey_ScreenRotation(180);
                                registryHandler.RegistryWrite("RotationState", "2");
                                break;
                            case 3:
                                HotTabDLL.HotKey_ScreenRotation(270);
                                registryHandler.RegistryWrite("RotationState", "3");
                                break;
                        }
                        break;
                    case 3:
                        switch (RotationModeNow)
                        {
                            case 1:
                                HotTabDLL.HotKey_ScreenRotation(0);
                                registryHandler.RegistryWrite("RotationState", "0");
                                break;
                            case 2:
                                HotTabDLL.HotKey_ScreenRotation(90);
                                registryHandler.RegistryWrite("RotationState", "1");
                                break;
                            case 3:
                                HotTabDLL.HotKey_ScreenRotation(180);
                                registryHandler.RegistryWrite("RotationState", "2");
                                break;
                            case 0:
                                HotTabDLL.HotKey_ScreenRotation(270);
                                registryHandler.RegistryWrite("RotationState", "3");
                                break;
                        }
                        break;
                }
                #endregion
            }
            #endregion
        }
    }
}
