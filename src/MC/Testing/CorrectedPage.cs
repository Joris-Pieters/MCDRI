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
    public class CorrectedPage : IDisposable
    {
        public int ID; // To know which one is selected in dataGrid
        public string FileName;
        public CheckImage CheckImage;

        /// <summary>
        /// Zero based!
        /// </summary>
        public int PageNumber = -1;
        public PageStatus Status;

        public CorrectedPage(string fileName, int id = -1)
        {
            ID = id;
            FileName = fileName;
            CheckImage = new CheckImage(FileName);
        }

        public string ShortFileName()
        {
            var spl = FileName.Split('\\');
            return spl[spl.GetLength(0) - 1];
        }

        /// <summary>
        /// Looks for itemsets of type "identification"
        /// </summary>        
        public string Subject()
        {
            if (Status.Analyzed && PageNumber >= 0)
            {
                var sub = new StringBuilder();
                foreach (var element in Program.Test.Pages[PageNumber].Controls)
                {
                    if (element.GetType() == typeof(ItemSet))
                    {
                        var set = (ItemSet)element;
                        if (set.Role == ItemSetRole.Identification)
                        {
                            set.UpdateItemCheckedState(this);
                            sub.Append(set.ScoreString(this));
                        }
                    }
                }
                string str = sub.ToString();
                if (str != "")
                {
                    return str;
                }
                else
                {
                    return FileName;                    
                }
            }
            else
            {
                return FileName;
            }         
        }

        public void AnalyzeGraphical(float criteriumSure, float criteriumDoubt)
        {
            CheckImage.Analyze(criteriumSure, criteriumDoubt, ref Status);           
            if (PageNumber == -1 && Status.Analyzed && !Status.AnyError())
            {
                if (CheckImage.BarCodeBytes[0] != 255)
                {
                    PageNumber = CheckImage.BarCodeBytes[0];
                }
                else
                {
                    PageNumber = -1;
                }
            }
        }

        public List<string> AnalyzeTooManyAndDoubts()
        {
            var list1 = new List<string>();
            if (PageNumber > -1)
            {
                list1 = TooManyChecked();
                var list2 = Doubts();
                for (int i = 0; i < list2.Count; i++)
                {
                    if (!list1.Contains(list2[i]))
                    {
                        list1.Add(list2[i]);
                    }
                }
            }
            return list1;
        }

        private List<string> TooManyChecked()
        {
            var locList = new List<string>();
            Status.ContainsTooManyChecked = false;
            foreach (var ctrl in Program.Test.Pages[PageNumber].Controls)
            {
                if (ctrl.GetType() == typeof(ItemSet))
                {
                    var set = (ItemSet)ctrl;
                    for (int i = 0; i < set.Items.Count; i++)
                    {
                        int count = 0;
                        for (int j = 0; j < set.NumberOfAlternatives; j++)
                        {
                            if (i < Program.Test.Paper.Blocks.X && j < Program.Test.Paper.Blocks.Y)
                            {                             
                                var locationOnPaper = set.ItemAltPointGrid(i, j);
                                if (locationOnPaper.X < Program.Test.Paper.Blocks.X && locationOnPaper.Y < Program.Test.Paper.Blocks.Y)
                                {
                                    if (CheckImage.ItemAltsCheckedState[locationOnPaper.X, locationOnPaper.Y] == ItemCheckedState.Checked)
                                    {
                                        count++;
                                    }
                                }
                            }
                        }
                        if (count > set.NumberOfCheckedAlternativesAllowed)
                        {
                            Status.ContainsTooManyChecked = true;
                            locList.Add(set.Name + "_" + i);
                        }
                    }
                }
            }
            return locList;
        }

        private List<string> Doubts()
        {
            var locList = new List<string>();
            Status.ContainsDoubts = false;
            foreach (var ctrl in Program.Test.Pages[PageNumber].Controls)
            {
                if (ctrl.GetType() == typeof(ItemSet))
                {
                    var set = (ItemSet)ctrl;
                    for (int i = 0; i < set.Items.Count; i++)
                    {
                        for (int j = 0; j < set.NumberOfAlternatives; j++)
                        {
                            var locationOnPaper = set.ItemAltPointGrid(i, j);
                            if (locationOnPaper.X < Program.Test.Paper.Blocks.X && locationOnPaper.Y < Program.Test.Paper.Blocks.Y)
                            {
                                if (CheckImage.ItemAltsCheckedState[locationOnPaper.X, locationOnPaper.Y] == ItemCheckedState.Doubt)
                                {
                                    Status.ContainsDoubts = true;
                                    locList.Add(set.Name + "_" + i);
                                }
                            }
                        }
                    }
                }
            }
            return locList;
        }

        public void Dispose()
        {
            CheckImage.Dispose();
            CheckImage = null;
        }
    }


}
