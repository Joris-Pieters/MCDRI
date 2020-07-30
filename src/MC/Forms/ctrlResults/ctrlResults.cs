using System;
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
using System.IO;
using System.Xml.Serialization;
using MC.Other;
using MC.Testing;
using MC.PaperTools;
using MC.Design;
using MC.IO;
using MC.Python;

namespace MC.Forms
{
    public partial class ctrlResults : UserControl
    {
        public List<CorrectedPage> CorrectedPages = new List<CorrectedPage>();
        public List<CorrectedSubject> CorrectedSubjects = new List<CorrectedSubject>();

        private int baseWidth = 60;
        private int currentNumberOfOutputVars;

        public ctrlResults()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            CorrectedPages.Clear();
            CorrectedSubjects.Clear();
            UpdateGrid();
        }

        public void UpdateGrid()
        {
            checkBasedOnScript.Checked = Program.Test.BasedOnScript;
            UpdateSubjects();
            DrawGrid();
        }

        private void UpdateSubjects()
        {
            // Get unique id's
            var ids = new List<string>();
            for (int i = 0; i < CorrectedPages.Count; i++)
            {
                string str = CorrectedPages[i].Subject();
                if (!ids.Contains(str))
                {
                    ids.Add(str);
                }
            }
            
            // Make subject per id
            CorrectedSubjects = new List<CorrectedSubject>();
            var doubles = new List<string>();
            for (int i = 0; i < ids.Count; i++)
            {
                var pagesOfThisSubject = new List<CorrectedPage>();
                for (int j = 0; j < CorrectedPages.Count; j++)
                {
                    if (CorrectedPages[j].Subject() == ids[i])
                    {
                        bool subjectAlreadyHasThisPage = false;
                        for (int k = 0; k < pagesOfThisSubject.Count; k++)
                        {
                            if (pagesOfThisSubject[k].PageNumber == CorrectedPages[j].PageNumber)
                            {
                                subjectAlreadyHasThisPage = true;
                                doubles.Add(CorrectedPages[j].FileName);
                            }
                        }
                        if(!subjectAlreadyHasThisPage)
                        {
                            pagesOfThisSubject.Add(CorrectedPages[j]);
                        }
                    }
                }
                CorrectedSubjects.Add(new CorrectedSubject(pagesOfThisSubject));
                Application.DoEvents();
            }

            if (doubles.Count > 0)
            {
                var str = new StringBuilder();
                for (int i = 0; i < doubles.Count - 1; i++)
                {
                    str.Append(doubles[i] + "\r\n");
                }
                str.Append(doubles[doubles.Count - 1]);
                MessageBox.Show("One or more files have a combination of subject identification and page number that occured earlier. This is most likely the result of a double scan or import.\r\n Following files will be ignored:\r\n" +
                    str.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }               
                
        private int ColumnByName(string name)
        {
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (name == dataGrid.Columns[i].HeaderText)
                {
                    return i;
                }
            }
            return -1;
        }

        private void btnScript_Click(object sender, EventArgs e)
        {
            LoadScriptBox();
        }

        private void checkTotal_CheckedChanged(object sender, EventArgs e)
        {
            DrawGrid();
        }

        private void checkSetTotals_CheckedChanged(object sender, EventArgs e)
        {
            DrawGrid();
        }

        private void checkItems_CheckedChanged(object sender, EventArgs e)
        {
            DrawGrid();
        }

        private void checkShowAsDecimal_CheckedChanged(object sender, EventArgs e)
        {
            DrawGrid();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.FileName = Program.Test.Name;
            dlg.DefaultExt = ".csv";
            dlg.Filter = "csv files (*.csv)|*.csv";
            dlg.InitialDirectory = Program.UserSettings.currentDirectory;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.FileName != "")
                {
                    if (Program.Test.BasedOnScript)
                    {
                        ExportScript(dlg.FileName);
                    }
                    else
                    {
                        ExportNoScript(dlg.FileName);
                    }
                    Program.UserSettings.currentDirectory = Path.GetDirectoryName(dlg.FileName);
                }
            }
        }               
   
        private void checkBasedOnScript_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBasedOnScript.Checked)
            {
                Program.Test.BasedOnScript = true;
                checkTotal.Enabled = false;
                checkSetTotals.Enabled = false;
                checkItems.Enabled = false;
                checkInclTotal.Enabled = false;
                checkInclSetTotals.Enabled = false;
                checkInclItems.Enabled = false;
            }
            else
            {
                Program.Test.BasedOnScript = false;
                checkTotal.Enabled = true;
                checkSetTotals.Enabled = true;
                checkItems.Enabled = true;
                checkInclTotal.Enabled = true;
                checkInclSetTotals.Enabled = true;
                checkInclItems.Enabled = true;
            }
            UpdateGrid();
        }

        private void LoadScriptBox()
        {
            var dlg = new frmScript();
            dlg.ShowDialog();
            if (dlg.DialogResult == DialogResult.OK)
            {
                if (Program.Test.BasedOnScript)
                {
                    UpdateGrid();
                }
            }
            dlg.Dispose();
        }

       


    }
}
