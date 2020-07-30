using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using MC.Other;

namespace MC.Forms
{
    public partial class frmProcessing : Form
    {
        Control parent;

        public frmProcessing(Control Parent)
        {
            TopLevel = false;
            parent = Parent;
            this.Parent = parent;
            parent.Enabled = false;
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Logger.LogLow("Error initializing frmProcessing. " + ex.Message);
            }
            timer.Interval = 50;
            timer.Enabled = true;            
            var p = parent.PointToClient(new Point(0, 0));
            this.Left = (parent.Width - this.Width) / 2;
            this.Top = (parent.Height - this.Height) / 2;
            BringToFront();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            processingImage.Increase();
        }
        
        public new void Dispose()
        {
            parent.Enabled = true;
            Dispose(true);
        }

        

    }

}
