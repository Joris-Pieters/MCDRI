using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel.Design;
using PdfSharp;
using PdfSharp.Drawing;
using MC.Testing;
using MC.PaperTools;
using MC.Other;
using MC.Forms;

namespace MC.Design
{
    public class TextBox : TestElement
    {
        // Contrary to description in itemset, this has no autosize because size can be set in both directions (a description in an itemset can only increase the height);
        
        [Editor(typeof(MultiLineStringEditor), typeof(UITypeEditor))]
        [CategoryAttribute("Content"), Browsable(true)]
        [DisplayName("Text"), Description("Edit the content.\r\nPress the drop-down arrow for a better overview. Then you can use Ctrl+Enter if you want to force start a new line.")]
        public new string Text { get; set; }

        private List<List<string>> textSplit;

        public TextBox(string name, Page parentPage, Point gridPosition = new Point(), Size gridSize = new Size())
            : base(name, parentPage, gridPosition, gridSize)
        {
            if (gridSize.Width == 0 || gridSize.Height == 0)
            {
                gridSize = new Size(6, 4);
            }
            GridSize = gridSize;
            Text = "[Empty text box]";
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
        {
            using (Graphics g = pe.Graphics)
            {
                g.FillRectangle(new SolidBrush(Color.White), this.DisplayRectangle);
                g.DrawRectangle(new Pen(Color.Black), new Rectangle(0, 0, Width - 1, Height - 1));

                var lineSplit = Text.Split('\n');
                textSplit = new List<List<string>>();
                for (int i = 0; i < lineSplit.Count(); i++)
                {
                    lineSplit[i]= lineSplit[i].TrimEnd();
                    if (lineSplit[i] == "")
                    {
                        lineSplit[i] += " ";
                    }
                    textSplit.Add(StringTools.Split(lineSplit[i], Width - 6, g, Settings.font));
                }

                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Near;
                format.LineAlignment = StringAlignment.Center;
                using (Brush b = new SolidBrush(Color.Black))
                {
                    int line = 0;
                    for (int i = 0; i < textSplit.Count; i++)
                    {
                        for (int j = 0; j < textSplit[i].Count; j++)
                        {
                            if (line < GridSize.Height)
                            {
                                g.DrawString(textSplit[i][j], Settings.font, b, new Point(2, (int)((line + .5) * ParentPage.ParentTest.Paper.BlockSize.Height)), format);
                                line++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                base.OnPaint(pe);
            }
        }

        public override void PdfDraw(XGraphics g)
        {
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Near;
            format.LineAlignment = XLineAlignment.Center;

            XBrush b = new XSolidBrush(Color.Black);
            int line = 0;
            for (int i = 0; i < textSplit.Count; i++)
            {
                for (int j = 0; j < textSplit[i].Count; j++)
                {
                    if (line < GridSize.Height)
                    {
                        g.DrawString(textSplit[i][j], Settings.xfont, b, new Point(Left + 2, Top + (int)((line + .5) * ParentPage.ParentTest.Paper.BlockSize.Height)), format);
                        line++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            base.PdfDraw(g);
        }

    }
}