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
using MC.Graphical;
using MC.Testing;
using MC.PaperTools;
using MC.Other;
using MC.Design;

namespace MC.Forms
{
    public partial class ctrlImport : UserControl
    {
        // todo V2: Wizard voor aangeven aangeduidde antwoorden indien bestand niet gecalibreerd kan worden

        public List<CorrectedPage> CorrectedPages = new List<CorrectedPage>();

        float criteriumSure;
        float criteriumDoubt;

        private Point selectedPosition = new Point(-1, -1);  // In gridvalue!
        private System.Windows.Forms.ContextMenu correctionMenu = new System.Windows.Forms.ContextMenu();
        private System.Windows.Forms.ContextMenu dataGridMenu = new System.Windows.Forms.ContextMenu();

        public ctrlImport()
        {
            InitializeComponent();

            correctionMenu.MenuItems.Add("Unchecked");
            correctionMenu.MenuItems[0].Click += new EventHandler(correctionMenuUnchecked);
            correctionMenu.MenuItems.Add("Doubt");
            correctionMenu.MenuItems[1].Click += new EventHandler(correctionMenuDoubt);
            correctionMenu.MenuItems.Add("Checked");
            correctionMenu.MenuItems[2].Click += new EventHandler(correctionMenuChecked);

            dataGridMenu.MenuItems.Add("Analyze");
            dataGridMenu.MenuItems[0].Click += new EventHandler(dataGridMenuAnalyze);
            dataGridMenu.MenuItems.Add("Set page number");
            dataGridMenu.MenuItems[1].Click += new EventHandler(dataGridMenuSetPageNumber);
            dataGridMenu.MenuItems.Add("Remove");
            dataGridMenu.MenuItems[2].Click += new EventHandler(dataGridMenuRemove);

            InitializeGrid();
        }

        public void Clear()
        {
            CorrectedPages.Clear();
            FillGrid();
            Draw();
        }

        /// <summary>
        /// To check when changing to design mode 
        /// </summary>        
        public bool ContainsAnalysedPages()
        {
            bool cont = false;
            for (int i = 0; i < CorrectedPages.Count; i++)
            {
                if(CorrectedPages[i].Status.Analyzed)
                {
                    cont = true;
                    break;
                }
            }
            return cont;
        }

        private void Draw()
        {
            int index = CorrectingPageIndex();

            if (index >= 0)
            {
                Image imageToDrawOn;
                if (CorrectedPages[index].CheckImage.ProcessedImage)
                {
                    imageToDrawOn = Processing.ByteToImage(CorrectedPages[index].CheckImage.ScannedImage);
                    
                    using (Graphics g = Graphics.FromImage(imageToDrawOn))
                    {                       
                        if (CorrectedPages[index].CheckImage.ItemAltsCheckedState != null)
                        {                           
                            // Draw box where there might be a problem
                            var problemList = CorrectedPages[index].AnalyzeTooManyAndDoubts();
                            using (var b = new SolidBrush(Color.FromArgb(
                                Program.UserSettings.colorMark[0], 
                                Program.UserSettings.colorMark[1], 
                                Program.UserSettings.colorMark[2], 
                                Program.UserSettings.colorMark[3])))
                            {
                                for (int i = 0; i < problemList.Count; i++)
                                {
                                    var str = problemList[i].Split('_');
                                    var problemString = str[0];
                                    for (int s = 1; s < str.GetLength(0) - 1; s++)
                                    {
                                        problemString += "_" + str[s];
                                    }
                                    var set = Program.Test.ItemSetByName(problemString);
                                    if (set != null)
                                    {
                                        int item = Convert.ToInt16(str[str.GetLength(0) - 1]);
                                        var r1 = CorrectedPages[index].CheckImage.ItemAltRectangle(set.ItemAltPointGrid(item, 0), 4);
                                        var r2 = CorrectedPages[index].CheckImage.ItemAltRectangle(set.ItemAltPointGrid(item, set.NumberOfAlternatives - 1), 4);
                                        var blockSize = CorrectedPages[index].CheckImage.AdjustedBlockSize();
                                        g.FillRectangle(b, new RectangleF(r1.Left, r1.Top, r2.Right - r1.Left, r2.Bottom - r1.Top));
                                    }
                                }
                            }

                            // Draw boxes around itemalts
                            using (Pen 
                                pGreen = new Pen(Color.FromArgb(
                                Program.UserSettings.colorSure[0], 
                                Program.UserSettings.colorSure[1], 
                                Program.UserSettings.colorSure[2], 
                                Program.UserSettings.colorSure[3]), 2), 
                                pRed = new Pen(Color.FromArgb(
                                Program.UserSettings.colorDoubt[0],
                                Program.UserSettings.colorDoubt[1],
                                Program.UserSettings.colorDoubt[2],
                                Program.UserSettings.colorDoubt[3]), 2))
                            {                                
                                for (int j = 0; j < Program.Test.Paper.Blocks.Y; j++)
                                {
                                    for (int i = 0; i < Program.Test.Paper.Blocks.X; i++)
                                    {
                                        var rect = CorrectedPages[index].CheckImage.ItemAltRectangle(i, j);
                                        if (CorrectedPages[index].CheckImage.ItemAltsCheckedState[i, j] == ItemCheckedState.Checked) // Sure that it is checked
                                        {
                                            g.DrawRectangle(pGreen, rect.X, rect.Y, rect.Width, rect.Height);
                                        }
                                        else if (CorrectedPages[index].CheckImage.ItemAltsCheckedState[i, j] == ItemCheckedState.Doubt) // Not sure if checked or not
                                        {
                                            g.DrawRectangle(pRed, rect.X, rect.Y, rect.Width, rect.Height);
                                        }
                                    }
                                }
                            }
                        }

                        // Draw "cursor"
                        if (selectedPosition.X >= 0 && selectedPosition.Y >= 0)
                        {
                            if (CorrectedPages[index].CheckImage.ItemAltsCheckedState[selectedPosition.X, selectedPosition.Y] != ItemCheckedState.Unknown &&
                                CorrectedPages[index].CheckImage.ItemAltsCheckedState[selectedPosition.X, selectedPosition.Y] != ItemCheckedState.Unavailable)
                            {
                                var rect = CorrectedPages[index].CheckImage.ItemAltRectangle(selectedPosition, 3);
                                g.DrawRectangle(new Pen(Color.FromArgb(
                                    Program.UserSettings.colorCursor[0],
                                    Program.UserSettings.colorCursor[1],
                                    Program.UserSettings.colorCursor[2],
                                    Program.UserSettings.colorCursor[3]), 2), rect.X, rect.Y, rect.Width, rect.Height);
                            }
                        }                       
                    }
                }
                else
                {
                    // No calibrated image available
                    imageToDrawOn = Processing.ByteToImage(CorrectedPages[index].CheckImage.ScannedImage);
                }
                SetImage(imageToDrawOn);
            }
            else
            {
                pictureBox.Image = null;
                pictureBox.Size = new Size(200, 200);
            }
        }

        private void SetImage()
        {
            if (pictureBox.Image != null)
            {
                pictureBox.Size = new Size(
                    (int)(pictureBox.Image.Width * Zoomvalue()),
                    (int)(pictureBox.Image.Height * Zoomvalue()));
            }
        }

        private void SetImage(Image img)
        {
            try
            {
                if (img != null)
                {
                    pictureBox.Image = img;
                }
                SetImage();
            }
            catch (Exception ex)
            {
                Logger.LogHigh("Error SetImage: " + ex.Message);
            }
        }

        private void Analyze(List<int> IdList)
        {
            if (IdList.Count > 0)
            {
                // These values can be obtained by scanning an EMPTY test (see calibration)
                criteriumSure = (float)(Program.Test.CalibrationMean - (Program.UserSettings.SDsure * Program.Test.CalibrationSD));
                criteriumDoubt = (float)(Program.Test.CalibrationMean - (Program.UserSettings.SDdoubt * Program.Test.CalibrationSD));

                var frmProc = new frmProcessing(this);
                frmProc.Show();
                
                if (IdList.Count == 1)
                {
                    // Easier for debugging not to run in parallel if not needed                    
                    Run(IdToPage(IdList[0]));
                }
                else
                {
                    Parallel.For(0, IdList.Count, new ParallelOptions { MaxDegreeOfParallelism = Program.UserSettings.numberOfParallelThreads }, i =>
                    {
                        Run(IdToPage(IdList[i]));
                    });
                }

                frmProc.Dispose();
                FillGrid();
                Draw();
            }
        }

        private void Run(CorrectedPage p)
        {
            p.AnalyzeGraphical(criteriumSure, criteriumDoubt);
            if (!p.Status.AnyError() && p.PageNumber > -1) // Passed all error checks
            {
                p.AnalyzeTooManyAndDoubts();
            }
            Application.DoEvents();
        }

        private void Calibrate(List<string> fileNames)
        {
            try // Number of files == pages in test (already checked)
            {
                var frmProc = new frmProcessing(this);
                frmProc.Show();

                var correctPageNumbers = new List<int>();
                var colDist = new ColorDistribution();

                // Better not do this in parallel (colDist is shared); most of the time this is one page only anyway
                for (int i = 0; i < fileNames.Count; i++)
                {
                    using (var cPage = new CorrectedPage(fileNames[i]))
                    {
                        cPage.AnalyzeGraphical(criteriumSure, criteriumDoubt);                       
                        if (cPage.PageNumber > -1) // Passed all error checks
                        {
                            correctPageNumbers.Add(cPage.PageNumber);
                            colDist.Add(cPage.CheckImage.ColorDistribution.Values);
                        }
                    }
                }

                frmProc.Dispose();

                bool pagesOk = true;
                if (correctPageNumbers.Count == Program.Test.Pages.Count)
                {
                    for (int i = 0; i < correctPageNumbers.Count; i++)
                    {
                        if (!correctPageNumbers.Contains(i))
                        {
                            pagesOk = false;
                            break;
                        }
                    }
                }
                else
                {
                    pagesOk = false;
                }                

                if (pagesOk)
                {
                    using (var frm = new frmCalibrationResults(colDist))
                    {
                        frm.ShowDialog();                        
                    }
                }                
                else
                {

                #if DEBUG
                    if (MessageBox.Show("The barcode of the file you selected could not be read correctly or doesn't match the open test.\r\nDo you want to continue the calibration? "
                           + "Try again with barcode check turned off?",
                           "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    {
                        Settings.IgnoreBarCodeErrors = true;
                        Calibrate(fileNames);
                        Settings.IgnoreBarCodeErrors = false;
                    }   
                #else
                    MessageBox.Show(Convert.ToString(Program.Test.Pages.Count - CorrectedPages.Count) + " of the selected images could not be used for calibration. " +
                        "Make sure the images you're trying to use correspond to the current test. Consider scanning images again using higher quality settings.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                #endif

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Calibration failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeGrid()
        {
            dataGrid.Rows.Clear();
            dataGrid.Columns.Clear();

            dataGrid.Columns.Add("ID", "");
            dataGrid.Columns[0].Visible = false;

            dataGrid.Columns.Add("File", "File");
            dataGrid.Columns[1].Width = 165;

            dataGrid.Columns.Add("Page", "Page");
            dataGrid.Columns[2].Width = 32;
            dataGrid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            for (int i = 3; i < 9; i++)
            {
                dataGrid.Columns.Add(new DataGridViewImageColumn());
                dataGrid.Columns[i].Width = 32;
                dataGrid.Columns[i].Resizable = DataGridViewTriState.False;
                dataGrid.Columns[i].HeaderText = "";
                dataGrid.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
            }
        }

        // Put new correctedPage on the grid, or change one row
        private void PutOnGrid(CorrectedPage correctedPage, int row = -1)
        {
            if (row == -1)
            {
                dataGrid.Rows.Add();
                dataGrid.Rows[dataGrid.RowCount - 1].Height = 32;
                row = dataGrid.RowCount - 1;
            }
            dataGrid[0, row].Value = correctedPage.ID;
            dataGrid[1, row].Value = correctedPage.ShortFileName();
            dataGrid[2, row].Value = correctedPage.PageNumber >= 0 ? Convert.ToString(correctedPage.PageNumber + 1) : "?";
            dataGrid[3, row].Value = correctedPage.Status.Analyzed ? Settings.Empty : Settings.NotAnalyzed;
            dataGrid[4, row].Value = correctedPage.Status.ContainsDoubts ? Settings.ContainsDoubts : Settings.Empty;
            dataGrid[5, row].Value = correctedPage.Status.ContainsTooManyChecked ? Settings.ContainsTooManyChecked : Settings.Empty;
            dataGrid[6, row].Value = correctedPage.Status.BarCodeError ? Settings.BarCodeError : Settings.Empty;
            dataGrid[7, row].Value = correctedPage.Status.PageNumberOrHashError ? Settings.PageNumberOrHashError : Settings.Empty;
            dataGrid[8, row].Value = correctedPage.Status.CalibrationError ? Settings.CalibrationError : Settings.Empty;
        }

        // Clears grid and refills => shown order may change
        private void FillGrid()
        {
            dataGrid.Rows.Clear();
            foreach (var page in CorrectedPages)
            {
                PutOnGrid(page);
            }
        }

        private void RemoveCorrectedPage(int id)
        {
            selectedPosition = new Point(-1, -1);
            CorrectedPages.RemoveAt(IdToIndex(id));
        }

        // The id of the page that is currently selected in the grid (only if only one is selected)
        private int CorrectingPageId(bool allowMultiple = false)
        {
            if (dataGrid.SelectedRows.Count == 1 || (dataGrid.SelectedRows.Count > 1 && allowMultiple))
            {
                return (int)dataGrid.SelectedRows[0].Cells[0].Value;
            }            
            else
            {
                return -1;
            }
        }

        // The index of the page that is currently selected in the grid (only if only one is selected)
        private int CorrectingPageIndex()
        {
            return IdToIndex(CorrectingPageId());
        }

    }
}