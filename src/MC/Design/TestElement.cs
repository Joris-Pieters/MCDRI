using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;
using PdfSharp;
using PdfSharp.Drawing;
using MC.Testing;
using MC.PaperTools;
using MC.Forms;

namespace MC.Design
{
    public abstract class TestElement : Control
    {

        [CategoryAttribute("General"), Browsable(true)]
        [DisplayName("Name"), Description("")]
        public new string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value == null)
                {
                    value = "unknown";
                }
                bool nameAlreadyExists = false;
                foreach (var existingName in ParentPage.ParentTest.ExistingNames())
                {                    
                    if (existingName == value)
                    {
                        nameAlreadyExists = true;
                        break;
                    }
                }
                if (nameAlreadyExists)
                {
                    MessageBox.Show("This name is already in use. Choose a different name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9_]*$"))
                    {
                        _name = value;
                    }
                    else
                    {
                        MessageBox.Show("No spaces or special characters (except underscore) allowed. Choose a different name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _name = ParentPage.ParentTest.GetFreeName();
                    }
                }
            }
        }
        private string _name;

        [CategoryAttribute("General")]
        [DisplayName("Border"), Description("Choose if a border has to be drawn.")]
        public bool Border { get; set; }

        [CategoryAttribute("Layout")]
        [DisplayName("Page"), Description("")]
        public int OnPage
        {
            get
            {
                return ParentPage.Number;
            }
        }

        [CategoryAttribute("Layout")]
        [DisplayName("Grid position"), Description("Position in the grid (starting from the left top corner).")]
        public Point GridPosition
        {
            get
            {
                return _gridPosition;
            }
            set
            {
                if (value.Y < 1 - _gridSize.Height && ParentPage.PageNumber > 0) // Move up a page
                {
                    MoveToPage(ParentPage.PageNumber - 1);
                    _gridPosition = new Point(Math.Max(value.X, 1 - _gridSize.Width), blockCount.Y - 1);
                    _gridPosition = new Point(Math.Min(_gridPosition.X, blockCount.X - 1), _gridPosition.Y);
                }
                else if (value.Y > blockCount.Y - 1 && ParentPage.PageNumber < ParentPage.ParentTest.Pages.Count - 1) // Move down a page
                {
                    MoveToPage(ParentPage.PageNumber + 1);
                    _gridPosition = new Point(Math.Max(value.X, 1 - _gridSize.Width), 0);
                    _gridPosition = new Point(Math.Min(_gridPosition.X, blockCount.X - 1), _gridPosition.Y);
                }
                else // Move on same page
                {
                    _gridPosition = new Point(Math.Max(value.X, 1 - _gridSize.Width), Math.Max(value.Y, 1 - _gridSize.Height));
                    _gridPosition = new Point(Math.Min(_gridPosition.X, blockCount.X - 1), Math.Min(_gridPosition.Y, blockCount.Y - 1));                   
                }

                Left = blockStart.X + (_gridPosition.X * blockSize.Width);
                Top = blockStart.Y + (_gridPosition.Y * blockSize.Height);
            }
        }
        private Point _gridPosition;

        [CategoryAttribute("Layout")]
        [DisplayName("Size"), Description("")]
        public Size GridSize
        {
            get
            {                
                return _gridSize;
            }
            set
            {
                // No maximum size, because that would screw up some itemsets (size is based on content)
                _gridSize = new Size(Math.Max(value.Width, 1), Math.Max(value.Height, 1));
                Width = (_gridSize.Width * blockSize.Width) + 1;
                Height = (_gridSize.Height * blockSize.Height) + 1;
            }
        }
        private Size _gridSize;

        [Browsable(false), XmlIgnore]
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
                        ParentPage.ParentTest.HasFocus = false;
                        foreach (var ctrl in ParentPage.Parent.Controls)
                        {
                            var page = (Page)ctrl;                            
                            page.HasFocus = false;
                            foreach (var ctrl2 in page.Controls)
                            {
                                var element = (TestElement)ctrl2;
                                if (this != element)
                                {
                                    element.HasFocus = false;
                                }
                            }
                        }
                        ParentPage.PropertyViewer().Focus(this.Name);
                        this.BringToFront();
                    }
                    _hasFocus = value;
                    this.Refresh();
                }
            }
        }
        private bool _hasFocus;

        public Page ParentPage;

        internal Size blockSize;
        internal Point blockCount;
        internal Point blockStart;
    
        private Point mouseClicked;
        private Point mouseDelta;
        private bool isDragging;

        internal System.Windows.Forms.ContextMenu testElementMenu = new System.Windows.Forms.ContextMenu();

        public TestElement(string name, Page parentPage, Point gridPosition = new Point(), Size gridSize = new Size(), bool register = true)
        {            
            ParentPage = parentPage;
            Name = name;

            Border = true;

            GridSize = gridSize;
            UpdateGrid();

            if (gridPosition.X > 0 || gridPosition.Y > 0)
            {
                GridPosition = gridPosition;
            }
            else
            {
                GridPosition = ParentPage.GetFreePoint();
            }

            testElementMenu.MenuItems.Add("Cut");
            testElementMenu.MenuItems[0].Shortcut = Shortcut.CtrlX;
            testElementMenu.MenuItems[0].Click += new EventHandler(menuElementCut);
            testElementMenu.MenuItems.Add("Copy");
            testElementMenu.MenuItems[1].Shortcut = Shortcut.CtrlC;
            testElementMenu.MenuItems[1].Click += new EventHandler(menuElementCopy);

            testElementMenu.MenuItems.Add("-");

            testElementMenu.MenuItems.Add("Send to background");
            testElementMenu.MenuItems[3].Click += new EventHandler(menuBackground);

            testElementMenu.MenuItems.Add("Remove");
            testElementMenu.MenuItems[4].Shortcut = Shortcut.Del;
            testElementMenu.MenuItems[4].Click += new EventHandler(menuRemove);                       

            if (register)
            {
                ParentPage.Add(this);
            }
        }

        private void menuElementCut(object sender, EventArgs e)
        {
            ParentPage.CopyElement(true);
        }
                
        private void menuElementCopy(object sender, EventArgs e)
        {
            ParentPage.CopyElement();
        }
        
        public void UpdateGrid()
        {
            blockSize = ParentPage.ParentTest.Paper.BlockSize;
            blockCount = ParentPage.ParentTest.Paper.Blocks;
            blockStart = ParentPage.ParentTest.Paper.BlockStart;
        }

        public void MoveToPage(int pageNumber)
        {
            // Has to be removed from parentpage, and added to another, and not just switch parentpage
            // Otherwise the GUI will not update properly

            int oldPage = ParentPage.PageNumber;
            ParentPage.CopyElement(true);

            if (pageNumber > oldPage)
            {
                Program.Test.Pages[pageNumber].PasteElement(new Point(GridPosition.X, 0));
            }
            else
            {
                Program.Test.Pages[pageNumber].PasteElement(new Point(GridPosition.X, Program.Test.Paper.Blocks.Y - 1));
            }
        }

        /// <summary>
        /// Before changing paper size/orientation, move elements within this new range. Otherwise will jump to other pages.
        /// </summary>
        public void SetWithinNewRange(Point newBlockCount)
        {
            _gridPosition = new Point(Math.Min(_gridPosition.X, newBlockCount.X - 1), Math.Min(_gridPosition.Y, newBlockCount.Y - 1));
        }        

        // Click on the testelement
        protected override void OnMouseDown(MouseEventArgs e)
        {
            HasFocus = true;
            this.BringToFront();
            if (e.Button == MouseButtons.Left)
            {
                mouseClicked = new Point(e.X, e.Y);
                Program.UndoManager.Update();
                isDragging = true;                
            }
            else if (e.Button == MouseButtons.Right)
            {
                testElementMenu.Show(this, new Point(e.X, e.Y));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isDragging)
            {
                if (Math.Abs(e.X - mouseDelta.X) > blockSize.Width / 2 || Math.Abs(e.Y - mouseDelta.Y) > blockSize.Height / 2)
                {
                    Point newPosition = new Point((this.Left + e.X - mouseClicked.X - blockStart.X) / blockSize.Width, 
                                                  (this.Top + e.Y - mouseClicked.Y - blockStart.Y) / blockSize.Height);
                    if (GridPosition != newPosition)
                    {                        
                        GridPosition = newPosition;
                        ParentPage.PropertyViewer().Focus(this);
                        mouseDelta = new Point(e.X, e.Y);                        
                    }
                }
            }            
        }

        public void ReDraw()
        {
            GridPosition = GridPosition;
        }

        private void menuBackground(object sender, EventArgs e)
        {
            this.SendToBack();
        }

        private void menuRemove(object sender, EventArgs e)
        {
            ParentPage.RemoveTestElement(this);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
        {
            if (HasFocus)
            {
                using (Pen p = new Pen(Color.Blue, 3))
                {
                    pe.Graphics.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
                }
                using (Pen p = new Pen(Color.DarkBlue, 1))
                {
                    pe.Graphics.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
                }
            }
            else if (Border)
            {
                pe.Graphics.DrawRectangle(new Pen(Color.FromArgb(128, 128, 128)), new Rectangle(0, 0, Width - 1, Height - 1));
            }
            else // No border, but drawn in GUI for clarity
            {
                pe.Graphics.DrawRectangle(new Pen(Color.LightGray), new Rectangle(0, 0, Width - 1, Height - 1));
            }
        }

        public virtual void PdfDraw(XGraphics g)
        {
            if (Border)
            {
                g.DrawRectangle(new Pen(Color.FromArgb(128, 128, 128)), new Rectangle(Left, Top, Width - 1, Height - 1));
            }
        }
   
        public override string ToString()
        {
            return Name;
        }

        [Browsable(false)]
        public new ControlBindingsCollection DataBindings { get; set; }

        [Browsable(false)]
        public new string AccessibleDescription { get; set; }

        [Browsable(false)]
        public new string AccessibleName { get; set; }

        [Browsable(false)]
        public new AccessibleRole AccessibleRole { get; set; }

        [Browsable(false)]
        public new bool AllowDrop { get; set; }

        [Browsable(false)]
        public new AnchorStyles Anchor { get; set; }

        [Browsable(false)]
        public new Color BackColor { get; set; }

        [Browsable(false)]
        public new Image BackgroundImage { get; set; }

        [Browsable(false)]
        public new ImageLayout BackgroundImageLayout { get; set; }

        [Browsable(false)]
        public new bool CausesValidation { get; set; }

        [Browsable(false)]
        public new ContextMenuStrip ContextMenuStrip { get; set; }

        [Browsable(false)]
        public new Cursor Cursor { get; set; }

        [Browsable(false)]
        public new DockStyle Dock { get; set; }

        [Browsable(false)]
        public new bool Enabled { get; set; }

        [Browsable(false)]
        public new Font Font { get; set; }

        [Browsable(false)]
        public new Color ForeColor { get; set; }

        [Browsable(false)]
        public new ImeMode ImeMode { get; set; }

        [Browsable(false)]
        public new Point Location { get; set; }

        [Browsable(false)]
        public new Rectangle Margin { get; set; }

        [Browsable(false)]
        public new Size MaximumSize { get; set; }

        [Browsable(false)]
        public new Size MinimumSize { get; set; }

        [Browsable(false)]
        public new Rectangle Padding { get; set; }

        [Browsable(false)]
        public new bool RightToLeft { get; set; }

        [Browsable(false)]
        public new Size Size { get; set; }

        [Browsable(false)]
        public new int TabIndex { get; set; }

        [Browsable(false)]
        public new bool TabStop { get; set; }

        [Browsable(false)]
        public new string Tag { get; set; }

        [Browsable(false)]
        public new string Text { get; set; }

        [Browsable(false)]
        public new bool UseWaitCursor { get; set; }

        [Browsable(false)]
        public new bool Visible { get; set; }

    }
}
