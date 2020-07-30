using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using PdfSharp;
using PdfSharp.Drawing;

namespace MC.Other
{
    public static class StringTools
    {
        public static char[] ForbiddenChars = CreateForbiddenChars();
        public static string PythonReserved = @"\b(and|as|assert|break|class|continue|def|del|elif|else|except|exec|finally|for|from|global|if|import|in|is|lambda|not|or|pass|print|raise|return|try|while|with|yield)\b";
        public static List<string> PythonReservedList = PythonReserved.Split(new string[] { @"\b(", "|", @")\b" }, StringSplitOptions.None).ToList();

        private static int[] Prime = new int[26] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101 };

        public static List<string> Split(string str, int maxWidth, Graphics g, Font font)
        {
            var stringList = new List<string>();
            if (str == "")
            {
                return stringList;
            }
            int totalWidth = pixelWidth(str, g, font);

            var strArr = str.Split(' ');

            string outstr = strArr[0];           
            for (int i = 0; i < strArr.Count(); i++)
            {                
                if (i == strArr.Count() - 1)
                {
                    stringList.Add(outstr);
                }
                else
                {
                    if (pixelWidth(outstr + " " + strArr[i + 1], g, font) > maxWidth)
                    {
                        stringList.Add(outstr);
                        outstr = strArr[i + 1];
                    }
                    else
                    {
                        outstr += " " + strArr[i + 1];
                    }
                }
            }
            return stringList;
        }

        public static List<string> Split(string str, int maxWidth, XGraphics g, XFont font)
        {
            var stringList = new List<string>();
            if (str == "")
            {
                return stringList;
            }
            int totalWidth = pixelWidth(str, g, font);

            var strArr = str.Split(' ');

            string outstr = strArr[0];
            for (int i = 0; i < strArr.Count(); i++)
            {
                if (i == strArr.Count() - 1)
                {
                    stringList.Add(outstr);
                }
                else
                {
                    if (pixelWidth(outstr + " " + strArr[i + 1], g, font) > maxWidth)
                    {
                        stringList.Add(outstr);
                        outstr = strArr[i + 1];
                    }
                    else
                    {
                        outstr += " " + strArr[i + 1];
                    }
                }
            }
            return stringList;
        }

        public static bool IsNumeric(string str)
        {
            double n;
            return double.TryParse(str, out n);
        }

        private static char[] CreateForbiddenChars()
        {
            var charList = new List<char>();
            for(int i = 0; i < 48; i++)
            {
                charList.Add((char)i);
            }
            for(int i = 58; i < 65; i++)
            {
                charList.Add((char)i);
            }
            for(int i = 91; i < 95; i++)
            {
                charList.Add((char)i);
            }
            charList.Add((char)96);            
            for(int i = 123; i < 256; i++)
            {
                charList.Add((char)i);
            }
            return charList.ToArray();
        }

        public static string Limit(string str, int width, Graphics g, Font font)
        {
            width = Math.Max(1, width);
            while (pixelWidth(str, g, font) > width)
            {
                str = str.Remove(str.Length - 1);
            }
            return str;
        }

        public static string Limit(string str, int width, XGraphics g, XFont font)
        {
            width = Math.Max(1, width);
            while (pixelWidth(str, g, font) > width)
            {
                str = str.Remove(str.Length - 1);
            }
            return str;
        }

        /// <summary>
        /// Gets the longest string in the list
        /// </summary>
        /// <returns>Number of pixels of that string</returns>
        public static int Longest(List<string> str, Graphics g, Font font)
        {
            int max = 0;
            for(int i = 0; i < str.Count; i++)
            {
                max = Math.Max(max, pixelWidth(str[i], g, font));
            }
            return max;
        }

        /// <summary>
        /// Gets the longest string in the list
        /// </summary>
        /// <returns>Number of pixels of that string</returns>
        public static int Longest(List<string> str, XGraphics g, XFont font)
        {
            int max = 0;
            for(int i = 0; i < str.Count; i++)
            {
                max = Math.Max(max, pixelWidth(str[i], g, font));
            }
            return max;
        }

        private static int pixelWidth(string str, Graphics g, Font font)
        {
            return (int)g.MeasureString(str,font).Width;
        }

        private static int pixelWidth(string str, XGraphics g, XFont font)
        {
            return (int)g.MeasureString(str, font).Width;
        }

        public static string Alphabet(int i)
        {
            if (i <= 0 || i > 702)
            {
                return "err";
            }
            else
            {
                if (i <= 26)
                {
                    return Convert.ToString((char)(i + 64));
                }
                else
                {
                    int rest = i % 26;
                    if (rest == 0)
                    {
                        rest = 26;
                    }
                    return Convert.ToString((char)(((i - rest) / 26) + 64)) + Convert.ToString((char)(rest + 64));
                }
            }
        }

        public static string GetBase(string str)
        {
            var split = str.Split('_');
            string strOut = "";
            if (IsNumeric(split[split.Count() - 1]))
            {
                strOut = split[0];
                if (split.Count() > 1)
                {
                    for (int i = 1; i < split.Count() - 1; i++)
                    {
                        strOut += "_" + split[i];
                    }
                }
            }
            else
            {
                strOut = str;
            }
            return strOut;
        }

        public static byte[] StringHash(string str, int bytes)
        {
            byte[] b = new byte[bytes];
            var arr = str.ToUpper().ToCharArray();
            double product = 1;
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                var byt = (byte)arr[i];
                if (byt >= 65 && byt <= 90)
                {
                    product *= i + Prime[byt - 65];
                }
            }
            for (int i = 0; i < bytes; i++)
            {
                // Not % or / 256, because leave 255 for error
                var remains = product % 255;
                b[i] = (byte)remains;
                product /= 255;
            }
            return b;
        }

        public static string CreateString(int[] array)
        {
            StringBuilder str = new StringBuilder();
            int last = 0;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                str.Append((char)(array[i] + last));
                last += array[i];
            }
            return str.ToString();
        }

    }
}
