using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using MC.Other;
using MC.PaperTools;
using MC.Design;
using MC.Testing;

namespace MC.Forms
{
    public partial class PropertyViewer : UserControl
    {
        public List<object> Content;
        public bool OpeningFile; // Prevent updating before file is loaded completely
        
        public PropertyViewer()
        {
            InitializeComponent();
            Content = new List<object>();
        }

        public void Focus(object o)
        {
            comboBox.SelectedItem = o.ToString();
            propertyGrid.SelectedObject = o;
        }       

        public void Focus(string str)
        {
            foreach (object o in Content)
            {
                if (o.ToString() == str)
                {
                    Focus(o);                    
                }
            }
        }

        public void Add(object o)
        {
            if (Content == null)
            {
                Content = new List<object>();
            }
            Content.Add(o);

            if (!OpeningFile)
            {
                Sort();
                comboBox.Items.Clear();
                for (int i = 0; i < Content.Count; i++)
                {
                    var str = Content[i].ToString();
                    if (str != null)
                    {
                        comboBox.Items.Add(str);
                    }
                }
                Focus(o);
            }
        }

        public void Clear()
        {
            Content.Clear();
            comboBox.Items.Clear();
            propertyGrid.SelectedObject = null;
        }

        /// <summary>
        /// Remove selected object
        /// </summary>
        public void Remove()
        {
            foreach (object o in Content)
            {
                try
                {
                    var element = (TestElement)o;
                    if (element.HasFocus)
                    {
                        Remove(element);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogLow(ex.Message);
                }
            }
            Refresh();
        }

        /// <summary>
        /// Remove object
        /// </summary>
        /// <param name="o"></param>
        public void Remove(object o)
        {
            comboBox.Items.Remove(o.ToString());
            Content.Remove(o);
            Refresh();
        }

        /// <summary>
        /// Remove object by name
        /// </summary>
        /// <param name="str"></param>
        public void Remove(string str)
        {            
            comboBox.Items.Remove(str);
            foreach (object o in Content)
            {
                if (o.ToString() == str)
                {
                    Content.Remove(o);
                }
            }
            Refresh();
        }

        private void Sort()
        {
            try
            {
                Content.Sort(
                    delegate(object o1, object o2)
                    {
                        return o1.ToString().CompareTo(o2.ToString());
                    });
            }
            catch
            {
                Logger.LogLow("Error sorting in propertyviewer.");
            }
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (object o in Content)
            {
                if (o.ToString() == comboBox.SelectedItem.ToString())
                {
                    propertyGrid.SelectedObject = o;
                    if (o.GetType() == typeof(Test))
                    {
                        var test = (Test)o;
                        test.HasFocus = true;
                    }
                    else if (o.GetType() == typeof(TestElement))
                    {
                        var element = (TestElement)o;
                        element.HasFocus = true;
                    }
                    break;
                }
            }
        }
        
        private void propertyGrid_Enter(object sender, EventArgs e)
        {
            Program.UndoManager.Update();
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Refresh(comboBox.SelectedIndex);
            Parent.Parent.Parent.Refresh();
        }
  
        public void Refresh(int setSelectionOn = 0)
        {
            comboBox.Items.Clear();
            for (int i = 0; i < Content.Count; i++)
            {
                comboBox.Items.Add(Content[i].ToString());
            }
            comboBox.SelectedIndex = setSelectionOn;
        }

        public void Lock(bool lockAll)
        {
            OpeningFile = lockAll;
            if (!OpeningFile)
            {
                Sort();
                Refresh();
            }
        }





    }
}
