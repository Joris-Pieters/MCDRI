using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using MC.Graphical;
using MC.Testing;
using MC.PaperTools;
using MC.Other;
using MC.Design;

namespace MC.Forms
{
    public partial class ctrlImport
    {
        // Id: unique to each page
        // Row: In GUI
        // Index: in CorrectedPages

        private int IdToRow(int id)
        {
            int row = -1;
            for (int i = 0; i < dataGrid.Rows.Count; i++)
            {
                if ((int)dataGrid[0, i].Value == id)
                {
                    row = i;
                    break;
                }
            }
            return row;
        }

        private int RowToId(int row)
        {
            return (int)dataGrid.Rows[row].Cells[0].Value;
        }

        private int IdToIndex(int id)
        {
            int index = -1;
            for (int i = 0; i < CorrectedPages.Count; i++)
            {
                if (CorrectedPages[i].ID == id)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private int IndexToId(int index)
        {
            return CorrectedPages[index].ID;
        }

        private int IndexToRow(int index)
        {
            return IdToRow(CorrectedPages[index].ID);
        }

        private int RowToIndex(int row)
        {
            return IdToIndex((int)dataGrid.Rows[row].Cells[0].Value);
        }

        private CorrectedPage RowToPage(int row)
        {
            return IdToPage(RowToId(row));
        }

        private int PageToRow(CorrectedPage page)
        {
            int row = -1;
            for (int i = 0; i < dataGrid.Rows.Count; i++)
            {
                if ((int)dataGrid[0, i].Value == page.ID)
                {
                    row = i;
                    break;
                }
            }
            return row;
        }

        private CorrectedPage IdToPage(int id)
        {
            CorrectedPage page = null;
            for (int i = 0; i < CorrectedPages.Count; i++)
            {
                if (CorrectedPages[i].ID == id)
                {
                    page = CorrectedPages[i];
                    break;
                }
            }
            return page;
        }

        private int PageToId(CorrectedPage page)
        {
            return page.ID;
        }

        private CorrectedPage IndexToPage(int index)
        {
            return CorrectedPages[index];
        }

        private int PageToIndex(CorrectedPage page)
        {
            int index = -1;
            for (int i = 0; i < CorrectedPages.Count; i++)
            {
                if (CorrectedPages[i].ID == page.ID)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

    }
}
