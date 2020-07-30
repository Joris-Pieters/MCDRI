using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Serialization;
using MC.Other;
using MC.Testing;
using MC.Design;


namespace MC.PaperTools
{
    [Serializable, TypeConverter(typeof(ExpandableObjectConverter))]
    public class Paper
    {
        [RefreshProperties(RefreshProperties.All)]
        [CategoryAttribute("Paper")]
        [DisplayName("Template"), Description("Set the paper template.")]
        public PaperTemplate Template
        {
            get
            {
                return _template;
            }
            set
            {
                _template = value;
                Program.UserSettings.defaultPaperTemplate = value;                
                SetSize();
            }
        }
        PaperTemplate _template;

        [RefreshProperties(RefreshProperties.All)]
        [CategoryAttribute("Paper")]
        [DisplayName("Orientation"), Description("Set the orientation of the paper.")]
        public PaperOrientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
                Program.UserSettings.defaultPaperOrientation = value;
                SetSize();
            }
        }
        PaperOrientation _orientation;

        [RefreshProperties(RefreshProperties.All)]
        [CategoryAttribute("Paper")]
        [DisplayName("Border (mm)"), Description("Set the size of the borders (in mm).\r\nKeep in mind the limits of the printer you will be using.")]
        public SizeF BorderMm
        {
            get
            {
                return new SizeF((float)Math.Round(_borderMm.Width, 2), (float)Math.Round(_borderMm.Height, 2));
            }
            set
            {
                if (Dimensions != null)
                {                  
                    value = new SizeF((float)Math.Min(Math.Max(value.Width, 6.35), 76.2), (float)Math.Min(Math.Max(value.Height, 6.35), 76.2));
                }
                _borderMm = value;
                _borderInch = new SizeF(value.Width / 25.4f, value.Height / 25.4f);
                _borderPixel = new Size((int)(value.Width * 2.83465f), (int)(value.Height * 2.83465f));
                SetSize();
            }
        }
        SizeF _borderMm;

        [RefreshProperties(RefreshProperties.All)]
        [CategoryAttribute("Paper"), XmlIgnore]
        [DisplayName("Border (inch)"), Description("Set the size of the borders (in inches).\r\nKeep in mind the limits of the printer you will be using.")]
        public SizeF BorderInch
        {
            get
            {
                return new SizeF((float)Math.Round(_borderInch.Width, 2), (float)Math.Round(_borderInch.Height, 2));
            }
            set
            {
                if (Dimensions != null)
                {
                    value = new SizeF((float)Math.Min(Math.Max(value.Width, .25), 3), (float)Math.Min(Math.Max(value.Height, .25), 3));
                }
                _borderInch = value;
                _borderMm = new SizeF(value.Width * 25.4f, value.Height * 25.4f);                
                _borderPixel = new Size((int)(value.Width * 72), (int)(value.Height * 72));
                Program.UserSettings.defaultPaperBorder = value;                
                SetSize();
            }
        }
        SizeF _borderInch;

        [RefreshProperties(RefreshProperties.All)]
        [Browsable(false)]
        public Size BorderPixel
        {
            get
            {
                return _borderPixel;
            }
        }
        Size _borderPixel;

        [RefreshProperties(RefreshProperties.All)]
        [Browsable(false)]
        public Size SafePixel
        {
            get
            {
                return new Size(Dimensions.Pixel.Width - (2 * BorderPixel.Width), Dimensions.Pixel.Height - (2 * BorderPixel.Height));
            }
        }

        [Browsable(false)]
        public Rectangle DesignZone
        {
            get
            {
                int calibrateMarginX = Settings.blockWidth + (int)(.5 * Settings.blockHeight);
                int calibrateMarginY = 2 * Settings.blockHeight;

                var _designZone = new Rectangle(0, 0,
                    (int)(Math.Floor((double)(SafePixel.Width - calibrateMarginX) / Settings.blockWidth) * Settings.blockWidth) + calibrateMarginX,
                    (int)(Math.Floor((double)(SafePixel.Height - calibrateMarginY) / Settings.blockHeight) * Settings.blockHeight) + calibrateMarginY);
                _designZone.X = (int)((Dimensions.Pixel.Width - _designZone.Width) / 2);
                _designZone.Y = (int)((Dimensions.Pixel.Height - _designZone.Height) / 2);
                return _designZone;
            }
        }

        [Browsable(false)]
        public Rectangle SaveZone
        {
            get
            {
                var des = DesignZone;
                return new Rectangle(des.X, des.Y, des.Width - (Settings.blockWidth + (int)(.5 * Settings.blockHeight)), des.Height - (2 * Settings.blockHeight));
            }
        }

        /// <summary>
        /// Pixels from left top where first block starts
        /// </summary>
        [Browsable(false)]
        public Point BlockStart
        {
            get
            {
                return DesignZone.Location;
            }
        }

        /// <summary>
        /// Size of a block in pixels
        /// </summary>
        [Browsable(false)]
        public Size BlockSize
        {
            get
            {
                return new Size(Settings.blockWidth, Settings.blockHeight);
            }
        }

        // Amount of blocks on a page horizontally and vertically
        [CategoryAttribute("Paper"), XmlIgnore]
        [DisplayName("Blocks"), Description("The number of 'blocks' available per page (horizontally and vertically).")]
        public Point Blocks
        {
            get
            {
                return new Point((int)(SaveZone.Width / Settings.blockWidth), (int)(SaveZone.Height / Settings.blockHeight));
            }
        }

        [Browsable(false), XmlIgnore]
        public Point CalibrationCircles { get;  set;}

        [NonSerialized, XmlIgnore] // Otherwise circular reference
        public Test ParentTest;

        public PaperDimensions Dimensions;

        // Parameterless constructor for serialization
        public Paper()
        {
        }
        
        public Paper(Test test)
        {
            ParentTest = test;
            Template = Program.UserSettings.defaultPaperTemplate;
            Orientation = Program.UserSettings.defaultPaperOrientation;
            BorderInch = Program.UserSettings.defaultPaperBorder;
            SetSize();
        }

        public Paper(Test test, PaperTemplate template)
        {
            ParentTest = test;
            Template = template;
            Orientation = Program.UserSettings.defaultPaperOrientation;
            BorderInch = Program.UserSettings.defaultPaperBorder;
            SetSize();    
        }

        public Paper(Test test, PaperTemplate template, PaperOrientation orientation)
        {
            ParentTest = test;
            Template = template;
            Orientation = orientation;
            BorderInch = Program.UserSettings.defaultPaperBorder;
            SetSize();
        }

        public Paper(Test test, PaperTemplate template, PaperOrientation orientation, SizeF border)
        {
            ParentTest = test;
            Template = template;
            Orientation = orientation;
            BorderInch = border;
            SetSize();
        }

        private void SetSize()
        {
            switch (Template)
            {
                case (PaperTemplate.Letter):
                    Dimensions = PaperCollection.Letter;
                    break;
                case (PaperTemplate.GovernmentLetter):
                    Dimensions = PaperCollection.GovernmentLetter;
                    break;
                case (PaperTemplate.Legal):
                    Dimensions = PaperCollection.Legal;
                    break;               
                case (PaperTemplate.Ledger):
                    Dimensions = PaperCollection.Ledger;
                    break;
                case (PaperTemplate.A0):
                    Dimensions = PaperCollection.A0;
                    break;
                case (PaperTemplate.A1):
                    Dimensions = PaperCollection.A1;
                    break;
                case (PaperTemplate.A2):
                    Dimensions = PaperCollection.A2;
                    break;
                case (PaperTemplate.A3):
                    Dimensions = PaperCollection.A3;
                    break;
                case (PaperTemplate.A4):
                    Dimensions = PaperCollection.A4;
                    break;
                case (PaperTemplate.A5):
                    Dimensions = PaperCollection.A5;
                    break;
            }

            switch (Orientation)
            {
                case (PaperOrientation.Portrait):
                    Dimensions.SetToPortrait();
                    break;
                case (PaperOrientation.Landscape):
                    Dimensions.SetToLandscape();
                    break;
            }

            // Limiting the bordersize based on dimensions of the paper
            float roudingTolerance = .01f;
            var newBorderMm = new SizeF(
                (float)((Dimensions.Pixel.Width - (9 * BlockSize.Width)) * .17639),
                (float)((Dimensions.Pixel.Height - (9 * BlockSize.Height)) * .17639));
            if (_borderMm.Width > newBorderMm.Width + roudingTolerance || _borderMm.Height > newBorderMm.Height + roudingTolerance)
            {
                BorderMm = new SizeF(Math.Min(_borderMm.Width, newBorderMm.Width), Math.Min(_borderMm.Height, newBorderMm.Height));
            }

            // During loading parentTest is not yet set
            if (ParentTest != null)
            {
                if (ParentTest.Pages.Count > 0)
                {
                    for (int i = 0; i < ParentTest.Pages.Count; i++)
                    {
                        foreach (var element in ParentTest.Pages[i].TestElements) // Move element within new range before switching paper size/orientation
                        {
                            element.SetWithinNewRange(Blocks);
                            element.UpdateGrid();
                            element.ReDraw();
                        }
                        ParentTest.Pages[i].SetSize(); // Give pagecontrols right size
                        if (i > 0)
                        {
                            ParentTest.Pages[i].Top = ParentTest.Pages[i - 1].Bottom; // Fit page top to previous page bottom
                        }
                        ParentTest.Pages[i].Refresh();
                    }
                }
            }                       
        }

        public Point ScreenPointToGridPoint(Point screenPoint)
        {
            return new Point((screenPoint.X - BorderPixel.Width) / Settings.blockWidth, (screenPoint.Y - BorderPixel.Height) / Settings.blockHeight);
        }

        public override string ToString()
        {
            return "[" + Template.ToString() + ", " + Orientation.ToString() + "]";
        }

    }
}
