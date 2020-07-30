using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MC.Other;
using MC.Design;
using MC.Testing;
using MC.Python;
using FastColoredTextBoxNS;

namespace MC.Forms
{
    public partial class frmScript : Form
    {
        Style KeywordsStyle = new TextStyle(Brushes.Blue, null, FontStyle.Bold);
        Style OutputStyle = new TextStyle(Brushes.Goldenrod, null, FontStyle.Regular);
        Style CommentStyle = new TextStyle(Brushes.Green, null, FontStyle.Regular);
        string outputVariables = "";
        List<string> inputVariables;
       
        public frmScript()
        {            
            InitializeComponent();

            btnFromCurrent.Image = Settings.CreateScript;
            btnUndo.Image = Settings.Undo;
            btnRedo.Image = Settings.Redo;            

            if (Program.Test.Code != null)
            {
                if (Program.Test.Code.OutputVariables != null)
                {
                    scriptBox.Text = Program.Test.Code.Script;
                    for (int i = 0; i < Program.Test.Code.OutputVariables.Count; i++)
                    {
                        outputBox.AppendText(Program.Test.Code.OutputVariables[i].Name + "\r\n");
                    }
                }
            }
            FillVarList();
            SetForm();
        }
        
        private void scriptBox_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            UpdateScriptStyle();
        }

        private void UpdateScriptStyle()
        {
            var range = scriptBox.Range;
            range.ClearStyle(KeywordsStyle, OutputStyle, CommentStyle);
            range.SetStyle(CommentStyle, @"#.*$", RegexOptions.Multiline);
            range.SetStyle(OutputStyle, outputVariables);
            range.SetStyle(KeywordsStyle, StringTools.PythonReserved, RegexOptions.IgnoreCase);
        }

        public string GetEditedScript()
        {
            return scriptBox.Text;
        }

        private void frmScript_SizeChanged(object sender, EventArgs e)
        {
            SetForm();
        }

        private void SetForm()
        {
            btnOk.Top = this.ClientSize.Height - btnOk.Height - 5;
            btnCancel.Top = this.ClientSize.Height - btnCancel.Height - 5;
            btnOk.Left = (this.ClientSize.Width / 2) - (btnOk.Width + 3);
            btnCancel.Left = (this.ClientSize.Width / 2) + 3;
            varTree.Height = (scriptBox.Height - 6) / 2;
            outputBox.Height = scriptBox.Height - varTree.Height - 6;
            outputBox.Top = varTree.Bottom + 6;
            scriptBox.Height = btnOk.Top - scriptBox.Top - 5;
            btnApply.Top = varTree.Top + ((varTree.Height - btnApply.Height) / 2);
        }     

        private void btnFromCurrent_Click(object sender, EventArgs e)
        {
            bool delete = true;
            if(scriptBox.Text != "")
            {
                if (MessageBox.Show("Are you sure you want to delete the current script?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    delete = false;
                }
            }
            if (delete)
            {
                outputBox.Text = "Total";
                scriptBox.Text = "";
                scriptBox.AppendText("def scoreItemset(name, weights):\r\n");
                scriptBox.AppendText("    score = 0\r\n");
                scriptBox.AppendText("    for i in range(len(weights)):\r\n");
                scriptBox.AppendText("        for a in range(len(weights[0])):\r\n");
                scriptBox.AppendText("            score += name[i][a] * weights[i][a]\r\n");
                scriptBox.AppendText("    return score\r\n");
                scriptBox.AppendText("\r\n");
                for (int i = 0; i < varTree.Nodes.Count; i++)
                {
                    if (varTree.Nodes[i].Name == "Question")
                    {                        
                        for (int j = 0; j < varTree.Nodes[i].Nodes.Count; j++)
                        {
                            scriptBox.AppendText("Total += scoreItemset(" + varTree.Nodes[i].Nodes[j].Name + ", [\\\r\n");
                            for (int k = 0; k < varTree.Nodes[i].Nodes[j].Nodes.Count; k++)
                            {
                                scriptBox.AppendText("    [");
                                for (int l = 0; l < varTree.Nodes[i].Nodes[j].Nodes[k].Nodes.Count; l++)
                                {
                                    scriptBox.AppendText(Program.Test.ItemSetByName(varTree.Nodes[i].Nodes[j].Name).Items[k].Weights[l].ToPython());
                                    if (l < varTree.Nodes[i].Nodes[j].Nodes[k].Nodes.Count - 1)
                                    {
                                        scriptBox.AppendText(", ");
                                    }
                                }
                                if (k < varTree.Nodes[i].Nodes[j].Nodes.Count - 1)
                                {
                                    scriptBox.AppendText("],\\\r\n");
                                }
                                else
                                {
                                    scriptBox.AppendText("]])\r\n\r\n");
                                }
                            }
                        }
                    }
                }
            }

         
        }

        private void FillVarList()
        {
            inputVariables = new List<string>();
            varTree.Nodes.Clear();

            FillVarList("Identification", ItemSetRole.Identification);
            FillVarList("Version", ItemSetRole.Version);
            FillVarList("Question", ItemSetRole.Question);

            varTree.Sort();

            outputBox.Clear();
            if (Program.Test.Code != null)
            {
                if (Program.Test.Code.OutputVariables != null)
                {
                    for (int i = 0; i < Program.Test.Code.OutputVariables.Count; i++)
                    {
                        outputBox.AppendText(Program.Test.Code.OutputVariables[i].Name + "\r\n");
                    }
                }
            }
        }

        private void FillVarList(string description, ItemSetRole role)
        {
            varTree.Nodes.Add(description, description);
            foreach (var page in Program.Test.Pages)
            {
                foreach (var ctrl in page.Controls)
                {
                    if (ctrl.GetType() == typeof(ItemSet))
                    {
                        var itemSet = (ItemSet)ctrl;
                        if (itemSet.Role == role)
                        {
                            varTree.Nodes[description].Nodes.Add(itemSet.Name, itemSet.Name);
                            inputVariables.Add(itemSet.Name);
                            for (int i = 0; i < itemSet.Items.Count; i++)
                            {
                                // Lowest 2 levels not by name but by number ==> arrays (0-based to comform with Python)                                
                                string itemName = itemSet.Name + "[" + i + "]";
                                varTree.Nodes[description].Nodes[itemSet.Name].Nodes.Add(itemName, itemName);
                                varTree.Nodes[description].Nodes[itemSet.Name].Nodes[itemName].ToolTipText = itemSet.Items[i].Name;
                                for (int j = 0; j < itemSet.Items[i].Weights.Length; j++)
                                {
                                    string altName = itemName + "[" + j + "]";
                                    varTree.Nodes[description].Nodes[itemSet.Name].Nodes[itemName].Nodes.Add(altName, altName);
                                    varTree.Nodes[description].Nodes[itemSet.Name].Nodes[itemName].Nodes[altName].ToolTipText = itemSet.Alternatives[j];
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var outVars = new List<PythonVariable>();
            for(int i = 0; i < outputBox.Lines.Count(); i++)
            {
                string str = outputBox.Lines[i].Trim();
                if(str.Length > 0)
                {
                    str = str.Replace(' ', '_');
                    outVars.Add(new PythonVariable(str));
                }
            }
            Program.Test.Code = new PythonCode(scriptBox.Text, outVars);
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void outputBox_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {            
            var outList = new List<string>();
            bool error = false;
            for (int i = 0; i < outputBox.Lines.Count; i++)
            {
                if (outputBox.Lines[i].Length > 0)
                {
                    var str = outputBox.Lines[i].Trim();
                    var split = str.Split(StringTools.ForbiddenChars);
                    for (int j = 0; j < split.Count(); j++)
                    {
                        for (int k = 0; k < StringTools.PythonReservedList.Count; k++)
                        {
                            if (split[j] == StringTools.PythonReservedList[k])
                            {
                                Logger.LogHigh(StringTools.PythonReservedList[k]);
                                error = true;
                                break;
                            }
                            else
                            {
                                if (char.IsNumber(split[j].FirstOrDefault()))
                                {
                                    error = true;
                                    break;
                                }   
                            }
                        }
                        if(outList.Contains(split[j]))
                        {
                            error = true;
                            break;
                        }
                        if (!error)
                        {
                            outList.Add(split[j]);
                        }
                    }
                    // Not an input variable?
                    for (int k = 0; k < inputVariables.Count; k++)
                    {
                        if (str == inputVariables[k])
                        {
                            error = true;
                            break;
                        }
                    }
                }
            }
            StringBuilder strB = new StringBuilder();
            strB.Append(@"\b(");
            for (int i = 0; i < outList.Count - 1; i++)
            {
                strB.Append(outList[i] + "|");
            }
            if(outList.Count > 0)
            {
                strB.Append(outList[outList.Count - 1]);
            }
            strB.Append(@")\b");
            outputVariables = strB.ToString();
            if (error)
            {
                MessageBox.Show("One or more variable names are not valid: variable name must be unique (so also differ from input variables), start with a letter or underscore, and may not be a reserved keyword.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            UpdateScriptStyle();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (varTree.SelectedNode != null)
            {
                if (varTree.SelectedNode.Level > 0)
                {
                    scriptBox.InsertText(varTree.SelectedNode.Name);
                }
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {            
            scriptBox.Undo();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            scriptBox.Redo();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {            
            base.OnPaintBackground(e);
            using (Pen p = new Pen(Color.FromKnownColor(KnownColor.ActiveBorder)))
            {
                e.Graphics.DrawRectangle(p, new Rectangle(varTree.Left - 1, varTree.Top - 1,
                                   varTree.Width + 1, varTree.Height + 1));
                e.Graphics.DrawRectangle(p,  new Rectangle(outputBox.Left - 1, outputBox.Top - 1,
                                   outputBox.Width + 1, outputBox.Height + 1));
                e.Graphics.DrawRectangle(p, new Rectangle(scriptBox.Left - 1, scriptBox.Top - 1,
                                   scriptBox.Width + 1, scriptBox.Height + 1));
            }
        }
        

    }
}
