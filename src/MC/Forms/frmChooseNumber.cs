using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MC.Forms
{
    public partial class frmChooseNumber : Form
    {
        public int Value = 0;

        public frmChooseNumber(int max, int current = -1)
        {
            InitializeComponent();
            numericUpDown.Maximum = max;
            if (current > 0)
            {
                numericUpDown.Value = current;
            }
            this.ClientSize = new Size(this.ClientSize.Width, numericUpDown.Bottom + btnOk.Height + 10);
            btnOk.Top = this.ClientSize.Height - btnOk.Height - 5;
            btnCancel.Top = this.ClientSize.Height - btnCancel.Height - 5;
            btnOk.Left = (this.ClientSize.Width / 2) - (btnOk.Width + 3);
            btnCancel.Left = (this.ClientSize.Width / 2) + 3;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Value = (int)numericUpDown.Value - 1;
            this.DialogResult = DialogResult.OK;
            this.Visible = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Visible = false;
        }
    }
}
