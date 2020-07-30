using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using MC.Other;
using MC.Design;

namespace MC.Testing
{
    public class ItemCollection
    {
        public string Name;
        public List<Item> Items = new List<Item>();
        public ItemSetRole Role;
        public List<string> Alternatives = new List<string>();
        private ItemSet ItemSet;

        public ItemCollection()
        {
        }

        public ItemCollection(ItemSet itemSet)
        {
            Name = itemSet.Name;
            Role = itemSet.Role;
            ItemSet = itemSet;
            for (int j = 0; j < itemSet.Items.Count; j++)
            {
                Items.Add(itemSet.Items[j].Copy());
            }
            for (int j = 0; j < itemSet.Alternatives.Count; j++)
            {
                Alternatives.Add(itemSet.Alternatives[j]);
            }
        }

        public Fraction Score(int item = -1)
        {
            Fraction sum = new Fraction();
            if (item >= 0 && item < Items.Count) // specific item
            {
                sum += Items[item].Score(ItemSet);
            }
            else // all items
            {                
                for (int i = 0; i < Items.Count; i++)
                {
                    sum += Items[i].Score(ItemSet);
                }
            }
            return sum;
        }

    }
}
