using barcode;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace camera
{
    public partial class camera : Form
    {
        #region Winmate WinIO DLL Declare

        [DllImport(@"WMIO2.dll")]
        public static extern bool SetFlashOn();

        [DllImport(@"WMIO2.dll")]
        public static extern bool SetFlashOff();

        class RealtekCamera8M
        {
            #region Realtek 8M Camera
            [DllImport("RvcLib.dll")]
            public static extern bool RvcLib_Initialize(bool Log, bool LibType);

            [DllImport("RvcLib.dll")]
            public static extern bool RvcLib_UnInitialize(bool LibType);

            [DllImport("RvcLib.dll")]
            unsafe public static extern bool UVC_GetDevVIDPIDSN(int devInst, ushort* pVID, ushort* pPID, ushort* pRev, string pSN);

            [DllImport("RvcLib.dll")]
            public static extern int UVC_Open(int index);

            [DllImport("RvcLib.dll")]
            public static extern int UVC_Close(int index);

            [DllImport("RvcLib.dll")]
            public static extern int UVC_I2CGenericWrite(int devInst, byte byI2CSlaveAddr, uint uLength, byte[] lpData);
            #endregion
        }

        #endregion

        #region Definition
        HotTabCamera mHotTabCamera;

        int TestCamera = 0;        // zero based index of video capture device to use
        int VIDEOWIDTH = 640;       // Depends on video device caps
        int VIDEOHEIGHT = 480;      // Depends on video device caps
        const int VIDEOBITSPERPIXEL = 24;   // BitsPerPixel values determined by device

        int iCameraAmount = 0;
        int counter = 0;            // 用以切換閃光燈狀態

        static Point CameraResolution; // 用來抓 Camera 最大解析度, Winmate kenkun add on 2015/12/15

        Object thisLock = new Object();

        // 用來控制Snapshot的時候應該要顯示在哪一個PictureBox之中, Winmate kenkun add on 2014/11/24
        int SwitchSnapshotPreview;
        Random RandomGenerator = new Random();
        int MinValue = 0;
        int MaxValue = 2; // 亂數產生Random()方法的最大值要+1 (如:要產生0-3的數值, MaxValue要設成4)

        decoder CaptureDecoder = null;
        string CaptureDecodeData;

        List<String> CameraList = null;

        uint WaitCaptureTimeCount = 1;

        bool IsDebugMode = true;
        string TestProduct = string.Empty;
        bool IsDeveloperMode = false;
        bool IsFM08 = false;     // 分辨FM08(True)與FM10(True), 用在Camera跟FunctionButton的部分功能
        int CameraAmount = 2; // 機器上的相機數量有幾個?
        bool IsAutoModeCamera = true;
        int TimerIntervalCamaraCapture = 5000;
        string ConfirmTagContextBarcode = "071589812308"; // 預設的Barcode掃描資料
        int ThresholdTimeoutAutoMode = 10;
        JObject result = new JObject();
        bool ShowWindow = false;

        bool Is8MCameraUsed
        {
            get
            {
                return TestProduct.Equals("IBWH") || TestProduct.Equals("101P") || TestProduct.Equals("101S");
            }
        }
        #endregion

        public camera()
        {
            InitializeComponent();
        }

        ~camera()
        {
            if (IsDebugMode) Trace.WriteLine("Camera Destructor");
            // Dispose 方法會執行所有物件清除, 所以記憶體回收行程不需要再呼叫物件的 Object.Finalize 覆寫
            GC.SuppressFinalize(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDebugMode) Trace.WriteLine("Dispose(" + this.Name + ") : " + disposing);
            if (disposing && (components != null))
            {
                components.Dispose(); // 釋放 Unmanaged 資源
            }
            base.Dispose(disposing);
        }

        string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(exepath, path);
        }

        void camera_Load(object sender, EventArgs e)
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
            CameraAmount = (int)jobject.CameraAmount;
            IsAutoModeCamera = (bool)jobject.IsAutoModeCamera;
            TimerIntervalCamaraCapture = (int)jobject.TimerIntervalCamaraCapture;
            ConfirmTagContextBarcode = jobject.ConfirmTagContextBarcode.ToString();
            TestProduct = jobject.TestProduct.ToString();
            IsFM08 = (bool)jobject.IsFM08;

            if (ShowWindow)
            {
                this.Opacity = 100;
                this.ShowInTaskbar = true;
            }
            if (IsDebugMode) Trace.WriteLine("Camera_Load, Is8MCameraUsed : " + Is8MCameraUsed);

            TestCamera = 0; // 確認測試過的相機數是初始值的0, Winmate Kenkun modify on 2017/07/07

            if (IsDeveloperMode)
            {
                UpdateDeviceList();
            }
            else if (HotTabCamera.GetCameraCount() <= 0)
            {
                if (IsDebugMode) Trace.WriteLine("Camera Not Found");
                comboBoxCameraDevices.Items.Add("Device Not Found");
                comboBoxCameraDevices.SelectedIndex = 0;
                checkTestStatus("Camera Not Found.");
                return;
            }
            else
            {
                FangtecCameraDLL.ALC_Initialization(0, "");  // About 1.3 Sec

                // 將Camera轉90度, 需搭配廠商提供的Rotation Driver
                if (TestProduct.Equals("FMB8") && !IsFM08) HotTabDLL.FangtecRotate090(); // 由於DLL改版, 只有FM10用, Winmate Kenkun modify on 2015/12/14

                UpdateDeviceList();
            }

            if (IsAutoModeCamera)
            {
                if (IsDebugMode) Trace.WriteLine("Camera - AutoMode");
                groupBoxCapturePreview.Visible = false;
                groupBoxPositionOfCapture.Visible = false;
                CaptureDecoder = new decoder();
            }
        }

        void UpdateDeviceList()
        {
            try
            {
                if (IsDeveloperMode) CameraList = new List<string>(new string[] { "10.1 TPC(vid_064e&pid_998d)", "PC Camera(vid_058f&pid_3841)" });
                else CameraList = HotTabCamera.GetListCamera();
                // 將抓到的Camera做升序排列
                // [WAY 1] LINQ
                // IEnumerable<string> sortAscendingQuery = from camera in CameraList
                //                                          orderby camera //"ascending" is default
                //                                          select camera;
                // CameraList = sortAscendingQuery.ToList();

                // [WAY 2] List<T>.Sort 方法()
                // CameraList.Sort();

                // 常見的三種Camera的升序排列結果:
                // 10.1 TPC(vid_064e & pid_998d)
                // IZONE UVC 5M Camera(vid_2081 & pid_270d)
                // PC Camera(vid_058f&pid_3841)

                comboBoxCameraDevices.Items.Clear();

                if (CameraList.Count > 0) // if (m_devicename != null)
                {
                    for (int i = 0; i < CameraList.Count; i++)
                    {
                        // DAP7會出現一個未知的VDP Source()裝置, 選擇後按下Snap會crash
                        // ID82會出現一個未知的Virtual Viscomsoft Screen Capture
                        if (!CameraList[i].Contains("VDP") && !CameraList[i].Contains("Virtual"))
                        {
                            iCameraAmount++;
                            comboBoxCameraDevices.Items.Add(CameraList[i]);
                            if (IsDebugMode) Trace.WriteLine("Index : " + i + " , Camara Name : " + CameraList[i]);
                        }
                    }

                    // comboBoxCameraDevices.Sorted = true;

                    if (IsDebugMode) Trace.WriteLine("Total Camara : " + CameraList.Count + "(" + CameraAmount + ")");
                    if (CameraAmount.Equals(CameraList.Count))
                    {
                        comboBoxCameraDevices.SelectedIndex = TestCamera;
                        comboBoxCameraDevices.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Front Camera or Rear Camera is blocked.\r\nPlease check module or config file.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        checkTestStatus("Front Camera or Rear Camera is blocked.");
                    }
                }
                else
                {
                    iCameraAmount = 0;
                    if (comboBoxCameraDevices.InvokeRequired) comboBoxCameraDevices.Invoke(new Action(() => { comboBoxCameraDevices.Items.Add("Device Not Found"); }));
                    else comboBoxCameraDevices.Items.Add("Device Not Found");
                    comboBoxCameraDevices.SelectedIndex = 0;
                    comboBoxCameraDevices.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
            }
        }

        void comboBoxCameraDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (iCameraAmount > 0)
            {
                InitializeCameraListDevice(comboBoxCameraDevices.SelectedIndex, pictureBoxLiveview);
                Trace.WriteLine("SelectedIndex : " + comboBoxCameraDevices.SelectedIndex);
            }
        }

        public void InitializeCameraListDevice(int iDeviceNum, PictureBox box)
        {
            lock (thisLock)
            {
                if (IsDebugMode) Trace.WriteLine("Init No." + iDeviceNum + " Camara : " + CameraList[iDeviceNum] + " by Thread " + Thread.CurrentThread.ManagedThreadId + " on " + System.DateTime.Now);

                groupBoxTestButton.Enabled = false;
                try
                {
                    if (mHotTabCamera != null)
                    {
                        mHotTabCamera.Dispose();
                        mHotTabCamera = null;
                    }

                    this.Invoke((MethodInvoker)delegate ()
                    {
                        if (IsDebugMode) Trace.WriteLine("New HotTabCamera No." + iDeviceNum + " by Thread " + Thread.CurrentThread.ManagedThreadId + " on " + System.DateTime.Now);
                        mHotTabCamera = new HotTabCamera(iDeviceNum, VIDEOWIDTH, VIDEOHEIGHT, VIDEOBITSPERPIXEL, box);
                    });

                    CameraResolution = HotTabCamera.CheckMaxResolution(iDeviceNum);
                    // 2M Camera : X = 1600, Y = 1200
                    // 5M Camera : X = 2592, Y = 1944
                    // 6M Camera : X = 3264, Y = 1836
                    if (IsDebugMode) Trace.WriteLine("CameraResolution, X : " + CameraResolution.X + " , Y : " + CameraResolution.Y);

                    // 1. 只有單個相機, 沒有閃光燈
                    // 2. 有前後相機, 後相機有閃光燈
                    // 像素不到500萬, 一定是前鏡頭
                    if ((CameraResolution.X < 1920) && (CameraResolution.Y < 1440))
                    {
                        labelTestCameraName.Text = "Front Camera";
                        buttonFlash.Visible = false;
                        buttonFlash.Enabled = false;
                        buttonCapture.Enabled = true;
                    }
                    else // ID82, IB10X, DAP7, M9020B, ID8HH, ID90 只有單一相機, 有閃光燈
                    {
                        labelTestCameraName.Text = "Rear Camera";
                        buttonFlash.Visible = true;
                        buttonFlash.Enabled = true;
                        buttonCapture.Enabled = false;

                        mHotTabCamera.Flash(0);
                        if (Is8MCameraUsed) Flash_8M(0);
                        else SetFlashOff();
                    }

                    if (IsAutoModeCamera)
                    {
                        buttonFlash.Visible = true;
                        buttonFlash.Enabled = false;
                        buttonCapture.Visible = true;
                        buttonCapture.Enabled = false;
                        TimerCamaraCapture.Interval = TimerIntervalCamaraCapture;
                        TimerCamaraCapture.Start();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionEvent(ex);
                }
                groupBoxTestButton.Enabled = true;
            }
        }

        void buttonFlash_Click(object sender, EventArgs e)
        {
            Flash();
        }

        void Flash()
        {
            try
            {
                if (mHotTabCamera == null) return;
                counter++;
                if (counter % 2 == 1)
                {
                    buttonFlash.Text = "ON";
                    mHotTabCamera.Flash(1);
                    if (Is8MCameraUsed) Flash_8M(1);
                    else SetFlashOn();
                    if (!IsAutoModeCamera) buttonCapture.Enabled = false; // 不關閃光就不能Capture
                }
                else
                {
                    buttonFlash.Text = "OFF";
                    mHotTabCamera.Flash(0);
                    if (Is8MCameraUsed) Flash_8M(0);
                    else SetFlashOff();
                    if (!IsAutoModeCamera) buttonCapture.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionEvent(ex);
            }
        }

        public int dwDevInst = 0;
        public void Flash_8M(byte on) // 1:on, 0:off
        {
            if (IsDebugMode) Trace.WriteLine("8M Camera Flash() : " + on);

            RealtekCamera8M.RvcLib_Initialize(false, true);
            dwDevInst = RealtekCamera8M.UVC_Open(0);

            byte[] lpdata;
            if (on == 0)
            {
                lpdata = new byte[] { 0x01, 0x04 };
                RealtekCamera8M.UVC_I2CGenericWrite(dwDevInst, 0x66, 2, lpdata);
            }
            else
            {
                lpdata = new byte[] { 0x01, 0xa4 };
                RealtekCamera8M.UVC_I2CGenericWrite(dwDevInst, 0x66, 2, lpdata);
            }

            RealtekCamera8M.UVC_Close(0);
            RealtekCamera8M.RvcLib_UnInitialize(true);
        }

        void buttonCapture_Click(object sender, EventArgs e)
        {
            CaptureClick();
        }

        void CaptureClick()
        {
            try
            {
                if (IsAutoModeCamera)
                {
                    TimerCamaraCapture.Stop();
                }
                else if (buttonFlash.Enabled)
                {
                    buttonFlash.Enabled = false;
                    buttonFlash.Visible = false;
                }

                // 一個image不能同時被兩個PictureBox使用。切換的時候，先把先前使用的PictureBox.Image = null
                pictureBoxPreview.Image = null; // pictureBoxPreview.Image.Dispose();
                pictureBoxPreview2.Image = null; // pictureBoxPreview2.Image.Dispose();

                if (mHotTabCamera == null)
                {
                    checkTestStatus("HotTab Camera is NULL!");
                    return;
                }
                IntPtr mIntPtr = IntPtr.Zero;
                Cursor.Current = Cursors.WaitCursor;

                // Release any previous buffer
                if (mIntPtr != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(mIntPtr);
                    mIntPtr = IntPtr.Zero;
                }

                // 亂數產生Snapshot所要使用的初始PictureBox應該要是哪一個
                SwitchSnapshotPreview = RandomGenerator.Next(MinValue, MaxValue);

                // Capture Image
                mIntPtr = mHotTabCamera.Click();
                // Stride : 指定一條掃描線 (Scan Line) 的開頭和下一條之間的位元組位移 (Offset).  這通常是像素格式的位元組數目 (例如, 16 位元/像素為 2) 乘以點陣圖寬度. 必須是四的倍數
                Bitmap CapturePreview = new Bitmap(mHotTabCamera.Width, mHotTabCamera.Height, mHotTabCamera.Stride, PixelFormat.Format24bppRgb, mIntPtr);

                // If the image is upsidedown 
                CapturePreview.RotateFlip(RotateFlipType.RotateNoneFlipY);

                // Snapshot截圖會亂數顯示在兩個Preview的PictureBox之間
                if (SwitchSnapshotPreview % 2 == 0)
                {
                    pictureBoxPreview.Image = CapturePreview;
                    Clipboard.SetDataObject(pictureBoxPreview.Image);
                }
                else
                {
                    pictureBoxPreview2.Image = CapturePreview;
                    Clipboard.SetDataObject(pictureBoxPreview2.Image);
                }

                Clipboard.GetImage().Save("Preview.bmp", ImageFormat.Bmp);

                if (IsAutoModeCamera)
                {

                    buttonPASS.Enabled = false;
                    buttonFAIL.Enabled = false;

                    CaptureDecodeData = CaptureDecoder.decode(CapturePreview);

                    if (IsDebugMode)
                    {
                        Trace.WriteLine("ConfirmTagContextBarcode : " + ConfirmTagContextBarcode);
                        Trace.WriteLine("Capture Barcode Decode   : " + CaptureDecodeData);
                    }

                    if (buttonFlash.Text.Equals("ON") && (CaptureDecodeData.Contains(ConfirmTagContextBarcode.Substring(0, ConfirmTagContextBarcode.Length - 3))))
                    {
                        checkTestStatus("Capture can be decoded in FLASH ON!");
                    }
                    else if (buttonFlash.Text.Equals("ON"))
                    {
                        TimerCamaraCapture.Start();
                    }
                    else if (CaptureDecodeData.Contains(ConfirmTagContextBarcode.Substring(0, ConfirmTagContextBarcode.Length - 3)))
                    {
                        TestNextCamera();
                    }
                    else
                    {
                        checkTestStatus("Capture Barcode Decode FAIL!");
                    }
                }

                Cursor.Current = Cursors.Default;

                // Release any previous buffer
                if (mIntPtr != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(mIntPtr);
                    mIntPtr = IntPtr.Zero;
                }

                // 手動測項防呆機制(不讓產線沒有測試就直接按PASS/FAIL)
                buttonPreviewTop.Enabled = true;
                buttonPreviewBottom.Enabled = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                ExceptionEvent(ex);
            }
        }

        // 手動測項防呆機制(不讓產線沒有測試就直接按PASS)
        void buttonPreviewSelect_Click(object sender, EventArgs e)
        {
            buttonPreviewTop.Enabled = false;
            buttonPreviewBottom.Enabled = false;

            if (IsDebugMode)
            {
                Trace.WriteLine("Click : " + ((Button)(sender)).Text);
                Trace.WriteLine("Index : " + ((Button)(sender)).TabIndex.ToString());
            }

            if ((((Button)(sender)).Text.Equals("Top") && SwitchSnapshotPreview % 2 == 0) ||
                (((Button)(sender)).Text.Equals("Bottom") && SwitchSnapshotPreview % 2 != 0))
            {
                // 強制前後鏡頭都要測試到
                TestNextCamera();
            }
            else
            {
                checkTestStatus("Select capture Fail");
            }
        }

        void TestNextCamera()
        {
            if ((iCameraAmount > 1) && (!TestCamera.Equals(iCameraAmount - 1)))
            {
                pictureBoxLiveview.Image = null; // pictureBoxLiveview.Image.Dispose(); // Dispose()會造成NullReferenceException 

                TestCamera++;
                InitializeCameraListDevice(TestCamera, pictureBoxLiveview);
            }
            else
            {
                checkTestStatus("PASS");
            }
        }

        void buttonCameraFAIL_Click(object sender, EventArgs e)
        {
            checkTestStatus("Module error");
        }

        void checkTestStatus(String testResult)
        {
            if (IsAutoModeCamera) TimerCamaraCapture.Stop();

            if (HotTabCamera.GetCameraCount() > 0)
            {
                if (mHotTabCamera != null) mHotTabCamera.Flash(0);
                if (Is8MCameraUsed) Flash_8M(0);
                else SetFlashOff();
            }

            labelTestCameraName.Text = "None";

            pictureBoxPreview.Image = null;
            pictureBoxPreview2.Image = null;

            buttonFAIL.Enabled = false;

            buttonFlash.Enabled = false;
            buttonCapture.Enabled = false;
            buttonPreviewTop.Enabled = false;
            buttonPreviewBottom.Enabled = false;

            FreeResources();

            if (testResult.Equals("PASS"))
            {
                labelResult.ForeColor = Color.Green;
                labelResult.Text = "PASS";
                result["result"] = true;
            }
            else
            {
                labelResult.ForeColor = Color.Red;
                labelResult.Text = "FAIL";
                result["result"] = false;
            }

            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            if (!ShowWindow)
                Environment.Exit(0);
        }

        void FreeResources()
        {
            string PreviewImage = System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + "Preview.jpg";
            if (File.Exists(PreviewImage)) File.Delete(PreviewImage);

#if IsInstallHotTab
            if (mHotTabCamera != null)
            {
                mHotTabCamera.Flash(0);
                SetFlashOff();

                mHotTabCamera.Dispose();
                mHotTabCamera = null;
            }
#endif
        }

        void ExceptionEvent(Exception ex)
        {
            FreeResources();
            if (IsDebugMode) Trace.WriteLine(ex.Message);
            checkTestStatus(ex.Message);
        }

        void TimerCamaraCapture_Tick(object sender, EventArgs e)
        {
            // if (IsDebugMode) Trace.WriteLine("WaitCount : " + WaitCaptureTimeCount + "\tThreshold : " + WaitCaptureTime);

            if (labelTestCameraName.Text.Equals("Rear Camera"))
            {
                labelTestCameraName.Text = "Flash On";
                labelTestCameraName.Update();
                Trace.WriteLine("Flash On");
                Flash();
                WaitCaptureTimeCount = 0;
            }
            else if (labelTestCameraName.Text.Equals("Flash On"))
            {
                labelTestCameraName.Text = "Capture (On)";
                labelTestCameraName.Update();
                Trace.WriteLine("Capture in Flash On");
                CaptureClick();
                WaitCaptureTimeCount = 0;
            }
            else if (labelTestCameraName.Text.Equals("Capture (On)"))
            {
                labelTestCameraName.Text = "Flash Off";
                labelTestCameraName.Update();
                Trace.WriteLine("Flash Off");
                Flash();
                WaitCaptureTimeCount = 0;
            }
            else if (labelTestCameraName.Text.Equals("Flash Off"))
            {
                labelTestCameraName.Text = "Capture (Off)";
                labelTestCameraName.Update();
                Trace.WriteLine("Capture in Flash Off");
                CaptureClick();
                WaitCaptureTimeCount = 0;
            }
            else if (labelTestCameraName.Text.Equals("Front Camera"))
            {
                Trace.WriteLine("Capture with Front Camera");
                CaptureClick();
                WaitCaptureTimeCount = 0;
            }
            else if (WaitCaptureTimeCount.Equals(ThresholdTimeoutAutoMode * TimeoutThresholdFactor(TimerCamaraCapture.Interval)))
            {
                checkTestStatus("Time out - Camera unknown error!");
            }

            WaitCaptureTimeCount++;
        }

        int TimeoutThresholdFactor(int TimerInterval)
        {
            return ((1000 / TimerInterval) > 0) ? (1000 / TimerInterval) : 1;
        }
    }
}
