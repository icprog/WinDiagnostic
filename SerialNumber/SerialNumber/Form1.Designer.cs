namespace SerialNumber
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
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
            this.components = new System.ComponentModel.Container();
            this.TimerCheckInputStatus = new System.Windows.Forms.Timer(this.components);
            this.labelSN = new System.Windows.Forms.Label();
            this.textBoxSN = new System.Windows.Forms.TextBox();
            this.buttonClearSN = new System.Windows.Forms.Button();
            this.groupBoxSN = new System.Windows.Forms.GroupBox();
            this.labelAutoFail = new System.Windows.Forms.Label();
            this.textBoxAutoTestItemFail = new System.Windows.Forms.TextBox();
            this.labelAutoPass = new System.Windows.Forms.Label();
            this.textBoxAutoTestItemPass = new System.Windows.Forms.TextBox();
            this.buttonBurnIn = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.groupBoxAlarm = new System.Windows.Forms.GroupBox();
            this.buttonExportKey = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.TimerBackgroundTwinkled = new System.Windows.Forms.Timer(this.components);
            this.buttonStart = new System.Windows.Forms.Button();
            this.labelLSN = new System.Windows.Forms.Label();
            this.lOperator = new System.Windows.Forms.Label();
            this.textBoxLSN = new System.Windows.Forms.TextBox();
            this.textBoxOperator = new System.Windows.Forms.TextBox();
            this.buttonClearLSN = new System.Windows.Forms.Button();
            this.buttonClearOperator = new System.Windows.Forms.Button();
            this.groupBoxOperator = new System.Windows.Forms.GroupBox();
            this.groupBoxLSN = new System.Windows.Forms.GroupBox();
            this.groupBoxSN.SuspendLayout();
            this.groupBoxAlarm.SuspendLayout();
            this.groupBoxOperator.SuspendLayout();
            this.groupBoxLSN.SuspendLayout();
            this.SuspendLayout();
            // 
            // TimerCheckInputStatus
            // 
            this.TimerCheckInputStatus.Enabled = true;
            this.TimerCheckInputStatus.Interval = 330;
            // 
            // labelSN
            // 
            this.labelSN.AutoSize = true;
            this.labelSN.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.labelSN.Location = new System.Drawing.Point(128, 25);
            this.labelSN.Name = "labelSN";
            this.labelSN.Size = new System.Drawing.Size(47, 29);
            this.labelSN.TabIndex = 6;
            this.labelSN.Text = "SN";
            // 
            // textBoxSN
            // 
            this.textBoxSN.Font = new System.Drawing.Font("Arial", 18F);
            this.textBoxSN.Location = new System.Drawing.Point(181, 24);
            this.textBoxSN.Name = "textBoxSN";
            this.textBoxSN.Size = new System.Drawing.Size(207, 35);
            this.textBoxSN.TabIndex = 3;
            // 
            // buttonClearSN
            // 
            this.buttonClearSN.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonClearSN.Location = new System.Drawing.Point(406, 15);
            this.buttonClearSN.Name = "buttonClearSN";
            this.buttonClearSN.Size = new System.Drawing.Size(150, 50);
            this.buttonClearSN.TabIndex = 6;
            this.buttonClearSN.Text = "Clear";
            this.buttonClearSN.UseVisualStyleBackColor = true;
            this.buttonClearSN.Click += new System.EventHandler(this.buttonClearData_Click);
            // 
            // groupBoxSN
            // 
            this.groupBoxSN.Controls.Add(this.labelSN);
            this.groupBoxSN.Controls.Add(this.textBoxSN);
            this.groupBoxSN.Controls.Add(this.buttonClearSN);
            this.groupBoxSN.Font = new System.Drawing.Font("Arial", 14F);
            this.groupBoxSN.Location = new System.Drawing.Point(124, 174);
            this.groupBoxSN.Name = "groupBoxSN";
            this.groupBoxSN.Size = new System.Drawing.Size(560, 70);
            this.groupBoxSN.TabIndex = 153;
            this.groupBoxSN.TabStop = false;
            this.groupBoxSN.Text = "Serial Number (Optional)";
            // 
            // labelAutoFail
            // 
            this.labelAutoFail.AutoSize = true;
            this.labelAutoFail.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAutoFail.ForeColor = System.Drawing.Color.Red;
            this.labelAutoFail.Location = new System.Drawing.Point(6, 82);
            this.labelAutoFail.Name = "labelAutoFail";
            this.labelAutoFail.Size = new System.Drawing.Size(263, 29);
            this.labelAutoFail.TabIndex = 11;
            this.labelAutoFail.Text = "AutoTest Items - FAIL";
            // 
            // textBoxAutoTestItemFail
            // 
            this.textBoxAutoTestItemFail.Enabled = false;
            this.textBoxAutoTestItemFail.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAutoTestItemFail.Location = new System.Drawing.Point(6, 114);
            this.textBoxAutoTestItemFail.Name = "textBoxAutoTestItemFail";
            this.textBoxAutoTestItemFail.Size = new System.Drawing.Size(548, 29);
            this.textBoxAutoTestItemFail.TabIndex = 12;
            // 
            // labelAutoPass
            // 
            this.labelAutoPass.AutoSize = true;
            this.labelAutoPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAutoPass.ForeColor = System.Drawing.Color.Green;
            this.labelAutoPass.Location = new System.Drawing.Point(4, 18);
            this.labelAutoPass.Name = "labelAutoPass";
            this.labelAutoPass.Size = new System.Drawing.Size(277, 29);
            this.labelAutoPass.TabIndex = 9;
            this.labelAutoPass.Text = "AutoTest Items - PASS";
            this.labelAutoPass.Visible = false;
            // 
            // textBoxAutoTestItemPass
            // 
            this.textBoxAutoTestItemPass.Enabled = false;
            this.textBoxAutoTestItemPass.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAutoTestItemPass.Location = new System.Drawing.Point(6, 50);
            this.textBoxAutoTestItemPass.Name = "textBoxAutoTestItemPass";
            this.textBoxAutoTestItemPass.Size = new System.Drawing.Size(548, 29);
            this.textBoxAutoTestItemPass.TabIndex = 10;
            this.textBoxAutoTestItemPass.Visible = false;
            // 
            // buttonBurnIn
            // 
            this.buttonBurnIn.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonBurnIn.ForeColor = System.Drawing.Color.Blue;
            this.buttonBurnIn.Location = new System.Drawing.Point(123, 250);
            this.buttonBurnIn.Name = "buttonBurnIn";
            this.buttonBurnIn.Size = new System.Drawing.Size(400, 50);
            this.buttonBurnIn.TabIndex = 155;
            this.buttonBurnIn.Text = "Skip to run burn-in test";
            this.buttonBurnIn.UseVisualStyleBackColor = true;
            this.buttonBurnIn.Visible = false;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 28F);
            this.labelTitle.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelTitle.Location = new System.Drawing.Point(116, -65);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(262, 43);
            this.labelTitle.TabIndex = 159;
            this.labelTitle.Text = "Serial Number";
            // 
            // groupBoxAlarm
            // 
            this.groupBoxAlarm.Controls.Add(this.labelAutoFail);
            this.groupBoxAlarm.Controls.Add(this.textBoxAutoTestItemFail);
            this.groupBoxAlarm.Controls.Add(this.labelAutoPass);
            this.groupBoxAlarm.Controls.Add(this.textBoxAutoTestItemPass);
            this.groupBoxAlarm.Font = new System.Drawing.Font("Arial", 14F);
            this.groupBoxAlarm.Location = new System.Drawing.Point(124, 362);
            this.groupBoxAlarm.Name = "groupBoxAlarm";
            this.groupBoxAlarm.Size = new System.Drawing.Size(560, 153);
            this.groupBoxAlarm.TabIndex = 158;
            this.groupBoxAlarm.TabStop = false;
            this.groupBoxAlarm.Text = "AutoTest Alarm";
            this.groupBoxAlarm.Visible = false;
            // 
            // buttonExportKey
            // 
            this.buttonExportKey.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonExportKey.Location = new System.Drawing.Point(373, 306);
            this.buttonExportKey.Name = "buttonExportKey";
            this.buttonExportKey.Size = new System.Drawing.Size(150, 50);
            this.buttonExportKey.TabIndex = 157;
            this.buttonExportKey.Text = "Export Key";
            this.buttonExportKey.UseVisualStyleBackColor = true;
            this.buttonExportKey.Visible = false;
            // 
            // buttonExit
            // 
            this.buttonExit.Enabled = false;
            this.buttonExit.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonExit.Location = new System.Drawing.Point(529, 306);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(150, 50);
            this.buttonExit.TabIndex = 156;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // TimerBackgroundTwinkled
            // 
            this.TimerBackgroundTwinkled.Interval = 1000;
            // 
            // buttonStart
            // 
            this.buttonStart.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonStart.Location = new System.Drawing.Point(529, 250);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(150, 50);
            this.buttonStart.TabIndex = 154;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // labelLSN
            // 
            this.labelLSN.AutoSize = true;
            this.labelLSN.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.labelLSN.Location = new System.Drawing.Point(114, 25);
            this.labelLSN.Name = "labelLSN";
            this.labelLSN.Size = new System.Drawing.Size(60, 29);
            this.labelLSN.TabIndex = 6;
            this.labelLSN.Text = "LSN";
            // 
            // lOperator
            // 
            this.lOperator.AutoSize = true;
            this.lOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.lOperator.Location = new System.Drawing.Point(66, 24);
            this.lOperator.Name = "lOperator";
            this.lOperator.Size = new System.Drawing.Size(109, 29);
            this.lOperator.TabIndex = 7;
            this.lOperator.Text = "Operator";
            // 
            // textBoxLSN
            // 
            this.textBoxLSN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.textBoxLSN.Font = new System.Drawing.Font("Arial", 18F);
            this.textBoxLSN.Location = new System.Drawing.Point(181, 24);
            this.textBoxLSN.Name = "textBoxLSN";
            this.textBoxLSN.Size = new System.Drawing.Size(207, 35);
            this.textBoxLSN.TabIndex = 1;
            // 
            // textBoxOperator
            // 
            this.textBoxOperator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.textBoxOperator.Font = new System.Drawing.Font("Arial", 18F);
            this.textBoxOperator.Location = new System.Drawing.Point(181, 24);
            this.textBoxOperator.Name = "textBoxOperator";
            this.textBoxOperator.Size = new System.Drawing.Size(207, 35);
            this.textBoxOperator.TabIndex = 2;
            // 
            // buttonClearLSN
            // 
            this.buttonClearLSN.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonClearLSN.Location = new System.Drawing.Point(406, 15);
            this.buttonClearLSN.Name = "buttonClearLSN";
            this.buttonClearLSN.Size = new System.Drawing.Size(150, 50);
            this.buttonClearLSN.TabIndex = 4;
            this.buttonClearLSN.Text = "Clear";
            this.buttonClearLSN.UseVisualStyleBackColor = true;
            this.buttonClearLSN.Click += new System.EventHandler(this.buttonClearData_Click);
            // 
            // buttonClearOperator
            // 
            this.buttonClearOperator.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonClearOperator.Location = new System.Drawing.Point(406, 15);
            this.buttonClearOperator.Name = "buttonClearOperator";
            this.buttonClearOperator.Size = new System.Drawing.Size(150, 50);
            this.buttonClearOperator.TabIndex = 5;
            this.buttonClearOperator.Text = "Clear";
            this.buttonClearOperator.UseVisualStyleBackColor = true;
            this.buttonClearOperator.Click += new System.EventHandler(this.buttonClearData_Click);
            // 
            // groupBoxOperator
            // 
            this.groupBoxOperator.Controls.Add(this.lOperator);
            this.groupBoxOperator.Controls.Add(this.textBoxOperator);
            this.groupBoxOperator.Controls.Add(this.buttonClearOperator);
            this.groupBoxOperator.Font = new System.Drawing.Font("Arial", 14F);
            this.groupBoxOperator.Location = new System.Drawing.Point(124, 98);
            this.groupBoxOperator.Name = "groupBoxOperator";
            this.groupBoxOperator.Size = new System.Drawing.Size(560, 70);
            this.groupBoxOperator.TabIndex = 152;
            this.groupBoxOperator.TabStop = false;
            this.groupBoxOperator.Text = "Operator (Required)";
            // 
            // groupBoxLSN
            // 
            this.groupBoxLSN.Controls.Add(this.labelLSN);
            this.groupBoxLSN.Controls.Add(this.textBoxLSN);
            this.groupBoxLSN.Controls.Add(this.buttonClearLSN);
            this.groupBoxLSN.Font = new System.Drawing.Font("Arial", 14F);
            this.groupBoxLSN.Location = new System.Drawing.Point(124, 22);
            this.groupBoxLSN.Name = "groupBoxLSN";
            this.groupBoxLSN.Size = new System.Drawing.Size(560, 70);
            this.groupBoxLSN.TabIndex = 151;
            this.groupBoxLSN.TabStop = false;
            this.groupBoxLSN.Text = "LSN (Required)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 528);
            this.Controls.Add(this.groupBoxSN);
            this.Controls.Add(this.buttonBurnIn);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.groupBoxAlarm);
            this.Controls.Add(this.buttonExportKey);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.groupBoxOperator);
            this.Controls.Add(this.groupBoxLSN);
            this.Name = "Form1";
            this.Text = "WinDiagnostic -Serial Number v180706";
            this.groupBoxSN.ResumeLayout(false);
            this.groupBoxSN.PerformLayout();
            this.groupBoxAlarm.ResumeLayout(false);
            this.groupBoxAlarm.PerformLayout();
            this.groupBoxOperator.ResumeLayout(false);
            this.groupBoxOperator.PerformLayout();
            this.groupBoxLSN.ResumeLayout(false);
            this.groupBoxLSN.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer TimerCheckInputStatus;
        private System.Windows.Forms.Label labelSN;
        public System.Windows.Forms.TextBox textBoxSN;
        public System.Windows.Forms.Button buttonClearSN;
        private System.Windows.Forms.GroupBox groupBoxSN;
        private System.Windows.Forms.Label labelAutoFail;
        public System.Windows.Forms.TextBox textBoxAutoTestItemFail;
        private System.Windows.Forms.Label labelAutoPass;
        public System.Windows.Forms.TextBox textBoxAutoTestItemPass;
        private System.Windows.Forms.Button buttonBurnIn;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.GroupBox groupBoxAlarm;
        private System.Windows.Forms.Button buttonExportKey;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Timer TimerBackgroundTwinkled;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label labelLSN;
        private System.Windows.Forms.Label lOperator;
        public System.Windows.Forms.TextBox textBoxLSN;
        public System.Windows.Forms.TextBox textBoxOperator;
        public System.Windows.Forms.Button buttonClearLSN;
        public System.Windows.Forms.Button buttonClearOperator;
        private System.Windows.Forms.GroupBox groupBoxOperator;
        private System.Windows.Forms.GroupBox groupBoxLSN;
    }
}

