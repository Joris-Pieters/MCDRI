using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MC.Graphical;
using MC.PaperTools;
using MC.Other;
using MC.Design;

namespace MC.Testing
{
    public struct PageStatus
    {
        public bool Analyzed;
        public bool ContainsDoubts;
        public bool ContainsTooManyChecked;
        public bool PageNumberOrHashError;
        public bool BarCodeError;
        public bool CalibrationError;

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("Analyzed\t\t\t\t\t" + Analyzed + "\n   ");
            str.Append("Contains doubts\t\t\t\t" + ContainsDoubts + "\n   ");
            str.Append("Contains too many checked\t" + ContainsTooManyChecked + "\n   ");
            str.Append("Calibration error\t\t\t" + CalibrationError + "\n   ");
            str.Append("Bar code error\t\t\t\t" + BarCodeError + "\n   ");
            str.Append("Page number of hash error\t" + PageNumberOrHashError);            
            return str.ToString();
        }

        public void Reset()
        {
            Analyzed = false;
            ContainsDoubts = false;
            ContainsTooManyChecked = false;
            PageNumberOrHashError = false;
            BarCodeError = false;
            CalibrationError = false;
        }

        public bool AnyError()
        {
            return ContainsDoubts || ContainsTooManyChecked || PageNumberOrHashError || BarCodeError || CalibrationError;
        }
    }
}

