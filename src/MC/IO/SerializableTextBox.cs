using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using MC.Design;
using MC.Testing;

namespace MC.IO
{
    [Serializable]
    public class SerializableTextBox : SerializableTestElement
    {
        public string Text { get; set; }

        public SerializableTextBox() : base()
        {
        }

        public SerializableTextBox(TextBox textBox) : base(textBox)
        {
            Text = textBox.Text;
        }

        public override TestElement Deserialize(Page page, bool isCopy = false)
        {
            if (isCopy)
            {
                Name = page.ParentTest.GetFreeName(Name);
            }
            TextBox textBox = new TextBox(Name, page, GridPosition, GridSize);
            textBox.Border = Border;
            textBox.Text = Text;
            return textBox;
        }

    }
}
