using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace functionbutton
{
    #region ViewModel
    public class ViewModel : INotifyPropertyChanged
    {
        private bool menu;
        private bool fn1;
        private bool fn2;
        private bool up;
        private bool down;
        private bool done;
        private bool hidden;

        public ViewModel()
        {
            menu = false;
            fn1 = false;
            fn2 = false;
            up = false;
            down = false;
            done = false;
            hidden = false;
        }

        public bool Hidden
        {
            get { return hidden; }
            set
            {
                hidden = value;
                NotifyPropertyChanged("Hidden");
            }
        }

        public bool Menu
        {
            get { return menu; }
            set
            {
                menu = value;
                NotifyPropertyChanged("Menu");
            }
        }

        public bool Fn1
        {
            get { return fn1; }
            set
            {
                fn1 = value;
                NotifyPropertyChanged("Fn1");
            }
        }

        public bool Fn2
        {
            get { return fn2; }
            set
            {
                fn2 = value;
                NotifyPropertyChanged("Fn2");
            }
        }

        public bool Up
        {
            get { return up; }
            set
            {
                up = value;
                NotifyPropertyChanged("Up");
            }
        }

        public bool Down
        {
            get { return down; }
            set
            {
                down = value;
                NotifyPropertyChanged("Down");
            }
        }

        // Declare the PropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        // NotifyPropertyChanged will raise the PropertyChanged event passing the
        // source property that is being updated.
        public void NotifyPropertyChanged(string propertyName)
        {
            if (done)
                return;

            int count = 0;

            foreach (PropertyInfo prop in typeof(ViewModel).GetProperties())
            {
                if ((bool)prop.GetValue(this, null) == true)
                {
                    if (MainWindow.TwoButton)
                    {
                        {
                            if (prop.Name == "Menu" || prop.Name == "Fn1")
                                count++;
                        }
                    }
                    else
                    {
                        count++;
                    }
                }
                //Trace.WriteLine(prop.Name + " " + prop.GetValue(this, null));
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            if (count == MainWindow.ButtonNumber)
            {
                done = true;
                MainWindow.checkTestStatus("PASS");
            }
        }
    }
    #endregion

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport(@"WMIO2.dll")]
        public static extern bool ModeOpen(uint uiMode);

        public static readonly DependencyProperty DependencyProperty = DependencyProperty.Register(
            "Flag", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));

        public bool Flag
        {
            get { return (bool)GetValue(DependencyProperty); }
            set { SetValue(DependencyProperty, value); }
        }

        KeyboardHook mKeyboardHook;
        ViewModel vm = new ViewModel();
        string TestProduct = string.Empty;
        public static bool TwoButton = false;
        public static int ButtonNumber = 0;
        static JObject result = new JObject();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += functionbutton_Load;
        }

        private void functionbutton_Load(object sender, RoutedEventArgs routedEventArgs)
        {
            result["result"] = "FAIL";
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Application.Current.Shutdown();
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            TwoButton = (bool)jobject.TwoButton;
            if (TwoButton)
            {
                ButtonNumber = 2;
                vm.Hidden = true;
            }
            else
                ButtonNumber = 5;

            Trace.WriteLine("FunctionButton_Load");
            Trace.WriteLine("Init KeyboardHook");
            mKeyboardHook = new KeyboardHook((int)GetHandle(this), true);
            WinIO_FunctionButtonLock();
            this.DataContext = vm;
        }

        static string GetFullPath(string path)
        {
            var exepath = System.AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(exepath, path);
        }

        public IntPtr GetHandle(Window w)
        {
            WindowInteropHelper h = new WindowInteropHelper(w);
            Trace.WriteLine("MainWindow handle: " + (int)h.Handle);
            return h.Handle;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                Trace.WriteLine("OnSourceInitialized: " + (int)handle);
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        public static void checkTestStatus(String testResult)
        {
            Task.Factory.StartNew(() =>
            {
                Trace.WriteLine("FunctionButton Result : " + testResult);
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
            });
        }

        public bool WinIO_FunctionButtonLock()
        {
            bool bResult = false;

#if IsInstallHotTab
            // Winmate Kenkun remove on 2015/09/09
            // if (MainForm.IsOldHottabVersion) 
            // {
            //    if (MainForm.IsDebugMode) Trace.WriteLine("Old Hottab Version, Hottab Mode(0).");
            //    bResult = ModeOpen(0); // Hottab Mode(Old Hottab : Only Hottab Mode)
            // }
            if (TestProduct.Equals("FMB8"))
            {
                Trace.WriteLine("Keyboard Mode - Standard Mode(1).");
                bResult = ModeOpen(1); // Standard Mode
            }
            else
            {
                Trace.WriteLine("Keyboard Mode - Consumer Mode(2).");
                bResult = ModeOpen(2); // Consumer Mode
            }

#else
            bResult = true;
#endif
            return bResult;
        }

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                #region Function Button Keyevent
                case KeyboardHookEventArgs.ECEvent_OpenMenu: // Combo Key專用, Single Key用ECEvent_MENU(因為Keycode都一樣)
                    vm.Menu = true;
                    break;
                case KeyboardHookEventArgs.ECEvent_Up:// WM_APP + 0x28
                    break;
                case KeyboardHookEventArgs.ECEvent_Down:// WM_APP + 0x29
                    break;
                case KeyboardHookEventArgs.ECEvent_VolumeUp:// WM_APP + 0x25
                    vm.Up = true;
                    break;
                case KeyboardHookEventArgs.ECEvent_VolumeDown:// WM_APP + 0x26
                    vm.Down = true;
                    break;
                case KeyboardHookEventArgs.ECEvent_Left:
                    break;
                case KeyboardHookEventArgs.ECEvent_Right:
                    break;
                case KeyboardHookEventArgs.ECEvent_Enter:
                    break;
                case KeyboardHookEventArgs.ECEvent_F1Short:
                    vm.Fn1 = true;
                    break;
                case KeyboardHookEventArgs.ECEvent_F2Short:
                    vm.Fn2 = true;
                    break;
                case KeyboardHookEventArgs.ECEvent_F3Short:
                    break;
                case KeyboardHookEventArgs.ECEvent_Q:
                    break;
                case KeyboardHookEventArgs.ECEvent_W:
                    break;
                case KeyboardHookEventArgs.ECEvent_E:
                    break;
                case KeyboardHookEventArgs.ECEvent_R:
                    break;
                case KeyboardHookEventArgs.ECEvent_T:
                    break;
                case KeyboardHookEventArgs.ECEvent_Y:
                    break;
                case KeyboardHookEventArgs.ECEvent_U:
                    break;
                case KeyboardHookEventArgs.ECEvent_I:
                    break;
                case KeyboardHookEventArgs.ECEvent_O:
                    break;
                case KeyboardHookEventArgs.ECEvent_P:
                    break;
                case KeyboardHookEventArgs.ECEvent_A:
                    break;
                case KeyboardHookEventArgs.ECEvent_S:
                    break;
                case KeyboardHookEventArgs.ECEvent_D:
                    break;
                case KeyboardHookEventArgs.ECEvent_F:
                    break;
                case KeyboardHookEventArgs.ECEvent_G:
                    break;
                case KeyboardHookEventArgs.ECEvent_H:
                    break;
                case KeyboardHookEventArgs.ECEvent_J:
                    break;
                case KeyboardHookEventArgs.ECEvent_K:
                    break;
                case KeyboardHookEventArgs.ECEvent_L:
                    break;
                case KeyboardHookEventArgs.ECEvent_Z:
                    break;
                case KeyboardHookEventArgs.ECEvent_X:
                    break;
                case KeyboardHookEventArgs.ECEvent_C:
                    break;
                case KeyboardHookEventArgs.ECEvent_V:
                    break;
                case KeyboardHookEventArgs.ECEvent_B:
                    break;
                case KeyboardHookEventArgs.ECEvent_N:
                    break;
                case KeyboardHookEventArgs.ECEvent_M:
                    break;
                case KeyboardHookEventArgs.ECEvent_BACKSPACE:
                    break;
                case KeyboardHookEventArgs.ECEvent_COMMA:
                    break;
                case KeyboardHookEventArgs.ECEvent_PERIOD:
                    break;
                case KeyboardHookEventArgs.ECEvent_0:
                    break;
                case KeyboardHookEventArgs.ECEvent_1:
                    break;
                case KeyboardHookEventArgs.ECEvent_2:
                    break;
                case KeyboardHookEventArgs.ECEvent_3:
                    break;
                case KeyboardHookEventArgs.ECEvent_4:
                    break;
                case KeyboardHookEventArgs.ECEvent_5:
                    break;
                case KeyboardHookEventArgs.ECEvent_6:
                    break;
                case KeyboardHookEventArgs.ECEvent_7:
                    break;
                case KeyboardHookEventArgs.ECEvent_8:
                    break;
                case KeyboardHookEventArgs.ECEvent_9:
                    break;
                case KeyboardHookEventArgs.ECEvent_F4Short:
                    break;
                case KeyboardHookEventArgs.ECEvent_F5Short:
                    break;
                case KeyboardHookEventArgs.ECEvent_CAMERA:
                    break;
                case KeyboardHookEventArgs.ECEvent_BARCODE:
                    break;
                case KeyboardHookEventArgs.ECEvent_ESC:
                    break;
                case KeyboardHookEventArgs.ECEvent_TAB:
                    break;
                case KeyboardHookEventArgs.ECEvent_SHIFT_Left:
                    break;
                case KeyboardHookEventArgs.ECEvent_SHIFT_Right:
                    break;
                case KeyboardHookEventArgs.ECEvent_DELETE:
                    break;
                case KeyboardHookEventArgs.ECEvent_SPACE:
                    break;
                case KeyboardHookEventArgs.ECEvent_CTRL:
                    break;
                case KeyboardHookEventArgs.ECEvent_ALT:
                    break;
                case KeyboardHookEventArgs.ECEvent_MENU: // Menu鍵(Single Keycode形式：91+None)用
                    vm.Menu = true;
                    break;
                case KeyboardHookEventArgs.ECEvent_F1:
                    break;
                case KeyboardHookEventArgs.ECEvent_F2:
                    break;
                case KeyboardHookEventArgs.ECEvent_F3:
                    break;
                case KeyboardHookEventArgs.ECEvent_F4:
                    break;
                case KeyboardHookEventArgs.ECEvent_F5:
                    break;
                case KeyboardHookEventArgs.ECEvent_F6:
                    break;
                case KeyboardHookEventArgs.ECEvent_F7:
                    break;
                case KeyboardHookEventArgs.ECEvent_F8:
                    break;
                case KeyboardHookEventArgs.ECEvent_F9:
                    break;
                case KeyboardHookEventArgs.ECEvent_F10:
                    break;
                case KeyboardHookEventArgs.ECEvent_F11:
                    break;
                case KeyboardHookEventArgs.ECEvent_F12:
                    break;
                case KeyboardHookEventArgs.ECEvent_F13:
                    break;
                case KeyboardHookEventArgs.ECEvent_F15:
                    break;
                case KeyboardHookEventArgs.ECEvent_OemMinus:
                    break;
                case KeyboardHookEventArgs.ECEvent_OemPlus:
                    break;
                case KeyboardHookEventArgs.ECEvent_OemOpenBrackets:
                    break;
                case KeyboardHookEventArgs.ECEvent_Oem6:
                    break;
                case KeyboardHookEventArgs.ECEvent_Oem5:
                    break;
                case KeyboardHookEventArgs.ECEvent_Oem1:
                    break;
                case KeyboardHookEventArgs.ECEvent_Oem7:
                    break;
                case KeyboardHookEventArgs.ECEvent_OemQuestion:
                    break;
                case KeyboardHookEventArgs.ECEvent_Pause:
                    break;
                case KeyboardHookEventArgs.ECEvent_PrtScr:
                    break;
                case KeyboardHookEventArgs.ECEvent_Insert:
                    break;
                case KeyboardHookEventArgs.ECEvent_Apps:
                    break;
                case KeyboardHookEventArgs.ECEvent_BrightnessUp:
                    break;
                case KeyboardHookEventArgs.ECEvent_BrightnessDown:
                    break;
                case KeyboardHookEventArgs.ECEvent_FM08BrightnessAuto:
                    break;
                case KeyboardHookEventArgs.ECEvent_CapsLock:
                    break;
                default:
                    break;
                    #endregion
            }

            return IntPtr.Zero;
        }

        private void FAIL_Click(object sender, RoutedEventArgs e)
        {
            checkTestStatus("FAIL");
        }
    }
}
