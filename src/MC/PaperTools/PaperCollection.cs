using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MC.Testing;
using MC.Graphical;
using MC.Other;

namespace MC.PaperTools
{
    public static class PaperCollection
    {

        /* Also add to 
            - enum in "Template.cs" 
            - Paper.SetSize()
            - PdfBuilder.cs
         */
 
        // Always shortest length first, then longest length!
        public static PaperDimensions Letter = new PaperDimensions(215.9f, 279.4f);
        public static PaperDimensions GovernmentLetter = new PaperDimensions(203.2f, 266.7f);
        public static PaperDimensions Legal = new PaperDimensions(219.9f, 355.6f);
        public static PaperDimensions Ledger = new PaperDimensions(279f, 432f);
        public static PaperDimensions A0 = new PaperDimensions(841f, 1189f);
        public static PaperDimensions A1 = new PaperDimensions(594f, 841f);
        public static PaperDimensions A2 = new PaperDimensions(420f, 594f);
        public static PaperDimensions A3 = new PaperDimensions(297f, 420f);
        public static PaperDimensions A4 = new PaperDimensions(210f, 297f);
        public static PaperDimensions A5 = new PaperDimensions(148f, 210f);

    }
}
