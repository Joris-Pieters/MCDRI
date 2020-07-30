using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Serialization;
using PdfSharp;
using PdfSharp.Drawing;
using MC.Testing;
using MC.Graphical;
using MC.PaperTools;

namespace MC.Other
{
    public class UserSettings
    {
        // Defaults in case user settings file is corrupt
                
        public bool openPdfAfterSave = true;
        public bool showWarningEditAnalysed = true;

        public int numberOfParallelThreads = -1; // -1 = unlimited
        public int numberOfDecimalsInResults = 4;
        public int numberOfUndo = 1000;

        public string currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public string defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Tests";
        public string logDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Tests\\Log";

        public PaperTemplate defaultPaperTemplate = PaperTemplate.A4;
        public PaperOrientation defaultPaperOrientation = PaperOrientation.Portrait;
        public SizeF defaultPaperBorder = new SizeF(.5f, .5f);

        public double calibrationMean = 92;
        public double calibrationSD = 1;

        public double SDsure = 6;
        public double SDdoubt = 4;

        public int maximumCannyAttempts = 70;
        public int calibrationCannyStart = 50;

        [XmlElement("colorSure", DataType = "hexBinary")]
        public byte[] colorSure = { 255, 0, 255, 0};

        [XmlElement("colorDoubt", DataType = "hexBinary")]
        public byte[] colorDoubt = {255, 255, 0, 0};

        [XmlElement("colorCursor", DataType = "hexBinary")]
        public byte[] colorCursor = {128, 0, 0, 255};

        [XmlElement("colorMark", DataType = "hexBinary")]
        public byte[] colorMark = {80, 255, 255, 0};
        
        // Should not be changed in current version!
        public string font = "verdana";
        public int fontSize = 8;
        public int xfontSize = 11;

        public UserSettings()
        {
            Settings.font = new Font(font, fontSize);
            Settings.xfont = new XFont(font, xfontSize);
            Settings.fontSmall = new Font(font, fontSize - 2);
            Settings.xfontSmall = new XFont(font, xfontSize - 2);
        }


    }
}
