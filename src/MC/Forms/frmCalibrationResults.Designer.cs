namespace MC.Forms
{
    partial class frmCalibrationResults
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOnlyTest = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.textBox = new System.Windows.Forms.TextBox();
            this.btnAlways = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtMean = new System.Windows.Forms.TextBox();
            this.txtSD = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Location = new System.Drawing.Point(693, 483);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(67, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOnlyTest
            // 
            this.btnOnlyTest.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOnlyTest.Location = new System.Drawing.Point(439, 483);
            this.btnOnlyTest.Name = "btnOnlyTest";
            this.btnOnlyTest.Size = new System.Drawing.Size(94, 23);
            this.btnOnlyTest.TabIndex = 3;
            this.btnOnlyTest.Text = "Only for this test";
            this.btnOnlyTest.UseVisualStyleBackColor = true;
            this.btnOnlyTest.Click += new System.EventHandler(this.btnOnlyTest_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(765, 400);
            this.pictureBox.TabIndex = 5;
            this.pictureBox.TabStop = false;
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox.BackColor = System.Drawing.SystemColors.Control;
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.ForeColor = System.Drawing.Color.Black;
            this.textBox.Location = new System.Drawing.Point(10, 406);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.ShortcutsEnabled = false;
            this.textBox.Size = new System.Drawing.Size(343, 104);
            this.textBox.TabIndex = 1000;
            this.textBox.Text = "[TEXT]";
            // 
            // btnAlways
            // 
            this.btnAlways.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAlways.Location = new System.Drawing.Point(539, 483);
            this.btnAlways.Name = "btnAlways";
            this.btnAlways.Size = new System.Drawing.Size(148, 23);
            this.btnAlways.TabIndex = 1001;
            this.btnAlways.Text = "For this test and future tests";
            this.btnAlways.UseVisualStyleBackColor = true;
            this.btnAlways.Click += new System.EventHandler(this.btnAlways_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.ForeColor = System.Drawing.Color.Black;
            this.textBox1.Location = new System.Drawing.Point(418, 406);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ShortcutsEnabled = false;
            this.textBox1.Size = new System.Drawing.Size(342, 64);
            this.textBox1.TabIndex = 1002;
            this.textBox1.Text = "[TEXT]";
            // 
            // txtMean
            // 
            this.txtMean.BackColor = System.Drawing.SystemColors.Control;
            this.txtMean.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMean.Location = new System.Drawing.Point(91, 471);
            this.txtMean.Name = "txtMean";
            this.txtMean.Size = new System.Drawing.Size(52, 13);
            this.txtMean.TabIndex = 1003;
            // 
            // txtSD
            // 
            this.txtSD.BackColor = System.Drawing.SystemColors.Control;
            this.txtSD.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSD.Location = new System.Drawing.Point(154, 484);
            this.txtSD.Name = "txtSD";
            this.txtSD.Size = new System.Drawing.Size(52, 13);
            this.txtSD.TabIndex = 1004;
            // 
            // frmCalibrationResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 510);
            this.Controls.Add(this.txtSD);
            this.Controls.Add(this.txtMean);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnAlways);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOnlyTest);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCalibrationResults";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Results of Calibration";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOnlyTest;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button btnAlways;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtMean;
        private System.Windows.Forms.TextBox txtSD;
    }
}