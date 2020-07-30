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
    public partial class ctrlResults
    {
        private void ExportNoScript(string fileName)
        {
            try
            {
                using (var writer = new StreamWriter(fileName, false))
                {
                    var str = new StringBuilder();
                    str.Append("Files;Subject;Pages;Error");
                    if (checkTotal.Checked)
                    {
                        str.Append(";Total");
                    }
                    var colNameList = new List<string>();

                    for (int i = 0; i < Program.Test.Pages.Count; i++)
                    {
                        foreach (var element in Program.Test.Pages[i].Controls)
                        {
                            if (element.GetType() == typeof(ItemSet))
                            {
                                var set = (ItemSet)element;
                                if (set.Role != ItemSetRole.Identification)
                                {
                                    if (checkInclSetTotals.Checked)
                                    {
                                        str.Append(";" + set.Name);
                                        colNameList.Add(set.Name);
                                    }
                                    if (checkInclItems.Checked)
                                    {
                                        for (int j = 0; j < set.Items.Count; j++)
                                        {
                                            str.Append(";" + set.Name + "_" + set.Items[j].Name);
                                            colNameList.Add(set.Name + "_" + set.Items[j].Name);
                                        }
                                    }
                                    if (checkInclItemAlts.Checked)
                                    {
                                        for (int j = 0; j < set.Items.Count; j++)
                                        {
                                            for (int k = 0; k < set.Alternatives.Count; k++)
                                            {
                                                str.Append(";" + set.Name + "_" + set.Items[j].Name + "_" + set.Alternatives[k]);
                                                colNameList.Add(set.Name + "_" + set.Items[j].Name + "_" + set.Alternatives[k]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    writer.WriteLine(str.ToString());

                    for (int i = 0; i < CorrectedSubjects.Count; i++)
                    {
                        Fraction total = new Fraction();
                        string[] values = new string[colNameList.Count];
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
                                if (checkInclSetTotals.Checked)
                                {
                                    int col = colNameList.IndexOf(CorrectedSubjects[i].ItemCollections[j].Name);
                                    if (col != -1)
                                    {
                                        values[col] = Convert.ToString(Math.Round(CorrectedSubjects[i].ItemCollections[j].Score().ToDecimal(), Program.UserSettings.numberOfDecimalsInResults));
                                    }
                                }
                                if (checkInclItems.Checked)
                                {
                                    int col = colNameList.IndexOf(CorrectedSubjects[i].ItemCollections[j].Name + "_" +
                                                                  CorrectedSubjects[i].ItemCollections[j].Items[0].Name);
                                    if (col != -1)
                                    {
                                        for (int k = 0; k < CorrectedSubjects[i].ItemCollections[j].Items.Count; k++)
                                        {
                                            values[col + k] = Convert.ToString(Math.Round(CorrectedSubjects[i].ItemCollections[j].Score(k).ToDecimal(), Program.UserSettings.numberOfDecimalsInResults));
                                        }
                                    }
                                }
                                if (checkInclItemAlts.Checked)
                                {
                                    for (int k = 0; k < CorrectedSubjects[i].ItemCollections[j].Items.Count; k++)
                                    {
                                        int col = colNameList.IndexOf(CorrectedSubjects[i].ItemCollections[j].Name + "_" +
                                                                      CorrectedSubjects[i].ItemCollections[j].Items[k].Name + "_" +
                                                                      CorrectedSubjects[i].ItemCollections[j].Alternatives[0]);
                                        if (col != -1)
                                        {
                                            for (int l = 0; l < CorrectedSubjects[i].ItemCollections[j].Alternatives.Count; l++)
                                            {
                                                values[col + l] = CorrectedSubjects[i].ItemCollections[j].Items[k].Checked[l] == ItemCheckedState.Checked ? "1" : "0";
                                            }
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
                        str.Clear();
                        for (int j = 0; j < CorrectedSubjects[i].CorrectedPages.Count; j++)
                        {
                            str.Append(CorrectedSubjects[i].CorrectedPages[j].FileName);
                            if (j < CorrectedSubjects[i].CorrectedPages.Count - 1)
                            {
                                str.Append(", ");
                            }
                        }
                        str.Append(";" + CorrectedSubjects[i].Id);
                        str.Append(";" + CorrectedSubjects[i].PageCount());
                        str.Append(";" + (errorAnalyse || CorrectedSubjects[i].HasError()).ToString());
                        if (checkInclTotal.Checked)
                        {
                            str.Append(";");
                            if (!errorAnalyse)
                            {
                                str.Append(Math.Round(total.ToDecimal(), Program.UserSettings.numberOfDecimalsInResults));
                            }
                        }
                        for (int j = 0; j < values.Count(); j++)
                        {
                            str.Append(";" + values[j]);
                        }
                        if (radLocale.Checked)
                        {
                            writer.WriteLine(str.ToString());
                        }
                        else if (radPoint.Checked)
                        {
                            writer.WriteLine(str.ToString().Replace(',', '.'));
                        }
                        else if (radComma.Checked)
                        {
                            writer.WriteLine(str.ToString().Replace('.', ','));
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong while trying to save " + fileName + ": " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportScript(string fileName)
        {
            try
            {
                using (var writer = new StreamWriter(fileName, false))
                {
                    var str = new StringBuilder();
                    str.Append("Files;Subject;Pages;Error");
                    for (int j = 2; j < dataGrid.Columns.Count; j++)
                    {
                        str.Append(";" + dataGrid.Columns[j].Name.ToString());
                    }

                    var colNameList = new List<string>();
                    if (checkInclItemAlts.Checked)
                    {
                        for (int i = 0; i < Program.Test.Pages.Count; i++)
                        {
                            foreach (var element in Program.Test.Pages[i].Controls)
                            {
                                if (element.GetType() == typeof(ItemSet))
                                {
                                    var set = (ItemSet)element;
                                    if (set.Role != ItemSetRole.Identification)
                                    {
                                        for (int j = 0; j < set.Items.Count; j++)
                                        {
                                            for (int k = 0; k < set.Alternatives.Count; k++)
                                            {
                                                str.Append(";" + set.Name + "_" + set.Items[j].Name + "_" + set.Alternatives[k]);
                                                colNameList.Add(set.Name + "_" + set.Items[j].Name + "_" + set.Alternatives[k]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    writer.WriteLine(str.ToString());

                    for (int i = 0; i < CorrectedSubjects.Count; i++)
                    {
                        string[] values = new string[colNameList.Count];
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
                        if (checkInclItemAlts.Checked)
                        {
                            for (int j = 0; j < CorrectedSubjects[i].ItemCollections.Count; j++)
                            {
                                for (int k = 0; k < CorrectedSubjects[i].ItemCollections[j].Items.Count; k++)
                                {
                                    int col = colNameList.IndexOf(CorrectedSubjects[i].ItemCollections[j].Name + "_" +
                                                                  CorrectedSubjects[i].ItemCollections[j].Items[k].Name + "_" +
                                                                  CorrectedSubjects[i].ItemCollections[j].Alternatives[0]);                                    
                                    if (col != -1)
                                    {
                                        for (int l = 0; l < CorrectedSubjects[i].ItemCollections[j].Alternatives.Count; l++)
                                        {
                                            values[col + l] = CorrectedSubjects[i].ItemCollections[j].Items[k].Checked[l] == ItemCheckedState.Checked ? "1" : "0";
                                        }
                                    }
                                }
                            }
                        }

                        str.Clear();
                        for (int j = 0; j < CorrectedSubjects[i].CorrectedPages.Count; j++)
                        {
                            str.Append(CorrectedSubjects[i].CorrectedPages[j].FileName);
                            if (j < CorrectedSubjects[i].CorrectedPages.Count - 1)
                            {
                                str.Append(", ");
                            }
                        }
                        str.Append(";" + CorrectedSubjects[i].Id);
                        str.Append(";" + CorrectedSubjects[i].PageCount());
                        str.Append(";" + (errorAnalyse || CorrectedSubjects[i].HasError()).ToString());
                        for (int j = 0; j < currentNumberOfOutputVars; j++) // Variables calculated by script
                        {
                            str.Append(";" + dataGrid.Rows[i].Cells[j + 2].Value);
                        }
                        for (int j = 0; j < values.Count(); j++)
                        {
                            str.Append(";" + values[j]);
                        }
                        if (radLocale.Checked)
                        {
                            writer.WriteLine(str.ToString());
                        }
                        else if (radPoint.Checked)
                        {
                            writer.WriteLine(str.ToString().Replace(',', '.'));
                        }
                        else if (radComma.Checked)
                        {
                            writer.WriteLine(str.ToString().Replace('.', ','));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong while trying to save " + fileName + ": " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
