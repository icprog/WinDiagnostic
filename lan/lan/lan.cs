using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace lan
{
    public partial class lan : Form
    {
        #region Definition
        private BackgroundWorker mBackgroundWorkerLAN;

        // 測試LAN重複連線測試穩定度用
        private int LoopCount; // 預設跑100次迴圈測試
        private int PassCount;
        private int FailCount;

        private int NumberOfEthernets = 0;// Ethernet網卡有幾張? EX: I211, I218
        private string Ethernet1_Description = "";  // 存放Ethernet網卡1的Description, Ex : Intel(R) PRO/1000 PT Desktop Adapter
        private string Ethernet1_MAC = "";          // 存放Ethernet網卡1的MAC, Ex : 001B21265573
        private string Ethernet1_OperationalStatus = ""; // 存放Ethernet網卡1的連線狀態, Ex : UP / DOWN
        private string Ethernet2_Description = "";  // 存放Ethernet網卡2的Description
        private string Ethernet2_MAC = "";          // 存放Ethernet網卡2的MAC
        private string Ethernet2_OperationalStatus = ""; // 存放Ethernet網卡1的連線狀態, Ex : UP / DOWN
        public static bool IsDebugMode = true;
        public static bool IsDeveloperMode = false;
        public static bool EnableAuthentication = false;
        bool ShowWindow = false;
        private BackgroundWorker mBackgroundWorker_FindDevice;
        JObject result = new JObject();

        string TestProduct = string.Empty;
        bool IsBenzIH83 = true;
        string PingAddress = string.Empty;
        int LanNum = 0;
        #endregion

        #region Connect to Network(LAN/WIFI/WWAN/DOCKING)
        public static Ping mPingTester;                         // 可以讓應用程式判斷是否能透過網路存取遠端電腦. 
        public static PingReply mPingReply;                     // 儲存由 Ping.Send 或 Ping.SendAsync而產生的狀態和資料等相關資訊
        // public string who = "192.168.100.46";
        public static string pingData = "TestWinmatePingTestWinmatePingTestWinmatePingTestWinmatePingTest";
        public static byte[] pingBuffer = Encoding.ASCII.GetBytes(pingData);
        public static PingOptions pingOptions = new PingOptions(64, true);
        public static AutoResetEvent pingWaiter = new AutoResetEvent(false); // 向等候的執行緒通知發生事件
        // public static int timeout = 250;

        public static int tryConnectNetCount = 0;   // 目前嘗試ping到遠端主機的次數
        public static int PingStatusSuccess = 0;    // Ping成功的次數
        #endregion

        #region LAN
        public static int TotalPingCount = 50;                      // 嘗試PING到遠端主機的次數
        public static double ConnectNetTolerance = 0.9;             // Ping連線時成功機率的最低容忍值
        public static int PingInterval = 1000;          	        // 每次Ping所等待回應的最大時間

        public static int isLoopTestLAN = 0;                        // 是否要開啟迴圈測試？開啟之後會連續測試100次網路
        public static double LANTime;                               // 用以計算單一測項的測試時間
        #endregion

        List<string> EthernetGateway = new List<string>();
        List<string> WiFiGateway = new List<string>();
        List<int> EthernetInterface = new List<int>();
        List<int> WiFiInterface = new List<int>();
        bool bDoNext = false;
        object objlock = new object();

        public enum Operation
        {
            Add,
            Delete,
            Change
        }

        #region DllImport
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [Conditional("DEBUG")]
        static extern void AllocTrace();
        #endregion

        public lan()
        {
            InitializeComponent();
        }

        string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(exepath, path);
        }

        private void LAN_Load(object sender, EventArgs e)
        {
            result["result"] = "FAIL";
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Environment.Exit(0);
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            TestProduct = (string)jobject.TestProduct;
            IsBenzIH83 = (bool)jobject.IsBenzIH83;
            PingAddress = (string)jobject.PingAddress;
            ShowWindow = (bool)jobject.ShowWindow;
            LanNum = (int)jobject.LanNum;

            if (ShowWindow)
            {
                this.Opacity = 100;
                this.ShowInTaskbar = true;
            }

            if (IsDebugMode) Trace.WriteLine("LAN_Load");

            //DisconnectNetwork();
            //PublicFunctionNetwork.EnableNetworkTypeAdapter(PublicFunctionNetwork.EthernetInterface);

            textBoxPingAddress.Text = PingAddress;
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChangedCallback);
            ShowEthernetInterfaces();
            Trace.WriteLine("Number Of Ethernets : " + NumberOfEthernets);

            // OP測試前要先插cable, 先尋找UP狀態的網卡再指定到第一順位的網路介面, 只測試具有連線能力的側邊LAN Port
            if ((TestProduct.Equals("IBWD") || IsBenzIH83))
            {
                Trace.WriteLine("WindyBox(IBWD) special rules. There are three network interfaces, only one can connect the Internet.");
                if (NumberOfEthernets.Equals(3))
                {
                    // 假定已連線狀態的P-BOX為網卡3, 檢查剩下兩張網卡哪一個具有連線能力, 只顯示它並連線測試
                    if (IsEthernet2Available())
                    {
                        labelEthernet1.Text = Ethernet2_Description.Substring(0, 27);
                        Ethernet1_OperationalStatus = "Up";
                        Ethernet2_OperationalStatus = "Down";
                        Ethernet1_MAC = Ethernet2_MAC;
                    }
                    else if (IsEthernet1Available())
                    {
                        labelEthernet1.Text = Ethernet1_Description.Substring(0, 27);
                    }
                    NumberOfEthernets = 1;
                }
                else
                {
                    checkTestStatus("Can't find all network interfaces. Expected value : 3");
                }
            }
            else if (NumberOfEthernets.Equals(0))
            {
                if (IsDebugMode)
                {
                    Trace.WriteLine("Can't find available network interfaces.");
                }
                checkTestStatus("Can't find available network interfaces.");
                return;
            }
            else if (NumberOfEthernets.Equals(1))
            {
                labelEthernet1.Text = Ethernet1_Description.Substring(0, 27);
            }
            else if (NumberOfEthernets >= 2)
            {
                labelEthernet1.Text = Ethernet1_Description.Substring(0, 27);
                groupBoxEthernet2.Visible = true;
                labelEthernet2.Text = Ethernet2_Description.Substring(0, 27);
            }

            #region Prepare BackgroundWorker
            this.mBackgroundWorkerLAN = new System.ComponentModel.BackgroundWorker();
            this.mBackgroundWorkerLAN.WorkerReportsProgress = true;
            this.mBackgroundWorkerLAN.WorkerSupportsCancellation = true;
            this.mBackgroundWorkerLAN.DoWork += new System.ComponentModel.DoWorkEventHandler(this.mBackgroundWorkerLAN_DoWork);
            this.mBackgroundWorkerLAN.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.mBackgroundWorkerLAN_ProgressChanged);
            this.mBackgroundWorkerLAN.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.mBackgroundWorkerLAN_RunWorkerCompleted);
            #endregion

            // If we are using the background worker, use its RunWorkerAsync() to tell it to start its work
            //if (CheckLanSettingReady())
            //{
            //    if (IsEthernet1Available()) buttonConnectEthernet1_Click(null, new EventArgs()); // mBackgroundWorkerLAN.RunWorkerAsync();
            //    else if (IsEthernet2Available()) buttonConnectEthernet2_Click(null, new EventArgs());
            //}
            if (NumberOfEthernets != LanNum)
                checkTestStatus("FAIL");
            else
                Trigger();
        }

        void Trigger()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            Task t = Task.Factory.StartNew(() =>
            {
                if (CheckLanSettingReady())
                {
                    if (IsEthernet1Available())
                    {
                        buttonConnectEthernet1_Click(null, new EventArgs());
                    }
                    else
                        bDoNext = true;

                    while (true)
                    {
                        if (bDoNext)
                            break;
                        Trace.WriteLine("Wait...");
                        Thread.Sleep(500);
                    }

                    if (NumberOfEthernets >= 2)
                    {
                        if (IsEthernet2Available()) buttonConnectEthernet2_Click(null, new EventArgs());
                        else
                            checkTestStatus("FAIL");
                    }
                }
            }, token);
        }

        private bool CheckLanSettingReady()
        {
            //return !mBackgroundWorkerLAN.IsBusy && !textBoxPingAddress.Text.Length.Equals(0) && IsSingleEthernetAvailable() && !NumberOfEthernets.Equals(0);
            return !mBackgroundWorkerLAN.IsBusy && !textBoxPingAddress.Text.Length.Equals(0) && !NumberOfEthernets.Equals(0);
        }

        private bool IsSingleEthernetAvailable()
        {
            if ((IsEthernet1Available() && !IsEthernet2Available()) ||
                (!IsEthernet1Available() && IsEthernet2Available()))
            {
                return true;
            }
            else return false;
        }
        private bool IsEthernet1Available()
        {
            Trace.WriteLine("IsEthernet1Available ****** {0}", Ethernet1_OperationalStatus);
            return Ethernet1_OperationalStatus.Equals("Up") ? true : false;
        }
        private bool IsEthernet2Available()
        {
            Trace.WriteLine("IsEthernet2Available ****** {0}", Ethernet2_OperationalStatus);
            return Ethernet2_OperationalStatus.Equals("Up") ? true : false;
        }

        private void AddressChangedCallback(object sender, EventArgs e)
        {
            ShowEthernetInterfaces();
        }

        private void ShowEthernetInterfaces()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType.Equals(NetworkInterfaceType.Ethernet) && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    if (NumberOfEthernets <= 2) NumberOfEthernets++;
                    // Trace.WriteLine("NumberOfEthernets : " + NumberOfEthernets);
                    if (NumberOfEthernets.Equals(1))
                    {
                        Ethernet1_Description = adapter.Description.ToString();
                        Ethernet1_MAC = adapter.GetPhysicalAddress().ToString();
                        Ethernet1_OperationalStatus = adapter.OperationalStatus.ToString();
                    }
                    else if (NumberOfEthernets.Equals(2))
                    {
                        Ethernet2_Description = adapter.Description.ToString();
                        Ethernet2_MAC = adapter.GetPhysicalAddress().ToString();
                        Ethernet2_OperationalStatus = adapter.OperationalStatus.ToString();
                    }
                    else
                    {
                        if (adapter.Description.Equals(Ethernet1_Description)) Ethernet1_OperationalStatus = adapter.OperationalStatus.ToString();
                        else if (adapter.Description.Equals(Ethernet2_Description)) Ethernet2_OperationalStatus = adapter.OperationalStatus.ToString();
                    }

                    var ip = adapter.GetIPProperties().UnicastAddresses.Where(d => d.Address.AddressFamily == AddressFamily.InterNetwork).Select(e => e.Address);
                    var gateway = adapter.GetIPProperties().GatewayAddresses.Where(d => d.Address.AddressFamily == AddressFamily.InterNetwork).Select(e => e.Address);
                    var interfaceindex = adapter.GetIPProperties().GetIPv4Properties().Index;
                    Trace.WriteLine(adapter.Description);
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
                        EthernetGateway.Add(s);
                        EthernetInterface.Add(interfaceindex);

                    }

                    if (IsDebugMode)
                    {
                        Trace.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '='));
                        Trace.WriteLine(adapter.Description);
                        Trace.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '='));
                        Trace.WriteLine("  Interface type .......................... : " + adapter.NetworkInterfaceType);
                        Trace.WriteLine("  Physical Address ........................ : " + adapter.GetPhysicalAddress().ToString());
                        Trace.WriteLine("  Operational status ...................... : " + adapter.OperationalStatus);
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        string versions = "";

                        Trace.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '=')); ;
                        Trace.WriteLine(adapter.Description);
                        Trace.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '='));
                        Trace.WriteLine("  Interface type .......................... : " + adapter.NetworkInterfaceType);
                        Trace.WriteLine("  Physical Address ........................ : " + adapter.GetPhysicalAddress().ToString());
                        Trace.WriteLine("  Operational status ...................... : " + adapter.OperationalStatus);

                        // Create a display string for the supported IP versions.
                        if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                        {
                            versions = "IPv4";
                        }
                        if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                        {
                            if (versions.Length > 0)
                            {
                                versions += " ";
                            }
                            versions += "IPv6";
                        }
                        Trace.WriteLine("  IP version .............................. : " + versions);

                        // The following information is not useful for loopback adapters.
                        if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                        {
                            continue;
                        }
                        Trace.WriteLine("  DNS suffix .............................. : " + properties.DnsSuffix);

                        if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                        {
                            IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                            Trace.WriteLine("  MTU...................................... : " + ipv4.Mtu);
                        }

                        Trace.WriteLine("  DNS enabled ............................. : " + properties.IsDnsEnabled);
                        Trace.WriteLine("  Dynamically configured DNS .............. : " + properties.IsDynamicDnsEnabled);
                        Trace.WriteLine("  Receive Only ............................ : " + adapter.IsReceiveOnly);
                        Trace.WriteLine("  Multicast ............................... : " + adapter.SupportsMulticast);
                        Trace.WriteLine("");

                        Trace.WriteLine("  DNS enabled ............................. : " + properties.IsDnsEnabled);
                        Trace.WriteLine(adapter.Description);
                        Trace.WriteLine("  Dynamically configured DNS .............. : " + properties.IsDynamicDnsEnabled);
                        Trace.WriteLine("  Interface type .......................... : " + adapter.NetworkInterfaceType);
                        Trace.WriteLine("  Receive Only ............................ : " + adapter.IsReceiveOnly);
                        Trace.WriteLine("  Multicast ............................... : " + adapter.SupportsMulticast);
                        Trace.WriteLine("");
                    }
                }
            }
        }

        #region BackgroundWorker
        /// <summary>
        /// The BackgroundWorker object runs its DoWork event handler in the background
        /// </summary>
        private void mBackgroundWorkerLAN_DoWork(object sender, DoWorkEventArgs e)
        {
            // 匿名方法 (Anonymous Method)
            // 匿名方法要求參數的是一個委託(delegate)類型, 編譯器在處理匿名方法的時候, 需要指定這個匿名方法將會返回什麼類型的委託, MethodInvoke和Action都是方法返回類型為空的委託
            if (this.InvokeRequired) this.Invoke(new Action(TestNetworkStability));
            else TestNetworkStability();
        }

        /// <summary>
        /// BackgroundWorker fires its ProgressChanged event when the worker thread reports progress
        /// </summary>
        private void mBackgroundWorkerLAN_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        /// <summary>
        /// BackgroundWorker fires its RunWorkerCompleted event when its work is done (or cancelled)
        /// </summary>
        private void mBackgroundWorkerLAN_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IsDebugMode) Trace.WriteLine("LAN Test Completed.");
        }
        #endregion

        public void TestNetworkStability()
        {
            try
            {
                UpdateUI("Test the Stability of Internet.");
                tryConnectNetCount = 0;
                PingStatusSuccess = 0;

                if (mPingTester == null)
                {
                    mPingTester = new Ping();
                    // When the PingCompleted event is raised, the PingCompletedCallback method is called.
                    mPingTester.PingCompleted += new PingCompletedEventHandler(this.PingCompletedCallBack);
                }

                Thread.Sleep(300);
                mPingTester.SendAsync(textBoxPingAddress.Text, PingInterval, pingBuffer, null);
            }
            catch (Exception ex)
            {
                UpdateUI(ex.Message);
                checkTestStatus(ex.Message);
            }
        }

        void PingCompletedCallBack(object sender, PingCompletedEventArgs e)
        {
            tryConnectNetCount++;
            #region If the operation was canceled, display a message to the user.
            if (e.Cancelled)
            {
                UpdateUI("Ping canceled.");
                checkNetworkStatus();
                return;
            }
            #endregion

            #region If an error occurred, display the exception to the user.
            if (e.Error != null)
            {
                UpdateUI("Ping failed : " + e.Error.ToString());
                checkNetworkStatus();
                return;
            }
            #endregion

            mPingReply = e.Reply;
            if (mPingReply == null) return;

            #region 取得傳送封包的狀態 (Time Out or Success)
            if (mPingReply.Status == IPStatus.TimedOut) UpdateUI("Connection : " + tryConnectNetCount + " , IP Address: " + mPingReply.Address.ToString() + " , Request Timed Out"); // 要求等候逾時
            else if (mPingReply.Status == IPStatus.Success)
            {
                PingStatusSuccess++;
                UpdateUI("Connection : " + tryConnectNetCount + " , IP Address: " + mPingReply.Address.ToString() + " , RoundTrip time: " + mPingReply.RoundtripTime + "ms"); // 取得主機位址跟封包傳送的往返時間
            }
            #endregion

            if (tryConnectNetCount < TotalPingCount)
            {
                Thread.Sleep(50); // LAN的PING速度太快, 有時會造成UI Thread假死, 用Sleep暫緩
                mPingTester.SendAsync(textBoxPingAddress.Text, PingInterval, pingBuffer, null);
            }
            else
            {
                checkNetworkStatus();
                bDoNext = true;
            }
        }

        private void checkNetworkStatus()
        {
            UpdateUI("Total : " + tryConnectNetCount + " , Success : " + PingStatusSuccess + " " + System.DateTime.Now);

            // 測試LAN重複連線測試穩定度用 >>
            if (isLoopTestLAN.Equals(1))
            {
                if (LoopCount < 100)
                {
                    LoopCount++;
                    if (PingStatusSuccess >= TotalPingCount * ConnectNetTolerance) PassCount++;
                    else FailCount++;
                    UpdateUI("\r\n" + "Loop No." + LoopCount + "  PassCount : " + PassCount + ", FailCount : " + FailCount + "\r\n");
                    tryConnectNetCount = 0;
                    if (!mBackgroundWorkerLAN.IsBusy) mBackgroundWorkerLAN.RunWorkerAsync();
                    return;
                }
                LoopCount = 0;
                PassCount = 0;
                FailCount = 0;
            }
            // 測試LAN重複連線測試穩定度用 <<

            if (mPingTester != null)
            {
                mPingTester.PingCompleted -= new PingCompletedEventHandler(this.PingCompletedCallBack);
                mPingTester = null;
            }

            if (PingStatusSuccess >= TotalPingCount * ConnectNetTolerance)
            {
                checkTestStatus("PASS");
            }
            else if (PingStatusSuccess == 0 && tryConnectNetCount >= 1)
            {
                checkTestStatus("Ping canceled or failed");
            }
            else if (PingStatusSuccess < TotalPingCount * ConnectNetTolerance) // Ping成功機率要大於ConnectNetTolerance
            {
                checkTestStatus("Unstable connection");
            }
            else
            {
                checkTestStatus("Can't connect network");
            }
        }

        #region Test Result
        private void checkTestStatus(String testResult)
        {
            Trace.WriteLine("testresult: " + testResult);
            buttonClearPingAddress.Enabled = true;

            if (NumberOfEthernets.Equals(1))
            {
                if (testResult.Equals("PASS"))
                {
                    labelEthernet1_Result.ForeColor = Color.Green;
                    labelEthernet1_Result.Text = "PASS";
                }
                else
                {
                    labelEthernet1_Result.ForeColor = Color.Red;
                    labelEthernet1_Result.Text = "FAIL";
                }
            }
            else
            {
                // 先判斷每一個network interface的PASS或是FAIL
                if (testResult.Equals("PASS"))
                {
                    if (IsEthernet1Available() && !IsEthernet2Available())
                    {
                        labelEthernet1_Result.ForeColor = Color.Green;
                        labelEthernet1_Result.Text = "PASS";
                    }
                    else if (!IsEthernet1Available() && IsEthernet2Available())
                    {
                        labelEthernet2_Result.ForeColor = Color.Green;
                        labelEthernet2_Result.Text = "PASS";
                    }
                    else if (IsEthernet1Available() && IsEthernet2Available()) //Peter Add
                    {
                        if (labelEthernet1_Result.Text.Equals("PASS"))
                        {
                            labelEthernet2_Result.ForeColor = Color.Green;
                            labelEthernet2_Result.Text = "PASS";
                        }
                        labelEthernet1_Result.ForeColor = Color.Green;
                        labelEthernet1_Result.Text = "PASS";
                    }

                }
                else
                {
                    if (IsEthernet1Available() && !IsEthernet2Available())
                    {
                        labelEthernet1_Result.ForeColor = Color.Red;
                        labelEthernet1_Result.Text = "FAIL";
                        //testResult = labelEthernet1.Text + " FAIL";
                    }
                    else if (!IsEthernet1Available() && IsEthernet2Available())
                    {
                        labelEthernet2_Result.ForeColor = Color.Red;
                        labelEthernet2_Result.Text = "FAIL";
                        //testResult = labelEthernet2.Text + " FAIL";
                    }
                    else
                    {
                        labelEthernet1_Result.ForeColor = Color.Red;
                        labelEthernet1_Result.Text = "FAIL";
                        labelEthernet2_Result.ForeColor = Color.Red;
                        labelEthernet2_Result.Text = "FAIL";
                        buttonConnectEthernet1.Enabled = false;
                        buttonConnectEthernet2.Enabled = false;
                    }
                }

                if (!IsLan1AndLan2Tested())
                {
                    if (IsEthernet1Available()) buttonConnectEthernet2.Enabled = true;
                    else buttonConnectEthernet1.Enabled = true;
                    //return;
                }
                else if (!IsEthernet1Available() && !IsEthernet2Available()) // 突然拔掉網路線或是兩個網路同時無法連線, 都是屬於這種無法預期的錯誤
                {
                    UpdateUI("Unknown error occurred. Please retry later or check network connection.");
                    if (labelEthernet2_Result.Text.Equals("Unknown"))
                    {
                        labelEthernet2_Result.ForeColor = Color.Red;
                        labelEthernet2_Result.Text = "FAIL";
                    }
                    if (labelEthernet1_Result.Text.Equals("Unknown"))
                    {
                        labelEthernet1_Result.ForeColor = Color.Red;
                        labelEthernet1_Result.Text = "FAIL";
                    }
                }
            }

            buttonConnectEthernet1.Enabled = false;
            buttonConnectEthernet2.Enabled = false;

            if (testResult.Equals("PASS") && IsLan1AndLan2Tested())
            {
                labelResult.ForeColor = Color.Green;
                labelResult.Text = "PASS";

                result["result"] = "PASS";
                result["EIPLog"] = new JObject
                {
                    { "LAN", "PASS" },
                    { "LAN_Info", "PASS"}
                };
            }
            else
            {
                labelResult.ForeColor = Color.Red;
                labelResult.Text = "FAIL";

                result["result"] = "FAIL";
                result["EIPLog"] = new JObject
                {
                    { "LAN", "FAIL" },
                    { "LAN_Info", testResult}
                };
            }
            lock (objlock)
            {
                if (IsLan1AndLan2Tested())
                    ResultToJsonFile();
            }

        }

        private bool IsLan1AndLan2Tested()
        {
            if ((NumberOfEthernets.Equals(1)) ||
                (!labelEthernet1_Result.Text.Equals("Unknown") && !labelEthernet2_Result.Text.Equals("Unknown")))
            {
                return true;
            }
            else return false;
        }

        #endregion

        #region Update UI
        public delegate void SafeWinFormsThreadDelegate(string msg);
        private void UpdateUI(string msg)
        {
            try
            {
                if (txtLANDetails.InvokeRequired) txtLANDetails.Invoke(new SafeWinFormsThreadDelegate(UpdateDetails), new object[] { msg });
                else UpdateDetails(msg);
            }
            catch (Exception ex)
            {
                UpdateDetails(ex.Message);
            }
        }
        private void UpdateDetails(string msg)
        {
            if (IsDebugMode) Trace.WriteLine(msg);
            if (IsDebugMode) Trace.WriteLine(msg);
            txtLANDetails.Text = txtLANDetails.Text + msg + Environment.NewLine;
            if (txtLANDetails.Text.Length > 10000) txtLANDetails.Text = txtLANDetails.Text.Substring(txtLANDetails.Text.Length - 1000, 1000);
            txtLANDetails.SelectionStart = txtLANDetails.Text.Length;
            txtLANDetails.ScrollToCaret();
        }
        #endregion

        #region Button Event
        private void buttonDisconnectLAN_Click(object sender, EventArgs e)
        {

        }

        private void buttonConnectEthernet1_Click(object sender, EventArgs e)
        {
            try
            {
                Trace.WriteLine("***** buttonConnectEthernet1_Click");

                if (EthernetGateway.Count >=2 && EthernetInterface.Count >=2)
                    SetRouteTable(EthernetGateway[1], EthernetInterface[1], 1, Operation.Delete);

                if (CheckLanSettingReady() && IsEthernet1Available())
                {
                    ResetUiToDefaultStatus();

                    labelEthernet1_Result.ForeColor = Color.Blue;
                    labelEthernet1_Result.Text = "Unknown";

                    mBackgroundWorkerLAN.RunWorkerAsync(); // TestNetworkStability();
                }
                else if (IsEthernet1Available() && IsEthernet2Available())
                {
                    // 如果是兩個網路都插著網路線而且不拔除，就不讓它繼續測試
                    //ShowDialogMessageBox("Make sure there is only one network that can be connected.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    if (IsDebugMode) Trace.WriteLine("Make sure there is only one network that can be connected.");
                    if (IsDebugMode) Trace.WriteLine("Make sure there is only one network that can be connected.");
                }
                else
                {
                    //DialogResult ResultLAN1 = ShowDialogMessageBox("LAN1 is unavailable. Please check setting or cable.\r\nThis application can be exited by click cancel button", "Attention", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    //if (ResultLAN1 == DialogResult.Cancel) Application.Exit();

                    if (IsDebugMode) Trace.WriteLine("LAN1 is unavailable. Please check setting or cable.\r\nThis application can be exited by click cancel button.");
                    if (IsDebugMode) Trace.WriteLine("LAN1 is unavailable. Please check setting or cable.\r\nThis application can be exited by click cancel button.");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }

        private void buttonConnectEthernet2_Click(object sender, EventArgs e)
        {
            Trace.WriteLine("***** buttonConnectEthernet2_Click");

            if (EthernetGateway.Count >= 1 && EthernetInterface.Count >= 1)
                SetRouteTable(EthernetGateway[0], EthernetInterface[0], 1, Operation.Delete);
            if (EthernetGateway.Count >= 2 && EthernetInterface.Count >= 2)
                SetRouteTable(EthernetGateway[1], EthernetInterface[1], 1, Operation.Add);

            if (CheckLanSettingReady() && IsEthernet2Available())
            {
                ResetUiToDefaultStatus();

                labelEthernet2_Result.ForeColor = Color.Blue;
                labelEthernet2_Result.Text = "Unknown";

                mBackgroundWorkerLAN.RunWorkerAsync();
            }
            else if (IsEthernet1Available() && IsEthernet2Available())
            {
                // 如果是兩個網路都插著網路線而且不拔除，就不讓它繼續測試
                //ShowDialogMessageBox("Make sure there is only one network that can be connected.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (IsDebugMode) Trace.WriteLine("Make sure there is only one network that can be connected.");
            }
            else
            {
                //DialogResult ResultLAN2 = MainForm.ShowDialogMessageBox("LAN2 is unavailable. Please check setting or cable.\r\nThis application can be exited by click cancel button", "Attention", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                //if (ResultLAN2 == DialogResult.Cancel) Application.Exit();

                if (IsDebugMode) Trace.WriteLine("LAN2 is unavailable. Please check setting or cable.\r\nThis application can be exited by click cancel button.");
            }
        }

        private void ResetUiToDefaultStatus()
        {
            labelResult.ForeColor = Color.Black;
            labelResult.Text = "Not Result";

            buttonConnectEthernet1.Enabled = false;
            buttonConnectEthernet2.Enabled = false;
            buttonClearPingAddress.Enabled = false;

            txtLANDetails.Text = "";
        }
        #endregion

        private void buttonClearPingAddress_Click(object sender, EventArgs e)
        {
            textBoxPingAddress.Text = "";
        }

        private void buttonLanFAIL_Click(object sender, EventArgs e)
        {
            checkTestStatus("Force stop LAN test");
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

        void ResultToJsonFile()
        {
            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            if (!ShowWindow)
                Exit();
        }

        bool SetRouteTable(string gateway, int interfaceindex, int metric, Operation operation)
        {
            Trace.WriteLine("***** SetRouteTable");

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

        private void lan_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < EthernetGateway.Count(); i++)
                SetRouteTable(EthernetGateway[i], EthernetInterface[i], 1, Operation.Add);
        }
    }
}
