
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

namespace MC.Forms
{
    public partial class frmEditItemSet : Form
    {  
        public ItemSet ItemSet;
        bool checkInput = false;
        bool filling = false;
        System.Windows.Forms.ContextMenu rightClickMenuHeaders = new System.Windows.Forms.ContextMenu();
        System.Windows.Forms.ContextMenu rightClickMenuCells = new System.Windows.Forms.ContextMenu(); 

        public frmEditItemSet(ItemSet itemSet)
        {
            ItemSet = itemSet.Copy();

            InitializeComponent();

            this.ClientSize = new Size(this.ClientSize.Width, txtScoreIfTooMany.Bottom + btnOk.Height + 10);
            btnOk.Left = (this.ClientSize.Width / 2) - (btnOk.Width + 3);
            btnCancel.Left = (this.ClientSize.Width / 2) + 3;
            btnOk.Top = this.ClientSize.Height - btnOk.Height - 5;
            btnCancel.Top = this.ClientSize.Height - btnCancel.Height - 5;

            int tmpItemsNaming = (int)ItemSet.ItemsNaming;
            int tmpAlternativesNaming = (int)ItemSet.AlternativesNaming;

            comboItemsNaming.DataSource = Enum.GetValues(typeof(Naming));
            comboAlternativesNaming.DataSource = Enum.GetValues(typeof(Naming));

            comboItemsNaming.SelectedIndex = tmpItemsNaming;
            comboAlternativesNaming.SelectedIndex = tmpAlternativesNaming;    
                       
            rightClickMenuHeaders.MenuItems.Add("Insert");
            rightClickMenuHeaders.MenuItems[0].Click += new EventHandler(menuHeadersInsert);
            rightClickMenuHeaders.MenuItems.Add("Remove");
            rightClickMenuHeaders.MenuItems[1].Click += new EventHandler(menuHeadersRemove);

            rightClickMenuCells.MenuItems.Add("Cut");
            rightClickMenuCells.MenuItems[0].Shortcut = Shortcut.CtrlX;
            rightClickMenuCells.MenuItems[0].Click += new EventHandler(menuCellsCut);
            rightClickMenuCells.MenuItems.Add("Copy");
            rightClickMenuCells.MenuItems[1].Shortcut = Shortcut.CtrlC;
            rightClickMenuCells.MenuItems[1].Click += new EventHandler(menuCellsCopy);
            rightClickMenuCells.MenuItems.Add("Paste");
            rightClickMenuCells.MenuItems[2].Shortcut = Shortcut.CtrlV;
            rightClickMenuCells.MenuItems[2].Click += new EventHandler(menuCellsPaste);
            rightClickMenuCells.MenuItems.Add("Set multiple weights");
            rightClickMenuCells.MenuItems[3].Click += new EventHandler(menuCellsSet);

            FillGrid();
            checkInput = true;
        }     

        private void FillGrid()
        {
            filling = true;

            dataGrid.Rows.Clear();
            dataGrid.Columns.Clear();
            dataGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;  


            // Columns
            
            dataGrid.Columns.Add("ItemName", "");
            dataGrid.Columns[0].Width = 80;
            dataGrid.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGrid.Columns[0].DefaultCellStyle.BackColor = SystemColors.Info;
            for (int j = 0; j < ItemSet.NumberOfAlternatives; j++)
            {
                dataGrid.Columns.Add("alt " + Convert.ToString(j + 1), "");
                dataGrid.Columns[j + 1].Width = 40;
                dataGrid.Columns[j + 1].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            
            
            // Rows
            
            dataGrid.Rows.Add(ItemSet.Items.Count + 1);
            dataGrid.Rows[0].DefaultCellStyle.BackColor = SystemColors.Info;
            for (int j = 0; j < ItemSet.NumberOfAlternatives; j++)
            {
                dataGrid[j + 1, 0].Value = ItemSet.Alternatives[j];
            }


            // Values

            dataGrid[0, 0].ReadOnly = true;
            for (int i = 0; i < ItemSet.Items.Count; i++)
            {
                dataGrid[0, i + 1].Value = ItemSet.Items[i].Name;
                for (int j = 0; j < ItemSet.NumberOfAlternatives; j++)
                {
                    dataGrid[j + 1, i + 1].Value = ItemSet.Items[i].Weights[j].ToString();
                }
            }
            

            // Other

            numNumItems.Value = ItemSet.NumberOfItems;
            numNumAlternatives.Value = ItemSet.NumberOfAlternatives;
            numNumAnswers.Value = ItemSet.NumberOfCheckedAlternativesAllowed;
            numNumAnswers.Maximum = numNumAlternatives.Value;
            checkScoreIfTooMany.Checked = ItemSet.ApplyScoreTooManyAlternatives;
            txtScoreIfTooMany.Text = ItemSet.ScoreIfTooManyAlternativesAreChecked.ToString();

            filling = false;
        }


        // +----------------------------------------------+
        // | Copy/Paste                                    |
        // +----------------------------------------------+        

        private void Copy(bool deleteAfter = false)
        {
            if (dataGrid.SelectedCells.Count > 0)
            {
                Point startSelection = new Point(dataGrid.Columns.Count, dataGrid.Rows.Count);
                Point stopSelection = new Point();
                for (int i = 0; i < dataGrid.SelectedCells.Count; i++)
                {
                    startSelection.X = Math.Min(startSelection.X, dataGrid.SelectedCells[i].ColumnIndex);
                    startSelection.Y = Math.Min(startSelection.Y, dataGrid.SelectedCells[i].RowIndex);
                    stopSelection.X = Math.Max(stopSelection.X, dataGrid.SelectedCells[i].ColumnIndex);
                    stopSelection.Y = Math.Max(stopSelection.Y, dataGrid.SelectedCells[i].RowIndex);
                }
                var str = new StringBuilder();
                for (int j = startSelection.Y; j <= stopSelection.Y; j++)
                {
                    for (int i = startSelection.X; i <= stopSelection.X; i++)
                    {
                        if (dataGrid[i, j].Selected)
                        {
                            str.Append(dataGrid[i, j].Value);
                            if (deleteAfter)
                            {
                                if ((i == 0 && j > 0) || (i > 0 && j == 0))
                                {
                                    dataGrid[i, j].Value = "";
                                }
                                else if (i > 0 && j > 0)
                                {
                                    dataGrid[i, j].Value = 0;
                                }
                            }
                        }
                        if (i < stopSelection.X)
                        {
                            str.Append("\t");
                        }
                    }
                    if (j < stopSelection.Y)
                    {
                        str.Append("\n");
                    }
                }
                if (str.Length > 0)
                {
                    Clipboard.SetText(str.ToString());
                }
            }
        }

        private void Paste()
        {
            if (dataGrid.SelectedCells.Count > 0)
            {
                Point startSelection = new Point(dataGrid.Columns.Count, dataGrid.Rows.Count);
                for (int i = 0; i < dataGrid.SelectedCells.Count; i++)
                {
                    startSelection.X = Math.Min(startSelection.X, dataGrid.SelectedCells[i].ColumnIndex);
                    startSelection.Y = Math.Min(startSelection.Y, dataGrid.SelectedCells[i].RowIndex);
                }
                string[] stringRows = Clipboard.GetText().Split(new string[] { "\n" }, StringSplitOptions.None);
                for (int j = 0; j < stringRows.Count(); j++)
                {
                    if (startSelection.Y + j < dataGrid.RowCount)
                    {
                        string[] stringElements = stringRows[j].Split(new string[] { "\t" }, StringSplitOptions.None);
                        for (int i = 0; i < stringElements.Count(); i++)
                        {
                            if (startSelection.X + i < dataGrid.ColumnCount)
                            {
                                if (!(startSelection.X + i == 0 && startSelection.Y + j == 0))
                                {
                                    if (stringElements[i] != "")
                                    {
                                        if (Fraction.IsFraction(stringElements[i]) || startSelection.X + i == 0 || startSelection.Y + j == 0)
                                        {
                                            dataGrid[startSelection.X + i, startSelection.Y + j].Value = stringElements[i];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        
        // +----------------------------------------------+
        // | Mouse clicks                                  |
        // +----------------------------------------------+        

        private void dataGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Only keep columns selected
            var selectColumns = dataGrid.SelectedColumns;
            dataGrid.ClearSelection();
            foreach (DataGridViewColumn column in selectColumns)
            {
                column.Selected = true;
            }

            dataGrid.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
            dataGrid.Columns[e.ColumnIndex].Selected = true;
            if (e.Button == MouseButtons.Right)
            {
                rightClickMenuHeaders.MenuItems[0].Enabled = dataGrid.SelectedColumns.Count == 1 ? true : false;
                rightClickMenuHeaders.MenuItems[0].Text = "Insert alternative";
                rightClickMenuHeaders.MenuItems[1].Text = dataGrid.SelectedColumns.Count == 1 ? "Remove alternative" : "Remove alternatives";
                rightClickMenuHeaders.Show(this, dataGrid.PointToClient(Cursor.Position));
            }
        }

        private void dataGrid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Only keep rows selected
            var selectRows = dataGrid.SelectedRows;
            dataGrid.ClearSelection();
            foreach (DataGridViewRow row in selectRows)
            {
                row.Selected = true;
            }

            dataGrid.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            dataGrid.Rows[e.RowIndex].Selected = true;
            if (e.Button == MouseButtons.Right)
            {
                rightClickMenuHeaders.MenuItems[0].Enabled = dataGrid.SelectedRows.Count == 1 ? true : false;
                rightClickMenuHeaders.MenuItems[0].Text = "Insert item";
                rightClickMenuHeaders.MenuItems[1].Text = dataGrid.SelectedRows.Count == 1 ? "Remove item" : "Remove items";
                rightClickMenuHeaders.Show(this, dataGrid.PointToClient(Cursor.Position));
            }
        }

        private void dataGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                dataGrid[e.ColumnIndex, e.RowIndex].Selected = true;
                int max = 0;
                for (int i = 0; i < dataGrid.ColumnCount; i++)
                {
                    for (int j = 0; j < dataGrid.RowCount; j++)
                    {
                        if (dataGrid[i, j].Selected)
                        {
                            max = Math.Max(max, i * j);
                        }
                    }
                }
                rightClickMenuCells.MenuItems[3].Enabled = max > 0 ? true : false;
                rightClickMenuCells.Show(this, dataGrid.PointToClient(Cursor.Position));
            }
        }


        // +----------------------------------------------+
        // | Menus                                         |
        // +----------------------------------------------+        

        private void menuCellsCut(object sender, EventArgs e) 
        {
            Copy(true);
        }

        private void menuCellsCopy(object sender, EventArgs e)
        {
            Copy();
        }

        private void menuCellsPaste(object sender, EventArgs e)
        {
            Paste();
        }

        private void menuHeadersInsert(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count == 1)
            {
                ItemSet.AddItem(new Item(), dataGrid.SelectedRows[0].Index - 1);
                FillGrid();
            }
            else if (dataGrid.SelectedColumns.Count == 1)
            {
                ItemSet.AddAlternative("", dataGrid.SelectedColumns[0].Index - 1);
                FillGrid();
            }
        }

        private void menuHeadersRemove(object sender, EventArgs e)
        {
            if (dataGrid.SelectedRows.Count > 0)
            {
                List<int> removeRowIndex = new List<int>();
                foreach (DataGridViewRow row in dataGrid.SelectedRows)
                {
                    if (row.Index > 0)
                    {
                        removeRowIndex.Add(row.Index - 1);
                    }
                }

                removeRowIndex.Sort();
                for (int i = removeRowIndex.Count - 1; i >= 0; i--)
                {                    
                    ItemSet.RemoveItem(removeRowIndex[i]);
                }
                FillGrid();
            }
            else if (dataGrid.SelectedColumns.Count > 0)
            {
                List<int> removeColumnIndex = new List<int>();
                foreach (DataGridViewColumn column in dataGrid.SelectedColumns)
                {
                    if (column.Index > 0)
                    {
                        removeColumnIndex.Add(column.Index - 1);
                    }
                }

                removeColumnIndex.Sort();
                for (int i = removeColumnIndex.Count - 1; i >= 0; i--)
                {
                    ItemSet.RemoveAlternative(removeColumnIndex[i]);
                }
                FillGrid();
            }
        }

        private void menuCellsSet(object sender, EventArgs e)
        {
            using (var frmSetWeights = new frmSetWeights())
            {
                frmSetWeights.ShowDialog();
                if (frmSetWeights.DialogResult == DialogResult.OK)
                {
                    foreach (DataGridViewCell Cell in dataGrid.SelectedCells)
                    {
                        if (Cell.OwningColumn.Index > 0 && Cell.OwningRow.Index > 0)
                        {
                             Cell.Value = frmSetWeights.Weight.ToString();
                        }
                    }
                }
            }
        }


        // +----------------------------------------------+
        // | Form stuff                                    |
        // +----------------------------------------------+        

        private void dataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (checkInput)
            {
                if (e.ColumnIndex > 0 && e.RowIndex > 0)
                {
                    try
                    {
                        Fraction frac = new Fraction(dataGrid[e.ColumnIndex, e.RowIndex].Value.ToString());
                        ItemSet.Items[e.RowIndex - 1].Weights[e.ColumnIndex - 1] = frac;
                        dataGrid[e.ColumnIndex, e.RowIndex].Value = frac.ToString();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Not a valid entry. Only numerical values are allowed. For fractions, use slash (/).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Logger.LogLow(ex.Message);
                    }
                }
                else if (e.ColumnIndex == 0 && e.RowIndex > 0 && !filling)
                {
                    ItemSet.Items[e.RowIndex - 1].Name = Convert.ToString(dataGrid[e.ColumnIndex, e.RowIndex].Value);
                    comboItemsNaming.SelectedIndex = (int)currentItemsNaming();
                }
                else if (e.ColumnIndex > 0 && e.RowIndex == 0 && !filling)
                {
                    ItemSet.RenameAlternative(e.ColumnIndex - 1, Convert.ToString(dataGrid[e.ColumnIndex, e.RowIndex].Value));
                    comboAlternativesNaming.SelectedIndex = (int)currentAlterativesNaming();
                }
            }
        }
      
        private void numNumItems_ValueChanged(object sender, EventArgs e)
        {
            ItemSet.NumberOfItems = (int)numNumItems.Value;
            FillGrid();
        }

        private void numNumAlternatives_ValueChanged(object sender, EventArgs e)
        {
            ItemSet.NumberOfAlternatives = (int)numNumAlternatives.Value;
            numNumAnswers.Maximum = numNumAlternatives.Value;
            FillGrid();
        }

        private void numNumAnswers_ValueChanged(object sender, EventArgs e)
        {
            ItemSet.NumberOfCheckedAlternativesAllowed = (int)numNumAnswers.Value;
            FillGrid();
        }

        private void comboItemsNaming_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkInput)
            {
                ItemSet.ItemsNaming = (Naming)comboItemsNaming.SelectedIndex;
                FillGrid();
            }
        }

        private void comboAlternativesNaming_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkInput)
            {
                ItemSet.AlternativesNaming = (Naming)comboAlternativesNaming.SelectedIndex;
                FillGrid();
            }
        }

        private void checkScoreIfTooMany_CheckedChanged(object sender, EventArgs e)
        {
            if (checkScoreIfTooMany.Checked)
            {
                txtScoreIfTooMany.Enabled = true;
                ItemSet.ApplyScoreTooManyAlternatives = true;
            }
            else
            {
                txtScoreIfTooMany.Enabled = false;
                ItemSet.ApplyScoreTooManyAlternatives = false;
            }
        }

        private void txtScoreIfTooMany_Leave(object sender, EventArgs e)
        {
            if(Fraction.IsFraction(txtScoreIfTooMany.Text))            
            {
                Fraction frac = new Fraction(txtScoreIfTooMany.Text);
                ItemSet.ScoreIfTooManyAlternativesAreChecked = frac;
            }
            else
            {
                MessageBox.Show("Not a valid entry. Only numerical values are allowed. For fractions, use slash (/).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtScoreIfTooMany.Text = ItemSet.ScoreIfTooManyAlternativesAreChecked.ToString();                
            }
        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            bool errors = false;
            for (int i = 1; i < dataGrid.ColumnCount - 1; i++)
            {
                for (int j = i + 1; j < dataGrid.ColumnCount; j++)
                {
                    if (dataGrid[i, 0].Value.ToString() == dataGrid[j, 0].Value.ToString())
                    {
                        errors = true;
                        break;
                    }
                }
            }
            if (!errors)
            {
                for (int i = 1; i < dataGrid.RowCount - 1; i++)
                {
                    for (int j = i + 1; j < dataGrid.RowCount; j++)
                    {
                        if (dataGrid[0, i].Value.ToString() == dataGrid[0, j].Value.ToString())
                        {
                            errors = true;
                            break;
                        }
                    }
                }
            }

            if (errors)
            {
                MessageBox.Show("Names of items and alternatives need to be unique.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult = DialogResult.OK;
                this.Visible = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Visible = false;
        }


        // +----------------------------------------------+
        // | Other                                         |
        // +----------------------------------------------+        

        private void Delete()
        {
            for (int i = 0; i < dataGrid.SelectedCells.Count; i++)
            {
                Point pnt = new Point(dataGrid.SelectedCells[i].ColumnIndex, dataGrid.SelectedCells[i].RowIndex);

                if (dataGrid.SelectedCells[i].ColumnIndex == 0 && dataGrid.SelectedCells[i].RowIndex > 0)
                {
                    ItemSet.RenameItem(dataGrid.SelectedCells[i].RowIndex - 1, "");
                }
                else if (dataGrid.SelectedCells[i].ColumnIndex > 0 && dataGrid.SelectedCells[i].RowIndex == 0)
                {
                    ItemSet.RenameAlternative(dataGrid.SelectedCells[i].ColumnIndex - 1, "");
                }
                else if (dataGrid.SelectedCells[i].ColumnIndex > 0 && dataGrid.SelectedCells[i].RowIndex > 0)
                {
                    ItemSet.Items[dataGrid.SelectedCells[i].RowIndex - 1].Weights[dataGrid.SelectedCells[i].ColumnIndex - 1] = new Fraction(0);
                }
            }
            FillGrid();
            comboAlternativesNaming.SelectedIndex = (int)currentAlterativesNaming();
            comboItemsNaming.SelectedIndex = (int)currentItemsNaming();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case (Keys.Control | Keys.X):
                    Copy(true);
                    return true;
                case (Keys.Control | Keys.C):
                    Copy();
                    return true;
                case (Keys.Control | Keys.V):
                    Paste();
                    return true;
                case(Keys.Delete):
                    if (dataGrid.SelectedCells.Count > 1)
                    {
                        Delete();
                    }
                    else
                    {
                        return base.ProcessCmdKey(ref msg, keyData);
                    }
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private Naming currentItemsNaming()
        {
            bool numeric = true;
            bool alphabet = true;
            for (int i = 0; i < ItemSet.Items.Count; i++)
            {
                if (Convert.ToString(dataGrid[0, i + 1].Value) != Convert.ToString(i + 1))
                {
                    numeric = false;
                    break;
                }
            }
            for (int i = 0; i < ItemSet.Items.Count; i++)
            {
                if (Convert.ToString(dataGrid[0, i + 1].Value) != StringTools.Alphabet(i + 1))
                {
                    alphabet = false;
                    break;
                }
            }
            if (numeric)
            {
                return Naming.Numeric;
            }
            else if (alphabet)
            {
                return Naming.Alphabetical;
            }
            else
            {
                return Naming.Custom;
            }
        }

        private Naming currentAlterativesNaming()
        {
            bool numeric = true;
            bool alphabet = true;
            for (int i = 0; i < ItemSet.Alternatives.Count; i++)
            {
                if (Convert.ToString(dataGrid[i + 1, 0].Value) != Convert.ToString(i + 1))
                {
                    numeric = false;
                    break;
                }
            }
            for (int i = 0; i < ItemSet.Alternatives.Count; i++)
            {
                if (Convert.ToString(dataGrid[i + 1, 0].Value) != StringTools.Alphabet(i + 1))
                {
                    alphabet = false;
                    break;
                }
            }
            if (numeric)
            {
                return Naming.Numeric;
            }
            else if (alphabet)
            {
                return Naming.Alphabetical;
            }
            else
            {
                return Naming.Custom;
            }
        }


    }
}
