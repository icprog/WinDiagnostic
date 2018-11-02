using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace lightsensor
{
    public partial class lightsensor : Form
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

        [DllImport(@"WMIO2.dll")]
        public static extern bool GetACStatus(out uint uiValue);
        #endregion

        #region Definition
        uint CountReadLightSensor = 0;
        bool IsDebugMode = true;
        bool isLightSensorExist = false;       // 判斷是否有LightSensor, 並顯示隱藏LightSensor的欄位
        bool isSensorHubExist = true;         // 判斷LightSensor是否走得是SensorHub還是EC
        double LIGHT_SENSOR_COEFFICIENT = 0.05;     // 判斷Light Sensor被校正過的數值是否符合FAE調整的校正值, IB80/IH80/M9020B/IBCMC=0.05, Taurus(FM10/FM08) = 0.028, IH83=0.02
        uint LightSensorMaxThreshold = 1000;        // Light Sensor最小值應該至少低於多少
        uint LightSensorMinThreshold = 5;           // Light Sensor最大值應該至少高於多少
        int TimerIntervalLightSensor = 250;         // 自動判斷LightSensor時, 要多久讀一次LightSensor數值
        bool IsSensorHubBuildInCPU = false;         // SersorHub是不是內建在CPU裡面
        int ThresholdTimeoutAutoMode = 10;
        bool IsTablet = false;
        bool IsIQCDatabase = false;
        bool IsFixtureExisted = true;
        SerialPort serialPortRTS = null;
        readonly string USB_RTS_Comport_SN = "RTS";
        string PortAddressRTS = "COM11";
        uint lightSensorTemp = 0;
        uint lightSensorMaxValue = 100;
        uint lightSensorMinValue = 100;
        int RetryCount = 0;
        int isChargingNow = -1;
        JObject result = new JObject();
        bool ShowWindow = false;

        #endregion

        static SelectQuery PnPEntityQuery = new SelectQuery("SELECT * FROM Win32_PnPEntity");
        ManagementObjectSearcher PnPEntitySearcher = new ManagementObjectSearcher(PnPEntityQuery);

        SerialPort SerialPortRTS
        {
            get
            {
                try
                {
                    if (serialPortRTS == null)
                    {
                        //if (IsDebugMode) Trace.WriteLine("serialPortRTS == null");
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

        public bool IsCharging
        {
            get
            {
                // AC_IN=119, AC_OUT=82
                // HotTabDLL.GetACInOutStatus(out isChargingNow);

                //winmate brian 20180515 modify >>>
                uint ivalue = 0;
                if (GetACStatus(out ivalue))
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

        void Deinit()
        {
            if (SerialPortRTS != null && SerialPortRTS.IsOpen)
            {
                SerialPortRTS.Close();
                Trace.WriteLine("Close SerialPortRTS in " + Name);
            }
        }

        public lightsensor()
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

        void lightsensor_Load(object sender, EventArgs e)
        {
            result["result"] = "FAIL";
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Exit();
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));

            ShowWindow = (bool)jobject.ShowWindow;
            LIGHT_SENSOR_COEFFICIENT = (double)jobject.LIGHT_SENSOR_COEFFICIENT;
            TimerIntervalLightSensor = (int)jobject.TimerIntervalLightSensor;
            LightSensorMaxThreshold = (uint)jobject.LightSensorMaxThreshold;
            LightSensorMinThreshold = (uint)jobject.LightSensorMinThreshold;
            PortAddressRTS = jobject.PortAddressRTS.ToString();

            if (ShowWindow)
            {
                this.Opacity = 100;
                this.ShowInTaskbar = true;
            }

            if (IsDebugMode) Trace.WriteLine("LightSensor_Load");

            if (isSensorHubExist)
            {
                buttonFAIL.Enabled = true;
                groupLightSensor.Enabled = true;

                labelLighSensorRule.Text = "Min<" + LightSensorMinThreshold + "  Max>" + LightSensorMaxThreshold; // Min<5   Max>1000
                if (IsSensorHubBuildInCPU)
                {
                    buttonCheckLightSensorValue.Enabled = true;
                    buttonCheckLightSensorValue.PerformClick();
                }
                else
                {
                    buttonSensorHub.Enabled = true;
                    GetSensorHubValue();
                }
            }
            else
            {
                checkTestStatus("LightSensor is not support this product.");
            }
        }

        void buttonSensorHub_Click(object sender, EventArgs e)
        {
            GetSensorHubValue();
        }

        void GetSensorHubValue()
        {
            // 只有由SensorHub去控制LightSensor才需要額外用STMicroSensorHub工具去抓SensorHub的值
            try
            {
                buttonSensorHub.Enabled = false;
                if (CultureInfo.CurrentCulture.Name.Equals("en-US") || CultureInfo.CurrentCulture.Name.Equals("zh-TW"))
                {
                    #region Step 1 : 先執行截取 SensorHub 數值的程式
                    // [WAY1} 程式碼最少, 但是會跳出DOS視窗, 執行後後自行關閉

                    using (var mSensorHubGeter = new Process())
                    {
                        mSensorHubGeter.StartInfo.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                        mSensorHubGeter.StartInfo.FileName = "cmd.exe";
                        mSensorHubGeter.StartInfo.Arguments = @"/c SensorHubGeter.bat";
                        mSensorHubGeter.Start();
                        mSensorHubGeter.WaitForExit();
                    }
                    #endregion

                    #region Step 2 : 將程式的output parse出想要的SensorHub校正值(LIGHT_SENSOR_COEFFICIENT)
                    // [WAY2] 速度較快/
                    foreach (string line in File.ReadLines(Application.StartupPath + "\\sensor\\als_cal_coeff.txt"))
                    {
                        // 當遇到目標欄位時, 直接抓數字的第一個索引, 取其後的值出來顯示在UI
                        if (line.Contains("LIGHT_SENSOR_COEFFICIENT"))
                        {
                            string[] SensorValue = line.Split(new string[] { " = " }, StringSplitOptions.RemoveEmptyEntries);
                            if (IsDebugMode) Trace.WriteLine("Light Sensor Value = " + SensorValue[1]);
                            labelSensorHub.Text = SensorValue[1];

                            // 當Light Sensor被校正過, 其值為符合FAE調整的校正值, 才給OP進一步確認Light Sensor做動是否正常
                            // IQC進行驗證時, 關閉LightSensor校正值的檢查, 因為這個數值是由產線出貨前才用Tool修改
                            if (!IsIQCDatabase && !(SensorValue[1] == LIGHT_SENSOR_COEFFICIENT.ToString()))
                            {
                                checkTestStatus("LightSensor returns wrong coefficient value : " + SensorValue[1].ToString());
                            }
                            else
                            {
                                buttonCheckLightSensorValue.Enabled = true;
                                buttonCheckLightSensorValue.PerformClick();
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    checkTestStatus("Non-English OS does not support SensorHub Value Geter.");
                }
            }
            catch (UnauthorizedAccessException UAEx)
            {
                checkTestStatus(UAEx.Message);
            }
            catch (PathTooLongException PathEx)
            {
                checkTestStatus(PathEx.Message);
            }
            catch (Exception ex)
            {
                checkTestStatus(ex.Message);
            }
        }

        void DisableLightSensorDetect()
        {
            if (IsDebugMode) Trace.WriteLine("Disable LightSensorDetect");
            if (isSensorHubExist && IsTablet)
            {
                SetAutoBrightnessStatus(false, 1);
                SetAutoBrightnessStatus(false, 0);
            }
            else if (IsTablet) SetLightSensorOnOff(1);
        }

        void buttonCheckLightSensorValue_Click(object sender, EventArgs e)
        {
            try
            {
                if (buttonCheckLightSensorValue.Enabled)
                {
                    if (isSensorHubExist) // 不管在AC模式或是電池模式都要把Lightsensor設成開啟(true)
                    {
                        if (IsDebugMode) Trace.WriteLine("isLightSensorHub, SetAutoBrightnessStatus = TRUE");
                        SetAutoBrightnessStatus(true, 1);
                        SetAutoBrightnessStatus(true, 0);
                    }
                    else SetLightSensorOnOff(1); // 不是走SensorHub的 

                    if (IsFixtureExisted)
                    {
                        CountReadLightSensor = 0;
                    }
                    TimerLightSensor.Interval = TimerIntervalLightSensor;
                    TimerLightSensor.Start();
                }
                else
                {
                    DisableLightSensorDetect();
                    labelLightSensor.Text = "";
                    TimerLightSensor.Stop();
                }
                buttonCheckLightSensorValue.Enabled = false;
            }
            catch (Exception ex)
            {
                buttonCheckLightSensorValue.Enabled = false;
                checkTestStatus("Error! Please check LightSensor firmware version.");
                if (IsDebugMode) Trace.WriteLine(ex.Message);
            }
        }

        int TimeoutThresholdFactor(int TimerInterval)
        {
            return ((1000 / TimerInterval) > 0) ? (1000 / TimerInterval) : 1;
        }

        bool EnableRTS()
        {
            if (SerialPortRTS == null) return false;

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

        bool DisableRTS()
        {
            if (SerialPortRTS == null) return false;

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
                return true;
            }

            RetryCount = 0;
            while (!IsCharging)
            {
                RetryCount++;
                if (RetryCount > 20)
                {
                    return false;
                }
                else
                {
                    Thread.Sleep(300);
                    Trace.WriteLine("Can't switch RTS Pin to Disable (AC In).");
                    SerialPortRTS.RtsEnable = false; // RTS disable 供電
                }
            }
            return true;
        }

        void TimerLightSensor_Tick(object sender, EventArgs e)
        {
            try
            {
                #region IsAutoModeLightSensor
                if (IsFixtureExisted)
                {
                    if (CountReadLightSensor.Equals(0))
                    {
                        if (!EnableRTS())
                        {
                            checkTestStatus("Enable RTS (AC Out) FAIL");
                            return;
                        }
                        labelTitle.Text = "Wait for Read Sensor minimum";
                        labelTitle.ForeColor = Color.Orange;
                        labelTitle.Update();
                    }
                    else if (CountReadLightSensor.Equals(ThresholdTimeoutAutoMode) || (lightSensorMinValue < LightSensorMinThreshold))
                    {
                        if (!DisableRTS())
                        {
                            checkTestStatus("Disable RTS (AC In) FAIL");
                            return;
                        }
                        labelTitle.Text = "Wait for Read Sensor maximum";
                        labelTitle.ForeColor = Color.Orange;
                        labelTitle.Update();
                    }
                    else if (CountReadLightSensor.Equals(ThresholdTimeoutAutoMode * TimeoutThresholdFactor(TimerLightSensor.Interval)))
                    {
                        checkTestStatus("LightSensor detect FAIL");
                    }

                    CountReadLightSensor++;
                    if (IsDebugMode) Trace.WriteLine("TimerLightSensor : " + CountReadLightSensor);
                }
                #endregion

                bool bRet = false;
                if (isSensorHubExist) bRet = GetLightVariable(out lightSensorTemp);
                else bRet = GetLightSensorChannel0(out lightSensorTemp);

                SetAutoBrightnessStatus(true, 1);
                SetAutoBrightnessStatus(true, 0);

                if (bRet)
                {
                    labelLightSensor.Text = lightSensorTemp.ToString();
                    labelLightSensor.Update();

                    if (lightSensorTemp > lightSensorMaxValue)
                    {
                        lightSensorMaxValue = lightSensorTemp;
                        if (IsDebugMode)
                        {
                            Trace.WriteLine("LightSensorMaxValue : " + lightSensorMaxValue + "\tLightSensorMaxThreshold : " + LightSensorMaxThreshold);
                        }
                    }
                    if (lightSensorTemp < lightSensorMinValue)
                    {
                        lightSensorMinValue = lightSensorTemp;
                        if (IsDebugMode)
                        {
                            Trace.WriteLine("LightSensorMinValue : " + lightSensorMinValue + "\tLightSensorMinThreshold : " + LightSensorMinThreshold);
                        }
                    }

                    // LightSensor大於最大值跟小於最小值, 自動判LightSensor測項PASS
                    if ((lightSensorMaxValue > LightSensorMaxThreshold) && (lightSensorMinValue < LightSensorMinThreshold))
                    {
                        checkTestStatus("PASS");
                    }
                }

                if (IsDebugMode) Trace.WriteLine("LightSensor : " + bRet + "\tValue : " + lightSensorTemp);
            }
            catch (Exception ex)
            {
                checkTestStatus(ex.Message);
            }
        }

        void buttonFAIL_Click(object sender, EventArgs e)
        {
            checkTestStatus("User canceled the test.");
        }

        void checkTestStatus(String testResult)
        {
            Trace.WriteLine(testResult);
            if (TimerLightSensor.Enabled) TimerLightSensor.Stop();
            if (IsDebugMode)
            {
                Trace.WriteLine("MaxValue : " + lightSensorMaxValue.ToString().PadRight(4, ' ') + "\tMaxThreshold : " + LightSensorMaxThreshold);
                Trace.WriteLine("MinValue : " + lightSensorMinValue.ToString().PadRight(4, ' ') + "\tMinThreshold : " + LightSensorMinThreshold);
            }

            labelTitle.Text = "LightSensor";
            labelTitle.ForeColor = Color.Black;
            labelTitle.Update();

            if (testResult.Equals("PASS"))
            {
                labelResult.ForeColor = Color.Green;
                labelResult.Text = "PASS";
                result["result"] = "PASS";
                result["EIPLog"] = new JObject
                {
                    { "LightSensor", "PASS" },
                    { "LightSensor_Info", "PASS"}
                };
            }
            else
            {
                labelResult.ForeColor = Color.Red;
                labelResult.Text = "FAIL";
                result["result"] = "FAIL";
                result["EIPLog"] = new JObject
                {
                    { "LightSensor", "FAIL" },
                    { "LightSensor_Info", testResult}
                };
            }

            buttonPASS.Enabled = false;
            buttonFAIL.Enabled = false;
            buttonCheckLightSensorValue.Enabled = false;
            buttonSensorHub.Enabled = false;
            Deinit();
            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            if (!ShowWindow)
                Exit();
        }
    }
}
