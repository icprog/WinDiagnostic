using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SerialNumber
{
    public partial class Form1 : Form
    {
        string NowLSN;
        string NowSN;
        string NowOperator;
        bool IsCustomerModeForBARTEC;
        int SerialNumberLength;
        int OperatorLength;
        bool IsDeveloperMode;
        string SerialNumberInSMBIOS;

        public Form1()
        {
            InitializeComponent();
        }

        #region Button Event
        
        private void buttonClearData_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            switch (button.Name)
            {
                case "buttonClearLSN":
                    textBoxLSN.Text = "";
                    break;
                case "buttonClearSN":
                    textBoxSN.Text = "";
                    break;
                case "buttonClearOperator":
                    textBoxOperator.Text = "";
                    break;
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void buttonExportKey_Click(object sender, EventArgs e)
        {
            string exportPath = "\"" + Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\BlackList.reg" + "\"";
            const string registryPath = "\"" + "HKEY_LOCAL_MACHINE\\SOFTWARE\\BlackList" + "\"";

            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "regedit.exe";
                proc.StartInfo.UseShellExecute = false;
                // proc.StartInfo.Verb = "runas";
                proc = Process.Start("regedit.exe", "/e " + exportPath + " " + registryPath + "");

                if (proc != null) proc.WaitForExit();
            }
            finally
            {
                if (proc != null) proc.Dispose();
            }
            MessageBox.Show("The export process is complete.\n\nCheck your desktop or the folder that you have specified as your save location.", "Attention");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //checkTestStatus("PASS");
        }
#if false
        private void checkTestStatus(String testResult)
        {
            NowLSN = textBoxLSN.Text;
            NowSN = textBoxSN.Text;
            NowOperator = textBoxOperator.Text;

            if (!buttonBurnIn.Enabled) Application.Exit();
            else if (IsCustomerModeForBARTEC)
            {
                if (string.IsNullOrEmpty(NowLSN))
                {   
                    MessageBox.Show("Please enter the serial number", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (textBoxLSN.Text.Length != SerialNumberLength)
                {
                    MessageBox.Show("Serial number was not in a correct format.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // LSN跟SN只要任一欄位有符合規則就讓可以繼續
                if (!textBoxLSN.Text.Length.Equals(SerialNumberLength) || string.IsNullOrEmpty(NowLSN) /* || !IsNumeric(MainForm.NowLSN) */)
                {
                    MessageBox.Show("Please enter the correct format LSN.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (textBoxOperator.Text.Length != OperatorLength || string.IsNullOrEmpty(NowOperator))
                {
                    MessageBox.Show("Operator ID were not in a correct format.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    this.Enabled = false; // 直接將頁面所有的控制項關閉
                    //if (IsDeveloperMode)
                    //    SerialNumberInSMBIOS = "1W10C0021009"; // 1W10C0021009 / 1W1840030065
                    //else
                    //    SerialNumberInSMBIOS = HotTabWMIInformation.GetWMI_BIOSSerialNumber();

                    //Trace.WriteLine("SerialNumber In SMBIOS : " + SerialNumberInSMBIOS);

                    ProductSerialNumber = NowLSN;

                    if (string.IsNullOrEmpty(NowSN))
                    {
                        Trace.WriteLine("Only LSN. Do not confirm whether LSN and SN are the same value.");
                    }
                    else
                    {
                        if (!SerialNumberInSMBIOS.Length.Equals(MainForm.SerialNumberLength))
                        {
                            // Trace.WriteLine("This is not 12-digit Product SerialNumber in SMBIOS.");
                            MessageBox.Show("SMBIOS serial number is invaild (" + SerialNumberInSMBIOS + ").\r\nPlease confirm whether the serial number has burned into the SMBIOS.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                        else if (NowSN.Length.Equals(SerialNumberLength) && NowSN.Equals(SerialNumberInSMBIOS))
                        {
                            if (!PublicFunctionNetwork.IsEipServerConnected())
                            {
                                MessageBox.Show("Please confirm whether it is connected to the EIP server (192.168.100.46).", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Application.Exit();
                                return;
                            }

                            // IsTwoSerialNumberMatched("http://www1.winmate/plms/SNMatch.asp", "201804004631", "1W1840030065");
                            if (IsTwoSerialNumberMatched("http://www1.winmate/plms/SNMatch.asp", NowLSN, NowSN))
                            {
                                if (ConfigProperty.IsUploadLogToEipWithSN) ProductSerialNumber = NowSN;
                            }
                            else
                            {
                                MessageBox.Show("LSN and SN don't match in the EIP Database.\r\nPlease confirm the serial number in EIP.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Application.Exit();
                            }
                        }
                        else
                        {
                            MessageBox.Show("SMBIOS serial number (" + SerialNumberInSMBIOS + ") and input SN are not the same value.\r\nPlease confirm whether the serial number has burned into the SMBIOS.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                    }

                    isLogExist = SaveTestResult();

                    // 如果有LOG檔就繼續往下一個人工測項繼續進行測試, 不然就停止測試
                    if (isLogExist && testResult.Equals("PASS"))
                    {
                        LogPath = TempLogPath;
                        OperatorNumber = textBoxOperator.Text;
                        EIPLogPath = TempEIPLogPath;

                        if (IsDebugMode) Trace.WriteLine("LogFile : " + LogPath + " , ProductSerialNumber : " + ProductSerialNumber + " , OP : " + OperatorNumber);

                        TestReport.setResult("SerialNumber", TestReport.PassState.PASS);
                        TestReport.setResult("ProductLSN", TestReport.PassState.SKIP, textBoxLSN.Text);
                        TestReport.setResult("ProductSN", TestReport.PassState.SKIP, textBoxSN.Text);
                        if (!IsCustomerModeForBARTEC) TestReport.setResult("Operator", TestReport.PassState.SKIP, MainForm.OperatorNumber);

                        // 將LSN寫入檔案給外部的Burn-In Agent用
                        if (File.Exists(PathLSN)) File.Delete(PathLSN);
                        using (StreamWriter StreamWriterLSN = File.CreateText(PathLSN))
                        {
                            StreamWriterLSN.WriteLine(MainForm.NowLSN);
                        }

                        GotoNextTestItem("PASS"); // 人工測項按下判斷按鈕後直接跳到下一個測項
                    }
                    else
                    {
                        GotoNextTestItem(testResult);
                        MainForm.StopNextTestItem();
                    }
                }
            }

            if (MainForm.EnableReportAlarm) TimerBackgroundTwinkled.Stop();
            MainForm.isAutoTestItemFail = false; // 離開SN頁面
        }
#endif
        #endregion
                        
        private void buttonStart_Click(object sender, EventArgs e)
        {
            string strPath = Application.StartupPath;
            string strConfigFile = "global_config.json";
            JObject j = JObject.Parse(File.ReadAllText(strConfigFile));
            JObject jobj_sn = new JObject();
            jobj_sn.Add("LSN", textBoxLSN.Text);
            jobj_sn.Add("SN", textBoxSN.Text);
            jobj_sn.Add("Operator", textBoxOperator.Text);
            j.Merge(jobj_sn, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            File.WriteAllText(strConfigFile, j.ToString());

            Process p = Process.Start("TestTool.exe");
            Close();
        }
    }
}
