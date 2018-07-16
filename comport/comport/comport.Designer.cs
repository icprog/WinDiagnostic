namespace comport
{
    partial class comport
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
            this.components = new System.ComponentModel.Container();
            this.labelTitle = new System.Windows.Forms.Label();
            this.groupComportDetails = new System.Windows.Forms.GroupBox();
            this.txtComportDetails = new System.Windows.Forms.TextBox();
            this.TimerSerialPortReceive = new System.Windows.Forms.Timer(this.components);
            this.groupSerialPort = new System.Windows.Forms.GroupBox();
            this.comboBoxSerialPorts = new System.Windows.Forms.ComboBox();
            this.buttonSerialPort = new System.Windows.Forms.Button();
            this.labelSerialPort4_Result = new System.Windows.Forms.Label();
            this.labelSerialPort3_Result = new System.Windows.Forms.Label();
            this.labelSerialPort2_Result = new System.Windows.Forms.Label();
            this.labelSerialPort1_Result = new System.Windows.Forms.Label();
            this.labelSerialPort4 = new System.Windows.Forms.Label();
            this.labelSerialPort3 = new System.Windows.Forms.Label();
            this.labelSerialPort2 = new System.Windows.Forms.Label();
            this.labelSerialPort1 = new System.Windows.Forms.Label();
            this.labelSerialPort4Topic = new System.Windows.Forms.Label();
            this.labelSerialPort3Topic = new System.Windows.Forms.Label();
            this.labelSerialPort2Topic = new System.Windows.Forms.Label();
            this.labelSerialPort1Topic = new System.Windows.Forms.Label();
            this.labelResultTopics = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.groupComportDetails.SuspendLayout();
            this.groupSerialPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 28F);
            this.labelTitle.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelTitle.Location = new System.Drawing.Point(13, 13);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(202, 53);
            this.labelTitle.TabIndex = 10;
            this.labelTitle.Text = "Comport";
            // 
            // groupComportDetails
            // 
            this.groupComportDetails.Controls.Add(this.txtComportDetails);
            this.groupComportDetails.Font = new System.Drawing.Font("Arial", 12F);
            this.groupComportDetails.Location = new System.Drawing.Point(24, 503);
            this.groupComportDetails.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupComportDetails.Name = "groupComportDetails";
            this.groupComportDetails.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupComportDetails.Size = new System.Drawing.Size(572, 300);
            this.groupComportDetails.TabIndex = 74;
            this.groupComportDetails.TabStop = false;
            this.groupComportDetails.Text = "Comport Details";
            // 
            // txtComportDetails
            // 
            this.txtComportDetails.Location = new System.Drawing.Point(13, 28);
            this.txtComportDetails.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtComportDetails.Multiline = true;
            this.txtComportDetails.Name = "txtComportDetails";
            this.txtComportDetails.ReadOnly = true;
            this.txtComportDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtComportDetails.Size = new System.Drawing.Size(549, 259);
            this.txtComportDetails.TabIndex = 9;
            // 
            // TimerSerialPortReceive
            // 
            this.TimerSerialPortReceive.Interval = 3000;
            this.TimerSerialPortReceive.Tick += new System.EventHandler(this.TimerSerialPortReceive_Tick);
            // 
            // groupSerialPort
            // 
            this.groupSerialPort.Controls.Add(this.comboBoxSerialPorts);
            this.groupSerialPort.Controls.Add(this.buttonSerialPort);
            this.groupSerialPort.Controls.Add(this.labelSerialPort4_Result);
            this.groupSerialPort.Controls.Add(this.labelSerialPort3_Result);
            this.groupSerialPort.Controls.Add(this.labelSerialPort2_Result);
            this.groupSerialPort.Controls.Add(this.labelSerialPort1_Result);
            this.groupSerialPort.Controls.Add(this.labelSerialPort4);
            this.groupSerialPort.Controls.Add(this.labelSerialPort3);
            this.groupSerialPort.Controls.Add(this.labelSerialPort2);
            this.groupSerialPort.Controls.Add(this.labelSerialPort1);
            this.groupSerialPort.Controls.Add(this.labelSerialPort4Topic);
            this.groupSerialPort.Controls.Add(this.labelSerialPort3Topic);
            this.groupSerialPort.Controls.Add(this.labelSerialPort2Topic);
            this.groupSerialPort.Controls.Add(this.labelSerialPort1Topic);
            this.groupSerialPort.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupSerialPort.Location = new System.Drawing.Point(24, 135);
            this.groupSerialPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupSerialPort.Name = "groupSerialPort";
            this.groupSerialPort.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupSerialPort.Size = new System.Drawing.Size(572, 360);
            this.groupSerialPort.TabIndex = 76;
            this.groupSerialPort.TabStop = false;
            this.groupSerialPort.Text = "Serial Port";
            this.groupSerialPort.Visible = false;
            // 
            // comboBoxSerialPorts
            // 
            this.comboBoxSerialPorts.Enabled = false;
            this.comboBoxSerialPorts.Font = new System.Drawing.Font("Arial", 18F);
            this.comboBoxSerialPorts.FormattingEnabled = true;
            this.comboBoxSerialPorts.Location = new System.Drawing.Point(223, 49);
            this.comboBoxSerialPorts.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxSerialPorts.Name = "comboBoxSerialPorts";
            this.comboBoxSerialPorts.Size = new System.Drawing.Size(215, 43);
            this.comboBoxSerialPorts.TabIndex = 78;
            this.comboBoxSerialPorts.Visible = false;
            // 
            // buttonSerialPort
            // 
            this.buttonSerialPort.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonSerialPort.ForeColor = System.Drawing.Color.Blue;
            this.buttonSerialPort.Location = new System.Drawing.Point(15, 33);
            this.buttonSerialPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSerialPort.Name = "buttonSerialPort";
            this.buttonSerialPort.Size = new System.Drawing.Size(200, 67);
            this.buttonSerialPort.TabIndex = 83;
            this.buttonSerialPort.Text = "Test";
            this.buttonSerialPort.UseVisualStyleBackColor = true;
            this.buttonSerialPort.Click += new System.EventHandler(this.buttonSerialPort_Click);
            // 
            // labelSerialPort4_Result
            // 
            this.labelSerialPort4_Result.AutoSize = true;
            this.labelSerialPort4_Result.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort4_Result.ForeColor = System.Drawing.Color.Blue;
            this.labelSerialPort4_Result.Location = new System.Drawing.Point(337, 301);
            this.labelSerialPort4_Result.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort4_Result.Name = "labelSerialPort4_Result";
            this.labelSerialPort4_Result.Size = new System.Drawing.Size(183, 45);
            this.labelSerialPort4_Result.TabIndex = 80;
            this.labelSerialPort4_Result.Text = "Unknown";
            this.labelSerialPort4_Result.Visible = false;
            // 
            // labelSerialPort3_Result
            // 
            this.labelSerialPort3_Result.AutoSize = true;
            this.labelSerialPort3_Result.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort3_Result.ForeColor = System.Drawing.Color.Blue;
            this.labelSerialPort3_Result.Location = new System.Drawing.Point(337, 241);
            this.labelSerialPort3_Result.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort3_Result.Name = "labelSerialPort3_Result";
            this.labelSerialPort3_Result.Size = new System.Drawing.Size(183, 45);
            this.labelSerialPort3_Result.TabIndex = 79;
            this.labelSerialPort3_Result.Text = "Unknown";
            this.labelSerialPort3_Result.Visible = false;
            // 
            // labelSerialPort2_Result
            // 
            this.labelSerialPort2_Result.AutoSize = true;
            this.labelSerialPort2_Result.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort2_Result.ForeColor = System.Drawing.Color.Blue;
            this.labelSerialPort2_Result.Location = new System.Drawing.Point(337, 180);
            this.labelSerialPort2_Result.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort2_Result.Name = "labelSerialPort2_Result";
            this.labelSerialPort2_Result.Size = new System.Drawing.Size(183, 45);
            this.labelSerialPort2_Result.TabIndex = 78;
            this.labelSerialPort2_Result.Text = "Unknown";
            this.labelSerialPort2_Result.Visible = false;
            // 
            // labelSerialPort1_Result
            // 
            this.labelSerialPort1_Result.AutoSize = true;
            this.labelSerialPort1_Result.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort1_Result.ForeColor = System.Drawing.Color.Blue;
            this.labelSerialPort1_Result.Location = new System.Drawing.Point(337, 121);
            this.labelSerialPort1_Result.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort1_Result.Name = "labelSerialPort1_Result";
            this.labelSerialPort1_Result.Size = new System.Drawing.Size(183, 45);
            this.labelSerialPort1_Result.TabIndex = 77;
            this.labelSerialPort1_Result.Text = "Unknown";
            this.labelSerialPort1_Result.Visible = false;
            // 
            // labelSerialPort4
            // 
            this.labelSerialPort4.AutoSize = true;
            this.labelSerialPort4.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort4.Location = new System.Drawing.Point(161, 301);
            this.labelSerialPort4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort4.Name = "labelSerialPort4";
            this.labelSerialPort4.Size = new System.Drawing.Size(86, 45);
            this.labelSerialPort4.TabIndex = 76;
            this.labelSerialPort4.Text = "Null";
            this.labelSerialPort4.Visible = false;
            // 
            // labelSerialPort3
            // 
            this.labelSerialPort3.AutoSize = true;
            this.labelSerialPort3.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort3.Location = new System.Drawing.Point(161, 241);
            this.labelSerialPort3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort3.Name = "labelSerialPort3";
            this.labelSerialPort3.Size = new System.Drawing.Size(86, 45);
            this.labelSerialPort3.TabIndex = 75;
            this.labelSerialPort3.Text = "Null";
            this.labelSerialPort3.Visible = false;
            // 
            // labelSerialPort2
            // 
            this.labelSerialPort2.AutoSize = true;
            this.labelSerialPort2.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort2.Location = new System.Drawing.Point(161, 180);
            this.labelSerialPort2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort2.Name = "labelSerialPort2";
            this.labelSerialPort2.Size = new System.Drawing.Size(86, 45);
            this.labelSerialPort2.TabIndex = 74;
            this.labelSerialPort2.Text = "Null";
            this.labelSerialPort2.Visible = false;
            // 
            // labelSerialPort1
            // 
            this.labelSerialPort1.AutoSize = true;
            this.labelSerialPort1.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort1.Location = new System.Drawing.Point(161, 121);
            this.labelSerialPort1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort1.Name = "labelSerialPort1";
            this.labelSerialPort1.Size = new System.Drawing.Size(86, 45);
            this.labelSerialPort1.TabIndex = 73;
            this.labelSerialPort1.Text = "Null";
            this.labelSerialPort1.Visible = false;
            // 
            // labelSerialPort4Topic
            // 
            this.labelSerialPort4Topic.AutoSize = true;
            this.labelSerialPort4Topic.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort4Topic.Location = new System.Drawing.Point(8, 301);
            this.labelSerialPort4Topic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort4Topic.Name = "labelSerialPort4Topic";
            this.labelSerialPort4Topic.Size = new System.Drawing.Size(137, 45);
            this.labelSerialPort4Topic.TabIndex = 72;
            this.labelSerialPort4Topic.Text = "Port4 :";
            this.labelSerialPort4Topic.Visible = false;
            // 
            // labelSerialPort3Topic
            // 
            this.labelSerialPort3Topic.AutoSize = true;
            this.labelSerialPort3Topic.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort3Topic.Location = new System.Drawing.Point(8, 241);
            this.labelSerialPort3Topic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort3Topic.Name = "labelSerialPort3Topic";
            this.labelSerialPort3Topic.Size = new System.Drawing.Size(137, 45);
            this.labelSerialPort3Topic.TabIndex = 71;
            this.labelSerialPort3Topic.Text = "Port3 :";
            this.labelSerialPort3Topic.Visible = false;
            // 
            // labelSerialPort2Topic
            // 
            this.labelSerialPort2Topic.AutoSize = true;
            this.labelSerialPort2Topic.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort2Topic.Location = new System.Drawing.Point(8, 180);
            this.labelSerialPort2Topic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort2Topic.Name = "labelSerialPort2Topic";
            this.labelSerialPort2Topic.Size = new System.Drawing.Size(137, 45);
            this.labelSerialPort2Topic.TabIndex = 70;
            this.labelSerialPort2Topic.Text = "Port2 :";
            this.labelSerialPort2Topic.Visible = false;
            // 
            // labelSerialPort1Topic
            // 
            this.labelSerialPort1Topic.AutoSize = true;
            this.labelSerialPort1Topic.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSerialPort1Topic.Location = new System.Drawing.Point(8, 119);
            this.labelSerialPort1Topic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSerialPort1Topic.Name = "labelSerialPort1Topic";
            this.labelSerialPort1Topic.Size = new System.Drawing.Size(137, 45);
            this.labelSerialPort1Topic.TabIndex = 69;
            this.labelSerialPort1Topic.Text = "Port1 :";
            this.labelSerialPort1Topic.Visible = false;
            // 
            // labelResultTopics
            // 
            this.labelResultTopics.AutoSize = true;
            this.labelResultTopics.Font = new System.Drawing.Font("Arial", 24F);
            this.labelResultTopics.Location = new System.Drawing.Point(16, 79);
            this.labelResultTopics.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResultTopics.Name = "labelResultTopics";
            this.labelResultTopics.Size = new System.Drawing.Size(163, 45);
            this.labelResultTopics.TabIndex = 149;
            this.labelResultTopics.Text = "Result : ";
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.labelResult.ForeColor = System.Drawing.Color.Blue;
            this.labelResult.Location = new System.Drawing.Point(200, 71);
            this.labelResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(259, 55);
            this.labelResult.TabIndex = 148;
            this.labelResult.Text = "Not Result";
            // 
            // comport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(909, 809);
            this.Controls.Add(this.labelResultTopics);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.groupSerialPort);
            this.Controls.Add(this.groupComportDetails);
            this.Controls.Add(this.labelTitle);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "comport";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.comport_Load);
            this.groupComportDetails.ResumeLayout(false);
            this.groupComportDetails.PerformLayout();
            this.groupSerialPort.ResumeLayout(false);
            this.groupSerialPort.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.GroupBox groupComportDetails;
        private System.Windows.Forms.TextBox txtComportDetails;
        private System.Windows.Forms.Timer TimerSerialPortReceive;
        private System.Windows.Forms.GroupBox groupSerialPort;
        private System.Windows.Forms.ComboBox comboBoxSerialPorts;
        private System.Windows.Forms.Button buttonSerialPort;
        private System.Windows.Forms.Label labelSerialPort4_Result;
        private System.Windows.Forms.Label labelSerialPort3_Result;
        private System.Windows.Forms.Label labelSerialPort2_Result;
        private System.Windows.Forms.Label labelSerialPort1_Result;
        private System.Windows.Forms.Label labelSerialPort4;
        private System.Windows.Forms.Label labelSerialPort3;
        private System.Windows.Forms.Label labelSerialPort2;
        private System.Windows.Forms.Label labelSerialPort1;
        private System.Windows.Forms.Label labelSerialPort4Topic;
        private System.Windows.Forms.Label labelSerialPort3Topic;
        private System.Windows.Forms.Label labelSerialPort2Topic;
        private System.Windows.Forms.Label labelSerialPort1Topic;
        private System.Windows.Forms.Label labelResultTopics;
        private System.Windows.Forms.Label labelResult;
    }
}

