using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using PdfSharp;
using PdfSharp.Drawing;
using MC.Testing;
using MC.PaperTools;
using MC.Other;
using MC.Forms;
using MC.Graphical;

namespace MC.Design
{
    public class ImageBox : TestElement
    {
        [CategoryAttribute("Layout")]
        [DisplayName("Image mode"), Description("Choose between different ways to place the image inside the box.")]
        public ImageMode ImageMode {get;set;}

        [CategoryAttribute("Layout")]
        [DisplayName("Rotation"), Description("Rotation of the image.")]
        public Rotation Rotation { get; set; }

        [CategoryAttribute("Layout")]
        [DisplayName("Flip"), Description("The way the image is flipped.")]
        public Flip Flip { get; set; }

        [CategoryAttribute("Content")]
        [DisplayName("Image"), Description("Set an image.")]
        public Image Image {get; set;}

        public ImageBox(string name, Page parentPage, Point gridPosition = new Point(), Size gridSize = new Size())
            : base(name, parentPage, gridPosition, gridSize)
        {
            if (gridSize.Width == 0 || gridSize.Height == 0)
            {
                gridSize = new Size(6, 6);
            }
            GridSize = gridSize;
            ImageMode = ImageMode.Fit;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
        {
            using (Graphics g = pe.Graphics)
            {
                g.FillRectangle(new SolidBrush(Color.White), this.DisplayRectangle);
                Size availableSize = new Size(Width - 6, Height - 6);

                if (Image != null)
                {
                    Image flippedRotated = (Image)Image.Clone();
                    if (Flip == Flip.Horizontal)
                    {
                        flippedRotated.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    else if (Flip == Flip.Vertical)
                    {
                        flippedRotated.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    }
                    if (Rotation == Rotation.Rotate_90)
                    {
                        flippedRotated.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    }
                    else if (Rotation == Rotation.Rotate_180)
                    {
                        flippedRotated.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }
                    else if (Rotation == Rotation.Rotate_270)
                    {
                        flippedRotated.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }
                    Image img = null;
                    switch (ImageMode)
                    {
                        case (ImageMode.Crop):
                            img = Processing.Crop(flippedRotated, new Rectangle(0, 0, availableSize.Width, availableSize.Height));
                            pe.Graphics.DrawImage(img, new Point(3, 3));
                            break;
                        case (ImageMode.Fit):
                            float rW = (float)flippedRotated.Width / (float)availableSize.Width;
                            float rH = (float)flippedRotated.Height / (float)availableSize.Height;
                            if (rW > rH)
                            {
                                img = Processing.Resize(flippedRotated, 100 / rW);
                                pe.Graphics.DrawImage(img, new Rectangle(3, 3 + (int)((availableSize.Height - img.Height) / 2), img.Width, img.Height));
                            }
                            else
                            {
                                img = Processing.Resize(flippedRotated, 100 / rH);
                                pe.Graphics.DrawImage(img, new Rectangle(3 + (int)((availableSize.Width - img.Width) / 2), 3, img.Width, img.Height));
                            }
                            break;
                        case (ImageMode.Resize):
                            pe.Graphics.DrawImage(flippedRotated, new Rectangle(3, 3, availableSize.Width, availableSize.Height));
                            break;
                    }
                }
                base.OnPaint(pe);
            }
        }

        public override void PdfDraw(XGraphics g)
        {
            Size availableSize = new Size(Width - 6, Height - 6);
            if (Image != null)
            {
                Image flippedRotated = (Image)Image.Clone();
                if (Flip == Flip.Horizontal)
                {
                    flippedRotated.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                else if (Flip == Flip.Vertical)
                {
                    flippedRotated.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }
                if (Rotation == Rotation.Rotate_90)
                {
                    flippedRotated.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
                else if (Rotation == Rotation.Rotate_180)
                {
                    flippedRotated.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                else if (Rotation == Rotation.Rotate_270)
                {
                    flippedRotated.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
                Image img = null;
                g.SmoothingMode = XSmoothingMode.HighQuality;
                int imageResizeFactor = 4;
                switch (ImageMode)
                {
                    case (ImageMode.Crop):
                        img = Processing.Crop(flippedRotated, new Rectangle(0, 0, availableSize.Width, availableSize.Height));
                        g.DrawImage(img, new Rectangle(Left + 3, Top + 3, availableSize.Width, availableSize.Height));
                        break;
                    case (ImageMode.Fit):
                        float rW = (float)flippedRotated.Width / (float)availableSize.Width;
                        float rH = (float)flippedRotated.Height / (float)availableSize.Height;
                        if (rW > rH)
                        {
                            img = Processing.Resize(flippedRotated, (imageResizeFactor * 100) / rW);
                            g.DrawImage(img, new Rectangle(Left + 3, Top + 3 + ((availableSize.Height - (int)((double)availableSize.Height * rH / rW)) / 2), 
                                availableSize.Width, (int)((double)availableSize.Height * rH / rW)));
                        }
                        else
                        {
                            img = Processing.Resize(flippedRotated, (imageResizeFactor * 100) / rH);
                            g.DrawImage(img, new Rectangle(Left + 3 + ((availableSize.Width - (int)((double)availableSize.Width * rW / rH)) / 2), Top + 3, 
                                (int)((double)availableSize.Width * rW / rH), availableSize.Height));
                        }
                        break;
                    case (ImageMode.Resize):
                        g.DrawImage(flippedRotated, new Rectangle(Left + 3, Top + 3, availableSize.Width, availableSize.Height));
                        break;
                }
            }
            base.PdfDraw(g);
        }
    }

    public enum ImageMode
    {
        Crop,
        Fit,
        Resize
    }

    public enum Rotation
    {
        None = 0,
        Rotate_90 = 1,
        Rotate_180 = 2,
        Rotate_270 = 3
    }

    public enum Flip
    {
        None = 0,
        Horizontal,
        Vertical
    }
}
