using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using MC.Other;
using MC.Testing;
using MC.PaperTools;
using MC.Design;
using MC.IO;


namespace MC.Forms
{
    public partial class frmSetWeights : Form
    {
        public Fraction Weight;

        public frmSetWeights()
        {
            InitializeComponent();
            this.ClientSize = new Size(this.ClientSize.Width, txtWeight.Bottom + btnOk.Height + 10);
            btnOk.Top = this.ClientSize.Height - btnOk.Height - 5;
            btnCancel.Top = this.ClientSize.Height - btnCancel.Height - 5;
            btnOk.Left = (this.ClientSize.Width / 2) - (btnOk.Width + 3);
            btnCancel.Left = (this.ClientSize.Width / 2) + 3;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                Weight = new Fraction(txtWeight.Text);
                this.DialogResult = DialogResult.OK;
                this.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Not a valid entry. Only numerical values are allowed. For fractions, use slash (/).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogLow(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Visible = false;
        }
    }
}
