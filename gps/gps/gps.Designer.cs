namespace gps
{
    partial class gps
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
            this.groupBoxGPSRawData = new System.Windows.Forms.GroupBox();
            this.dumpRawDataCheck = new System.Windows.Forms.CheckBox();
            this.txtNMEARawData = new System.Windows.Forms.TextBox();
            this.labelResult = new System.Windows.Forms.Label();
            this.groupBoxGPSFixInfo = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.labelLatitude = new System.Windows.Forms.Label();
            this.txtLatitude = new System.Windows.Forms.TextBox();
            this.labelLongitude = new System.Windows.Forms.Label();
            this.txtLongitude = new System.Windows.Forms.TextBox();
            this.txtSatellitesSNRAverage = new System.Windows.Forms.TextBox();
            this.labelAltitude = new System.Windows.Forms.Label();
            this.labelSNR = new System.Windows.Forms.Label();
            this.txtSeaLevel = new System.Windows.Forms.TextBox();
            this.txtQuality = new System.Windows.Forms.TextBox();
            this.labelFixMode = new System.Windows.Forms.Label();
            this.txtGPSStatus = new System.Windows.Forms.TextBox();
            this.txtGPSMode = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBoxSatellitesDetails = new System.Windows.Forms.GroupBox();
            this.listSatellites = new System.Windows.Forms.ListView();
            this.PRNColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ElevationColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AzimuthColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SNRColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelSatelitesInView = new System.Windows.Forms.Label();
            this.textBoxSatellitesInView = new System.Windows.Forms.TextBox();
            this.labelResultTopics = new System.Windows.Forms.Label();
            this.txtGPSFixTime = new System.Windows.Forms.TextBox();
            this.labelFixTime = new System.Windows.Forms.Label();
            this.txtGPSConnectTime = new System.Windows.Forms.TextBox();
            this.labelTestTime = new System.Windows.Forms.Label();
            this.txtSTQty = new System.Windows.Forms.TextBox();
            this.labelSsatellitesInUse = new System.Windows.Forms.Label();
            this.txtPortStatus = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.buttonDisconnectGPS = new System.Windows.Forms.Button();
            this.buttonConnectGPS = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxGPSdata = new System.Windows.Forms.GroupBox();
            this.comboBoxSerialPortGPS = new System.Windows.Forms.ComboBox();
            this.groupBoxGPSRawData.SuspendLayout();
            this.groupBoxGPSFixInfo.SuspendLayout();
            this.groupBoxSatellitesDetails.SuspendLayout();
            this.groupBoxGPSdata.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxGPSRawData
            // 
            this.groupBoxGPSRawData.Controls.Add(this.dumpRawDataCheck);
            this.groupBoxGPSRawData.Controls.Add(this.txtNMEARawData);
            this.groupBoxGPSRawData.Font = new System.Drawing.Font("Arial", 14.25F);
            this.groupBoxGPSRawData.Location = new System.Drawing.Point(4, 610);
            this.groupBoxGPSRawData.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxGPSRawData.Name = "groupBoxGPSRawData";
            this.groupBoxGPSRawData.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxGPSRawData.Size = new System.Drawing.Size(845, 185);
            this.groupBoxGPSRawData.TabIndex = 92;
            this.groupBoxGPSRawData.TabStop = false;
            this.groupBoxGPSRawData.Text = "NMEA Raw Data";
            // 
            // dumpRawDataCheck
            // 
            this.dumpRawDataCheck.Checked = true;
            this.dumpRawDataCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dumpRawDataCheck.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dumpRawDataCheck.Location = new System.Drawing.Point(12, 30);
            this.dumpRawDataCheck.Margin = new System.Windows.Forms.Padding(4);
            this.dumpRawDataCheck.Name = "dumpRawDataCheck";
            this.dumpRawDataCheck.Size = new System.Drawing.Size(241, 30);
            this.dumpRawDataCheck.TabIndex = 60;
            this.dumpRawDataCheck.Text = "Show NMEA Raw Data";
            // 
            // txtNMEARawData
            // 
            this.txtNMEARawData.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNMEARawData.Location = new System.Drawing.Point(6, 64);
            this.txtNMEARawData.Margin = new System.Windows.Forms.Padding(4);
            this.txtNMEARawData.Multiline = true;
            this.txtNMEARawData.Name = "txtNMEARawData";
            this.txtNMEARawData.ReadOnly = true;
            this.txtNMEARawData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNMEARawData.Size = new System.Drawing.Size(828, 116);
            this.txtNMEARawData.TabIndex = 8;
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.labelResult.ForeColor = System.Drawing.Color.Blue;
            this.labelResult.Location = new System.Drawing.Point(185, 6);
            this.labelResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(259, 55);
            this.labelResult.TabIndex = 91;
            this.labelResult.Text = "Not Result";
            // 
            // groupBoxGPSFixInfo
            // 
            this.groupBoxGPSFixInfo.Controls.Add(this.label8);
            this.groupBoxGPSFixInfo.Controls.Add(this.labelLatitude);
            this.groupBoxGPSFixInfo.Controls.Add(this.txtLatitude);
            this.groupBoxGPSFixInfo.Controls.Add(this.labelLongitude);
            this.groupBoxGPSFixInfo.Controls.Add(this.txtLongitude);
            this.groupBoxGPSFixInfo.Controls.Add(this.txtSatellitesSNRAverage);
            this.groupBoxGPSFixInfo.Controls.Add(this.labelAltitude);
            this.groupBoxGPSFixInfo.Controls.Add(this.labelSNR);
            this.groupBoxGPSFixInfo.Controls.Add(this.txtSeaLevel);
            this.groupBoxGPSFixInfo.Controls.Add(this.txtQuality);
            this.groupBoxGPSFixInfo.Controls.Add(this.labelFixMode);
            this.groupBoxGPSFixInfo.Controls.Add(this.txtGPSStatus);
            this.groupBoxGPSFixInfo.Controls.Add(this.txtGPSMode);
            this.groupBoxGPSFixInfo.Controls.Add(this.label11);
            this.groupBoxGPSFixInfo.Font = new System.Drawing.Font("Arial", 12F);
            this.groupBoxGPSFixInfo.Location = new System.Drawing.Point(4, 254);
            this.groupBoxGPSFixInfo.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxGPSFixInfo.Name = "groupBoxGPSFixInfo";
            this.groupBoxGPSFixInfo.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxGPSFixInfo.Size = new System.Drawing.Size(342, 350);
            this.groupBoxGPSFixInfo.TabIndex = 90;
            this.groupBoxGPSFixInfo.TabStop = false;
            this.groupBoxGPSFixInfo.Text = "GPS Fix Data ";
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Arial", 14F);
            this.label8.Location = new System.Drawing.Point(9, 220);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 25);
            this.label8.TabIndex = 43;
            this.label8.Text = "HDOP ";
            // 
            // labelLatitude
            // 
            this.labelLatitude.Font = new System.Drawing.Font("Arial", 14F);
            this.labelLatitude.Location = new System.Drawing.Point(9, 129);
            this.labelLatitude.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelLatitude.Name = "labelLatitude";
            this.labelLatitude.Size = new System.Drawing.Size(124, 25);
            this.labelLatitude.TabIndex = 47;
            this.labelLatitude.Text = "Latitude";
            // 
            // txtLatitude
            // 
            this.txtLatitude.Font = new System.Drawing.Font("Arial", 14F);
            this.txtLatitude.Location = new System.Drawing.Point(140, 125);
            this.txtLatitude.Margin = new System.Windows.Forms.Padding(4);
            this.txtLatitude.Name = "txtLatitude";
            this.txtLatitude.ReadOnly = true;
            this.txtLatitude.Size = new System.Drawing.Size(189, 34);
            this.txtLatitude.TabIndex = 13;
            this.txtLatitude.Text = "None";
            // 
            // labelLongitude
            // 
            this.labelLongitude.Font = new System.Drawing.Font("Arial", 14F);
            this.labelLongitude.Location = new System.Drawing.Point(9, 174);
            this.labelLongitude.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelLongitude.Name = "labelLongitude";
            this.labelLongitude.Size = new System.Drawing.Size(124, 32);
            this.labelLongitude.TabIndex = 46;
            this.labelLongitude.Text = "Longitude";
            // 
            // txtLongitude
            // 
            this.txtLongitude.Font = new System.Drawing.Font("Arial", 14F);
            this.txtLongitude.Location = new System.Drawing.Point(140, 170);
            this.txtLongitude.Margin = new System.Windows.Forms.Padding(4);
            this.txtLongitude.Name = "txtLongitude";
            this.txtLongitude.ReadOnly = true;
            this.txtLongitude.Size = new System.Drawing.Size(189, 34);
            this.txtLongitude.TabIndex = 19;
            this.txtLongitude.Text = "None";
            // 
            // txtSatellitesSNRAverage
            // 
            this.txtSatellitesSNRAverage.Font = new System.Drawing.Font("Arial", 14F);
            this.txtSatellitesSNRAverage.Location = new System.Drawing.Point(140, 76);
            this.txtSatellitesSNRAverage.Margin = new System.Windows.Forms.Padding(4);
            this.txtSatellitesSNRAverage.Name = "txtSatellitesSNRAverage";
            this.txtSatellitesSNRAverage.ReadOnly = true;
            this.txtSatellitesSNRAverage.Size = new System.Drawing.Size(189, 34);
            this.txtSatellitesSNRAverage.TabIndex = 64;
            this.txtSatellitesSNRAverage.Text = "0";
            // 
            // labelAltitude
            // 
            this.labelAltitude.Font = new System.Drawing.Font("Arial", 14F);
            this.labelAltitude.Location = new System.Drawing.Point(9, 268);
            this.labelAltitude.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelAltitude.Name = "labelAltitude";
            this.labelAltitude.Size = new System.Drawing.Size(104, 25);
            this.labelAltitude.TabIndex = 44;
            this.labelAltitude.Text = "Altitude";
            // 
            // labelSNR
            // 
            this.labelSNR.Font = new System.Drawing.Font("Arial", 14F);
            this.labelSNR.Location = new System.Drawing.Point(9, 80);
            this.labelSNR.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSNR.Name = "labelSNR";
            this.labelSNR.Size = new System.Drawing.Size(81, 25);
            this.labelSNR.TabIndex = 65;
            this.labelSNR.Text = "SNR (Avg)";
            // 
            // txtSeaLevel
            // 
            this.txtSeaLevel.Font = new System.Drawing.Font("Arial", 14F);
            this.txtSeaLevel.Location = new System.Drawing.Point(140, 262);
            this.txtSeaLevel.Margin = new System.Windows.Forms.Padding(4);
            this.txtSeaLevel.Name = "txtSeaLevel";
            this.txtSeaLevel.ReadOnly = true;
            this.txtSeaLevel.Size = new System.Drawing.Size(189, 34);
            this.txtSeaLevel.TabIndex = 29;
            this.txtSeaLevel.Text = "0";
            // 
            // txtQuality
            // 
            this.txtQuality.Font = new System.Drawing.Font("Arial", 14F);
            this.txtQuality.Location = new System.Drawing.Point(140, 214);
            this.txtQuality.Margin = new System.Windows.Forms.Padding(4);
            this.txtQuality.Name = "txtQuality";
            this.txtQuality.ReadOnly = true;
            this.txtQuality.Size = new System.Drawing.Size(189, 34);
            this.txtQuality.TabIndex = 33;
            this.txtQuality.Text = "None";
            // 
            // labelFixMode
            // 
            this.labelFixMode.Font = new System.Drawing.Font("Arial", 14F);
            this.labelFixMode.Location = new System.Drawing.Point(9, 30);
            this.labelFixMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFixMode.Name = "labelFixMode";
            this.labelFixMode.Size = new System.Drawing.Size(124, 25);
            this.labelFixMode.TabIndex = 55;
            this.labelFixMode.Text = "Fix Mode";
            // 
            // txtGPSStatus
            // 
            this.txtGPSStatus.Font = new System.Drawing.Font("Arial", 14F);
            this.txtGPSStatus.Location = new System.Drawing.Point(140, 306);
            this.txtGPSStatus.Margin = new System.Windows.Forms.Padding(4);
            this.txtGPSStatus.Name = "txtGPSStatus";
            this.txtGPSStatus.ReadOnly = true;
            this.txtGPSStatus.Size = new System.Drawing.Size(189, 34);
            this.txtGPSStatus.TabIndex = 56;
            this.txtGPSStatus.Text = "None";
            // 
            // txtGPSMode
            // 
            this.txtGPSMode.Font = new System.Drawing.Font("Arial", 14F);
            this.txtGPSMode.Location = new System.Drawing.Point(140, 26);
            this.txtGPSMode.Margin = new System.Windows.Forms.Padding(4);
            this.txtGPSMode.Name = "txtGPSMode";
            this.txtGPSMode.ReadOnly = true;
            this.txtGPSMode.Size = new System.Drawing.Size(189, 34);
            this.txtGPSMode.TabIndex = 54;
            this.txtGPSMode.Text = "None";
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Arial", 14F);
            this.label11.Location = new System.Drawing.Point(9, 310);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(124, 25);
            this.label11.TabIndex = 57;
            this.label11.Text = "Fix Status";
            // 
            // groupBoxSatellitesDetails
            // 
            this.groupBoxSatellitesDetails.Controls.Add(this.listSatellites);
            this.groupBoxSatellitesDetails.Controls.Add(this.labelSatelitesInView);
            this.groupBoxSatellitesDetails.Controls.Add(this.textBoxSatellitesInView);
            this.groupBoxSatellitesDetails.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSatellitesDetails.Location = new System.Drawing.Point(354, 138);
            this.groupBoxSatellitesDetails.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxSatellitesDetails.Name = "groupBoxSatellitesDetails";
            this.groupBoxSatellitesDetails.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxSatellitesDetails.Size = new System.Drawing.Size(495, 466);
            this.groupBoxSatellitesDetails.TabIndex = 89;
            this.groupBoxSatellitesDetails.TabStop = false;
            this.groupBoxSatellitesDetails.Text = "Satelites Details";
            // 
            // listSatellites
            // 
            this.listSatellites.AllowColumnReorder = true;
            this.listSatellites.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PRNColumn,
            this.ElevationColumn,
            this.AzimuthColumn,
            this.SNRColumn});
            this.listSatellites.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listSatellites.FullRowSelect = true;
            this.listSatellites.Location = new System.Drawing.Point(12, 75);
            this.listSatellites.Margin = new System.Windows.Forms.Padding(4);
            this.listSatellites.Name = "listSatellites";
            this.listSatellites.Size = new System.Drawing.Size(472, 383);
            this.listSatellites.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listSatellites.TabIndex = 61;
            this.listSatellites.UseCompatibleStateImageBehavior = false;
            this.listSatellites.View = System.Windows.Forms.View.Details;
            // 
            // PRNColumn
            // 
            this.PRNColumn.Text = "PRN";
            this.PRNColumn.Width = 69;
            // 
            // ElevationColumn
            // 
            this.ElevationColumn.Text = "Elevation";
            this.ElevationColumn.Width = 99;
            // 
            // AzimuthColumn
            // 
            this.AzimuthColumn.Text = "Azimuth";
            this.AzimuthColumn.Width = 99;
            // 
            // SNRColumn
            // 
            this.SNRColumn.Text = "SNR";
            this.SNRColumn.Width = 87;
            // 
            // labelSatelitesInView
            // 
            this.labelSatelitesInView.Font = new System.Drawing.Font("Arial", 14F);
            this.labelSatelitesInView.Location = new System.Drawing.Point(9, 36);
            this.labelSatelitesInView.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSatelitesInView.Name = "labelSatelitesInView";
            this.labelSatelitesInView.Size = new System.Drawing.Size(150, 25);
            this.labelSatelitesInView.TabIndex = 63;
            this.labelSatelitesInView.Text = "Satelites in View";
            // 
            // textBoxSatellitesInView
            // 
            this.textBoxSatellitesInView.Font = new System.Drawing.Font("Arial", 14F);
            this.textBoxSatellitesInView.Location = new System.Drawing.Point(166, 32);
            this.textBoxSatellitesInView.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxSatellitesInView.Name = "textBoxSatellitesInView";
            this.textBoxSatellitesInView.ReadOnly = true;
            this.textBoxSatellitesInView.Size = new System.Drawing.Size(49, 34);
            this.textBoxSatellitesInView.TabIndex = 62;
            this.textBoxSatellitesInView.Text = "0";
            // 
            // labelResultTopics
            // 
            this.labelResultTopics.Font = new System.Drawing.Font("Arial", 24F);
            this.labelResultTopics.Location = new System.Drawing.Point(10, 14);
            this.labelResultTopics.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResultTopics.Name = "labelResultTopics";
            this.labelResultTopics.Size = new System.Drawing.Size(168, 46);
            this.labelResultTopics.TabIndex = 88;
            this.labelResultTopics.Text = "Result :";
            // 
            // txtGPSFixTime
            // 
            this.txtGPSFixTime.Font = new System.Drawing.Font("Arial", 14F);
            this.txtGPSFixTime.Location = new System.Drawing.Point(296, 64);
            this.txtGPSFixTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtGPSFixTime.Name = "txtGPSFixTime";
            this.txtGPSFixTime.ReadOnly = true;
            this.txtGPSFixTime.Size = new System.Drawing.Size(39, 34);
            this.txtGPSFixTime.TabIndex = 86;
            this.txtGPSFixTime.Text = "0";
            // 
            // labelFixTime
            // 
            this.labelFixTime.Font = new System.Drawing.Font("Arial", 14F);
            this.labelFixTime.Location = new System.Drawing.Point(176, 68);
            this.labelFixTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelFixTime.Name = "labelFixTime";
            this.labelFixTime.Size = new System.Drawing.Size(112, 25);
            this.labelFixTime.TabIndex = 87;
            this.labelFixTime.Text = "Fix Time";
            // 
            // txtGPSConnectTime
            // 
            this.txtGPSConnectTime.Font = new System.Drawing.Font("Arial", 14F);
            this.txtGPSConnectTime.Location = new System.Drawing.Point(296, 24);
            this.txtGPSConnectTime.Margin = new System.Windows.Forms.Padding(4);
            this.txtGPSConnectTime.Name = "txtGPSConnectTime";
            this.txtGPSConnectTime.ReadOnly = true;
            this.txtGPSConnectTime.Size = new System.Drawing.Size(39, 34);
            this.txtGPSConnectTime.TabIndex = 84;
            this.txtGPSConnectTime.Text = "0";
            // 
            // labelTestTime
            // 
            this.labelTestTime.Font = new System.Drawing.Font("Arial", 14F);
            this.labelTestTime.Location = new System.Drawing.Point(176, 29);
            this.labelTestTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTestTime.Name = "labelTestTime";
            this.labelTestTime.Size = new System.Drawing.Size(112, 25);
            this.labelTestTime.TabIndex = 85;
            this.labelTestTime.Text = "TestTime";
            // 
            // txtSTQty
            // 
            this.txtSTQty.Font = new System.Drawing.Font("Arial", 14F);
            this.txtSTQty.Location = new System.Drawing.Point(90, 64);
            this.txtSTQty.Margin = new System.Windows.Forms.Padding(4);
            this.txtSTQty.Name = "txtSTQty";
            this.txtSTQty.ReadOnly = true;
            this.txtSTQty.Size = new System.Drawing.Size(78, 34);
            this.txtSTQty.TabIndex = 80;
            this.txtSTQty.Text = "0";
            // 
            // labelSsatellitesInUse
            // 
            this.labelSsatellitesInUse.Font = new System.Drawing.Font("Arial", 14F);
            this.labelSsatellitesInUse.Location = new System.Drawing.Point(9, 64);
            this.labelSsatellitesInUse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSsatellitesInUse.Name = "labelSsatellitesInUse";
            this.labelSsatellitesInUse.Size = new System.Drawing.Size(81, 25);
            this.labelSsatellitesInUse.TabIndex = 81;
            this.labelSsatellitesInUse.Text = "In Use";
            // 
            // txtPortStatus
            // 
            this.txtPortStatus.Font = new System.Drawing.Font("Arial", 14F);
            this.txtPortStatus.Location = new System.Drawing.Point(90, 21);
            this.txtPortStatus.Margin = new System.Windows.Forms.Padding(4);
            this.txtPortStatus.Name = "txtPortStatus";
            this.txtPortStatus.ReadOnly = true;
            this.txtPortStatus.Size = new System.Drawing.Size(78, 34);
            this.txtPortStatus.TabIndex = 79;
            this.txtPortStatus.Text = "Close";
            // 
            // labelPort
            // 
            this.labelPort.Font = new System.Drawing.Font("Arial", 14F);
            this.labelPort.Location = new System.Drawing.Point(9, 28);
            this.labelPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(62, 25);
            this.labelPort.TabIndex = 82;
            this.labelPort.Text = "Port";
            // 
            // buttonDisconnectGPS
            // 
            this.buttonDisconnectGPS.Enabled = false;
            this.buttonDisconnectGPS.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.buttonDisconnectGPS.Location = new System.Drawing.Point(211, 68);
            this.buttonDisconnectGPS.Margin = new System.Windows.Forms.Padding(4);
            this.buttonDisconnectGPS.Name = "buttonDisconnectGPS";
            this.buttonDisconnectGPS.Size = new System.Drawing.Size(188, 62);
            this.buttonDisconnectGPS.TabIndex = 77;
            this.buttonDisconnectGPS.Text = "Disconnect";
            this.buttonDisconnectGPS.Click += new System.EventHandler(this.buttonDisconnectGPS_Click);
            // 
            // buttonConnectGPS
            // 
            this.buttonConnectGPS.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.buttonConnectGPS.Location = new System.Drawing.Point(16, 68);
            this.buttonConnectGPS.Margin = new System.Windows.Forms.Padding(4);
            this.buttonConnectGPS.Name = "buttonConnectGPS";
            this.buttonConnectGPS.Size = new System.Drawing.Size(188, 62);
            this.buttonConnectGPS.TabIndex = 76;
            this.buttonConnectGPS.Text = "Connect";
            this.buttonConnectGPS.Click += new System.EventHandler(this.buttonConnectGPS_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 14F);
            this.label1.Location = new System.Drawing.Point(472, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 25);
            this.label1.TabIndex = 83;
            this.label1.Text = "GPS Port";
            this.label1.Visible = false;
            // 
            // groupBoxGPSdata
            // 
            this.groupBoxGPSdata.Controls.Add(this.labelPort);
            this.groupBoxGPSdata.Controls.Add(this.txtPortStatus);
            this.groupBoxGPSdata.Controls.Add(this.labelSsatellitesInUse);
            this.groupBoxGPSdata.Controls.Add(this.txtSTQty);
            this.groupBoxGPSdata.Controls.Add(this.labelTestTime);
            this.groupBoxGPSdata.Controls.Add(this.txtGPSConnectTime);
            this.groupBoxGPSdata.Controls.Add(this.txtGPSFixTime);
            this.groupBoxGPSdata.Controls.Add(this.labelFixTime);
            this.groupBoxGPSdata.Font = new System.Drawing.Font("Arial", 12F);
            this.groupBoxGPSdata.Location = new System.Drawing.Point(4, 138);
            this.groupBoxGPSdata.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxGPSdata.Name = "groupBoxGPSdata";
            this.groupBoxGPSdata.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxGPSdata.Size = new System.Drawing.Size(342, 112);
            this.groupBoxGPSdata.TabIndex = 93;
            this.groupBoxGPSdata.TabStop = false;
            this.groupBoxGPSdata.Text = "GPS Data";
            // 
            // comboBoxSerialPortGPS
            // 
            this.comboBoxSerialPortGPS.Font = new System.Drawing.Font("Tahoma", 16F);
            this.comboBoxSerialPortGPS.Items.AddRange(new object[] {
            "COM3",
            "COM9"});
            this.comboBoxSerialPortGPS.Location = new System.Drawing.Point(609, 19);
            this.comboBoxSerialPortGPS.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxSerialPortGPS.Name = "comboBoxSerialPortGPS";
            this.comboBoxSerialPortGPS.Size = new System.Drawing.Size(222, 41);
            this.comboBoxSerialPortGPS.TabIndex = 78;
            this.comboBoxSerialPortGPS.Visible = false;
            // 
            // gps
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(852, 875);
            this.Controls.Add(this.groupBoxGPSdata);
            this.Controls.Add(this.groupBoxGPSRawData);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.groupBoxGPSFixInfo);
            this.Controls.Add(this.groupBoxSatellitesDetails);
            this.Controls.Add(this.labelResultTopics);
            this.Controls.Add(this.comboBoxSerialPortGPS);
            this.Controls.Add(this.buttonDisconnectGPS);
            this.Controls.Add(this.buttonConnectGPS);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "gps";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.GPS_Load);
            this.groupBoxGPSRawData.ResumeLayout(false);
            this.groupBoxGPSRawData.PerformLayout();
            this.groupBoxGPSFixInfo.ResumeLayout(false);
            this.groupBoxGPSFixInfo.PerformLayout();
            this.groupBoxSatellitesDetails.ResumeLayout(false);
            this.groupBoxSatellitesDetails.PerformLayout();
            this.groupBoxGPSdata.ResumeLayout(false);
            this.groupBoxGPSdata.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxGPSRawData;
        private System.Windows.Forms.CheckBox dumpRawDataCheck;
        private System.Windows.Forms.TextBox txtNMEARawData;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.GroupBox groupBoxGPSFixInfo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelLatitude;
        private System.Windows.Forms.TextBox txtLatitude;
        private System.Windows.Forms.Label labelLongitude;
        private System.Windows.Forms.TextBox txtLongitude;
        private System.Windows.Forms.TextBox txtSatellitesSNRAverage;
        private System.Windows.Forms.Label labelAltitude;
        private System.Windows.Forms.Label labelSNR;
        private System.Windows.Forms.TextBox txtSeaLevel;
        private System.Windows.Forms.TextBox txtQuality;
        private System.Windows.Forms.Label labelFixMode;
        private System.Windows.Forms.TextBox txtGPSStatus;
        private System.Windows.Forms.TextBox txtGPSMode;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBoxSatellitesDetails;
        private System.Windows.Forms.ListView listSatellites;
        private System.Windows.Forms.ColumnHeader PRNColumn;
        private System.Windows.Forms.ColumnHeader ElevationColumn;
        private System.Windows.Forms.ColumnHeader AzimuthColumn;
        private System.Windows.Forms.ColumnHeader SNRColumn;
        private System.Windows.Forms.Label labelSatelitesInView;
        private System.Windows.Forms.TextBox textBoxSatellitesInView;
        private System.Windows.Forms.Label labelResultTopics;
        private System.Windows.Forms.TextBox txtGPSFixTime;
        private System.Windows.Forms.Label labelFixTime;
        private System.Windows.Forms.TextBox txtGPSConnectTime;
        private System.Windows.Forms.Label labelTestTime;
        private System.Windows.Forms.TextBox txtSTQty;
        private System.Windows.Forms.Label labelSsatellitesInUse;
        private System.Windows.Forms.TextBox txtPortStatus;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.Button buttonDisconnectGPS;
        private System.Windows.Forms.Button buttonConnectGPS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxGPSdata;
        private System.Windows.Forms.ComboBox comboBoxSerialPortGPS;
    }
}

