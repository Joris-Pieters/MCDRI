using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Xml.Serialization;
using System.Reflection;
using MC;
using MC.Other;
using MC.Testing;
using MC.PaperTools;
using MC.Design;
using MC.IO;

namespace MC.Forms
{
    public partial class frmMain : Form
    {      
        string openFileName = "";

        public frmMain(string testOnLoading = "")
        {
            Settings.SetIcons();            
            Program.UndoManager = new UndoManager();
            InitializeComponent();

            menuStrip.Renderer = new CustomToolStripRenderer();

            List<CorrectedPage> CorrectedPages = new List<CorrectedPage>();
            ImportControl.CorrectedPages = CorrectedPages;
            ResultsControl.CorrectedPages = CorrectedPages;

            DesignControl.Dock = DockStyle.Fill;
            ImportControl.Dock = DockStyle.Fill;
            ResultsControl.Dock = DockStyle.Fill;

            menuViewDesign.Checked = true;
            ViewDesign();

            // Do anyway, even if test is loaded afterwards in case something goes wrong loading that test.
            NewTest();

            if (testOnLoading != "")
            {
                LoadTest(testOnLoading);
            }            
        }

        private void Setup()
        {
            SetFormText();
            ImportControl.Clear();
            ResultsControl.Clear();
            ViewDesign();
        }

        private void SaveUserSettings()
        {
            try
            {
                using (var writer = new StreamWriter(Settings.UserSettingsFile))
                {
                    var xmlSerial = new XmlSerializer(typeof(UserSettings));
                    xmlSerial.Serialize(writer, Program.UserSettings);
                }              
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving user settings. " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NewTest()
        {
            openFileName = "";
            Program.Test = new Test();
            Program.Test.Pages.Add(new Page(Program.Test));
            DesignControl.InitializeTest();
            Setup();    
        }

        void LoadTest(string fileName)
        {
            try
            {
                DesignControl.Lock(true);
                Program.Test.Pages = new List<Page>();
                DesignControl.InitializeTest();
                using (var fs = new FileStream(fileName, FileMode.Open))
                {
                    using (var archive = new GZipStream(fs, CompressionMode.Decompress))
                    {
                        var xmlSerial = new XmlSerializer(typeof(Test));
                        Program.Test = (Test)xmlSerial.Deserialize(archive);                        
                    }
                }
                Program.Test.Deserialize(DesignControl);
                openFileName = fileName;
                Setup();
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show("Error trying to open " + fileName + ": " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }
                 finally
                 {
                DesignControl.Lock(false);
                DesignControl.PropertyFocus(Program.Test);
            }
        }

        private void SaveTest(string fileName)
        {
            try
            {                
                Program.Test.Serialize();
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    using (var archive = new GZipStream(fs, CompressionMode.Compress))
                    {
                        var xmlSerial = new XmlSerializer(typeof(Test));
                        xmlSerial.Serialize(archive, Program.Test);
                    }
                }
                Program.Test.SerializedPages = null;
                openFileName = fileName;
                SetFormText();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error trying to save " + fileName + ": " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }         
        }

        public Test CopyTest(Test original)
        {
            Test copy = new Test();
            original.Serialize();
            using (var ms = new MemoryStream())
            {
                var xmlSerial = new XmlSerializer(typeof(Test));
                xmlSerial.Serialize(ms, original);
                copy = (Test)xmlSerial.Deserialize(ms);
                for (int i = 0; i < copy.SerializedPages.Count; i++)
                {
                    for (int j = 0; j < copy.SerializedPages[i].SerializableTestElements.Count; j++)
                    {
                        copy.SerializedPages[i].SerializableTestElements[j].Deserialize(Program.Test.Pages[i]);
                    }
                }
                copy.SerializedPages = null;
                copy.Paper.ParentTest = Program.Test; // Is not saved due to circular reference  
            }
            original.SerializedPages = null;
            return copy;
        }

        private void SetFormText()
        {
            this.Text = Settings.SoftwareName();
            if (openFileName != "")
            {
                var split = openFileName.Split('\\');
                this.Text += " - " + split[split.Count() - 1];
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit? Unsaved changes will be lost.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                e.Cancel = true;
            } 
            SaveUserSettings();
            Logger.Stop();
        }

        private bool CurrentIsEmpty()
        {
            return Program.Test.Pages.Count == 1 && Program.Test.Pages[0].TestElements.Count == 0 ? true : false;
        }
        
        private void ViewDesign()
        {
            if (Program.UserSettings.showWarningEditAnalysed)
            {
                try
                {
                    foreach (var ctrl in this.Controls)
                    {
                        if (ctrl.GetType() == typeof(ctrlImport))
                        {
                            ctrlImport ctrlImp = (ctrlImport)ctrl;
                            if (ctrlImp.ContainsAnalysedPages())
                            {
                                MessageBox.Show("One or more pages have already been analysed. If you make changes to the current test the obtained results may not be valid anymore and require re-analysation.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogLow("Error switching to Design: " + ex.Message);
                }
            }

            DesignControl.Visible = true;
            ImportControl.Visible = false;
            ResultsControl.Visible = false;

            menuViewDesign.Checked = true;
            menuViewImport.Checked = false;
            menuViewResults.Checked = false;
        }

        private void ViewImport()
        {
            DesignControl.Visible = false;
            ImportControl.Visible = true;
            ResultsControl.Visible = false;

            menuViewDesign.Checked = false;
            menuViewImport.Checked = true;
            menuViewResults.Checked = false;
        }

        private void ViewResults()
        {
            DesignControl.Visible = false;
            ImportControl.Visible = false;
            ResultsControl.Visible = true;

            menuViewDesign.Checked = false;
            menuViewImport.Checked = false;
            menuViewResults.Checked = true;

            ResultsControl.UpdateGrid();
        }

        #region menu

        private void menuFileNew_Click(object sender, EventArgs e)
        {            
            if (MessageBox.Show("Are you sure you want to start a new test? Unsaved changes will be lost.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                NewTest();
            }
        }

        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            if (CurrentIsEmpty() || MessageBox.Show("Are you sure you want to open a test? Unsaved changes will be lost.", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var dlg = new OpenFileDialog();
                dlg.DefaultExt = ".test";
                dlg.Filter = "test files (*.test)|*.test";
                dlg.InitialDirectory = Program.UserSettings.currentDirectory;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.FileName != "")
                    {
                        LoadTest(dlg.FileName);
                        ViewDesign();
                        Program.UserSettings.currentDirectory = Path.GetDirectoryName(dlg.FileName);
                    }
                }                
            }
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {
            if (openFileName == "")
            {
                var dlg = new SaveFileDialog();
                dlg.FileName = Program.Test.Name;
                dlg.DefaultExt = ".test";
                dlg.Filter = "test files (*.test)|*.test";
                dlg.InitialDirectory = Program.UserSettings.currentDirectory;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.FileName != "")
                    {
                        SaveTest(dlg.FileName);
                        Program.UserSettings.currentDirectory = Path.GetDirectoryName(dlg.FileName);
                    }
                }
            }
            else
            {
                SaveTest(openFileName);
            }
        }

        private void menuFileSaveAs_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            if (openFileName == "")
            {
                dlg.FileName = Program.Test.Name;
            }
            else
            {
                dlg.FileName = openFileName;
            }
            dlg.DefaultExt = ".test";
            dlg.Filter = "test files (*.test)|*.test";
            dlg.InitialDirectory = Program.UserSettings.currentDirectory;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.FileName != "")
                {
                    SaveTest(dlg.FileName);
                    Program.UserSettings.currentDirectory = Path.GetDirectoryName(dlg.FileName);
                }
            }
        }

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuViewDesign_Click(object sender, EventArgs e)
        {
            ViewDesign();
        }

        private void menuViewImport_Click(object sender, EventArgs e)
        {
            ViewImport();
        }

        private void menuViewAnalyze_Click(object sender, EventArgs e)
        {
            ViewResults();
        }

        private void menuManual_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Settings.ManualFile);
            }
            catch(Exception ex)
            {
                Logger.LogLow(ex.Message);
                MessageBox.Show("Error loading " + Settings.ManualFile + ". The file was not found on your system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Settings.SoftwareName() + "\r\n\r\n" + "Joris Pieters - 2014", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

       


    }
}

