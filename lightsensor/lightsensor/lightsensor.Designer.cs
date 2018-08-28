namespace lightsensor
{
    partial class lightsensor
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

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelResultTopics = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.buttonPASS = new System.Windows.Forms.Button();
            this.buttonFAIL = new System.Windows.Forms.Button();
            this.groupLightSensor = new System.Windows.Forms.GroupBox();
            this.buttonSensorHub = new System.Windows.Forms.Button();
            this.labelLighSensorRule = new System.Windows.Forms.Label();
            this.buttonCheckLightSensorValue = new System.Windows.Forms.Button();
            this.labelSensorHub = new System.Windows.Forms.Label();
            this.labelLightSensor = new System.Windows.Forms.Label();
            this.TimerLightSensor = new System.Windows.Forms.Timer(this.components);
            this.groupLightSensor.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 28F);
            this.labelTitle.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelTitle.Location = new System.Drawing.Point(3, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(232, 43);
            this.labelTitle.TabIndex = 109;
            this.labelTitle.Text = "Light Sensor";
            // 
            // labelResultTopics
            // 
            this.labelResultTopics.AutoSize = true;
            this.labelResultTopics.Font = new System.Drawing.Font("Arial", 24F);
            this.labelResultTopics.Location = new System.Drawing.Point(3, 48);
            this.labelResultTopics.Name = "labelResultTopics";
            this.labelResultTopics.Size = new System.Drawing.Size(123, 36);
            this.labelResultTopics.TabIndex = 106;
            this.labelResultTopics.Text = "Result :";
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.labelResult.ForeColor = System.Drawing.Color.Blue;
            this.labelResult.Location = new System.Drawing.Point(132, 42);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(210, 45);
            this.labelResult.TabIndex = 105;
            this.labelResult.Text = "Not Result";
            // 
            // buttonPASS
            // 
            this.buttonPASS.Enabled = false;
            this.buttonPASS.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonPASS.ForeColor = System.Drawing.Color.Green;
            this.buttonPASS.Location = new System.Drawing.Point(11, 88);
            this.buttonPASS.Name = "buttonPASS";
            this.buttonPASS.Size = new System.Drawing.Size(150, 50);
            this.buttonPASS.TabIndex = 108;
            this.buttonPASS.Text = "PASS";
            this.buttonPASS.UseVisualStyleBackColor = true;
            // 
            // buttonFAIL
            // 
            this.buttonFAIL.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonFAIL.ForeColor = System.Drawing.Color.Red;
            this.buttonFAIL.Location = new System.Drawing.Point(166, 88);
            this.buttonFAIL.Name = "buttonFAIL";
            this.buttonFAIL.Size = new System.Drawing.Size(150, 50);
            this.buttonFAIL.TabIndex = 107;
            this.buttonFAIL.Text = "FAIL";
            this.buttonFAIL.UseVisualStyleBackColor = true;
            this.buttonFAIL.Click += new System.EventHandler(this.buttonFAIL_Click);
            // 
            // groupLightSensor
            // 
            this.groupLightSensor.Controls.Add(this.buttonSensorHub);
            this.groupLightSensor.Controls.Add(this.labelLighSensorRule);
            this.groupLightSensor.Controls.Add(this.buttonCheckLightSensorValue);
            this.groupLightSensor.Controls.Add(this.labelSensorHub);
            this.groupLightSensor.Controls.Add(this.labelLightSensor);
            this.groupLightSensor.Enabled = false;
            this.groupLightSensor.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.groupLightSensor.Location = new System.Drawing.Point(0, 144);
            this.groupLightSensor.Name = "groupLightSensor";
            this.groupLightSensor.Size = new System.Drawing.Size(593, 169);
            this.groupLightSensor.TabIndex = 110;
            this.groupLightSensor.TabStop = false;
            this.groupLightSensor.Text = "Check LightSensor";
            // 
            // buttonSensorHub
            // 
            this.buttonSensorHub.Enabled = false;
            this.buttonSensorHub.Font = new System.Drawing.Font("Arial", 21F, System.Drawing.FontStyle.Bold);
            this.buttonSensorHub.Location = new System.Drawing.Point(11, 42);
            this.buttonSensorHub.Name = "buttonSensorHub";
            this.buttonSensorHub.Size = new System.Drawing.Size(180, 50);
            this.buttonSensorHub.TabIndex = 87;
            this.buttonSensorHub.Text = "SensorHub";
            this.buttonSensorHub.UseVisualStyleBackColor = true;
            this.buttonSensorHub.Click += new System.EventHandler(this.buttonSensorHub_Click);
            // 
            // labelLighSensorRule
            // 
            this.labelLighSensorRule.AutoSize = true;
            this.labelLighSensorRule.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.labelLighSensorRule.Location = new System.Drawing.Point(373, 117);
            this.labelLighSensorRule.Name = "labelLighSensorRule";
            this.labelLighSensorRule.Size = new System.Drawing.Size(217, 29);
            this.labelLighSensorRule.TabIndex = 86;
            this.labelLighSensorRule.Text = "Min<5   Max>1000";
            // 
            // buttonCheckLightSensorValue
            // 
            this.buttonCheckLightSensorValue.Enabled = false;
            this.buttonCheckLightSensorValue.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonCheckLightSensorValue.ForeColor = System.Drawing.Color.Blue;
            this.buttonCheckLightSensorValue.Location = new System.Drawing.Point(11, 103);
            this.buttonCheckLightSensorValue.Name = "buttonCheckLightSensorValue";
            this.buttonCheckLightSensorValue.Size = new System.Drawing.Size(180, 50);
            this.buttonCheckLightSensorValue.TabIndex = 85;
            this.buttonCheckLightSensorValue.Text = "Test";
            this.buttonCheckLightSensorValue.UseVisualStyleBackColor = true;
            this.buttonCheckLightSensorValue.Click += new System.EventHandler(this.buttonCheckLightSensorValue_Click);
            // 
            // labelSensorHub
            // 
            this.labelSensorHub.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelSensorHub.Font = new System.Drawing.Font("Arial", 24F);
            this.labelSensorHub.Location = new System.Drawing.Point(197, 42);
            this.labelSensorHub.Name = "labelSensorHub";
            this.labelSensorHub.Size = new System.Drawing.Size(170, 50);
            this.labelSensorHub.TabIndex = 14;
            this.labelSensorHub.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelLightSensor
            // 
            this.labelLightSensor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelLightSensor.Font = new System.Drawing.Font("Arial", 24F);
            this.labelLightSensor.Location = new System.Drawing.Point(197, 103);
            this.labelLightSensor.Name = "labelLightSensor";
            this.labelLightSensor.Size = new System.Drawing.Size(170, 50);
            this.labelLightSensor.TabIndex = 13;
            this.labelLightSensor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TimerLightSensor
            // 
            this.TimerLightSensor.Interval = 250;
            this.TimerLightSensor.Tick += new System.EventHandler(this.TimerLightSensor_Tick);
            // 
            // LightSensor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupLightSensor);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.labelResultTopics);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.buttonPASS);
            this.Controls.Add(this.buttonFAIL);
            this.Name = "LightSensor";
            this.Size = new System.Drawing.Size(700, 600);
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.lightsensor_Load);
            this.groupLightSensor.ResumeLayout(false);
            this.groupLightSensor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelResultTopics;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Button buttonPASS;
        private System.Windows.Forms.Button buttonFAIL;
        private System.Windows.Forms.GroupBox groupLightSensor;
        private System.Windows.Forms.Button buttonSensorHub;
        private System.Windows.Forms.Label labelLighSensorRule;
        private System.Windows.Forms.Button buttonCheckLightSensorValue;
        public System.Windows.Forms.Label labelSensorHub;
        public System.Windows.Forms.Label labelLightSensor;
        private System.Windows.Forms.Timer TimerLightSensor;
    }
}

