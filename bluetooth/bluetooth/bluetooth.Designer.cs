namespace bluetooth
{
    partial class bluetooth
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
            this.txtBTDeviceDetails = new System.Windows.Forms.TextBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonSendFile = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonFindBluetoothDevice = new System.Windows.Forms.Button();
            this.comboBoxBluetoothDevices = new System.Windows.Forms.ComboBox();
            this.TextBoxPin = new System.Windows.Forms.TextBox();
            this.ComboBoxServices = new System.Windows.Forms.ComboBox();
            this.BTServicesTopic = new System.Windows.Forms.Label();
            this.labelDeviceListTopic = new System.Windows.Forms.Label();
            this.buttonBTServiceListen = new System.Windows.Forms.Button();
            this.CheckBoxUsePin = new System.Windows.Forms.CheckBox();
            this.CheckBoxEncrypt = new System.Windows.Forms.CheckBox();
            this.CheckBoxAuthenticate = new System.Windows.Forms.CheckBox();
            this.buttonBTServiceSend = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.groupDeviceDetails = new System.Windows.Forms.GroupBox();
            this.groupBoxBluetoothConnect = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxEncoding = new System.Windows.Forms.ComboBox();
            this.CheckBoxSetServiceState = new System.Windows.Forms.CheckBox();
            this.buttonAudioTest = new System.Windows.Forms.Button();
            this.groupFindBTDevice = new System.Windows.Forms.GroupBox();
            this.labelBtLocalAddress = new System.Windows.Forms.Label();
            this.labelBtLocalAddressTopic = new System.Windows.Forms.Label();
            this.groupBTDeviceConnection = new System.Windows.Forms.GroupBox();
            this.buttonFileTestFAIL = new System.Windows.Forms.Button();
            this.txtCompareFileResult = new System.Windows.Forms.Label();
            this.buttonReceiveFile = new System.Windows.Forms.Button();
            this.groupTest = new System.Windows.Forms.GroupBox();
            this.buttonFsquirt = new System.Windows.Forms.Button();
            this.BluetoothOBEXTimer = new System.Windows.Forms.Timer(this.components);
            this.buttonAudioPASS = new System.Windows.Forms.Button();
            this.buttonAudioFAIL = new System.Windows.Forms.Button();
            this.buttonPairRequest = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.groupBoxBluetoothPair = new System.Windows.Forms.GroupBox();
            this.labelResultTopics = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.groupDeviceDetails.SuspendLayout();
            this.groupBoxBluetoothConnect.SuspendLayout();
            this.groupFindBTDevice.SuspendLayout();
            this.groupBTDeviceConnection.SuspendLayout();
            this.groupTest.SuspendLayout();
            this.groupBoxBluetoothPair.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtBTDeviceDetails
            // 
            this.txtBTDeviceDetails.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBTDeviceDetails.Location = new System.Drawing.Point(8, 41);
            this.txtBTDeviceDetails.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtBTDeviceDetails.Multiline = true;
            this.txtBTDeviceDetails.Name = "txtBTDeviceDetails";
            this.txtBTDeviceDetails.ReadOnly = true;
            this.txtBTDeviceDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBTDeviceDetails.Size = new System.Drawing.Size(671, 178);
            this.txtBTDeviceDetails.TabIndex = 9;
            // 
            // buttonExit
            // 
            this.buttonExit.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExit.Location = new System.Drawing.Point(8, 219);
            this.buttonExit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(160, 38);
            this.buttonExit.TabIndex = 25;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Visible = false;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonSendFile
            // 
            this.buttonSendFile.Enabled = false;
            this.buttonSendFile.Font = new System.Drawing.Font("Arial", 21F, System.Drawing.FontStyle.Bold);
            this.buttonSendFile.Location = new System.Drawing.Point(460, 36);
            this.buttonSendFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonSendFile.Name = "buttonSendFile";
            this.buttonSendFile.Size = new System.Drawing.Size(200, 62);
            this.buttonSendFile.TabIndex = 24;
            this.buttonSendFile.TabStop = false;
            this.buttonSendFile.Text = "Send File";
            this.buttonSendFile.UseVisualStyleBackColor = true;
            this.buttonSendFile.Click += new System.EventHandler(this.buttonSendFile_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.Enabled = false;
            this.buttonConnect.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConnect.Location = new System.Drawing.Point(8, 185);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(160, 26);
            this.buttonConnect.TabIndex = 23;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // buttonFindBluetoothDevice
            // 
            this.buttonFindBluetoothDevice.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonFindBluetoothDevice.Location = new System.Drawing.Point(489, 29);
            this.buttonFindBluetoothDevice.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonFindBluetoothDevice.Name = "buttonFindBluetoothDevice";
            this.buttonFindBluetoothDevice.Size = new System.Drawing.Size(200, 62);
            this.buttonFindBluetoothDevice.TabIndex = 22;
            this.buttonFindBluetoothDevice.Text = "Find BT";
            this.buttonFindBluetoothDevice.UseVisualStyleBackColor = true;
            this.buttonFindBluetoothDevice.Click += new System.EventHandler(this.buttonFindBluetoothDevice_Click);
            // 
            // comboBoxBluetoothDevices
            // 
            this.comboBoxBluetoothDevices.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxBluetoothDevices.Location = new System.Drawing.Point(131, 45);
            this.comboBoxBluetoothDevices.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxBluetoothDevices.Name = "comboBoxBluetoothDevices";
            this.comboBoxBluetoothDevices.Size = new System.Drawing.Size(349, 39);
            this.comboBoxBluetoothDevices.TabIndex = 27;
            // 
            // TextBoxPin
            // 
            this.TextBoxPin.Enabled = false;
            this.TextBoxPin.Location = new System.Drawing.Point(89, 214);
            this.TextBoxPin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TextBoxPin.Name = "TextBoxPin";
            this.TextBoxPin.Size = new System.Drawing.Size(91, 25);
            this.TextBoxPin.TabIndex = 30;
            // 
            // ComboBoxServices
            // 
            this.ComboBoxServices.Enabled = false;
            this.ComboBoxServices.Location = new System.Drawing.Point(11, 41);
            this.ComboBoxServices.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ComboBoxServices.Name = "ComboBoxServices";
            this.ComboBoxServices.Size = new System.Drawing.Size(161, 23);
            this.ComboBoxServices.TabIndex = 32;
            // 
            // BTServicesTopic
            // 
            this.BTServicesTopic.AutoSize = true;
            this.BTServicesTopic.Location = new System.Drawing.Point(8, 22);
            this.BTServicesTopic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.BTServicesTopic.Name = "BTServicesTopic";
            this.BTServicesTopic.Size = new System.Drawing.Size(49, 15);
            this.BTServicesTopic.TabIndex = 33;
            this.BTServicesTopic.Text = "Service";
            // 
            // labelDeviceListTopic
            // 
            this.labelDeviceListTopic.AutoSize = true;
            this.labelDeviceListTopic.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDeviceListTopic.Location = new System.Drawing.Point(11, 50);
            this.labelDeviceListTopic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDeviceListTopic.Name = "labelDeviceListTopic";
            this.labelDeviceListTopic.Size = new System.Drawing.Size(112, 32);
            this.labelDeviceListTopic.TabIndex = 34;
            this.labelDeviceListTopic.Text = "Devices";
            // 
            // buttonBTServiceListen
            // 
            this.buttonBTServiceListen.Location = new System.Drawing.Point(8, 60);
            this.buttonBTServiceListen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonBTServiceListen.Name = "buttonBTServiceListen";
            this.buttonBTServiceListen.Size = new System.Drawing.Size(160, 26);
            this.buttonBTServiceListen.TabIndex = 35;
            this.buttonBTServiceListen.Text = "藍牙對傳 Server";
            this.buttonBTServiceListen.UseVisualStyleBackColor = true;
            this.buttonBTServiceListen.Click += new System.EventHandler(this.buttonBTServiceListen_Click);
            // 
            // CheckBoxUsePin
            // 
            this.CheckBoxUsePin.AutoSize = true;
            this.CheckBoxUsePin.Enabled = false;
            this.CheckBoxUsePin.Location = new System.Drawing.Point(11, 216);
            this.CheckBoxUsePin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CheckBoxUsePin.Name = "CheckBoxUsePin";
            this.CheckBoxUsePin.Size = new System.Drawing.Size(73, 19);
            this.CheckBoxUsePin.TabIndex = 38;
            this.CheckBoxUsePin.Text = "Use &Pin";
            this.CheckBoxUsePin.UseVisualStyleBackColor = true;
            this.CheckBoxUsePin.CheckedChanged += new System.EventHandler(this.CheckBoxUsePin_CheckedChanged);
            // 
            // CheckBoxEncrypt
            // 
            this.CheckBoxEncrypt.AutoSize = true;
            this.CheckBoxEncrypt.Enabled = false;
            this.CheckBoxEncrypt.Location = new System.Drawing.Point(11, 161);
            this.CheckBoxEncrypt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CheckBoxEncrypt.Name = "CheckBoxEncrypt";
            this.CheckBoxEncrypt.Size = new System.Drawing.Size(74, 19);
            this.CheckBoxEncrypt.TabIndex = 37;
            this.CheckBoxEncrypt.Text = "&Encrypt";
            this.CheckBoxEncrypt.UseVisualStyleBackColor = true;
            // 
            // CheckBoxAuthenticate
            // 
            this.CheckBoxAuthenticate.AutoSize = true;
            this.CheckBoxAuthenticate.Enabled = false;
            this.CheckBoxAuthenticate.Location = new System.Drawing.Point(11, 189);
            this.CheckBoxAuthenticate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CheckBoxAuthenticate.Name = "CheckBoxAuthenticate";
            this.CheckBoxAuthenticate.Size = new System.Drawing.Size(100, 19);
            this.CheckBoxAuthenticate.TabIndex = 36;
            this.CheckBoxAuthenticate.Text = "&Authenticate";
            this.CheckBoxAuthenticate.UseVisualStyleBackColor = true;
            // 
            // buttonBTServiceSend
            // 
            this.buttonBTServiceSend.Location = new System.Drawing.Point(8, 91);
            this.buttonBTServiceSend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonBTServiceSend.Name = "buttonBTServiceSend";
            this.buttonBTServiceSend.Size = new System.Drawing.Size(160, 26);
            this.buttonBTServiceSend.TabIndex = 40;
            this.buttonBTServiceSend.Text = "藍牙對傳 Client";
            this.buttonBTServiceSend.UseVisualStyleBackColor = true;
            this.buttonBTServiceSend.Click += new System.EventHandler(this.buttonBTServiceSend_Click);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Location = new System.Drawing.Point(8, 26);
            this.buttonDisconnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(160, 26);
            this.buttonDisconnect.TabIndex = 43;
            this.buttonDisconnect.Text = "中斷藍牙配對";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // groupDeviceDetails
            // 
            this.groupDeviceDetails.Controls.Add(this.txtBTDeviceDetails);
            this.groupDeviceDetails.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupDeviceDetails.Location = new System.Drawing.Point(24, 582);
            this.groupDeviceDetails.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupDeviceDetails.Name = "groupDeviceDetails";
            this.groupDeviceDetails.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupDeviceDetails.Size = new System.Drawing.Size(697, 237);
            this.groupDeviceDetails.TabIndex = 45;
            this.groupDeviceDetails.TabStop = false;
            this.groupDeviceDetails.Text = "Bluetooth Device Details";
            // 
            // groupBoxBluetoothConnect
            // 
            this.groupBoxBluetoothConnect.Controls.Add(this.label7);
            this.groupBoxBluetoothConnect.Controls.Add(this.comboBoxEncoding);
            this.groupBoxBluetoothConnect.Controls.Add(this.CheckBoxSetServiceState);
            this.groupBoxBluetoothConnect.Controls.Add(this.BTServicesTopic);
            this.groupBoxBluetoothConnect.Controls.Add(this.TextBoxPin);
            this.groupBoxBluetoothConnect.Controls.Add(this.ComboBoxServices);
            this.groupBoxBluetoothConnect.Controls.Add(this.CheckBoxUsePin);
            this.groupBoxBluetoothConnect.Controls.Add(this.CheckBoxAuthenticate);
            this.groupBoxBluetoothConnect.Controls.Add(this.CheckBoxEncrypt);
            this.groupBoxBluetoothConnect.Location = new System.Drawing.Point(732, 126);
            this.groupBoxBluetoothConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxBluetoothConnect.Name = "groupBoxBluetoothConnect";
            this.groupBoxBluetoothConnect.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxBluetoothConnect.Size = new System.Drawing.Size(189, 251);
            this.groupBoxBluetoothConnect.TabIndex = 46;
            this.groupBoxBluetoothConnect.TabStop = false;
            this.groupBoxBluetoothConnect.Text = "配對設定 (Connect)";
            this.groupBoxBluetoothConnect.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 79);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(91, 15);
            this.label7.TabIndex = 42;
            this.label7.Text = "Text Encoding";
            // 
            // comboBoxEncoding
            // 
            this.comboBoxEncoding.Enabled = false;
            this.comboBoxEncoding.FormattingEnabled = true;
            this.comboBoxEncoding.Items.AddRange(new object[] {
            "x-IA5",
            "iso-8859-1",
            "utf-8",
            "ASCII"});
            this.comboBoxEncoding.Location = new System.Drawing.Point(11, 98);
            this.comboBoxEncoding.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxEncoding.Name = "comboBoxEncoding";
            this.comboBoxEncoding.Size = new System.Drawing.Size(161, 23);
            this.comboBoxEncoding.TabIndex = 43;
            // 
            // CheckBoxSetServiceState
            // 
            this.CheckBoxSetServiceState.AutoSize = true;
            this.CheckBoxSetServiceState.Enabled = false;
            this.CheckBoxSetServiceState.Location = new System.Drawing.Point(11, 134);
            this.CheckBoxSetServiceState.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CheckBoxSetServiceState.Name = "CheckBoxSetServiceState";
            this.CheckBoxSetServiceState.Size = new System.Drawing.Size(117, 19);
            this.CheckBoxSetServiceState.TabIndex = 39;
            this.CheckBoxSetServiceState.Text = "SetServiceState";
            this.CheckBoxSetServiceState.UseVisualStyleBackColor = true;
            // 
            // buttonAudioTest
            // 
            this.buttonAudioTest.Enabled = false;
            this.buttonAudioTest.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            this.buttonAudioTest.Location = new System.Drawing.Point(424, 30);
            this.buttonAudioTest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonAudioTest.Name = "buttonAudioTest";
            this.buttonAudioTest.Size = new System.Drawing.Size(200, 62);
            this.buttonAudioTest.TabIndex = 25;
            this.buttonAudioTest.TabStop = false;
            this.buttonAudioTest.Text = "Audio Test";
            this.buttonAudioTest.UseVisualStyleBackColor = true;
            this.buttonAudioTest.Click += new System.EventHandler(this.buttonAudioTest_Click);
            // 
            // groupFindBTDevice
            // 
            this.groupFindBTDevice.Controls.Add(this.labelBtLocalAddress);
            this.groupFindBTDevice.Controls.Add(this.labelBtLocalAddressTopic);
            this.groupFindBTDevice.Controls.Add(this.labelDeviceListTopic);
            this.groupFindBTDevice.Controls.Add(this.comboBoxBluetoothDevices);
            this.groupFindBTDevice.Controls.Add(this.buttonFindBluetoothDevice);
            this.groupFindBTDevice.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupFindBTDevice.Location = new System.Drawing.Point(23, 126);
            this.groupFindBTDevice.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupFindBTDevice.Name = "groupFindBTDevice";
            this.groupFindBTDevice.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupFindBTDevice.Size = new System.Drawing.Size(697, 145);
            this.groupFindBTDevice.TabIndex = 47;
            this.groupFindBTDevice.TabStop = false;
            this.groupFindBTDevice.Text = "Find Bluetooth Device";
            // 
            // labelBtLocalAddress
            // 
            this.labelBtLocalAddress.AutoSize = true;
            this.labelBtLocalAddress.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold);
            this.labelBtLocalAddress.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelBtLocalAddress.Location = new System.Drawing.Point(236, 102);
            this.labelBtLocalAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBtLocalAddress.Name = "labelBtLocalAddress";
            this.labelBtLocalAddress.Size = new System.Drawing.Size(80, 40);
            this.labelBtLocalAddress.TabIndex = 36;
            this.labelBtLocalAddress.Text = "Null";
            // 
            // labelBtLocalAddressTopic
            // 
            this.labelBtLocalAddressTopic.AutoSize = true;
            this.labelBtLocalAddressTopic.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBtLocalAddressTopic.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelBtLocalAddressTopic.Location = new System.Drawing.Point(15, 105);
            this.labelBtLocalAddressTopic.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBtLocalAddressTopic.Name = "labelBtLocalAddressTopic";
            this.labelBtLocalAddressTopic.Size = new System.Drawing.Size(211, 32);
            this.labelBtLocalAddressTopic.TabIndex = 35;
            this.labelBtLocalAddressTopic.Text = "Local Address : ";
            // 
            // groupBTDeviceConnection
            // 
            this.groupBTDeviceConnection.Controls.Add(this.buttonFileTestFAIL);
            this.groupBTDeviceConnection.Controls.Add(this.txtCompareFileResult);
            this.groupBTDeviceConnection.Controls.Add(this.buttonSendFile);
            this.groupBTDeviceConnection.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBTDeviceConnection.Location = new System.Drawing.Point(24, 458);
            this.groupBTDeviceConnection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBTDeviceConnection.Name = "groupBTDeviceConnection";
            this.groupBTDeviceConnection.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBTDeviceConnection.Size = new System.Drawing.Size(697, 118);
            this.groupBTDeviceConnection.TabIndex = 48;
            this.groupBTDeviceConnection.TabStop = false;
            this.groupBTDeviceConnection.Text = "Bluetooth File Transfer";
            this.groupBTDeviceConnection.Visible = false;
            // 
            // buttonFileTestFAIL
            // 
            this.buttonFileTestFAIL.Enabled = false;
            this.buttonFileTestFAIL.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonFileTestFAIL.ForeColor = System.Drawing.Color.Red;
            this.buttonFileTestFAIL.Location = new System.Drawing.Point(252, 36);
            this.buttonFileTestFAIL.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonFileTestFAIL.Name = "buttonFileTestFAIL";
            this.buttonFileTestFAIL.Size = new System.Drawing.Size(200, 62);
            this.buttonFileTestFAIL.TabIndex = 56;
            this.buttonFileTestFAIL.TabStop = false;
            this.buttonFileTestFAIL.Text = "FAIL";
            this.buttonFileTestFAIL.UseVisualStyleBackColor = true;
            this.buttonFileTestFAIL.Click += new System.EventHandler(this.buttonFileTestFAIL_Click);
            // 
            // txtCompareFileResult
            // 
            this.txtCompareFileResult.AutoSize = true;
            this.txtCompareFileResult.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.txtCompareFileResult.ForeColor = System.Drawing.Color.Blue;
            this.txtCompareFileResult.Location = new System.Drawing.Point(9, 45);
            this.txtCompareFileResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.txtCompareFileResult.Name = "txtCompareFileResult";
            this.txtCompareFileResult.Size = new System.Drawing.Size(217, 46);
            this.txtCompareFileResult.TabIndex = 32;
            this.txtCompareFileResult.Text = "Not Result";
            // 
            // buttonReceiveFile
            // 
            this.buttonReceiveFile.Location = new System.Drawing.Point(8, 122);
            this.buttonReceiveFile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonReceiveFile.Name = "buttonReceiveFile";
            this.buttonReceiveFile.Size = new System.Drawing.Size(160, 26);
            this.buttonReceiveFile.TabIndex = 25;
            this.buttonReceiveFile.Text = "接收檔案(Listener)";
            this.buttonReceiveFile.UseVisualStyleBackColor = true;
            this.buttonReceiveFile.Click += new System.EventHandler(this.buttonReceiveFile_Click);
            // 
            // groupTest
            // 
            this.groupTest.Controls.Add(this.buttonFsquirt);
            this.groupTest.Controls.Add(this.buttonReceiveFile);
            this.groupTest.Controls.Add(this.buttonBTServiceListen);
            this.groupTest.Controls.Add(this.buttonBTServiceSend);
            this.groupTest.Controls.Add(this.buttonDisconnect);
            this.groupTest.Controls.Add(this.buttonConnect);
            this.groupTest.Controls.Add(this.buttonExit);
            this.groupTest.Location = new System.Drawing.Point(732, 385);
            this.groupTest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupTest.Name = "groupTest";
            this.groupTest.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupTest.Size = new System.Drawing.Size(189, 265);
            this.groupTest.TabIndex = 50;
            this.groupTest.TabStop = false;
            this.groupTest.Text = "Test Zone";
            this.groupTest.Visible = false;
            // 
            // buttonFsquirt
            // 
            this.buttonFsquirt.Location = new System.Drawing.Point(8, 156);
            this.buttonFsquirt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonFsquirt.Name = "buttonFsquirt";
            this.buttonFsquirt.Size = new System.Drawing.Size(160, 26);
            this.buttonFsquirt.TabIndex = 50;
            this.buttonFsquirt.Text = "藍牙檔案傳輸精靈";
            this.buttonFsquirt.UseVisualStyleBackColor = true;
            this.buttonFsquirt.Click += new System.EventHandler(this.buttonFsquirt_Click);
            // 
            // BluetoothOBEXTimer
            // 
            this.BluetoothOBEXTimer.Tick += new System.EventHandler(this.BluetoothOBEXTimer_Tick);
            // 
            // buttonAudioPASS
            // 
            this.buttonAudioPASS.Enabled = false;
            this.buttonAudioPASS.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonAudioPASS.ForeColor = System.Drawing.Color.Green;
            this.buttonAudioPASS.Location = new System.Drawing.Point(8, 100);
            this.buttonAudioPASS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonAudioPASS.Name = "buttonAudioPASS";
            this.buttonAudioPASS.Size = new System.Drawing.Size(200, 62);
            this.buttonAudioPASS.TabIndex = 51;
            this.buttonAudioPASS.TabStop = false;
            this.buttonAudioPASS.Text = "PASS";
            this.buttonAudioPASS.UseVisualStyleBackColor = true;
            this.buttonAudioPASS.Click += new System.EventHandler(this.buttonPASS_Click);
            // 
            // buttonAudioFAIL
            // 
            this.buttonAudioFAIL.Enabled = false;
            this.buttonAudioFAIL.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonAudioFAIL.ForeColor = System.Drawing.Color.Red;
            this.buttonAudioFAIL.Location = new System.Drawing.Point(216, 101);
            this.buttonAudioFAIL.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonAudioFAIL.Name = "buttonAudioFAIL";
            this.buttonAudioFAIL.Size = new System.Drawing.Size(200, 62);
            this.buttonAudioFAIL.TabIndex = 52;
            this.buttonAudioFAIL.TabStop = false;
            this.buttonAudioFAIL.Text = "FAIL";
            this.buttonAudioFAIL.UseVisualStyleBackColor = true;
            this.buttonAudioFAIL.Click += new System.EventHandler(this.buttonFAIL_Click);
            // 
            // buttonPairRequest
            // 
            this.buttonPairRequest.Enabled = false;
            this.buttonPairRequest.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonPairRequest.Location = new System.Drawing.Point(8, 30);
            this.buttonPairRequest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonPairRequest.Name = "buttonPairRequest";
            this.buttonPairRequest.Size = new System.Drawing.Size(200, 62);
            this.buttonPairRequest.TabIndex = 53;
            this.buttonPairRequest.Text = "Pair";
            this.buttonPairRequest.UseVisualStyleBackColor = true;
            this.buttonPairRequest.Click += new System.EventHandler(this.buttonPairRequest_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Enabled = false;
            this.buttonRemove.Font = new System.Drawing.Font("Arial", 24F, System.Drawing.FontStyle.Bold);
            this.buttonRemove.Location = new System.Drawing.Point(216, 30);
            this.buttonRemove.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(200, 62);
            this.buttonRemove.TabIndex = 54;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonSecurityRemoveDevice_Click);
            // 
            // groupBoxBluetoothPair
            // 
            this.groupBoxBluetoothPair.Controls.Add(this.buttonPairRequest);
            this.groupBoxBluetoothPair.Controls.Add(this.buttonAudioPASS);
            this.groupBoxBluetoothPair.Controls.Add(this.buttonAudioFAIL);
            this.groupBoxBluetoothPair.Controls.Add(this.buttonRemove);
            this.groupBoxBluetoothPair.Controls.Add(this.buttonAudioTest);
            this.groupBoxBluetoothPair.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxBluetoothPair.Location = new System.Drawing.Point(23, 279);
            this.groupBoxBluetoothPair.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxBluetoothPair.Name = "groupBoxBluetoothPair";
            this.groupBoxBluetoothPair.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxBluetoothPair.Size = new System.Drawing.Size(699, 171);
            this.groupBoxBluetoothPair.TabIndex = 55;
            this.groupBoxBluetoothPair.TabStop = false;
            this.groupBoxBluetoothPair.Text = "Bluetooth Pair and Audio Test";
            this.groupBoxBluetoothPair.Visible = false;
            // 
            // labelResultTopics
            // 
            this.labelResultTopics.AutoSize = true;
            this.labelResultTopics.Font = new System.Drawing.Font("Arial", 24F);
            this.labelResultTopics.Location = new System.Drawing.Point(16, 72);
            this.labelResultTopics.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResultTopics.Name = "labelResultTopics";
            this.labelResultTopics.Size = new System.Drawing.Size(163, 45);
            this.labelResultTopics.TabIndex = 104;
            this.labelResultTopics.Text = "Result : ";
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold);
            this.labelResult.ForeColor = System.Drawing.Color.Blue;
            this.labelResult.Location = new System.Drawing.Point(200, 65);
            this.labelResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(259, 55);
            this.labelResult.TabIndex = 103;
            this.labelResult.Text = "Not Result";
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 28F);
            this.labelTitle.Location = new System.Drawing.Point(13, 12);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(220, 53);
            this.labelTitle.TabIndex = 102;
            this.labelTitle.Text = "Bluetooth";
            // 
            // bluetooth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 865);
            this.Controls.Add(this.labelResultTopics);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.groupBoxBluetoothPair);
            this.Controls.Add(this.groupTest);
            this.Controls.Add(this.groupBTDeviceConnection);
            this.Controls.Add(this.groupFindBTDevice);
            this.Controls.Add(this.groupBoxBluetoothConnect);
            this.Controls.Add(this.groupDeviceDetails);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "bluetooth";
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.bluetooth_Load);
            this.groupDeviceDetails.ResumeLayout(false);
            this.groupDeviceDetails.PerformLayout();
            this.groupBoxBluetoothConnect.ResumeLayout(false);
            this.groupBoxBluetoothConnect.PerformLayout();
            this.groupFindBTDevice.ResumeLayout(false);
            this.groupFindBTDevice.PerformLayout();
            this.groupBTDeviceConnection.ResumeLayout(false);
            this.groupBTDeviceConnection.PerformLayout();
            this.groupTest.ResumeLayout(false);
            this.groupBoxBluetoothPair.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonSendFile;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonFindBluetoothDevice;
        private System.Windows.Forms.ComboBox comboBoxBluetoothDevices;
        private System.Windows.Forms.TextBox TextBoxPin;
        private System.Windows.Forms.ComboBox ComboBoxServices;
        private System.Windows.Forms.Label BTServicesTopic;
        private System.Windows.Forms.Label labelDeviceListTopic;
        private System.Windows.Forms.Button buttonBTServiceListen;
        private System.Windows.Forms.TextBox txtBTDeviceDetails;
        internal System.Windows.Forms.CheckBox CheckBoxUsePin;
        internal System.Windows.Forms.CheckBox CheckBoxEncrypt;
        internal System.Windows.Forms.CheckBox CheckBoxAuthenticate;
        private System.Windows.Forms.Button buttonBTServiceSend;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.GroupBox groupDeviceDetails;
        private System.Windows.Forms.GroupBox groupBoxBluetoothConnect;
        private System.Windows.Forms.GroupBox groupFindBTDevice;
        private System.Windows.Forms.GroupBox groupBTDeviceConnection;
        private System.Windows.Forms.GroupBox groupTest;
        internal System.Windows.Forms.CheckBox CheckBoxSetServiceState;
        private System.Windows.Forms.Button buttonReceiveFile;
        private System.Windows.Forms.Timer BluetoothOBEXTimer;
        private System.Windows.Forms.Label txtCompareFileResult;
        private System.Windows.Forms.Button buttonFsquirt;
        private System.Windows.Forms.Button buttonAudioTest;
        private System.Windows.Forms.Button buttonAudioPASS;
        private System.Windows.Forms.Button buttonAudioFAIL;
        private System.Windows.Forms.Button buttonPairRequest;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.GroupBox groupBoxBluetoothPair;
        private System.Windows.Forms.Label labelBtLocalAddress;
        private System.Windows.Forms.Label labelBtLocalAddressTopic;
        private System.Windows.Forms.Button buttonFileTestFAIL;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxEncoding;
        private System.Windows.Forms.Label labelResultTopics;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Label labelTitle;
    }
}

