namespace MC.Forms
{
    partial class frmScript
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnFromCurrent = new System.Windows.Forms.ToolStripButton();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnRedo = new System.Windows.Forms.ToolStripButton();
            this.btnApply = new System.Windows.Forms.Button();
            this.outputBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.varTree = new System.Windows.Forms.TreeView();
            this.scriptBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outputBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scriptBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.Location = new System.Drawing.Point(384, 450);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOk.Location = new System.Drawing.Point(303, 450);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFromCurrent,
            this.btnUndo,
            this.btnRedo});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(789, 25);
            this.toolStrip.TabIndex = 9;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnFromCurrent
            // 
            this.btnFromCurrent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFromCurrent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFromCurrent.Name = "btnFromCurrent";
            this.btnFromCurrent.Size = new System.Drawing.Size(23, 22);
            this.btnFromCurrent.ToolTipText = "Create script based on current situation";
            this.btnFromCurrent.Click += new System.EventHandler(this.btnFromCurrent_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(23, 22);
            this.btnUndo.ToolTipText = "Undo (Ctrl+Z)";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(23, 22);
            this.btnRedo.ToolTipText = "Redo (Ctrl+Y)";
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(275, 178);
            this.btnApply.Name = "btnApply";
            this.btnApply.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnApply.Size = new System.Drawing.Size(23, 23);
            this.btnApply.TabIndex = 11;
            this.btnApply.Text = "<";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // outputBox
            // 
            this.outputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.outputBox.AutoScrollMinSize = new System.Drawing.Size(25, 15);
            this.outputBox.BackBrush = null;
            this.outputBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.outputBox.CharHeight = 15;
            this.outputBox.CharWidth = 7;
            this.outputBox.CommentPrefix = "";
            this.outputBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.outputBox.DelayedEventsInterval = 1000;
            this.outputBox.DelayedTextChangedInterval = 1000;
            this.outputBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.outputBox.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.outputBox.ForeColor = System.Drawing.Color.Goldenrod;
            this.outputBox.IsReplaceMode = false;
            this.outputBox.Location = new System.Drawing.Point(0, 307);
            this.outputBox.Name = "outputBox";
            this.outputBox.Paddings = new System.Windows.Forms.Padding(0);
            this.outputBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.outputBox.Size = new System.Drawing.Size(270, 137);
            this.outputBox.TabIndex = 14;
            this.outputBox.Zoom = 100;
            this.outputBox.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.outputBox_TextChangedDelayed);
            // 
            // varTree
            // 
            this.varTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.varTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.varTree.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.varTree.HideSelection = false;
            this.varTree.Location = new System.Drawing.Point(0, 28);
            this.varTree.Name = "varTree";
            this.varTree.ShowNodeToolTips = true;
            this.varTree.Size = new System.Drawing.Size(270, 273);
            this.varTree.TabIndex = 13;
            // 
            // scriptBox
            // 
            this.scriptBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scriptBox.AutoScrollMinSize = new System.Drawing.Size(25, 15);
            this.scriptBox.BackBrush = null;
            this.scriptBox.CharHeight = 15;
            this.scriptBox.CharWidth = 7;
            this.scriptBox.CommentPrefix = "#";
            this.scriptBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.scriptBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.scriptBox.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.scriptBox.IsReplaceMode = false;
            this.scriptBox.Location = new System.Drawing.Point(303, 28);
            this.scriptBox.Name = "scriptBox";
            this.scriptBox.Paddings = new System.Windows.Forms.Padding(0);
            this.scriptBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.scriptBox.Size = new System.Drawing.Size(486, 416);
            this.scriptBox.TabIndex = 12;
            this.scriptBox.Zoom = 100;
            this.scriptBox.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.scriptBox_TextChangedDelayed);
            // 
            // frmScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 485);
            this.Controls.Add(this.scriptBox);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.outputBox);
            this.Controls.Add(this.varTree);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(573, 300);
            this.Name = "frmScript";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Script";
            this.SizeChanged += new System.EventHandler(this.frmScript_SizeChanged);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outputBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scriptBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnFromCurrent;
        private System.Windows.Forms.Button btnApply;
        private FastColoredTextBoxNS.FastColoredTextBox outputBox;
        private System.Windows.Forms.TreeView varTree;
        private FastColoredTextBoxNS.FastColoredTextBox scriptBox;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripButton btnRedo;
    }
}