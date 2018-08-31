namespace rotation
{
    partial class rotation
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
            this.RotationModeTopic = new System.Windows.Forms.Label();
            this.RotationMode = new System.Windows.Forms.Label();
            this.TimerRotation = new System.Windows.Forms.Timer(this.components);
            this.ScreenWidth = new System.Windows.Forms.Label();
            this.ScreenHeight = new System.Windows.Forms.Label();
            this.ScreenWidthTopic = new System.Windows.Forms.Label();
            this.ScreenHeightTopic = new System.Windows.Forms.Label();
            this.buttonFAIL = new System.Windows.Forms.Button();
            this.buttonPASS = new System.Windows.Forms.Button();
            this.DisplayMode = new System.Windows.Forms.Label();
            this.DisplayModeTopic = new System.Windows.Forms.Label();
            this.SensorY = new System.Windows.Forms.Label();
            this.SensorX = new System.Windows.Forms.Label();
            this.TopicSensorY = new System.Windows.Forms.Label();
            this.TopicSensorX = new System.Windows.Forms.Label();
            this.SensorZ = new System.Windows.Forms.Label();
            this.TopicSensorZ = new System.Windows.Forms.Label();
            this.groupBoxDisplayMode = new System.Windows.Forms.GroupBox();
            this.groupBoxRotationMode = new System.Windows.Forms.GroupBox();
            this.TimerRotationWin8 = new System.Windows.Forms.Timer(this.components);
            this.labelResultTopics = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.groupBoxDisplayMode.SuspendLayout();
            this.groupBoxRotationMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 28F);
            this.labelTitle.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelTitle.Location = new System.Drawing.Point(10, 10);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(161, 43);
            this.labelTitle.TabIndex = 10;
            this.labelTitle.Text = "Rotation";
            // 
            // RotationModeTopic
            // 
            this.RotationModeTopic.AutoSize = true;
            this.RotationModeTopic.BackColor = System.Drawing.SystemColors.Control;
            this.RotationModeTopic.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.RotationModeTopic.ForeColor = System.Drawing.Color.Black;
            this.RotationModeTopic.Location = new System.Drawing.Point(6, 132);
            this.RotationModeTopic.Name = "RotationModeTopic";
            this.RotationModeTopic.Size = new System.Drawing.Size(205, 32);
            this.RotationModeTopic.TabIndex = 26;
            this.RotationModeTopic.Text = "Rotation Mode";
            // 
            // RotationMode
            // 
            this.RotationMode.AutoSize = true;
            this.RotationMode.BackColor = System.Drawing.SystemColors.Control;
            this.RotationMode.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.RotationMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.RotationMode.Location = new System.Drawing.Point(214, 132);
            this.RotationMode.Name = "RotationMode";
            this.RotationMode.Size = new System.Drawing.Size(139, 32);
            this.RotationMode.TabIndex = 27;
            this.RotationMode.Text = "Unknown";
            // 
            // TimerRotation
            // 
            this.TimerRotation.Interval = 500;
            this.TimerRotation.Tick += new System.EventHandler(this.TimerRotation_Tick);
            // 
            // ScreenWidth
            // 
            this.ScreenWidth.AutoSize = true;
            this.ScreenWidth.BackColor = System.Drawing.SystemColors.Control;
            this.ScreenWidth.Font = new System.Drawing.Font("Arial", 18F);
            this.ScreenWidth.ForeColor = System.Drawing.Color.Black;
            this.ScreenWidth.Location = new System.Drawing.Point(213, 63);
            this.ScreenWidth.Name = "ScreenWidth";
            this.ScreenWidth.Size = new System.Drawing.Size(25, 27);
            this.ScreenWidth.TabIndex = 31;
            this.ScreenWidth.Text = "0";
            // 
            // ScreenHeight
            // 
            this.ScreenHeight.AutoSize = true;
            this.ScreenHeight.BackColor = System.Drawing.SystemColors.Control;
            this.ScreenHeight.Font = new System.Drawing.Font("Arial", 18F);
            this.ScreenHeight.ForeColor = System.Drawing.Color.Black;
            this.ScreenHeight.Location = new System.Drawing.Point(213, 25);
            this.ScreenHeight.Name = "ScreenHeight";
            this.ScreenHeight.Size = new System.Drawing.Size(25, 27);
            this.ScreenHeight.TabIndex = 30;
            this.ScreenHeight.Text = "0";
            // 
            // ScreenWidthTopic
            // 
            this.ScreenWidthTopic.AutoSize = true;
            this.ScreenWidthTopic.BackColor = System.Drawing.SystemColors.Control;
            this.ScreenWidthTopic.Font = new System.Drawing.Font("Arial", 18F);
            this.ScreenWidthTopic.ForeColor = System.Drawing.Color.Black;
            this.ScreenWidthTopic.Location = new System.Drawing.Point(6, 63);
            this.ScreenWidthTopic.Name = "ScreenWidthTopic";
            this.ScreenWidthTopic.Size = new System.Drawing.Size(158, 27);
            this.ScreenWidthTopic.TabIndex = 29;
            this.ScreenWidthTopic.Text = "Screen Width";
            // 
            // ScreenHeightTopic
            // 
            this.ScreenHeightTopic.AutoSize = true;
            this.ScreenHeightTopic.BackColor = System.Drawing.SystemColors.Control;
            this.ScreenHeightTopic.Font = new System.Drawing.Font("Arial", 18F);
            this.ScreenHeightTopic.ForeColor = System.Drawing.Color.Black;
            this.ScreenHeightTopic.Location = new System.Drawing.Point(6, 25);
            this.ScreenHeightTopic.Name = "ScreenHeightTopic";
            this.ScreenHeightTopic.Size = new System.Drawing.Size(165, 27);
            this.ScreenHeightTopic.TabIndex = 28;
            this.ScreenHeightTopic.Text = "Screen Height";
            // 
            // buttonFAIL
            // 
            this.buttonFAIL.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonFAIL.ForeColor = System.Drawing.Color.Red;
            this.buttonFAIL.Location = new System.Drawing.Point(174, 101);
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
            this.buttonPASS.Location = new System.Drawing.Point(18, 101);
            this.buttonPASS.Name = "buttonPASS";
            this.buttonPASS.Size = new System.Drawing.Size(150, 50);
            this.buttonPASS.TabIndex = 77;
            this.buttonPASS.Text = "PASS";
            this.buttonPASS.UseVisualStyleBackColor = true;
            this.buttonPASS.Click += new System.EventHandler(this.buttonPASS_Click);
            // 
            // DisplayMode
            // 
            this.DisplayMode.AutoSize = true;
            this.DisplayMode.BackColor = System.Drawing.SystemColors.Control;
            this.DisplayMode.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.DisplayMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.DisplayMode.Location = new System.Drawing.Point(214, 99);
            this.DisplayMode.Name = "DisplayMode";
            this.DisplayMode.Size = new System.Drawing.Size(139, 32);
            this.DisplayMode.TabIndex = 80;
            this.DisplayMode.Text = "Unknown";
            // 
            // DisplayModeTopic
            // 
            this.DisplayModeTopic.AutoSize = true;
            this.DisplayModeTopic.BackColor = System.Drawing.SystemColors.Control;
            this.DisplayModeTopic.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.DisplayModeTopic.ForeColor = System.Drawing.Color.Black;
            this.DisplayModeTopic.Location = new System.Drawing.Point(6, 99);
            this.DisplayModeTopic.Name = "DisplayModeTopic";
            this.DisplayModeTopic.Size = new System.Drawing.Size(191, 32);
            this.DisplayModeTopic.TabIndex = 79;
            this.DisplayModeTopic.Text = "Display Mode";
            // 
            // SensorY
            // 
            this.SensorY.AutoSize = true;
            this.SensorY.BackColor = System.Drawing.SystemColors.Control;
            this.SensorY.Font = new System.Drawing.Font("Arial", 18F);
            this.SensorY.ForeColor = System.Drawing.Color.Black;
            this.SensorY.Location = new System.Drawing.Point(213, 59);
            this.SensorY.Name = "SensorY";
            this.SensorY.Size = new System.Drawing.Size(25, 27);
            this.SensorY.TabIndex = 84;
            this.SensorY.Text = "0";
            // 
            // SensorX
            // 
            this.SensorX.AutoSize = true;
            this.SensorX.BackColor = System.Drawing.SystemColors.Control;
            this.SensorX.Font = new System.Drawing.Font("Arial", 18F);
            this.SensorX.ForeColor = System.Drawing.Color.Black;
            this.SensorX.Location = new System.Drawing.Point(213, 25);
            this.SensorX.Name = "SensorX";
            this.SensorX.Size = new System.Drawing.Size(25, 27);
            this.SensorX.TabIndex = 83;
            this.SensorX.Text = "0";
            // 
            // TopicSensorY
            // 
            this.TopicSensorY.AutoSize = true;
            this.TopicSensorY.BackColor = System.Drawing.SystemColors.Control;
            this.TopicSensorY.Font = new System.Drawing.Font("Arial", 18F);
            this.TopicSensorY.ForeColor = System.Drawing.Color.Black;
            this.TopicSensorY.Location = new System.Drawing.Point(6, 59);
            this.TopicSensorY.Name = "TopicSensorY";
            this.TopicSensorY.Size = new System.Drawing.Size(188, 27);
            this.TopicSensorY.TabIndex = 82;
            this.TopicSensorY.Text = "Accelerometer Y";
            // 
            // TopicSensorX
            // 
            this.TopicSensorX.AutoSize = true;
            this.TopicSensorX.BackColor = System.Drawing.SystemColors.Control;
            this.TopicSensorX.Font = new System.Drawing.Font("Arial", 18F);
            this.TopicSensorX.ForeColor = System.Drawing.Color.Black;
            this.TopicSensorX.Location = new System.Drawing.Point(7, 25);
            this.TopicSensorX.Name = "TopicSensorX";
            this.TopicSensorX.Size = new System.Drawing.Size(194, 27);
            this.TopicSensorX.TabIndex = 81;
            this.TopicSensorX.Text = "Accelerometer X ";
            // 
            // SensorZ
            // 
            this.SensorZ.AutoSize = true;
            this.SensorZ.BackColor = System.Drawing.SystemColors.Control;
            this.SensorZ.Font = new System.Drawing.Font("Arial", 18F);
            this.SensorZ.ForeColor = System.Drawing.Color.Black;
            this.SensorZ.Location = new System.Drawing.Point(213, 96);
            this.SensorZ.Name = "SensorZ";
            this.SensorZ.Size = new System.Drawing.Size(25, 27);
            this.SensorZ.TabIndex = 86;
            this.SensorZ.Text = "0";
            // 
            // TopicSensorZ
            // 
            this.TopicSensorZ.AutoSize = true;
            this.TopicSensorZ.BackColor = System.Drawing.SystemColors.Control;
            this.TopicSensorZ.Font = new System.Drawing.Font("Arial", 18F);
            this.TopicSensorZ.ForeColor = System.Drawing.Color.Black;
            this.TopicSensorZ.Location = new System.Drawing.Point(6, 96);
            this.TopicSensorZ.Name = "TopicSensorZ";
            this.TopicSensorZ.Size = new System.Drawing.Size(187, 27);
            this.TopicSensorZ.TabIndex = 85;
            this.TopicSensorZ.Text = "Accelerometer Z";
            // 
            // groupBoxDisplayMode
            // 
            this.groupBoxDisplayMode.Controls.Add(this.ScreenHeightTopic);
            this.groupBoxDisplayMode.Controls.Add(this.ScreenWidthTopic);
            this.groupBoxDisplayMode.Controls.Add(this.ScreenHeight);
            this.groupBoxDisplayMode.Controls.Add(this.ScreenWidth);
            this.groupBoxDisplayMode.Controls.Add(this.DisplayModeTopic);
            this.groupBoxDisplayMode.Controls.Add(this.DisplayMode);
            this.groupBoxDisplayMode.Font = new System.Drawing.Font("Arial", 14F);
            this.groupBoxDisplayMode.Location = new System.Drawing.Point(18, 157);
            this.groupBoxDisplayMode.Name = "groupBoxDisplayMode";
            this.groupBoxDisplayMode.Size = new System.Drawing.Size(502, 134);
            this.groupBoxDisplayMode.TabIndex = 87;
            this.groupBoxDisplayMode.TabStop = false;
            this.groupBoxDisplayMode.Text = "Display Mode";
            // 
            // groupBoxRotationMode
            // 
            this.groupBoxRotationMode.Controls.Add(this.TopicSensorX);
            this.groupBoxRotationMode.Controls.Add(this.RotationModeTopic);
            this.groupBoxRotationMode.Controls.Add(this.SensorZ);
            this.groupBoxRotationMode.Controls.Add(this.RotationMode);
            this.groupBoxRotationMode.Controls.Add(this.TopicSensorZ);
            this.groupBoxRotationMode.Controls.Add(this.TopicSensorY);
            this.groupBoxRotationMode.Controls.Add(this.SensorY);
            this.groupBoxRotationMode.Controls.Add(this.SensorX);
            this.groupBoxRotationMode.Font = new System.Drawing.Font("Arial", 14F);
            this.groupBoxRotationMode.Location = new System.Drawing.Point(18, 297);
            this.groupBoxRotationMode.Name = "groupBoxRotationMode";
            this.groupBoxRotationMode.Size = new System.Drawing.Size(502, 169);
            this.groupBoxRotationMode.TabIndex = 88;
            this.groupBoxRotationMode.TabStop = false;
            this.groupBoxRotationMode.Text = "Rotation Mode";
            this.groupBoxRotationMode.Visible = false;
            // 
            // TimerRotationWin8
            // 
            this.TimerRotationWin8.Tick += new System.EventHandler(this.TimerRotationWin8_Tick);
            // 
            // labelResultTopics
            // 
            this.labelResultTopics.AutoSize = true;
            this.labelResultTopics.Font = new System.Drawing.Font("Arial", 24F);
            this.labelResultTopics.Location = new System.Drawing.Point(12, 62);
            this.labelResultTopics.Name = "labelResultTopics";
            this.labelResultTopics.Size = new System.Drawing.Size(132, 36);
            this.labelResultTopics.TabIndex = 90;
            this.labelResultTopics.Text = "Result : ";
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.labelResult.ForeColor = System.Drawing.Color.Blue;
            this.labelResult.Location = new System.Drawing.Point(150, 56);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(210, 45);
            this.labelResult.TabIndex = 89;
            this.labelResult.Text = "Not Result";
            // 
            // Rotation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.labelResultTopics);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.groupBoxRotationMode);
            this.Controls.Add(this.groupBoxDisplayMode);
            this.Controls.Add(this.buttonFAIL);
            this.Controls.Add(this.buttonPASS);
            this.Controls.Add(this.labelTitle);
            this.Name = "Rotation";
            this.Size = new System.Drawing.Size(700, 600);
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.rotation_Load);
            this.groupBoxDisplayMode.ResumeLayout(false);
            this.groupBoxDisplayMode.PerformLayout();
            this.groupBoxRotationMode.ResumeLayout(false);
            this.groupBoxRotationMode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label RotationModeTopic;
        private System.Windows.Forms.Label RotationMode;
        private System.Windows.Forms.Timer TimerRotation;
        private System.Windows.Forms.Label ScreenWidth;
        private System.Windows.Forms.Label ScreenHeight;
        private System.Windows.Forms.Label ScreenWidthTopic;
        private System.Windows.Forms.Label ScreenHeightTopic;
        private System.Windows.Forms.Button buttonFAIL;
        private System.Windows.Forms.Button buttonPASS;
        private System.Windows.Forms.Label DisplayMode;
        private System.Windows.Forms.Label DisplayModeTopic;
        private System.Windows.Forms.Label SensorY;
        private System.Windows.Forms.Label SensorX;
        private System.Windows.Forms.Label TopicSensorY;
        private System.Windows.Forms.Label TopicSensorX;
        private System.Windows.Forms.Label SensorZ;
        private System.Windows.Forms.Label TopicSensorZ;
        private System.Windows.Forms.GroupBox groupBoxDisplayMode;
        private System.Windows.Forms.GroupBox groupBoxRotationMode;
        private System.Windows.Forms.Timer TimerRotationWin8;
        private System.Windows.Forms.Label labelResultTopics;
        private System.Windows.Forms.Label labelResult;
    }
}

