using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace comport
{
    public partial class comport : Form
    {
        bool IsDebugMode = true; // 是否開啟 Trace.WriteLine()顯示
        bool ShowWindow = false;
        string[] allSerialPorts = SerialPort.GetPortNames();
        SerialPort _serialPort = new SerialPort();
        string ComportTestMsg = "WinmateComportTest"; // "Winmate- Industrial LCD Display, Panel pc, industrial pc, Digital Signage, Marine LCD,HMI & rugged tablet pc, Rugged pc, panel mount PC & LCD, rugged mobile pc manufacturer.";
        bool IsSerialPortReceiving; // 功能: 1.為了能讓DoReceive方法內的While迴圈停止 2.讓開啟SerialPort後, 如果3秒內沒有觸發RS232_Receive事件就強制停止測試
        List<string> SerialPortList;        // 存放要測試的Comport位置
        List<string> DockingSerialPortList;
        int SerialPortTestNowCount = 0;     // 目前Comport正在測試的數量
        int SerialPortTestPassCount = 0;    // 目前測試成功的Comport數量
        string SerialPortTestPassName = ""; // 目前測試成功的Comport名稱
        string SerialPortTestFailName = ""; // 目前測試失敗的Comport名稱
        int SerialPortCount = 1;
        int DockingSerialPortCount = 1;
        Label[] label;


        delegate void SetCallBack();
        delegate void SafeWinFormsThread(string msg);
        JObject result = new JObject();

        public comport()
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

        private void comport_Load(object sender, EventArgs e)
        {
            result["result"] = "FAIL";
            var jsonconfig = GetFullPath("config.json");
            if (!File.Exists(jsonconfig))
            {
                MessageBox.Show("config.json not founded");
                Exit();
            }

            dynamic jobject = JObject.Parse(File.ReadAllText(jsonconfig));
            SerialPortCount = (int)jobject.SerialPortCount;
            DockingSerialPortCount = (int)jobject.DockingSerialPortCount;
            ShowWindow = (bool)jobject.ShowWindow;
            SerialPortList = jobject.SerialPort.ToObject<List<string>>();
            SerialPortList = SerialPortList.Take(SerialPortCount).ToList();
            DockingSerialPortList = jobject.DockingSerialPort.ToObject<List<string>>();
            DockingSerialPortList = DockingSerialPortList.Take(DockingSerialPortCount).ToList();            

            if (ShowWindow)
            {
                this.Opacity = 100;
                this.ShowInTaskbar = true;
            }

            if (IsDebugMode) Trace.WriteLine("Comport_Load");

            UpdateComportDetails("System comport count : " + allSerialPorts.Length);
            groupSerialPort.Visible = true;
            
            // Fill in Combobox with serial port names
            comboBoxSerialPorts.DataSource = allSerialPorts;
            buttonSerialPort.PerformClick();
            File.WriteAllText(GetFullPath("result.json"), result.ToString());
            Thread.Sleep(200);
            File.Create(GetFullPath("completed"));
            if (!ShowWindow)
                Exit();
        }

        private void buttonSerialPort_Click(object sender, EventArgs e)
        {
            SetDefaultTestUI();
            SerialPortTest();
        }

        private void SetDefaultTestUI()
        {
            buttonSerialPort.Enabled = false;
            txtComportDetails.Text = "";
            buttonSerialPort.Text = "Test";

            SerialPortTestNowCount = 0;
            SerialPortTestPassCount = 0;

            SerialPortTestPassName = "";
            SerialPortTestFailName = "";

            Label[] portlabel = null;
            Label[] dockinglabel = null;


            if (SerialPortCount != 0)
            {
                portlabel = new Label[SerialPortCount];
                for (int i = 0; i < SerialPortCount; i++)
                {
                    portlabel[i] = new Label();
                    portlabel[i].Text = string.Format("Port{0} : {1}                 ", i + 1, SerialPortList[i]);
                    portlabel[i].Left = this.buttonSerialPort.Left;
                    portlabel[i].Font = new System.Drawing.Font("Arial", 15F);
                    portlabel[i].AutoSize = true;
                    if (i > 0)
                    {
                        portlabel[i].Top = portlabel[i - 1].Bottom;
                    }
                    else
                    {
                        portlabel[i].Top = this.buttonSerialPort.Bottom;
                    }

                    this.groupSerialPort.Controls.Add(portlabel[i]);
                }
            }

            if (DockingSerialPortCount != 0)
            {
                dockinglabel = new Label[DockingSerialPortCount];
                for (int i = 0; i < DockingSerialPortCount; i++)
                {
                    dockinglabel[i] = new Label();
                    dockinglabel[i].Text = string.Format("DockingPort{0} : {1}", i + 1, DockingSerialPortList[i]);
                    dockinglabel[i].Left = this.buttonSerialPort.Left;
                    dockinglabel[i].Font = new System.Drawing.Font("Arial", 15F);
                    dockinglabel[i].AutoSize = true;
                    if (i > 0)
                    {
                        dockinglabel[i].Top = dockinglabel[i - 1].Bottom;
                    }
                    else if (SerialPortCount == 0)
                    {
                        dockinglabel[i].Top = this.buttonSerialPort.Bottom;
                    }
                    else
                    {
                        dockinglabel[i].Top = portlabel[SerialPortCount - 1].Bottom;
                    }

                    this.groupSerialPort.Controls.Add(dockinglabel[i]);
                }
            }

            if (SerialPortCount != 0 || DockingSerialPortCount != 0)
            {
                label = new Label[SerialPortCount + DockingSerialPortCount];
                for (int i = 0; i < SerialPortCount + DockingSerialPortCount; i++)
                {
                    label[i] = new Label();
                    label[i].Text = "Unknown";
                    label[i].ForeColor = Color.Blue;
                    if (SerialPortCount != 0)
                        label[i].Left = portlabel[0].Right;
                    else
                        label[i].Left = dockinglabel[0].Right;

                    label[i].Font = new System.Drawing.Font("Arial", 15F);
                    if (i > 0)
                    {
                        label[i].Top = label[i - 1].Bottom;
                    }
                    else
                    {
                        label[i].Top = this.buttonSerialPort.Bottom;
                    }

                    this.groupSerialPort.Controls.Add(label[i]);
                }
            }
        }

        private void SerialPortTest()
        {
            try
            {
                SerialPortTestPassCount = 0;
                SerialPortTestNowCount = 0;
                SerialPortList.AddRange(DockingSerialPortList);
                foreach (string TestSerialPort in SerialPortList)
                {
                    SerialPortTestNowCount++;
                    SendToComportData(TestSerialPort);
                }
            }
            catch (Exception ex)
            {
                UpdateComportDetails(ex.Message);
            }
        }

        public void SendToComportData(String mSerialPortAddress)
        {
            // using 雖然有 try 與 finally但並不包含 catch, 換句話說, 使用 using 陳述式並不會幫你捕捉例外狀況
            using (SerialPort mSerialPort = new SerialPort(mSerialPortAddress, 9600, Parity.None, 8, StopBits.One))
            {
                try
                {
                    // 在沒有設定此屬性的情況, 當程式執行到SerialPort.Read/Write時執行緒會封鎖在這個位置直到讀取資料完成為止；在指定時間內沒有讀取到任何資料就會發生TimeoutException
                    mSerialPort.ReadTimeout = 1000;
                    mSerialPort.WriteTimeout = 1000;

                    mSerialPort.Open();

                    if (mSerialPort.IsOpen)
                    {
                        IsSerialPortReceiving = true; // 如果能夠成功開啟SerialPort, 就將flag設成true
                        TimerSerialPortReceive.Enabled = true;

                        mSerialPort.WriteLine(ComportTestMsg);

                        string ComportReceiveMsg = mSerialPort.ReadLine();
                        UpdateComportDetails("\r\nTX : " + ComportTestMsg + "(" + ComportTestMsg.Length + ")" + "\r\nRX : " + ComportReceiveMsg + "(" + ComportReceiveMsg.Length + ")" + "\r\nResult = " + ComportReceiveMsg.Equals(ComportTestMsg));
                        Trace.WriteLine("\r\nTX : " + ComportTestMsg + "(" + ComportTestMsg.Length + ")" + "\r\nRX : " + ComportReceiveMsg + "(" + ComportReceiveMsg.Length + ")" + "\r\nResult = " + ComportReceiveMsg.Equals(ComportTestMsg));

                        if (ComportReceiveMsg.Equals(ComportTestMsg)) UpdateSerialPortResultUI(true, mSerialPortAddress);
                        else UpdateSerialPortResultUI(false, mSerialPortAddress);
                    }
                    else
                    {
                        UpdateComportDetails("Can't open Comport. " + System.DateTime.Now + "\r\n");
                    }
                }
                catch (Exception)
                {
                    UpdateSerialPortResultUI(false, mSerialPortAddress); // checkTestStatus(ex.Message);
                }
                finally
                {

                }
            }
        }

        private void TimerSerialPortReceive_Tick(object sender, EventArgs e)
        {
            TimerSerialPortReceive.Enabled = false;
            if (IsDebugMode) Trace.WriteLine("RS232 -- SerialPortReceiveTimer_Tick , IsSerialPortReceiving = " + IsSerialPortReceiving);
            if (IsSerialPortReceiving) checkTestStatus("Serial port response data over time.");
        }

        private void UpdateSerialPortResultUI(bool isTestPass, string testSerialPort)
        {
            #region Update UI
            if (isTestPass)
            {
                ++SerialPortTestPassCount;
                SerialPortTestPassName += testSerialPort + " ";
                UpdateComportDetails(testSerialPort + " is PASS.\r\n");
                if (SerialPortCount != 0 || DockingSerialPortCount != 0)
                {
                    label[SerialPortList.IndexOf(testSerialPort)].Text = "PASS";
                    label[SerialPortList.IndexOf(testSerialPort)].ForeColor = Color.Green;
                }
            }
            else
            {
                SerialPortTestFailName += testSerialPort + " ";
                UpdateComportDetails(testSerialPort + " is FAIL!\r\n");
                if (SerialPortCount != 0 || DockingSerialPortCount != 0)
                {
                    label[SerialPortList.IndexOf(testSerialPort)].Text = "FAIL";
                    label[SerialPortList.IndexOf(testSerialPort)].ForeColor = Color.Red;
                }
            }
            #endregion

            if (SerialPortTestNowCount.Equals(SerialPortCount + DockingSerialPortCount) && SerialPortTestPassCount.Equals(SerialPortCount + DockingSerialPortCount))
            {
                checkTestStatus("PASS");
            }
            else if (SerialPortTestNowCount.Equals(SerialPortCount))
            {
                checkTestStatus("FAIL");
            }
            else
            {
                checkTestStatus("FAIL");
            }
        }

        private void checkTestStatus(string testResult)
        {
            if (TimerSerialPortReceive.Enabled) TimerSerialPortReceive.Stop();

            // 接收Data完畢或是接收Data時發生錯誤, 將接收資料的flag設為false
            IsSerialPortReceiving = false;
            if (testResult.Equals("PASS"))
            {
                labelResult.Text = "PASS";
                labelResult.ForeColor = Color.Green;
                result["result"] = "PASS";
                result["EIPLog"] = new JObject
                {
                    { "Comport", "PASS" },
                    { "Comport_Info", "PASS"}
                };
            }
            else
            {
                labelResult.Text = "FAIL";
                labelResult.ForeColor = Color.Red;
                result["EIPLog"] = new JObject
                {
                    { "Comport", "FAIL" },
                    { "Comport_Info", testResult}
                };
            }
        }

        #region Update UI
        private void UpdateComportDetails(string msg)
        {
            try
            {
                if (txtComportDetails.InvokeRequired) txtComportDetails.Invoke(new SafeWinFormsThread(UpdateComportDetailsUI), new object[] { msg });
                else UpdateComportDetailsUI(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception = " + ex.Message);
            }
        }
        private void UpdateComportDetailsUI(string msg)
        {
            txtComportDetails.Text = txtComportDetails.Text + msg + Environment.NewLine;
            if (txtComportDetails.Text.Length > 2000) txtComportDetails.Text = txtComportDetails.Text.Substring(1000, 1000);
            txtComportDetails.SelectionStart = txtComportDetails.Text.Length;
            txtComportDetails.ScrollToCaret();
        }
        #endregion
    }
}
