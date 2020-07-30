using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using MC.Other;
using MC.Testing;
using MC.PaperTools;

namespace MC.Graphical
{
    public static class Processing
    {
        const float g22 = 0.012146f;
        const float g12 = 0.026110f;
        const float g02 = 0.033697f;
        const float g11 = 0.056127f;
        const float g01 = 0.072438f;
        const float g00 = 0.093487f;

        const float PI = (float)Math.PI;

        public static byte[,] ImageToByte(Image data)
        {
            return ImageToByte(new Bitmap(data));
        }

        public static byte[,] ImageToByte(Bitmap data)
        {
            System.Windows.Forms.Application.DoEvents();
            int w = data.Width;
            int h = data.Height;
            byte[,] dataOut = new byte[w, h];
            Bitmap bmp2 = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bmp2))
            {
                g.DrawImage(data, 0, 0);
            }
            BitmapData bitmapData = bmp2.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, bmp2.PixelFormat);
            unsafe
            {
                byte* ptr = (byte*)bitmapData.Scan0;
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        dataOut[j, i] = (byte)(.0721 * ptr[0] + .7154 * ptr[1] + .2125 * ptr[2]);
                        ptr += 3;
                    }
                    ptr += bitmapData.Stride - (3 * w);
                }
            }
            bmp2.UnlockBits(bitmapData);
            return dataOut;
        }

        public static Image ByteToImage(byte[,] data)
        {
            System.Windows.Forms.Application.DoEvents();
            Bitmap bitmap;
            if (data != null)
            {
                int w = data.GetLength(0);
                int h = data.GetLength(1);
                bitmap = new Bitmap(w, h, PixelFormat.Format24bppRgb);
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                unsafe
                {
                    int stride = bitmapData.Stride;
                    byte* ptr = (byte*)bitmapData.Scan0;
                    for (int i = 0; i < h; i++)
                    {
                        for (int j = 0; j < w; j++)
                        {
                            ptr[0] = data[j, i];
                            ptr[1] = data[j, i];
                            ptr[2] = data[j, i];
                            ptr += 3;
                        }
                        ptr += bitmapData.Stride - (3 * w);
                    }
                }
                bitmap.UnlockBits(bitmapData);
            }
            else
            {
                bitmap = new Bitmap(1, 1);
            }
            return (Image)bitmap;
        }

        public static byte[,] FloatToByte(float[,] data)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);

            float min = 1000000;
            float max = -1000000;
            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < h; i++)
                {
                    min = Math.Min(min, data[j, i]);
                    max = Math.Max(max, data[j, i]);
                }
            }

            var dataOut = new byte[w, h];
            if (max > min)
            {
                for (int j = 0; j < w; j++)
                {
                    for (int i = 0; i < h; i++)
                    {
                        dataOut[j, i] = (byte)(255 * (data[j, i] - min) / (max - min));
                    }
                }
            }
            return dataOut;
        }

        public static byte[,] Copy(byte[,] data)
        {
            byte[,] floatOut = new byte[data.GetLength(0), data.GetLength(1)];
            for (int j = 0; j < data.GetLength(1); j++)
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    floatOut[i, j] = data[i, j];
                }
            }
            return floatOut;
        }

        public static Image Resize(Image data, double percent)
        {
            Bitmap bmp = new Bitmap((int)(data.Width * percent / 100), (int)(data.Height * percent / 100));
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(data, new Rectangle(0, 0, bmp.Width, bmp.Height));
            }
            return (Image)bmp;
        }

        public static Image Resize(Image data, int width, int height, int border = 0)
        {
            Bitmap bmp = new Bitmap(width + (2 * border), height + (2 * border));
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(data, new Rectangle(border, border, width, height));
            }    
            return (Image)bmp;
        }

        public static byte[,] Resize(byte[,] data, double percent)
        {
            return ImageToByte(Resize(ByteToImage(data), percent));
        }

        public static byte[,] Resize(byte[,] data, int width, int height)
        {
            return ImageToByte(Resize(ByteToImage(data), width, height));
        }

        public static Image Crop(Image data, RectangleF rectangle)
        {
            return Crop(data, IntegerRectangle(rectangle));
        }

        public static Image Crop(Image data, Rectangle rectangle)
        {
            Image dataOut = null;
            if (rectangle.Width > 0 && rectangle.Height > 0)
            {
                dataOut = (Image)(new Bitmap(rectangle.Width, rectangle.Height));
                using (Graphics g = Graphics.FromImage(dataOut))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.DrawImage(data, new Rectangle(0, 0, rectangle.Width, rectangle.Height), rectangle, GraphicsUnit.Pixel);
                }
            }
            return dataOut;
        }

        public static byte[,] Crop(byte[,] data, RectangleF rectangle)
        {
            return Crop(data, IntegerRectangle(rectangle));
        }

        public static byte[,] Crop(byte[,] data, Rectangle rectangle)
        {
            System.Windows.Forms.Application.DoEvents();
            int w = data.GetLength(0);
            int h = data.GetLength(1);

            byte[,] dataOut = null;
            if (rectangle.Width > 0 && rectangle.Height > 0)
            {
                dataOut = new byte[rectangle.Width, rectangle.Height];
                for (int j = 0; j < rectangle.Width; j++)
                {
                    for (int i = 0; i < rectangle.Height; i++)
                    {
                        if (rectangle.Left + j >= 0 && rectangle.Top + i >= 0 && rectangle.Left + j < w && rectangle.Top + i < h)
                        {
                            dataOut[j, i] = data[rectangle.Left + j, rectangle.Top + i];
                        }
                    }
                }
            }
            return dataOut;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="threshold">In percent</param>
        /// <returns></returns>
        public static byte[,] EdgeDetection(byte[,] data, float threshold = 50)
        {
            float[,] gauss = Gaussian(data);
            int w = gauss.GetLength(0);
            int h = gauss.GetLength(1);

            float[,] derivativeX = SobelX(gauss);
            float[,] derivativeY = SobelY(gauss);
            float[,] gradient = new float[w, h];
            float[,] arctan = new float[w, h];
            float[,] nonMax = new float[w, h];

            System.Windows.Forms.Application.DoEvents();
            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < h; i++)
                {
                    gradient[j, i] = (float)Math.Sqrt((derivativeX[j, i] * derivativeX[j, i]) + (derivativeY[j, i] * derivativeY[j, i]));
                    if (derivativeX[j, i] == 0)
                    {
                        arctan[j, i] = PI / 2;
                    }
                    else
                    {
                        arctan[j, i] = (float)Math.Atan(derivativeY[j, i] / derivativeX[j, i]);
                    }
                    nonMax[j, i] = gradient[j, i];
                }
            }

            System.Windows.Forms.Application.DoEvents();
            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < h; i++)
                {
                    if (i == 0 || i == h - 1 || j == 0 || j == w - 1)
                    {
                        nonMax[j, i] = 0;
                    }
                    else
                    {
                        if (arctan[j, i] > -3 * PI / 8 && arctan[j, i] <= -PI / 8)
                        {
                            nonMax[j, i] = gradient[j, i] < gradient[j + 1, i - 1] || gradient[j, i] < gradient[j - 1, i + 1] ? 0 : nonMax[j, i];
                        }
                        else if (arctan[j, i] > -PI / 8 && arctan[j, i] <= PI / 8)
                        {
                            nonMax[j, i] = gradient[j, i] < gradient[j, i + 1] || gradient[j, i] < gradient[j, i - 1] ? 0 : nonMax[j, i];
                        }
                        else if (arctan[j, i] > PI / 8 && arctan[j, i] <= 3 * PI / 8)
                        {
                            nonMax[j, i] = gradient[j, i] < gradient[j + 1, i + 1] || gradient[j, i] < gradient[j - 1, i - 1] ? 0 : nonMax[j, i];
                        }
                        else
                        {
                            nonMax[j, i] = gradient[j, i] < gradient[j + 1, i] || gradient[j, i] < gradient[j - 1, i] ? 0 : nonMax[j, i];
                        }
                    }
                }
            }

            System.Windows.Forms.Application.DoEvents();
            float max = -1000;
            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < h; i++)
                {
                    max = Math.Max(max, nonMax[j, i]);
                }
            }

            System.Windows.Forms.Application.DoEvents();
            threshold *= max / 100;
            byte[,] imgOut = new byte[w, h];
            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < h; i++)
                {
                    if (nonMax[j, i] >= threshold)
                    {
                        imgOut[j, i] = 255;
                    }
                    else
                    {
                        imgOut[j, i] = 0;
                    }
                }
            }

            GC.Collect();

            return imgOut;
        }

        public static Image DrawCurve(Image data, PointF[] points, Color color, float thickness)
        {
            if (points.GetLength(0) >= 2)
            {
                using (Graphics g = Graphics.FromImage(data))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (Pen p = new Pen(color, thickness))
                    {
                        for (int i = 0; i < points.GetLength(0) - 1; i++)
                        {
                            if (!float.IsNaN(points[i].Y) && !float.IsNaN(points[i + 1].Y))
                            {
                                g.DrawLine(p, points[i], points[i + 1]);
                            }
                        }
                    }
                }
            }
            return data;
        }

        // Size = 5, sigma = 1.4
        public static float[,] Gaussian(byte[,] data)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);
            int w2 = w - 4;
            int h2 = h - 4;
            float[,] output = new float[w2, h2];

            for (int j = 2; j < w2 + 2; j++)
            {
                for (int i = 2; i < h2 + 2; i++)
                {
                    output[j - 2, i - 2] =
                        +data[j - 2, i - 2] * g22
                        + data[j - 2, i - 1] * g12
                        + data[j - 2, i] * g02
                        + data[j - 2, i + 1] * g12
                        + data[j - 2, i + 2] * g22
                        + data[j - 1, i - 2] * g12
                        + data[j - 1, i - 1] * g11
                        + data[j - 1, i] * g01
                        + data[j - 1, i + 1] * g11
                        + data[j - 1, i + 2] * g12
                        + data[j, i - 2] * g02
                        + data[j, i - 1] * g01
                        + data[j, i] * g00
                        + data[j, i + 1] * g01
                        + data[j, i + 2] * g02
                        + data[j + 1, i - 2] * g12
                        + data[j + 1, i - 1] * g11
                        + data[j + 1, i] * g01
                        + data[j + 1, i + 1] * g11
                        + data[j + 1, i + 2] * g12
                        + data[j + 2, i - 2] * g22
                        + data[j + 2, i - 1] * g12
                        + data[j + 2, i] * g02
                        + data[j + 2, i + 1] * g12
                        + data[j + 2, i + 2] * g22;
                }
            }
            return output;
        }

        private static float[,] SobelX(float[,] data)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);
            int w1 = w - 1;
            int h1 = h - 1;
            float[,] output = new float[w, h];

            output[0, 0] = -data[0, 1] - data[1, 1];
            output[0, h1] = data[0, h1 - 1] + data[1, h1 - 1];
            output[w1, 0] = -data[w1, 1] - data[w1 - 1, 1];
            output[w1, h1] = data[w1, h1 - 1] + data[w1 - 1, h1 - 1];
            for (int j = 1; j < w1; j++)
            {
                output[j, 0] =
                        -data[j - 1, 1]
                        - data[j, 1]
                        - data[j + 1, 1];
                output[j, h1] =
                        +data[j - 1, h1 - 1]
                        + data[j, h1 - 1]
                        + data[j + 1, h1 - 1];
                for (int i = 1; i < h1; i++)
                {
                    output[j, i] =
                        +data[j - 1, i - 1]
                        - data[j - 1, i + 1]
                        + data[j, i - 1]
                        - data[j, i + 1]
                        + data[j + 1, i - 1]
                        - data[j + 1, i + 1];
                }
            }
            for (int i = 1; i < h1; i++)
            {
                output[0, i] =
                        +data[0, i - 1]
                        - data[0, i + 1]
                        + data[1, i - 1]
                        - data[1, i + 1];
                output[w1, i] =
                        +data[w1 - 1, i - 1]
                        - data[w1 - 1, i + 1]
                        + data[w1, i - 1]
                        - data[w1, i + 1];
            }
            return output;
        }

        private static float[,] SobelY(float[,] data)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);
            int w1 = w - 1;
            int h1 = h - 1;
            float[,] output = new float[w, h];

            output[0, 0] = -data[1, 0] - data[1, 1];
            output[0, h1] = -data[1, h1] - data[1, h1 - 1];
            output[w1, 0] = data[w1 - 1, 0] + data[w1 - 1, 1];
            output[w1, h1] = data[w1 - 1, h1] + data[w1 - 1, h1 - 1];
            for (int j = 1; j < w1; j++)
            {
                output[j, 0] =
                      data[j - 1, 0]
                    + data[j - 1, 1]
                    - data[j + 1, 0]
                    - data[j + 1, 1];
                output[j, h1] =
                      data[j - 1, h1 - 1]
                    + data[j - 1, h1]
                    - data[j + 1, h1 - 1]
                    - data[j + 1, h1];
                for (int i = 1; i < h1; i++)
                {
                    output[j, i] =
                          data[j - 1, i - 1]
                        + data[j - 1, i]
                        + data[j - 1, i + 1]
                        - data[j + 1, i - 1]
                        - data[j + 1, i]
                        - data[j + 1, i + 1];
                }
            }
            for (int i = 1; i < h1; i++)
            {
                output[0, i] =
                        -data[1, i - 1]
                        - data[1, i]
                        - data[1, i + 1];
                output[w1, i] =
                          data[w1 - 1, i - 1]
                        + data[w1 - 1, i]
                        + data[w1 - 1, i + 1];

            }
            return output;
        }

        public static float[,] Convolution(float[,] data, float[,] kernel)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);
            int Fw = kernel.GetLength(0) / 2;
            int Fh = kernel.GetLength(1) / 2;

            float sum = 0;
            float[,] output = new float[w, h];

            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < h; i++)
                {
                    sum = 0;
                    for (int k = -Fh; k <= Fh; k++)
                    {
                        if (j + k >= 0 && j + k < w)
                        {
                            for (int l = -Fw; l <= Fw; l++)
                            {
                                if (i + l >= 0 && i + l < h)
                                {
                                    sum += data[j + k, i + l] * kernel[Fw + k, Fh + l];
                                }
                            }
                        }
                    }
                    output[j, i] = sum;
                }
            }
            return output;
        }

        private static float[,] CreateGaussianKernel(int size, float sigma)
        {
            if (size % 2 == 0)
            {
                size++;
            }
            float[,] output = new float[size, size];

            float d1 = (float)(1 / (2 * Math.PI * sigma * sigma));
            float d2 = 2 * sigma * sigma;
            float sum = 0;

            for (int j = -size / 2; j <= size / 2; j++)
            {
                for (int i = -size / 2; i <= size / 2; i++)
                {
                    output[size / 2 + i, size / 2 + j] = d1 * (float)Math.Exp(-(i * i + j * j) / d2);
                    sum += output[size / 2 + i, size / 2 + j];
                }
            }

            for (int j = -size / 2; j <= size / 2; j++)
            {

                for (int i = -size / 2; i <= size / 2; i++)
                {
                    output[size / 2 + i, size / 2 + j] /= sum;
                }
            }
            return output;
        }

        public static byte[,] WhiteCircle(int width, int height, RectangleF rect)
        {
            byte[,] data = new byte[width, height];

            PointF mid = new PointF(rect.Left + ((float)rect.Width / 2), rect.Top + ((float)rect.Height / 2));
            float r = (((float)rect.Width / 2) + ((float)rect.Height / 2)) / 2;

            for (int j = 0; j < width; j++)
            {
                for (int i = 0; i < height; i++)
                {
                    if (Math.Sqrt(((j - mid.X) * (j - mid.X)) + ((i - mid.Y) * (i - mid.Y))) <= r)
                    {
                        data[j, i] = 1;
                    }
                    else
                    {
                        data[j, i] = 0;
                    }
                }
            }

            return data;
        }

        public static double AverageDifference(byte[,] a, byte[,] b)
        {
            int w = a.GetLength(0);
            int h = a.GetLength(1);

            if (b.GetLength(0) != w || b.GetLength(1) != h)
            {
                return double.MaxValue;
            }
            else
            {
                double sum = 0;
                for (int j = 0; j < w; j++)
                {
                    for (int i = 0; i < h; i++)
                    {
                        if ((a[j, i] == 0 && b[j, i] != 0) || (a[j, i] != 0 && b[j, i] == 0))
                        {
                            sum++;
                        }
                    }
                }
                return sum / (double)(w * h);
            }
        }

        public static void Print(float[,] data, int round = 0, string split = "")
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);

            for (int i = 0; i < h; i++)
            {
                string str = "";
                for (int j = 0; j < w; j++)
                {
                    str += Math.Round(data[j, i], round) + split;
                }
                Logger.LogHigh(str);
            }
            Logger.LogHigh("---------------------------------");
        }

        public static byte[,] FloodFill(byte[,] data, float value, PointF start)
        {
            System.Windows.Forms.Application.DoEvents();
            int w = data.GetLength(0);
            int h = data.GetLength(1);

            byte[,] dataOut = new byte[w, h];
            Array.Copy(data, dataOut, w * h);
            if (!(start.X < 0 || start.Y < 0 || start.X >= w || start.Y >= w))
            {
                var pntList = new List<Point>();
                pntList.Add(new Point((int)start.X, (int)start.Y));
                float current = data[pntList[0].X, pntList[0].Y];
                while (pntList.Count > 0)
                {
                    var pnt = pntList[0];
                    pntList.RemoveAt(0);
                    if (!(pnt.X < 0 || pnt.Y < 0 || pnt.X >= w || pnt.Y >= h))
                    {
                        if (dataOut[pnt.X, pnt.Y] == current)
                        {
                            dataOut[pnt.X, pnt.Y] = (byte)value;
                            if (pnt.X > 0)
                                if (dataOut[pnt.X - 1, pnt.Y] != value)
                                    pntList.Add(new Point(pnt.X - 1, pnt.Y));
                            if (pnt.Y > 0)
                                if (dataOut[pnt.X, pnt.Y - 1] != value)
                                    pntList.Add(new Point(pnt.X, pnt.Y - 1));
                            if (pnt.X < w - 1)
                                if (dataOut[pnt.X + 1, pnt.Y] != value)
                                    pntList.Add(new Point(pnt.X + 1, pnt.Y));
                            if (pnt.Y < h - 1)
                                if (dataOut[pnt.X, pnt.Y + 1] != value)
                                    pntList.Add(new Point(pnt.X, pnt.Y + 1));
                        }
                    }
                }
            }
            return dataOut;
        }

        public static float SumOfBorder(byte[,] data)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);
            float sum = 0;

            for (int j = 0; j < h; j++)
            {
                sum += data[0, j] + data[w - 1, j];
            }
            for (int i = 1; i < w - 1; i++)
            {
                sum += data[i, 0] + data[i, h - 1];
            }
            return sum;
        }

        public static byte[,] Transpose(byte[,] data)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);

            var dataOut = new byte[h, w];
            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < h; i++)
                {
                    dataOut[i, j] = data[j, i];
                }
            }
            return dataOut;
        }        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>N E(x) E(y) Var(x) Var(y)</returns>
        public static float[] Moments(byte[,] data)
        {
            float[] mom = new float[5];

            int w = data.GetLength(0);
            int h = data.GetLength(1);

            for (int j = 0; j < w; j++)
            {
                for (int i = 0; i < h; i++)
                {
                    mom[0] += data[j, i];
                    mom[1] += j * data[j, i];
                    mom[2] += i * data[j, i];
                    mom[3] += j * j * data[j, i];
                    mom[4] += i * i * data[j, i];
                }
            }
            for (int i = 1; i <= 4; i++)
            {
                if (mom[0] != 0)
                {
                    mom[i] /= mom[0];
                }
                else
                {
                    mom[i] = 0;
                }
            }
            mom[3] -= mom[1] * mom[1];
            mom[4] -= mom[2] * mom[2];

            return mom;
        }

        public static float Average(byte[,] data)
        {
            int w = data.GetLength(0);
            int h = data.GetLength(1);

            float sum = 0;
            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    sum += data[i, j];
                }
            }
            return sum / (w * h);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="percetile">Between 0 and 1</param>
        /// <returns></returns>
        public static float Percentile(byte[,] data, float percetile)
        {
            if (percetile < 0 || percetile > 1)
            {
                return 0;
            }
            else
            {
                int w = data.GetLength(0);
                int h = data.GetLength(1);

                List<float> valList = new List<float>();
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        valList.Add(data[j, i]);
                    }
                }
                valList.Sort();
                return valList[(int)((float)(valList.Count - 1) * percetile)];
            }
        }

        public static Image Histogram(List<double> data, bool addNormal = false, int min = 0, int max = 100, int width = 600, int height = 400)
        {
            try
            {
                int[] values = new int[max - min + 1];
                float a1 = 0;
                float a2 = 0;
                int highest = 0;
                foreach (float dat in data)
                {
                    var iDat = (int)dat;
                    if (iDat >= min && iDat <= max)
                    {
                        values[iDat - min]++;
                        highest = Math.Max(highest, values[iDat]);
                        a1 += dat;
                        a2 += dat * dat;
                    }
                }
                a1 /= data.Count;
                a2 /= data.Count;
                float var = a2 - (a1 * a1);

                int xAxisHeight = height - 25;
                int pixelPerF = (int)Math.Floor((double)xAxisHeight / highest);

                Single d = (Single)width / (max - min + 1);
                Image img = (Image)new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(img))
                {
                    g.FillRectangle(new SolidBrush(Color.White), 0, 0, width, xAxisHeight);
                    g.FillRectangle(new SolidBrush(Color.FromKnownColor(KnownColor.Control)), 0, xAxisHeight + 1, width, height - xAxisHeight);
                    using (var p = new Pen(Color.FromKnownColor(KnownColor.ControlDark)))
                    {
                        using (var b = new SolidBrush(Color.FromKnownColor(KnownColor.ControlDark)))
                        {
                            for (int i = 0; i < max - min + 1; i++)
                            {
                                if (values[i] != 0)
                                {
                                    g.FillRectangle(b, i * d, xAxisHeight - (pixelPerF * values[i]), d, pixelPerF * values[i]);
                                }
                                if (i < max && i % 10 == 0)
                                {
                                    g.DrawLine(new Pen(Color.Black), i * d, xAxisHeight, i * d, xAxisHeight + 4);
                                    g.DrawString(Convert.ToString(i), new Font("Microsoft Sans Serif", 8.25F), new SolidBrush(Color.Black), i * d, xAxisHeight + 4);
                                }
                            }
                        }
                    }
                    if (addNormal)
                    {
                        using (var p2 = new Pen(Color.FromKnownColor(KnownColor.ControlDarkDark), 2))
                        {
                            if (var > 0)
                            {
                                for (int i = 0; i < (int)(max - min + 1) - 1; i++)
                                {
                                    g.DrawLine(p2, i * d, xAxisHeight - (int)(((pixelPerF * data.Count) / (Math.Sqrt(2 * Math.PI * var))) * Math.Exp(-((i - a1) * (i - a1)) / (2 * var))),
                                               (i + 1) * d, xAxisHeight - (int)(((pixelPerF * data.Count) / (Math.Sqrt(2 * Math.PI * var))) * Math.Exp(-((i + 1 - a1) * (i + 1 - a1)) / (2 * var))));
                                }
                            }
                        }
                    }
                    g.DrawLine(new Pen(Color.Black), 0, xAxisHeight, (int)(max - min + 1), xAxisHeight);
                }

                return img;
            }
            catch (Exception ex)
            {
                Logger.LogHigh("Error creating histogram: " + ex.Message);
            }
            return (Image)(new Bitmap(0, 0));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="an">In radians</param>
        /// <returns></returns>
        public static byte[,] Rotate(byte[,] data, double angle)
        {
            return ImageToByte(Rotate(ByteToImage(data), angle));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="an">In radians</param>
        /// <returns></returns>
        public static Image Rotate(Image image, double angle)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            else if (image.Width > 0 && image.Height > 0 && !double.IsNaN(angle))
            {
                const double pi2 = Math.PI / 2.0;

                double oldWidth = (double)image.Width;
                double oldHeight = (double)image.Height;
                double locked_theta = angle;

                while (locked_theta < 0.0)
                    locked_theta += 2 * Math.PI;

                double newWidth, newHeight;
                int nWidth, nHeight;

                double adjacentTop, oppositeTop;
                double adjacentBottom, oppositeBottom;

                if ((locked_theta >= 0.0 && locked_theta < pi2) ||
                    (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2)))
                {
                    adjacentTop = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                    oppositeTop = Math.Abs(Math.Sin(locked_theta)) * oldWidth;

                    adjacentBottom = Math.Abs(Math.Cos(locked_theta)) * oldHeight;
                    oppositeBottom = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                }
                else
                {
                    adjacentTop = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                    oppositeTop = Math.Abs(Math.Cos(locked_theta)) * oldHeight;

                    adjacentBottom = Math.Abs(Math.Sin(locked_theta)) * oldWidth;
                    oppositeBottom = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                }

                newWidth = adjacentTop + oppositeBottom;
                newHeight = adjacentBottom + oppositeTop;

                nWidth = (int)Math.Ceiling(newWidth);
                nHeight = (int)Math.Ceiling(newHeight);

                Bitmap rotatedBmp = new Bitmap(nWidth, nHeight);

                using (Graphics g = Graphics.FromImage(rotatedBmp))
                {
                    using (Brush b = new SolidBrush(Color.White))
                    {
                        g.FillRectangle(b, new Rectangle(0, 0, nWidth, nHeight));
                    }
                    Point[] points;

                    if (locked_theta >= 0.0 && locked_theta < pi2)
                    {
                        points = new Point[] { 
											 new Point((int)oppositeBottom, 0 ), 
											 new Point(nWidth, (int)oppositeTop ),
											 new Point(0, (int)adjacentBottom )
										 };
                    }
                    else if (locked_theta >= pi2 && locked_theta < Math.PI)
                    {
                        points = new Point[] { 
											 new Point(nWidth, (int)oppositeTop ),
											 new Point((int)adjacentTop, nHeight ),
											 new Point((int)oppositeBottom, 0 )						 
										 };
                    }
                    else if (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2))
                    {
                        points = new Point[] { 
											 new Point((int)adjacentTop, nHeight ), 
											 new Point(0, (int)adjacentBottom ),
											 new Point(nWidth, (int)oppositeTop )
										 };
                    }
                    else
                    {
                        points = new Point[] { 
											 new Point(0, (int)adjacentBottom ), 
											 new Point((int)oppositeBottom, 0 ),
											 new Point((int)adjacentTop, nHeight )		
										 };
                    }

                    g.DrawImage(image, points);
                }

                return (Image)rotatedBmp;
            }
            else
            {
                return image;
            }
        }

        public static PointF Rotate(this PointF point, PointF center, double angle)
        {
            double newAngle = Math.Atan((point.Y - center.Y) / (point.X - center.X)) + angle;
            if (point.X < center.X)
            {
                newAngle += (float)Math.PI;
            }
            while (newAngle > 2 * Math.PI)
            {
                newAngle -= (float)(2 * Math.PI);
            }
            float r = (float)Math.Sqrt(Math.Pow(point.X - center.X, 2) + Math.Pow(point.Y - center.Y, 2));
            return new PointF((float)(center.X + r * Math.Cos(newAngle)),
                              (float)(center.Y + r * Math.Sin(newAngle)));
        }

        public static Rectangle IntegerRectangle(RectangleF rectangleF)
        {
            return new Rectangle((int)rectangleF.X, (int)rectangleF.Y, (int)rectangleF.Width, (int)rectangleF.Height);
        }

        public static byte[,] Binarize(byte[,] data, byte threshold)
        {
            System.Windows.Forms.Application.DoEvents();
            byte[,] imgOut = new byte[data.GetLength(0), data.GetLength(1)];
            for (int j = 0; j < data.GetLength(1); j++)
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    imgOut[i, j] = data[i, j] < threshold ? (byte)0 : (byte)255;
                }
            }
            return imgOut;
        }

        // Mean split
        public static byte[,] Binarize(byte[,] data)
        {
            System.Windows.Forms.Application.DoEvents();
            byte[,] imgOut = new byte[data.GetLength(0), data.GetLength(1)];

            int sum = 0;
            for (int j = 0; j < data.GetLength(1); j++)
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    sum += data[i, j];
                }
            }
            return Binarize(data, (byte)((double)sum / (data.GetLength(0) * data.GetLength(1))));
        }

        // "Otsu's method"
        public static byte[,] BinarizeOtsu(byte[,] data, int min = 0, int max = 255, int step = 1)
        {
            System.Windows.Forms.Application.DoEvents();

            int range = (int)Math.Floor((double)(max - min) / (double)step) + 1;
            int[] values = new int[range];
            for (int j = 0; j < data.GetLength(1); j++)
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    values[(int)Math.Floor(data[i, j] / (float)step)]++;
                }
            }

            double sum = 0;
            for (int i = 0; i < range; i++)
            {
                sum += i * values[i];
            }

            double sumB = 0;
            double wB = 0;
            double wF = 0;
            double mB;
            double mF;
            double mMax = 0;
            double between;
            double threshold = 0;
            double count = data.GetLength(0) * data.GetLength(1);

            for (int i = 0; i < range; i++)
            {
                wB += values[i];
                if (wB != 0)
                {
                    wF = count - wB;
                    if (wF == 0)
                    {
                        break;
                    }
                    sumB += i * values[i];
                    mB = sumB / wB;
                    mF = (sum - sumB) / wF;
                    between = wB * wF * Math.Pow(mB - mF, 2);
                    if (between >= mMax)
                    {
                        mMax = between;
                        threshold = i;
                    }
                }
            }
            return Binarize(data, (byte)threshold);
        }

    }
}