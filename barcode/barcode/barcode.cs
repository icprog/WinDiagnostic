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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace barcode
{
    public partial class barcode : Form
    {
        #region DllImport
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint RegisterWindowMessage(string lpString);
        #endregion

        #region Definition
        private PublicFunctionBarcode mBarcodePublicFunction;
        private const byte BCD = 0x60; // BARCODE DATA
        private uint WM_ISCP_FRAME = RegisterWindowMessage("WM_ISCP_FRAME");
        private uint WM_RAW_DATA = RegisterWindowMessage("WM_RAW_DATA");
        public System.Windows.Forms.Timer TimerBarcode;
        bool IsDebugMode = true;
        static DialogMessageBox newMessageBox;
        static string MessageBoxResult = "None";
        public static string PortAddressBarcode = "COM15";
        public static int BarcodeType = 2;                      // Modify default value for_Golden-Age_UserMode value
        public static int BarcodeReadTime = 3000;               // 送出Trigger command之後要間隔多久自動停止接收資料
        public static bool IsConfirmTagContextBarcode = true;  // 是否要確認Barcode掃描進來的資料? ( 0=不確認, 1=確認 )
        public static bool IsHardwareTrigger = false;           // 是否是用Honeywell (USB) hardware trigger Barcode
        int ThresholdTimeoutScanner = 3;
        static string ConfirmTagContextBarcode = "071589812308";
        bool ShowWindow = false;
        JObject result = new JObject();
        const int Device2AllPowerOnOthers = 0x3F;   // [X]      / [X]       / ExtendPort/ Extend USB/ GPS A / RFID / F Cam / Barcode
        #endregion

        public barcode()
        {
            InitializeComponent();
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

        void CloseWidget()
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                // 啟動測試程式前要先關閉 Hottab 跟 衍生 Widget
                if (clsProcess.ProcessName.Contains("HotTab") || clsProcess.ProcessName.Contains("Hottab") ||
                clsProcess.ProcessName.Contains("ScannerManager") || clsProcess.ProcessName.Contains("RF521Scan") ||
                clsProcess.ProcessName.Contains("EA30Scan") || clsProcess.ProcessName.Contains("UHF_RFID") ||
                clsProcess.ProcessName.Contains("SE4500DLScan") || clsProcess.ProcessName.Contains("Ignition_Control") ||
                clsProcess.ProcessName.Contains("WMCamera") || clsProcess.ProcessName.Contains("WinSet") ||
                clsProcess.ProcessName.Contains("WMControlCenter") || clsProcess.ProcessName.Contains("BatteryInfo")
                )
                {
                    if (IsDebugMode) Trace.WriteLine("Kill 3rd-party process : " + clsProcess.ProcessName.ToString());
                    clsProcess.Kill();
                }
            }
        }

        private void Barcode_Load(object sender, EventArgs e)
        {
            CloseWidget();
            result["result"] = "FAIL";
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Exit();
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            ShowWindow = (bool)jobject.ShowWindow;
            IsHardwareTrigger = (bool)jobject.IsHardwareTrigger;
            BarcodeType = (int)jobject.BarcodeType;
            PortAddressBarcode = jobject.PortAddressBarcode.ToString();
            BarcodeReadTime = (int)jobject.BarcodeReadTime;
            IsConfirmTagContextBarcode = (bool)jobject.IsConfirmTagContextBarcode;
            ConfirmTagContextBarcode = jobject.ConfirmTagContextBarcode.ToString();

            if (ShowWindow)
            {
                this.Opacity = 100;
                this.ShowInTaskbar = true;
            }

            if (IsDebugMode) Trace.WriteLine("Barcode_Load");
            Trace.WriteLine("Open all device power.");
            HotTabDLL.WinIO_SetDevice2State(Device2AllPowerOnOthers); // IB80 Bit6(Battery_Max_Charge) 只會讓電池充到50%
            if (TimerBarcode == null) TimerBarcode = new System.Windows.Forms.Timer();
            TimerBarcode.Tick += new EventHandler(TimerBarcode_Tick);
            // 給予使用者指定秒數讓Barcode進行掃描, 時間到就啟動Timer關閉Barcode
            TimerBarcode.Interval = BarcodeReadTime;

            if (IsHardwareTrigger)
            {
                buttonFAIL.Enabled = true;
                TimerBarcode.Start();
            }
            else
            {
                PublicFunctionBarcode.BarcodeType = Convert.ToUInt16(BarcodeType);
                if (IsDebugMode) Trace.WriteLine("Barcod Port : " + PortAddressBarcode + " , Type : " + PublicFunctionBarcode.BarcodeType);

                PublicFunctionBarcode.IsdcRsApiInit();

                try
                {
                    barcodeConnect();
                }
                catch (Exception ex)
                {
                    checkTestStatus(ex.Message);
                }
            }
        }

        #region Step1 : Conncet to Barcode
        private void buttonBarcodeConnect_Click(object sender, EventArgs e)
        {
            barcodeConnect();
        }

        public static DialogResult ShowDialogMessageBox(string text, string title, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            PublicFunction.CloseSpecificProcess("Attention");

            if (newMessageBox == null)
            {
                newMessageBox = new DialogMessageBox();
            }
            newMessageBox.textBoxMessage.Text = text;
            newMessageBox.Text = title;

            if (buttons.Equals(MessageBoxButtons.OK)) newMessageBox.buttonOK.Visible = true;
            else if (buttons.Equals(MessageBoxButtons.YesNo))
            {
                newMessageBox.buttonYes.Visible = true;
                newMessageBox.buttonNo.Visible = true;
            }

            newMessageBox.ShowDialog();
            MessageBoxResult = newMessageBox.labelResult.Text;
            newMessageBox = null; // newMessageBox.Dispose();
                                  // if (IsDebugMode) Trace.WriteLine("DialogMessage : " + MessageBoxResult + "\t" + (newMessageBox) == null);

            switch (MessageBoxResult)
            {
                case "OK":
                    return DialogResult.OK;
                case "Yes":
                    return DialogResult.Yes;
                case "No":
                    return DialogResult.No;
                default:
                    return DialogResult.OK;
            }
        }

        private void barcodeConnect()
        {
            if (IsDebugMode) Trace.WriteLine("barcodeConnect - Port : " + PortAddressBarcode + " , Type : " + PublicFunctionBarcode.BarcodeType);
            try
            {
                if (!HotTabDLL.CheckComExist(PortAddressBarcode)) // 檢查指定的Comport是否存在(掃描Barcode Comport,先開啟Comport一次後關閉)
                {
                    DialogResult dr = ShowDialogMessageBox(PortAddressBarcode + " can't use or not find.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (dr == DialogResult.OK) checkTestStatus(PortAddressBarcode + " can't use or not find.");
                    return;
                }

                if (mBarcodePublicFunction != null)
                {
                    mBarcodePublicFunction.free(); // Release Barcode佔用的資源
                    mBarcodePublicFunction = null;
                }
                if (mBarcodePublicFunction == null)
                {
                    mBarcodePublicFunction = new PublicFunctionBarcode(PortAddressBarcode, Handle); // 開啟新的COM
                    mBarcodePublicFunction.initinalBarCode();
                }

                if (PublicFunctionBarcode.BarcodeVisible == 0)
                {
                    if (PublicFunctionBarcode.BarcodeType == 2)
                    {
                        Visible = true; // for ISDC_RS use fixed WM_ISCP_FRAME and WM_RAW_DATA can't receive
                        Thread.Sleep(BarcodeReadTime);
                    }
                    Visible = false;
                }
                else
                {
                    Visible = true;
                }

                lblVersionString.Text = mBarcodePublicFunction.GetBarcodeName2();

                buttonFAIL.Enabled = true; // 如果沒有Barcode初始化完就可以直接按FAIL

                buttonScan.Enabled = true;

                if (mBarcodePublicFunction.IsComportReady() && mBarcodePublicFunction.IsSupportBarCode())  // driver usb-comport ready
                {
                    textBoxReceiveTag.Focus();
                }
                else if (mBarcodePublicFunction.IsComportReady())
                {
                    mBarcodePublicFunction.IsForceOpticonMDI3100();
                    textBoxReceiveTag.Focus();
                }
                else
                {
                    ShowDialogMessageBox("Module is instable, please check Device power or relogin Windows", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    checkTestStatus("Module is instable, please check Device power or relogin Windows");
                    return;
                }
                ReadyToScanTag();
            }
            catch (Exception ex)
            {
                checkTestStatus("barcodeConnect() Exception : " + ex.Message);
                ShowDialogMessageBox("barcodeConnect() Exception : " + ex.Message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region Step2 : Barcode Trigger & Scan
        private void buttonScan_Click(object sender, EventArgs e)
        {
            PublicFunction.SingleTriggerCount = 0;
            ReadyToScanTag();
        }

        private void ReadyToScanTag()
        {
            // 當按下Scan的時候, 要將所有Button都鎖起來, 不然發光時直接跳到下一頁時就會有Module恆亮問題
            buttonScan.Enabled = false;
            buttonPASS.Enabled = false;
            buttonFAIL.Enabled = false;

            textBoxReceiveTag.Focus();

            TimerBarcode.Start();

            StartScan();
        }

        public void StartScan()
        {
            if (mBarcodePublicFunction.IsComportReady() && mBarcodePublicFunction.IsSupportBarCode())  // driver usb-comport ready
            {
                // 由於Intermec有把Comport存取的方法包裝在DLL, 所以我們不需要額外控制Conport的開啟. 但是其他Module要下Trigger command前就要先將SerialPort打開
                if ((PublicFunctionBarcode.type != 4) && (PublicFunctionBarcode.type != 5) && (!mBarcodePublicFunction.isOpen()))
                {
                    mBarcodePublicFunction.openPort();
                }

                if (!textBoxReceiveTag.Text.Length.Equals(0))
                {
                    rtxMsgList.AppendText(textBoxReceiveTag.Text + "\r\n");
                    rtxMsgList.SelectionStart = rtxMsgList.Text.Length;
                    rtxMsgList.ScrollToCaret();
                }
                textBoxReceiveTag.Clear();

                mBarcodePublicFunction.BarcodeScan(); // 對Comport寫入Trigger指令
            }
            else
            {
                ShowDialogMessageBox("Module is not available, Please Check the Status of Device.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void TimerBarcode_Tick(object sender, EventArgs e)
        {
            if (IsDebugMode) Trace.WriteLine("Module Scan is Completed. Data : " + textBoxReceiveTag.Text + "\r\n");

            TimerBarcode.Stop();
            if (!IsHardwareTrigger) mBarcodePublicFunction.BarcodeScanStop();
            textBoxReceiveTag.Focus();
            // buttonScan.Enabled = true;

            PublicFunction.SingleTriggerCount++;

            // barcode 掃進來的資料有時候會含\r\n, 會使Equals判斷式不成立
            // 由於產線反映測試的Barcode條碼資料有6種, 驗證機制只針對單一種Barcode, 不符合需求, 故新增一個驗證開關
            if (textBoxReceiveTag.Text.Contains(ConfirmTagContextBarcode) && !textBoxReceiveTag.Text.Length.Equals(0))
            {
                buttonPASS.Enabled = true;
                buttonPASS.PerformClick();
            }
            else
            {
                buttonPASS.Enabled = false;
                if (PublicFunction.SingleTriggerCount < ThresholdTimeoutScanner)
                {
                    if (IsHardwareTrigger)
                    {
                        Thread.Sleep(1000);
                        TimerBarcode.Start();
                    }
                    else
                    {
                        ReadyToScanTag();
                    }
                    // return;
                }
                // else if (PublicFunction.SingleTriggerCount.Equals((TimeoutThresholdAutoMode * PublicFunction.TimeoutThresholdFactor(TimerBarcode.Interval))))
                // {
                //     checkTestStatus("Time out - Unable to scan data");
                // }
                else
                {
                    checkTestStatus("No Barcode Tag can be scanned.");
                }
            }
        }
        #endregion

        #region Step3: Disconnect
        // 已隱藏停用Disconnect按鈕
        private void buttonBarcodeDisconnect_Click(object sender, EventArgs e)
        {
            if (mBarcodePublicFunction != null) barcodeDisconnect();
        }

        private void barcodeDisconnect()
        {
            if (IsDebugMode) Trace.WriteLine("barcodeDisconnect()");
            if (PublicFunctionBarcode.type.Equals(4) || PublicFunctionBarcode.type.Equals(5))
            {
                mBarcodePublicFunction.free();
            }
            else if (mBarcodePublicFunction.isOpen())
            {
                mBarcodePublicFunction.closePort(); // 非Intermec型號專用
            }

            mBarcodePublicFunction = null;
            TimerBarcode = null;

            buttonBarcodeConnect.Enabled = true;
            buttonBarcodeDisconnect.Enabled = false;
            buttonScan.Enabled = false;
        }
        #endregion

        #region Button Event
        private void buttonPASS_Click(object sender, EventArgs e)
        {
            checkTestStatus("PASS");
        }

        private void buttonFAIL_Click(object sender, EventArgs e)
        {
            checkTestStatus("User canceled the test.");
        }

        public void checkTestStatus(String testResult)
        {
            Trace.WriteLine("testresult: " + testResult);
            if (TimerBarcode != null)
                TimerBarcode.Stop();

            textBoxReceiveTag.Enabled = false;
            buttonScan.Enabled = false;
            buttonPASS.Enabled = false;
            buttonFAIL.Enabled = false;

            if (mBarcodePublicFunction != null) barcodeDisconnect();

            if (testResult.Equals("PASS"))
            {
                labelResult.ForeColor = Color.Green;
                labelResult.Text = "PASS";
                result["result"] = "PASS";
                result["EIPLog"] = new JObject
                {
                    { "Barcode", "PASS" },
                    { "Barcode_Info", "PASS"}
                };
            }
            else
            {
                labelResult.ForeColor = Color.Red;
                labelResult.Text = "FAIL";
                result["result"] = "FAIL";
                result["EIPLog"] = new JObject
                {
                    { "Barcode", "FAIL" },
                    { "Barcode_Info", testResult}
                };
            }

            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            if (!ShowWindow)
                Exit();
        }

        private void bnCopyRecMsg_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(textBoxReceiveTag.Text, true);
        }

        private void bnCopyMsgList_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(rtxMsgList.Text, true);
        }
        #endregion

        protected override void WndProc(ref Message m)
        {
            if (m == null) return;

            #region Intermec 用來收 Comport 回傳 Data 的事件（Intermec自己將Comport操作的function直接包成DLL給User用）
            if (m.Msg == PublicFunctionBarcode.WM_ISCP_FRAME)
            {
                switch ((byte)m.WParam)
                {
                    case PublicFunctionBarcode.BCD:
                        PublicFunctionBarcode.GetBarcodeDataIsdcRs();
                        break;
                }
            }
            else if (m.Msg == PublicFunctionBarcode.WM_RAW_DATA) PublicFunctionBarcode.GetRawDataIsdcRs();
            #endregion

            #region Moto 用來收 Comport 回傳 Data 的事件（Moto自己將Comport操作的function直接包成DLL給User用）
            if (m.Msg == 0x8001)
            {
                PublicFunctionBarcode.GetDecodeData();
                SsiDllApi.SetDecodeBuffer(Convert.ToInt32(PortAddressBarcode.Remove(0, 3)), SsiDllApi.DecodeData, SsiDllApi.MAX_LEN);
            }
            else if (m.Msg == 0x8008)
            {
                PublicFunctionBarcode.GetVersionData();
                SsiDllApi.SetDecodeBuffer(Convert.ToInt32(PortAddressBarcode.Remove(0, 3)), SsiDllApi.DecodeData, SsiDllApi.MAX_LEN);
            }
            #endregion

            base.WndProc(ref m);
        }
    }
}
