using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Text;
using System.Diagnostics;

namespace MC.Forms
{
    public class MultiLineStringEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc == null)
            {
                return null;
            }

            RichTextBox richTextBox = new RichTextBox();
            richTextBox.Multiline = true;
            richTextBox.Height = 250;
            richTextBox.Width = 500;
            string sText = value as string;
            if (string.IsNullOrEmpty(sText) == false)
            {
                richTextBox.Text = sText;
            }
            edSvc.DropDownControl(richTextBox);
            return richTextBox.Text;         
        }

    }
}