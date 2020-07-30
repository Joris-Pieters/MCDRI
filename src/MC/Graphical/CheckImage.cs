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
using MC.Graphical;
using MC.Testing;
using MC.PaperTools;
using MC.Other;

namespace MC.Graphical
{   
    // Represents one scanned page
    public class CheckImage : IDisposable
    {
        public byte[,] ScannedImage;
        public ItemCheckedState[,] ItemAltsCheckedState;

        /// <summary>
        ///  Relative value between 0 and 1.
        /// </summary>
        public PointF[,] ItemAltsLocation;
        
        public SizeF ObservedBlockSize;
        public ColorDistribution ColorDistribution;
        public byte[] BarCodeBytes;

        /// <summary>
        /// TRUE = ScannedImage is processed image. FALSE: ScannedImage is original scanned image.
        /// </summary>
        public bool ProcessedImage; 

        private string fileName;

        /// <summary>
        /// Calibrates the image and deterimines for each block the ItemCheckedState (even if there is no itemalt at that place)
        /// </summary>
        public CheckImage(string file)
        {
            fileName = file;
            LoadImage();
            ProcessedImage = false;

            BarCodeBytes = new byte[8];
            // 255 means not valid/unknown/didn't pass parity check
            for (int i = 0; i < 8; i++)
            {
                BarCodeBytes[i] = 255;
            }
        }

        private void LoadImage()
        {
            try
            {
                using (Image img = Image.FromFile(fileName))
                {                    
                    ScannedImage = Processing.ImageToByte(img);
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not load " + fileName + ". It might be replaced or removed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.LogLow(ex);
                return;
            }                         
        }

        public void Analyze(float sure, float doubt, ref PageStatus status)
        {            
            // If the image has been analyzed earlier it has to be reloaded
            if (ProcessedImage)
            {
                LoadImage();
                status.Reset();
            }

            // Resizing can only happen as part of RotateAndCrop
            int resize = (int)((Math.Max(Program.Test.Paper.Dimensions.Pixel.Width, Program.Test.Paper.Dimensions.Pixel.Height) * 150) / Math.Max(ScannedImage.GetLength(0), ScannedImage.GetLength(1)));

            // Half searchsize must be bigger than calibrationpoint, but not too big (border / other points) => Difficult, depends on how it is scanned (rotation)
            // After resize the picture is at a standard size ==> fixed searchsize
            int searchSize = 40;

            // Midpoints of itemalts
            ItemAltsLocation = Calibration.RotateAndCrop(Program.Test, ref ScannedImage, ref BarCodeBytes, resize, searchSize);

            if (ItemAltsLocation[0, 0].X != 0 && ItemAltsLocation[0, 0].Y != 0)
            {
                ItemAltsCheckedState = new ItemCheckedState[Program.Test.Paper.Blocks.X, Program.Test.Paper.Blocks.Y];
                ObservedBlockSize = new SizeF(
                    (float)ScannedImage.GetLength(0) / Program.Test.Paper.Blocks.X,
                    (float)ScannedImage.GetLength(1) / Program.Test.Paper.Blocks.Y);
                ColorDistribution = new ColorDistribution();

                if (!BarError(BarCodeBytes))
                {
                    // BarCodeBytes[0] is page number (0 based) ==> has to be smaller than count
                    // Only BarMatch for first three bytes (first is page number, second and third are hash, rest doesn't matter)
                    if (BarCodeBytes[0] < Program.Test.Pages.Count && BarMatch(BarCodeBytes, Program.Test.Pages[BarCodeBytes[0]].Hash(), 3))
                    {
                        CalculateInitialItemAltsCheckedState(BarCodeBytes[0], sure, doubt); // Other way to reach this is after manually setting page number
                    }                
                    else
                    {
                        status.PageNumberOrHashError = true; // Fail: invalid page number according to barcode or page hash not correct
                    }
                }
                else
                {                    
                    status.BarCodeError = true; // Fail: could not read barcode, or not correctly (parity check fail)                    
                }
            }
            else
            {
                status.CalibrationError = true; // Fail: probably couldn't calibrate
            }
            ProcessedImage = true;
            status.Analyzed = true;
        }

        public void CalculateInitialItemAltsCheckedState(int pageNumber, float sure, float doubt)
        {
            if (ItemAltsCheckedState == null)
            {
                ItemAltsCheckedState = new ItemCheckedState[Program.Test.Paper.Blocks.X, Program.Test.Paper.Blocks.Y];
            }
            var itemAlternativeOnLocation = Program.Test.Pages[pageNumber].ItemAlternativeOnLocation();
          
            for (int i = 0; i < Program.Test.Paper.Blocks.X; i++)
            {
                for (int j = 0; j < Program.Test.Paper.Blocks.Y; j++)
                {
                    if (itemAlternativeOnLocation[i, j])
                    {
                        var crop = Processing.Crop(ScannedImage, ItemAltRectangle(i, j));
                        int w = crop.GetLength(0);
                        int h = crop.GetLength(1);
                        
                        double val = 0;
                        Single w2 = (Single)(w - 1) / 2;
                        Single h2 = (Single)(h - 1) / 2;
                        Single max = w2 * w2 * h2 * h2;
                        double old = 0;
                        double nw = 0;
                        for (int x = 0; x < w; x++)
                        {
                            for (int y = 0; y < h; y++)
                            {
                                val += crop[x, y] * (max - ((x - w2) * (x - w2) * (y - h2) * (y - h2))); ;
                                old += crop[x, y];
                                nw += crop[x, y] * (max - ((x - w2) * (x - w2) * (y - h2) * (y - h2)));
                            }
                        }
                        val /= 2.55f * ((w * h * max) - (
                            (w * (w - 1) * (w + 1) / 12) *
                            (h * (h - 1) * (h + 1) / 12)
                            ));
                        ColorDistribution.Add(val);
                        ItemCheckedState state = ItemCheckedState.Unknown;

                        if (val < sure)
                        {
                            state = ItemCheckedState.Checked;                            
                        }
                        else if (val < doubt)
                        {
                            state = ItemCheckedState.Doubt;
                        }
                        else
                        {
                            state = ItemCheckedState.Unchecked;
                        }
                        ItemAltsCheckedState[i, j] = state;
                    }
                    else
                    {
                        ItemAltsCheckedState[i, j] = ItemCheckedState.Unavailable;
                    }
                }
            }
        }

        public SizeF AdjustedBlockSize()
        {
            return new SizeF(
                (2 * (float)ScannedImage.GetLength(0)) / (3 * Program.Test.Paper.Blocks.X), 
                (float)ScannedImage.GetLength(1) / (2 * Program.Test.Paper.Blocks.Y));
        }

        public RectangleF ItemAltRectangle(Point p, int border = 0)
        {
            return ItemAltRectangle(p.X, p.Y, border);
        }

        public RectangleF ItemAltRectangle(int i, int j, int border = 0)
        {
            var sizeAdj = AdjustedBlockSize();           
            return new RectangleF(
                ((ItemAltsLocation[i, j].X * ScannedImage.GetLength(0)) - sizeAdj.Width / 2) - border,
                ((ItemAltsLocation[i, j].Y * ScannedImage.GetLength(1)) - sizeAdj.Height / 2) - border,
                sizeAdj.Width + (2 * border), sizeAdj.Height + (2 * border));
        }

        private bool BarError(byte[] bar)
        {
            bool err = false;
            for (int i = 0; i < bar.GetLength(0); i++)
            {
                if(bar[i] == 255)
                {
                    err = true;
                    break;
                }
            }
            return err;
        }

        private bool BarMatch(byte[] bar1, byte[] bar2, int count = -1)
        {
            if (Settings.IgnoreBarCodeErrors)
            {
                return true;
            }
            int max = Math.Max(bar1.GetLength(0), bar2.GetLength(0));
            if (count > -1 && count < max)
            {
                max = count;
            }
            for(int i = 0; i < max; i++)
            {
                if(bar1[i] != bar2[i])
                {                   
                     return false;
                }
            }
            return true;
        }
        
        public void Dispose()
        {
            ScannedImage = null;
            ItemAltsCheckedState = null;
            ItemAltsLocation = null;
            ColorDistribution = null;
            BarCodeBytes = null;
        }
    }
}
