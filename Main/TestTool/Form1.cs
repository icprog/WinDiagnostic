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
using System.Threading;
using System.Diagnostics;

namespace TestTool
{
    public partial class Form1 : Form
    {
        dynamic jsobj = null;
        List<Thread> threads = new List<Thread>();
        Thread thread_test;
        JObject js_result = new JObject();
        List<string> list_strItems = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private TreeNode build_node(string key, JObject jobj)
        {
            TreeNode node = new TreeNode(key);
            if (jobj["items"] != null)
            {
                foreach (string item in jobj["items"])
                {
                    node.Nodes.Add(build_node(item, (JObject)jobj[item]));
                }
            }
            else
            {
                list_strItems.Add(key);
                js_result.Add(key,jobj);
            }
            return node;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string filepath = "testitem_config.json";
            if (!File.Exists(filepath))
            {
                MessageBox.Show(filepath + " not found.");
                return ;
            }            
            jsobj = JObject.Parse(File.ReadAllText(filepath));
            treeView_items.ShowLines = true;

            treeView_items.Nodes.Add("Serial Number : " + jsobj.sno);
            treeView_items.Nodes.Add("Model : " + jsobj.model);

            foreach (string item in jsobj["items"])
            {
                treeView_items.Nodes.Add(build_node(item, (JObject)jsobj[item]));
            }
            return;
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (btn_start.Text == "Start")
            {
                btn_start.Text = "Stop";
                thread_test = new Thread(new ParameterizedThreadStart(TestThread));
                thread_test.Start(jsobj);

            }
            else
            {
                btn_start.Text = "Start";
                if (thread_test.IsAlive)
                    thread_test.Abort();
                foreach (Thread t in threads)
                {
                    if (t.IsAlive)
                    {
                        t.Abort();
                    }
                }
            }
        }

        public void TestThread(object obj)
        {
            JObject jobj = obj as JObject;

            foreach (string item in jobj["manual"]["items"])
            {
                createProcess(jobj["manual"][item]);
            }

            foreach (string item in jobj["automatic"]["items"])
            {
                Thread t = new Thread(new ParameterizedThreadStart(MultiThread));
                threads.Add(t);
                t.Start(jobj["automatic"][item]);
            }

            bool isCompleted = false;
            while(!isCompleted)
            {
                isCompleted = true;
                foreach(string item in list_strItems)
                {
                    if (js_result[item]["result"]==null)
                    {
                        isCompleted = false;
                    }
                }
            }
            btn_start.Text = "Start";
        }

        public void MultiThread(object obj)
        {
            JObject jobj = obj as JObject;
            if (jobj["items"] == null)
            {
                createProcess(jobj);
            }
            else
            {
                foreach (string item in jobj["items"])
                {
                    MultiThread(jobj[item]);
                }
            }
        }

        public void createProcess(object obj)
        {
            //Process.Start(strFile);
            int i = 0;
            JObject jobj = obj as JObject;
            while (true)
            {
                Console.WriteLine((string)jobj["name"] + i.ToString());
                Thread.Sleep(1000);
                i++;
                if (i > 10) break;
            }
        }
    }    
}
