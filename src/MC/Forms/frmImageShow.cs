using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MC.Graphical;

namespace MC.Forms
{
   
    public partial class frmImageShow : Form
    {        
        public Image Image
        { 
            get
            {
                return _image;
            }
            set 
            { 
                _image = value; 
                ReNew();
            }
        }
        private Image _image;

        public frmImageShow(Image image, string caption = "")
        {
            InitializeComponent();
            Image = image;
            this.BackgroundImage = Image;
            this.ClientSize = Image.Size;
            this.Text = caption;
            this.Show();
        }

        public frmImageShow(byte[,] imageData, string caption = "")
        {
            InitializeComponent();
            Image = Processing.ByteToImage(imageData);
            this.BackgroundImage = Image;            
            this.ClientSize = Image.Size;
            this.Text = caption;
            this.Show();
        }

        private void ReNew()
        {
            this.BackgroundImage = Image;
        }

    }
}
