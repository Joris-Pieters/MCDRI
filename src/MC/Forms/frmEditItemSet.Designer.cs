namespace MC.Forms
{
    partial class frmEditItemSet
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.numNumItems = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numNumAlternatives = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numNumAnswers = new System.Windows.Forms.NumericUpDown();
            this.checkScoreIfTooMany = new System.Windows.Forms.CheckBox();
            this.txtScoreIfTooMany = new System.Windows.Forms.TextBox();
            this.comboItemsNaming = new System.Windows.Forms.ComboBox();
            this.comboAlternativesNaming = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumAlternatives)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumAnswers)).BeginInit();
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
            this.dataGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGrid.ColumnHeadersHeight = 18;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGrid.ColumnHeadersVisible = false;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGrid.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGrid.EnableHeadersVisualStyles = false;
            this.dataGrid.Location = new System.Drawing.Point(0, 0);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGrid.RowHeadersVisible = false;
            this.dataGrid.RowHeadersWidth = 18;
            this.dataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dataGrid.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGrid.Size = new System.Drawing.Size(444, 267);
            this.dataGrid.TabIndex = 1;
            this.dataGrid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGrid_CellMouseClick);
            this.dataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGrid_CellValueChanged);
            this.dataGrid.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGrid_ColumnHeaderMouseClick);
            this.dataGrid.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGrid_RowHeaderMouseClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.Location = new System.Drawing.Point(229, 389);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOk.Location = new System.Drawing.Point(148, 389);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 9;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // numNumItems
            // 
            this.numNumItems.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.numNumItems.Location = new System.Drawing.Point(155, 274);
            this.numNumItems.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numNumItems.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumItems.Name = "numNumItems";
            this.numNumItems.Size = new System.Drawing.Size(50, 20);
            this.numNumItems.TabIndex = 2;
            this.numNumItems.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numNumItems.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumItems.ValueChanged += new System.EventHandler(this.numNumItems_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 276);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Number of items:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 304);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Number of alternatives:";
            // 
            // numNumAlternatives
            // 
            this.numNumAlternatives.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.numNumAlternatives.Location = new System.Drawing.Point(155, 302);
            this.numNumAlternatives.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numNumAlternatives.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumAlternatives.Name = "numNumAlternatives";
            this.numNumAlternatives.Size = new System.Drawing.Size(50, 20);
            this.numNumAlternatives.TabIndex = 3;
            this.numNumAlternatives.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numNumAlternatives.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumAlternatives.ValueChanged += new System.EventHandler(this.numNumAlternatives_ValueChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 332);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Number of answers allowed:";
            // 
            // numNumAnswers
            // 
            this.numNumAnswers.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.numNumAnswers.Location = new System.Drawing.Point(155, 330);
            this.numNumAnswers.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.numNumAnswers.Name = "numNumAnswers";
            this.numNumAnswers.Size = new System.Drawing.Size(50, 20);
            this.numNumAnswers.TabIndex = 4;
            this.numNumAnswers.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numNumAnswers.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNumAnswers.ValueChanged += new System.EventHandler(this.numNumAnswers_ValueChanged);
            // 
            // checkScoreIfTooMany
            // 
            this.checkScoreIfTooMany.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.checkScoreIfTooMany.AutoSize = true;
            this.checkScoreIfTooMany.Checked = true;
            this.checkScoreIfTooMany.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkScoreIfTooMany.Location = new System.Drawing.Point(4, 360);
            this.checkScoreIfTooMany.Name = "checkScoreIfTooMany";
            this.checkScoreIfTooMany.Size = new System.Drawing.Size(231, 17);
            this.checkScoreIfTooMany.TabIndex = 5;
            this.checkScoreIfTooMany.Text = "Score if too many alternatives are checked:";
            this.checkScoreIfTooMany.UseVisualStyleBackColor = true;
            this.checkScoreIfTooMany.CheckedChanged += new System.EventHandler(this.checkScoreIfTooMany_CheckedChanged);
            // 
            // txtScoreIfTooMany
            // 
            this.txtScoreIfTooMany.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.txtScoreIfTooMany.Location = new System.Drawing.Point(235, 358);
            this.txtScoreIfTooMany.Name = "txtScoreIfTooMany";
            this.txtScoreIfTooMany.Size = new System.Drawing.Size(50, 20);
            this.txtScoreIfTooMany.TabIndex = 6;
            this.txtScoreIfTooMany.Text = "0";
            this.txtScoreIfTooMany.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtScoreIfTooMany.Leave += new System.EventHandler(this.txtScoreIfTooMany_Leave);
            // 
            // comboItemsNaming
            // 
            this.comboItemsNaming.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.comboItemsNaming.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboItemsNaming.FormattingEnabled = true;
            this.comboItemsNaming.Location = new System.Drawing.Point(359, 273);
            this.comboItemsNaming.Name = "comboItemsNaming";
            this.comboItemsNaming.Size = new System.Drawing.Size(83, 21);
            this.comboItemsNaming.TabIndex = 7;
            this.comboItemsNaming.SelectedIndexChanged += new System.EventHandler(this.comboItemsNaming_SelectedIndexChanged);
            // 
            // comboAlternativesNaming
            // 
            this.comboAlternativesNaming.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.comboAlternativesNaming.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAlternativesNaming.FormattingEnabled = true;
            this.comboAlternativesNaming.Location = new System.Drawing.Point(359, 301);
            this.comboAlternativesNaming.Name = "comboAlternativesNaming";
            this.comboAlternativesNaming.Size = new System.Drawing.Size(83, 21);
            this.comboAlternativesNaming.TabIndex = 8;
            this.comboAlternativesNaming.SelectedIndexChanged += new System.EventHandler(this.comboAlternativesNaming_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(252, 304);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "AlternativesNaming:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(282, 276);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "ItemsNaming:";
            // 
            // frmEditItemSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 424);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboAlternativesNaming);
            this.Controls.Add(this.comboItemsNaming);
            this.Controls.Add(this.txtScoreIfTooMany);
            this.Controls.Add(this.checkScoreIfTooMany);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numNumAnswers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numNumAlternatives);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numNumItems);
            this.Controls.Add(this.dataGrid);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(452, 456);
            this.Name = "frmEditItemSet";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Item Set";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumAlternatives)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNumAnswers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.NumericUpDown numNumItems;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numNumAlternatives;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numNumAnswers;
        private System.Windows.Forms.CheckBox checkScoreIfTooMany;
        private System.Windows.Forms.TextBox txtScoreIfTooMany;
        private System.Windows.Forms.ComboBox comboItemsNaming;
        private System.Windows.Forms.ComboBox comboAlternativesNaming;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}