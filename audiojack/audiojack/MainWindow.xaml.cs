using Newtonsoft.Json.Linq;
using SpeakerMicAutoTestApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace audiojack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int mRandom = 0;
        int SelectedAudioName = 0;
        uint WaitTimeCount = 0;
        uint WaitTime = 2;
        uint CountdownForUI = 0;
        bool IsAutoModeAudioJack = true;
        JObject result = new JObject();
        System.Windows.Forms.Timer timer;
        AudioTest AutoModeAudioTest = null;
        string TestProduct = string.Empty;
        double ExternalRecordThreshold = 0.0;
        double InternalRecordThreshold = 0.0;
        double AudioJackRecordThreshold = 0.0;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += audiojack_Load;
        }

        private void audiojack_Load(object sender, RoutedEventArgs routedEventArgs)
        {
            result["result"] = "FAIL";
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Application.Current.Shutdown();
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            TestProduct = jobject.TestProduct.ToString();
            ExternalRecordThreshold = (double)jobject.ExternalRecordThreshold;
            InternalRecordThreshold = (double)jobject.InternalRecordThreshold;
            AudioJackRecordThreshold = (double)jobject.AudioJackRecordThreshold;
            IsAutoModeAudioJack = (bool)jobject.IsAutoModeAudioJack;

            Trace.WriteLine("AudioJack_Load");

            if (IsAutoModeAudioJack)
            {
                timer = new System.Windows.Forms.Timer();
                //設定計時器的速度
                timer.Interval = 750;
                timer.Tick += new EventHandler(TimerAutoTestAudioJack_Tick);
                timer.Start();
            }
            else
            {
                ManualMode.Visibility = Visibility.Visible;
                mRandom = 0;
            }
        }

        private void TimerAutoTestAudioJack_Tick(object sender, EventArgs e)
        {
            if (WaitTimeCount < WaitTime)
            {
                Title.Content = "Wait " + (WaitTime - CountdownForUI) + " sec for Test";
                Title.Foreground = Brushes.Orange;
                Title.UpdateLayout();
            }
            if (WaitTimeCount.Equals(WaitTime))
            {
                timer.Stop();
                Title.Content = "AudioJack";
                Title.Foreground = Brushes.Black;
                Title.UpdateLayout();
                checkTestStatus(AutoModeAudioTestFunction());
            }

            WaitTimeCount++;
            CountdownForUI++;
        }

        string AutoModeAudioTestFunction()
        {
            try
            {
                if (AutoModeAudioTest == null) AutoModeAudioTest = new AudioTest(TestProduct, true);

                string wavFile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "WinmateAudioTest.wav");
                if (File.Exists(wavFile))
                {
                    AutoModeAudioTest.WavFileName = wavFile;

                    AutoModeAudioTest.ExternalRecordThreshold = ExternalRecordThreshold;
                    AutoModeAudioTest.InternalRecordThreshold = InternalRecordThreshold;
                    AutoModeAudioTest.AudioJackRecordThreshold = AudioJackRecordThreshold;

                    Trace.WriteLine("AudioJack Test");
                    AutoModeAudioTest.AudioJackTest();
                    Trace.WriteLine("AudioJackIntensity : " + AutoModeAudioTest.AudioJackIntensity);
                    Trace.WriteLine("AudioJackRecordThreshold : " + AudioJackRecordThreshold);

                    if (IsIntensityCorrect())
                    {
                        return "PASS";
                    }
                    else
                    {
                        return "Intensity does not meet the Threshold.";
                    }
                }
                else
                {
                    Trace.WriteLine("Audio test file (" + wavFile + ") does not exist.");
                    return "Audio test file does not exist";
                }
            }
            catch (Exception ex)
            {
                if (AutoModeAudioTest != null) AutoModeAudioTest = null;
                return "(AudioAutoTest) " + ex.Message;
            }
        }

        bool IsIntensityCorrect()
        {
            return (AutoModeAudioTest.AudioJackIntensity > AudioJackRecordThreshold) && !AutoModeAudioTest.AudioJackIntensity.Equals(-1);
        }

        string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(exepath, path);
        }

        int MakeRand(int intLower, int intUpper)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            return random.Next(intLower, intUpper + 1);
        }

        public Dialog PlayTestAudio(Button button)
        {
            string AudioFileName = "";

            mRandom = MakeRand(1, 5);
            Trace.WriteLine("New Random: " + mRandom);

            if (button.Name.Contains("Left")) AudioFileName = "Left" + mRandom.ToString();
            else AudioFileName = "Right" + mRandom.ToString();
            Trace.WriteLine("AudioFileName = " + AudioFileName);

            Dialog mAudioSelect = new Dialog(AudioFileName);
            return mAudioSelect;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Console.WriteLine(button.Name);
            if (button != null)
            {
                Dialog mAudioSelect = PlayTestAudio(button);
                if ((bool)mAudioSelect.ShowDialog())
                {
                    //    // 從Form(mAudioSelect)取值並顯示到UserControl(Audio), 並判斷是否測試檔有無選擇正確, 如果選擇錯誤直接給FAIL
                    if (mAudioSelect.AudioResult.Equals("FAIL"))
                    {
                        Fail.RaiseEvent(e);
                    }
                    else // 當測試ok, 才允許測試下一個聲道
                    {
                        switch (button.Name)
                        {
                            case "Left":
                                Left.IsEnabled = false;
                                Right.IsEnabled = true;
                                break;
                            case "Right":
                                Right.IsEnabled = false;
                                checkTestStatus("PASS");
                                break;
                        }
                    }
                }
            }
        }

        public void checkTestStatus(String testResult)
        {
            mRandom = 0;
            SelectedAudioName = 0;

            if (testResult.Equals("PASS"))
            {
                Console.WriteLine("PASS");
                result["result"] = "PASS";
            }
            else
            {
                result["result"] = "FAIL";
                Console.WriteLine("FAIL");
                Left.IsEnabled = true;
                Right.IsEnabled = false;
            }

            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Send);
        }

        private void Fail_Click(object sender, RoutedEventArgs e)
        {
            checkTestStatus("FAIL");
        }
    }
}
