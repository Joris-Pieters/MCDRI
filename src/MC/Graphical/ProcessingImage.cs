using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MC.Graphical
{
    /// <summary>
    /// "Waiting bar" during processing
    /// </summary>
    public class ProcessingImage : Control
    {        
        public int Value = 0;
        bool goingUp = true;
        ColorBlend colorBlend;
        
        public ProcessingImage()
        {
            BackgroundImageLayout = ImageLayout.Stretch;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            colorBlend = new ColorBlend();
            colorBlend.Colors = new Color[] {Color.White, Color.Blue, Color.White};
            colorBlend.Positions = new float[] { 0f, .5f, 1f };
        }

        public void Increase(int i = 5)
        {
            if (goingUp)
            {
                Value += i;
            }
            else
            {
                Value -= i;
            }
            if (Value == 100)
            {
                goingUp = false;
            }
            else if (Value == 0)
            {
                goingUp = true;
            }
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (BackgroundImage == null)
            {
                BackgroundImage = (Image)(new Bitmap(100, 1));
            }
            else if (BackgroundImage.Width != 100 || BackgroundImage.Height != 1)
            {
                BackgroundImage = (Image)(new Bitmap(100, 1));
            }
            using (Graphics g = Graphics.FromImage(this.BackgroundImage))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, 100, 1);
                int mid = (int)((Value * 1.2) - 10);
                int halfW = 70 - Math.Abs(50 - mid);
                Rectangle r = new Rectangle(mid - halfW, 0, halfW * 2, 1);
                var b = new LinearGradientBrush(r, Color.Blue, Color.White, 0f);
                b.InterpolationColors = colorBlend;
                g.FillRectangle(b, r);
            }
        }

        
    }
}
