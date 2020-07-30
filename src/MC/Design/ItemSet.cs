using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Serialization;
using PdfSharp;
using PdfSharp.Drawing;
using MC.Testing;
using MC.PaperTools;
using MC.Other;
using MC.Forms;

namespace MC.Design
{
    public class ItemSet : TestElement
    {
        // todo V2: "minimum number of expected" => aangevuld met optie om ontbrekende antwoorden als error te zien (moet optie zijn, want bij sommige testen wel mogelijk)

        [Browsable(false)]
        public ReadOnlyCollection<Item> Items { get {return _items.AsReadOnly(); } }
        private List<Item> _items = new List<Item>();

        [CategoryAttribute("Items")]
        [DisplayName("Items naming"), Description("Set the way the items are named.")]
        public Naming ItemsNaming { get { return _itemsNaming; } set { _itemsNaming = value; SetItemsNames(); } }
        private Naming _itemsNaming;

        [CategoryAttribute("Items")]
        [DisplayName("Number of items"), Description("The number of items in the itemset.")]
        public int NumberOfItems
        {
            get
            {
                return Items.Count;
            }
            set
            {
                if (value != Items.Count)
                {
                    value = Math.Min(1000, Math.Max(1, value));
                    int dif = value - Items.Count;
                    if (dif > 0)
                    {
                        for (int i = 0; i < dif; i++)
                        {
                            AddItem(new Item());
                        }
                    }
                    else if (dif < 0)
                    {
                        for (int i = 0; i < -dif; i++)
                        {
                            RemoveItem();
                        }
                    }
                    Columns = Math.Min(Columns, NumberOfItems);
                    columnSplit = Splitting(Items.Count, Columns);
                }
            }
        }
                
        [Browsable(false)]
        public ReadOnlyCollection<string> Alternatives { get { return _alternatives.AsReadOnly(); } }
        private List<string> _alternatives = new List<string>();

        [CategoryAttribute("Alternatives")]
        [DisplayName("Alternatives naming"), Description("Set the way the alternatives are named.")]
        public Naming AlternativesNaming { get { return _alternativesNaming; } set { _alternativesNaming = value; SetAlternativesNames(); } }
        private Naming _alternativesNaming;

        [CategoryAttribute("Alternatives")]
        [DisplayName("Number of alternatives"), Description("The number of alternatives in the itemset.")]
        public int NumberOfAlternatives
        {
            get
            {
                return Alternatives.Count;
            }
            set
            {
                if (value != Alternatives.Count)
                {
                    value = Math.Min(50, Math.Max(1, value));
                    int dif = value - Alternatives.Count;
                    if (dif > 0)
                    {
                        for (int i = 0; i < dif; i++)
                        {
                            AddAlternative();
                        }
                    }
                    else if (dif < 0)
                    {
                        for (int i = 0; i < -dif; i++)
                        {
                            RemoveAlternative();
                        }
                    }
                }
            }
        }
        
        [CategoryAttribute("Alternatives")]
        [DisplayName("Number of checked alternatives allowed"), Description("The maximum number of alternatives of one item that can be checked whereby the answer still remains valid.\r\nIn most cases this equals one.")]
        public int NumberOfCheckedAlternativesAllowed
        {
            get
            {
                return _numberOfCheckedAlternativesAllowed;
            }
            set
            {
                _numberOfCheckedAlternativesAllowed = Math.Max(0, Math.Min(NumberOfAlternatives, value));
            }
        }
        private int _numberOfCheckedAlternativesAllowed;

        [CategoryAttribute("Content")]
        [DisplayName("Role"), Description("The role of the itemset, which can be 'Question', 'Identification', or 'Version'.")]
        public ItemSetRole Role { get; set; }

        [CategoryAttribute("Content")]
        [DisplayName("Description"), Description("The description to show on top of the itemset.")]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
        private string _description = "";

        [CategoryAttribute("Layout")]
        [DisplayName("Columns"), Description("The number of columns.")]
        public int Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                _columns = Math.Max(1, Math.Min(NumberOfItems, value));
                columnSplit = Splitting(Items.Count, Columns);
            }
        }
        private int _columns;

        public bool ApplyScoreTooManyAlternatives = true;
        public Fraction ScoreIfTooManyAlternativesAreChecked = new Fraction(0);
        
        /// <summary>
        /// Contains the number of items in each column; changed if number of columns or number of items is changed
        /// </summary>
        private int[] columnSplit;

        /// <summary>
        /// Per column the x-location of the first alternatives
        /// </summary>
        private int[] columnAltStart;

        private int[] blocksForItemNames;
        private int blocksForAlternatives;

        private List<string> descriptionSplit;

        public ItemSet(string name, Page parentPage, Point gridPosition = new Point(), Size gridSize = new Size(), bool register = true)
            : base(name, parentPage, gridPosition, gridSize, register)
        {
            ItemsNaming = Naming.Numeric;
            AlternativesNaming = Naming.Alphabetical;

            testElementMenu.MenuItems.Add("Edit");
            testElementMenu.MenuItems[5].Click += new EventHandler(menuEdit);

            this.DoubleClick += ItemSet_DoubleClick;
            
            Columns = 1;
            this.Invalidate(); // Otherwise itemset not always well drawn on first attempt
        }

        private void Edit()
        {
            using (var editForm = new frmEditItemSet(this))
            {
                editForm.ShowDialog();
                if (editForm.DialogResult == DialogResult.OK)
                {
                    ClearItems();
                    ClearAlternatives();

                    ItemsNaming = editForm.ItemSet.ItemsNaming;
                    foreach (Item it in editForm.ItemSet.Items)
                    {
                        AddItem(it.Copy());
                    }

                    AlternativesNaming = editForm.ItemSet.AlternativesNaming;
                    string[] altNames = new string[editForm.ItemSet.NumberOfAlternatives];
                    editForm.ItemSet.Alternatives.CopyTo(altNames, 0);
                    _alternatives = altNames.ToList();

                    NumberOfCheckedAlternativesAllowed = editForm.ItemSet.NumberOfCheckedAlternativesAllowed;

                    ApplyScoreTooManyAlternatives = editForm.ItemSet.ApplyScoreTooManyAlternatives;
                    ScoreIfTooManyAlternativesAreChecked = editForm.ItemSet.ScoreIfTooManyAlternativesAreChecked;

                    Description = editForm.ItemSet.Description;
                    Columns = editForm.ItemSet.Columns;

                    this.Invalidate();
                    ParentPage.PropertyViewer().Focus(this);
                }
            }
        }

        void ItemSet_DoubleClick(object sender, EventArgs e)
        {
            Edit();
        }

        public ItemSet Copy()
        {
            string newName = ParentPage.ParentTest.GetFreeName(Name);
            var tmp = new ItemSet(newName, ParentPage, GridPosition, Size, false);
            tmp.Border = Border;

            tmp.ItemsNaming = ItemsNaming;
            foreach (Item it in Items)
            {
                tmp.AddItem(it.Copy());
            }

            tmp.AlternativesNaming = AlternativesNaming;
            string[] altNames = new string[NumberOfAlternatives];
            Alternatives.CopyTo(altNames, 0);
            tmp.SetAlternatives(altNames.ToList());           

            tmp.NumberOfCheckedAlternativesAllowed = NumberOfCheckedAlternativesAllowed;

            tmp.ApplyScoreTooManyAlternatives = ApplyScoreTooManyAlternatives;
            tmp.ScoreIfTooManyAlternativesAreChecked = ScoreIfTooManyAlternativesAreChecked;

            tmp.Description = Description;
            tmp.Columns = Columns;

            return tmp;
        }

        private void menuEdit(object sender, EventArgs e)
        {
            Edit();
        }

        public void AddItem(Item item, int number = -1)
        {
            if (item.Weights == null)
            {
                item.Weights = new Fraction[NumberOfAlternatives];
                item.Checked = new ItemCheckedState[NumberOfAlternatives];
                for (int i = 0; i < NumberOfAlternatives; i++)
                {
                    item.Weights[i] = new Fraction(0);
                    item.Checked[i] = ItemCheckedState.Unknown;
                }
            }

            if (number == -1)
            {
                number = NumberOfItems;
            }           
            _items.Insert(number, item);
            SetItemsNames();
        }

        public void RemoveItem(int number = -1)
        {
            if (number == -1)
            {
                number = NumberOfItems - 1;
            }
            _items.RemoveAt(number);
        }

        public void ClearItems()
        {
            while (NumberOfItems > 0)
            {
                RemoveItem();
            }
        }
        
        public void RenameItem(int i, string name)
        {
            _items[i].Name = name;
        }

        private void SetItemsNames()
        {
            switch (ItemsNaming)
            {
                case (Naming.Numeric):
                    for (int i = 0; i < NumberOfItems; i++)
                    {
                        _items[i].Name = Convert.ToString(i + 1);                        
                    }
                    break;
                case (Naming.Alphabetical):
                    for (int i = 0; i < NumberOfItems; i++)
                    {
                        _items[i].Name = StringTools.Alphabet(i + 1);
                    }
                    break;
            }
        }
        
        public void SetAlternatives(List<string> alternatives)
        {
            _alternatives = new List<string>();
            for (int i = 0; i < alternatives.Count; i++)
            {
                _alternatives.Add(alternatives[i]);
            }
        }

        public void AddAlternative(string alternative = "", int number = -1)
        {
            if (number == -1)
            {
                number = NumberOfAlternatives;
            }            
            _alternatives.Insert(number, alternative);

            foreach (Item it in _items)
            {
                Fraction[] tmpWeights = new Fraction[it.Weights.Count()];
                it.Weights.CopyTo(tmpWeights, 0);
                ItemCheckedState[] tmpChecked = new ItemCheckedState[it.Checked.Count()];
                it.Checked.CopyTo(tmpChecked, 0);

                it.Weights = new Fraction[NumberOfAlternatives];
                it.Checked = new ItemCheckedState[NumberOfAlternatives];
                for (int i = 0; i < number; i++)
                {
                    it.Weights[i] = tmpWeights[i];
                    it.Checked[i] = tmpChecked[i];
                }
                it.Weights[number] = new Fraction(0);
                it.Checked[number] = ItemCheckedState.Unknown;
                for (int i = number + 1; i < NumberOfAlternatives; i++)
                {
                    it.Weights[i] = tmpWeights[i - 1];
                    it.Checked[i] = tmpChecked[i - 1];
                }
            }
            SetAlternativesNames();
        }

        public void RemoveAlternative(int number = -1)            
        {
            if (number == -1)
            {
                number = NumberOfAlternatives - 1;
            }
            _alternatives.RemoveAt(number);

            foreach (Item it in _items)
            {
                Fraction[] tmpWeights = new Fraction[it.Weights.Count()];
                it.Weights.CopyTo(tmpWeights, 0);
                ItemCheckedState[] tmpChecked = new ItemCheckedState[it.Weights.Count()];
                if (it.Checked != null)
                {
                    it.Checked.CopyTo(tmpChecked, 0);
                }

                it.Weights = new Fraction[NumberOfAlternatives];
                it.Checked = new ItemCheckedState[NumberOfAlternatives];
                for (int i = 0; i < number; i++)
                {
                    it.Weights[i] = tmpWeights[i];
                    it.Checked[i] = tmpChecked[i];
                }
                for (int i = number; i < NumberOfAlternatives; i++)
                {
                    it.Weights[i] = tmpWeights[i + 1];
                    it.Checked[i] = tmpChecked[i + 1];
                }
            }
        }

        public void ClearAlternatives()
        {
            while (NumberOfAlternatives > 0)
            {
                RemoveAlternative();
            }
        }

        public void RenameAlternative(int i, string name)
        {
            _alternatives[i] = name;
        }

        private void SetAlternativesNames()
        {
            switch (AlternativesNaming)
            {
                case (Naming.Numeric):
                    for (int i = 0; i < NumberOfAlternatives; i++)
                    {
                        _alternatives[i] = Convert.ToString(i + 1);
                    }
                    break;
                case (Naming.Alphabetical):
                    for (int i = 0; i < NumberOfAlternatives; i++)
                    {
                        _alternatives[i] = StringTools.Alphabet(i + 1);
                    }
                    break;
            }
        }

        private void SetColumnAltStart()
        {
            blocksForItemNames = BlocksForItemNames();
            columnAltStart = new int[Columns];
            int total = 0;
            for (int c = 0; c < Columns; c++)
            {
                total += blocksForItemNames[c];
                columnAltStart[c] = total;
                total += NumberOfAlternatives;
            }
        }

        /// <summary>
        /// Returns the position of a certain alternative for a certain item within the itemset.
        /// </summary>
        public Point ItemAltPoint(int item, int alternative)
        {
            if (item < 0 || item >= NumberOfItems || alternative < 0 || alternative >= NumberOfAlternatives)
            {
                return new Point(-1, -1);
            }
            else
            {                
                // Find out in which column the items is and on wich row
                int col = -1;
                int row = -1;
                int thisColGoesUntil = 0;
                for (int c = 0; c < Columns; c++)
                {
                    thisColGoesUntil += columnSplit[c];
                    if (item < thisColGoesUntil)
                    {
                        col = c;
                        row = item + columnSplit[c] - thisColGoesUntil;                        
                        break;
                    }
                }

                // necessary, because normally set during drawing, but with multiple pages itemssets are not necessarily drawn
                if (columnAltStart == null)
                {                    
                    SetColumnAltStart();
                }
                if (descriptionSplit == null)
                {
                    using (Graphics g = Graphics.FromImage((Image)new Bitmap(1,1)))
                    {
                        int totalWidth = Columns * NumberOfAlternatives;
                        for (int c = 0; c < Columns; c++)
                        {
                            totalWidth += blocksForItemNames[c];
                        }
                        descriptionSplit = StringTools.Split(Description, (totalWidth * blockSize.Width) - 4, g, Settings.font);
                    }
                }
                if (blocksForAlternatives == 0)
                {
                    blocksForAlternatives = BlocksForAlternatives();
                }

                return new Point(columnAltStart[col] + alternative, descriptionSplit.Count + blocksForAlternatives + row);
            }
        }

        /// <summary>
        /// Returns the GRIDPOSITION of a certain alternative for a certain item (is not the postion in the itemset).
        /// </summary>
        public Point ItemAltPointGrid(int item, int alternative)
        {
            var p = ItemAltPoint(item, alternative);
            if (p.X == -1)
            {
                return new Point(-1, -1);
            }
            else
            {
                return new Point(GridPosition.X + p.X, GridPosition.Y + p.Y);
            }
        }
                
        /// <summary>
        /// Get the score of a total itemset or single item
        /// </summary>                
        public Fraction Score(CorrectedPage page, int item = -1)
        {
            Fraction sum = new Fraction();
            if (page.Status.Analyzed && !page.Status.AnyError())
            {
                if (item >= 0 && item < Items.Count) // specific item
                {
                    sum += Items[item].Score(this);
                }
                else // all items
                {  
                    for (int i = 0; i < Items.Count; i++)
                    {
                        sum += Items[i].Score(this);
                    }
                }
            }
            return sum;
        }

        /// <summary>
        /// Get the score for identification
        /// </summary>                
        public string ScoreString(CorrectedPage page, int item = -1)
        {
            var str = new StringBuilder();
            if (page.Status.Analyzed && page.PageNumber >= 0)
            {
                if (item >= 0 && item < Items.Count) // specific item
                {
                    str.Append(Items[item].ScoreString(Alternatives));
                }
                else // all items
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        str.Append(Items[i].ScoreString(Alternatives));
                    }
                }
            }
            return str.ToString();
        }

        public void UpdateItemCheckedState(CorrectedPage page)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Checked = GetCheckedStateFromCorrectedPage(page, i);
            }
        }

        private ItemCheckedState[] GetCheckedStateFromCorrectedPage(CorrectedPage page, int item)
        {
            var check = new ItemCheckedState[NumberOfAlternatives];
            for (int i = 0; i < NumberOfAlternatives; i++)
            {
                var grid = ItemAltPointGrid(item, i);
                check[i] = page.CheckImage.ItemAltsCheckedState[grid.X, grid.Y];
            }
            return check;
        }
        
        /// <summary>
        /// Array with the number of items per column
        /// </summary>
        private int[] Splitting(int items, int columns)
        {
            columns = Math.Max(1, columns);
            int[] spl = new int[columns];
            int inFirst = items / columns;
            for (int i = 0; i < columns; i++)
            {
                spl[i] = inFirst;
            }
            int inNext = items - (inFirst * columns);
            int ind = 0;
            while (inNext > 0)
            {
                spl[ind]++;
                ind++;
                inNext--;
            }            
            return spl;
        }

        private int[] BlocksForItemNames()
        {
            var blocksItemNamesPerColumn = new int[_columns];
            using (var g = Graphics.FromImage((Image)(new Bitmap(1, 1))))
            {
                int counter = 0;
                for (int i = 0; i < _columns; i++)
                {
                    var itemNameList = new List<string>();
                    for (int j = 0; j < columnSplit[i]; j++)
                    {
                        itemNameList.Add(Items[counter + j].Name);
                    }
                    int longestItemName = StringTools.Longest(itemNameList, g, Settings.font);
                    blocksItemNamesPerColumn[i] = ((int)Math.Ceiling((double)longestItemName / Settings.blockWidth));
                    counter += columnSplit[i];
                }
            }
            return blocksItemNamesPerColumn;
        }

        private int BlocksForAlternatives()
        {
            using (var g = Graphics.FromImage((Image)(new Bitmap(1, 1))))
            {
                int longestAlternative = StringTools.Longest(_alternatives, g, Settings.font);
                if (longestAlternative == 0)
                {
                    return 0;
                }
                else
                {
                    return longestAlternative <= Settings.blockWidth - 3 ? 1 : (int)Math.Ceiling((double)longestAlternative / Settings.blockHeight);
                }
            }
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
        {
            blocksForItemNames = BlocksForItemNames();
            blocksForAlternatives = BlocksForAlternatives();

            Size blockSize = ParentPage.ParentTest.Paper.BlockSize;
            using (Graphics g = pe.Graphics)
            {
                Brush brush = new SolidBrush(Color.Black);
                Pen pen = new Pen(Color.Black);
                Pen pen2 = new Pen(Color.Gray);

                StringFormat formatDec = new StringFormat();
                formatDec.Alignment = StringAlignment.Near;
                formatDec.LineAlignment = StringAlignment.Center;

                StringFormat formatAlt = new StringFormat();
                formatAlt.Alignment = StringAlignment.Center;
                formatAlt.LineAlignment = StringAlignment.Center;

                StringFormat formatNumber = new StringFormat();
                formatNumber.Alignment = StringAlignment.Far;
                formatNumber.LineAlignment = StringAlignment.Center;

                int totalWidth = Columns * NumberOfAlternatives;
                for (int c = 0; c < Columns; c++)
                {
                    totalWidth += blocksForItemNames[c];
                }

                descriptionSplit = StringTools.Split(Description, (totalWidth * blockSize.Width) - 4, g, Settings.font);
                GridSize = new Size(totalWidth, columnSplit[0] + blocksForAlternatives + descriptionSplit.Count);
                g.FillRectangle(new SolidBrush(Color.White), this.DisplayRectangle);

                for (int i = 0; i < descriptionSplit.Count; i++)
                {
                    g.DrawString(descriptionSplit[i], Settings.font, brush, new Point(2, (int)((i + .5) * blockSize.Height)), formatDec);
                }
                if (Border)
                {
                    g.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), 0, descriptionSplit.Count * blockSize.Height, Width, descriptionSplit.Count * blockSize.Height);
                }
                else // No border, but drawn in GUI for clarity
                {
                    g.DrawLine(new Pen(Color.LightGray), 0, descriptionSplit.Count * blockSize.Height, Width, descriptionSplit.Count * blockSize.Height);
                }

                columnAltStart = new int[Columns]; // x-value for firts alt for each column (need this to get right itemAltLocation)
                int itemCount = 1;
                int blockBegin = 0;
                for (int c = 0; c < Columns; c++)
                {
                    int gridBegin = blockBegin * blockSize.Width;

                    // Line separating columns
                    if (Border)
                    {
                        g.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), gridBegin, descriptionSplit.Count * blockSize.Height, gridBegin, Height);
                    }
                    else // No border, but drawn in GUI for clarity
                    {
                        g.DrawLine(new Pen(Color.LightGray), gridBegin, descriptionSplit.Count * blockSize.Height, gridBegin, Height);
                    }

                    columnAltStart[c] = blockBegin + blocksForItemNames[c];
                    for (int a = 0; a < NumberOfAlternatives; a++)
                    {
                        if (blocksForAlternatives == 1) // Place alternatives horizontally
                        {
                            g.DrawString(Alternatives[a], Settings.font, brush, new Point((int)(gridBegin + ((a + blocksForItemNames[c] + .5) * blockSize.Width)), (int)((.5 + descriptionSplit.Count) * blockSize.Height)), formatAlt);
                        }
                        else // Place alternatives vertically
                        {
                            formatAlt.FormatFlags = StringFormatFlags.DirectionVertical;
                            formatAlt.Alignment = StringAlignment.Far;
                            g.DrawString(Alternatives[a], Settings.font, brush, new Point((int)(gridBegin + ((a + blocksForItemNames[c] + .5) * blockSize.Width)), (int)((blocksForAlternatives + descriptionSplit.Count) * blockSize.Height)), formatAlt);
                        }
                        for (int r = 0; r < columnSplit[c]; r++)
                        {
                            g.DrawRectangle(pen2, gridBegin + ((a + blocksForItemNames[c]) * blockSize.Width) + 4, (r + descriptionSplit.Count + blocksForAlternatives) * blockSize.Height + 4, blockSize.Width - 8, blockSize.Height - 8);                        
                        }
                    }
                    for(int i = 0; i < columnSplit[c]; i++)
                    {
                        g.DrawString(Items[itemCount - 1].Name, Settings.font, brush, new Point(gridBegin + (blocksForItemNames[c] * blockSize.Width), (int)((i + descriptionSplit.Count + blocksForAlternatives + .5) * blockSize.Height)), formatNumber);
                        itemCount++;
                    }
                    blockBegin += blocksForItemNames[c] + NumberOfAlternatives;                    
                }
                base.OnPaint(pe);
            }
        }

        public override void PdfDraw(XGraphics g)
        {
            XBrush brush = new XSolidBrush(Color.Black);
            XPen pen = new XPen(Color.Black);
            XPen pen2 = new XPen(Color.Gray);

            XStringFormat formatDec = new XStringFormat();
            formatDec.Alignment = XStringAlignment.Near;
            formatDec.LineAlignment = XLineAlignment.Center;

            XStringFormat formatAlt = new XStringFormat();
            formatAlt.Alignment = XStringAlignment.Center;
            formatAlt.LineAlignment = XLineAlignment.Center;

            XStringFormat formatNumber = new XStringFormat();
            formatNumber.Alignment = XStringAlignment.Far;
            formatNumber.LineAlignment = XLineAlignment.Center;

            for (int i = 0; i < descriptionSplit.Count; i++)
            {
                g.DrawString(descriptionSplit[i], Settings.xfont, brush, new Point(Left + 2, Top + (int)((i + .5) * blockSize.Height)), formatDec);
            }
            if (Border)
            {
                g.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), Left, Top + (descriptionSplit.Count * blockSize.Height), Left + Width - 1, Top + (descriptionSplit.Count * blockSize.Height));
            }
            
            int itemCount = 1;
            int blockBegin = 0;
            for (int c = 0; c < Columns; c++)
            {
                int gridBegin = blockBegin * blockSize.Width;
                
                // Line separating columns
                if (Border)
                {
                    g.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), Left + gridBegin, Top + (descriptionSplit.Count * blockSize.Height), Left + gridBegin, Top + Height - 1);
                }

                for (int a = 0; a < NumberOfAlternatives; a++)
                {
                    if (blocksForAlternatives == 1) // Place alternatives horizontally
                    {
                        g.DrawString(Alternatives[a], Settings.xfont, brush, new Point(Left + (int)(gridBegin + ((a + blocksForItemNames[c] + .5) * blockSize.Width)), Top + (int)((.5 + descriptionSplit.Count) * blockSize.Height)), formatAlt);
                    }
                    else // Place alternatives vertically
                    {
                        XGraphicsState gs = g.Save();
                        g.RotateAtTransform(90, new XPoint(Left + (int)(gridBegin + ((a + blocksForItemNames[c] + .5) * blockSize.Width)), Top + (int)((blocksForAlternatives + descriptionSplit.Count) * blockSize.Height)));
                        formatAlt.Alignment = XStringAlignment.Far;
                        g.DrawString(Alternatives[a], Settings.xfont, brush, new Point(Left + (int)(gridBegin + ((a + blocksForItemNames[c] + .5) * blockSize.Width)), Top + (int)((blocksForAlternatives + descriptionSplit.Count) * blockSize.Height)), formatAlt);
                        g.Restore(gs);
                    }
                    for (int r = 0; r < columnSplit[c]; r++)
                    {
                        g.DrawRectangle(pen2, Left + gridBegin + ((a + blocksForItemNames[c]) * blockSize.Width) + 4, Top + (r + descriptionSplit.Count + blocksForAlternatives) * blockSize.Height + 4, blockSize.Width - 8, blockSize.Height - 8);
                    }
                }
                for (int i = 0; i < columnSplit[c]; i++)
                {
                    g.DrawString(Items[itemCount - 1].Name, Settings.xfont, brush, new Point(Left + gridBegin + (blocksForItemNames[c] * blockSize.Width), Top + (int)((i + descriptionSplit.Count + blocksForAlternatives + .5) * blockSize.Height)), formatNumber);
                    itemCount++;
                }
                blockBegin += blocksForItemNames[c] + NumberOfAlternatives;     
            }
            base.PdfDraw(g);
        }

    }

    public enum Naming
    {
        Numeric = 0,
        Alphabetical,
        Custom
    }

    public enum ItemSetRole
    {
        Question = 0,
        Identification,
        Version
    }

}