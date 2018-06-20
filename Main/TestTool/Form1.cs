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
        JObject jobj_global = null;        
        List<Thread> threads = new List<Thread>();
        Thread thread_test;
        JObject js_result = new JObject();
        List<TreeNode> list_treenode_items = new List<TreeNode>();

        public Form1()
        {
            InitializeComponent();
        }

        private TreeNode build_node(string key, JObject jobj)
        {
            TreeNode node = new TreeNode(key);
            node.Name=key;
            if (jobj["items"] != null)
            {
                foreach (string item in jobj["items"])
                {
                    node.Nodes.Add(build_node(item, (JObject)jobj[item]));
                }
            }
            else
            {                
                js_result.Add(key,jobj);
                JObject j = JObject.Parse(File.ReadAllText(key+"\\"+key+"_"+ jsobj.model + ".json"));
                j.Merge(jobj_global, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                File.WriteAllText(key + "\\config.json", j.ToString());
                list_treenode_items.Add(node);
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
            jobj_global = JObject.Parse(File.ReadAllText("global_config.json"));
            jsobj = JObject.Parse(File.ReadAllText(filepath));
            treeView_items.ShowLines = true;

            treeView_items.Nodes.Add("Serial Number : " + jsobj.sno);
            treeView_items.Nodes.Add("Model : " + jsobj.model);

            js_result.Add("sno", jsobj.sno);
            js_result.Add("model", jsobj.model);

            foreach (string item in jsobj["items"])
            {
                treeView_items.Nodes.Add(build_node(item, (JObject)jsobj[item]));                
            }
            treeView_items.ExpandAll();            
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
                TreeNode node = treeView_items.Nodes["manual"].Nodes[item];
                ShowStatus(node, 2);
                createProcess(jobj["manual"][item]);
                ShowStatus(node, isfailed(node.Text));
            }

            foreach (string item in jobj["automatic"]["items"])
            {
                Thread t = new Thread(new ParameterizedThreadStart(MultiThread));
                threads.Add(t);
                t.Start(jobj["automatic"][item]);
            }

            bool isCompleted = false;            
            while (!isCompleted)
            {
                isCompleted = true;
                foreach (TreeNode node in list_treenode_items)
                {
                    if (node.Text.Contains("PASS") || node.Text.Contains("FAIL"))
                        continue;
                    int r = isfailed(node.Name);
                    if (r == -1)
                    {
                        isCompleted = false;
                        continue;
                    }
                    else
                        ShowStatus(node, r);
                }
                Thread.Sleep(3000);
            }
            File.WriteAllText("test_result.json", js_result.ToString());
            ModifyControlStr(btn_start, "Start");
        }
        private int isfailed(string item)
        {
            string strFile = item + "\\result.json";
            if (!File.Exists(strFile))
            {
                return -1;
            }
            JObject j = new JObject();
            j.Add(item, JObject.Parse(File.ReadAllText(strFile)));
            js_result.Merge(j, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            if ((bool)j[item]["result"] == true)
                return 1;
            return 0;
        }

        public void MultiThread(object obj)
        {
            JObject jobj = obj as JObject;
            if (jobj["items"] == null)
            {
                TreeNode node = findNode(treeView_items.Nodes, (string)jobj["name"]);
                ShowStatus(node, 2);
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


        public delegate void ModifyTreeNodeStrDelegate(TreeNode node, string str);
        private void ModifyControlStr(TreeNode node, string str)
        {            
            if (this.InvokeRequired)
            {
                ModifyTreeNodeStrDelegate d = new ModifyTreeNodeStrDelegate(ModifyControlStr);
                this.Invoke(d, node, str);
            }
            else
                node.Text = str;
        }

        public delegate void ModifyControlStrDelegate(Control ctl, string str);
        private void ModifyControlStr(Control ctl, string str)
        {
            if (this.InvokeRequired)
            {
                ModifyControlStrDelegate d = new ModifyControlStrDelegate(ModifyControlStr);
                this.Invoke(d, ctl, str);
            }
            else
                ctl.Text = str;
        }

        private void ShowStatus(TreeNode node, int i)
        {
            string str = "";
            if (i == -1)
                return;
            switch(i)
            {
                case 0:
                    str = "  _FAIL";
                    break;
                case 1:
                    str = "  _PASS";
                    break;
                case 2:
                    str = "  _TESTING...";
                    break;
            }
            ModifyControlStr(node, node.Name + str);
        }

        private TreeNode findNode(TreeNodeCollection nodes, string key)
        {
            TreeNode node = nodes[key];
            if ( node == null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    node = findNode(nodes[i].Nodes, key);
                    if (node!=null) break;
                }                
            }
            return node;
        }

        public void createProcess(object obj)
        {
            JObject jobj = obj as JObject;

            Process p = Process.Start((string)jobj["path"]+"\\"+ (string)jobj["name"]+".exe");
            if (!p.WaitForExit(1000*(int)jobj["timeout"]))
                p.Kill();

            //int i = 0;
            //while (true)
            //{
            //    Console.WriteLine((string)jobj["name"] + i.ToString());
            //    Thread.Sleep(1000);
            //    i++;
            //    if (i > 1) break;
            //}
        }
    }    
}
