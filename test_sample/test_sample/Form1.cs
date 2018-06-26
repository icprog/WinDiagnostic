using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.IO;

namespace test_sample
{
    public partial class Form1 : Form
    {
        string strPath;
        string strConfigFile;
        string strResultFile;

        public Form1()
        {
            InitializeComponent();

            strPath = Application.StartupPath;
            strConfigFile = strPath + "\\config.json";
            strResultFile = strPath + "\\result.json";

            JObject j = JObject.Parse(File.ReadAllText(strConfigFile));
            textBox1.AppendText(j.ToString());
        }

        private void btn_pass_Click(object sender, EventArgs e)
        {
            JObject j = new JObject();
            j.Add("result", true);
            File.WriteAllText(strResultFile, j.ToString());
            Close();
        }

        private void btn_fail_Click(object sender, EventArgs e)
        {
            JObject j = new JObject();
            j.Add("result", false);
            File.WriteAllText(strResultFile, j.ToString());
            Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(strPath+"\\completed", "");
        }
    }
}
