using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Serialization;
using PdfSharp;
using PdfSharp.Drawing;
using MC.Other;
using MC.PaperTools;
using MC.Design;
using MC.Forms;
using MC.IO;
using MC.Python;

namespace MC.Testing
{
    [Serializable]
    public class Test
    {
        [Browsable(false)]
        public int FileVersion { get; set; }

        [CategoryAttribute("General"), Browsable(true), RefreshProperties(RefreshProperties.All)]
        [DisplayName("Name"), Description("Name of the test.")]
        public string Name { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [CategoryAttribute("Layout"), Browsable(true), RefreshProperties(RefreshProperties.All)]
        [DisplayName("Paper"), Description("Template and orientation of the paper.")]
        public Paper Paper { get; set; }

        [CategoryAttribute("General"), Browsable(true), RefreshProperties(RefreshProperties.All)]
        [DisplayName("Bar code gray"), Description("Gray level of the barcode. This should be a value between 1 (nearly total black) and 254 (nearly total black). Only change this value if after printing the bar code appears very light or very dark.")]
        public byte BarCodeGray { get; set; }

        [Browsable(false)]
        public bool BasedOnScript { get; set; }

        [Browsable(false)]
        public PythonCode Code { get; set; }

        [Browsable(false)]
        public double CalibrationMean;

        [Browsable(false)]
        public double CalibrationSD;

        [XmlIgnore]
        public List<Page> Pages = new List<Page>();

        [Browsable(false)]
        public List<SerializablePage> SerializedPages { get; set; }

        [Browsable(false), XmlIgnore]
        public bool HasFocus
        {
            get
            {
                return _hasFocus;
            }
            set
            {
                if (_hasFocus != value)
                {
                    if (value == true)
                    {
                        foreach (var page in Pages)
                        {
                            page.HasFocus = false;
                            foreach (var ctrl in page.Controls)
                            {
                                var element = (TestElement)ctrl;
                                element.HasFocus = false;
                            }
                        }
                        PropertyViewer().Focus(this);
                    }
                    _hasFocus = value;
                }
            }
        }
        private bool _hasFocus;

        public Test()
        {
            Name = "unnamed test";
            BarCodeGray = 128;
            FileVersion = Settings.FileVersion;
            CalibrationMean = Program.UserSettings.calibrationMean;
            CalibrationSD = Program.UserSettings.calibrationSD;
            Code = new PythonCode();
            Paper = new Paper(this);
        }

        public override string ToString()
        {
            return Name;
        }

        public PropertyViewer PropertyViewer()
        {
            if (Pages.Count > 0)
            {
                return Pages[0].PropertyViewer();
            }
            else
            {
                return new PropertyViewer();
            }
        }

        public string GetFreeName()
        {
            return GetFreeName("unknown");
        }

        public string GetFreeName(string name)
        {
            name = StringTools.GetBase(name);
            int outInt = 1;
            var existing = ExistingNames();
            foreach (string str in existing)
            {
                if (str.Contains(name))
                {
                    var spl = str.Split('_');
                    for (int i = 0; i < spl.Count(); i++)
                    {
                        try
                        {
                            Single sing = 0;
                            if (StringTools.IsNumeric(spl[i]))
                            {
                                sing = Convert.ToSingle(spl[i]);
                            }
                            outInt = Math.Max(outInt, (int)Math.Ceiling(sing + 1));
                        }
                        catch (Exception ex)
                        {
                            Logger.LogLow("Error GetFreeName: " + ex.Message);
                        }
                    }
                }
            }
            return name + "_" + Convert.ToString(outInt);
        }

        public List<string> ExistingNames()
        {
            var existing = new List<string>();
            foreach (var page in Pages)
            {
                foreach (var ctrl in page.Controls)
                {
                    existing.Add(ctrl.ToString());
                }
            }
            return existing;
        }

        public void Add(Page page, int pageNumber = -1)
        {
            if (Pages.Count < 256) // Because pagenumber in barcode is represented by a single byte
            {
                if (pageNumber == -1)
                {
                    Pages.Add(page);
                }
                else
                {
                    Pages.Insert(pageNumber, page);
                }
            }
            else
            {
                MessageBox.Show("Maximum number of pages reached.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Remove(Page page)
        {
            Pages.Remove(page);
        }

        public void Remove(int page)
        {
            Pages.RemoveAt(page);
        }

        public void Serialize()
        {
            SerializedPages = new List<SerializablePage>();
            foreach (var page in Pages)
            {
                SerializedPages.Add(new SerializablePage(page));
            }
        }

        public void Deserialize(ctrlDesign designControl)
        {
            designControl.InitializeTest();
            for (int i = 0; i < SerializedPages.Count; i++)
            {
                designControl.InsertPage(i);
                for (int j = 0; j < Program.Test.SerializedPages[i].SerializableTestElements.Count; j++)
                {
                    Program.Test.SerializedPages[i].SerializableTestElements[j].Deserialize(Program.Test.Pages[i]);
                }
            }
            Program.Test.SerializedPages = null;
            Program.Test.Paper.ParentTest = Program.Test; // Is not saved due to circular reference       
        }

        public Test Copy()
        {
            var copy = new Test();
            copy.Name = Name;
            copy.BasedOnScript = BasedOnScript;
            copy.CalibrationMean = CalibrationMean;
            copy.CalibrationSD = CalibrationSD;
            copy.FileVersion = FileVersion;
            Serialize();

            MemoryStream ms;
            var formatter = new BinaryFormatter();

            ms = new MemoryStream();
            formatter.Serialize(ms, Paper);
            ms.Position = 0;
            copy.Paper = (Paper)formatter.Deserialize(ms);

            ms = new MemoryStream();
            formatter.Serialize(ms, Code);
            ms.Position = 0;
            copy.Code = (PythonCode)formatter.Deserialize(ms);

            ms = new MemoryStream();
            formatter.Serialize(ms, SerializedPages);
            ms.Position = 0;
            copy.SerializedPages = (List<SerializablePage>)formatter.Deserialize(ms);

            ms.Dispose();

            return copy;
        }
                
        public Page PageCurrentFocus()
        {
            Page current = null;
            foreach (Page page in Pages)
            {
                if (page.HasFocus)
                {
                    current = page;
                    break;
                }
            }
            return current;
        }

        public TestElement TestElementCurrentFocus()
        {
            TestElement current = null;
            foreach (Page page in Pages)
            {
                foreach (TestElement element in page.Controls)
                {
                    if (element.HasFocus)
                    {
                        current = element;
                        break;
                    }
                }
            }
            return current;
        }

        public ItemSet ItemSetByName(string name)
        {
            foreach (Page page in Pages)
            {
                foreach (TestElement element in page.Controls)
                {
                    if (element.GetType() == typeof(ItemSet))
                    {
                        var itemSet = (ItemSet)element;
                        if (itemSet.Name == name)
                        {
                            return itemSet;
                        }                       
                    }
                }
            }
            return null;
        }
    }
}
