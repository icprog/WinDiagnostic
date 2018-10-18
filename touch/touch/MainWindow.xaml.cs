using Newtonsoft.Json.Linq;
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
using System.Windows.Shapes;

namespace touch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int TotalTestCount = 9;
        int TotalFailCount = 3;
        int TestCount = 0;
        int FailCount = 0;
        bool UseGalaxSensorTester = false;
        JObject result = new JObject();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += touch_Load;
        }

        string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(exepath, path);
        }

        private void touch_Load(object sender, RoutedEventArgs routedEventArgs)
        {
            result["result"] = "FAIL";
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Send);
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            TotalFailCount = (int)jobject.TotalFailCount;
            UseGalaxSensorTester = (bool)jobject.UseGalaxSensorTester;


            Trace.WriteLine("Touch_Load");
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button.Background.Equals(Brushes.LimeGreen))
                return;

            button.Background = Brushes.LimeGreen;
            button.Content = "O";
            TestCount++;
            if (TestCount.Equals(TotalTestCount))
                checkTestStatus("PASS");
        }

        private void checkTestStatus(string testResult)
        {
            if (testResult.Equals("PASS"))
            {
                result["result"] = "PASS";
            }
            else
            {
                result["result"] = "FAIL";
            }

            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            Application.Current.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Send);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FailCount++;
            if (FailCount >= TotalFailCount)
                checkTestStatus("FAIL");            
        }
    }
}
