namespace barcode
{
    partial class barcode
    {
        /// <summary> 
        /// 設計工具所需的變數. 
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源. 
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true, 否則為 false. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容. 
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonFAIL = new System.Windows.Forms.Button();
            this.buttonPASS = new System.Windows.Forms.Button();
            this.buttonBarcodeDisconnect = new System.Windows.Forms.Button();
            this.buttonBarcodeConnect = new System.Windows.Forms.Button();
            this.gbMsgList = new System.Windows.Forms.GroupBox();
            this.bnCopyMsgList = new System.Windows.Forms.Button();
            this.rtxMsgList = new System.Windows.Forms.RichTextBox();
            this.gbReceiveMsg = new System.Windows.Forms.GroupBox();
            this.textBoxReceiveTag = new System.Windows.Forms.RichTextBox();
            this.bnCopyRecMsg = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblVersionString = new System.Windows.Forms.Label();
            this.buttonScan = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelResultTopics = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.gbMsgList.SuspendLayout();
            this.gbReceiveMsg.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonFAIL
            // 
            this.buttonFAIL.Enabled = false;
            this.buttonFAIL.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonFAIL.ForeColor = System.Drawing.Color.Red;
            this.buttonFAIL.Location = new System.Drawing.Point(174, 157);
            this.buttonFAIL.Name = "buttonFAIL";
            this.buttonFAIL.Size = new System.Drawing.Size(150, 50);
            this.buttonFAIL.TabIndex = 78;
            this.buttonFAIL.Text = "FAIL";
            this.buttonFAIL.UseVisualStyleBackColor = true;
            this.buttonFAIL.Click += new System.EventHandler(this.buttonFAIL_Click);
            // 
            // buttonPASS
            // 
            this.buttonPASS.Enabled = false;
            this.buttonPASS.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonPASS.ForeColor = System.Drawing.Color.Green;
            this.buttonPASS.Location = new System.Drawing.Point(18, 157);
            this.buttonPASS.Name = "buttonPASS";
            this.buttonPASS.Size = new System.Drawing.Size(150, 50);
            this.buttonPASS.TabIndex = 77;
            this.buttonPASS.Text = "PASS";
            this.buttonPASS.UseVisualStyleBackColor = true;
            this.buttonPASS.Click += new System.EventHandler(this.buttonPASS_Click);
            // 
            // buttonBarcodeDisconnect
            // 
            this.buttonBarcodeDisconnect.Enabled = false;
            this.buttonBarcodeDisconnect.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.buttonBarcodeDisconnect.Location = new System.Drawing.Point(174, 219);
            this.buttonBarcodeDisconnect.Name = "buttonBarcodeDisconnect";
            this.buttonBarcodeDisconnect.Size = new System.Drawing.Size(150, 50);
            this.buttonBarcodeDisconnect.TabIndex = 80;
            this.buttonBarcodeDisconnect.Text = "Disconnect";
            this.buttonBarcodeDisconnect.Visible = false;
            this.buttonBarcodeDisconnect.Click += new System.EventHandler(this.buttonBarcodeDisconnect_Click);
            // 
            // buttonBarcodeConnect
            // 
            this.buttonBarcodeConnect.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.buttonBarcodeConnect.Location = new System.Drawing.Point(18, 219);
            this.buttonBarcodeConnect.Name = "buttonBarcodeConnect";
            this.buttonBarcodeConnect.Size = new System.Drawing.Size(150, 50);
            this.buttonBarcodeConnect.TabIndex = 79;
            this.buttonBarcodeConnect.Text = "Connect";
            this.buttonBarcodeConnect.Visible = false;
            this.buttonBarcodeConnect.Click += new System.EventHandler(this.buttonBarcodeConnect_Click);
            // 
            // gbMsgList
            // 
            this.gbMsgList.Controls.Add(this.bnCopyMsgList);
            this.gbMsgList.Controls.Add(this.rtxMsgList);
            this.gbMsgList.Font = new System.Drawing.Font("Arial", 12F);
            this.gbMsgList.Location = new System.Drawing.Point(346, 271);
            this.gbMsgList.Name = "gbMsgList";
            this.gbMsgList.Size = new System.Drawing.Size(318, 234);
            this.gbMsgList.TabIndex = 82;
            this.gbMsgList.TabStop = false;
            this.gbMsgList.Text = "Message List";
            this.gbMsgList.Visible = false;
            // 
            // bnCopyMsgList
            // 
            this.bnCopyMsgList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCopyMsgList.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.bnCopyMsgList.Location = new System.Drawing.Point(157, 178);
            this.bnCopyMsgList.Name = "bnCopyMsgList";
            this.bnCopyMsgList.Size = new System.Drawing.Size(150, 50);
            this.bnCopyMsgList.TabIndex = 6;
            this.bnCopyMsgList.Text = "Copy";
            this.bnCopyMsgList.UseVisualStyleBackColor = true;
            this.bnCopyMsgList.Click += new System.EventHandler(this.bnCopyMsgList_Click);
            // 
            // rtxMsgList
            // 
            this.rtxMsgList.BackColor = System.Drawing.SystemColors.Control;
            this.rtxMsgList.Font = new System.Drawing.Font("Arial", 12F);
            this.rtxMsgList.Location = new System.Drawing.Point(7, 21);
            this.rtxMsgList.Name = "rtxMsgList";
            this.rtxMsgList.ReadOnly = true;
            this.rtxMsgList.Size = new System.Drawing.Size(300, 150);
            this.rtxMsgList.TabIndex = 5;
            this.rtxMsgList.Text = "";
            // 
            // gbReceiveMsg
            // 
            this.gbReceiveMsg.Controls.Add(this.textBoxReceiveTag);
            this.gbReceiveMsg.Controls.Add(this.bnCopyRecMsg);
            this.gbReceiveMsg.Font = new System.Drawing.Font("Arial", 12F);
            this.gbReceiveMsg.Location = new System.Drawing.Point(18, 271);
            this.gbReceiveMsg.Name = "gbReceiveMsg";
            this.gbReceiveMsg.Size = new System.Drawing.Size(318, 234);
            this.gbReceiveMsg.TabIndex = 81;
            this.gbReceiveMsg.TabStop = false;
            this.gbReceiveMsg.Text = "Receive Message";
            // 
            // textBoxReceiveTag
            // 
            this.textBoxReceiveTag.Font = new System.Drawing.Font("Arial", 12F);
            this.textBoxReceiveTag.Location = new System.Drawing.Point(6, 22);
            this.textBoxReceiveTag.Name = "textBoxReceiveTag";
            this.textBoxReceiveTag.Size = new System.Drawing.Size(300, 150);
            this.textBoxReceiveTag.TabIndex = 3;
            this.textBoxReceiveTag.Text = "";
            // 
            // bnCopyRecMsg
            // 
            this.bnCopyRecMsg.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bnCopyRecMsg.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.bnCopyRecMsg.Location = new System.Drawing.Point(156, 178);
            this.bnCopyRecMsg.Name = "bnCopyRecMsg";
            this.bnCopyRecMsg.Size = new System.Drawing.Size(150, 50);
            this.bnCopyRecMsg.TabIndex = 4;
            this.bnCopyRecMsg.Text = "Copy";
            this.bnCopyRecMsg.UseVisualStyleBackColor = true;
            this.bnCopyRecMsg.Visible = false;
            this.bnCopyRecMsg.Click += new System.EventHandler(this.bnCopyRecMsg_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Arial", 24F);
            this.lblVersion.Location = new System.Drawing.Point(330, 165);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(138, 36);
            this.lblVersion.TabIndex = 84;
            this.lblVersion.Text = "Version :";
            this.lblVersion.Visible = false;
            // 
            // lblVersionString
            // 
            this.lblVersionString.AutoSize = true;
            this.lblVersionString.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.lblVersionString.ForeColor = System.Drawing.Color.Blue;
            this.lblVersionString.Location = new System.Drawing.Point(474, 159);
            this.lblVersionString.Name = "lblVersionString";
            this.lblVersionString.Size = new System.Drawing.Size(210, 45);
            this.lblVersionString.TabIndex = 83;
            this.lblVersionString.Text = "Not Result";
            this.lblVersionString.Visible = false;
            // 
            // buttonScan
            // 
            this.buttonScan.Enabled = false;
            this.buttonScan.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonScan.ForeColor = System.Drawing.Color.Blue;
            this.buttonScan.Location = new System.Drawing.Point(18, 101);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(150, 50);
            this.buttonScan.TabIndex = 85;
            this.buttonScan.Text = "Scan";
            this.buttonScan.Click += new System.EventHandler(this.buttonScan_Click);
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 28F);
            this.labelTitle.Location = new System.Drawing.Point(10, 10);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(159, 43);
            this.labelTitle.TabIndex = 86;
            this.labelTitle.Text = "Barcode";
            // 
            // labelResultTopics
            // 
            this.labelResultTopics.AutoSize = true;
            this.labelResultTopics.Font = new System.Drawing.Font("Arial", 24F);
            this.labelResultTopics.Location = new System.Drawing.Point(12, 59);
            this.labelResultTopics.Name = "labelResultTopics";
            this.labelResultTopics.Size = new System.Drawing.Size(132, 36);
            this.labelResultTopics.TabIndex = 101;
            this.labelResultTopics.Text = "Result : ";
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.labelResult.ForeColor = System.Drawing.Color.Blue;
            this.labelResult.Location = new System.Drawing.Point(150, 53);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(210, 45);
            this.labelResult.TabIndex = 100;
            this.labelResult.Text = "Not Result";
            // 
            // Barcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.labelResultTopics);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.buttonScan);
            this.Controls.Add(this.buttonBarcodeConnect);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblVersionString);
            this.Controls.Add(this.gbMsgList);
            this.Controls.Add(this.gbReceiveMsg);
            this.Controls.Add(this.buttonBarcodeDisconnect);
            this.Controls.Add(this.buttonFAIL);
            this.Controls.Add(this.buttonPASS);
            this.DoubleBuffered = true;
            this.Name = "Barcode";
            this.Size = new System.Drawing.Size(700, 597);
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Barcode_Load);
            this.gbMsgList.ResumeLayout(false);
            this.gbReceiveMsg.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonFAIL;
        private System.Windows.Forms.Button buttonPASS;
        private System.Windows.Forms.Button buttonBarcodeDisconnect;
        private System.Windows.Forms.Button buttonBarcodeConnect;
        private System.Windows.Forms.GroupBox gbMsgList;
        private System.Windows.Forms.Button bnCopyMsgList;
        private System.Windows.Forms.RichTextBox rtxMsgList;
        private System.Windows.Forms.GroupBox gbReceiveMsg;
        private System.Windows.Forms.RichTextBox textBoxReceiveTag;
        private System.Windows.Forms.Button bnCopyRecMsg;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblVersionString;
        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelResultTopics;
        private System.Windows.Forms.Label labelResult;
    }
}

