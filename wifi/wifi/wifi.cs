using Newtonsoft.Json.Linq;
using SimpleWifi;
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
using System.Threading.Tasks;
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
        bool IsWiFiScanned = false;  // 判斷是否有在範圍內找到欲連線的SSID
        bool IsWiFi24gPassed = false;// 2.4G頻段是否已經測試完畢
        int WiFiScanCount = 1;          // 進行WIFI連線前必須先掃描5次, 計算SSID的平均SNR強度
        int WiFiSignalAvg = 0;          // 用來計算WiFi訊號平均強度
        // 測試WIFI重複連線測試穩定度用
        int LoopCount = 0; // 預設跑100次迴圈測試
        int PassCount = 0;
        int FailCount = 0;
        int TotalPingCountWiFi = 50;                    // 嘗試PING到遠端主機的次數
        double ConnectNetToleranceWiFi = 0.9;           // Ping連線時成功機率的最低容忍值
        int PingIntervalWiFi = 1000;                    // 每次Ping所等待回應的最大時間
        int SNRToleranceWiFi = 65;                      // 連線WIFI訊號強度的最低容忍值
        int DelayWiFiTestTime = 1000;                     // 當送出WIFI連線指令後, 要延遲多少秒後才開始進行WIFI連線測試?
        int isLoopTestWiFi = 0;                         // 是否要開啟迴圈測試? 開啟之後會連續測試100次網路
        int isConnectTestWiFi = 1;
        int WiFiScanTime = 3;                           // 如果找不到既定的SSID要重新掃描幾次?
        bool IsTestWiFiTwoBands = false;                //是否要同時測試WIFI 2.4G跟5G的兩個頻段
        Ping mPingTester;                         // 可以讓應用程式判斷是否能透過網路存取遠端電腦. 
        PingReply mPingReply;                     // 儲存由 Ping.Send 或 Ping.SendAsync而產生的狀態和資料等相關資訊
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
        List<bool> IsWiFiPassedList;
        List<AccessPoint> ScanWiFiList;
        int WiFiTestCount = 0;
        Wifi Wifi = null;
        AccessPoint ConnectAccessPoint = null;
        string ip = string.Empty;
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

        void wifi_Load(object sender, EventArgs e)
        {
            result["result"] = false;
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Exit();
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            ShowWindow = (bool)jobject.ShowWindow;
            SNRToleranceWiFi = (int)jobject.SNRTolerance;
            ConnectNetToleranceWiFi = (double)jobject.PingSuccessRate;
            TotalPingCountWiFi = (int)jobject.TotalPingCount;
            WiFiScanTime = (int)jobject.WiFiScanTime;
            PingIntervalWiFi = (int)jobject.PingIntervalWiFi;
            PingAddressList = jobject.PingAddressWiFi.ToObject<List<string>>();
            WiFiSSIDList = jobject.SSID.ToObject<List<string>>();
            WiFiPasswordList = jobject.Password.ToObject<List<string>>();
            IsWiFiPassedList = new List<bool>();
            IsWiFiPassedList.AddRange(Enumerable.Repeat(false, WiFiSSIDList.Count()));

            if (ShowWindow)
            {
                this.Opacity = 100;
                this.ShowInTaskbar = true;
            }

            if (IsDebugMode) Trace.WriteLine("WiFi_Load");

            Wifi = new Wifi();
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

        void TimerWifiTest_Tick(object sender, EventArgs e)
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
            Wifi.Disconnect();
            ScanWiFiList = Wifi.GetAccessPoints();
            ScanWiFiList.ForEach(e => e.DeleteProfile());
        }        

        public Task<bool> ConnectWiFiSSID(string Password)
        {
            var tcs = new TaskCompletionSource<bool>();
            AuthRequest authRequest = new AuthRequest(ConnectAccessPoint);
            bool overwrite = true;
            if (authRequest.IsPasswordRequired)
            {
                if (ConnectAccessPoint.HasProfile)
                // If there already is a stored profile for the network, we can either use it or overwrite it with a new password.
                {
                    overwrite = false;
                }

                if (overwrite)
                {
                    authRequest.Password = Password;

                    //if (authRequest.IsUsernameRequired)
                    //{
                    //    Console.Write("\r\nPlease enter a username: ");
                    //    authRequest.Username = Console.ReadLine();
                    //}

                    //if (authRequest.IsDomainSupported)
                    //{
                    //    Console.Write("\r\nPlease enter a domain: ");
                    //    authRequest.Domain = Console.ReadLine();
                    //}
                }
            }

            ConnectAccessPoint.ConnectAsync(authRequest, overwrite, e => tcs.TrySetResult(e));
            return tcs.Task;
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
                        Trace.WriteLine(command);
                        sw.WriteLine(command);
                        command = string.Format("route delete 192.168.0.0");
                        Trace.WriteLine(command);
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
                Trace.WriteLine(ex);
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

        public string GetIpByHostName(string hostName)
        {
            hostName = hostName.Trim();
            if (hostName == string.Empty)
                return string.Empty;

            try
            {
                System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(hostName);
                return host.AddressList.GetValue(0).ToString();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("hostname exception {0}",ex));
                return string.Empty;
            }
        }

        public string DoGetHostAddresses(string hostname)
        {
            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(hostname);
                return addresses.FirstOrDefault().ToString();
            }
            catch(Exception ex)
            {
                Trace.WriteLine(Wifi.ConnectionStatus);
                Trace.WriteLine(string.Format("DoGetHostAddresses exception {0}", ex));
                return string.Empty;
            }
        }

        public void GetIpFromHost(int TimeOut)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var GetIpTask = Task.Factory.StartNew(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    ip = GetIpByHostName(PingAddressList[WiFiTestCount]);
                    if (!string.IsNullOrEmpty(ip))
                    {
                        tokenSource.Cancel();
                    }

                    Thread.Sleep(1000);
                }
            }, token);

            GetIpTask.Wait(TimeOut);
            tokenSource.Cancel();
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
                    ConnectWiFiSSID(WiFiPasswordList[WiFiTestCount]).Wait();
                    UpdateUI("WIFI connectivity testing... " + DateTime.Now);
                    tryConnectNetCount = 0;
                    PingStatusSuccess = 0;

                    GetIpFromHost(6000);

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

                        Thread.Sleep(50);
                        
                        mPingTester.SendAsync(ip, PingIntervalWiFi, pingBuffer, null);
                        //mPingTester.SendAsync(ip, PingIntervalWiFi, pingBuffer, null); // 開啟一個Thread來Ping遠程主機
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

        void ScanWiFiSSID()
        {
            UpdateUI("Scan: " + WiFiScanCount);
            ScanWiFiList = Wifi.GetAccessPoints();

            if (ScanWiFiList.Any(e => e.Name.Contains(WiFiSSIDList[WiFiTestCount])))
            {
                ConnectAccessPoint = ScanWiFiList.FindLast(e => string.Equals(e.Name, WiFiSSIDList[WiFiTestCount]));
                var quality = ConnectAccessPoint.SignalStrength;
                var dbm = (Convert.ToInt32(quality) / 2) - 100;
                IsWiFiScanned = true;
                WiFiSignalAvg = dbm;
                UpdateUI("WIFI SSID: " + ConnectAccessPoint.Name + ", SNR : " + dbm.ToString() + " dbm");
            }
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
                mPingTester.SendAsync(ip, PingIntervalWiFi, pingBuffer, null);
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
                        UpdateUI("Go to test next Wi-Fi.\t" + DateTime.Now);
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
        void checkTestStatus(String testResult)
        {
            DisconnectNetwork();
            if (TimerWifiTest.Enabled) TimerWifiTest.Stop();

            if (mPingTester != null)
            {
                mPingTester.PingCompleted -= new PingCompletedEventHandler(this.PingCompletedCallBack);
                mPingTester = null;
            }

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
                Exit();
        }

        #endregion

        #region Update UI
        public delegate void SafeWinFormsThreadDelegate(string msg);
        void UpdateUI(string msg)
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
        void UpdateDetails(string msg)
        {
            if (IsDebugMode) Trace.WriteLine(msg);
            txtWiFiDetails.Text = txtWiFiDetails.Text + msg + Environment.NewLine;
            if (txtWiFiDetails.Text.Length > 10000) txtWiFiDetails.Clear(); // txtWiFiDetails.Text = txtWiFiDetails.Text.Substring(7500, 2500);
            txtWiFiDetails.SelectionStart = txtWiFiDetails.Text.Length;
            txtWiFiDetails.ScrollToCaret();
        }
        #endregion

        #region Button Event
        void buttonDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectNetwork();
        }

        void buttonRetry_Click(object sender, EventArgs e)
        {
            RestoreToDefaultStatus();
        }

        void RestoreToDefaultStatus()
        {
            // 一旦載入測項/重測, 除非有測試結果, 不然不讓使用者點選清單

            labelResult.ForeColor = Color.Black;
            labelResult.Text = "Not Result";

            IsWiFiScanned = false;  // 判斷是否有在範圍內找到欲連線的SSID
            WiFiScanCount = 1;
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
