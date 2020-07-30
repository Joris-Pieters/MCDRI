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
        protected Point clickPosition;
        protected Point scrollPosition;

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            int currentId = CorrectingPageId();
            if (currentId >= 0)
            {
                Point p = pictureBox.PointToClient(MousePosition);
                PointF pRel = new PointF((float)p.X / pictureBox.Width, (float)p.Y / pictureBox.Height);

                selectedPosition = ClosestPosition(IdToPage(currentId).CheckImage.ItemAltsLocation, pRel);
                Draw();

                if (e.Button == MouseButtons.Right && (CorrectedPages[IdToIndex(currentId)].Status.Analyzed))
                {                    
                    switch (CorrectedPages[IdToIndex(currentId)].CheckImage.ItemAltsCheckedState[selectedPosition.X, selectedPosition.Y])
                    {
                        case (ItemCheckedState.Unchecked):
                            correctionMenu.MenuItems[0].Checked = true;
                            correctionMenu.MenuItems[1].Checked = false;
                            correctionMenu.MenuItems[2].Checked = false;
                            correctionMenu.Show(pictureBox, new Point(p.X, p.Y));
                            break;
                        case (ItemCheckedState.Doubt):
                            correctionMenu.MenuItems[0].Checked = false;
                            correctionMenu.MenuItems[1].Checked = true;
                            correctionMenu.MenuItems[2].Checked = false;
                            correctionMenu.Show(pictureBox, new Point(p.X, p.Y));
                            break;
                        case (ItemCheckedState.Checked):
                            correctionMenu.MenuItems[0].Checked = false;
                            correctionMenu.MenuItems[1].Checked = false;
                            correctionMenu.MenuItems[2].Checked = true;
                            correctionMenu.Show(pictureBox, new Point(p.X, p.Y));
                            break;
                    }
                }

                clickPosition = MousePosition;
                scrollPosition = new Point(panelImage.HorizontalScroll.Value, panelImage.VerticalScroll.Value);
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Cursor = Cursors.Hand;
                panelImage.HorizontalScroll.Value = Math.Min(Math.Max(scrollPosition.X + clickPosition.X - MousePosition.X, 0), panelImage.HorizontalScroll.Maximum);
                panelImage.VerticalScroll.Value = Math.Min(Math.Max(scrollPosition.Y + clickPosition.Y - MousePosition.Y, 0), panelImage.VerticalScroll.Maximum);
            }
        }

        private Point ClosestPosition(PointF[,] grid, PointF point)
        {
            Point p = new Point(0, 0);
            if (grid != null)
            {
                double dist = 10000;
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    for (int i = 0; i < grid.GetLength(0); i++)
                    {
                        var newDist = (grid[i, j].X - point.X) * (grid[i, j].X - point.X) + (grid[i, j].Y - point.Y) * (grid[i, j].Y - point.Y);
                        if (newDist < dist)
                        {
                            p = new Point(i, j);
                            dist = newDist;
                        }
                    }
                }
            }
            return p;
        }
    }
}
