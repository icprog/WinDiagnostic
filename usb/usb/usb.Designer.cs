namespace usb
{
    partial class usb
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelUSBResult = new System.Windows.Forms.Label();
            this.groupUSBDetails = new System.Windows.Forms.GroupBox();
            this.txtUSBDetails = new System.Windows.Forms.TextBox();
            this.USBTopic = new System.Windows.Forms.Label();
            this.SDTopic = new System.Windows.Forms.Label();
            this.labelSDResult = new System.Windows.Forms.Label();
            this.groupSDCard = new System.Windows.Forms.GroupBox();
            this.labelExistingSDCard = new System.Windows.Forms.Label();
            this.groupUSB = new System.Windows.Forms.GroupBox();
            this.labelExistingRemovableDevice = new System.Windows.Forms.Label();
            this.labelResultTopics = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.groupUSBDetails.SuspendLayout();
            this.groupSDCard.SuspendLayout();
            this.groupUSB.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 28F);
            this.labelTitle.Location = new System.Drawing.Point(13, 13);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(119, 53);
            this.labelTitle.TabIndex = 3;
            this.labelTitle.Text = "USB";
            // 
            // labelUSBResult
            // 
            this.labelUSBResult.AutoSize = true;
            this.labelUSBResult.Font = new System.Drawing.Font("Arial", 20F);
            this.labelUSBResult.Location = new System.Drawing.Point(324, 35);
            this.labelUSBResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelUSBResult.Name = "labelUSBResult";
            this.labelUSBResult.Size = new System.Drawing.Size(343, 39);
            this.labelUSBResult.TabIndex = 11;
            this.labelUSBResult.Text = "Finding USB Devices.";
            // 
            // groupUSBDetails
            // 
            this.groupUSBDetails.Controls.Add(this.txtUSBDetails);
            this.groupUSBDetails.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupUSBDetails.Location = new System.Drawing.Point(24, 333);
            this.groupUSBDetails.Margin = new System.Windows.Forms.Padding(4);
            this.groupUSBDetails.Name = "groupUSBDetails";
            this.groupUSBDetails.Padding = new System.Windows.Forms.Padding(4);
            this.groupUSBDetails.Size = new System.Drawing.Size(855, 439);
            this.groupUSBDetails.TabIndex = 54;
            this.groupUSBDetails.TabStop = false;
            this.groupUSBDetails.Text = "Mass Storage Device Details";
            // 
            // txtUSBDetails
            // 
            this.txtUSBDetails.Font = new System.Drawing.Font("Arial", 12F);
            this.txtUSBDetails.Location = new System.Drawing.Point(13, 28);
            this.txtUSBDetails.Margin = new System.Windows.Forms.Padding(4);
            this.txtUSBDetails.Multiline = true;
            this.txtUSBDetails.Name = "txtUSBDetails";
            this.txtUSBDetails.ReadOnly = true;
            this.txtUSBDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUSBDetails.Size = new System.Drawing.Size(832, 396);
            this.txtUSBDetails.TabIndex = 9;
            this.txtUSBDetails.Text = "Unknown";
            // 
            // USBTopic
            // 
            this.USBTopic.AutoSize = true;
            this.USBTopic.Font = new System.Drawing.Font("Arial", 24F);
            this.USBTopic.Location = new System.Drawing.Point(12, 29);
            this.USBTopic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.USBTopic.Name = "USBTopic";
            this.USBTopic.Size = new System.Drawing.Size(125, 45);
            this.USBTopic.TabIndex = 56;
            this.USBTopic.Text = "USB :";
            // 
            // SDTopic
            // 
            this.SDTopic.AutoSize = true;
            this.SDTopic.Font = new System.Drawing.Font("Arial", 24F);
            this.SDTopic.Location = new System.Drawing.Point(12, 29);
            this.SDTopic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.SDTopic.Name = "SDTopic";
            this.SDTopic.Size = new System.Drawing.Size(98, 45);
            this.SDTopic.TabIndex = 58;
            this.SDTopic.Text = "SD :";
            // 
            // labelSDResult
            // 
            this.labelSDResult.AutoSize = true;
            this.labelSDResult.Font = new System.Drawing.Font("Arial", 20F);
            this.labelSDResult.Location = new System.Drawing.Point(324, 39);
            this.labelSDResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSDResult.Name = "labelSDResult";
            this.labelSDResult.Size = new System.Drawing.Size(320, 39);
            this.labelSDResult.TabIndex = 57;
            this.labelSDResult.Text = "Finding SD Devices.";
            // 
            // groupSDCard
            // 
            this.groupSDCard.Controls.Add(this.labelExistingSDCard);
            this.groupSDCard.Controls.Add(this.SDTopic);
            this.groupSDCard.Controls.Add(this.labelSDResult);
            this.groupSDCard.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupSDCard.Location = new System.Drawing.Point(24, 233);
            this.groupSDCard.Margin = new System.Windows.Forms.Padding(4);
            this.groupSDCard.Name = "groupSDCard";
            this.groupSDCard.Padding = new System.Windows.Forms.Padding(4);
            this.groupSDCard.Size = new System.Drawing.Size(855, 93);
            this.groupSDCard.TabIndex = 59;
            this.groupSDCard.TabStop = false;
            this.groupSDCard.Text = "SD Card";
            this.groupSDCard.Visible = false;
            // 
            // labelExistingSDCard
            // 
            this.labelExistingSDCard.AutoSize = true;
            this.labelExistingSDCard.Font = new System.Drawing.Font("Arial", 24F);
            this.labelExistingSDCard.ForeColor = System.Drawing.Color.Blue;
            this.labelExistingSDCard.Location = new System.Drawing.Point(119, 29);
            this.labelExistingSDCard.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelExistingSDCard.Name = "labelExistingSDCard";
            this.labelExistingSDCard.Size = new System.Drawing.Size(183, 45);
            this.labelExistingSDCard.TabIndex = 59;
            this.labelExistingSDCard.Text = "Unknown";
            // 
            // groupUSB
            // 
            this.groupUSB.Controls.Add(this.labelExistingRemovableDevice);
            this.groupUSB.Controls.Add(this.USBTopic);
            this.groupUSB.Controls.Add(this.labelUSBResult);
            this.groupUSB.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupUSB.Location = new System.Drawing.Point(24, 132);
            this.groupUSB.Margin = new System.Windows.Forms.Padding(4);
            this.groupUSB.Name = "groupUSB";
            this.groupUSB.Padding = new System.Windows.Forms.Padding(4);
            this.groupUSB.Size = new System.Drawing.Size(855, 93);
            this.groupUSB.TabIndex = 60;
            this.groupUSB.TabStop = false;
            this.groupUSB.Text = "USB";
            // 
            // labelExistingRemovableDevice
            // 
            this.labelExistingRemovableDevice.AutoSize = true;
            this.labelExistingRemovableDevice.Font = new System.Drawing.Font("Arial", 24F);
            this.labelExistingRemovableDevice.ForeColor = System.Drawing.Color.Blue;
            this.labelExistingRemovableDevice.Location = new System.Drawing.Point(119, 29);
            this.labelExistingRemovableDevice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelExistingRemovableDevice.Name = "labelExistingRemovableDevice";
            this.labelExistingRemovableDevice.Size = new System.Drawing.Size(183, 45);
            this.labelExistingRemovableDevice.TabIndex = 57;
            this.labelExistingRemovableDevice.Text = "Unknown";
            // 
            // labelResultTopics
            // 
            this.labelResultTopics.AutoSize = true;
            this.labelResultTopics.Font = new System.Drawing.Font("Arial", 24F);
            this.labelResultTopics.Location = new System.Drawing.Point(13, 77);
            this.labelResultTopics.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResultTopics.Name = "labelResultTopics";
            this.labelResultTopics.Size = new System.Drawing.Size(152, 45);
            this.labelResultTopics.TabIndex = 62;
            this.labelResultTopics.Text = "Result :";
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.labelResult.ForeColor = System.Drawing.Color.Blue;
            this.labelResult.Location = new System.Drawing.Point(185, 69);
            this.labelResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(259, 55);
            this.labelResult.TabIndex = 61;
            this.labelResult.Text = "Not Result";
            // 
            // usb
            // 
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(909, 865);
            this.Controls.Add(this.labelResultTopics);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.groupUSB);
            this.Controls.Add(this.groupSDCard);
            this.Controls.Add(this.groupUSBDetails);
            this.Controls.Add(this.labelTitle);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "usb";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.usb_Load);
            this.groupUSBDetails.ResumeLayout(false);
            this.groupUSBDetails.PerformLayout();
            this.groupSDCard.ResumeLayout(false);
            this.groupSDCard.PerformLayout();
            this.groupUSB.ResumeLayout(false);
            this.groupUSB.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTitle;
        public System.Windows.Forms.Label labelUSBResult;
        private System.Windows.Forms.GroupBox groupUSBDetails;
        private System.Windows.Forms.TextBox txtUSBDetails;
        private System.Windows.Forms.Label USBTopic;
        private System.Windows.Forms.Label SDTopic;
        public System.Windows.Forms.Label labelSDResult;
        private System.Windows.Forms.GroupBox groupSDCard;
        private System.Windows.Forms.GroupBox groupUSB;
        private System.Windows.Forms.Label labelExistingRemovableDevice;
        private System.Windows.Forms.Label labelExistingSDCard;
        private System.Windows.Forms.Label labelResultTopics;
        private System.Windows.Forms.Label labelResult;
    }
}

