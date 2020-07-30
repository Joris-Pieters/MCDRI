using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.Diagnostics;
using MC.Testing;
using MC.Design;
using MC.Other;

namespace MC.IO
{
    [Serializable]
    public class SerializableItemSet : SerializableTestElement
    {
        public List<Item> Items {get; set;}
        public Naming ItemsNaming { get; set; }
        public ItemSetRole Role { get; set; }

        public List<string> Alternatives { get; set; }        
        public Naming AlternativesNaming { get; set; }

        public int NumberOfCheckedAlternativesAllowed { get; set; }

        public bool ApplyScoreTooManyAlternatives { get; set; }
        public Fraction ScoreIfTooManyAlternativesAreChecked { get; set; }

        public string Description {get; set;}
        public int Columns {get; set;}
                
        public SerializableItemSet() : base ()
        {
        }

        public SerializableItemSet(ItemSet itemSet) : base(itemSet)
        {
            Items = new List<Item>();
            foreach (Item it in itemSet.Items)
            {
                Items.Add(it);
            }
            ItemsNaming = itemSet.ItemsNaming;
            Role = itemSet.Role;

            Alternatives = new List<string>();
            foreach (string str in itemSet.Alternatives)
            {
                Alternatives.Add(str);
            }
            AlternativesNaming = itemSet.AlternativesNaming;

            NumberOfCheckedAlternativesAllowed = itemSet.NumberOfCheckedAlternativesAllowed;

            ApplyScoreTooManyAlternatives = itemSet.ApplyScoreTooManyAlternatives;
            ScoreIfTooManyAlternativesAreChecked = itemSet.ScoreIfTooManyAlternativesAreChecked;
            
            Description = itemSet.Description;
            Columns = itemSet.Columns;
        }

        public override TestElement Deserialize(Page page, bool isCopy = false)
        {
            if (isCopy)
            {
                Name = page.ParentTest.GetFreeName(Name);
            }
            ItemSet itemSet = new ItemSet(Name, page, GridPosition, GridSize);
            itemSet.Border = Border;
            itemSet.Role = Role;

            itemSet.AlternativesNaming = AlternativesNaming;
            foreach (string str in Alternatives)
            {
                itemSet.AddAlternative(str);
            }

            itemSet.ItemsNaming = ItemsNaming;
            foreach (Item it in Items)
            {
                itemSet.AddItem(it);
            }
            
            itemSet.NumberOfCheckedAlternativesAllowed = NumberOfCheckedAlternativesAllowed;

            itemSet.ApplyScoreTooManyAlternatives = ApplyScoreTooManyAlternatives;
            itemSet.ScoreIfTooManyAlternativesAreChecked = ScoreIfTooManyAlternativesAreChecked;
            
            itemSet.Description = Description;
            itemSet.Columns = Columns;
            
            return itemSet;
        }
    }
}
