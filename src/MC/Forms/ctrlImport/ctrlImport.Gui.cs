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
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using MC.Graphical;
using MC.Testing;
using MC.PaperTools;
using MC.Other;
using MC.Design;

namespace MC.Forms
{
    public partial class ctrlImport
    {        
        // todo V2: multiselect => geen afbeelding tonen (is reeds in orde indien multiselect via toetsenbord, maar niet via muis) => Blijkt lastig; dataGrid_RowStateChanged is miserie!

        private void btnSortAnalyzed_Click(object sender, EventArgs e)
        {
            Sort(3);
        }

        private void btnSortDoubts_Click(object sender, EventArgs e)
        {
            Sort(4);
        }

        private void btnSortTooMany_Click(object sender, EventArgs e)
        {
            Sort(5);
        }

        private void btnSortBarcode_Click(object sender, EventArgs e)
        {
            Sort(6);
        }

        private void btnSortPageHash_Click(object sender, EventArgs e)
        {
            Sort(7);
        }

        private void Calibration_Click(object sender, EventArgs e)
        {
            Sort(8);
        }

        public class ImageColumComparer : IComparer
        {
            private int column;
            
            public ImageColumComparer(int Column)
            {
                column = Column;
            }

            int IComparer.Compare(object x, object y)
            {
                return (((Bitmap)((DataGridViewRow)x).Cells[column].Value).Width < 
                    ((Bitmap)((DataGridViewRow)y).Cells[column].Value).Width) ? 1 : -1;
            }
        }

        private void Sort(int column)
        {
            dataGrid.Sort(new ImageColumComparer(column));
            dataGrid.ClearSelection();
            dataGrid.Refresh();
        }

        private void btnCalibrate_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();

            // Due to ShowHelp the dialog does not show up in the middle of the parent (stupid, but not a disaster)
            dlg.HelpRequest += CalibrationOpenDialogHelpRequest;
            dlg.ShowHelp = true;
            
            dlg.Filter = Settings.readableFilter;
            dlg.InitialDirectory = Program.UserSettings.currentDirectory;
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.FileNames.GetLength(0) == Program.Test.Pages.Count)
                {
                    List<string> files = new List<string>();
                    foreach (var file in dlg.FileNames)
                    {
                        files.Add(file);
                    }
                    if (dlg.FileNames.GetLength(0) > 0)
                    {
                        Program.UserSettings.currentDirectory = Path.GetDirectoryName(dlg.FileNames[0]);
                    }
                    Calibrate(files);
                }
                else
                {
                    MessageBox.Show("The number of image files you're trying to import is not equal to the number of pages in the test. " +
                        "Please select all pages of the test loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = Settings.readableFilter;
            dlg.InitialDirectory = Program.UserSettings.currentDirectory;
            dlg.Multiselect = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var frmProc = new frmProcessing(this);
                frmProc.Show();

                foreach (var file in dlg.FileNames)
                {
                    Application.DoEvents();
                    if (!ContainsFile(file))
                    {
                        ImportImageFile(file);
                    }
                    else
                    {
                        var answ = MessageBox.Show("The file " + file + " is already imported. Do you want to replace it? Press cancel to interrupt all imports.", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                        if (answ == DialogResult.OK)
                        {
                            ImportImageFile(file);
                        }
                        else if (answ == DialogResult.Cancel)
                        {
                            break;
                        }
                    }
                    Program.UserSettings.currentDirectory = Path.GetDirectoryName(file);
                }

                frmProc.Dispose();
            }
            FillGrid();
            Draw();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            dataGrid.ClearSelection();
            bool testContainsItemsets = false;
            foreach (Page p in Program.Test.Pages)
            {
                foreach (var ctrl in p.Controls)
                {
                    if (ctrl.GetType() == typeof(ItemSet))
                    {
                        testContainsItemsets = true;
                        break;
                    }
                }
            }
            var idList = new List<int>();
            for (int i = 0; i < CorrectedPages.Count; i++)
            {
                idList.Add(CorrectedPages[i].ID);
            }

            if (testContainsItemsets || MessageBox.Show("The current test open does not contain any itemsets. You probably haven't opened the right test. Do you want to start analyzing anyway?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                Analyze(idList);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove all files from the list?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                CorrectedPages.Clear();
                dataGrid.Rows.Clear();

                pictureBox.Image = null;
                pictureBox.Size = new Size(200, 200);
            }
        }
       
        void CalibrationOpenDialogHelpRequest(object sender, EventArgs e)
        {
            MessageBox.Show("Select an scan of the test loaded (one file for each page).\n" +
                       "The scanned files should be completely empty (not filled out), and free of any dirt or smudges.\n" +
                       "Based on this information the system will be able to determine if an item is checked or not.\n\n" +
                       "If you change the the printer or scanning settings, it is adviced to do a new calibration.\n" +
                       "A bad calibration will result in more uncertainty about the items (i.e. are they checked or not), resulting in more time needed to correct the tests."                      
                       , "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void zoom_Scroll(object sender, EventArgs e)
        {
            SetImage();
        }

        private float Zoomvalue()
        {
            return (float)zoom.Value / 8 + .125f;
        }

        private void correctionMenuChecked(object sender, EventArgs e)
        {
            CheckChanged(ItemCheckedState.Checked);
        }

        private void correctionMenuDoubt(object sender, EventArgs e)
        {
            CheckChanged(ItemCheckedState.Doubt);
        }

        private void correctionMenuUnchecked(object sender, EventArgs e)
        {
            CheckChanged(ItemCheckedState.Unchecked);
        }

        private void CheckChanged(ItemCheckedState state)
        {
            var scroll = panelImage.AutoScrollPosition;
            switch (state)
            {
                case (ItemCheckedState.Checked):
                    correctionMenu.MenuItems[2].Checked = true;
                    break;
                case (ItemCheckedState.Doubt):
                    correctionMenu.MenuItems[1].Checked = true;
                    break;
                case (ItemCheckedState.Unchecked):
                    correctionMenu.MenuItems[0].Checked = true;
                    break;
            }

            int indexOfChangingPage = CorrectingPageIndex();

            CorrectedPages[indexOfChangingPage].CheckImage.ItemAltsCheckedState[selectedPosition.X, selectedPosition.Y] = state;
            CorrectedPages[indexOfChangingPage].AnalyzeTooManyAndDoubts();

            PutOnGrid(CorrectedPages[indexOfChangingPage], IndexToRow(indexOfChangingPage));
            panelImage.AutoScrollPosition = new Point(-scroll.X, -scroll.Y);

            Draw();
        }

        public void ImportImageFile(string fileName)
        {
            int fileAlreadyIncluded = -1;
            for (int i = 0; i < CorrectedPages.Count; i++)
            {
                if (fileName == CorrectedPages[i].FileName)
                {
                    fileAlreadyIncluded = i;
                    break;
                }
            }

            if (fileAlreadyIncluded >= 0)
            {
                CorrectedPages.RemoveAt(fileAlreadyIncluded);
            }
            CorrectedPages.Add(new CorrectedPage(fileName, FreeId()));
        }

        public bool ContainsFile(string file)
        {
            bool answ = false;
            for (int i = 0; i < CorrectedPages.Count; i++)
            {
                if (CorrectedPages[i].FileName == file)
                {
                    answ = true;
                    break;
                }
            }
            return answ;
        }

        private int FreeId()
        {
            int max = -1;
            for (int i = 0; i < CorrectedPages.Count; i++)
            {
                max = Math.Max(max, CorrectedPages[i].ID);
            }
            return max + 1;
        }

        private void dataGrid_KeyUp(object sender, KeyEventArgs e)
        {
            selectedPosition = new Point(-1, -1);
            Draw();
        }

        private void dataGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            selectedPosition = new Point(-1, -1);
            Draw();

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    dataGrid.Rows[e.RowIndex].Selected = true;
                    dataGridMenu.Show(dataGrid, dataGrid.PointToClient(Cursor.Position));
                }
            }
        }

        private void dataGridMenuAnalyze(object sender, EventArgs e)
        {
            List<int> idList = new List<int>();
            for (int i = 0; i < dataGrid.SelectedRows.Count; i++)
            {
                idList.Add((int)dataGrid.SelectedRows[i].Cells[0].Value);
            }
            Analyze(idList);
        }

        private void dataGridMenuSetPageNumber(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count > 0)
            {
                using (var dlg = new frmChooseNumber(Program.Test.Pages.Count))
                {
                    dlg.ShowDialog();
                    if (dlg.DialogResult == DialogResult.OK)
                    {
                        for (int i = 0; i < dataGrid.SelectedRows.Count; i++)
                        {
                            CorrectedPage page = IdToPage((int)dataGrid.SelectedRows[i].Cells[0].Value);
                            if (page.Status.Analyzed && !page.Status.CalibrationError) // In case of non calibrated page or calibrationerror
                            {                                                          // it makes no sense of giving a page number!
                                page.PageNumber = dlg.Value;
                                page.CheckImage.CalculateInitialItemAltsCheckedState(page.PageNumber, criteriumSure, criteriumDoubt);
                                page.Status.BarCodeError = false;
                                page.Status.PageNumberOrHashError = false;
                                page.AnalyzeTooManyAndDoubts();
                            }
                            PutOnGrid(page, dataGrid.SelectedRows[i].Index);
                        }                       
                        Draw();
                    }
                }
            }
        }

        private void dataGridMenuRemove(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count == 1)
            {
                if (MessageBox.Show("Are you sure you want to delete " + CorrectedPages[dataGrid.SelectedRows[0].Index].FileName + " from the list?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    RemoveCorrectedPage((int)dataGrid[0, dataGrid.SelectedRows[0].Index].Value);
                    FillGrid();
                }
            }
            else if (dataGrid.SelectedRows.Count > 1)
            {
                if (MessageBox.Show("Are you sure you want to delete multiple items from the list?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var idToRemoveList = new List<int>(); // Works best due to changing collection
                    for (int i = 0; i < dataGrid.SelectedRows.Count; i++)
                    {
                        idToRemoveList.Add((int)dataGrid[0, dataGrid.SelectedRows[i].Index].Value);
                    }
                    for (int i = idToRemoveList.Count - 1; i >= 0; i--) // Reversed order because of changing collection
                    {
                        RemoveCorrectedPage(idToRemoveList[i]);
                    }
                    FillGrid();
                }
            }
            Draw();
        }

    }

}
