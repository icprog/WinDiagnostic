using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace audiojack
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    /// 
    public partial class Dialog : Window
    {
        #region Audio DLL Import
        [DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
        // The PlaySound method supports the playing of normal wave files stored on the disk.
        private static extern bool PlaySound(string pszSound, int hmod, int fdwSound);
        [DllImport("winmm.DLL")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

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
        #endregion

        string AudioFileLocation = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Left1.wav"); // 測試音效的預設路徑
        string _FileName = "";
        string _AudioResult = "";

        public string AudioResult
        {
            get
            {
                return _AudioResult;
            }
        }

        public Dialog(string DataFromUserControl)
        {
            InitializeComponent();
            _FileName = DataFromUserControl;
            Loaded += dialog_Load;
        }

        private void dialog_Load(object sender, RoutedEventArgs routedEventArgs)
        {
            AudioFileLocation = System.AppDomain.CurrentDomain.BaseDirectory + "\\" + _FileName + ".wav";

            if (File.Exists(AudioFileLocation))
            {
                if (_FileName.Contains("Right")) waveOutSetVolume(IntPtr.Zero, 0xFFFF0000);
                else waveOutSetVolume(IntPtr.Zero, 0x0000FFFF);
                PlaySound(AudioFileLocation, 0, (int)(PlaySoundFlags.SND_ASYNC | PlaySoundFlags.SND_FILENAME)); // 調用API播放聲音
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Audio File does not exist.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                _AudioResult = "FAIL";
                this.DialogResult = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            Console.WriteLine("dialog button name: {0}",button.Name);
            Console.WriteLine("_FileName: {0}", _FileName);
            if (button != null)
            {
                Regex regex = new Regex(@"\d+");
                var ButtonMatch = regex.Match(button.Name);
                var FileNameMatch = regex.Match(_FileName);

                if(ButtonMatch.Success && FileNameMatch.Success)
                {
                    if (string.Equals(ButtonMatch.Value,FileNameMatch.Value))
                    {
                        _AudioResult = "PASS";
                    }
                    else
                    {
                        _AudioResult = "FAIL";
                    }
                    Trace.WriteLine("Audio Select Button : " + button.Name + " , Result : " + _AudioResult);
                }
                else
                {
                    throw new Exception();
                }                
            }
            waveOutSetVolume(IntPtr.Zero, 0xFFFFFFFF); // 修正單聲道測試時, 左右聲道都有聲音
            this.DialogResult = true;
        }
    }
}
