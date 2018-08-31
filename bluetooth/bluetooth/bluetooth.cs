using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using InTheHand.Windows.Forms;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bluetooth
{
    public partial class bluetooth : Form
    {
        [DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true, ThrowOnUnmappableChar = true)]
        private static extern bool PlaySound(string pszSound, int hmod, int fdwSound);

        [System.Flags]
        public enum PlaySoundFlags : int
        {
            SND_SYNC = 0x0000,/* play synchronously (default) */
            SND_ASYNC = 0x0001, /* play asynchronously */
            SND_NODEFAULT = 0x0002, /* silence (!default) if sound not found */
            SND_MEMORY = 0x0004, /* pszSound points to a memory file */
            SND_LOOP = 0x0008, /* loop the sound until next sndPlaySound */
            SND_NOSTOP = 0x0010, /* don't stop any currently playing sound */
            SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
            SND_ALIAS = 0x00010000,/* name is a registry alias */
            SND_ALIAS_ID = 0x00110000, /* alias is a pre d ID */
            SND_FILENAME = 0x00020000, /* name is file name */
            SND_RESOURCE = 0x00040004, /* name is resource name or atom */
            SND_PURGE = 0x0040,  /* purge non-static events for task */
            SND_APPLICATION = 0x0080 /* look for application specific association */
        }

        #region Definition
        private BluetoothClient mBluetoothClient; // 先 new 一個 BluetoothClient
        private BluetoothDeviceInfo[] mBluetoothDeviceInfo; // BluetoothDeviceInfo[] 存放藍牙裝置資訊
        private BluetoothDeviceInfo selectedDevice; // 存放SelectBluetoothDeviceDialog 所選取的藍牙裝置資訊
        private BluetoothRadio mBluetoothRadio;
        private bool allowBTConnection = false;  // 當兩台PC端利用S/C架構對傳資料時, 需要將此flag設置成true才可以進行傳輸
        public bool EnableFileTransfer = false;
        public bool EnablePlayAudio = false;
        private string TransferTextChecksum = "";   // PC對傳資料時, Server端用以判斷從Client收到的資料是否重複, 重複就代表Client端已經停止傳輸

        private ObexListener mObexListener; // 用以讀取經由ObexWebRequest傳送的檔案
        private bool serviceStarted; // 判斷 Bluetooth Obex Listener是否已經啟動
        bool IsDebugMode = true;
        bool ShowWindow = false;
        private BackgroundWorker mBackgroundWorker_FindDevice;
        JObject result = new JObject();
        #endregion

        public bluetooth()
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

        private void bluetooth_Closing(object sender, CancelEventArgs e)
        {
            if (mObexListener != null) mObexListener.Stop();
        }

        private void bluetooth_Load(object sender, EventArgs e)
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

            if (ShowWindow)
            {
                this.Opacity = 100;
                this.ShowInTaskbar = true;
            }

            if (IsDebugMode) Trace.WriteLine("BT_Load");

            #region Prepare Bluetooth
            ComboBoxServices.Items.AddRange(new object[] { "SerialPort", "Handsfree", "HumanInterfaceDevice", "SDP Protocol", "DialupNetworking" });  // 藍芽連線用以配對的Service類型
            ComboBoxServices.SelectedIndex = 1;   // Service類型預設為SerialPort
            TextBoxPin.Enabled = CheckBoxUsePin.Checked;    // 如果UsePin被勾選就要讓試用者能輸入PinCode
            txtBTDeviceDetails.Text = "";  // 用以顯示藍芽連線時的各種資訊

            mBackgroundWorker_FindDevice = new BackgroundWorker();
            mBackgroundWorker_FindDevice.WorkerSupportsCancellation = true;
            mBackgroundWorker_FindDevice.WorkerReportsProgress = true;
            mBackgroundWorker_FindDevice.DoWork += new DoWorkEventHandler(FindBluetoothDevice_DoWork);
            mBackgroundWorker_FindDevice.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FindBluetoothDevice_RunWorkerCompleted);
            #endregion

            #region 可選測試清單(檔案傳輸/音樂播放)
            // 當要進行藍牙音樂播放測試的時候, 才會將音樂播放測試清單顯示
            if (EnablePlayAudio) groupBoxBluetoothPair.Visible = true;
            else if (EnableFileTransfer)  // 當要進行藍牙檔案測試的時候, 才會將藍牙檔案測試清單顯示
            {
                groupBTDeviceConnection.Visible = true;
                // 這個Timer主要提供給測試Server作為分辨藍牙傳輸後檔案的正確性（因為不知道何時會有藍牙傳輸, 所以必須定時檢查指定資料夾有沒有檔案進來）
                BluetoothOBEXTimer.Interval = 3000;
                BluetoothOBEXTimer.Start();
            }
            #endregion

            if (!mBackgroundWorker_FindDevice.IsBusy) buttonFindBluetoothDevice.PerformClick(); // mBackgroundWorker_FindDevice.RunWorkerAsync();
            else MessageBox.Show("Searching for Bluetooth devices...Please Wait", "Attention");
        }

        #region Step 1. 藍牙硬體初始化, 尋找藍牙裝置
        void FindBluetoothDevice_DoWork(object sender, DoWorkEventArgs e)
        {
            DisplayBluetoothRadio();
            if (mBluetoothRadio != null) FindBTDevice();
        }

        void FindBluetoothDevice_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateBTDeviceDetails("Search Completed.");
            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            if (!ShowWindow)
                Exit();
        }

        private void buttonFindBluetoothDevice_Click(object sender, EventArgs e)
        {
            if (!mBackgroundWorker_FindDevice.IsBusy)
            {
                Cursor.Current = Cursors.WaitCursor; // 設定Current屬性會變更目前顯示的游標
                buttonFindBluetoothDevice.Enabled = false;
                labelResult.ForeColor = Color.Orange;
                labelResult.Text = "Devices searching ...";
                UpdateBTDeviceDetails("Searching for Bluetooth devices...");
                buttonConnect.Enabled = false;
                buttonAudioTest.Enabled = false;
                mBackgroundWorker_FindDevice.RunWorkerAsync();
            }
            else
            {
                if (IsDebugMode) Trace.WriteLine("Searching for Bluetooth devices...");
                //  MessageBox.Show("Searching for Bluetooth devices...");
            }
        }

        // 顯示Local端藍芽設備的資訊, 以及把Local端的藍芽設備設置成可被發現(RadioMode.Discoverable)
        public void DisplayBluetoothRadio()
        {
            if (IsDebugMode) UpdateBTDeviceDetails("Display Bluetooth Details");
            if (mBluetoothRadio == null) mBluetoothRadio = BluetoothRadio.PrimaryRadio;
            if (mBluetoothRadio == null)
            {
                UpdateBTDeviceDetails("There's no BT radio hardware or unsupported software stack. Stop.");
                UpdateFindDeviceResult("Fail. There's no BT device");
                labelResult.ForeColor = Color.Red;

                checkTestStatus("There's no BT device");

                return;
            }

            Cursor.Current = Cursors.WaitCursor; // 設定Current屬性會變更目前顯示的游標

            #region BluetoothRadio Information
            // Warning: LocalAddress is null if the radio is powered-off.
            labelBtLocalAddress.Text = String.Format("{0:C}", mBluetoothRadio.LocalAddress);
            if (IsDebugMode)
            {
                UpdateBTDeviceDetails("Name: " + mBluetoothRadio.Name + String.Format("\r\nLocal Address: {0:C}", mBluetoothRadio.LocalAddress));
                UpdateBTDeviceDetails("Radio Mode: " + mBluetoothRadio.Mode.ToString());
            }

            Console.WriteLine("HardwareStatus: " + mBluetoothRadio.HardwareStatus);
            Console.WriteLine("HciRevision: " + mBluetoothRadio.HciRevision);
            Console.WriteLine("HciVersion: " + mBluetoothRadio.HciVersion);
            Console.WriteLine("LmpVersion: " + mBluetoothRadio.LmpVersion);
            Console.WriteLine("LocalAddress: " + mBluetoothRadio.LocalAddress);
            Console.WriteLine("Manufacturer: " + mBluetoothRadio.Manufacturer);
            Console.WriteLine("Remote:" + mBluetoothRadio.Remote);
            Console.WriteLine("SoftwareManufacturer: " + mBluetoothRadio.SoftwareManufacturer);
            Console.WriteLine("StackFactory: " + mBluetoothRadio.StackFactory);
            #endregion

            // Enable discoverable mode
            // Bluetooth is turned on. The local device is listed, when a peer device searches for available devices within range. The peer device can pair with the local device. 
            mBluetoothRadio.Mode = RadioMode.Discoverable; // Winmate kenkun modify on 2014/08/11
            // RadioMode.Connectable 
            // Bluetooth is turned on. The local device can initiate a connection with a peer device but the local device is not discoverable by peer devices that are within range.
            // mBluetoothRadio.Mode = RadioMode.Connectable;

            if (IsDebugMode) UpdateBTDeviceDetails("Now Radio Mode : " + mBluetoothRadio.Mode.ToString());
        }

        private void FindBTDevice()
        {
            #region Discover Bluetooth Device
            // Scan the nearby devices
            if (mBluetoothClient != null) mBluetoothClient = null;
            mBluetoothClient = new BluetoothClient();
            mBluetoothDeviceInfo = mBluetoothClient.DiscoverDevices();

            #endregion

            #region 更新ComboBox的資料來源
            comboBoxBluetoothDevices.DataSource = null;
            comboBoxBluetoothDevices.Items.Clear();
            if (mBluetoothDeviceInfo.Length > 0)
            {
                comboBoxBluetoothDevices.DataSource = mBluetoothDeviceInfo;
                comboBoxBluetoothDevices.DisplayMember = "DeviceName"; // 顯示為 DeviceName
                comboBoxBluetoothDevices.ValueMember = "DeviceAddress"; // 值為 DeviceAddress(Connect 時會用到)
            }
            else
            {
                comboBoxBluetoothDevices.Items.Insert(0, "None Selected");
                comboBoxBluetoothDevices.SelectedIndex = 0;
            }
            comboBoxBluetoothDevices.Focus();
            #endregion

            if (mBluetoothDeviceInfo.Length.Equals(0))
            {
                UpdateFindDeviceResult("Fail. No Bluetooth device");
                labelResult.ForeColor = Color.Red;

                checkTestStatus("There is no BT hardware, or it uses unsupported software");

                buttonFindBluetoothDevice.Enabled = true;
            }
            else if (!EnableFileTransfer && !EnablePlayAudio)  // 當沒有開啟檔案傳輸跟播放音樂的進階測試 & 搜尋藍牙裝置!=0, 就直接結束測試
            {
                UpdateFindDeviceResult("PASS. Find " + mBluetoothDeviceInfo.Length + " device(s).");
                labelResult.ForeColor = Color.Green;

                checkTestStatus("PASS");
            }
            else if (EnablePlayAudio) // 有找到藍牙, 開啟播放音樂測試
            {
                UpdateFindDeviceResult("Find " + mBluetoothDeviceInfo.Length + " device(s).");
                labelResult.ForeColor = Color.Blue;
                buttonPairRequest.Enabled = true; // 當開啟藍牙音樂播放測試, 要先啟用裝置配對按鈕 Winmate Kenkun add on 2014/08/26
                buttonAudioFAIL.Enabled = true;
            }
            else if (EnableFileTransfer) // 有找到藍牙, 開啟檔案傳輸測試
            {
                UpdateFindDeviceResult("Find " + mBluetoothDeviceInfo.Length + " device(s).");
                labelResult.ForeColor = Color.Blue;
                buttonSendFile.Enabled = true; // 當開啟藍牙音樂播放測試, 要先啟用裝置配對按鈕 Winmate Kenkun add on 2014/08/26
                buttonFileTestFAIL.Enabled = true;
            }
            else
            {
                UpdateFindDeviceResult("Find " + mBluetoothDeviceInfo.Length + " device");
            }
            Cursor.Current = Cursors.Default;
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
        #endregion

        #region Step 2-1.進行藍牙播放驗證驗證
        #region Bluetooth Pair Request
        delegate bool ActionSecurityWithPin(BluetoothAddress device, string passcode);
        private void buttonPairRequest_Click(object sender, EventArgs e)
        {
            labelResult.ForeColor = Color.Orange;
            labelResult.Text = "Try to pairing ...";
            UpdateBTDeviceDetails("Pairing for Bluetooth devices...");
            // 建立委派物件（C# 3.0 only）
            ActionSecurityWithPin actionPair = (BluetoothAddress device, string passcode) => BluetoothSecurity.PairRequest(device, passcode);
            DoSecurityActionWithPinInput("Pair Request", actionPair);
        }

        private void DoSecurityActionWithPinInput(string name, ActionSecurityWithPin action)
        {
            try
            {
                SelectBluetoothDeviceDialog mDeviceDialog = new SelectBluetoothDeviceDialog(); // 選擇藍牙裝置清單
                mDeviceDialog.ShowRemembered = true;    // 顯示已經記住的藍牙設備  
                mDeviceDialog.ShowAuthenticated = true; // 顯示認證過的藍牙設備  
                mDeviceDialog.ShowUnknown = true;       // 顯示位置藍牙設備  
                if (mDeviceDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedDevice = (BluetoothDeviceInfo)mDeviceDialog.SelectedDevice;
                    UpdateBTDeviceDetails("\r\nDevice Name : " + mDeviceDialog.SelectedDevice.DeviceName.ToString());
                    UpdateBTDeviceDetails("Address : " + mDeviceDialog.SelectedDevice.DeviceAddress.ToString());// 獲取選擇的遠程藍牙地址  

                    bool result = action(selectedDevice.DeviceAddress, "");
                    UpdateBTDeviceDetails("Bluetooth Pair Request, Result : " + result);

                    if (result)
                    {
                        UpdateFindDeviceResult("Device connected.");
                        buttonPairRequest.Enabled = false;
                        buttonAudioTest.Enabled = true;
                    }
                    else SetFailEvent();
                }
            }
            catch (SocketException sex)
            {
                UpdateBTDeviceDetails("Connect failed : " + sex.Message + ", Code :  " + sex.SocketErrorCode.ToString("D"));
                SetFailEvent();
            }
            catch (Exception ex)
            {
                UpdateBTDeviceDetails(ex.Message);
                SetFailEvent();
            }
        }

        private void SetFailEvent()
        {
            UpdateFindDeviceResult("Connect FAILED");
            labelResult.ForeColor = Color.Red;

            checkTestStatus("Can't connect to BT device");
        }

        public Nullable<bool> promptYesNo(string txt, string authType_)
        {
            txt += Environment.NewLine + "----" + Environment.NewLine + authType_;
            DialogResult rslt = MessageBox.Show(this, txt, authType_, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
            switch (rslt)
            {
                case DialogResult.Yes:
                    return true;
                case DialogResult.No:
                    return false;
                case DialogResult.Cancel:
                    return null;
                default:
                    return null;
            }
        }
        #endregion

        private void buttonAudioTest_Click(object sender, EventArgs e)
        {
            PlaySound(Application.StartupPath + "\\Sample.wav", 0, (int)(PlaySoundFlags.SND_ASYNC | PlaySoundFlags.SND_FILENAME)); // 調用API播放聲音

            buttonRemove.Enabled = true;
        }

        #region Remove Bluetooth Device
        delegate bool ActionSecurity(BluetoothAddress device);
        private void buttonSecurityRemoveDevice_Click(System.Object sender, System.EventArgs e)
        {
            buttonAudioTest.Enabled = false;
            UpdateFindDeviceResult("Removing a device...");
            labelResult.ForeColor = Color.Orange;
            ActionSecurity actionRemove = (BluetoothAddress device) => BluetoothSecurity.RemoveDevice(device);
            DoSecurityAction("Remove Bluetooth Device", actionRemove);
        }

        private void DoSecurityAction(string name, ActionSecurity action)
        {
            if (selectedDevice == null && IsDebugMode)
            {
                Trace.WriteLine("No device selected");
            }
            // MessageBox.Show(this, "No device selected", name);
            else
            {
                bool result = action(selectedDevice.DeviceAddress);
                UpdateBTDeviceDetails("Remove Bluetooth Device, Result: " + result);

                if (result)
                {
                    UpdateFindDeviceResult("Device Removed");
                    buttonRemove.Enabled = false;
                    buttonAudioPASS.Enabled = true;
                    buttonAudioFAIL.Enabled = true;
                }
                else
                {
                    UpdateFindDeviceResult("Device not Removed");
                    labelResult.ForeColor = Color.Red;
                }
            }
        }
        #endregion

        private void buttonPASS_Click(object sender, EventArgs e)
        {
            UpdateFindDeviceResult("PASS");
            labelResult.ForeColor = Color.Green;

            checkTestStatus("PASS");
        }

        private void buttonFAIL_Click(object sender, EventArgs e)
        {
            UpdateFindDeviceResult("FAIL");
            labelResult.ForeColor = Color.Red;

            checkTestStatus("Bluetooth Media Test is failed.");
        }
        #endregion

        #region Step 2-2.進行藍牙檔案傳輸驗證
        // 利用藍牙檔案傳輸驗證需要兩台裝置, 並且均要打開測試程式, 一台傳檔, 一台收檔, Client端跟Server端的行為不可同時在同一台裝置執行
        #region Client端
        // Client端用來傳送測試文字檔到Server端的流程
        private void buttonSendFile_Click(object sender, EventArgs e)
        {
            if (!mBackgroundWorker_FindDevice.IsBusy) pushObexFileToBeam();
            else if (IsDebugMode) Trace.WriteLine("Bluetooth connecting...Please wait");
        }

        private void pushObexFileToBeam()
        {
            // use the new select bluetooth device dialog
            SelectBluetoothDeviceDialog mSelectBluetoothDeviceDialog = new SelectBluetoothDeviceDialog();
            mSelectBluetoothDeviceDialog.ShowAuthenticated = true;  // 顯示已經記住的藍牙設備
            mSelectBluetoothDeviceDialog.ShowRemembered = true;     // 顯示認證過的藍牙設備
            mSelectBluetoothDeviceDialog.ShowUnknown = true;        // 顯示位置藍牙設備
            if (mSelectBluetoothDeviceDialog.ShowDialog() == DialogResult.OK)
            {
                OpenFileDialog ofdFileToBeam = new OpenFileDialog(); // 選擇要傳送文件的目的地後, 通過OpenFileDialog選擇要傳輸的文件
                ofdFileToBeam.Filter = "Only Text File (*.txt)|*.txt";// ofdFileToBeam.Filter = "All Files (*.*)|*.*";
                if (ofdFileToBeam.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    System.Uri uri = new Uri("obex:// " + mSelectBluetoothDeviceDialog.SelectedDevice.DeviceAddress.ToString() + "/" + System.IO.Path.GetFileName(ofdFileToBeam.FileName));
                    ObexWebResponse response = null;
                    try
                    {
                        // ObexWebRequest 的實現模式和HttpWebRequest類似, 都是發送請求, 等等回應, 回應封裝在ObexWebResponse 類裡面. 
                        ObexWebRequest request = new ObexWebRequest(uri); // 通過ObexWebRequest 來傳送文件到目標機器
                        request.ReadFile(ofdFileToBeam.FileName);

                        response = request.GetResponse() as ObexWebResponse;
                        txtCompareFileResult.ForeColor = Color.Green;
                        txtCompareFileResult.Text = "PASS";

                        if (IsDebugMode) Trace.WriteLine("File transfer was successful.");

                        checkTestStatus("PASS");
                    }
                    catch (Exception ex)
                    {
                        txtCompareFileResult.ForeColor = Color.Red;
                        txtCompareFileResult.Text = "FAIL";

                        if (IsDebugMode) Trace.WriteLine("File transfer failed. Path : " + uri);

                        checkTestStatus(ex.Message);
                    }
                    finally
                    {
                        if (response != null) response.Close();
                    }
                    Cursor.Current = Cursors.Default;
                }
            }
        }
        #endregion

        #region Server端
        // Server端用來定時到指定資料夾去辨識是否有指定名稱的文件（經藍牙傳輸的檔案）, 並檢查檔案的正確性
        private void BluetoothOBEXTimer_Tick(object sender, EventArgs e)
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal); // 回傳系統預設的"我的文件"資料夾路徑
            string filename = "WinmateBluetoothTest.txt";
            string sourceFile = path + Path.DirectorySeparatorChar + filename;
            if (File.Exists(sourceFile) == true)
            {
                BluetoothOBEXTimer.Stop();
                Boolean fileChecksum = CompareFile(sourceFile);
                if (fileChecksum)
                {
                    txtCompareFileResult.ForeColor = Color.Green;
                    txtCompareFileResult.Text = "Pass!";
                    DialogResult fileTransferResult = MessageBox.Show("Bluetooth file transfer the same content, Pass.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (fileTransferResult == DialogResult.OK) BluetoothOBEXTimer.Start();
                }
                else
                {
                    txtCompareFileResult.ForeColor = Color.Red;
                    txtCompareFileResult.Text = "Fail!";
                    DialogResult fileTransferResult = MessageBox.Show("Bluetooth file content is not the same, Fail.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (fileTransferResult == DialogResult.OK) BluetoothOBEXTimer.Start();
                }
                System.IO.File.Delete(sourceFile);
            }
            else
            {
                txtCompareFileResult.ForeColor = Color.Blue;
                txtCompareFileResult.Text = "Not Result";
            }
        }

        private bool CompareFile(string sourceFile)
        {
            string fileContent = File.ReadAllText(sourceFile);
            string destStringContent = "WinmateBluetoothTest";
            if (fileContent.Length != destStringContent.Length && String.Compare(fileContent, destStringContent) != 0) return false;
            else return true;
        }
        #endregion

        private void buttonFileTestFAIL_Click(object sender, EventArgs e)
        {
            txtCompareFileResult.ForeColor = Color.Red;
            txtCompareFileResult.Text = "FAIL";

            checkTestStatus("Bluetooth File Test is failed.");
        }
        #endregion

        #region 藍牙檔案傳輸驗證 (舊方法, 未使用)
        private void buttonBTServiceListen_Click(object sender, EventArgs e)
        {
            UpdateBTDeviceDetails("Create BT Concection Service Side");
            DisplayBluetoothRadio();
            if (mBluetoothRadio != null) StartService();
        }
        private void StartService()
        {
            allowBTConnection = true;
            BluetoothListener mServerListener = new BluetoothListener(BluetoothService.SerialPort);
            mServerListener.Start();
            UpdateBTDeviceDetails("Server service started!");
            BluetoothClient mServiceBluetoothClient = mServerListener.AcceptBluetoothClient();
            UpdateBTDeviceDetails("Got a request!");

            Stream peerStream = mServiceBluetoothClient.GetStream();

            string dataToSend = "Start BT connection from BT Server Side! \r\n";

            // Convert dataToSend into a byte array
            byte[] dataBuffer = System.Text.ASCIIEncoding.ASCII.GetBytes(dataToSend);

            // Output data to stream
            peerStream.Write(dataBuffer, 0, dataBuffer.Length);

            byte[] buffer = new byte[2000];
            while (allowBTConnection)
            {
                if (peerStream.CanRead)
                {
                    peerStream.Read(buffer, 0, 50);
                    string data = System.Text.ASCIIEncoding.ASCII.GetString(buffer, 0, 50);
                    if (TransferTextChecksum == data)
                    {
                        allowBTConnection = false;
                        UpdateBTDeviceDetails("Server -- Close BT Service");
                        // Close network stream
                        peerStream.Close();
                        mServiceBluetoothClient.Close();
                        return;
                    }
                    TransferTextChecksum = data;
                    UpdateBTDeviceDetails("Server -- Receiving = " + data);
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void buttonBTServiceSend_Click(object sender, EventArgs e)
        {
            UpdateBTDeviceDetails("Create BT Concection Client Side");
            DisplayBluetoothRadio();
            if (mBluetoothRadio != null) ConnectService();
        }
        private void ConnectService()
        {
            allowBTConnection = true;
            BluetoothClient mClientBluetoothClient = new BluetoothClient();
            BluetoothDeviceInfo[] devices = mClientBluetoothClient.DiscoverDevices();
            BluetoothDeviceInfo device = null;
            foreach (BluetoothDeviceInfo d in devices)
            {
                if (d.DeviceAddress.ToString() == comboBoxBluetoothDevices.SelectedValue.ToString())
                {
                    device = d;
                    break;
                }
            }
            txtBTDeviceDetails.Text = txtBTDeviceDetails.Text + "Connect BT Service started!" + Environment.NewLine;
            if (device != null)
            {
                UpdateBTDeviceDetails(String.Format("Name:{0} Address:{1:C}", device.DeviceName, device.DeviceAddress));

                mClientBluetoothClient.Connect(device.DeviceAddress, BluetoothService.SerialPort);
                Stream peerStream = mClientBluetoothClient.GetStream();

                // Create storage for receiving data
                byte[] buffer = new byte[2000];

                // Read Data
                peerStream.Read(buffer, 0, 50);

                // Convert Data to String
                string data = System.Text.ASCIIEncoding.ASCII.GetString(buffer, 0, 50);
                UpdateBTDeviceDetails("Client -- Receiving data = " + data);

                int i = 0;
                while (allowBTConnection)
                {
                    UpdateBTDeviceDetails("Client -- Sending = " + i.ToString());
                    byte[] dataBuffer = System.Text.ASCIIEncoding.ASCII.GetBytes(i.ToString() + "\r\n");

                    peerStream.Write(dataBuffer, 0, dataBuffer.Length);
                    ++i;
                    if (i > 10)
                    {
                        i = 0;
                        allowBTConnection = false;
                        UpdateBTDeviceDetails("Client -- Close BT Service");
                    }
                    System.Threading.Thread.Sleep(500);
                }
                // Close network stream
                peerStream.Close();
                mClientBluetoothClient.Close();
            }
            Cursor.Current = Cursors.Default;
        }
        #endregion

        #region 藍牙配對設定 (Pair & Connect) (舊方法, 未使用)
        // 一直沒有辦法利用Connect指令跟藍牙裝置連線, 但是InTheHand的Sample Code卻可以
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            #region BT Connect Init
            if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows)
            {
                // Only the full framework supports encoding x-IA5, NETCF and Mono don't.
                comboBoxEncoding.SelectedIndex = 0;
            }
            else comboBoxEncoding.SelectedIndex = 3;
            #endregion

            if (mBluetoothRadio != null && mBluetoothClient != null)
            {
                labelResult.ForeColor = Color.Orange;
                labelResult.Text = "Try to pairing ...";
                UpdateBTDeviceDetails("Pairing for Bluetooth devices...");
                BTDevicePair();
            }
            else MessageBox.Show("Please search for Bluetooth devices or choose connected device from a list of Bluetooth devices", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void BTDevicePair()
        {
            try
            {
                SelectBluetoothDeviceDialog dialog = new SelectBluetoothDeviceDialog(); // 選擇藍牙裝置清單
                dialog.ShowRemembered = true;// 顯示已經記住的藍牙設備  
                dialog.ShowAuthenticated = true;// 顯示認證過的藍牙設備  
                dialog.ShowUnknown = true;// 顯示位置藍牙設備  
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    UpdateBTDeviceDetails("\r\nDevice Name : " + dialog.SelectedDevice.DeviceName.ToString());
                    UpdateBTDeviceDetails("Send Address : " + dialog.SelectedDevice.DeviceAddress.ToString());// 獲取選擇的遠程藍牙地址  
                    UpdateBTDeviceDetails("Authenticated : " + dialog.SelectedDevice.Authenticated);
                    UpdateBTDeviceDetails("Connected : " + dialog.SelectedDevice.Connected);
                    UpdateBTDeviceDetails("Remembered : " + dialog.SelectedDevice.Remembered);

                    UpdateBTDeviceDetails("Bluetooth Pairing...");

                    // if the device is not paired, pair it!
                    // replace DEVICE_PIN here, synchronous method, but fast
                    BluetoothSecurity.PairRequest(dialog.SelectedDevice.DeviceAddress, TextBoxPin.Text.Trim());

                    UpdateBTDeviceDetails("Bluetooth Connecting...");

                    using (BluetoothClient PairClient = new BluetoothClient())
                    {
                        PairClient.Encrypt = CheckBoxEncrypt.Checked;
                        PairClient.Authenticate = CheckBoxAuthenticate.Checked;
                        // set pin of device to connect with
                        if (CheckBoxUsePin.Checked) PairClient.SetPin(TextBoxPin.Text.Trim());

                        System.Text.Encoding mEncoding = System.Text.Encoding.GetEncoding(comboBoxEncoding.Text);

                        // async connection method
                        // PairClient.BeginConnect(dialog.SelectedDevice.DeviceAddress, BluetoothService.Handsfree, new AsyncCallback(Connect), device);
                        PairClient.Connect(dialog.SelectedDevice.DeviceAddress, BluetoothService.Handsfree);

                        UpdateFindDeviceResult("Device connected.");
                        UpdateBTDeviceDetails("Bluetooth device pairing is successful.");
                    }
                }
            }
            catch (SocketException sex)
            {
                UpdateFindDeviceResult("Connect FAILED");
                labelResult.ForeColor = Color.Red;
                UpdateBTDeviceDetails("Connect failed : " + sex.Message + ", Code :  " + sex.SocketErrorCode.ToString("D"));

                checkTestStatus(sex.Message);
            }
            catch (Exception ex)
            {
                UpdateFindDeviceResult("FAIL");
                labelResult.ForeColor = Color.Red;
                UpdateBTDeviceDetails(ex.Message);

                checkTestStatus(ex.Message);
            }
        }

        // Callback
        private void Connect(IAsyncResult result)
        {
            // client is connected now
            if (result.IsCompleted) MessageBox.Show(result.AsyncState.ToString(), "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CheckBoxUsePin_CheckedChanged(object sender, EventArgs e)
        {
            TextBoxPin.Enabled = CheckBoxUsePin.Checked;
        }
        #endregion

        #region 測試區域
        // 中斷藍牙配對
        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            if (mBluetoothClient.Connected)
            {
                mBluetoothClient.Close();
                UpdateBTDeviceDetails("Bluetooth disconnection.");
            }
        }

        private void buttonReceiveFile_Click(object sender, EventArgs e)
        {
            if (!mBackgroundWorker_FindDevice.IsBusy) createObexListener();
            else MessageBox.Show("Bluetooth connecting...Please wait", "Attention");
        }
        private void createObexListener()
        {
            UpdateBTDeviceDetails("Create Bluetooth Obex Listener");
            DisplayBluetoothRadio();
            if (mBluetoothRadio == null)
            {
                UpdateBTDeviceDetails("No support Bluetooth radio/stack found.");
                return;
            }
            else if (mBluetoothRadio.Mode != InTheHand.Net.Bluetooth.RadioMode.Discoverable)
            {
                DialogResult rslt = MessageBox.Show("Make BluetoothRadio Discoverable?", "DeviceListener", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (rslt == DialogResult.Yes) mBluetoothRadio.Mode = InTheHand.Net.Bluetooth.RadioMode.Discoverable;
                else return;
            }

            mObexListener = new ObexListener(ObexTransport.Bluetooth);
            if (!serviceStarted)
            {
                serviceStarted = true;
                mObexListener.Start();
                System.Threading.Thread mThread = new System.Threading.Thread(new System.Threading.ThreadStart(DealWithRequest));
                mThread.IsBackground = true;
                mThread.Start();
                UpdateBTDeviceDetails("Bluetooth Obex Listener started.");
            }
            else
            {
                serviceStarted = false;
                mObexListener.Stop();
                UpdateBTDeviceDetails("Bluetooth Obex Listener stopped.");
            }
        }
        private void DealWithRequest()
        {
            while (mObexListener.IsListening)
            {
                try
                {
                    ObexListenerContext olc = mObexListener.GetContext();
                    ObexListenerRequest olr = olc.Request;
                    string filename = Uri.UnescapeDataString(olr.RawUrl.TrimStart(new char[] { '/' }));
                    // olr.WriteFile(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "\\" + DateTime.Now.ToString("yyMMddHHmmss") + " " + filename);
                    olr.WriteFile(Environment.SpecialFolder.MyDocuments + DateTime.Now.ToString("yyMMddHHmmss") + " " + filename);
                    // olr.WriteFile("\\My Documents\\" + DateTime.Now.ToString("yyMMddHHmmss") + " " + filename);
                    // 匿名方法 (Anonymous Method)
                    // 匿名方法要求參數的是一個委託(delegate)類型, 編譯器在處理匿名方法的時候, 需要指定這個匿名方法將會返回什麼類型的委託, MethodInvoke和Action都是方法返回類型為空的委託
                    BeginInvoke(new MethodInvoker(delegate ()
                    {
                        UpdateBTDeviceDetails("Received file : " + filename + "\r\nFolder Path :" + System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                    }));
                }
                catch (Exception ex)
                {
                    BeginInvoke(new MethodInvoker(delegate () { UpdateBTDeviceDetails(ex.Message); }));
                    continue;
                }
            }
        }
        #endregion

        #region 微軟藍牙檔案傳輸精靈fsquirt
        private void buttonFsquirt_Click(object sender, EventArgs e)
        {
            // 舊有DRS Test用的是微軟內建的藍牙檔案傳輸精靈(fsquirt.exe)
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + "fsquirt");
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.Close();
        }
        #endregion

        #region Update UI
        public delegate void SafeWinFormsThread(string msg);
        private void UpdateBTDeviceDetails(string msg)
        {
            try
            {
                if (txtBTDeviceDetails.InvokeRequired)	// test the UI Control requires invoke or not
                {   // if yes, invoke
                    SafeWinFormsThread deviceDetail = new SafeWinFormsThread(UpdateDeviceDetailsUI);
                    txtBTDeviceDetails.Invoke(deviceDetail, new object[] { msg });
                }
                else// if invoke is not necessary, process it directly.
                {
                    UpdateDeviceDetailsUI(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Bluetooth =  " + ex.Message);
            }
        }
        private void UpdateDeviceDetailsUI(string msg)
        {
            if (IsDebugMode) Trace.WriteLine(msg); // Winmate Kenkun add on 2014/08/07

            txtBTDeviceDetails.Text = txtBTDeviceDetails.Text + msg + Environment.NewLine;
            if (txtBTDeviceDetails.Text.Length > 2000) txtBTDeviceDetails.Text = txtBTDeviceDetails.Text.Substring(1000, 1000);
            txtBTDeviceDetails.SelectionStart = txtBTDeviceDetails.Text.Length;
            txtBTDeviceDetails.ScrollToCaret();
        }

        private void UpdateFindDeviceResult(string msg)
        {
            try
            {
                if (labelResult.InvokeRequired) labelResult.Invoke(new SafeWinFormsThread(UpdateFindDeviceResultUI), new object[] { msg });
                else UpdateFindDeviceResultUI(msg);
            }
            catch (Exception ex) // InvalidOperationException
            {
                Console.WriteLine("Bluetooth =  " + ex.Message);
            }
        }
        private void UpdateFindDeviceResultUI(string msg)
        {
            labelResult.ForeColor = Color.Blue;
            labelResult.Text = msg;
        }

        public void checkTestStatus(String testResult)
        {
            buttonAudioPASS.Enabled = false;
            buttonAudioFAIL.Enabled = false;

            if (testResult.Equals("PASS"))
            {
                result["result"] = true;
            }
            else
            {
                result["result"] = false;
            }

        }

        #endregion
    }
}
