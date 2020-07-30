using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Drawing;
using MC.Other;

namespace MC.PaperTools
{
    [Serializable]
    public class PaperDimensions
    {
        [RefreshProperties(RefreshProperties.All)]
        [CategoryAttribute("Size")]
        [DisplayName("Size (mm)"), Description("The size of the paper in mm.")]
        public SizeF Mm
        {
            get
            {
                return new SizeF((float)Math.Round(_mm.Width, 2), (float)Math.Round(_mm.Height, 2));
            }
            set
            {
                _mm = value;
                _inch = new SizeF(value.Width / 25.4f, value.Height / 25.4f);
                _pixel = new Size((int)(value.Width * 2.83465f), (int)(value.Height * 2.83465f));
            }
        }
        SizeF _mm;

        [RefreshProperties(RefreshProperties.All)]
        [CategoryAttribute("Size"), XmlIgnore]
        [DisplayName("Size (inch)"), Description("The size of the paper in inches.")]
        public SizeF Inch
        {
            get
            {
                return new SizeF((float)Math.Round(_inch.Width, 2), (float)Math.Round(_inch.Height, 2));
            }
            set
            {
                _inch = value;
                _mm = new SizeF(value.Width * 25.4f, value.Height * 25.4f);
                _pixel = new Size((int)(value.Width * 72), (int)(value.Height * 72));
            }
        }
        SizeF _inch;

        [Browsable(false)]
        public Size Pixel
        {
            get
            {
                return _pixel;
            }
        }
        Size _pixel;

       // Parameterless constructor for serialization
        public PaperDimensions()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width">In millimeter</param>
        /// <param name="height">In millimeter</param>
        public PaperDimensions(Single width, Single height)
        {
            Mm = new SizeF(width, height);
        }

        public void SetToPortrait()
        {
            Single s1 = Mm.Width;
            Single s2 = Mm.Height;
            Mm = new SizeF(Math.Min(s1, s2), Math.Max(s1, s2));
        }

        public void SetToLandscape()
        {
            Single s1 = Mm.Width;
            Single s2 = Mm.Height;
            Mm = new SizeF(Math.Max(s1, s2), Math.Min(s1, s2));
        }

        public override string ToString()
        {
            return Mm.Width + "x" + Mm.Height + " millimeter   " + Inch.Width + "x" + Inch.Height + " inch   " + Pixel.Width + "x" + Pixel.Height + " pixel";
        }
    }
}
