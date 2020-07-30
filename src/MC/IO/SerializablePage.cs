using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;
using MC.Design;
using MC.Testing;

namespace MC.IO
{
    [Serializable]
    public class SerializablePage
    {
        public List<SerializableTestElement> SerializableTestElements { get; set; }

        public SerializablePage()
        {
        }

        public SerializablePage(Page page)
        {
            SerializableTestElements = new List<SerializableTestElement>();
            foreach (TestElement element in page.TestElements)
            {
                if (element.GetType() == typeof(ImageBox))
                {
                    SerializableTestElements.Add(new SerializableImageBox((ImageBox)element));
                }
                else if (element.GetType() == typeof(ItemSet))
                {
                    SerializableTestElements.Add(new SerializableItemSet((ItemSet)element));
                }
                else if (element.GetType() == typeof(TextBox))
                {
                    SerializableTestElements.Add(new SerializableTextBox((TextBox)element));
                }
            }
        }

    }
}
