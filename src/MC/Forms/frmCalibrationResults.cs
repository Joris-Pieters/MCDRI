using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using MC.Graphical;
using MC.Testing;
using MC.PaperTools;
using MC.Other;
using MC.Design;

namespace MC.Forms
{
    public partial class frmCalibrationResults : Form
    {        
        public double Mean;
        public double SD;

        public frmCalibrationResults(ColorDistribution distribution)
        {
            InitializeComponent();
            Mean = Math.Round(distribution.Mean(), 4);
            SD = Math.Round(distribution.StandardDeviation(), 4);

            pictureBox.Image = Processing.Histogram(distribution.Values, true, 0, 100, pictureBox.Width, pictureBox.Height);
            textBox.Text =
                  "The current settings are:"
                + "\r\n\tMean: " + Program.UserSettings.calibrationMean
                + "\r\n\tStandard deviation: " + Program.UserSettings.calibrationSD
                + "\r\n\r\nThe results from the calibration are:"
                + "\r\n\tMean:"
                + "\r\n\tStandard deviation:";
            textBox1.Text =
                "Do you want to use the results from this calibration from now on?"
                + "\r\nYou can also manually change the newly obtained values if necessary."
                + "\r\n"
                + "\r\n(This values will only be changed in the test if you also save it.)";

            txtMean.Text = Convert.ToString(Mean);
            txtSD.Text = Convert.ToString(SD);

            btnOnlyTest.Top = this.ClientSize.Height - btnOnlyTest.Height - 5;
            btnAlways.Top = this.ClientSize.Height - btnAlways.Height - 5;
            btnCancel.Top = this.ClientSize.Height - btnCancel.Height - 5;
            btnCancel.Left = this.ClientSize.Width - btnCancel.Width - 5;
            btnAlways.Left = btnCancel.Left - btnAlways.Width - 3;
            btnOnlyTest.Left = btnAlways.Left - btnOnlyTest.Width - 3;
        }

        private void CheckCustom()
        {
            try
            {
                var m = Convert.ToDouble(txtMean.Text);
                var s = Convert.ToDouble(txtSD.Text);
                if (m > 0 && m < 100 & s > 0 & s < 10)
                {
                    Mean = m;
                    SD = s;
                }
                else
                {
                    MessageBox.Show("The values you entered are outside the bounds. No changes were made.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("The values you entered are not valid numbers. No changes were made.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOnlyTest_Click(object sender, EventArgs e)
        {
            CheckCustom();
            Program.Test.CalibrationMean = Mean;
            Program.Test.CalibrationSD = SD;
            this.DialogResult = DialogResult.Yes;
            this.Visible = false;
        }

        private void btnAlways_Click(object sender, EventArgs e)
        {
            CheckCustom();
            Program.Test.CalibrationMean = Mean;
            Program.Test.CalibrationSD = SD;
            Program.UserSettings.calibrationMean = Mean;
            Program.UserSettings.calibrationSD = SD;
            this.DialogResult = DialogResult.Yes;
            this.Visible = false;
        }               

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Visible = false;
        }

        
    }
}
