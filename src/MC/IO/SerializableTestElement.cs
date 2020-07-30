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
    [XmlInclude(typeof(SerializableImageBox))]
    [XmlInclude(typeof(SerializableItemSet))]
    [XmlInclude(typeof(SerializableTextBox))]
    public abstract class SerializableTestElement
    {    
        public string Name {get; set; }
        public bool Border { get; set; }
        public Point GridPosition {get; set; }       
        public Size GridSize {get; set; }

        public SerializableTestElement()
        {
        }

        public SerializableTestElement(TestElement testElement)
        {
            Name = testElement.Name;
            Border = testElement.Border;
            GridPosition = testElement.GridPosition;
            GridSize = testElement.GridSize;
        }

        public abstract TestElement Deserialize(Page page, bool isCopy = false);

    }
}
