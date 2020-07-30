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
using IronPython;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using MC.Other;
using MC.Testing;
using MC.PaperTools;
using MC.Design;
using MC.IO;
using MC.Python;

namespace MC.Forms
{
    public partial class ctrlResults
    {
        private void DrawGrid()
        {
            dataGrid.Rows.Clear();
            dataGrid.Columns.Clear();

            dataGrid.Columns.Add("Subject", "Subject");
            dataGrid.Columns.Add("Pages", "Pages");
            dataGrid.Columns[1].Width = baseWidth;
            
            if (Program.Test.BasedOnScript)
            {
                DrawGridScript();
            }
            else
            {
                DrawGridNonScript();
            }
            dataGrid.ClearSelection();
      }

        private void DrawGridNonScript()
        {
            if (checkTotal.Checked)
            {
                dataGrid.Columns.Add("Total", "Total");
                dataGrid.Columns[2].Width = baseWidth;
            }

            // Create columns
            int setCount = 0;
            for (int i = 0; i < Program.Test.Pages.Count; i++)
            {
                foreach (var element in Program.Test.Pages[i].Controls)
                {
                    if (element.GetType() == typeof(ItemSet))
                    {
                        var set = (ItemSet)element;
                        if (set.Role != ItemSetRole.Identification)
                        {
                            setCount++;
                            if (checkSetTotals.Checked)
                            {
                                dataGrid.Columns.Add(set.Name, set.Name);
                                dataGrid.Columns[dataGrid.Columns.Count - 1].Width = baseWidth;
                                if (setCount % 2 == 0)
                                {
                                    dataGrid.Columns[dataGrid.Columns.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(200, 200, 230);
                                }
                                else
                                {
                                    dataGrid.Columns[dataGrid.Columns.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(200, 230, 200);
                                }
                            }
                            if (checkItems.Checked)
                            {
                                for (int j = 0; j < set.Items.Count; j++)
                                {
                                    dataGrid.Columns.Add(set.Name + " Item " + set.Items[j].Name, set.Name + " Item " + set.Items[j].Name);
                                    dataGrid.Columns[dataGrid.Columns.Count - 1].Width = 60;
                                    if (setCount % 2 == 0)
                                    {
                                        dataGrid.Columns[dataGrid.Columns.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(200, 230, 255);
                                    }
                                    else
                                    {
                                        dataGrid.Columns[dataGrid.Columns.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 230);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Create rows
            for (int i = 0; i < CorrectedSubjects.Count; i++)
            {
                Fraction total = new Fraction();
                dataGrid.Rows.Add(1);
                int row = dataGrid.RowCount - 1;
                dataGrid.Rows[row].Cells[0].Value = CorrectedSubjects[i].Id;
                dataGrid.Rows[row].Cells[1].Value = CorrectedSubjects[i].PageCount();
                bool errorAnalyse = false;

                // Not enough pages (or too many):
                if (CorrectedSubjects[i].CorrectedPages.Count != Program.Test.Pages.Count)
                {
                    errorAnalyse = true;
                }
                else
                {
                    // Some error on page level?
                    for (int j = 0; j < CorrectedSubjects[i].CorrectedPages.Count; j++)
                    {
                        if (!CorrectedSubjects[i].CorrectedPages[j].Status.Analyzed || CorrectedSubjects[i].CorrectedPages[j].Status.BarCodeError
                            || CorrectedSubjects[i].CorrectedPages[j].Status.CalibrationError || CorrectedSubjects[i].CorrectedPages[j].Status.ContainsDoubts
                            || CorrectedSubjects[i].CorrectedPages[j].Status.PageNumberOrHashError)
                        {   // Error "too many checked" is allowed
                            errorAnalyse = true;
                        }
                    }
                }

                for (int j = 0; j < CorrectedSubjects[i].ItemCollections.Count; j++)
                {
                    if (CorrectedSubjects[i].ItemCollections[j].Role != ItemSetRole.Identification)
                    {
                        if (checkSetTotals.Checked)
                        {
                            int col = ColumnByName(CorrectedSubjects[i].ItemCollections[j].Name);
                            if (col != -1)
                            {
                                dataGrid.Rows[row].Cells[col].Value = Math.Round(CorrectedSubjects[i].ItemCollections[j].Score().ToDecimal(), Program.UserSettings.numberOfDecimalsInResults);
                            }
                        }
                        if (checkItems.Checked)
                        {
                            int col = ColumnByName(CorrectedSubjects[i].ItemCollections[j].Name + " Item " + CorrectedSubjects[i].ItemCollections[j].Items[0].Name);
                            if (col != -1)
                            {
                                for (int k = 0; k < CorrectedSubjects[i].ItemCollections[j].Items.Count; k++)
                                {
                                    dataGrid.Rows[row].Cells[col + k].Value = Math.Round(CorrectedSubjects[i].ItemCollections[j].Score(k).ToDecimal(), Program.UserSettings.numberOfDecimalsInResults);
                                }
                            }
                        }
                    }
                    if (CorrectedSubjects[i].ItemCollections[j].Role == ItemSetRole.Question)
                    {
                        // Itemsets with identification or version role do not count for total
                        total += CorrectedSubjects[i].ItemCollections[j].Score();
                    }
                }
                if (errorAnalyse)
                {
                    dataGrid.Rows[row].DefaultCellStyle.BackColor = Color.Red;
                }
                else if (checkTotal.Checked)
                {
                    dataGrid.Rows[row].Cells[2].Value = total.ToDecimal();
                }
            }
        }

        private void DrawGridScript()
        {
            if (CorrectedSubjects != null)
            {
                if (Program.Test.Code != null)
                {
                    // Create columns
                    if (Program.Test.Code.OutputVariables != null)
                    {
                        for (int i = 0; i < Program.Test.Code.OutputVariables.Count; i++)
                        {
                            dataGrid.Columns.Add(Program.Test.Code.OutputVariables[i].Name, Program.Test.Code.OutputVariables[i].Name);
                        }
                    }

                    var interp = new PythonInterpreter();
                    try
                    {                        
                        interp.Initialize(Program.Test.Code);
                        Fraction total = new Fraction();
                        // Create rows
                        for (int i = 0; i < CorrectedSubjects.Count; i++)
                        {
                            dataGrid.Rows.Add(1);
                            int row = dataGrid.RowCount - 1;
                            bool errorAnalyse = false;

                            // Not enough pages (or too many):
                            if (CorrectedSubjects[i].CorrectedPages.Count != Program.Test.Pages.Count)
                            {
                                errorAnalyse = true;
                            }
                            else
                            {
                                // Some error on page level?
                                for (int j = 0; j < CorrectedSubjects[i].CorrectedPages.Count; j++)
                                {
                                    if (!CorrectedSubjects[i].CorrectedPages[j].Status.Analyzed || CorrectedSubjects[i].CorrectedPages[j].Status.BarCodeError
                                        || CorrectedSubjects[i].CorrectedPages[j].Status.CalibrationError || CorrectedSubjects[i].CorrectedPages[j].Status.ContainsDoubts
                                        || CorrectedSubjects[i].CorrectedPages[j].Status.PageNumberOrHashError)
                                    {   // Error "too many checked" is allowed
                                        errorAnalyse = true;                                        
                                        break;
                                    }
                                }
                            }

                            if (errorAnalyse)
                            {
                                dataGrid.Rows[row].DefaultCellStyle.BackColor = Color.Red;
                            }

                            dataGrid.Rows[row].Cells[0].Value = CorrectedSubjects[i].Id;
                            dataGrid.Rows[row].Cells[1].Value = CorrectedSubjects[i].PageCount();
                            if (!errorAnalyse)
                            {
                                var outputVars = interp.Score(CorrectedSubjects[i]);
                                currentNumberOfOutputVars = outputVars.Count;
                                for (int j = 0; j < outputVars.Count; j++)
                                {
                                    if (outputVars[j].Value != null)
                                    {
                                        dataGrid.Rows[i].Cells[outputVars[j].Name].Value = Math.Round(outputVars[j].Value.ToDecimal(), Program.UserSettings.numberOfDecimalsInResults);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in script:\r\n" + interp.engine.GetService<ExceptionOperations>().FormatException(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }                
                }
            }
        }
    }
}
