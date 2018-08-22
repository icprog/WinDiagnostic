namespace camera
{
    partial class camera
    {
        /// <summary> 
        /// 設計工具所需的變數. 
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源. 
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true, 否則為 false. </param>
        // protected override void Dispose(bool disposing)
        // {
        //     if (disposing && (components != null))
        //     {
        //         components.Dispose();
        //     }
        //     base.Dispose(disposing);
        // }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容. 
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonPreviewTop = new System.Windows.Forms.Button();
            this.buttonPreviewBottom = new System.Windows.Forms.Button();
            this.buttonFAIL = new System.Windows.Forms.Button();
            this.buttonCapture = new System.Windows.Forms.Button();
            this.groupBoxTestButton = new System.Windows.Forms.GroupBox();
            this.groupBoxPositionOfCapture = new System.Windows.Forms.GroupBox();
            this.buttonPASS = new System.Windows.Forms.Button();
            this.buttonFlash = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pictureBoxPreview2 = new System.Windows.Forms.PictureBox();
            this.labelLiveview = new System.Windows.Forms.Label();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.comboBoxCameraDevices = new System.Windows.Forms.ComboBox();
            this.pictureBoxLiveview = new System.Windows.Forms.PictureBox();
            this.labelTestCameraName = new System.Windows.Forms.Label();
            this.labelResultTopics = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.groupBoxCapturePreview = new System.Windows.Forms.GroupBox();
            this.TimerCamaraCapture = new System.Windows.Forms.Timer(this.components);
            this.groupBoxTestButton.SuspendLayout();
            this.groupBoxPositionOfCapture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLiveview)).BeginInit();
            this.groupBoxCapturePreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonPreviewTop
            // 
            this.buttonPreviewTop.Enabled = false;
            this.buttonPreviewTop.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonPreviewTop.Location = new System.Drawing.Point(4, 49);
            this.buttonPreviewTop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonPreviewTop.Name = "buttonPreviewTop";
            this.buttonPreviewTop.Size = new System.Drawing.Size(200, 67);
            this.buttonPreviewTop.TabIndex = 87;
            this.buttonPreviewTop.Text = "Top";
            this.buttonPreviewTop.UseVisualStyleBackColor = true;
            this.buttonPreviewTop.Click += new System.EventHandler(this.buttonPreviewSelect_Click);
            // 
            // buttonPreviewBottom
            // 
            this.buttonPreviewBottom.Enabled = false;
            this.buttonPreviewBottom.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonPreviewBottom.Location = new System.Drawing.Point(209, 49);
            this.buttonPreviewBottom.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonPreviewBottom.Name = "buttonPreviewBottom";
            this.buttonPreviewBottom.Size = new System.Drawing.Size(200, 67);
            this.buttonPreviewBottom.TabIndex = 86;
            this.buttonPreviewBottom.Text = "Bottom";
            this.buttonPreviewBottom.UseVisualStyleBackColor = true;
            this.buttonPreviewBottom.Click += new System.EventHandler(this.buttonPreviewSelect_Click);
            // 
            // buttonFAIL
            // 
            this.buttonFAIL.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonFAIL.ForeColor = System.Drawing.Color.Red;
            this.buttonFAIL.Location = new System.Drawing.Point(216, 249);
            this.buttonFAIL.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonFAIL.Name = "buttonFAIL";
            this.buttonFAIL.Size = new System.Drawing.Size(200, 67);
            this.buttonFAIL.TabIndex = 76;
            this.buttonFAIL.Text = "FAIL";
            this.buttonFAIL.UseVisualStyleBackColor = true;
            this.buttonFAIL.Click += new System.EventHandler(this.buttonCameraFAIL_Click);
            // 
            // buttonCapture
            // 
            this.buttonCapture.Enabled = false;
            this.buttonCapture.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonCapture.Location = new System.Drawing.Point(216, 37);
            this.buttonCapture.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCapture.Name = "buttonCapture";
            this.buttonCapture.Size = new System.Drawing.Size(200, 67);
            this.buttonCapture.TabIndex = 78;
            this.buttonCapture.Text = "Capture";
            this.buttonCapture.UseVisualStyleBackColor = true;
            this.buttonCapture.Click += new System.EventHandler(this.buttonCapture_Click);
            // 
            // groupBoxTestButton
            // 
            this.groupBoxTestButton.Controls.Add(this.groupBoxPositionOfCapture);
            this.groupBoxTestButton.Controls.Add(this.buttonPASS);
            this.groupBoxTestButton.Controls.Add(this.buttonFAIL);
            this.groupBoxTestButton.Controls.Add(this.buttonFlash);
            this.groupBoxTestButton.Controls.Add(this.buttonCapture);
            this.groupBoxTestButton.Font = new System.Drawing.Font("Arial", 14F);
            this.groupBoxTestButton.Location = new System.Drawing.Point(1, 461);
            this.groupBoxTestButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxTestButton.Name = "groupBoxTestButton";
            this.groupBoxTestButton.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxTestButton.Size = new System.Drawing.Size(427, 335);
            this.groupBoxTestButton.TabIndex = 97;
            this.groupBoxTestButton.TabStop = false;
            this.groupBoxTestButton.Text = "Test Button";
            // 
            // groupBoxPositionOfCapture
            // 
            this.groupBoxPositionOfCapture.Controls.Add(this.buttonPreviewBottom);
            this.groupBoxPositionOfCapture.Controls.Add(this.buttonPreviewTop);
            this.groupBoxPositionOfCapture.Font = new System.Drawing.Font("Arial", 20F);
            this.groupBoxPositionOfCapture.Location = new System.Drawing.Point(7, 112);
            this.groupBoxPositionOfCapture.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxPositionOfCapture.Name = "groupBoxPositionOfCapture";
            this.groupBoxPositionOfCapture.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxPositionOfCapture.Size = new System.Drawing.Size(412, 129);
            this.groupBoxPositionOfCapture.TabIndex = 105;
            this.groupBoxPositionOfCapture.TabStop = false;
            this.groupBoxPositionOfCapture.Text = "Position of Capture ?";
            // 
            // buttonPASS
            // 
            this.buttonPASS.Enabled = false;
            this.buttonPASS.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonPASS.ForeColor = System.Drawing.Color.Green;
            this.buttonPASS.Location = new System.Drawing.Point(7, 249);
            this.buttonPASS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonPASS.Name = "buttonPASS";
            this.buttonPASS.Size = new System.Drawing.Size(200, 67);
            this.buttonPASS.TabIndex = 90;
            this.buttonPASS.Text = "PASS";
            this.buttonPASS.UseVisualStyleBackColor = true;
            // 
            // buttonFlash
            // 
            this.buttonFlash.Enabled = false;
            this.buttonFlash.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonFlash.Location = new System.Drawing.Point(8, 37);
            this.buttonFlash.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonFlash.Name = "buttonFlash";
            this.buttonFlash.Size = new System.Drawing.Size(200, 67);
            this.buttonFlash.TabIndex = 55;
            this.buttonFlash.Text = "Flash";
            this.buttonFlash.UseVisualStyleBackColor = true;
            this.buttonFlash.Click += new System.EventHandler(this.buttonFlash_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(443, 377);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 320);
            this.label4.TabIndex = 96;
            this.label4.Text = "Bottom";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(444, 49);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 171);
            this.label3.TabIndex = 95;
            this.label3.Text = "Top";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBoxPreview2
            // 
            this.pictureBoxPreview2.Location = new System.Drawing.Point(8, 377);
            this.pictureBoxPreview2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBoxPreview2.Name = "pictureBoxPreview2";
            this.pictureBoxPreview2.Size = new System.Drawing.Size(427, 320);
            this.pictureBoxPreview2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview2.TabIndex = 94;
            this.pictureBoxPreview2.TabStop = false;
            // 
            // labelLiveview
            // 
            this.labelLiveview.AutoSize = true;
            this.labelLiveview.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLiveview.Location = new System.Drawing.Point(-7, 83);
            this.labelLiveview.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelLiveview.Name = "labelLiveview";
            this.labelLiveview.Size = new System.Drawing.Size(143, 39);
            this.labelLiveview.TabIndex = 93;
            this.labelLiveview.Text = "Liveview";
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Location = new System.Drawing.Point(8, 49);
            this.pictureBoxPreview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(427, 320);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 90;
            this.pictureBoxPreview.TabStop = false;
            // 
            // comboBoxCameraDevices
            // 
            this.comboBoxCameraDevices.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxCameraDevices.FormattingEnabled = true;
            this.comboBoxCameraDevices.IntegralHeight = false;
            this.comboBoxCameraDevices.ItemHeight = 40;
            this.comboBoxCameraDevices.Location = new System.Drawing.Point(436, 13);
            this.comboBoxCameraDevices.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxCameraDevices.Name = "comboBoxCameraDevices";
            this.comboBoxCameraDevices.Size = new System.Drawing.Size(476, 48);
            this.comboBoxCameraDevices.TabIndex = 89;
            this.comboBoxCameraDevices.SelectedIndexChanged += new System.EventHandler(this.comboBoxCameraDevices_SelectedIndexChanged);
            // 
            // pictureBoxLiveview
            // 
            this.pictureBoxLiveview.Location = new System.Drawing.Point(1, 133);
            this.pictureBoxLiveview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBoxLiveview.Name = "pictureBoxLiveview";
            this.pictureBoxLiveview.Size = new System.Drawing.Size(427, 320);
            this.pictureBoxLiveview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLiveview.TabIndex = 88;
            this.pictureBoxLiveview.TabStop = false;
            // 
            // labelTestCameraName
            // 
            this.labelTestCameraName.AutoSize = true;
            this.labelTestCameraName.Font = new System.Drawing.Font("Arial", 22F, System.Drawing.FontStyle.Bold);
            this.labelTestCameraName.ForeColor = System.Drawing.Color.Blue;
            this.labelTestCameraName.Location = new System.Drawing.Point(140, 79);
            this.labelTestCameraName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTestCameraName.Name = "labelTestCameraName";
            this.labelTestCameraName.Size = new System.Drawing.Size(269, 44);
            this.labelTestCameraName.TabIndex = 98;
            this.labelTestCameraName.Text = "Camera Name";
            // 
            // labelResultTopics
            // 
            this.labelResultTopics.AutoSize = true;
            this.labelResultTopics.Font = new System.Drawing.Font("Arial", 24F);
            this.labelResultTopics.Location = new System.Drawing.Point(-7, 19);
            this.labelResultTopics.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResultTopics.Name = "labelResultTopics";
            this.labelResultTopics.Size = new System.Drawing.Size(130, 45);
            this.labelResultTopics.TabIndex = 103;
            this.labelResultTopics.Text = "Result";
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.labelResult.ForeColor = System.Drawing.Color.Blue;
            this.labelResult.Location = new System.Drawing.Point(148, 11);
            this.labelResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(259, 55);
            this.labelResult.TabIndex = 102;
            this.labelResult.Text = "Not Result";
            // 
            // groupBoxCapturePreview
            // 
            this.groupBoxCapturePreview.Controls.Add(this.pictureBoxPreview);
            this.groupBoxCapturePreview.Controls.Add(this.pictureBoxPreview2);
            this.groupBoxCapturePreview.Controls.Add(this.label3);
            this.groupBoxCapturePreview.Controls.Add(this.label4);
            this.groupBoxCapturePreview.Font = new System.Drawing.Font("Arial", 20F);
            this.groupBoxCapturePreview.Location = new System.Drawing.Point(436, 83);
            this.groupBoxCapturePreview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxCapturePreview.Name = "groupBoxCapturePreview";
            this.groupBoxCapturePreview.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxCapturePreview.Size = new System.Drawing.Size(493, 713);
            this.groupBoxCapturePreview.TabIndex = 104;
            this.groupBoxCapturePreview.TabStop = false;
            this.groupBoxCapturePreview.Text = "Capture Preview";
            // 
            // TimerCamaraCapture
            // 
            this.TimerCamaraCapture.Interval = 5000;
            this.TimerCamaraCapture.Tick += new System.EventHandler(this.TimerCamaraCapture_Tick);
            // 
            // camera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(970, 842);
            this.Controls.Add(this.groupBoxCapturePreview);
            this.Controls.Add(this.labelResultTopics);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.labelTestCameraName);
            this.Controls.Add(this.groupBoxTestButton);
            this.Controls.Add(this.labelLiveview);
            this.Controls.Add(this.comboBoxCameraDevices);
            this.Controls.Add(this.pictureBoxLiveview);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "camera";
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.camera_Load);
            this.groupBoxTestButton.ResumeLayout(false);
            this.groupBoxPositionOfCapture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLiveview)).EndInit();
            this.groupBoxCapturePreview.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Button buttonPreviewTop;
        public System.Windows.Forms.Button buttonPreviewBottom;
        private System.Windows.Forms.Button buttonFAIL;
        public System.Windows.Forms.Button buttonCapture;
        private System.Windows.Forms.GroupBox groupBoxTestButton;
        public System.Windows.Forms.Button buttonFlash;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelLiveview;
        public System.Windows.Forms.ComboBox comboBoxCameraDevices;
        public System.Windows.Forms.PictureBox pictureBoxLiveview;
        public System.Windows.Forms.PictureBox pictureBoxPreview2;
        public System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Label labelTestCameraName;
        private System.Windows.Forms.Label labelResultTopics;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Button buttonPASS;
        private System.Windows.Forms.GroupBox groupBoxCapturePreview;
        private System.Windows.Forms.GroupBox groupBoxPositionOfCapture;
        private System.Windows.Forms.Timer TimerCamaraCapture;
    }
}

