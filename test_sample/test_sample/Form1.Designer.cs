namespace test_sample
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btn_pass = new System.Windows.Forms.Button();
            this.btn_fail = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 34);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(254, 374);
            this.textBox1.TabIndex = 0;
            // 
            // btn_pass
            // 
            this.btn_pass.Location = new System.Drawing.Point(381, 72);
            this.btn_pass.Name = "btn_pass";
            this.btn_pass.Size = new System.Drawing.Size(169, 63);
            this.btn_pass.TabIndex = 1;
            this.btn_pass.Text = "PASS";
            this.btn_pass.UseVisualStyleBackColor = true;
            this.btn_pass.Click += new System.EventHandler(this.btn_pass_Click);
            // 
            // btn_fail
            // 
            this.btn_fail.Location = new System.Drawing.Point(381, 190);
            this.btn_fail.Name = "btn_fail";
            this.btn_fail.Size = new System.Drawing.Size(169, 63);
            this.btn_fail.TabIndex = 2;
            this.btn_fail.Text = "FAIL";
            this.btn_fail.UseVisualStyleBackColor = true;
            this.btn_fail.Click += new System.EventHandler(this.btn_fail_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btn_fail);
            this.Controls.Add(this.btn_pass);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btn_pass;
        private System.Windows.Forms.Button btn_fail;
    }
}

