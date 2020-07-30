using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using MC.Other;
using MC.Design;

namespace MC.Testing
{
    [Serializable]
    public class Item
    {
        public string Name { get; set; }
        
        public Fraction[] Weights { get; set; }

        [XmlIgnore]
        public ItemCheckedState[] Checked { get; set; }

        public Item()
        {
            Name = "";
        }

        public Item(string name)
        {
            Name = name;
        }
      
        public Item Copy()
        {
            var tmp = new Item();
            tmp.Name = Name;
            tmp.Weights = new Fraction[Weights.Count()];
            tmp.Checked = new ItemCheckedState[Weights.Count()];
            if (Weights != null)
            {
                for (int i = 0; i < Weights.Count(); i++)
                {
                    tmp.Weights[i] = new Fraction(Weights[i]);
                    if (Checked != null)
                    {
                        tmp.Checked[i] = Checked[i];
                    }
                }
            }
            return tmp;
        }

        public Fraction Score(ItemSet parent)
        {
            int countChecked = 0;
            for (int i = 0; i < Checked.Count(); i++)
            {
                if (Checked[i] == ItemCheckedState.Checked)
                {
                    countChecked++;
                }
            }

            if (parent.ApplyScoreTooManyAlternatives && countChecked > parent.NumberOfCheckedAlternativesAllowed)
            {
                return parent.ScoreIfTooManyAlternativesAreChecked;
            }

            Fraction f = new Fraction();
            for (int i = 0; i < Checked.Count(); i++)
            {
                if (Checked[i] == ItemCheckedState.Checked)
                {
                    f += Weights[i];
                }
            }
            return f;
        }

        // For identification
        public string ScoreString(ReadOnlyCollection<string> alternatives)
        {
            var str = new StringBuilder();            
            for (int i = 0; i < Checked.Count(); i++)
            {
                if (Checked[i] == ItemCheckedState.Checked)
                {
                    str.Append(alternatives[i]);
                }
            }
            return str.ToString();
        }
    }
    
    public enum ItemCheckedState
    {
        Unknown = 0,
        Unchecked = 1,
        Doubt = 2,
        Checked = 3,
        Unavailable = 4        
    }
}
