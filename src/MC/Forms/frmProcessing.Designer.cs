namespace MC.Forms
{
    partial class frmProcessing
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProcessing));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.processingImage = new MC.Graphical.ProcessingImage();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // processingImage
            // 
            this.processingImage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("processingImage.BackgroundImage")));
            this.processingImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.processingImage.Location = new System.Drawing.Point(5, 5);
            this.processingImage.Name = "processingImage";
            this.processingImage.Size = new System.Drawing.Size(390, 30);
            this.processingImage.TabIndex = 0;
            this.processingImage.Text = "processingImage";
            // 
            // frmProcessing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(400, 40);
            this.Controls.Add(this.processingImage);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmProcessing";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Processing...";
            this.ResumeLayout(false);

        }

        #endregion

        private Graphical.ProcessingImage processingImage;
        private System.Windows.Forms.Timer timer;
    }
}