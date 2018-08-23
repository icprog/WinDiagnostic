namespace wifi
{
    partial class wifi
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            this.labelResult = new System.Windows.Forms.Label();
            this.groupWiFiDetails = new System.Windows.Forms.GroupBox();
            this.txtWiFiDetails = new System.Windows.Forms.TextBox();
            this.labelResultTopics = new System.Windows.Forms.Label();
            this.listViewWifiStatus = new System.Windows.Forms.ListView();
            this.Mac_Address = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SSID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Channel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Signal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Authentication = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Encryption = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Radio = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NetworkType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Speed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelTitle = new System.Windows.Forms.Label();
            this.groupWiFiDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Arial", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResult.ForeColor = System.Drawing.Color.Blue;
            this.labelResult.Location = new System.Drawing.Point(131, 50);
            this.labelResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(186, 40);
            this.labelResult.TabIndex = 5;
            this.labelResult.Text = "Not Result";
            // 
            // groupWiFiDetails
            // 
            this.groupWiFiDetails.Controls.Add(this.txtWiFiDetails);
            this.groupWiFiDetails.Font = new System.Drawing.Font("Arial", 12F);
            this.groupWiFiDetails.Location = new System.Drawing.Point(4, 446);
            this.groupWiFiDetails.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupWiFiDetails.Name = "groupWiFiDetails";
            this.groupWiFiDetails.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupWiFiDetails.Size = new System.Drawing.Size(896, 248);
            this.groupWiFiDetails.TabIndex = 46;
            this.groupWiFiDetails.TabStop = false;
            this.groupWiFiDetails.Text = "WIFI Details";
            // 
            // txtWiFiDetails
            // 
            this.txtWiFiDetails.Font = new System.Drawing.Font("Arial", 12F);
            this.txtWiFiDetails.Location = new System.Drawing.Point(13, 28);
            this.txtWiFiDetails.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtWiFiDetails.Multiline = true;
            this.txtWiFiDetails.Name = "txtWiFiDetails";
            this.txtWiFiDetails.ReadOnly = true;
            this.txtWiFiDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWiFiDetails.Size = new System.Drawing.Size(867, 205);
            this.txtWiFiDetails.TabIndex = 9;
            // 
            // labelResultTopics
            // 
            this.labelResultTopics.AutoEllipsis = true;
            this.labelResultTopics.AutoSize = true;
            this.labelResultTopics.Font = new System.Drawing.Font("Arial", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResultTopics.ForeColor = System.Drawing.Color.Black;
            this.labelResultTopics.Location = new System.Drawing.Point(-7, 50);
            this.labelResultTopics.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResultTopics.Name = "labelResultTopics";
            this.labelResultTopics.Size = new System.Drawing.Size(130, 39);
            this.labelResultTopics.TabIndex = 47;
            this.labelResultTopics.Text = "Result :";
            // 
            // listViewWifiStatus
            // 
            this.listViewWifiStatus.AllowColumnReorder = true;
            this.listViewWifiStatus.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Mac_Address,
            this.SSID,
            this.Channel,
            this.Signal,
            this.Authentication,
            this.Encryption,
            this.Radio,
            this.NetworkType,
            this.Speed});
            this.listViewWifiStatus.Font = new System.Drawing.Font("PMingLiU", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.listViewWifiStatus.FullRowSelect = true;
            this.listViewWifiStatus.GridLines = true;
            this.listViewWifiStatus.Location = new System.Drawing.Point(4, 139);
            this.listViewWifiStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listViewWifiStatus.Name = "listViewWifiStatus";
            this.listViewWifiStatus.Size = new System.Drawing.Size(899, 299);
            this.listViewWifiStatus.TabIndex = 49;
            this.listViewWifiStatus.UseCompatibleStateImageBehavior = false;
            this.listViewWifiStatus.View = System.Windows.Forms.View.Details;
            // 
            // Mac_Address
            // 
            this.Mac_Address.Text = "Mac Address";
            this.Mac_Address.Width = 130;
            // 
            // SSID
            // 
            this.SSID.Text = "SSID";
            this.SSID.Width = 76;
            // 
            // Channel
            // 
            this.Channel.Text = "Channel";
            this.Channel.Width = 88;
            // 
            // Signal
            // 
            this.Signal.Text = "Signal";
            this.Signal.Width = 80;
            // 
            // Authentication
            // 
            this.Authentication.Text = "Authentication";
            this.Authentication.Width = 126;
            // 
            // Encryption
            // 
            this.Encryption.Text = "Encryption";
            this.Encryption.Width = 105;
            // 
            // Radio
            // 
            this.Radio.Text = "Radio";
            this.Radio.Width = 93;
            // 
            // NetworkType
            // 
            this.NetworkType.Text = "Network";
            this.NetworkType.Width = 129;
            // 
            // Speed
            // 
            this.Speed.Text = "Speed";
            this.Speed.Width = 93;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(-7, 0);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(86, 39);
            this.labelTitle.TabIndex = 50;
            this.labelTitle.Text = "WiFi";
            // 
            // wifi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(955, 699);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.listViewWifiStatus);
            this.Controls.Add(this.labelResultTopics);
            this.Controls.Add(this.groupWiFiDetails);
            this.Controls.Add(this.labelResult);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "wifi";
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.wifi_Load);
            this.groupWiFiDetails.ResumeLayout(false);
            this.groupWiFiDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.GroupBox groupWiFiDetails;
        private System.Windows.Forms.TextBox txtWiFiDetails;
        private System.Windows.Forms.Label labelResultTopics;
        private System.Windows.Forms.ListView listViewWifiStatus;
        private System.Windows.Forms.ColumnHeader Mac_Address;
        private System.Windows.Forms.ColumnHeader SSID;
        private System.Windows.Forms.ColumnHeader Channel;
        private System.Windows.Forms.ColumnHeader Signal;
        private System.Windows.Forms.ColumnHeader Authentication;
        private System.Windows.Forms.ColumnHeader Encryption;
        private System.Windows.Forms.ColumnHeader Radio;
        private System.Windows.Forms.ColumnHeader NetworkType;
        private System.Windows.Forms.ColumnHeader Speed;
        private System.Windows.Forms.Label labelTitle;
    }
}

