namespace MC.Forms
{
    partial class ctrlDesign
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
            this.components = new System.ComponentModel.Container();
            this.panelDesign = new System.Windows.Forms.Panel();
            this.btnAddItemSet = new System.Windows.Forms.Button();
            this.btnAddText = new System.Windows.Forms.Button();
            this.btnAddPage = new System.Windows.Forms.Button();
            this.btnAddImageBox = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.btnRedo = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnExportPdf = new System.Windows.Forms.Button();
            this.propertyViewer = new MC.Forms.PropertyViewer();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelDesign
            // 
            this.panelDesign.AutoScroll = true;
            this.panelDesign.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panelDesign.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDesign.Location = new System.Drawing.Point(0, 0);
            this.panelDesign.Margin = new System.Windows.Forms.Padding(0);
            this.panelDesign.Name = "panelDesign";
            this.panelDesign.Size = new System.Drawing.Size(629, 903);
            this.panelDesign.TabIndex = 2;
            this.panelDesign.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelDesign_MouseClick);
            // 
            // btnAddItemSet
            // 
            this.btnAddItemSet.Location = new System.Drawing.Point(122, 6);
            this.btnAddItemSet.Name = "btnAddItemSet";
            this.btnAddItemSet.Size = new System.Drawing.Size(65, 23);
            this.btnAddItemSet.TabIndex = 12;
            this.btnAddItemSet.Text = "Add items";
            this.btnAddItemSet.UseVisualStyleBackColor = true;
            this.btnAddItemSet.Click += new System.EventHandler(this.btnAddItemSet_Click);
            // 
            // btnAddText
            // 
            this.btnAddText.Location = new System.Drawing.Point(193, 6);
            this.btnAddText.Name = "btnAddText";
            this.btnAddText.Size = new System.Drawing.Size(65, 23);
            this.btnAddText.TabIndex = 13;
            this.btnAddText.Text = "Add text";
            this.btnAddText.UseVisualStyleBackColor = true;
            this.btnAddText.Click += new System.EventHandler(this.btnAddTextBox_Click);
            // 
            // btnAddPage
            // 
            this.btnAddPage.Location = new System.Drawing.Point(51, 6);
            this.btnAddPage.Name = "btnAddPage";
            this.btnAddPage.Size = new System.Drawing.Size(65, 23);
            this.btnAddPage.TabIndex = 11;
            this.btnAddPage.Text = "Add page";
            this.btnAddPage.UseVisualStyleBackColor = true;
            this.btnAddPage.Click += new System.EventHandler(this.btnAddPage_Click);
            // 
            // btnAddImageBox
            // 
            this.btnAddImageBox.Location = new System.Drawing.Point(264, 6);
            this.btnAddImageBox.Name = "btnAddImageBox";
            this.btnAddImageBox.Size = new System.Drawing.Size(65, 23);
            this.btnAddImageBox.TabIndex = 14;
            this.btnAddImageBox.Text = "Add image box";
            this.btnAddImageBox.UseVisualStyleBackColor = true;
            this.btnAddImageBox.Click += new System.EventHandler(this.btnAddImageBox_Click);
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
            this.splitContainer.Panel1.Controls.Add(this.panelDesign);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.btnRedo);
            this.splitContainer.Panel2.Controls.Add(this.btnUndo);
            this.splitContainer.Panel2.Controls.Add(this.btnExportPdf);
            this.splitContainer.Panel2.Controls.Add(this.btnAddImageBox);
            this.splitContainer.Panel2.Controls.Add(this.propertyViewer);
            this.splitContainer.Panel2.Controls.Add(this.btnAddItemSet);
            this.splitContainer.Panel2.Controls.Add(this.btnAddText);
            this.splitContainer.Panel2.Controls.Add(this.btnAddPage);
            this.splitContainer.Size = new System.Drawing.Size(1039, 903);
            this.splitContainer.SplitterDistance = 629;
            this.splitContainer.TabIndex = 1;
            // 
            // btnRedo
            // 
            this.btnRedo.Location = new System.Drawing.Point(22, 6);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(23, 23);
            this.btnRedo.TabIndex = 17;
            this.toolTip.SetToolTip(this.btnRedo, "Redo: Ctrl+Y");
            this.btnRedo.UseVisualStyleBackColor = true;
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(0, 6);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(23, 23);
            this.btnUndo.TabIndex = 16;
            this.toolTip.SetToolTip(this.btnUndo, "Undo: Ctrl+Z");
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnExportPdf
            // 
            this.btnExportPdf.Location = new System.Drawing.Point(335, 6);
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.Size = new System.Drawing.Size(65, 23);
            this.btnExportPdf.TabIndex = 15;
            this.btnExportPdf.Text = "Export Pdf";
            this.btnExportPdf.UseVisualStyleBackColor = true;
            this.btnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);
            // 
            // propertyViewer
            // 
            this.propertyViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyViewer.AutoScroll = true;
            this.propertyViewer.Location = new System.Drawing.Point(0, 35);
            this.propertyViewer.Name = "propertyViewer";
            this.propertyViewer.Size = new System.Drawing.Size(400, 862);
            this.propertyViewer.TabIndex = 10;
            // 
            // ctrlDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "ctrlDesign";
            this.Size = new System.Drawing.Size(1039, 903);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelDesign;
        private System.Windows.Forms.Button btnAddItemSet;
        private System.Windows.Forms.Button btnAddText;
        private System.Windows.Forms.Button btnAddPage;
        private System.Windows.Forms.Button btnAddImageBox;
        private System.Windows.Forms.SplitContainer splitContainer;
        private PropertyViewer propertyViewer;
        private System.Windows.Forms.Button btnExportPdf;
        private System.Windows.Forms.Button btnRedo;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.ToolTip toolTip;


    }
}
