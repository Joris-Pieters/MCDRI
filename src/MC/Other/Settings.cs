using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Reflection;
using PdfSharp;
using PdfSharp.Drawing;
using MC.Testing;
using MC.Graphical;
using MC.PaperTools;

namespace MC.Other
{
    public static class Settings
    {
        public static int blockWidth = 22;
        public static int blockHeight = 14;

        public static Font font;
        public static XFont xfont;

        public static Font fontSmall;
        public static XFont xfontSmall;

        /// <summary>
        /// Consider every page as a valid first page.
        /// </summary>
        public static bool IgnoreBarCodeErrors = false; 

        // todo V2: pdf processing door ImageMagick in background

        public static string readableFilter = "Image Files (*.bmp;*.jpg;*.jpeg;*.gif;*.png)|*.bmp;*.jpg;*.jpeg;*.gif;*.png";            

        public static int FileVersion = 1;
        
        public static int[] SuccessFullCanny = new int[100];

        public static string ManualFile = "manual.pdf";

        public static string UserSettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Application.ProductName + "\\usersettings.xml";

        public static string SoftwareName()
        {
            var v = Assembly.GetExecutingAssembly();
            return "MCDRI " + v.GetName().Version.Major + "." + v.GetName().Version.Minor;
        }

        private static string iconDirectory = Application.StartupPath + "\\icons\\";
        public static Image Empty;
        public static Image NotAnalyzed;
        public static Image BarCodeError;
        public static Image CalibrationError;
        public static Image ContainsDoubts;
        public static Image ContainsTooManyChecked;
        public static Image PageNumberOrHashError;
        public static Image Undo;
        public static Image Redo;
        public static Image CreateScript;

        public static void SetIcons()
        {
            Empty = Image.FromFile(iconDirectory + "Empty.png");
            NotAnalyzed = Image.FromFile(iconDirectory + "NotAnalyzed.png");
            BarCodeError = Image.FromFile(iconDirectory + "BarCodeError.png");
            CalibrationError = Image.FromFile(iconDirectory + "CalibrationError.png");
            ContainsDoubts = Image.FromFile(iconDirectory + "ContainsDoubts.png");
            ContainsTooManyChecked = Image.FromFile(iconDirectory + "ContainsTooManyChecked.png");
            PageNumberOrHashError = Image.FromFile(iconDirectory + "PageNumberOrHashError.png");
            Undo = Image.FromFile(iconDirectory + "ArrowUndo.png");
            Redo = Image.FromFile(iconDirectory + "ArrowRedo.png");
            CreateScript = Image.FromFile(iconDirectory + "Create.png");
        }

    }
}
