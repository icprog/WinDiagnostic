namespace lan
{
    partial class lan
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonConnectEthernet1 = new System.Windows.Forms.Button();
            this.groupLANDetails = new System.Windows.Forms.GroupBox();
            this.txtLANDetails = new System.Windows.Forms.TextBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelPingAddressTopic = new System.Windows.Forms.Label();
            this.textBoxPingAddress = new System.Windows.Forms.TextBox();
            this.buttonClearPingAddress = new System.Windows.Forms.Button();
            this.groupBoxEthernet1 = new System.Windows.Forms.GroupBox();
            this.labelEthernet1_Result = new System.Windows.Forms.Label();
            this.labelEthernet1 = new System.Windows.Forms.Label();
            this.groupBoxEthernet2 = new System.Windows.Forms.GroupBox();
            this.labelEthernet2_Result = new System.Windows.Forms.Label();
            this.labelEthernet2 = new System.Windows.Forms.Label();
            this.buttonConnectEthernet2 = new System.Windows.Forms.Button();
            this.labelResultTopics = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.groupLANDetails.SuspendLayout();
            this.groupBoxEthernet1.SuspendLayout();
            this.groupBoxEthernet2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonConnectEthernet1
            // 
            this.buttonConnectEthernet1.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.buttonConnectEthernet1.Location = new System.Drawing.Point(499, 19);
            this.buttonConnectEthernet1.Name = "buttonConnectEthernet1";
            this.buttonConnectEthernet1.Size = new System.Drawing.Size(150, 50);
            this.buttonConnectEthernet1.TabIndex = 3;
            this.buttonConnectEthernet1.Text = "Connect";
            this.buttonConnectEthernet1.UseVisualStyleBackColor = true;
            this.buttonConnectEthernet1.Click += new System.EventHandler(this.buttonConnectEthernet1_Click);
            // 
            // groupLANDetails
            // 
            this.groupLANDetails.Controls.Add(this.txtLANDetails);
            this.groupLANDetails.Font = new System.Drawing.Font("Arial", 14.25F);
            this.groupLANDetails.Location = new System.Drawing.Point(14, 321);
            this.groupLANDetails.Name = "groupLANDetails";
            this.groupLANDetails.Size = new System.Drawing.Size(651, 277);
            this.groupLANDetails.TabIndex = 46;
            this.groupLANDetails.TabStop = false;
            this.groupLANDetails.Text = "LAN Details";
            // 
            // txtLANDetails
            // 
            this.txtLANDetails.Font = new System.Drawing.Font("新細明體", 12F);
            this.txtLANDetails.Location = new System.Drawing.Point(10, 29);
            this.txtLANDetails.Multiline = true;
            this.txtLANDetails.Name = "txtLANDetails";
            this.txtLANDetails.ReadOnly = true;
            this.txtLANDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLANDetails.Size = new System.Drawing.Size(635, 242);
            this.txtLANDetails.TabIndex = 9;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 28F);
            this.labelTitle.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelTitle.Location = new System.Drawing.Point(10, 10);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(92, 43);
            this.labelTitle.TabIndex = 51;
            this.labelTitle.Text = "LAN";
            // 
            // labelPingAddressTopic
            // 
            this.labelPingAddressTopic.AutoSize = true;
            this.labelPingAddressTopic.Font = new System.Drawing.Font("Arial", 21.75F, System.Drawing.FontStyle.Bold);
            this.labelPingAddressTopic.Location = new System.Drawing.Point(12, 107);
            this.labelPingAddressTopic.Name = "labelPingAddressTopic";
            this.labelPingAddressTopic.Size = new System.Drawing.Size(199, 34);
            this.labelPingAddressTopic.TabIndex = 52;
            this.labelPingAddressTopic.Text = "Ping Address";
            // 
            // textBoxPingAddress
            // 
            this.textBoxPingAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPingAddress.Location = new System.Drawing.Point(215, 104);
            this.textBoxPingAddress.Name = "textBoxPingAddress";
            this.textBoxPingAddress.Size = new System.Drawing.Size(312, 40);
            this.textBoxPingAddress.TabIndex = 53;
            // 
            // buttonClearPingAddress
            // 
            this.buttonClearPingAddress.Enabled = false;
            this.buttonClearPingAddress.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.buttonClearPingAddress.Location = new System.Drawing.Point(533, 100);
            this.buttonClearPingAddress.Name = "buttonClearPingAddress";
            this.buttonClearPingAddress.Size = new System.Drawing.Size(150, 50);
            this.buttonClearPingAddress.TabIndex = 54;
            this.buttonClearPingAddress.Text = "Clear";
            this.buttonClearPingAddress.UseVisualStyleBackColor = true;
            this.buttonClearPingAddress.Click += new System.EventHandler(this.buttonClearPingAddress_Click);
            // 
            // groupBoxEthernet1
            // 
            this.groupBoxEthernet1.Controls.Add(this.labelEthernet1_Result);
            this.groupBoxEthernet1.Controls.Add(this.labelEthernet1);
            this.groupBoxEthernet1.Controls.Add(this.buttonConnectEthernet1);
            this.groupBoxEthernet1.Font = new System.Drawing.Font("Arial", 12F);
            this.groupBoxEthernet1.Location = new System.Drawing.Point(16, 155);
            this.groupBoxEthernet1.Name = "groupBoxEthernet1";
            this.groupBoxEthernet1.Size = new System.Drawing.Size(664, 79);
            this.groupBoxEthernet1.TabIndex = 55;
            this.groupBoxEthernet1.TabStop = false;
            this.groupBoxEthernet1.Text = "LAN1";
            // 
            // labelEthernet1_Result
            // 
            this.labelEthernet1_Result.AutoSize = true;
            this.labelEthernet1_Result.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEthernet1_Result.ForeColor = System.Drawing.Color.Blue;
            this.labelEthernet1_Result.Location = new System.Drawing.Point(366, 30);
            this.labelEthernet1_Result.Name = "labelEthernet1_Result";
            this.labelEthernet1_Result.Size = new System.Drawing.Size(127, 32);
            this.labelEthernet1_Result.TabIndex = 67;
            this.labelEthernet1_Result.Text = "Unknown";
            // 
            // labelEthernet1
            // 
            this.labelEthernet1.AutoSize = true;
            this.labelEthernet1.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEthernet1.Location = new System.Drawing.Point(4, 30);
            this.labelEthernet1.Name = "labelEthernet1";
            this.labelEthernet1.Size = new System.Drawing.Size(61, 32);
            this.labelEthernet1.TabIndex = 66;
            this.labelEthernet1.Text = "Null";
            // 
            // groupBoxEthernet2
            // 
            this.groupBoxEthernet2.Controls.Add(this.labelEthernet2_Result);
            this.groupBoxEthernet2.Controls.Add(this.labelEthernet2);
            this.groupBoxEthernet2.Controls.Add(this.buttonConnectEthernet2);
            this.groupBoxEthernet2.Font = new System.Drawing.Font("Arial", 12F);
            this.groupBoxEthernet2.Location = new System.Drawing.Point(16, 236);
            this.groupBoxEthernet2.Name = "groupBoxEthernet2";
            this.groupBoxEthernet2.Size = new System.Drawing.Size(664, 79);
            this.groupBoxEthernet2.TabIndex = 56;
            this.groupBoxEthernet2.TabStop = false;
            this.groupBoxEthernet2.Text = "LAN2";
            this.groupBoxEthernet2.Visible = false;
            // 
            // labelEthernet2_Result
            // 
            this.labelEthernet2_Result.AutoSize = true;
            this.labelEthernet2_Result.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEthernet2_Result.ForeColor = System.Drawing.Color.Blue;
            this.labelEthernet2_Result.Location = new System.Drawing.Point(366, 31);
            this.labelEthernet2_Result.Name = "labelEthernet2_Result";
            this.labelEthernet2_Result.Size = new System.Drawing.Size(127, 32);
            this.labelEthernet2_Result.TabIndex = 67;
            this.labelEthernet2_Result.Text = "Unknown";
            // 
            // labelEthernet2
            // 
            this.labelEthernet2.AutoSize = true;
            this.labelEthernet2.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEthernet2.Location = new System.Drawing.Point(4, 31);
            this.labelEthernet2.Name = "labelEthernet2";
            this.labelEthernet2.Size = new System.Drawing.Size(61, 32);
            this.labelEthernet2.TabIndex = 66;
            this.labelEthernet2.Text = "Null";
            // 
            // buttonConnectEthernet2
            // 
            this.buttonConnectEthernet2.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.buttonConnectEthernet2.Location = new System.Drawing.Point(499, 19);
            this.buttonConnectEthernet2.Name = "buttonConnectEthernet2";
            this.buttonConnectEthernet2.Size = new System.Drawing.Size(150, 50);
            this.buttonConnectEthernet2.TabIndex = 3;
            this.buttonConnectEthernet2.Text = "Connect";
            this.buttonConnectEthernet2.UseVisualStyleBackColor = true;
            this.buttonConnectEthernet2.Click += new System.EventHandler(this.buttonConnectEthernet2_Click);
            // 
            // labelResultTopics
            // 
            this.labelResultTopics.AutoSize = true;
            this.labelResultTopics.Font = new System.Drawing.Font("Arial", 24F);
            this.labelResultTopics.Location = new System.Drawing.Point(10, 58);
            this.labelResultTopics.Name = "labelResultTopics";
            this.labelResultTopics.Size = new System.Drawing.Size(132, 36);
            this.labelResultTopics.TabIndex = 99;
            this.labelResultTopics.Text = "Result : ";
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.labelResult.ForeColor = System.Drawing.Color.Blue;
            this.labelResult.Location = new System.Drawing.Point(148, 52);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(210, 45);
            this.labelResult.TabIndex = 98;
            this.labelResult.Text = "Not Result";
            // 
            // lan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(704, 601);
            this.Controls.Add(this.labelResultTopics);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.groupBoxEthernet2);
            this.Controls.Add(this.groupBoxEthernet1);
            this.Controls.Add(this.buttonClearPingAddress);
            this.Controls.Add(this.textBoxPingAddress);
            this.Controls.Add(this.labelPingAddressTopic);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.groupLANDetails);
            this.DoubleBuffered = true;
            this.Name = "lan";
            this.Load += new System.EventHandler(this.LAN_Load);
            this.groupLANDetails.ResumeLayout(false);
            this.groupLANDetails.PerformLayout();
            this.groupBoxEthernet1.ResumeLayout(false);
            this.groupBoxEthernet1.PerformLayout();
            this.groupBoxEthernet2.ResumeLayout(false);
            this.groupBoxEthernet2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonConnectEthernet1;
        private System.Windows.Forms.GroupBox groupLANDetails;
        private System.Windows.Forms.TextBox txtLANDetails;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelPingAddressTopic;
        private System.Windows.Forms.TextBox textBoxPingAddress;
        private System.Windows.Forms.Button buttonClearPingAddress;
        private System.Windows.Forms.GroupBox groupBoxEthernet1;
        private System.Windows.Forms.Label labelEthernet1_Result;
        private System.Windows.Forms.Label labelEthernet1;
        private System.Windows.Forms.GroupBox groupBoxEthernet2;
        private System.Windows.Forms.Label labelEthernet2_Result;
        private System.Windows.Forms.Label labelEthernet2;
        private System.Windows.Forms.Button buttonConnectEthernet2;
        private System.Windows.Forms.Label labelResultTopics;
        private System.Windows.Forms.Label labelResult;
    }
}

