namespace MC.Forms
{
    partial class ctrlImport
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnCalibrate = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.panelImage = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.zoom = new System.Windows.Forms.TrackBar();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.Calibration = new System.Windows.Forms.Button();
            this.btnSortDoubts = new System.Windows.Forms.Button();
            this.btnSortTooMany = new System.Windows.Forms.Button();
            this.btnSortBarcode = new System.Windows.Forms.Button();
            this.btnSortPageHash = new System.Windows.Forms.Button();
            this.btnSortAnalyzed = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.btnClear = new System.Windows.Forms.Button();
            this.panelImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCalibrate
            // 
            this.btnCalibrate.Location = new System.Drawing.Point(327, 5);
            this.btnCalibrate.Name = "btnCalibrate";
            this.btnCalibrate.Size = new System.Drawing.Size(75, 23);
            this.btnCalibrate.TabIndex = 4;
            this.btnCalibrate.Text = "Calibrate";
            this.btnCalibrate.UseVisualStyleBackColor = true;
            this.btnCalibrate.Click += new System.EventHandler(this.btnCalibrate_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(165, 5);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Analyze all";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // panelImage
            // 
            this.panelImage.AutoScroll = true;
            this.panelImage.AutoScrollMargin = new System.Drawing.Size(12, 12);
            this.panelImage.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelImage.Controls.Add(this.pictureBox);
            this.panelImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImage.Location = new System.Drawing.Point(0, 0);
            this.panelImage.Name = "panelImage";
            this.panelImage.Size = new System.Drawing.Size(629, 903);
            this.panelImage.TabIndex = 7;
            // 
            // pictureBox
            // 
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox.Location = new System.Drawing.Point(12, 12);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(12);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(200, 200);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // zoom
            // 
            this.zoom.LargeChange = 3;
            this.zoom.Location = new System.Drawing.Point(-1, 0);
            this.zoom.Maximum = 12;
            this.zoom.Minimum = 1;
            this.zoom.Name = "zoom";
            this.zoom.Size = new System.Drawing.Size(79, 42);
            this.zoom.TabIndex = 8;
            this.zoom.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.zoom.Value = 4;
            this.zoom.Scroll += new System.EventHandler(this.zoom_Scroll);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.panelImage);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.Calibration);
            this.splitContainer.Panel2.Controls.Add(this.btnSortDoubts);
            this.splitContainer.Panel2.Controls.Add(this.btnSortTooMany);
            this.splitContainer.Panel2.Controls.Add(this.btnSortBarcode);
            this.splitContainer.Panel2.Controls.Add(this.btnSortPageHash);
            this.splitContainer.Panel2.Controls.Add(this.btnSortAnalyzed);
            this.splitContainer.Panel2.Controls.Add(this.btnImport);
            this.splitContainer.Panel2.Controls.Add(this.dataGrid);
            this.splitContainer.Panel2.Controls.Add(this.btnClear);
            this.splitContainer.Panel2.Controls.Add(this.zoom);
            this.splitContainer.Panel2.Controls.Add(this.btnStart);
            this.splitContainer.Panel2.Controls.Add(this.btnCalibrate);
            this.splitContainer.Size = new System.Drawing.Size(1038, 903);
            this.splitContainer.SplitterDistance = 629;
            this.splitContainer.TabIndex = 9;
            // 
            // Calibration
            // 
            this.Calibration.Location = new System.Drawing.Point(360, 34);
            this.Calibration.Name = "Calibration";
            this.Calibration.Size = new System.Drawing.Size(26, 13);
            this.Calibration.TabIndex = 18;
            this.Calibration.Text = "^";
            this.Calibration.UseVisualStyleBackColor = true;
            this.Calibration.Click += new System.EventHandler(this.Calibration_Click);
            // 
            // btnSortDoubts
            // 
            this.btnSortDoubts.Location = new System.Drawing.Point(232, 34);
            this.btnSortDoubts.Name = "btnSortDoubts";
            this.btnSortDoubts.Size = new System.Drawing.Size(26, 13);
            this.btnSortDoubts.TabIndex = 17;
            this.btnSortDoubts.Text = "^";
            this.btnSortDoubts.UseVisualStyleBackColor = true;
            this.btnSortDoubts.Click += new System.EventHandler(this.btnSortDoubts_Click);
            // 
            // btnSortTooMany
            // 
            this.btnSortTooMany.Location = new System.Drawing.Point(264, 34);
            this.btnSortTooMany.Name = "btnSortTooMany";
            this.btnSortTooMany.Size = new System.Drawing.Size(26, 13);
            this.btnSortTooMany.TabIndex = 16;
            this.btnSortTooMany.Text = "^";
            this.btnSortTooMany.UseVisualStyleBackColor = true;
            this.btnSortTooMany.Click += new System.EventHandler(this.btnSortTooMany_Click);
            // 
            // btnSortBarcode
            // 
            this.btnSortBarcode.Location = new System.Drawing.Point(296, 34);
            this.btnSortBarcode.Name = "btnSortBarcode";
            this.btnSortBarcode.Size = new System.Drawing.Size(26, 13);
            this.btnSortBarcode.TabIndex = 15;
            this.btnSortBarcode.Text = "^";
            this.btnSortBarcode.UseVisualStyleBackColor = true;
            this.btnSortBarcode.Click += new System.EventHandler(this.btnSortBarcode_Click);
            // 
            // btnSortPageHash
            // 
            this.btnSortPageHash.Location = new System.Drawing.Point(328, 34);
            this.btnSortPageHash.Name = "btnSortPageHash";
            this.btnSortPageHash.Size = new System.Drawing.Size(26, 13);
            this.btnSortPageHash.TabIndex = 14;
            this.btnSortPageHash.Text = "^";
            this.btnSortPageHash.UseVisualStyleBackColor = true;
            this.btnSortPageHash.Click += new System.EventHandler(this.btnSortPageHash_Click);
            // 
            // btnSortAnalyzed
            // 
            this.btnSortAnalyzed.Location = new System.Drawing.Point(200, 34);
            this.btnSortAnalyzed.Name = "btnSortAnalyzed";
            this.btnSortAnalyzed.Size = new System.Drawing.Size(26, 13);
            this.btnSortAnalyzed.TabIndex = 13;
            this.btnSortAnalyzed.Text = "^";
            this.btnSortAnalyzed.UseVisualStyleBackColor = true;
            this.btnSortAnalyzed.Click += new System.EventHandler(this.btnSortAnalyzed_Click);
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(84, 5);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 12;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // dataGrid
            // 
            this.dataGrid.AllowDrop = true;
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.AllowUserToDeleteRows = false;
            this.dataGrid.AllowUserToResizeRows = false;
            this.dataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.EnableHeadersVisualStyles = false;
            this.dataGrid.GridColor = System.Drawing.SystemColors.AppWorkspace;
            this.dataGrid.Location = new System.Drawing.Point(0, 33);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.ReadOnly = true;
            this.dataGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGrid.RowHeadersVisible = false;
            this.dataGrid.RowHeadersWidth = 18;
            this.dataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGrid.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGrid.Size = new System.Drawing.Size(405, 870);
            this.dataGrid.TabIndex = 10;
            this.dataGrid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGrid_CellMouseClick);
            this.dataGrid.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGrid_KeyUp);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(246, 5);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 11;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // ctrlImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "ctrlImport";
            this.Size = new System.Drawing.Size(1038, 903);
            this.panelImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zoom)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCalibrate;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Panel panelImage;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.TrackBar zoom;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnSortAnalyzed;
        private System.Windows.Forms.Button Calibration;
        private System.Windows.Forms.Button btnSortDoubts;
        private System.Windows.Forms.Button btnSortTooMany;
        private System.Windows.Forms.Button btnSortBarcode;
        private System.Windows.Forms.Button btnSortPageHash;
    }
}
