using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml.Serialization;
using MC.Other;
using MC.Testing;
using MC.PaperTools;
using MC.Design;
using MC.IO;
using TextBox = MC.Design.TextBox;

namespace MC.Forms
{
    public partial class ctrlDesign : UserControl
    {      
        // todo V2: genummerde antwoordbladen printen: properties: 
            // start with 
            // increment 
            // maximum
            // value if barcode is unreadable
            // ==> zichtbaar en/of barcode!
            // test wat dit geeft indien meerdere pagina's

        public ctrlDesign()
        {
            InitializeComponent();

            btnUndo.Image = Settings.Undo;
            btnRedo.Image = Settings.Redo;
        }

        public void Clear()
        {
            propertyViewer.Clear();
            panelDesign.Controls.Clear();
        }
        
        public void InitializeTest(bool clearUndoList = true)
        {
            if (clearUndoList)
            {
                Program.UndoManager.Reset();
            }
            Clear();
            foreach (Page page in Program.Test.Pages)
            {
                panelDesign.Controls.Add(page);
                foreach (TestElement element in page.Controls)
                {
                    propertyViewer.Add(element);
                }
            }
            panelDesign.Invalidate();

            propertyViewer.Add(Program.Test);
            propertyViewer.Focus(Program.Test);
        }

        public void PropertyFocus(Object o)
        {
            propertyViewer.Focus(o);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber">0 -> It will become the first page, etc. If -1 or left empty, the page will be added at the bottom.</param>
        public void InsertPage(int pageNumber = -1)
        {
            var newPage = new Page(Program.Test);
            panelDesign.Controls.Add(newPage);
            Program.Test.Add(newPage, pageNumber);
            if (panelDesign.Controls.Count == 1) // First/only page
            {
                newPage.Top = 0;
            }
            else
            {
                if (pageNumber == -1)
                {
                    newPage.Top = Program.Test.Pages[panelDesign.Controls.Count - 2].Bottom;
                }
                else
                {
                    if (pageNumber == 0)
                    {
                        newPage.Top = 0;
                    }
                    else
                    {
                        newPage.Top = Program.Test.Pages[pageNumber - 1].Bottom;
                    }
                    for (int page = pageNumber + 1; page < panelDesign.Controls.Count; page++)
                    {
                        Program.Test.Pages[page].Top += Program.Test.Pages[page].Height;
                    }
                }
            }
            // Keep in mind: panelDesign.Controls are not in the right order! (test.Pages are)

            panelDesign.Invalidate();
            propertyViewer.Focus(Program.Test.Paper);
        }

        public void SwitchPages(int pageNumber1, int pageNumber2)
        {
            if (pageNumber1 >= 0 && pageNumber2 >= 0 && Program.Test.Pages.Count >= Math.Max(pageNumber1, pageNumber2))
            {
                var page1 = Program.Test.Pages[pageNumber1];
                Program.Test.Pages[pageNumber1] = Program.Test.Pages[pageNumber2];
                Program.Test.Pages[pageNumber2] = page1;
                Program.Test.Pages[0].Top = 0;
                for (int i = 1; i < Program.Test.Pages.Count; i++)
                {
                    Program.Test.Pages[i].Top = Program.Test.Pages[i - 1].Bottom;
                }
                InitializeTest(false);
            }
        }

        public void ScrollTo(Control control)
        {
            panelDesign.ScrollControlIntoView(control);
        }

        public void ScrollToPage(int pageNumber)
        {
            panelDesign.ScrollControlIntoView(Program.Test.Pages[pageNumber]);
        }

        public int GetCurrentPageNumber()
        {
            if (Program.Test.Pages.Count > 0)
            {
                int dif = 1000000;
                int closest = 0;
                for (int i = 0; i < Program.Test.Pages.Count; i++)
                {
                    if (Program.Test.Pages[i].HasFocus)
                    {
                        closest = i;
                        break;
                    }
                    if (Math.Abs(Program.Test.Pages[i].Top) < dif)
                    {
                        dif = Math.Abs(Program.Test.Pages[i].Top);
                        closest = i;
                    }
                }
                return closest;
            }
            else
            {
                return -1;
            }
        }

        // Click next to the pages
        private void panelDesign_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Program.Test.HasFocus = true;
            }
        }

        private void btnAddPage_Click(object sender, EventArgs e)
        {
            Program.UndoManager.Update();
            InsertPage();

            // Strange, but otherwise no autoscroll to page added at the bottom
            ScrollToPage(Program.Test.Pages.Count - 1);
        }

        private void btnAddItemSet_Click(object sender, EventArgs e)
        {
            int currentPage = GetCurrentPageNumber();
            if (currentPage >= 0)
            {
                Program.UndoManager.Update();
                var set = new ItemSet(Program.Test.GetFreeName("Itemset"), Program.Test.Pages[currentPage]);
                set.NumberOfItems = 10;
                set.NumberOfAlternatives = 5;
                set.NumberOfCheckedAlternativesAllowed = 1;
                set.HasFocus = true;
            }
            else
            {
                MessageBox.Show("You need at least one page.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddTextBox_Click(object sender, EventArgs e)
        {
            int currentPage = GetCurrentPageNumber();
            if (currentPage >= 0)
            {
                Program.UndoManager.Update();
                var text = new TextBox(Program.Test.GetFreeName("Textbox"), Program.Test.Pages[currentPage]);
                text.HasFocus = true;
            }
            else
            {
                MessageBox.Show("You need at least one page.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddImageBox_Click(object sender, EventArgs e)
        {            
            int currentPage = GetCurrentPageNumber();
            if (currentPage >= 0)
            {
                Program.UndoManager.Update();
                var image = new ImageBox(Program.Test.GetFreeName("Imagebox"), Program.Test.Pages[currentPage]);
                image.HasFocus = true;
            }
            else
            {
                MessageBox.Show("You need at least one page.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool SuitableForPdf()
        {
            foreach (Page page in Program.Test.Pages)
            {
                foreach (TestElement element1 in page.Controls)
                {
                    foreach (TestElement element2 in page.Controls)
                    {
                        if (element1.Name != element2.Name)
                        {
                            var rect1 = new Rectangle(element1.Left, element1.Top, element1.Width, element1.Height);
                            var rect2 = new Rectangle(element2.Left, element2.Top, element2.Width, element2.Height);
                            var intersect = Rectangle.Intersect(rect1, rect2);
                            if(intersect.Width > 1 && intersect.Height > 1)
                            {
                                return false;
                            }
                        }
                    }
                    if (element1.Left < Program.Test.Paper.SaveZone.Left || element1.Top < Program.Test.Paper.SaveZone.Top || element1.Right - 1 > Program.Test.Paper.SaveZone.Right || element1.Bottom - 1 > Program.Test.Paper.SaveZone.Bottom)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            if (SuitableForPdf())
            {
                var dlg = new SaveFileDialog();
                dlg.FileName = Program.Test.Name;
                dlg.DefaultExt = ".pdf";
                dlg.Filter = "pdf files (*.pdf)|*.pdf";
                dlg.InitialDirectory = Program.UserSettings.currentDirectory;
                if (dlg.ShowDialog() == DialogResult.OK && dlg.FileName != "")
                {
                    try
                    {

                        PdfBuilder.Create(Program.Test).Save(dlg.FileName);
                        if (Program.UserSettings.openPdfAfterSave)
                        {
                            try
                            {
                                Process.Start(dlg.FileName);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        Program.UserSettings.currentDirectory = Path.GetDirectoryName(dlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Something went wrong while trying to save " + dlg.FileName + ": " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("One or more test elements overlap or are outside of the design area.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            int pageNum = GetCurrentPageNumber();
            if (pageNum < 0)
            {
                InsertPage();
                pageNum = 0;
            }
            Page page = Program.Test.Pages[pageNum];
            switch (keyData)
            {
                case(Keys.Control | Keys.Z):
                    Undo();
                    return true;
                case(Keys.Control | Keys.Y):
                    Redo();
                    return true;
                case (Keys.Control | Keys.X):
                    Program.UndoManager.Update();
                    page.CopyElement(true);
                    return true;
                case (Keys.Control | Keys.C):
                    page.CopyElement();
                    return true;
                case (Keys.Control | Keys.V):
                    Program.UndoManager.Update();
                    page.PasteElement(page.ParentTest.Paper.ScreenPointToGridPoint(panelDesign.PointToClient(MousePosition)));
                    return true;
                case (Keys.Delete):                    
                    if (panelDesign.ClientRectangle.Contains(PointToClient(Cursor.Position)))
                    {
                        Program.UndoManager.Update();
                        Page p = Program.Test.PageCurrentFocus();
                        if (p != null)
                        {
                            p.RemovePage();
                        }
                        else
                        {
                            TestElement e = Program.Test.TestElementCurrentFocus();
                            if (e != null)
                            {
                                e.ParentPage.RemoveTestElement(e);
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return base.ProcessCmdKey(ref msg, keyData);
                    }                    
            }            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void Lock(bool lockAll)
        {
            propertyViewer.Lock(lockAll);            
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
          Undo();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
          Redo();
        }

        public void Undo()
        {
            if (Program.UndoManager.UndoStack.Count > 0)
            {
                Lock(true);
                Program.UndoManager.RedoStack.Push(Program.Test.Copy());
                Clear();
                Program.Test.Pages = new List<Page>();
                Program.Test = Program.UndoManager.UndoStack.Pop();
                Program.Test.Deserialize(this);
                InitializeTest(false);      
                Lock(false);
            }
        }

        public void Redo()
        {
            if (Program.UndoManager.RedoStack.Count > 0)
            {
                Lock(true);
                Program.UndoManager.UndoStack.Push(Program.Test.Copy());
                Clear();
                Program.Test.Pages = new List<Page>();
                Program.Test = Program.UndoManager.RedoStack.Pop();
                Program.Test.Deserialize(this);     
                InitializeTest(false);
                Lock(false);
            }
        }

    }
}