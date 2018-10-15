/****************************************************************************
While the underlying libraries are covered by LGPL, this sample is released 
as public domain.  It is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  
*****************************************************************************/

// HIRES Webcam  (Thomas Naiser, 2009) demonstrates how a high resolution Webcam  
// like the Quickcam Pro 9000 with a resolution of 1600x1200 pixel can be controlled from a C# 
// application (via Directshow). For application with other Webcams than the Quickcam Pro 9000 minor 
// changes will be necessary.

// The code of HIRES Webcam is based on CameraTest from Mark Deraeve
// (which can be downloaded on http://www.dailycode.net/blog/page/Downloads.aspx)



using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using DirectShowLib;
using System.Text;

namespace HotTabFunction
{
    enum CameraDeviceIdList : int
    {
        Normal = 0,
        AG3820A11_S1_3ND,
        AG3820A11_S1_3ND_V_1_0_RT,
        AG3820A11_S1_3ND_V_1_0_RT_RIGHT,
        AG3820A11_S1_3ND_V_1_1_RT,
        AG3820A11_S1_3ND_V_1_1_RT_RIGHT,
        AV3850A22_SB_4B0,
        AV5050A22_V_1_0,
        DAP7_5M,
        DAP7_2084_0405,
        AH4220C2_S2_2Z7_V_1_1
    }

    /// <summary> Summary description for MainForm. </summary>
    public class HotTabCamera : ISampleGrabberCB, IDisposable
   // internal class HotTabCamera : ISampleGrabberCB, IDisposable
    {
        #region Member variables

        /// <summary> graph builder interface. </summary>
        private IFilterGraph2 m_FilterGraph = null;
     
        // Used to snap picture on Still pin
        private IAMVideoControl m_VidControl = null;
        private IPin m_pinStill = null;

        private IAMCameraControl m_CamControl = null;

        /// <summary> so we can wait for the async job to finish </summary>
        private ManualResetEvent m_PictureReady = null;

        private bool m_WantOne = false;

        /// <summary> Dimensions of the image, calculated once in constructor for perf. </summary>
        private int m_videoWidth;
        private int m_videoHeight;
        private int m_stride;
        private static Point m_pp;

        /// <summary> buffer for bitmap data.  Always release by caller</summary>
        private IntPtr m_ipBuffer = IntPtr.Zero;

        public int iDeviceId;// winmate brian add

#if DEBUG
        // Allow you to "Connect to remote graph" from GraphEdit
        DsROTEntry m_rot = null;
#endif
        #endregion

        #region APIs
        [DllImport("Kernel32.dll", EntryPoint="RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] int Length);
        #endregion

        private static int CameraDeviceIdCompare(int iDeviceNum, string szDevicePatch)
        {
            int iRet = (int)CameraDeviceIdList.Normal;
            int iRetTmp = 0;
            bool bRet = false;
            string szStringTmp = "";
            string sFirstVersion = "";
            string sSecondVersion = "";
            int iDate = 0;

            m_pp = CheckMaxResolution(iDeviceNum);

            // GlobalVariable.DebugMessage("winmate", "max = " + m_pp.X + "x" + m_pp.Y, GlobalVariable.bDebug);

            if (szDevicePatch.IndexOf("vid_058f&pid_3821") != -1)// 2M
            {
                iRetTmp = FangtecCameraDLL.ALC_Initialization(iDeviceNum, szDevicePatch);
                bRet = FangtecCameraDLL.ALC_SetDeviceIdx(iDeviceNum);

                bRet = FangtecCameraDLL.ALC_GetSensorSettingVersion(out szStringTmp);

                // GlobalVariable.DebugMessage("winmate", "ALC_GetSensorSettingVersion = " + szStringTmp, GlobalVariable.bDebug);// brian add

                if (szStringTmp != "")
                {
                    sFirstVersion = szStringTmp.Substring(0, 2);
                    sSecondVersion = szStringTmp.Substring(2, 6);

                    if (sFirstVersion == "6L")
                    {
                        iDate = Convert.ToInt32(sSecondVersion);
                        if (iDate >= 121120)
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_1_RT;
                        else
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND;

                    }
                    else if (sFirstVersion == "6R")
                    {
                        iDate = Convert.ToInt32(sSecondVersion);
                        if (iDate >= 121120)
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT;
                        else
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND;
                    }
                }

                // bRet = FangtecCameraDLL.ALC_UnInitialization();
            }
            else if ((szDevicePatch.IndexOf("vid_058f&pid_5650") != -1) && (m_pp.X == 2592) && (m_pp.Y == 1944))// 5M
            {
                iRetTmp = FangtecCameraDLL.ALC_Initialization(iDeviceNum, szDevicePatch);
                bRet = FangtecCameraDLL.ALC_SetDeviceIdx(iDeviceNum);

                bRet = FangtecCameraDLL.ALC_GetSensorSettingVersion(out szStringTmp);

                // GlobalVariable.DebugMessage("winmate", "ALC_GetSensorSettingVersion = " + szStringTmp, GlobalVariable.bDebug);// brian add

                iRet = (int)CameraDeviceIdList.AV3850A22_SB_4B0;

                // bRet = FangtecCameraDLL.ALC_UnInitialization();
            }
            else if (szDevicePatch.IndexOf("vid_058f&pid_5650") != -1)// 2M
            {
                iRetTmp = FangtecCameraDLL.ALC_Initialization(iDeviceNum, szDevicePatch);
                bRet = FangtecCameraDLL.ALC_SetDeviceIdx(iDeviceNum);

                bRet = FangtecCameraDLL.ALC_GetSensorSettingVersion(out szStringTmp);

                // GlobalVariable.DebugMessage("winmate", "ALC_GetSensorSettingVersion = " + szStringTmp, GlobalVariable.bDebug);// brian add

                if (szStringTmp != "")
                {
                    sFirstVersion = szStringTmp.Substring(0, 2);
                    sSecondVersion = szStringTmp.Substring(2, 6);

                    if (sFirstVersion == "6L")
                    {
                        iDate = Convert.ToInt32(sSecondVersion);
                        if (iDate >= 121120)
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_1_RT;
                        else
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_0_RT;

                    }
                    else if (sFirstVersion == "6R")
                    {
                        iDate = Convert.ToInt32(sSecondVersion);
                        if (iDate >= 121120)
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT;
                        else
                            iRet = (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_0_RT_RIGHT;
                    }
                }

                // bRet = FangtecCameraDLL.ALC_UnInitialization();
            }
            else if (szDevicePatch.IndexOf("vid_058f&pid_3823") != -1)// 5M
            {
                iRet = (int)CameraDeviceIdList.AV5050A22_V_1_0;
            }
            else if (szDevicePatch.IndexOf("vid_1e4e&pid_0109") != -1)// 5M
            {
                iRet = (int)CameraDeviceIdList.DAP7_5M;
            }
            else if (szDevicePatch.IndexOf("vid_2084&pid_0405") != -1)// 7DAP 5M REV01
            {
                iRet = (int)CameraDeviceIdList.DAP7_2084_0405;
            }
            else if (szDevicePatch.IndexOf("vid_058f&pid_3832") != -1)
            {
                iRet = (int)CameraDeviceIdList.AH4220C2_S2_2Z7_V_1_1;
            }


            return iRet;
        }


        public int GetZoom()
        {
            int min = 0;
            int max = 0;
            return GetControlProperties(CameraControlProperty.Focus, ref min, ref max);

        }

        private int GetControlProperties(DirectShowLib.CameraControlProperty CtrlProp, ref int iMin, ref int iMax)
        {
            try
            {
                int iStep = 0;
                int iDft = 0;
                CameraControlFlags flags = new CameraControlFlags();
                flags = CameraControlFlags.Auto;

                int iResult = 0;
                iResult = m_CamControl.GetRange(CtrlProp, out iMin, out iMax, out iStep, out iDft, out flags);
                string s1;
                s1 = String.Format("{0:X2}", iResult);

                return iResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
                // Interaction.MsgBox(ex.Message);
            }
        }

        // CameraControl auto focus
        public void SetupProperties(int iDeviceNum, int focus, bool auto)
        {
            DsDevice[] capDevices;
            // Get the collection of video devices
            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            DsDevice dev = capDevices[iDeviceNum];
            // Set up the capture graph
            SetupProperties(dev, focus, auto);
        }

        // CameraControl auto focus
        public void SetupProperties(DsDevice dev, int focus, bool auto)
        {
            // CameraControl
            int pMin, pMax, pSteppingDelta, pDefault;
            CameraControlFlags pFlags;
            object o;

            Guid IID_IBaseFilter = new Guid("56a86895-0ad4-11ce-b03a-0020af0ba770");
            dev.Mon.BindToObject(null, null, ref IID_IBaseFilter, out o);

            IAMCameraControl icc;

            // Get the graphbuilder object
            IFilterGraph2 graphBuilder = new FilterGraph() as IFilterGraph2;
            IBaseFilter capFilter = null;
            int hr = graphBuilder.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
            DsError.ThrowExceptionForHR(hr);
            icc = capFilter as IAMCameraControl;

            icc.GetRange(CameraControlProperty.Focus, out pMin, out pMax, out pSteppingDelta, out pDefault, out pFlags);

            if (auto)
                pFlags = CameraControlFlags.Auto;
            else
                pFlags = CameraControlFlags.Manual;

            if (focus >= pMin && focus <= pMax)
            {
                icc.Set(CameraControlProperty.Focus, focus, pFlags);
            }
        }

        // Zero based device index and device params and output window
        public HotTabCamera(int iDeviceNum, int iWidth, int iHeight, short iBPP, Control hControl)
        {
            DsDevice [] capDevices;
       
            // Get the collection of video devices
            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            if (iDeviceNum + 1 > capDevices.Length) throw new Exception("No video capture devices found at that index!");
            try
            {
                DsDevice dev = capDevices[iDeviceNum];

                iDeviceId = CameraDeviceIdCompare(iDeviceNum, dev.DevicePath);

                SetupGraph(dev, iWidth, iHeight, iBPP, hControl); // Set up the capture graph

                SetupProperties(dev, 0, true); // Camera Control

                if ((iDeviceId == (int)CameraDeviceIdList.AG3820A11_S1_3ND) || (iDeviceId == (int)CameraDeviceIdList.AV3850A22_SB_4B0) || (iDeviceId == (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_0_RT) || (iDeviceId == (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_0_RT_RIGHT) || (iDeviceId == (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_1_RT) || (iDeviceId == (int)CameraDeviceIdList.AG3820A11_S1_3ND_V_1_1_RT_RIGHT))
                {
                    FangtecCameraDLL.ALC_Initialization(iDeviceNum, dev.DevicePath);
                    FangtecCameraDLL.ALC_SetDeviceIdx(iDeviceNum);
                }

                m_PictureReady = new ManualResetEvent(false); // tell the callback to ignore new images
            }
            catch
            {
                Dispose(); // throw;
            }
        }

        public static Point CheckMaxResolution(int iDeviceNum)
        {
            // GlobalVariable.DebugMessage("winmate", "CheckMaxResolution start", GlobalVariable.bDebug);// brian add

            Point pp = new Point(0, 0);
            DsDevice[] capDevices;

            // Get the collection of video devices
            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            if (iDeviceNum + 1 > capDevices.Length)
            {
                throw new Exception("No video capture devices found at that index!");
            }

            try
            {
                DsDevice dev = capDevices[iDeviceNum];

                // Set up the capture graph
                pp = SetupGraph2(dev);

            }
            catch
            {

            }

            // GlobalVariable.DebugMessage("winmate", "CheckMaxResolution end", GlobalVariable.bDebug);// brian add

            return pp;


        }



        /// <summary> release everything. </summary>
        public void Dispose()
        {
            // FangtecCameraDLL.ALC_UnInitialization();
#if DEBUG
            if (m_rot != null)
            {
                m_rot.Dispose();
            }
#endif
            CloseInterfaces();
            if (m_PictureReady != null)
            {
                m_PictureReady.Close();
            }
        }
        // Destructor
        ~HotTabCamera()
        {
            Dispose();
        }

        /// <summary>
        /// Get the image from the Still pin.  The returned image can turned into a bitmap with
        /// Bitmap b = new Bitmap(cam.Width, cam.Height, cam.Stride, PixelFormat.Format24bppRgb, m_ip);
        /// If the image is upside down, you can fix it with
        /// b.RotateFlip(RotateFlipType.RotateNoneFlipY);
        /// </summary>
        /// <returns>Returned pointer to be freed by caller with Marshal.FreeCoTaskMem</returns>
        public IntPtr Click()
        {
            int hr;

            // get ready to wait for new image
            m_PictureReady.Reset();
            m_ipBuffer = Marshal.AllocCoTaskMem(Math.Abs(m_stride) * m_videoHeight);

            try
            {
                m_WantOne = true;
                
                // If we are using a still pin, ask for a picture
                if (m_VidControl != null)
                {
                    hr = m_VidControl.SetMode(m_pinStill, VideoControlFlags.Trigger);
                }

                // Start waiting
                if ( ! m_PictureReady.WaitOne(30000, false) )
                {
                    // throw new Exception("Timeout waiting to get picture");
                    // MainForm.ShowDialogMessageBox("Timeout waiting to get picture","Attention");
                    Marshal.FreeCoTaskMem(m_ipBuffer);
                    m_ipBuffer = IntPtr.Zero;
                }
            }
            catch
            {
                Marshal.FreeCoTaskMem(m_ipBuffer);
                m_ipBuffer = IntPtr.Zero;
                // throw;
                m_PictureReady.Set();
                m_WantOne = false;
            }
	
            // Got one
            return m_ipBuffer;
        }

        public int Width
        {
            get
            {
                return m_videoWidth;
            }
        }
        public int Height
        {
            get
            {
                return m_videoHeight;
            }
        }
        public int Stride
        {
            get
            {
                return m_stride;
            }
        }

        /// <summary> build the capture graph for grabber. </summary>
        private void SetupGraph(DsDevice dev, int iWidth, int iHeight, short iBPP, Control hControl)
        {
            int hr;

            ISampleGrabber sampGrabber = null;
            IBaseFilter capFilter = null;
            IPin pCaptureOut = null;
            IPin pSampleIn = null;
            IPin pRenderIn = null;
            // Get the graphbuilder object
            m_FilterGraph = new FilterGraph() as IFilterGraph2;

            try
            {
#if DEBUG
                m_rot = new DsROTEntry(m_FilterGraph);
#endif
                // add the video input device
                hr = m_FilterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
                DsError.ThrowExceptionForHR(hr);
           
                m_CamControl = (IAMCameraControl)capFilter;

                // Didn't find one.  Is there a preview pin?
                if (m_pinStill == null)
                {
                    m_pinStill = DsFindPin.ByCategory(capFilter, PinCategory.Preview, 0);
                }

                // Still haven't found one.  Need to put a splitter in so we have
                // one stream to capture the bitmap from, and one to display.  Ok, we
                // don't *have* to do it that way, but we are going to anyway.
                if (m_pinStill == null)
                {
                    IPin pRaw = null;
                    IPin pSmart = null;

                    // There is no still pin
                    m_VidControl = null;

                    // Add a splitter
                    IBaseFilter iSmartTee = (IBaseFilter)new SmartTee();

                    try
                    {
                        hr = m_FilterGraph.AddFilter(iSmartTee, "SmartTee");
                        DsError.ThrowExceptionForHR(hr);

                        // Find the find the capture pin from the video device and the
                        // input pin for the splitter, and connnect them
                        pRaw = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);
                        pSmart = DsFindPin.ByDirection(iSmartTee, PinDirection.Input, 0);
                     
                        hr = m_FilterGraph.Connect(pRaw, pSmart);
                        DsError.ThrowExceptionForHR(hr);

                        // Now set the capture and still pins (from the splitter)
                        m_pinStill = DsFindPin.ByName(iSmartTee, "Preview");
                        pCaptureOut = DsFindPin.ByName(iSmartTee, "Capture");

                        // If any of the default config items are set, perform the config
                        // on the actual video device (rather than the splitter)
                        if (iHeight + iWidth + iBPP > 0)
                        {
                            SetConfigParms(pRaw, iWidth, iHeight, iBPP);
                        }
                    }
                    finally
                    {
                        if (pRaw != null)
                        {
                            Marshal.ReleaseComObject(pRaw);
                        }
                        if (pRaw != pSmart)
                        {
                            Marshal.ReleaseComObject(pSmart);
                        }
                        if (pRaw != iSmartTee)
                        {
                            Marshal.ReleaseComObject(iSmartTee);
                        }
                    }
                }
                else
                {
                    // Get a control pointer (used in Click())
                    m_VidControl = capFilter as IAMVideoControl;

                    pCaptureOut = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);

                    // If any of the default config items are set
                    if (iHeight + iWidth + iBPP > 0)
                    {
                        SetConfigParms(m_pinStill, iWidth, iHeight, iBPP);
                    }
                }

                // Get the SampleGrabber interface
                sampGrabber = new SampleGrabber() as ISampleGrabber;

                // Configure the sample grabber
                IBaseFilter baseGrabFlt = sampGrabber as IBaseFilter;
                ConfigureSampleGrabber(sampGrabber);
                pSampleIn = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);

                // Get the default video renderer
                // IBaseFilter pRenderer = new VideoRendererDefault() as IBaseFilter;
                IBaseFilter pRenderer = new VideoRenderer() as IBaseFilter;
                hr = m_FilterGraph.AddFilter(pRenderer, "Renderer");
                DsError.ThrowExceptionForHR(hr);
                
                pRenderIn = DsFindPin.ByDirection(pRenderer, PinDirection.Input, 0);

                // Add the sample grabber to the graph
                hr = m_FilterGraph.AddFilter(baseGrabFlt, "Ds.NET Grabber");
                DsError.ThrowExceptionForHR(hr);

                if (m_VidControl == null)
                {
                    // Smart Tree

                    // Connect the Still pin to the sample grabber
                    hr = m_FilterGraph.Connect(m_pinStill, pSampleIn);
                    DsError.ThrowExceptionForHR(hr);

                    // Connect the capture pin to the renderer
                    hr = m_FilterGraph.Connect(pCaptureOut, pRenderIn);
                    DsError.ThrowExceptionForHR(hr);
                }
                else
                {
                    // Still Image

                    // Connect the capture pin to the renderer
                    hr = m_FilterGraph.Connect(pCaptureOut, pRenderIn);
                    DsError.ThrowExceptionForHR(hr);

                    // Connect the Still pin to the sample grabber
                    hr = m_FilterGraph.Connect(m_pinStill, pSampleIn);
                    DsError.ThrowExceptionForHR(hr);
                }

                // Learn the video properties
                SaveSizeInfo(sampGrabber);
                ConfigVideoWindow(hControl);

                // Start the graph
                IMediaControl mediaCtrl = m_FilterGraph as IMediaControl;
                hr = mediaCtrl.Run();
                DsError.ThrowExceptionForHR(hr);
            }
            finally
            {
                if (sampGrabber != null)
                {
                    Marshal.ReleaseComObject(sampGrabber);
                    sampGrabber = null;
                }
                if (pCaptureOut != null)
                {
                    Marshal.ReleaseComObject(pCaptureOut);
                    pCaptureOut = null;
                }
                if (pRenderIn != null)
                {
                    Marshal.ReleaseComObject(pRenderIn);
                    pRenderIn = null;
                }
                if (pSampleIn != null)
                {
                    Marshal.ReleaseComObject(pSampleIn);
                    pSampleIn = null;
                }
            }
        }

        public static Point SetupGraph2(DsDevice dev)
        {
            // GlobalVariable.DebugMessage("winmate", "SetupGraph2 start", GlobalVariable.bDebug);// brian add

            int hr;
            Point pp = new Point(0, 0);
            IBaseFilter capFilter = null;
            IFilterGraph2 m_FilterGraph2 = null;

            // Get the graphbuilder object
            m_FilterGraph2 = new FilterGraph() as IFilterGraph2;

            try
            {

                // add the video input device
                hr = m_FilterGraph2.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
                DsError.ThrowExceptionForHR(hr);

                IPin mStill = DsFindPin.ByCategory(capFilter, PinCategory.Still, 0);
                if (mStill != null)
                {
                    pp = GetMaxFrameSize(mStill);
                    // GlobalVariable.DebugMessage("winmate", "still max=" + pp.X + "x" + pp.Y, GlobalVariable.bDebug);
                    if (mStill != null)
                    {
                        Marshal.ReleaseComObject(mStill);
                        mStill = null;
                    }
                }
                else
                {
                    IPin mCapture = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);
                    if (mCapture != null)
                    {
                        pp = GetMaxFrameSize(mCapture);
                        // GlobalVariable.DebugMessage("winmate", "capture max=" + pp.X + "x" + pp.Y, GlobalVariable.bDebug);
                        if (mCapture != null)
                        {
                            Marshal.ReleaseComObject(mCapture);
                            mCapture = null;
                        }
                    }
                }
            }
            finally
            {

            }

            // GlobalVariable.DebugMessage("winmate", "SetupGraph2 end", GlobalVariable.bDebug);// brian add

            return pp;
        }

        private void SaveSizeInfo(ISampleGrabber sampGrabber)
        {
            int hr;

            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();

            hr = sampGrabber.GetConnectedMediaType( media );
            DsError.ThrowExceptionForHR( hr );

            if( (media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero) )
            {
                throw new NotSupportedException( "Unknown Grabber Media Format" );
            }

            // Grab the size info
            VideoInfoHeader videoInfoHeader = (VideoInfoHeader) Marshal.PtrToStructure( media.formatPtr, typeof(VideoInfoHeader) );
            m_videoWidth = videoInfoHeader.BmiHeader.Width;
            m_videoHeight = videoInfoHeader.BmiHeader.Height;
            m_stride = m_videoWidth * (videoInfoHeader.BmiHeader.BitCount / 8);
            
            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        // Set the video window within the control specified by hControl
        private void ConfigVideoWindow(Control hControl)
        {
            int hr;

            IVideoWindow ivw = m_FilterGraph as IVideoWindow;

            // Set the parent
            hr = ivw.put_Owner(hControl.Handle);
            DsError.ThrowExceptionForHR( hr );

            // Turn off captions, etc
            hr = ivw.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);
            DsError.ThrowExceptionForHR( hr );

            // Yes, make it visible
            hr = ivw.put_Visible( OABool.True );
            DsError.ThrowExceptionForHR( hr );

            // Move to upper left corner
            Rectangle rc = hControl.ClientRectangle;
            hr = ivw.SetWindowPosition( 0, 0, rc.Right, rc.Bottom );
            DsError.ThrowExceptionForHR( hr );


        }

        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
        {
            int hr;
            AMMediaType media = new AMMediaType();
            
            // Set the media type to Video/RBG24
            media.majorType = MediaType.Video;
            media.subType = MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;
            hr = sampGrabber.SetMediaType( media );
            DsError.ThrowExceptionForHR( hr );

            DsUtils.FreeAMMediaType(media);
            media = null;

            // Configure the samplegrabber
            hr = sampGrabber.SetCallback( this, 1 );
            DsError.ThrowExceptionForHR( hr );
        }

        public static Point GetMaxFrameSize(IPin pStill)
        {
            VideoInfoHeader v;

            IAMStreamConfig videoStreamConfig = pStill as IAMStreamConfig;

            int iCount = 0, iSize = 0;
            videoStreamConfig.GetNumberOfCapabilities(out iCount, out iSize);

            IntPtr TaskMemPointer = Marshal.AllocCoTaskMem(iSize);

            int iMaxHeight = 0;
            int iMaxWidth = 0;
            AMMediaType pmtConfig = null;
            for (int iFormat = 0; iFormat < iCount; iFormat++)
            {
                IntPtr ptr = IntPtr.Zero;

                videoStreamConfig.GetStreamCaps(iFormat, out pmtConfig, TaskMemPointer);

                v = (VideoInfoHeader)Marshal.PtrToStructure(pmtConfig.formatPtr, typeof(VideoInfoHeader));
                String ss = v.BmiHeader.Width.ToString() + "x" + v.BmiHeader.Height.ToString();
                if (v.BmiHeader.Width > iMaxWidth)
                {
                    iMaxWidth = v.BmiHeader.Width;
                    iMaxHeight = v.BmiHeader.Height;
                }

            }

            Marshal.FreeCoTaskMem(TaskMemPointer);
            DsUtils.FreeAMMediaType(pmtConfig);

            videoStreamConfig = null;

            return new Point(iMaxWidth, iMaxHeight);
        }

        // Set the Framerate, and video size
        private void SetConfigParms(IPin pStill, int iWidth, int iHeight, short iBPP)
        {
            int hr;
            AMMediaType media;
            VideoInfoHeader v;

            IAMStreamConfig videoStreamConfig = pStill as IAMStreamConfig;
   
            // Get the existing format block
            hr = videoStreamConfig.GetFormat(out media);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // copy out the videoinfoheader
                v = new VideoInfoHeader();
                Marshal.PtrToStructure( media.formatPtr, v );
                
                // if overriding the width, set the width
                if (iWidth > 0)
                {
                    v.BmiHeader.Width = iWidth;
                }
                
                // if overriding the Height, set the Height
                if (iHeight > 0)
                {
                    v.BmiHeader.Height = iHeight;
                }

                // if overriding the bits per pixel
                if (iBPP > 0)
                {
                    v.BmiHeader.BitCount = iBPP;
                }
                
                // Copy the media structure back
                Marshal.StructureToPtr( v, media.formatPtr, false );

                // Set the new format
                hr = videoStreamConfig.SetFormat( media );
                DsError.ThrowExceptionForHR( hr );
            }
            finally
            {
                DsUtils.FreeAMMediaType(media);
                media = null;
            }
        }

        /// <summary> Shut down capture </summary>
        private void CloseInterfaces()
        {
            int hr;

            try
            {
                if( m_FilterGraph != null )
                {
                    IMediaControl mediaCtrl = m_FilterGraph as IMediaControl;

                    // Stop the graph
                    hr = mediaCtrl.Stop();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (m_FilterGraph != null)
            {
                Marshal.ReleaseComObject(m_FilterGraph);
                m_FilterGraph = null;
            }

            if (m_VidControl != null)
            {
                Marshal.ReleaseComObject(m_VidControl);
                m_VidControl = null;
            }

            if (m_pinStill != null)
            {
                Marshal.ReleaseComObject(m_pinStill);
                m_pinStill = null;
            }
        }

        /// <summary> sample callback, NOT USED. </summary>
        int ISampleGrabberCB.SampleCB( double SampleTime, IMediaSample pSample )
        {
            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        /// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
        int ISampleGrabberCB.BufferCB( double SampleTime, IntPtr pBuffer, int BufferLen )
        {
            // Note that we depend on only being called once per call to Click.  Otherwise
            // a second call can overwrite the previous image.
            Debug.Assert(BufferLen == Math.Abs(m_stride) * m_videoHeight, "Incorrect buffer length");

            if (m_WantOne)
            {
                Debug.Assert(m_ipBuffer != IntPtr.Zero, "Unitialized buffer");
            
                // Save the buffer
                CopyMemory(m_ipBuffer, pBuffer, BufferLen);

                // Picture is ready.
                m_PictureReady.Set();

                m_WantOne = false;
            }

            return 0;
        }

        // 直接呼叫使用=======================================================================================
        public static int GetCameraCount()
        {
            DsDevice[] capDevices;

            // Get the collection of video devices
            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            if (capDevices.Length < 1) return 0;
            else return capDevices.Length;
        }
        // 直接呼叫使用=======================================================================================
        public static List<String> GetListCamera()
        {
            DsDevice[] capDevices;
            List<String> list = new List<String>();

            // Get the collection of video devices
            capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            if (capDevices.Length != 0)
            {
                for (int i = 0; i < capDevices.Length; i++)
                {
                    string szTmp = "";
                    if (capDevices[i].DevicePath.IndexOf("vid_") != -1) szTmp = capDevices[i].DevicePath.Substring(20, 17);
                    list.Add(capDevices[i].Name + "(" + szTmp + ")");
                }
            }
            return list;
        }

        public void Flash(byte on)// 1:on, 0:off
        {
            if (on == 0)
                FangtecCameraDLL.Flash(0);
            else
                FangtecCameraDLL.Flash(1);
        }
    }
}
