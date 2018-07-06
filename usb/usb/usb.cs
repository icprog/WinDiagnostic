using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace usb
{
    public partial class usb : Form
    {
        bool IsDebugMode = true; // 是否開啟 Trace.WriteLine()顯示
        bool ShowWindow = false;
        string[] USBFlash = new string[255];
        string SourceFileName = "";
        int USBDevice = 1;    // 有幾個USB插槽需要測試
        int SDDevice = 1;     // 0代表沒有SD裝置, 1代表有（會切換SD Card GroupBox頁面顯示與否）
        int EnableUsbTransferTest = 0; // 是否要啟用檔案傳輸測試功能, 0為關閉, 1為開啟
        int IsRemovableTransferred = 0;
        int ExistingRemovableDevice = 0; // 用來計算測試時機器上有多少已偵測到的USB裝置跟SD卡
        int ExistingSDCard = 0; // 用來計算測試時機器上有多少已偵測到的SD卡
        private readonly ManagementScope ScopeCIMV2 = new ManagementScope("\\\\.\\ROOT\\cimv2");
        private readonly ObjectQuery queryPnPEntity = new ObjectQuery("SELECT * FROM Win32_PnPEntity");
        JObject result = new JObject();

        public usb()
        {
            //this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            result["result"] = false;
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Application.Exit();
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            USBDevice = (int)jobject.USBDevice;
            SDDevice = (int)jobject.SDDevice;
            EnableUsbTransferTest = (int)jobject.EnableUsbTransferTest;
            ShowWindow = (bool)jobject.ShowWindow;
            InitializeComponent();
        }

        private void usb_Load(object sender, EventArgs e)
        {
            if (ShowWindow)
                this.WindowState = System.Windows.Forms.FormWindowState.Normal;

            if (IsDebugMode) Trace.WriteLine("USB_Load");

            if (SDDevice.Equals(1)) groupSDCard.Visible = true;
            MassStorageDeviceTest();
            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            if (!ShowWindow)
                Application.Exit();
        }
        private bool HasMemoryCardSlot()
        {
            return SDDevice.Equals(1) ? true : false;
        }
        private void MassStorageDeviceTest()
        {
            try
            {
                IsRemovableTransferred = 0;
                if (USBFlashTest())
                {
                    UpdateUSBDeviceDetails("Test is completed successfully. " + System.DateTime.Now);
                }
                else
                {
                    UpdateUSBDeviceDetails("Failure to complete Test. " + System.DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                checkTestStatus(ex.Message);
            }
        }
        public int RemovableDeviceTest(String path)
        {
            String str = "WinmateUSBTest";
            String sLine = "";
            FileStream fs = null;

            fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.WriteLine(str);
            sw.Close();
            fs.Close();

            fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            while ((sLine = sr.ReadLine()) != null)
            {
                if (sLine.Equals(str))
                {
                    Trace.WriteLine("File transfer success.");
                    IsRemovableTransferred++;
                }
                else
                {
                    Trace.WriteLine("An error occured during file transfer.");
                }
            }
            sr.Close();
            fs.Close();

            if (File.Exists(SourceFileName)) File.Delete(SourceFileName);
            return IsRemovableTransferred;
        }
        private Boolean USBFlashTest()
        {
            txtUSBDetails.Clear(); // txtUSBDetails.Text = String.Empty;
            UpdateUSBDeviceDetails("Mass Storage Device Test Start.");
            if (IsDebugMode) UpdateUSBDeviceDetails(string.Empty.PadRight(40, '=') + Environment.NewLine);

            ExistingRemovableDevice = 0;
            ExistingSDCard = 0;

            #region 用ManagementObject 去找 USB / SD 相關資訊    
            ManagementObjectSearcher HardwareSearcher = new ManagementObjectSearcher(ScopeCIMV2, queryPnPEntity);
            ManagementObjectCollection HardwareCollection = HardwareSearcher.Get();
            foreach (ManagementObject eachDevice in HardwareCollection)
            {
                if (eachDevice["Name"] != null)
                {
                    // 公司內建的讀卡機在DeviceID都是以GENERIC開頭
                    if (eachDevice["DeviceID"].ToString().Contains("DISK") && eachDevice["DeviceID"].ToString().Contains("GENERIC") && eachDevice["Description"].ToString().Contains("Disk drive"))
                    {
                        txtUSBDetails.Text = txtUSBDetails.Text + "SD  Name : " + eachDevice["Name"].ToString() + Environment.NewLine;
                        if (IsDebugMode) Trace.WriteLine("SD  Name : " + eachDevice["Name"].ToString().PadRight(35, ' ') + "\t Status : " + eachDevice["Status"] + "\t DeviceID : " + eachDevice["DeviceID"] + Environment.NewLine);
                        ExistingSDCard++;
                    }
                    // USB 隨身碟 (需要過濾Portable Device Enumerator (WPDBusEnum))
                    else if (eachDevice["Name"].ToString().Contains("USB") && eachDevice["PNPDeviceID"].ToString().Contains("USBSTOR") && !eachDevice["PNPDeviceID"].ToString().Contains("SWD\\WPDBUSENUM"))
                    {
                        txtUSBDetails.Text = txtUSBDetails.Text + "USB Name : " + eachDevice["Name"].ToString() + Environment.NewLine;
                        if (IsDebugMode) Trace.WriteLine("USB Name : " + eachDevice["Name"].ToString().PadRight(35, ' ') + "\t Status : " + eachDevice["Status"] + "\t DeviceID : " + eachDevice["DeviceID"] + Environment.NewLine);
                        ExistingRemovableDevice++;
                    }
                }
            }
            if (ExistingSDCard.Equals(0) && ExistingRemovableDevice.Equals(0)) UpdateUSBDeviceDetails("Can't Get List of connected USB Devices.");
            #endregion

            if (IsDebugMode) UpdateUSBDeviceDetails(Environment.NewLine + string.Empty.PadRight(40, '=') + Environment.NewLine);

            #region 利用foreach迴圈取得磁碟類型常數, 包括 CDRom、Fixed、Network、NoRootDirectory、Ram、Removable 和 Unknown
            var LogicalDrives = DriveInfo.GetDrives();
            foreach (var mLogicalDrive in LogicalDrives)
            { // IsReady 指出磁碟是否就緒. 例如, 指出光碟機中是否有光碟片, 或是抽取式儲存裝置是否已就緒, 可進行讀取/寫入作業. 
                if (mLogicalDrive.IsReady == true)
                {
                    if (IsDebugMode)
                    {
                        USBFlash[ExistingRemovableDevice] = mLogicalDrive.ToString();
                        txtUSBDetails.Text = Environment.NewLine + txtUSBDetails.Text + "Drive : " + mLogicalDrive.ToString() + Environment.NewLine;
                        txtUSBDetails.Text = txtUSBDetails.Text + "Drive Type : " + mLogicalDrive.DriveType + Environment.NewLine;
                        txtUSBDetails.Text = txtUSBDetails.Text + "Volume label : " + mLogicalDrive.VolumeLabel + Environment.NewLine;
                        txtUSBDetails.Text = txtUSBDetails.Text + "File system : " + mLogicalDrive.DriveFormat + Environment.NewLine;
                        // txtUSBDetails.Text = txtUSBDetails.Text + "File system : " + mLogicalDrive.DriveFormat + Environment.NewLine;
                        // txtUSBDetails.Text = txtUSBDetails.Text + "Total available space : " + FormatBytesToHumanReadable(mLogicalDrive.AvailableFreeSpace) + Environment.NewLine;
                        txtUSBDetails.Text = txtUSBDetails.Text + "Total size of drive : " + FormatBytesToHumanReadable(mLogicalDrive.TotalSize) + Environment.NewLine;
                        if (IsDebugMode) UpdateUSBDeviceDetails(Environment.NewLine + string.Empty.PadRight(40, '=') + Environment.NewLine);
                    }
                }
                else txtUSBDetails.Text = txtUSBDetails.Text + "The device is not ready." + Environment.NewLine;
            }
            #endregion

            labelExistingRemovableDevice.Text = ExistingRemovableDevice.ToString();
            labelExistingSDCard.Text = ExistingSDCard.ToString();

            if ((ExistingRemovableDevice + ExistingSDCard) >= (USBDevice + SDDevice)) // Handheld主機板基本上都有兩個USB孔, 要確保每個USB孔上面都有插入USB裝置. 如果有SD卡, 那需要偵測的Removable裝置多一個. 
            {
                UpdateUSBDeviceDetails("Device : " + (ExistingRemovableDevice + ExistingSDCard) + " >= (USB : " + USBDevice + " + SD : " + SDDevice + ")");

                if (HasMemoryCardSlot() && ExistingSDCard.Equals(0))
                {
                    return CanNotFindDevicesForAllSlots();
                }
                else if (EnableUsbTransferTest.Equals(1))
                {
                    #region 在Removable裝置的根目錄, 新增一個測試檔案
                    for (int k = 1; k <= ExistingRemovableDevice; k++)
                    {
                        SourceFileName = USBFlash[k] + "WinmateUSBTest.txt"; // 在裝置的根目錄, 新增一個測試檔案
                        UpdateUSBDeviceDetails("Path to USB test file : " + SourceFileName);
                        try
                        {
                            IsRemovableTransferred = RemovableDeviceTest(SourceFileName);
                            UpdateUSBDeviceDetails("IsRemovableTransferred : " + IsRemovableTransferred);
                        }
                        catch (Exception ex)
                        {
                            if (IsDebugMode) UpdateUSBDeviceDetails(ex.Message);
                            checkTestStatus("FAIL");
                            return false;
                        }
                    }
                    //IsRemovableTransferred = 0; // 回復初始值, 不然Retry的時候會出現數值沒有清除直接累加的錯誤
                    if (IsRemovableTransferred.Equals(ExistingRemovableDevice)) checkTestStatus("PASS");
                    else
                    {
                        checkTestStatus("FAIL");
                        return false;
                    }
                    #endregion
                }
                else checkTestStatus("PASS");
                return true;
            }
            else if ((ExistingRemovableDevice + ExistingSDCard) < 1) // 完全沒有偵測到任何裝置
            {
                UpdateUSBDeviceDetails("ExistingRemovableDevice : " + ExistingRemovableDevice + " < (USB : " + USBDevice + " + SD : " + SDDevice + ")");
                checkTestStatus("Can't Find Devices.");
                labelUSBResult.Text = "Can't Find Devices.";
                if (HasMemoryCardSlot() && ExistingSDCard.Equals(0)) labelSDResult.Text = "Can't Find Devices.";
                else if (HasMemoryCardSlot() && ExistingSDCard.Equals(1)) labelSDResult.Text = "OK.";
                UpdateUSBDeviceDetails("Can't Find Devices.");
                return false;
            }
            else // 沒有插滿裝置要請測試重新插滿再測試
            {
                return CanNotFindDevicesForAllSlots();
            }
        }
        private bool CanNotFindDevicesForAllSlots()
        {
            checkTestStatus("Plug devices into all slots");
            if (ExistingRemovableDevice.Equals(USBDevice)) labelUSBResult.Text = "OK.";
            else labelUSBResult.Text = "Plug device into USB slot.";
            if (HasMemoryCardSlot() && ExistingSDCard.Equals(0)) labelSDResult.Text = "Plug device into SD slot.";
            else if (HasMemoryCardSlot() && ExistingSDCard.Equals(0)) labelSDResult.Text = "OK.";
            UpdateUSBDeviceDetails("Please plug device into all SD / USB slot.");
            return false;
        }
        private void checkTestStatus(String testResult)
        {
            if (testResult.Equals("PASS"))
            {
                labelUSBResult.Font = new Font("Arial", 18);
                labelUSBResult.ForeColor = Color.Green;
                labelUSBResult.Text = "OK";
                if (HasMemoryCardSlot())
                {
                    labelSDResult.Font = new Font("Arial", 18);
                    labelSDResult.ForeColor = Color.Green;
                    labelSDResult.Text = "OK";
                }
                labelResult.Text = "PASS";
                labelResult.ForeColor = Color.Green;
                result["result"] = true;
                Trace.WriteLine(testResult);
            }
            else
            {
                labelUSBResult.Font = new Font("Arial", 18);
                labelUSBResult.ForeColor = Color.Red;
                if (HasMemoryCardSlot())
                {
                    labelSDResult.Font = new Font("Arial", 18);
                    labelSDResult.ForeColor = Color.Red;
                }
                labelResult.Text = "FAIL";
                labelResult.ForeColor = Color.Red;
                result["result"] = false;
                Trace.WriteLine(testResult);
            }
        }
        public string FormatBytesToHumanReadable(long bytes)
        {
            if (bytes > 1073741824) // more than 1 GB
                return Math.Ceiling(bytes / 1073741824M).ToString("#,### GB");
            else if (bytes > 1048576) // more than 1 MB
                return Math.Ceiling(bytes / 1048576M).ToString("#,### MB");
            else if (bytes >= 1) // more than 1 KB
                return Math.Ceiling(bytes / 1024M).ToString("#,### KB");
            else if (bytes < 0)
                return "";
            else
                return bytes.ToString("#,### B");
        }

        string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(exepath, path);
        }

        #region Update UI
        public delegate void SafeWinFormsThreadDelegate(string msg);
        private void UpdateUSBDeviceDetails(string msg)
        {
            if (txtUSBDetails.InvokeRequired) txtUSBDetails.Invoke(new SafeWinFormsThreadDelegate(UpdateDeviceDetailsUI), new object[] { msg });
            else UpdateDeviceDetailsUI(msg);
        }
        private void UpdateDeviceDetailsUI(string msg)
        {
            txtUSBDetails.Text = txtUSBDetails.Text + msg + Environment.NewLine;
            if (txtUSBDetails.Text.Length > 5000) txtUSBDetails.Text = txtUSBDetails.Text.Substring(2500, 2500);
            txtUSBDetails.SelectionStart = txtUSBDetails.Text.Length;
            txtUSBDetails.ScrollToCaret();
        }
        #endregion
        #region 偵測 USB 插拔狀態(ManagementBaseObject)
        public delegate void UpdateTextBoxDelegate(string data);
        UpdateTextBoxDelegate updateDelegate = null;

        private void USBWatcher_EventArrived(object sender, System.Management.EventArrivedEventArgs e)
        {
            try
            {
                ManagementBaseObject newEvent = e.NewEvent, newEventTarget = (newEvent["TargetInstance"] as ManagementBaseObject);
                if (newEventTarget["InterfaceType"].ToString() == "USB")
                {
                    switch (newEvent.ClassPath.ClassName)
                    {
                        case "__InstanceCreationEvent":
                            BeginInvoke(updateDelegate, new object[] { Convert.ToString(newEventTarget["Caption"]) + " has been plugged in." });
                            Console.WriteLine(Convert.ToString(newEventTarget["Caption"]) + " has been plugged in.");
                            break;
                        case "__InstanceDeletionEvent":
                            BeginInvoke(updateDelegate, new object[] { Convert.ToString(newEventTarget["Caption"]) + " has been plugged out." });
                            Console.WriteLine(Convert.ToString(newEventTarget["Caption"]) + " has been plugged out.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdateUSBStatus(string data)
        {
            txtUSBDetails.Text = txtUSBDetails.Text + data + Environment.NewLine;
            txtUSBDetails.SelectionStart = txtUSBDetails.Text.Length;
            txtUSBDetails.ScrollToCaret();
        }
        #endregion
    }
}
