using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace wifi
{
    public partial class wifi : Form
    {
        #region Definition

        public enum Operation
        {
            Add,
            Delete,
            Change
        }

        System.Windows.Forms.Timer TimerWifiTest = new System.Windows.Forms.Timer();
        bool IsDebugMode = true;
        private bool IsWiFiScanned = false;  // 判斷是否有在範圍內找到欲連線的SSID
        private bool IsWiFi24gPassed = false;// 2.4G頻段是否已經測試完畢
        private int WiFiScanCount = 1;          // 進行WIFI連線前必須先掃描5次, 計算SSID的平均SNR強度
        private int WiFiSignal = 0;             // 已連線的WiFi訊號強度
        private int WiFiSignalAvg = 0;          // 用來計算WiFi訊號平均強度
        // 測試WIFI重複連線測試穩定度用
        private int LoopCount = 0; // 預設跑100次迴圈測試
        private int PassCount = 0;
        private int FailCount = 0;
        int TotalPingCountWiFi = 50;                    // 嘗試PING到遠端主機的次數
        double ConnectNetToleranceWiFi = 0.9;           // Ping連線時成功機率的最低容忍值
        int PingIntervalWiFi = 1000;                    // 每次Ping所等待回應的最大時間
        int SNRToleranceWiFi = 65;                      // 連線WIFI訊號強度的最低容忍值
        //bool EnableAuthentication = false;              // 用來判斷要用哪一種Wireless Profile(有無密碼)
        //string connectWiFiSSID = "WinmateTest";         // SSID
        //string connectWiFiSSID24G = "WinmateTest";
        //string connectWiFiSSID5G = "WinmateTest-5GHz";
        //string connectWiFiPassword = "";                // SSID密碼  // "5296930209";// "0933214371";
        //string connectWiFiAuthentication = "open";      // SSID使用的驗證類型, open/WPA/WPA2
        //string connectWiFiEncryption = "none";          // SSID使用的加密類型, WEP/TKIP/AES
        //string connectWiFiKeyType = "";                 // SSID的KeyType, networkKey/passPhrase
        string WifiProfile = "";                        // 儲存WIFI用以連線的SSID XML設定檔格式
        int DelayWiFiTestTime = 1000;                     // 當送出WIFI連線指令後, 要延遲多少秒後才開始進行WIFI連線測試?
        int isLoopTestWiFi = 0;                         // 是否要開啟迴圈測試? 開啟之後會連續測試100次網路
        int isConnectTestWiFi = 1;
        int WiFiScanTime = 3;                           // 如果找不到既定的SSID要重新掃描幾次?
        bool IsTestWiFiTwoBands = false;                //是否要同時測試WIFI 2.4G跟5G的兩個頻段
        Ping mPingTester;                         // 可以讓應用程式判斷是否能透過網路存取遠端電腦. 
        PingReply mPingReply;                     // 儲存由 Ping.Send 或 Ping.SendAsync而產生的狀態和資料等相關資訊
                                                  // public string who = "192.168.100.46";
                                                  //string PingAddressWiFi = "192.168.100.46";    // Ping的連線位置
        static string pingData = "TestWinmatePingTestWinmatePingTestWinmatePingTestWinmatePingTest";
        byte[] pingBuffer = Encoding.ASCII.GetBytes(pingData);
        PingOptions pingOptions = new PingOptions(64, true);
        AutoResetEvent pingWaiter = new AutoResetEvent(false); // 向等候的執行緒通知發生事件
        int tryConnectNetCount = 0;   // 目前嘗試ping到遠端主機的次數
        int PingStatusSuccess = 0;    // Ping成功的次數
        bool ShowWindow = false;
        JObject result = new JObject();
        List<string> EthernetGateway = new List<string>();
        List<string> WiFiGateway = new List<string>();
        List<int> EthernetInterface = new List<int>();
        List<int> WiFiInterface = new List<int>();
        List<string> WiFiSSIDList;
        List<string> PingAddressList;
        List<string> WiFiPasswordList;
        List<string> WiFiAuthenticationList;
        List<string> WiFiEncryptionList;
        List<string> WiFiKeyTypeList;
        List<bool> EnableAuthenticationList;
        List<bool> IsWiFiPassedList;
        int WiFiTestCount = 0;
        #endregion



        public wifi()
        {
            InitializeComponent();
        }

        string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(exepath, path);
        }

        private void wifi_Load(object sender, EventArgs e)
        {
            result["result"] = false;
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Environment.Exit(0);
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            ShowWindow = (bool)jobject.ShowWindow;
            SNRToleranceWiFi = (int)jobject.SNRTolerance;
            ConnectNetToleranceWiFi = (double)jobject.PingSuccessRate;
            TotalPingCountWiFi = (int)jobject.TotalPingCount;
            WiFiScanTime = (int)jobject.WiFiScanTime;
            DelayWiFiTestTime = (int)jobject.DelayWiFiTestTime;
            PingIntervalWiFi = (int)jobject.PingIntervalWiFi;
            PingAddressList = jobject.PingAddressWiFi.ToObject<List<string>>();
            EnableAuthenticationList = jobject.EnableAuthentication.ToObject<List<bool>>();
            WiFiSSIDList = jobject.SSID.ToObject<List<string>>();
            WiFiPasswordList = jobject.Password.ToObject<List<string>>();
            WiFiAuthenticationList = jobject.Authentication.ToObject<List<string>>();
            WiFiEncryptionList = jobject.Encryption.ToObject<List<string>>();
            WiFiKeyTypeList = jobject.KeyType.ToObject<List<string>>();
            IsWiFiPassedList = new List<bool>();
            IsWiFiPassedList.AddRange(Enumerable.Repeat(false, WiFiSSIDList.Count()));

            if (ShowWindow)
                this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            if (IsDebugMode) Trace.WriteLine("WiFi_Load");

            DisconnectNetwork();
            #region Prepare Timer
            TimerWifiTest.Tick += new EventHandler(TimerWifiTest_Tick);
            TimerWifiTest.Interval = 2000;
            #endregion

            // 判斷OS版本, 作業系統Major Version小於6的沒有netsh wlan的指令
            if (Environment.OSVersion.Version.Major >= 6 && TimerWifiTest.Enabled.Equals(false))
            {
                TimerWifiTest.Start(); // WIFI要使用Timer是因為連線前要先抓WIFI訊號強度
            }
            else // WIFI測試模組必須在作業系統版本5.1以上(Windows XP以上)才可運行
            {
                UpdateUI("The system version is lower than the minimum OS version");
                checkTestStatus("OS Version mismatch");
            }
        }

        private void TimerWifiTest_Tick(object sender, EventArgs e)
        {
            try
            {
                TestNetworkStability();
                WiFiScanCount++;
            }
            catch (Exception ex)
            {
                checkTestStatus("TimerWifi : " + ex.Message);
            }
        }

        public void DisconnectNetwork()
        {
            DisconnectWiFi();
        }

        public void DisconnectWiFi()
        {
            DisconnectWireless("netsh", "wlan disconnect interface = *");
            DisconnectWireless("netsh", "wlan delete name=*");
        }

        public void DisconnectWireless(string FileName, string Arguments)
        {
            using (Process mProcess = new Process()) // Process用來呼叫外部程式
            {
                if (IsDebugMode)
                {
                    Trace.WriteLine("Disconnect Wireless Network. cmd : " + Arguments + " " + DateTime.Now);
                }
                mProcess.StartInfo.CreateNoWindow = true; // 不顯示CMD的執行視窗
                mProcess.StartInfo.FileName = FileName; // 呼叫netsh
                mProcess.StartInfo.Arguments = Arguments;
                mProcess.StartInfo.RedirectStandardOutput = true;// 取得或設定值, 指出應用程式的輸出是否寫入至 Process.StandardOutput 資料流. 
                mProcess.StartInfo.UseShellExecute = false; // 取得或設定值, 指出是否要使用作業系統 Shell 來啟動處理序. 
                mProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                mProcess.Start();
                if (IsDebugMode)
                {
                    Trace.WriteLine("Disconnect Wireless Network. Result = " + mProcess.StandardOutput.ReadToEnd() + DateTime.Now);
                }
                mProcess.WaitForExit();
            }
        }

        public void ConnectWiFiSSID(string SSID, string Password, string Authentication, string Encryption, string KeyType)
        {
            string command = string.Empty;

            if (EnableAuthenticationList[WiFiTestCount])
            {
                WifiProfile = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>" + SSID + "</name><SSIDConfig><SSID><name>" + SSID + "</name></SSID></SSIDConfig><connectionType>ESS</connectionType><MSM><security><authEncryption><authentication>" + Authentication + "</authentication><encryption>" + Encryption + "</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>" + KeyType + "</keyType><protected>false</protected><keyMaterial>" + Password + "</keyMaterial></sharedKey><keyIndex>0</keyIndex></security></MSM></WLANProfile>");
            }
            else
            {
                WifiProfile = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>" + SSID + "</name><SSIDConfig><SSID><name>" + SSID + "</name></SSID></SSIDConfig><connectionType>ESS</connectionType><MSM><security><authEncryption><authentication>" + Authentication + "</authentication><encryption>" + Encryption + "</encryption><useOneX>false</useOneX></authEncryption></security></MSM></WLANProfile>");
            }

            Trace.WriteLine("Wi-Fi Profile : " + WifiProfile);

            StreamWriter wifiProfileWriter = new System.IO.StreamWriter(GetFullPath(WiFiSSIDList[WiFiTestCount] + ".xml"));
            wifiProfileWriter.WriteLine(WifiProfile);
            wifiProfileWriter.Close();

            Process WiFiConnect = new Process();
            WiFiConnect.StartInfo.CreateNoWindow = true;
            WiFiConnect.StartInfo.RedirectStandardInput = true;
            WiFiConnect.StartInfo.FileName = "cmd.exe";
            if (SSID.IndexOf(' ') != -1)
            {
                command = string.Format("netsh wlan add profile filename ={0}", Path.Combine(Application.StartupPath, "TestWifi.xml"));
            }
            else
            {
                command = string.Format("netsh wlan add profile filename ={0}", Path.Combine(Application.StartupPath, WiFiSSIDList[WiFiTestCount] + ".xml"));
            }

            Trace.WriteLine("Wi-Fi Connect Info : " + WiFiConnect.StartInfo.Arguments);
            WiFiConnect.StartInfo.RedirectStandardOutput = true;// 取得或設定值，指出應用程式的輸出是否寫入至 Process.StandardOutput 資料流。
            WiFiConnect.StartInfo.UseShellExecute = false; // 取得或設定值，指出是否要使用作業系統 Shell 來啟動處理序。
            WiFiConnect.Start();
            using (var sw = new StreamWriter(WiFiConnect.StandardInput.BaseStream))
            {
                Trace.WriteLine(command);
                sw.WriteLine(command);
                command = string.Format("netsh wlan connect name={0}", WiFiSSIDList[WiFiTestCount]);
                Trace.WriteLine(command);
                sw.WriteLine(command);
            }
            WiFiConnect.WaitForExit();

            if (IsDebugMode)
            {
                Trace.WriteLine("Connect to Wi-Fi. " + DateTime.Now);
            }
        }

        string DoGetHostEntry(string hostname)
        {
            IPHostEntry host;
            IPAddress ip = null;
            host = Dns.GetHostEntry(hostname);
            foreach (var v in host.AddressList)
            {
                ip = v;
            }
            return ip.ToString();
        }

        bool SetRouteTable(string gateway, int interfaceindex, int metric, Operation operation)
        {
            try
            {
                using (var process = new Process())
                {
                    var psi = new ProcessStartInfo();
                    psi.FileName = "cmd.exe";
                    psi.RedirectStandardInput = true;
                    psi.RedirectStandardOutput = false;
                    psi.RedirectStandardError = false;
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    process.StartInfo = psi;
                    process.Start();

                    using (var sw = new StreamWriter(process.StandardInput.BaseStream))
                    {
                        string command = string.Empty;
                        if (operation == Operation.Add)
                        {
                            command = string.Format("route add 0.0.0.0 mask 0.0.0.0 {0} metric {1} if {2}", gateway, metric, interfaceindex);
                        }
                        else if (operation == Operation.Delete)
                        {
                            command = string.Format("route delete 0.0.0.0 mask 0.0.0.0 {0} metric {1} if {2}", gateway, metric, interfaceindex);
                        }
                        else if (operation == Operation.Change)
                        {
                            command = string.Format("route change 0.0.0.0 mask 0.0.0.0 {0} metric {1} if {2}", gateway, metric, interfaceindex);
                        }
                        Console.WriteLine(command);
                        sw.WriteLine(command);
                        command = string.Format("route delete 192.168.0.0");
                        Console.WriteLine(command);
                        sw.WriteLine(command);
                        command = string.Format("route delete 192.168.120.0");
                        sw.WriteLine(command);
                    }

                    process.WaitForExit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        bool GetNetworkInformation(NetworkInterfaceType NetworkInterfaceType)
        {
            var network = NetworkInterface.GetAllNetworkInterfaces()
                .Where(e => e.NetworkInterfaceType == NetworkInterfaceType
                && e.OperationalStatus == OperationalStatus.Up).ToList();

            foreach (var v in network)
            {
                var ip = v.GetIPProperties().UnicastAddresses.Where(d => d.Address.AddressFamily == AddressFamily.InterNetwork).Select(e => e.Address);
                var gateway = v.GetIPProperties().GatewayAddresses.Where(d => d.Address.AddressFamily == AddressFamily.InterNetwork).Select(e => e.Address);
                var interfaceindex = v.GetIPProperties().GetIPv4Properties().Index;
                Trace.WriteLine(v.Description);
                Trace.WriteLine(string.Format("interface: ", interfaceindex));
                Trace.WriteLine(string.Format("ip count: {0}", ip.Count()));
                Trace.WriteLine(string.Format("gateway count: {0}", gateway.Count()));
                foreach (var k in ip)
                {
                    var s = k.ToString();
                    Trace.WriteLine("ip: {0}", s);
                }
                foreach (var k in gateway)
                {
                    var s = k.ToString();
                    Trace.WriteLine("gateway: {0}", s);
                    if (NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        EthernetGateway.Add(s);
                        EthernetInterface.Add(interfaceindex);
                    }
                    else if (NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    {
                        WiFiGateway.Add(s);
                        WiFiInterface.Add(interfaceindex);
                    }
                }
            }

            return true;
        }

        public void TestNetworkStability()
        {
            if (IsWiFiScanned)
            {
                TimerWifiTest.Stop();

                #region 判斷訊號強度和是否要進行連線測試
                if (WiFiSignalAvg < -SNRToleranceWiFi && isConnectTestWiFi.Equals(1))
                {
                    UpdateUI("SNR(Avg) : " + WiFiSignalAvg + " < " + -SNRToleranceWiFi + " dbm");
                    checkTestStatus("SNR(Avg) : " + WiFiSignalAvg + " < " + -SNRToleranceWiFi + " dbm");
                }
                else if (WiFiSignalAvg > -SNRToleranceWiFi && isConnectTestWiFi.Equals(0))
                {

                    UpdateUI("SNR(Avg) : " + WiFiSignalAvg + " > " + -SNRToleranceWiFi + " dbm");
                    checkTestStatus("PASS");
                }
                else
                {
                    UpdateUI("WIFI is trying to connect... " + DateTime.Now);

                    ConnectWiFiSSID(WiFiSSIDList[WiFiTestCount], WiFiPasswordList[WiFiTestCount], WiFiAuthenticationList[WiFiTestCount], WiFiEncryptionList[WiFiTestCount], WiFiKeyTypeList[WiFiTestCount]);

                    Thread.Sleep(DelayWiFiTestTime);
                    UpdateUI("WIFI connectivity testing... " + DateTime.Now);

                    tryConnectNetCount = 0;
                    PingStatusSuccess = 0;

                    if (mPingTester == null)
                    {
                        UpdateUI("Initial Ping Tester");

                        mPingTester = new Ping();
                        // When the PingCompleted event is raised, the PingCompletedCallback method is called.
                        mPingTester.PingCompleted += new PingCompletedEventHandler(this.PingCompletedCallBack);
                        GetNetworkInformation(NetworkInterfaceType.Wireless80211);
                        GetNetworkInformation(NetworkInterfaceType.Ethernet);
                        for (int i = 0; i < EthernetGateway.Count(); i++)
                            SetRouteTable(EthernetGateway[i], EthernetInterface[i], 1, Operation.Delete);
                        Thread.Sleep(DelayWiFiTestTime);
                        mPingTester.SendAsync(PingAddressList[WiFiTestCount], PingIntervalWiFi, pingBuffer, null); // 開啟一個Thread來Ping遠程主機
                    }
                }
                #endregion
            }
            else if (!IsWiFiScanned && WiFiScanCount > WiFiScanTime)
            {
                TimerWifiTest.Stop();
                UpdateUI("Unable to Detect Wireless Network Name (SSID). " + DateTime.Now);
                checkTestStatus("Can't find SSID : " + WiFiSSIDList[WiFiTestCount]);
            }
            else
            {
                ScanWiFiSSID();
            }
        }

        void SetHosts(string ip, string hostname)
        {
            using (StreamWriter w = File.AppendText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts")))
            {
                w.WriteLine("{0} {1}", ip, hostname);
            }
        }

        void RemoveHosts(string hostname)
        {
            string hostFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts");
            IEnumerable<string> contents = File.ReadAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts"));

            File.WriteAllLines(hostFile, File.ReadAllLines(hostFile).Where(e => !e.Contains(hostname)));
        }

        private void ScanWiFiSSID()
        {
            #region Variable
            string NetshOutput, WlanData;
            string[,] Networks = new string[100, 9];
            int BSSIDNumber = 0;
            int NetworkIndex = -1;
            int Signaldbm = -100;
            #endregion

            using (Process proc = new Process()) //Process用來呼叫外部程式
            {
                proc.StartInfo.CreateNoWindow = true; //不顯示CMD的執行視窗
                proc.StartInfo.FileName = "netsh"; //呼叫netsh
                proc.StartInfo.Arguments = "wlan show networks mode=bssid"; //需要netsh執行的指令參數
                proc.StartInfo.RedirectStandardOutput = true;//取得或設定值，指出應用程式的輸出是否寫入至 Process.StandardOutput 資料流。
                proc.StartInfo.UseShellExecute = false; //取得或設定值，指出是否要使用作業系統 Shell 來啟動處理序。
                proc.Start();
                NetshOutput = proc.StandardOutput.ReadToEnd(); //讀取從目前位置到資料流末端的所有字元。
                proc.WaitForExit(); //設定要等待相關的處理序結束的時間，並且阻止目前的執行緒執行，直到等候時間耗盡或者處理序已經結束為止。
            }

            //if (IsDebugMode) Trace.WriteLine(NetshOutput); //一次將所有資料倒出來量會太大

            if (NetshOutput.IndexOf("no wireless interface") >= 0)
            {
                TimerWifiTest.Stop();
                checkTestStatus("No wireless interface");  // There is no wireless interface on the system.
            }

            #region Parse WIFI SSID data
            StringReader netshOutputReader = new StringReader(NetshOutput.ToString());
            while ((WlanData = netshOutputReader.ReadLine()) != null)
            {
                //if (IsDebugMode) Trace.WriteLine(WlanData);
                if (WlanData.StartsWith("General Failure"))
                {
                    TimerWifiTest.Stop();
                    checkTestStatus("Wifi not installed."); // Wifi disconnected or not installed
                    break;
                }
                if (WlanData.StartsWith("SSID"))
                {
                    NetworkIndex++;
                    for (int i = 0; i < 9; i++) Networks[NetworkIndex, i] = " "; // prevent exception finding null on search 
                    Networks[NetworkIndex, 3] = "0%"; // prevent exception for trim
                    BSSIDNumber = 0;// reset the BSSID number
                    Networks[NetworkIndex, 1] = WlanData.Substring(WlanData.IndexOf(":") + 1).TrimEnd(' ').TrimStart(' ');
                    continue;
                }
                if (WlanData.IndexOf("Network type") > 0 || WlanData.IndexOf("網路類型") > 0)
                {
                    if (WlanData.EndsWith("Infrastructure") || WlanData.IndexOf("基礎結構") > 0)
                    {
                        Networks[NetworkIndex, 7] = "AP";
                        continue;
                    }
                    else Networks[NetworkIndex, 7] = WlanData.Substring(WlanData.IndexOf(":") + 1); //"Ad-hoc";
                }
                if (WlanData.IndexOf("Authentication") > 0 || WlanData.IndexOf("驗證") > 0)
                {
                    Networks[NetworkIndex, 4] = WlanData.Substring(WlanData.IndexOf(":") + 1).TrimStart(' ').TrimEnd(' ');
                    continue;
                }
                if (WlanData.IndexOf("Encryption") > 0 || WlanData.IndexOf("加密") > 0)
                {
                    Networks[NetworkIndex, 5] = WlanData.Substring(WlanData.IndexOf(":") + 1).TrimStart(' ').TrimEnd(' ');
                    continue;
                }
                if (WlanData.IndexOf("BSSID") > 0)
                {
                    if ((Convert.ToInt32(WlanData.IndexOf("BSSID" + 6)) > BSSIDNumber))
                    {
                        BSSIDNumber = Convert.ToInt32(WlanData.IndexOf("BSSID" + 6));
                        NetworkIndex++;
                        Networks[NetworkIndex, 1] = Networks[NetworkIndex - 1, 1]; // same SSID 
                        Networks[NetworkIndex, 7] = Networks[NetworkIndex - 1, 7]; // same Network Type
                        Networks[NetworkIndex, 4] = Networks[NetworkIndex - 1, 4]; // Same authorization
                        Networks[NetworkIndex, 5] = Networks[NetworkIndex - 1, 5]; // same encryption
                    }
                    Networks[NetworkIndex, 0] = WlanData.Substring(WlanData.IndexOf(":") + 1);
                    continue;
                }
                if (WlanData.IndexOf("Signal") > 0 || WlanData.IndexOf("訊號") > 0)
                {
                    Networks[NetworkIndex, 3] = WlanData.Substring(WlanData.IndexOf(":") + 1);
                    continue;
                }
                if (WlanData.IndexOf("Radio Type") > 0 || WlanData.IndexOf("無線電波類型") > 0)
                {
                    Networks[NetworkIndex, 6] = WlanData.Substring(WlanData.IndexOf(":") + 1);
                    continue;
                }
                if (WlanData.IndexOf("Channel") > 0 || WlanData.IndexOf("通道") > 0)
                {
                    Networks[NetworkIndex, 2] = WlanData.Substring(WlanData.IndexOf(":") + 1);
                    continue;
                }
                if (WlanData.IndexOf("Basic Rates") > 0 || WlanData.IndexOf("基本速率") > 0)
                {
                    //Networks[NetworkIndex, 8] = line.Substring(line.Length - 2, 2);
                    Networks[NetworkIndex, 8] = WlanData.Substring(WlanData.IndexOf(":"));
                    if (Networks[NetworkIndex, 8] == ":") { Networks[NetworkIndex, 8] = "not shown"; continue; }
                    Networks[NetworkIndex, 8] = Networks[NetworkIndex, 8].TrimStart(':').TrimStart(' ').TrimEnd(' ');
                    for (int i = Networks[NetworkIndex, 8].Length - 1; i > 0; i--)
                    {
                        if (Networks[NetworkIndex, 8].Substring(i, 1) == " ")
                        {
                            Networks[NetworkIndex, 8] = Networks[NetworkIndex, 8].Substring(i + 1, Networks[NetworkIndex, 8].Length - 1 - i);
                            break;
                        }
                    }
                }
                if (WlanData.IndexOf("Other Rates") > 0 || WlanData.IndexOf("其他速率") > 0)
                {
                    // overwrite the basic rates if this entry is present
                    Networks[NetworkIndex, 8] = WlanData.Substring(WlanData.IndexOf(":"));
                    if (Networks[NetworkIndex, 8] == ":") { Networks[NetworkIndex, 8] = "not shown"; continue; }
                    Networks[NetworkIndex, 8] = Networks[NetworkIndex, 8].TrimStart(':').TrimStart(' ').TrimEnd(' ');
                    for (int i = Networks[NetworkIndex, 8].Length - 1; i >= 0; i--)
                    {
                        if (Networks[NetworkIndex, 8].Substring(i, 1) == " ")
                        {
                            Networks[NetworkIndex, 8] = Networks[NetworkIndex, 8].Substring(i + 1, Networks[NetworkIndex, 8].Length - 1 - i);
                            break;
                        }
                    }
                }
            }
            #endregion
            #region Update WIFI Status
            listViewWifiStatus.Items.Clear(); // 先清除ListView所有資料再更新
            for (int i = 0; i < listViewWifiStatus.Items.Count; i++)
            {
                // set signal to zero on all items in list
                listViewWifiStatus.Items[i].SubItems[3].Text = "dbm";
                listViewWifiStatus.Items[i].ImageIndex = 5;
            }

            for (int i = 0; i < NetworkIndex + 1; i++)
            {
                if (Networks[i, 0] == " ") continue; // Don't search if no valid MAC Address
                SystemSounds.Hand.Play();// New discovery - add it to the list
                listViewWifiStatus.Items.Add(Networks[i, 0]);                                                   // MAC Address
                listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].SubItems.Add(Networks[i, 1]);      // SSID
                listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].SubItems.Add(Networks[i, 2]);      // Channel
                Signaldbm = Convert.ToInt32(Networks[i, 3].TrimEnd(' ').TrimEnd('%'));
                Signaldbm = (Signaldbm / 2) - 100;
                listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].SubItems.Add(Signaldbm + " dbm");  // Signal
                listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].SubItems.Add(Networks[i, 4]);      // Authenticatiopn
                listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].SubItems.Add(Networks[i, 5]);      // Encryption
                listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].SubItems.Add(Networks[i, 6]);      // Radio Type
                listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].SubItems.Add(Networks[i, 7]);      // Network Type
                listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].SubItems.Add(Networks[i, 8]);      // Speed
                //Trace.WriteLine("MAC : " + Networks[i, 0] + " SSID : " + Networks[i, 1] + " Channel : " + Networks[i, 2] + " SNR : " + Networks[i, 3] + "(" + Convert.ToString(Signaldbm) + ") dbm");

                if (Networks[i, 1].Equals(WiFiSSIDList[WiFiTestCount]))
                {
                    IsWiFiScanned = true; //判斷是否有在範圍內找到欲連線的SSID
                    WiFiSignal = Signaldbm;
                    if (WiFiScanCount.Equals(1)) WiFiSignalAvg = Signaldbm; // 對WiFiSignalAvg進行初始化
                    WiFiSignalAvg = (WiFiSignalAvg + WiFiSignal) / 2; // 用來計算WiFi訊號平均強度
                    UpdateUI("No." + WiFiScanCount + " , WIFI SSID: " + Networks[i, 1] + ", SNR : " + Convert.ToString(Signaldbm) + " dbm");
                }

                int SignalInt = Convert.ToInt32(Networks[i, 3].TrimEnd(' ').TrimEnd('%'));
                if (SignalInt > 50) listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].ImageIndex = 0;
                else if (SignalInt > 40) listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].ImageIndex = 1;
                else if (SignalInt > 30) listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].ImageIndex = 2;
                else if (SignalInt > 20) listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].ImageIndex = 3;
                else if (SignalInt > 0) listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].ImageIndex = 4;

                if ((Networks[i, 4].IndexOf("Open") != -1) & (Networks[i, 5].IndexOf("None") != -1))
                    listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].BackColor = Color.PaleGreen;
                listViewWifiStatus.Items[listViewWifiStatus.Items.Count - 1].EnsureVisible();
            }
            #endregion

            if (!IsWiFiScanned) UpdateUI("Can't find " + WiFiSSIDList[WiFiTestCount] + "\t" + DateTime.Now);
        }

        void PingCompletedCallBack(object sender, PingCompletedEventArgs e)
        {
            tryConnectNetCount++;
            #region If the operation was canceled, display a message to the user.
            if (e.Cancelled)
            {
                UpdateUI("Ping canceled.");
                checkTestStatus("Ping canceled");
                return;
            }
            #endregion

            #region If an error occurred, display the exception to the user.
            if (e.Error != null)
            {
                UpdateUI("Ping failed : " + e.Error.ToString());
                checkTestStatus("Ping failed");
                return;
            }
            #endregion

            mPingReply = e.Reply;
            if (mPingReply == null) return;

            #region 取得傳送封包的狀態 TimedOut / Success
            if (mPingReply.Status == IPStatus.TimedOut) UpdateUI("Connection : " + tryConnectNetCount + " , IP Address: " + mPingReply.Address + " , Request Timed Out"); // 要求等候逾時
            else if (mPingReply.Status == IPStatus.Success)
            {
                PingStatusSuccess++;
                UpdateUI("Connection : " + tryConnectNetCount + " , IP Address: " + mPingReply.Address + " , RoundTrip time: " + mPingReply.RoundtripTime + "ms"); // 取得主機位址跟封包傳送的往返時間
            }
            #endregion

            if (tryConnectNetCount < TotalPingCountWiFi)
            {
                mPingTester.SendAsync(PingAddressList[WiFiTestCount], PingIntervalWiFi, pingBuffer, null);
            }
            else
            {
                UpdateUI("Total : " + tryConnectNetCount + "\tSuccess : " + PingStatusSuccess + "\t" + DateTime.Now);
                DisconnectNetwork();
                if (PingStatusSuccess >= TotalPingCountWiFi * ConnectNetToleranceWiFi)
                {
                    IsWiFiPassedList[WiFiTestCount] = true;
                    WiFiTestCount++;
                    if (WiFiTestCount < WiFiSSIDList.Count())
                    {
                        UpdateUI("WiFiTestCount " + WiFiTestCount + "WiFiSSIDList " + WiFiSSIDList.Count());
                        //UpdateUI("Go to test Wi-Fi 5GHz frequency bands.\t" + DateTime.Now);
                        UpdateUI("Go to test next Wi-Fi.\t" + DateTime.Now);
                        //IsWiFi24gPassed = true;
                        //connectWiFiSSID = connectWiFiSSID5G;
                        //SetHosts("192.168.120.120", "www.winmate");
                        RestoreToDefaultStatus();
                    }
                    else if (!IsTestWiFiTwoBands || IsWiFi24gPassed)
                    {
                        checkTestStatus("PASS");
                    }
                }
                else if (PingStatusSuccess > 0) checkTestStatus("Unstable connection"); // 訊號強度夠, 但是Ping的成功機率小於預設的成功率
                else checkTestStatus("FAIL");
            }
        }

        #region Test Result
        private void checkTestStatus(String testResult)
        {
            DisconnectWiFi(); // DisconnectNetwork();
            if (TimerWifiTest.Enabled) TimerWifiTest.Stop();

            if (mPingTester != null)
            {
                mPingTester.PingCompleted -= new PingCompletedEventHandler(this.PingCompletedCallBack);
                mPingTester = null;
            }

            //connectWiFiSSID = connectWiFiSSID24G;

            // 測試WIFI重複連線測試穩定度用
            if (isLoopTestWiFi.Equals(1))
            {
                if (LoopCount < 100)
                {
                    LoopCount++;
                    if (PingStatusSuccess >= TotalPingCountWiFi * ConnectNetToleranceWiFi) PassCount++;
                    else FailCount++;
                    UpdateUI("\r\n" + "Loop No." + LoopCount + "  PassCount : " + PassCount + ", FailCount : " + FailCount + "\r\n");
                    tryConnectNetCount = 0;
                    WiFiScanCount = 1;
                    TimerWifiTest.Start();
                    return;
                }
                LoopCount = 0;
                PassCount = 0;
                FailCount = 0;
            }

            if (testResult.Equals("PASS"))
            {
                labelResult.Text = "PASS";
                labelResult.ForeColor = Color.Green;
                result["result"] = true;
            }
            else
            {
                if (!testResult.Equals("FAIL")) labelResult.Text = "FAIL. " + testResult;
                else labelResult.Text = "FAIL";
                labelResult.ForeColor = Color.Red;
                result["result"] = false;
                //if (PingStatusSuccess <= 0)
            }

            if (tryConnectNetCount > 1) // 當Ping的次數>1才是真正的有開始進行連線測試, 此時才需要紀錄SNR跟連線成功率
            {
            }

            for (int i = 0; i < EthernetGateway.Count(); i++)
                SetRouteTable(EthernetGateway[i], EthernetInterface[i], 1, Operation.Add);

            //RemoveHosts("www.winmate");
            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            if (!ShowWindow)
                Environment.Exit(0);
        }

        #endregion

        #region Update UI
        public delegate void SafeWinFormsThreadDelegate(string msg);
        private void UpdateUI(string msg)
        {
            if (txtWiFiDetails.InvokeRequired)// 如果控制項的 Handle 是在UI執行緒以外的執行緒上建立, 則為 true (表示必須透過叫用方法呼叫), 否則為 false. 
            {   // 如果您從不同的執行緒呼叫方法, 則必須使用Invoke來封送處理對適當執行緒的呼叫. 
                SafeWinFormsThreadDelegate deviceDetail = new SafeWinFormsThreadDelegate(UpdateDetails);
                txtWiFiDetails.Invoke(deviceDetail, new object[] { msg });   // Organize the parameters as an object array
            }
            else// if invoke is not necessary, process it directly.
            {
                UpdateDetails(msg);
            }
        }
        private void UpdateDetails(string msg)
        {
            if (IsDebugMode) Trace.WriteLine(msg);
            txtWiFiDetails.Text = txtWiFiDetails.Text + msg + Environment.NewLine;
            if (txtWiFiDetails.Text.Length > 10000) txtWiFiDetails.Clear(); // txtWiFiDetails.Text = txtWiFiDetails.Text.Substring(7500, 2500);
            txtWiFiDetails.SelectionStart = txtWiFiDetails.Text.Length;
            txtWiFiDetails.ScrollToCaret();
        }
        #endregion

        #region Button Event
        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectNetwork();
        }

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            RestoreToDefaultStatus();
        }

        private void RestoreToDefaultStatus()
        {
            // 一旦載入測項/重測, 除非有測試結果, 不然不讓使用者點選清單

            labelResult.ForeColor = Color.Black;
            labelResult.Text = "Not Result";

            IsWiFiScanned = false;  // 判斷是否有在範圍內找到欲連線的SSID
            WiFiScanCount = 1;
            WiFiSignal = 0;
            WiFiSignalAvg = 0;

            if (mPingTester != null)
            {
                mPingTester.PingCompleted -= new PingCompletedEventHandler(this.PingCompletedCallBack);
                mPingTester = null;
            }

            UpdateUI("\r\nWIFI automatically to reconnect ... \r\n");

            TimerWifiTest.Start();
        }
        #endregion

    }
}
