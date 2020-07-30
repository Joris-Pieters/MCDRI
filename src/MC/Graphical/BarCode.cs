using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using MC.Testing;
using MC.PaperTools;
using MC.Forms;
using MC.Other;

namespace MC.Graphical
{
    public static class BarCode
    {        
        /* Disadvantage: upside down gives other result, but that's not a problem here
          CC => calibration (4)
          ██ => bit (64)
          PP => parity (8)
          
          ╔════════════════════════════════════════╗
          ║ CC████████████████PP████████████████CC ║
          ║ PP████████████████PP████████████████PP ║
          ║ PP████████████████PP████████████████PP ║
          ║ CC████████████████PP████████████████CC ║
          ╚════════════════════════════════════════╝
                    
          +----------------------------+
          |                             |
          |     bbbbbbbb -> P bbbbbbbb -+
          |                                
          +-> P bbbbbbbb -> P bbbbbbbb -> P
          
          +-> P bbbbbbbb -> P bbbbbbbb -> P
          |                             
          |     bbbbbbbb -> P bbbbbbbb -+
          |                             |
          +----------------------------+
         
         */

        public static Image CreateImage(byte[] bytes, int blockSize)
        {
            var array = CreateArray(bytes);
            var img = (Image)(new Bitmap(19 * blockSize, 4 * blockSize));
            using (Graphics g = Graphics.FromImage(img))
            {
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, img.Width, img.Height));
                using (var b = new SolidBrush(Color.FromArgb(Program.Test.BarCodeGray, Program.Test.BarCodeGray, Program.Test.BarCodeGray)))
                {
                    for (int i = 0; i < 19; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (array[i, j] == 0) // Dark is zero, white is one (same as binarized image)
                            {
                                g.FillRectangle(b, i * blockSize, j * blockSize, blockSize, blockSize);
                            }
                        }
                    }
                }
            }
            return img;
        }

        public static byte[,] CreateArray(byte[] bytes)
        {
            var array = new byte[19, 4];
            if (bytes.GetLength(0) == 8)
            {
                for (int b = 0; b < 8; b++)
                {
                    var binary = Split(bytes[b]);
                    int row = (int)Math.Floor((double)b / 2);
                    int startColumn = b % 2 == 0 ? 1 : 10;
                    for (int i = 0; i < 8; i++)
                    {
                        array[startColumn + i, row] = binary[i];
                    }
                    if (startColumn == 1 || row == 1 || row == 2)
                    {
                        array[startColumn + 8, row] = Parity(binary);
                    }
                    else if (row == 0)
                    {
                        array[0, 1] = Parity(binary);
                    }
                    else if (row == 3)
                    {
                        array[0, 2] = Parity(binary);
                    }
                }
                array[0, 0] = 0;
                array[18, 0] = 0;
                array[0, 3] = 0;
                array[18, 3] = 0;
            }
            return array;
        }

        public static byte[] ReadFrom2DArray(byte[,] array)
        {
            byte[] bytes = new byte[8];
            if (array != null)
            {
                if (array.GetLength(0) == 19 && array.GetLength(1) == 4)
                {                    
                    for (int b = 0; b < 8; b++)
                    {
                        int row = (int)Math.Floor((double)b / 2);
                        int startColumn = b % 2 == 0 ? 1 : 10;                       
                        byte byt = 0;
                        byte parBit = 0;
                        if (startColumn == 1 || row == 1 || row == 2)
                        {
                            parBit = array[startColumn + 8, row];
                        }
                        else if (row == 0)
                        {
                            parBit = array[0, 1];
                        }
                        else if (row == 3)
                        {
                            parBit = array[0, 2];
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            byt += (byte)(array[startColumn + i, row] * Math.Pow(2, 7 - i));                            
                        }
                        if(Parity(Split(byt)) != parBit)
                        {
                            byt = 255; // give byte value 255 in case of invalid parity
                        }
                        bytes[b] = byt;
                    }
                }
            }           
            return bytes;
        }

        public static string ToString(byte[] bytes)
        {
            string str = "";
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                if (bytes[i] == 255)
                {
                    str += "?";
                } 
                else
                {
                    str += (char)bytes[i];
                }
            }
            return str;
        }

        /// <summary>
        /// Converts a byte to binary
        /// </summary>
        private static byte[] Split(byte b)
        {
            var charArr = Convert.ToString(Convert.ToInt32(Convert.ToString(b), 10), 2).ToCharArray();
            var binary = new byte[8];
            for (int i = 0; i < 8 - charArr.GetLength(0); i++)
            {
                binary[i] = 0;
            }
            for (int i = 0; i < charArr.GetLength(0); i++)
            {
                if (charArr[i] == 49)
                {
                    binary[8 - charArr.GetLength(0) + i] = 1;
                }
                else
                {
                    binary[8 - charArr.GetLength(0) + i] = 0;
                }
            }            
            return binary;
        }

        private static byte Parity(byte[] data)
        {            
            int sum = 0;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i] == 1)
                {
                    sum++;
                }
            }
            return sum % 2 == 0 ? (byte)1 : (byte)0;
        }

        private static byte Parity(byte data)
        {           
           return Parity(Split(data));
        }

        public static byte[] Read(Image image)
        {
            return Read(Processing.ImageToByte(image));
        }

        // Persumed to be rotated correctly (but not yet cropped)
        public static byte[] Read(byte[,] imageData)
        {            
            byte[,] array = new byte[19, 4];

            if (imageData != null)
            {                
                if (imageData.GetLength(0) > 0 && imageData.GetLength(1) > 0)
                {                    
                    imageData = Processing.FloatToByte(Processing.Gaussian(imageData));
                    imageData = Processing.Binarize(imageData, 200);

                    int minBlack = Math.Min(4, Math.Min(imageData.GetLength(0) / 19, imageData.GetLength(1) / 4));
                    
                    Point p1 = new Point(-1, -1);
                    Point p2 = new Point(-1, -1);
                   
                    for (int i = 0; i < (imageData.GetLength(0) / 2); i++)
                    {
                        int sum1 = 0;
                        int sum2 = 0;
                        for (int j = 0; j < imageData.GetLength(1); j++)
                        {
                            sum1 += imageData[i, j] > 0 ? 0 : 1;
                            sum2 += imageData[imageData.GetLength(0) - i - 1, j] > 0 ? 0 : 1;
                        }
                        if (p1.X == -1 && sum1 >= minBlack)
                        {
                            p1.X = i;
                        }
                        if (p2.X == -1 && sum2 >= minBlack)
                        {
                            p2.X = imageData.GetLength(0) - i - 1;
                        }
                        if (p1.X != -1 && p2.X != -1)
                        {
                            break;
                        }
                    }

                    for (int j = 0; j < (imageData.GetLength(1) / 2); j++)
                    {
                        int sum1 = 0;
                        int sum2 = 0;
                        for (int i = 0; i < imageData.GetLength(0); i++)
                        {
                            sum1 += imageData[i, j] > 0 ? 0 : 1;
                            sum2 += imageData[i, imageData.GetLength(1) - j - 1] > 0 ? 0 : 1;
                        }
                        if (p1.Y == -1 && sum1 >= minBlack)
                        {
                            p1.Y = j;
                        }
                        if (p2.Y == -1 && sum2 >= minBlack)
                        {
                            p2.Y = imageData.GetLength(1) - j - 1;
                        }
                        if (p1.Y != -1 && p2.Y != -1)
                        {
                            break;
                        }
                    }

                    if (p1.X != -1 && p1.Y != -1 && p2.X != -1 && p2.Y != -1)
                    {
                        imageData = Processing.Crop(imageData, new Rectangle(p1.X, p1.Y, p2.X - p1.X + 1, p2.Y - p1.Y + 1));                       

                        int blockWidth = (int)Math.Floor((double)imageData.GetLength(0) / 19);
                        int blockHeight = (int)Math.Floor((double)imageData.GetLength(1) / 4);

                        int newWidth = blockWidth * 19;
                        int newHeight = blockHeight * 4;

                        imageData = Processing.Resize(imageData, newWidth, newHeight);
                        int threshold = (blockWidth * blockHeight) / 2;

                        for (int j = 0; j < 4; j++)
                        {
                            for (int i = 0; i < 19; i++)
                            {
                                double sum = 0;
                                for (int k = 0; k < blockHeight; k++)
                                {
                                    for (int l = 0; l < blockWidth; l++)
                                    {
                                        sum += (int)imageData[(i * blockWidth) + l, (j * blockHeight) + k] > 1 ? 1 : 0;
                                    }
                                }
                                array[i, j] = sum < threshold ? (byte)0 : (byte)1;
                            }
                        }
                    }
                    else
                    {
                        Logger.LogLow("Could not crop barcode.");
                    }
                }
            }
            return ReadFrom2DArray(array);
        }

        /// <summary>
        /// Logs graphical representation of the barcode. Easy to compare with picture.
        /// </summary>
        public static void Print(byte[] bytes)
        {
            Print(CreateArray(bytes));
        }

        /// <summary>
        /// Logs graphical representation of the barcode. Easy to compare with picture.
        /// </summary>
        public static void Print(byte[,] bytes)
        {
            var str = new StringBuilder("\r\n╔════════════════════════════════════════╗\r\n");
            for (int i = 0; i < bytes.GetLength(1); i++)
            {
                str.Append("║ ");
                for (int j = 0; j < bytes.GetLength(0); j++)
                {
                    if (bytes[j, i] == 0)
                    {
                        str.Append("██");                        
                    }
                    else
                    {
                        str.Append("  ");
                    }
                }
                str.Append(" ║\r\n");
            }
            str.Append("╚════════════════════════════════════════╝\r\n");
            Logger.LogHigh(str.ToString());
        }
    }
}
