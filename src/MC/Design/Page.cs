using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using PdfSharp;
using PdfSharp.Drawing;
using MC.Graphical;
using MC.Other;
using MC.PaperTools;
using MC.Design;
using MC.Forms;
using MC.IO;
using MC.Testing;
using TextBox = MC.Design.TextBox;

namespace MC.Design
{
    public class Page : Panel
    {
        /// <summary>
        /// First = 1
        /// </summary>
        [CategoryAttribute("Layout")]
        [DisplayName("Number"), Description("The number of the page (first = 1).")]
        public int Number
        {
            get
            {
                return PageNumber + 1;
            }
        }

        /// <summary>
        /// First = 0
        /// </summary>
        [Browsable(false)]
        public int PageNumber
        {
            get
            {
                int pageNum = 0;
                for (int i = 0; i < ParentTest.Pages.Count; i++)
                {
                    if (this == (Page)ParentTest.Pages[i])
                    {
                        pageNum = i;
                        break;
                    }
                }
                return pageNum;
            }
        }

        [Browsable(false)]
        public bool HasFocus
        {
            get
            {
                return _hasFocus;
            }
            set
            {
                if (_hasFocus != value)
                {
                    if (value == true)
                    {
                        ParentTest.HasFocus = false;
                        foreach (var ctrl in Parent.Controls)
                        {
                            var page = (Page)ctrl;
                            page.HasFocus = false;
                        }
                        foreach (var ctrl in Controls)
                        {
                            var element = (TestElement)ctrl;
                            element.HasFocus = false;
                        }
                        PropertyViewer().Focus(this.ParentTest);
                        var frm = (ctrlDesign)Parent.Parent.Parent.Parent;
                        frm.ScrollTo(this);
                    }
                    _hasFocus = value;
                    this.Refresh();
                }
            }
        }
        private bool _hasFocus;

        public List<TestElement> TestElements = new List<TestElement>();
        public Test ParentTest;

        private Rectangle DesignZone;

        private System.Windows.Forms.ContextMenu pageMenu = new System.Windows.Forms.ContextMenu();        
        private Point mousePositionOnRightClick;        
        private bool lastWasCopy = false; // Difference in namegiving for paste after cut versus copy
        
        public Page(Test parentTest)
        {
            ParentTest = parentTest;
            SetSize();

            pageMenu.MenuItems.Add("Paste");
            pageMenu.MenuItems[0].Shortcut = Shortcut.CtrlV;
            pageMenu.MenuItems[0].Click += new EventHandler(menuPaste);

            pageMenu.MenuItems.Add("-");

            pageMenu.MenuItems.Add("Add");
            pageMenu.MenuItems[2].MenuItems.Add("Item set");
            pageMenu.MenuItems[2].MenuItems[0].Click += new EventHandler(menuAddItemSet);
            pageMenu.MenuItems[2].MenuItems.Add("Text box");
            pageMenu.MenuItems[2].MenuItems[1].Click += new EventHandler(menuAddTextBox);
            pageMenu.MenuItems[2].MenuItems.Add("Image");
            pageMenu.MenuItems[2].MenuItems[2].Click += new EventHandler(menuAddImage);

            pageMenu.MenuItems.Add("Insert page");
            pageMenu.MenuItems[3].Click += new EventHandler(menuInsertPage);

            pageMenu.MenuItems.Add("Remove");
            pageMenu.MenuItems[4].Shortcut = Shortcut.Del;
            pageMenu.MenuItems[4].Click += new EventHandler(menuRemove);

            pageMenu.MenuItems.Add("Move page");
            pageMenu.MenuItems[5].MenuItems.Add("Up");
            pageMenu.MenuItems[5].MenuItems[0].Click += new EventHandler(menuMoveUp);
            pageMenu.MenuItems[5].MenuItems.Add("Down");
            pageMenu.MenuItems[5].MenuItems[1].Click += new EventHandler(menuMoveDown);

            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void menuAddItemSet(object sender, EventArgs e)
        {
            var set = new ItemSet(ParentTest.GetFreeName("Itemset"), this, mousePositionOnRightClick);
            set.NumberOfItems = 10;
            set.NumberOfAlternatives = 5;
            set.NumberOfCheckedAlternativesAllowed = 1;
            set.HasFocus = true;
        }

        private void menuAddTextBox(object sender, EventArgs e)
        {
            var text = new TextBox(ParentTest.GetFreeName("Textbox"), this, mousePositionOnRightClick);
            text.HasFocus = true;
        }

        private void menuAddImage(object sender, EventArgs e)
        {
            var image = new ImageBox(ParentTest.GetFreeName("Imagebox"), this, mousePositionOnRightClick);
            image.HasFocus = true;
        }

        public void CopyElement(bool deleteAfter = false)
        {
            TestElement element = ElementWithFocus();
            if (element != null)
            {
                Clipboard.Clear();
                var xmlSerial = new XmlSerializer(typeof(SerializableTestElement));
                using (var stream = new MemoryStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        if(element.GetType() == typeof(ImageBox))
                        {
                            xmlSerial.Serialize(stream, new SerializableImageBox((ImageBox)element));
                        }
                        else if (element.GetType() == typeof(ItemSet))
                        {
                            xmlSerial.Serialize(stream, new SerializableItemSet((ItemSet)element));
                        }
                        else if (element.GetType() == typeof(TextBox))
                        {
                            xmlSerial.Serialize(stream, new SerializableTextBox((TextBox)element));
                        }
                        stream.Position = 0;
                        Clipboard.SetText(reader.ReadToEnd());
                    }
                }
            }       
            if (deleteAfter)
            {
                RemoveTestElement(element, false);
            }
            lastWasCopy = !deleteAfter;
        }

        public void PasteElement(Point locationToPaste)
        {         
            try
            {
                var xmlSerial = new XmlSerializer(typeof(SerializableTestElement));
                string str = Clipboard.GetText();

                TestElement pastedElement = null;
                using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(str)))
                {
                    if (str.Contains("SerializableImageBox"))
                    {
                        pastedElement = ((SerializableImageBox)xmlSerial.Deserialize(stream)).Deserialize(this, lastWasCopy);
                    }
                    else if (str.Contains("SerializableItemSet"))
                    {
                        pastedElement = ((SerializableItemSet)xmlSerial.Deserialize(stream)).Deserialize(this, lastWasCopy);
                    }
                    else if (str.Contains("SerializableTextBox"))
                    {
                        pastedElement = ((SerializableTextBox)xmlSerial.Deserialize(stream)).Deserialize(this, lastWasCopy);
                    }
                }
                if (pastedElement != null)
                {
                    pastedElement.GridPosition = locationToPaste;
                    pastedElement.HasFocus = true;
                }
           }
            catch (Exception ex)
            {
                Logger.LogLow(ex.Message);
            }
            lastWasCopy = true; // If multiple pastes after one cut then different names are needed
        }

        private void menuPaste(object sender, EventArgs e)
        {
            PasteElement(mousePositionOnRightClick);
        }

        private void menuInsertPage(object sender, EventArgs e)
        {
              var frm = (ctrlDesign)Parent.Parent.Parent.Parent;
              frm.InsertPage(PageNumber);
        }

        private void menuRemove(object sender, EventArgs e)
        {
            RemovePage();            
        }

        private void menuMoveUp(object sender, EventArgs e)
        {
            if (PageNumber > 0)
            {
                var frm = (ctrlDesign)Parent.Parent.Parent.Parent;
                frm.SwitchPages(PageNumber, PageNumber - 1);
            }
        }

        private void menuMoveDown(object sender, EventArgs e)
        {
            if (PageNumber < Program.Test.Pages.Count - 1)
            {
                var frm = (ctrlDesign)Parent.Parent.Parent.Parent;
                frm.SwitchPages(PageNumber, PageNumber + 1);
            }
        }

        public TestElement ElementWithFocus()
        {
            TestElement elFocus = null;
            foreach (Control ctrl in this.Controls)
            {
                TestElement element = (TestElement)ctrl;
                if (element.HasFocus)
                {
                    elFocus = element;
                    break;
                }
            }
            return elFocus;
        }

        // Don't put in OnPaint method (recursion)
        public void SetSize()
        {
            Size = new Size(ParentTest.Paper.Dimensions.Pixel.Width,
                            ParentTest.Paper.Dimensions.Pixel.Height);
        }

        // Click on the page
        protected override void OnMouseDown(MouseEventArgs e)
        {
            HasFocus = true;
            if (e.Button == MouseButtons.Right)
            {
                mousePositionOnRightClick = ParentTest.Paper.ScreenPointToGridPoint(e.Location);
                pageMenu.Show(this, new Point(e.X, e.Y));
            }
        }

        public PropertyViewer PropertyViewer()
        {
            PropertyViewer viewer = null;
            foreach (var v1 in Parent.Parent.Parent.Parent.Controls)
            {
                if (v1.GetType() == typeof(SplitContainer))
                {
                    foreach (var v2 in ((SplitContainer)v1).Panel2.Controls)
                    {
                        if (v2.GetType() == typeof(PropertyViewer))
                        {
                            viewer = (PropertyViewer)v2;
                            break;
                        }
                    }
                }
            }
            return viewer;
        }

        public void Add(TestElement testElement)
        {
            TestElements.Add(testElement);
            Controls.Add(testElement);
            PropertyViewer().Add(testElement);
        }

        public void RemoveTestElement(string testElement)
        {
            foreach (var element in TestElements)
            {
                if (element.Name == testElement)
                {
                    RemoveTestElement(element);
                    break;
                }
            }
        }

        public void RemoveTestElement(TestElement testElement, bool confirmationNecessary = true)
        {
            bool confirmed = false;
            if (confirmationNecessary)
            {
                if (MessageBox.Show("Are you sure you want to delete " + testElement.Name + "?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    confirmed = true;
                }
            }
            else
            {
                confirmed = true;
            }
            if (confirmed)
            {
                TestElements.Remove(testElement);
                Controls.Remove(testElement);
                PropertyViewer().Remove(testElement);
                testElement.Dispose();
            }
        }

        public void RemovePage()
        {
            if (MessageBox.Show("Are you sure you want to delete " + ToString() + "? All content of this page will be deleted.", 
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                // Remove testelements on this page
                while (TestElements.Count > 0)
                {
                    RemoveTestElement(TestElements[0], false);
                }

                int pageToRemove = PageNumber;
                ParentTest.Pages.RemoveAt(pageToRemove);

                var parentPanel = (Panel)this.Parent;
                parentPanel.Controls.Remove(this);

                if (pageToRemove == 0 && parentPanel.Controls.Count > 0)
                {
                    parentPanel.VerticalScroll.Value = 0;
                    parentPanel.Controls[0].Top = 0;
                }
                for (int i = 1; i < parentPanel.Controls.Count; i++)
                {
                    parentPanel.Controls[i].Top = parentPanel.Controls[i - 1].Bottom;
                }

                this.Dispose();
            }
        }

        public Point GetFreePoint()
        {
            var p = new Point();
            var diagLocList = new List<int>(); 
            foreach (var element in TestElements)
            {
                if (element.GridPosition.X == element.GridPosition.Y)
                {
                    diagLocList.Add(element.GridPosition.X);
                }
            }
            if (diagLocList.Count > 0)
            {
                diagLocList.Sort();
                for (int i = 0; i < diagLocList.Count; i++)
                {
                    if (diagLocList[i] > i)
                    {
                        p = new Point(diagLocList[i] - 1, diagLocList[i] - 1);
                        break;
                    }
                }
                if (p.X == 0)
                {
                    p = new Point(diagLocList[diagLocList.Count - 1] + 1, diagLocList[diagLocList.Count - 1] + 1);
                }
            }
            return p;
        }
        
        public bool[,] ItemAlternativeOnLocation()
        {
            bool[,] thereIsOne = new bool[Program.Test.Paper.Blocks.X, Program.Test.Paper.Blocks.Y];
            foreach (var ctrl in Controls)
            {
                if (ctrl.GetType() == typeof(ItemSet))
                {
                    var itemSet = (ItemSet)ctrl;
                    for (int i = 0; i < itemSet.NumberOfItems; i++)
                    {
                        for (int a = 0; a < itemSet.NumberOfAlternatives; a++)
                        {
                            var loc = itemSet.ItemAltPointGrid(i, a);
                            thereIsOne[loc.X, loc.Y] = true;
                        }
                    }
                }
            }
            return thereIsOne;
        }
       
        public override string ToString()
        {
            return "Page " + Convert.ToString(Number);
        }

        public byte[] Hash()
        {
            byte[] bytes = new byte[8];
            bytes[0] = (byte)PageNumber; // Page
            int h = Program.Test.Paper.Blocks.X * Program.Test.Paper.Blocks.Y;
            if (Program.Test.Paper.Orientation == PaperOrientation.Landscape)
            {
                h += 3;
            }

            foreach (var ctrl in this.Controls)
            {
                if (ctrl.GetType() == typeof(ItemSet))
                {
                    // Not perfect, does not take into account if length of item names in second,... column changes
                    // But don't change, because that would make older tests unreadable
                    var itemSet = (ItemSet)ctrl;
                    h += 
                        itemSet.ItemAltPointGrid(0, 0).X * 7 +
                        itemSet.ItemAltPointGrid(0, 0).Y * 19 +
                        itemSet.NumberOfAlternatives * 37 +
                        itemSet.NumberOfItems * 53 +
                        itemSet.Columns * 71;
                }
            }
            
            // Not % or / 256, because leave 255 for error
            bytes[1] = (byte)(h % 255); // Hash itemsets
            bytes[2] = (byte)(h / 255); // Hash itemsets

            return bytes;          
        }
        
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
        {
            Rectangle saveZone = ParentTest.Paper.SaveZone;
            DesignZone = ParentTest.Paper.DesignZone;

            if (Size != ParentTest.Paper.Dimensions.Pixel)
            {
                SetSize();
            }
            if (BackgroundImage == null)
            {
                BackgroundImage = (Image)(new Bitmap(Size.Width, Size.Height));
            }
            else if (BackgroundImage.Width != Size.Width || BackgroundImage.Height != Size.Height)
            {               
                BackgroundImage = (Image)(new Bitmap(Size.Width, Size.Height));
            }

            using (Graphics g = Graphics.FromImage(BackgroundImage))
            {
                g.FillRectangle(new SolidBrush(Color.FromKnownColor(KnownColor.ControlDark)), new Rectangle(0, 0, Width, Height));
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(12, 12, Width - 24, Height - 24));
                using (Pen p = new Pen(Color.LightGray))
                {
                    for (int i = saveZone.X; i <= saveZone.Right; i += Settings.blockWidth)
                    {
                        g.DrawLine(p, i, saveZone.Y, i, saveZone.Bottom - 1);
                    }
                    for (int j = saveZone.Y; j <= saveZone.Bottom; j += Settings.blockHeight)
                    {
                        g.DrawLine(p, saveZone.X, j, saveZone.Right - 1, j);
                    }
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;
                StringFormat formatDec = new StringFormat();
                formatDec.Alignment = StringAlignment.Near;
                formatDec.LineAlignment = StringAlignment.Center;
                using (Brush b = new SolidBrush(Color.Black))
                {
                    int pH = 0;
                    int pV = 0;
                    for (int i = 0; i < ParentTest.Paper.Blocks.X - 1; i += 7)
                    {
                        g.FillEllipse(b, new Rectangle(DesignZone.Left + ((Settings.blockWidth - Settings.blockHeight) / 2) + (i * Settings.blockWidth),
                            DesignZone.Bottom - 1 - Settings.blockHeight, Settings.blockHeight, Settings.blockHeight));
                        pH++;
                    }
                    for (int i = 0; i < ParentTest.Paper.Blocks.Y - 1; i += 7)
                    {
                        g.FillEllipse(b, new Rectangle(DesignZone.Right - 1 - Settings.blockHeight,
                            DesignZone.Top + (i * Settings.blockHeight), Settings.blockHeight, Settings.blockHeight));
                        pV++;
                    }
                    g.FillEllipse(b, new Rectangle(DesignZone.Right - 1 - Settings.blockHeight,
                        DesignZone.Bottom - 1 - Settings.blockHeight, Settings.blockHeight, Settings.blockHeight));
                    pH++;
                    pV++;
                    Program.Test.Paper.CalibrationCircles = new Point(pH, pV);

                    StringFormat pageNumFormat = new StringFormat();
                    pageNumFormat.FormatFlags = StringFormatFlags.DirectionVertical;
                    pageNumFormat.LineAlignment = StringAlignment.Far;

                }
                Image dummyBarCode = BarCode.CreateImage(new byte[8] { 69, 120, 112, 105, 83, 111, 102, 116 }, 6);
                g.DrawImage(dummyBarCode, DesignZone.Left + (4 * Settings.blockWidth) - (dummyBarCode.Width / 2), DesignZone.Bottom - dummyBarCode.Height);
            }
            foreach (TestElement testElement in TestElements)
            {
                testElement.ReDraw();
            }
            if (HasFocus)
            {
                using (Pen p = new Pen(Color.Blue, 3))
                {
                    pe.Graphics.DrawRectangle(p, new Rectangle(12, 12, Width - 25, Height - 25));
                }
                using (Pen p = new Pen(Color.DarkBlue, 1))
                {
                    pe.Graphics.DrawRectangle(p, new Rectangle(12, 12, Width - 25, Height - 25));
                }
            }
        }

        public void PdfDraw(XGraphics g)
        {
            XStringFormat formatDec = new XStringFormat();
            formatDec.Alignment = XStringAlignment.Near;
            formatDec.LineAlignment = XLineAlignment.Center;
            XSolidBrush b = new XSolidBrush(Color.Black);

            for (int i = 0; i < ParentTest.Paper.Blocks.X - 1; i += 7)
            {
                g.DrawEllipse(b, new Rectangle(DesignZone.Left + ((Settings.blockWidth - Settings.blockHeight) / 2) + (i * Settings.blockWidth),
                    DesignZone.Bottom - 1 - Settings.blockHeight, Settings.blockHeight, Settings.blockHeight));
            }
            for (int i = 0; i < ParentTest.Paper.Blocks.Y - 1; i += 7)
            {
                g.DrawEllipse(b, new Rectangle(DesignZone.Right - 1 - Settings.blockHeight, DesignZone.Top + (i * Settings.blockHeight),
                    Settings.blockHeight, Settings.blockHeight));
            }
            g.DrawEllipse(b, new Rectangle(DesignZone.Right - 1 - Settings.blockHeight, DesignZone.Bottom - 1 - Settings.blockHeight,
                Settings.blockHeight, Settings.blockHeight));

            XGraphicsState gs = g.Save();
            g.RotateAtTransform(90, new XPoint(DesignZone.Right - 1 - (int)(Settings.blockHeight / 2), DesignZone.Top + (2 * Settings.blockHeight)));
            XStringFormat pageNumFormat = new XStringFormat();
            pageNumFormat.LineAlignment = XLineAlignment.Far;
        
            g.Restore(gs);

            var barcode = BarCode.CreateArray(Hash());
            Point start = new Point(DesignZone.Left + (4 * Settings.blockWidth) - 57, DesignZone.Bottom - 25);

            XBrush bGray = new XSolidBrush(XColor.FromArgb(Program.Test.BarCodeGray, Program.Test.BarCodeGray, Program.Test.BarCodeGray));
            for (int i = 0; i < 19; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (barcode[i, j] == 0)
                    {
                        g.DrawRectangle(bGray, start.X + (i * 6), start.Y + (j * 6), 6, 6);
                    }
                }
            }

            foreach (TestElement testElement in TestElements)
            {
                testElement.PdfDraw(g);
            }

        }

    }
}
