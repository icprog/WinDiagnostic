using HotTabFunction;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace gps
{
    public partial class gps : Form
    {
        #region Definition
        private const int DataDetectTimeWithoutPositioning = 5;
        private const int DataDetectTimeMinThreshold = 3;
        private SerialPort mSerialPortGPS;
        private System.Windows.Forms.Timer mTimerGPS;
        private ListViewItem satellitesDataItem;
        private string Latitude, Longitude;
        private int connectGPSTime = 0;     // 接收GPS資料的總時數
        private int GPSFixTime = 0;         // GPS成功定位的時間
        private int InUSESatellites = 0;    // 定位成功的最大衛星總數
        private Boolean GPSStatus = false;  // 是否已經定位成功
        private int[] SatellitesSNRRank = new int[20];  // 衛星訊號強度排行
        private int SatellitesSNRAvg = 0;   // 定位衛星平均SNR強度
        private bool IsComportOpened = false;
        FileReader KernelModeReader;
        private string TempDataGPS = ""; // ReadGPSData() 暫存變數
        private static Encoding EncodingUTF8 = Encoding.UTF8;
        string TestProduct = string.Empty;
        bool IsTablet = true;
        bool IsDebugMode = true;
        string PortAddressGPS = "COM9";
        int SNRAvgTolerance = 40;     // 定位衛星中訊號強度最高的三顆平均的最低容忍值
        int InUseTolerance = 3;       // 定位衛星數量最低容忍值
        int FixTimeTolerance = 60;    // 成功定位時間最低容忍值
        int WithoutPositioning = 0;   // 是否要進行定位測試? (1 = 不測試, 0 = 測試)
        bool IsSerialPortInKernelMode = true; // u-blox M8 Module 搭配 VCP Driver, Comport會變成Kernel Mode, 預設為 u-blox M8 Module + VCP Driver
        bool PowerOnOffGPS = true; // 開始測試GPS之前先開關一次電源, 嘗試讓device初始化

        private const int Device1AllPowerOnIBWD = 0x11;     // [X]      / [X]       / [X]       / Bluetooth / [X]   / [X]   / [X]   / WIFI
        private const int Device1AllPowerOnFMB8 = 0x13;     // [X]      / [X]       / [X]       / Bluetooth / [X]   / [X]   / WWAN  / WIFI
        // IH83 / M9020B / IB10X 
        private const int Device1AllPowerOnIH83 = 0x37;     // [X]      / [X]       / RearCam   / Bluetooth / [X]   / GPS   / WWAN  / WIFI
        private const int Device1AllPowerOnIBWH = 0x37;     // [X]      / ConfigLED / RearCam   / Bluetooth / [X]   / GPS   / WWAN  / WIFI
        private const int Device1AllPowerOnBTZ1 = 0xF6;     // All LED  / Touch     / RearCam   / Bluetooth / X     / GPS   / WWAN  / X
        private const int Device1AllPowerOnCMC = 0x3F;      // B.LEDCtrl/ A.LEDCtrl / FrontCam  / Bluetooth / FKeyB / GPS   / WWAN  / WIFI
        // M101B / M101H / M101P / M101S / M101BK / M133W      
        // IB80 Bit6 : Charge Control, 設成1時EC會停止充電
        private const int Device1AllPowerOnOthers = 0xB7;   // All LED  / ChargeCtrl/ RearCam   / Bluetooth / [X]   / GPS   / WWAN  / WIFI


        private const int Device2AllPowerOnIH83 = 0x01;
        // IB10X  / M133W
        private const int Device2AllPowerOn101S = 0x0F;
        private const int Device2AllPowerOn9020 = 0x13;     // [X]      / [X]       / [X]       / ExtendPort/ [X]   / [X]  / F Cam / Barcode
        private const int Device2AllPowerOnIBWH = 0x23;     // [X]      / [X]       / RFID      / [X]       / [X]   / [X]  / F Cam / Barcode 
        private const int Device2AllPowerOnFMB8 = 0x4A;     // [X]      / WIFI A    / [X]       / [X]       / KEY BL/ [X]  / Light / STOP
        // M101P
        private const int Device2AllPowerOnBTZ1 = 0x1F;     // [X]      / [X]       / [X]       / ExtendPort/ GPA A / RFID / F Cam / Barcode
        private const int Device2AllPowerOnIH80 = 0x3B;     // [X]      / [X]       / ExtendPort/ Extend USB/ GPS A / [X]  / F Cam / Barcode
        private const int Device2AllPowerOn101P = 0x5F;
        private const int Device2AllPowerOnOthers = 0x3F;   // [X]      / [X]       / ExtendPort/ Extend USB/ GPS A / RFID / F Cam / Barcode

        static SelectQuery PnPEntityQuery = new SelectQuery("SELECT * FROM Win32_PnPEntity");
        ManagementObjectSearcher PnPEntitySearcher = new ManagementObjectSearcher(PnPEntityQuery);
        JObject result = new JObject();
        bool ShowWindow = false;

        #endregion

        // 針對IBWH(Handheld 8"/M800B) & Golden-Age 的u-blox M8N進行亂碼過濾, 其他機種使用u-blox 5/6
        // bool isGPSusingUbloxM8 = false;

        public gps()
        {
            InitializeComponent();
        }

        ~gps()
        {
            closeGPSPort();
        }

        uint GetGPSModuleType()
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

        void EnableDevicePower(string ProductName)
        {
            // 將HotTab功能全開(開啟所有電源), 確保測試時一切功能正常
            if (IsDebugMode)
            {
                Trace.WriteLine("Open all device power.");
            }
            if (IsTablet)
            {
                // SetDevice
                if (ProductName.Equals("IBWD"))
                {
                    HotTabDLL.WinIO_SetDeviceState(Device1AllPowerOnIBWD); // 10110111, WindyBox Bit6 : Charge Control, 設成1時EC會停止充電
                }
                else if (ProductName.Equals("BTZ1"))
                {
                    HotTabDLL.WinIO_SetDeviceState(Device1AllPowerOnBTZ1); // 11110110
                }
                else
                {
                    HotTabDLL.WinIO_SetDeviceState(Device1AllPowerOnOthers);// ID82 Bit6(3G_Antenna) Bit7(GPS_Antenna) 會偵測到有無Docking才會切換到外部天線
                }

                // SetDevice2
                if (ProductName.Equals("FMB8"))
                {
                    HotTabDLL.WinIO_SetDevice2State(Device2AllPowerOnFMB8); // FMB80 : Bit0 = Screen close, Bit1 = Docking Lamp
                }
                else if (ProductName.Equals("BTZ1"))
                {
                    HotTabDLL.WinIO_SetDevice2State(Device2AllPowerOnBTZ1); // 00011111
                }
                else if (ProductName.Equals("101P"))
                {
                    HotTabDLL.WinIO_SetDevice2State(Device2AllPowerOn101P); // 01011111
                }
                else
                {
                    HotTabDLL.WinIO_SetDevice2State(Device2AllPowerOnOthers); // IB80 Bit6(Battery_Max_Charge) 只會讓電池充到50%
                }
                // IBxx & IHxx 系列需開啟Bit5(ExpUSBEn)電源, 才有辦法測試背後接點
            }
        }

        void DevicePowerOffAndOn(string deviceName)
        {
            Trace.WriteLine("Disable Device Power : " + deviceName);
            switch (deviceName)
            {
                case "ExtendPort":
                     //if (TestProduct.Equals("BTZ1")) HotTabDLL.WinIO_SetDevice2State(0x0F); // 00001111
                    //break;
                case "GPS":
                    if (TestProduct.Equals("BTZ1")) HotTabDLL.WinIO_SetDeviceState(0xF2); // LED / Touch / RearCam / BT / X / GPS / WWAN / X
                    else if (TestProduct.Equals("IB80")) HotTabDLL.WinIO_SetDeviceState(0xB3); // AllLED / - / RearCam / BT / - / GPS / 3G / WIFI, Bit6 : Charge Control, 設成1時EC會停止充電
                    break;
            }
            Thread.Sleep(3000);

            Trace.WriteLine("Enable Device Power : " + deviceName);
            EnableDevicePower(TestProduct);
            Thread.Sleep(3000);
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

        string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(exepath, path);
        }

        private void GPS_Load(object sender, EventArgs e)
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
            TestProduct = jobject.TestProduct.ToString();
            IsTablet = (bool)jobject.IsTablet;
            WithoutPositioning = (int)jobject.WithoutPositioning;
            PowerOnOffGPS = (bool)jobject.PowerOnOffGPS;
            SNRAvgTolerance = (int)jobject.SNRAvgTolerance;
            PortAddressGPS = jobject.PortAddressGPS.ToString();

            if (ShowWindow)
            {
                this.Opacity = 100;
                this.ShowInTaskbar = true;
            }

            if (IsDebugMode) Trace.WriteLine("GPS_Load");

            GetGPSModuleType();

            #region Prepare Timer
            mTimerGPS = new System.Windows.Forms.Timer();
            mTimerGPS.Tick += new EventHandler(mTimerGPS_Tick);
            mTimerGPS.Interval = 1000;
            mTimerGPS.Enabled = false;
            #endregion

            if (PowerOnOffGPS)
            {
                DevicePowerOffAndOn("GPS");
            }

            openGPSPort();
        }

        #region Button Event
        private void buttonConnectGPS_Click(object sender, EventArgs e)
        {
            openGPSPort();
        }

        private void buttonDisconnectGPS_Click(object sender, EventArgs e)
        {
            closeGPSPort();
        }
        #endregion

        #region SerialPort Control
        private void openGPSPort()
        {
            try
            {
                txtPortStatus.Text = "Open";
                comboBoxSerialPortGPS.Enabled = false;
                buttonConnectGPS.Enabled = false;
                buttonDisconnectGPS.Enabled = true;

                restoreDefaultValue();

                if (IsSerialPortInKernelMode)
                {
                    KernelModeReader = new FileReader();
                    IsComportOpened = KernelModeReader.Open(PortAddressGPS);
                    if (IsDebugMode) Trace.WriteLine("Is GPS Port Open ? : " + IsComportOpened);
                }
                else
                {
                    mSerialPortGPS = new SerialPort(PortAddressGPS, 9600, Parity.None, 8, StopBits.One);
                    mSerialPortGPS.Open();
                }

                mTimerGPS.Enabled = true;
            }
            catch (Exception ex)
            {
                checkTestStatus(ex.Message);
            }
        }

        private void closeGPSPort()
        {
            try
            {
                checkTestStatus("User canceled the test.");
            }
            catch (Exception ex)
            {
                checkTestStatus(ex.Message);
            }
        }

        private void ReleaseGpsResource()
        {
            if (IsDebugMode) Trace.WriteLine("Release GPS resource.");
            mTimerGPS.Enabled = false;

            if (IsSerialPortInKernelMode)
            {
                if (KernelModeReader != null)
                {
                    if (IsComportOpened) KernelModeReader.Close();
                    KernelModeReader = null;
                }
            }
            else
            {
                if (mSerialPortGPS != null)
                {
                    if (mSerialPortGPS.IsOpen) mSerialPortGPS.Close();
                    mSerialPortGPS = null;
                }
            }

            // Restore Default Setting
            txtPortStatus.Text = "Close";
            comboBoxSerialPortGPS.Enabled = false;
            buttonConnectGPS.Enabled = false;
            buttonDisconnectGPS.Enabled = false;
        }
        #endregion

        // Interprets a "Satellites in View" NMEA sentence
        public void ParseGPGSV(string[] satellitesData)
        {
            string SatellitePRN = "";
            string Azimuth = "";
            string Elevation = "";
            string SignalToNoiseRatio = "";

            int Count = 0;
            for (Count = 1; Count <= 4; Count++)
            {
                // 判斷是否有足夠的衛星資料可以分析
                if ((satellitesData.Length - 1) >= (Count * 4 + 3))
                {
                    // 有資料, 開始處理區塊內的資料, 確認區塊內的各項資訊非null
                    if (satellitesData[Count * 4] != "" &&
                        satellitesData[Count * 4 + 1] != "" &&
                        satellitesData[Count * 4 + 2] != "" &&
                        satellitesData[Count * 4 + 3] != "")
                    {
                        SatellitePRN = satellitesData[Count * 4];           // PseudoRandomCode = System.Convert.ToInt32(satellitesData[Count * 4]);
                        Elevation = satellitesData[Count * 4 + 1];          // Elevation = Convert.ToInt32(satellitesData[Count * 4 + 1]);
                        Azimuth = satellitesData[Count * 4 + 2];            // Azimuth = Convert.ToInt32(satellitesData[Count * 4 + 2]);
                        SignalToNoiseRatio = satellitesData[Count * 4 + 3]; // SignalToNoiseRatio = Convert.ToInt32(satellitesData[Count * 4 + 3]);

                        if (IsDebugMode) Trace.Write("GPGSV - PRN : " + SatellitePRN + " , Elevation : " + Elevation + " , Azimuth : " + Azimuth + " , SNR : " + SignalToNoiseRatio); // Winmate kenkun add on 2016/08/29

                        // 如果是最後一個含有Checksum的資料要過濾掉*後的所有資料
                        if (SignalToNoiseRatio.IndexOf("*") >= 0)
                        {
                            string[] SNR = SignalToNoiseRatio.Split('*');
                            SignalToNoiseRatio = SNR[0];
                        }

                        // 將每個衛星的資訊顯示在Listview
                        satellitesDataItem = new ListViewItem(new string[] { SatellitePRN, Elevation, Azimuth, SignalToNoiseRatio });
                        bool CheckSatellites = false;
                        for (int i = 0; i < listSatellites.Items.Count; i++)
                        {
                            if (listSatellites.Items[i].Text == satellitesDataItem.Text)
                            {
                                CheckSatellites = true;
                                listSatellites.Items[i].Text = SatellitePRN;
                                listSatellites.Items[i].SubItems[1].Text = Elevation;
                                listSatellites.Items[i].SubItems[2].Text = Azimuth;
                                if (SignalToNoiseRatio.Equals("") || SignalToNoiseRatio.Equals(String.Empty) || SignalToNoiseRatio == null)
                                {
                                    listSatellites.Items[i].SubItems[3].Text = "01";
                                }
                                else listSatellites.Items[i].SubItems[3].Text = SignalToNoiseRatio;
                            }
                        }
                        if (!CheckSatellites) listSatellites.Items.Add(satellitesDataItem);

                        // 單獨抓取各個衛星的訊號強度
                        for (int i = 0; i < listSatellites.Items.Count; i++)
                        {
                            if (listSatellites.Items[i].SubItems[3].Text != "") SatellitesSNRRank[i] = Convert.ToInt32(listSatellites.Items[i].SubItems[3].Text);
                            else SatellitesSNRRank[i] = 1;
                        }

                        // 當定位成功(3D Mode) && 衛星數量 >= 3, 就計算目前的衛星平均訊號強度
                        if (GPSStatus && InUSESatellites >= 3)
                        {
                            SatellitesSNRAvg = 0;
                            Array.Sort(SatellitesSNRRank);      // 由小到大排序SNR值
                            Array.Reverse(SatellitesSNRRank);   // 反轉SNR值排序
                                                                // 只利用目前用來定位的衛星, 不去利用目前看得到的衛星數量去算訊號強度
                            for (int i = 0; i < 3; i++)
                            {
                                SatellitesSNRAvg = SatellitesSNRAvg + SatellitesSNRRank[i]; // 只計算訊號最強三顆的衛星平均強度
                            }

                            SatellitesSNRAvg = SatellitesSNRAvg / 3;
                            txtSatellitesSNRAverage.Text = SatellitesSNRAvg.ToString();
                        }

                        textBoxSatellitesInView.Text = listSatellites.Items.Count.ToString();
                        listSatellites.Items[listSatellites.Items.Count - 1].EnsureVisible();
                    }
                }
            }
        }

        private void DisplayNMEARawData(string bData)
        {
            Trace.WriteLine("NMEA Raw Data : \r\n" + bData + "\r\n");
            if (dumpRawDataCheck.Checked && IsGPSDataCorrected(bData))
            {
                txtNMEARawData.Text = bData;
            }
            else
            {
                txtNMEARawData.Text = "";
            }
        }

        private bool IsGPSDataCorrected(string receiveData)
        {
            if ((receiveData.Length > 0) &&
                (receiveData.Contains("GPRMC") || receiveData.Contains("GNRMC") ||
                 receiveData.Contains("GPVTG") || receiveData.Contains("GNVTG") ||
                 receiveData.Contains("GPGGA") || receiveData.Contains("GNGGA") ||
                 receiveData.Contains("GPGSA") || receiveData.Contains("GNGSA") ||
                 receiveData.Contains("GPGSV") || receiveData.Contains("GNGSV") ||
                 receiveData.Contains("GPGLL") || receiveData.Contains("GNGLL") ||
                 receiveData.Contains("GPGST") ||
                 receiveData.Contains("GPZDA")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void restoreDefaultValue()
        {
            connectGPSTime = 0;
            txtQuality.Text = "None";
            txtSTQty.Text = "0";
            txtSeaLevel.Text = "0";
            txtGPSMode.Text = "None";
            txtGPSStatus.Text = "None";
            txtGPSFixTime.Text = "";
            txtGPSConnectTime.Text = "";
            txtNMEARawData.Text = "";
            textBoxSatellitesInView.Text = "0";
            txtSatellitesSNRAverage.Text = "0";
            listSatellites.Items.Clear();
            labelResult.ForeColor = Color.Blue;
            labelResult.Text = "Not Result";

            GPSStatus = false;
        }

        private void checkTestStatus(String testResult)
        {
            ReleaseGpsResource();

            if (testResult.Equals("PASS"))
            {
                labelResult.ForeColor = Color.Green;
                labelResult.Text = "Pass";
                result["result"] = "PASS";
            }
            else
            {
                txtNMEARawData.Text = testResult;
                labelResult.ForeColor = Color.Red;
                labelResult.Text = "Fail";
                result["result"] = "FAIL";
            }

            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            if (!ShowWindow)
                Exit();
        }

        private void mTimerGPS_Tick(object sender, EventArgs e)
        {
            if (connectGPSTime >= 0)
            {
                // Display the new time left by updating the txtConncentTime label.
                txtGPSConnectTime.Text = (++connectGPSTime).ToString();

                try // FM10/FM08的GPS在Docking上, 有可能中途會被斷電
                {
                    ReadGPSData();
                }
                catch (Exception ex)
                {
                    mTimerGPS.Stop();
                    if (IsDebugMode) Trace.WriteLine("Unable to get GPS data. Please confirm the GPS status. \r\nReason : " + ex.Message);
                    checkTestStatus("FAIL");
                }

                if (GPSStatus) txtGPSStatus.Text = "Fix";
                else txtGPSStatus.Text = "No fix";
            }

            // 將連線時間限定在3秒以上才可以送出PASS EVENT是因為如果GPS瞬間定位成功, 表單可能不會更新
            if (InUSESatellites >= InUseTolerance && SatellitesSNRAvg >= SNRAvgTolerance && GPSFixTime <= FixTimeTolerance && connectGPSTime >= DataDetectTimeMinThreshold) checkTestStatus("PASS");
            else if (WithoutPositioning.Equals(1) && connectGPSTime >= DataDetectTimeWithoutPositioning)
            {
                if (txtNMEARawData.Text.Length > 0 && IsGPSDataCorrected(txtNMEARawData.Text))
                {
                    checkTestStatus("PASS");
                }
                else
                {
                    checkTestStatus("FAIL");
                }
            }
            else if (connectGPSTime > FixTimeTolerance) checkTestStatus("Satellite positioning failure");
        }

        int GarbledTextIndex = -1;
        private void ReadGPSData()
        {
            if ((IsSerialPortInKernelMode && KernelModeReader != null && IsComportOpened) ||
                (IsSerialPortInKernelMode && mSerialPortGPS != null && mSerialPortGPS.IsOpen))
            {
                if (IsSerialPortInKernelMode)
                {
                    byte[] buffer = new byte[256];
                    int bytesRead;
                    TempDataGPS = "";
                    do
                    {
                        bytesRead = KernelModeReader.Read(buffer, 0, buffer.Length);
                        string content = EncodingUTF8.GetString(buffer, 0, bytesRead);
                        TempDataGPS += content;
                    }
                    while (bytesRead > 0);
                    Trace.WriteLine(Environment.NewLine + TempDataGPS + Environment.NewLine);

                }
                else
                {
                    TempDataGPS = mSerialPortGPS.ReadExisting();
                }

                if (TempDataGPS.Length < 1)
                {
                    return; // 當沒有接收到資料時直接return
                }

                // 看是否有抓到亂碼, 如果 index >= 0 就是有抓到亂碼
                if (TempDataGPS.Contains("$GNRMC")) // if (IsGPSusingUbloxM8)
                {
                    GarbledTextIndex = TempDataGPS.IndexOf("$GNRMC");
                    if (GarbledTextIndex >= 0 && TempDataGPS.Length > GarbledTextIndex) TempDataGPS = TempDataGPS.Substring(TempDataGPS.IndexOf("$GNRMC"));
                }
                else if (TempDataGPS.Contains("$GPRMC"))
                {
                    GarbledTextIndex = TempDataGPS.IndexOf("$GPRMC");
                    if (GarbledTextIndex >= 0 && TempDataGPS.Length > GarbledTextIndex) TempDataGPS = TempDataGPS.Substring(TempDataGPS.IndexOf("$GPRMC"));
                }
                if (IsDebugMode) Trace.WriteLine("RAW Length : " + TempDataGPS.Length + " , Garbled Text Index : " + GarbledTextIndex);

                DisplayNMEARawData(TempDataGPS);

                string[] gpsArr = TempDataGPS.Split('$');

                for (int i = 0; i < gpsArr.Length; i++)
                {
                    string strTemp = gpsArr[i];
                    string[] satellitesDataArray = strTemp.Split(',');

                    #region GPGGA
                    if (satellitesDataArray[0] == "GPGGA" || satellitesDataArray[0] == "GNGGA") // ublox 6 跟 ublox 8 送出的關鍵字不一樣
                    {
                        try
                        {
                            // Latitude
                            Double dLat = Convert.ToDouble(satellitesDataArray[2]);
                            dLat = dLat / 100;
                            string[] lat = dLat.ToString().Split('.');
                            string la = (((Convert.ToDouble(lat[1]) / 60) * 10000)).ToString("#");
                            for (int a = 0; a < lat[1].Length; a++)
                            {
                                if (lat[1].Substring(a, 1) == "0") la = "0" + la;
                                else break;
                            }
                            Latitude = lat[0].ToString() + "." + la.Substring(0, 6);

                            // Longitude
                            Double dLon = Convert.ToDouble(satellitesDataArray[4]);
                            dLon = dLon / 100;
                            string[] lon = dLon.ToString().Split('.');
                            string lo = (((Convert.ToDouble(lon[1]) / 60) * 10000)).ToString("#");
                            for (int b = 0; b < lon[1].Length; b++)
                            {
                                if (lon[1].Substring(b, 1) == "0") lo = "0" + lo;
                                else break;
                            }
                            Longitude = lon[0].ToString() + "." + lo.Substring(0, 6);

                            // Display
                            txtLatitude.Text = Latitude;
                            txtLongitude.Text = Longitude;
                            txtSTQty.Text = satellitesDataArray[7];
                            // if (GPSStatus && System.Convert.ToInt32(satellitesDataArray[7]) >= InUSESatellites) InUSESatellites = System.Convert.ToInt32(satellitesDataArray[7]);
                            InUSESatellites = System.Convert.ToInt32(satellitesDataArray[7]);
                            if (InUSESatellites > 3 & GPSStatus == false)
                            {
                                GPSStatus = true;
                                GPSFixTime = connectGPSTime;
                                txtGPSFixTime.Text = GPSFixTime + "";
                            }
                            txtSeaLevel.Text = satellitesDataArray[9];

                            if (IsDebugMode) Trace.WriteLine("GPGGA - Latitude : " + Latitude + " , Longitude : " + Longitude + " , STQty : " + satellitesDataArray[7] + " , Satellites : " + InUSESatellites + " , SeaLevel : " + satellitesDataArray[9]); // Winmate kenkun add on 2016/08/29

                            switch (satellitesDataArray[6])
                            {
                                #region GPS Status
                                case "0":
                                    txtQuality.Text = "No Quality";
                                    break;
                                case "1":
                                    txtQuality.Text = "GPS fix(SPS)";
                                    break;
                                case "2":
                                    txtQuality.Text = "DGPS fix";
                                    break;
                                case "3":
                                    txtQuality.Text = "PPS fix";
                                    break;
                                case "4":
                                    txtQuality.Text = "Real Time Kinematic";
                                    break;
                                case "5":
                                    txtQuality.Text = "Float RTK";
                                    break;
                                case "6":
                                    txtQuality.Text = "Estimated";
                                    break;
                                case "7":
                                    txtQuality.Text = "Manual input mode";
                                    break;
                                case "8":
                                    txtQuality.Text = "Simulation mode";
                                    break;
                                default:
                                    txtQuality.Text = "None";
                                    break;
                                    #endregion
                            }
                        }
                        catch
                        {
                            #region Reset UI
                            txtLatitude.Text = "None";
                            txtLongitude.Text = "None";
                            txtQuality.Text = "None";
                            txtSTQty.Text = "0";
                            txtSeaLevel.Text = "0";
                            #endregion
                        }
                    }
                    #endregion

                    #region GPGSV
                    if (satellitesDataArray[0] == "GPGSV")
                    {
                        ParseGPGSV(satellitesDataArray);
                    }
                    #endregion

                    #region GPGSA
                    if (satellitesDataArray[0] == "GPGSA" || satellitesDataArray[0] == "GNGSA")  // ublox 6 跟 ublox 8 送出的關鍵字不一樣
                    {
                        if (IsDebugMode) Trace.WriteLine("GPGSA - GPSMode : " + satellitesDataArray[2]);
                        switch (satellitesDataArray[2])
                        {
                            case "1":
                                txtGPSMode.ForeColor = Color.Black;
                                txtGPSMode.Text = "Fix not available";
                                GPSStatus = false;
                                break;
                            case "2":
                                txtGPSMode.ForeColor = Color.Red;
                                txtGPSMode.Text = "2D";
                                GPSStatus = false;
                                break;
                            case "3":
                                txtGPSMode.ForeColor = Color.Green;
                                txtGPSMode.Text = "3D";
                                if (!GPSStatus)
                                {
                                    GPSFixTime = connectGPSTime;
                                    txtGPSFixTime.Text = GPSFixTime + "";
                                }
                                GPSStatus = true;
                                break;
                            default:
                                txtGPSMode.ForeColor = Color.Black;
                                txtGPSMode.Text = "None";
                                break;
                        }
                    }
                    #endregion
                }
            }
            else
            {
                #region Port Close
                txtLatitude.Text = "Port Close";
                txtLongitude.Text = "Port Close";
                txtQuality.Text = "Port Close";
                txtSTQty.Text = "Port Close";
                txtSeaLevel.Text = "Port Close";
                txtGPSMode.Text = "Port Close";
                #endregion
            }
        }
    }
}
