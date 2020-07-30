using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using MC.Graphical;
using MC.PaperTools;
using MC.Other;
using MC.Design;

namespace MC.Testing
{
    public class CorrectedSubject : IDisposable
    {
        public string Id
        {
            get
            {
                if (_id == "unknown")
                {
                    _id = GetId();
                }
                return _id;
            }
        }
        private string _id = "unknown";

        public List<CorrectedPage> CorrectedPages;

        // Light version of an itemset. Contains only the name, role, and the items
        public List<ItemCollection> ItemCollections;

        /// <summary>
        /// Not all correctedPages, only those of a certain subject!
        /// </summary>        
        public CorrectedSubject(List<CorrectedPage> CorrectedPages)
        {
            Update(CorrectedPages);            
        }

        private string GetId()
        {
            if (CorrectedPages.Count > 0)
            {
                return CorrectedPages[0].Subject();
            }
            else
            {
                return "Unknown";
            }
        }

        public int PageCount()
        {
            return CorrectedPages.Count;
        }

        public void Update(List<CorrectedPage> correctedPages)
        {
            CorrectedPages = correctedPages;
            ItemCollections = new List<ItemCollection>();

            // First, add itemcollections to subject based on itemsets in the test
            // Information that is scanned only provides the checked values, not everything else you need to know!
            for (int i = 0; i < Program.Test.Pages.Count; i++)
            {
                foreach (var element in Program.Test.Pages[i].Controls)
                {
                    if (element.GetType() == typeof(ItemSet))
                    {
                       ItemCollections.Add(new ItemCollection((ItemSet)element));
                    }
                }
            }
            
            // Check the scanned pages, and use this information to set the values in the itemCollections            
            for (int i = 0; i < CorrectedPages.Count; i++)
            {
                if (CorrectedPages[i].Status.Analyzed && CorrectedPages[i].PageNumber >= 0 && !CorrectedPages[i].Status.CalibrationError)
                {
                    foreach (var element in Program.Test.Pages[CorrectedPages[i].PageNumber].Controls)
                    {
                        if (element.GetType() == typeof(ItemSet))
                        {
                            var set = (ItemSet)element;
                            var collection = ItemCollectionByName(set.Name); // An itemcollection has already been made
                            for (int j = 0; j < set.Items.Count; j++)
                            {
                                var item = set.Items[j].Copy();
                                for (int k = 0; k < set.NumberOfAlternatives; k++)
                                {
                                    var p = set.ItemAltPointGrid(j, k);
                                    // ItemCheckedState from scanned image is transfered to items who are part of an
                                    // itemCollection which belongs to a subject
                                    item.Checked[k] = CorrectedPages[i].CheckImage.ItemAltsCheckedState[p.X, p.Y];
                                }
                                collection.Items[j] = item;
                            }
                        }
                    }
                }
            }
        }

        public ItemCollection ItemCollectionByName(string name)
        {            
            for (int i = 0; i < ItemCollections.Count; i++)
            {
                if (ItemCollections[i].Name == name)
                {
                    return (ItemCollections[i]);         
                }
            }
            return null;
        }

        public bool HasError()
        {
            for (int j = 0; j < CorrectedPages.Count; j++)
            {
                if (CorrectedPages[j].Status.AnyError())
                {
                    return true;
                }         
            }
            return false;
        }

        public void Dispose()
        {
            for (int i = CorrectedPages.Count - 1; i >= 0; i--)
            {
                CorrectedPages[i] = null;
            }
            CorrectedPages.Clear();
            ItemCollections.Clear();
        }
    }
}
