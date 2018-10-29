using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace barcode
{
    public partial class DialogMessageBox : Form
    {
        public DialogMessageBox()
        {
            InitializeComponent();
        }

        private void DialogMessageBox_Load(object sender, EventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            labelResult.Text = "OK";
            this.Visible = false;  // this.Hide(); // this.Close(); // Application.Exit();
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            labelResult.Text = "Yes";
            this.Hide(); // this.Close(); // Application.Exit();
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            labelResult.Text = "No";
            this.Hide(); // this.Close(); // Application.Exit();
        }
    }
}
