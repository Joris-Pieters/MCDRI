namespace MC.Forms
{
    partial class ctrlResults
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.checkSetTotals = new System.Windows.Forms.CheckBox();
            this.checkItems = new System.Windows.Forms.CheckBox();
            this.checkInclSetTotals = new System.Windows.Forms.CheckBox();
            this.checkInclItems = new System.Windows.Forms.CheckBox();
            this.checkInclItemAlts = new System.Windows.Forms.CheckBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.checkTotal = new System.Windows.Forms.CheckBox();
            this.checkInclTotal = new System.Windows.Forms.CheckBox();
            this.radLocale = new System.Windows.Forms.RadioButton();
            this.radPoint = new System.Windows.Forms.RadioButton();
            this.radComma = new System.Windows.Forms.RadioButton();
            this.btnScript = new System.Windows.Forms.Button();
            this.checkBasedOnScript = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGrid.EnableHeadersVisualStyles = false;
            this.dataGrid.GridColor = System.Drawing.SystemColors.AppWorkspace;
            this.dataGrid.Location = new System.Drawing.Point(0, 127);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.ReadOnly = true;
            this.dataGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGrid.RowHeadersVisible = false;
            this.dataGrid.RowHeadersWidth = 18;
            this.dataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGrid.Size = new System.Drawing.Size(979, 551);
            this.dataGrid.TabIndex = 11;
            // 
            // checkSetTotals
            // 
            this.checkSetTotals.AutoSize = true;
            this.checkSetTotals.Checked = true;
            this.checkSetTotals.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSetTotals.Location = new System.Drawing.Point(6, 42);
            this.checkSetTotals.Name = "checkSetTotals";
            this.checkSetTotals.Size = new System.Drawing.Size(88, 17);
            this.checkSetTotals.TabIndex = 12;
            this.checkSetTotals.Text = "Itemset totals";
            this.checkSetTotals.UseVisualStyleBackColor = true;
            this.checkSetTotals.CheckedChanged += new System.EventHandler(this.checkSetTotals_CheckedChanged);
            // 
            // checkItems
            // 
            this.checkItems.AutoSize = true;
            this.checkItems.Location = new System.Drawing.Point(6, 65);
            this.checkItems.Name = "checkItems";
            this.checkItems.Size = new System.Drawing.Size(98, 17);
            this.checkItems.TabIndex = 13;
            this.checkItems.Text = "Individual items";
            this.checkItems.UseVisualStyleBackColor = true;
            this.checkItems.CheckedChanged += new System.EventHandler(this.checkItems_CheckedChanged);
            // 
            // checkInclSetTotals
            // 
            this.checkInclSetTotals.AutoSize = true;
            this.checkInclSetTotals.Checked = true;
            this.checkInclSetTotals.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkInclSetTotals.Location = new System.Drawing.Point(6, 42);
            this.checkInclSetTotals.Name = "checkInclSetTotals";
            this.checkInclSetTotals.Size = new System.Drawing.Size(125, 17);
            this.checkInclSetTotals.TabIndex = 14;
            this.checkInclSetTotals.Text = "Include itemset totals";
            this.checkInclSetTotals.UseVisualStyleBackColor = true;
            // 
            // checkInclItems
            // 
            this.checkInclItems.AutoSize = true;
            this.checkInclItems.Location = new System.Drawing.Point(6, 65);
            this.checkInclItems.Name = "checkInclItems";
            this.checkInclItems.Size = new System.Drawing.Size(135, 17);
            this.checkInclItems.TabIndex = 15;
            this.checkInclItems.Text = "Include individual items";
            this.checkInclItems.UseVisualStyleBackColor = true;
            // 
            // checkInclItemAlts
            // 
            this.checkInclItemAlts.AutoSize = true;
            this.checkInclItemAlts.Location = new System.Drawing.Point(6, 88);
            this.checkInclItemAlts.Name = "checkInclItemAlts";
            this.checkInclItemAlts.Size = new System.Drawing.Size(184, 17);
            this.checkInclItemAlts.TabIndex = 16;
            this.checkInclItemAlts.Text = "Include individual itemalternatives";
            this.checkInclItemAlts.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(233, 88);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(90, 23);
            this.btnExport.TabIndex = 17;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // checkTotal
            // 
            this.checkTotal.AutoSize = true;
            this.checkTotal.Checked = true;
            this.checkTotal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkTotal.Location = new System.Drawing.Point(6, 19);
            this.checkTotal.Name = "checkTotal";
            this.checkTotal.Size = new System.Drawing.Size(50, 17);
            this.checkTotal.TabIndex = 18;
            this.checkTotal.Text = "Total";
            this.checkTotal.UseVisualStyleBackColor = true;
            this.checkTotal.CheckedChanged += new System.EventHandler(this.checkTotal_CheckedChanged);
            // 
            // checkInclTotal
            // 
            this.checkInclTotal.AutoSize = true;
            this.checkInclTotal.Checked = true;
            this.checkInclTotal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkInclTotal.Location = new System.Drawing.Point(6, 19);
            this.checkInclTotal.Name = "checkInclTotal";
            this.checkInclTotal.Size = new System.Drawing.Size(84, 17);
            this.checkInclTotal.TabIndex = 19;
            this.checkInclTotal.Text = "Include total";
            this.checkInclTotal.UseVisualStyleBackColor = true;
            // 
            // radLocale
            // 
            this.radLocale.AutoSize = true;
            this.radLocale.Checked = true;
            this.radLocale.Location = new System.Drawing.Point(233, 19);
            this.radLocale.Name = "radLocale";
            this.radLocale.Size = new System.Drawing.Size(130, 17);
            this.radLocale.TabIndex = 21;
            this.radLocale.TabStop = true;
            this.radLocale.Text = "Use computer settings";
            this.radLocale.UseVisualStyleBackColor = true;
            // 
            // radPoint
            // 
            this.radPoint.AutoSize = true;
            this.radPoint.Location = new System.Drawing.Point(233, 43);
            this.radPoint.Name = "radPoint";
            this.radPoint.Size = new System.Drawing.Size(70, 17);
            this.radPoint.TabIndex = 22;
            this.radPoint.Text = "Use point";
            this.radPoint.UseVisualStyleBackColor = true;
            // 
            // radComma
            // 
            this.radComma.AutoSize = true;
            this.radComma.Location = new System.Drawing.Point(233, 65);
            this.radComma.Name = "radComma";
            this.radComma.Size = new System.Drawing.Size(81, 17);
            this.radComma.TabIndex = 23;
            this.radComma.Text = "Use comma";
            this.radComma.UseVisualStyleBackColor = true;
            // 
            // btnScript
            // 
            this.btnScript.Location = new System.Drawing.Point(6, 42);
            this.btnScript.Name = "btnScript";
            this.btnScript.Size = new System.Drawing.Size(90, 23);
            this.btnScript.TabIndex = 24;
            this.btnScript.Text = "View/edit script";
            this.btnScript.UseVisualStyleBackColor = true;
            this.btnScript.Click += new System.EventHandler(this.btnScript_Click);
            // 
            // checkBasedOnScript
            // 
            this.checkBasedOnScript.AutoSize = true;
            this.checkBasedOnScript.Location = new System.Drawing.Point(6, 19);
            this.checkBasedOnScript.Name = "checkBasedOnScript";
            this.checkBasedOnScript.Size = new System.Drawing.Size(149, 17);
            this.checkBasedOnScript.TabIndex = 25;
            this.checkBasedOnScript.Text = "Correction based on script";
            this.checkBasedOnScript.UseVisualStyleBackColor = true;
            this.checkBasedOnScript.CheckedChanged += new System.EventHandler(this.checkBasedOnScript_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnScript);
            this.groupBox1.Controls.Add(this.checkBasedOnScript);
            this.groupBox1.Location = new System.Drawing.Point(3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(158, 117);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Script";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkItems);
            this.groupBox2.Controls.Add(this.checkSetTotals);
            this.groupBox2.Controls.Add(this.checkTotal);
            this.groupBox2.Location = new System.Drawing.Point(167, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(108, 117);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Show";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnExport);
            this.groupBox3.Controls.Add(this.radComma);
            this.groupBox3.Controls.Add(this.checkInclTotal);
            this.groupBox3.Controls.Add(this.radPoint);
            this.groupBox3.Controls.Add(this.checkInclSetTotals);
            this.groupBox3.Controls.Add(this.radLocale);
            this.groupBox3.Controls.Add(this.checkInclItems);
            this.groupBox3.Controls.Add(this.checkInclItemAlts);
            this.groupBox3.Location = new System.Drawing.Point(281, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(368, 117);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Export";
            // 
            // ctrlResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Name = "ctrlResults";
            this.Size = new System.Drawing.Size(979, 681);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.CheckBox checkSetTotals;
        private System.Windows.Forms.CheckBox checkItems;
        private System.Windows.Forms.CheckBox checkInclSetTotals;
        private System.Windows.Forms.CheckBox checkInclItems;
        private System.Windows.Forms.CheckBox checkInclItemAlts;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.CheckBox checkTotal;
        private System.Windows.Forms.CheckBox checkInclTotal;
        private System.Windows.Forms.RadioButton radLocale;
        private System.Windows.Forms.RadioButton radPoint;
        private System.Windows.Forms.RadioButton radComma;
        private System.Windows.Forms.Button btnScript;
        private System.Windows.Forms.CheckBox checkBasedOnScript;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}