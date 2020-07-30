using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using MC.Testing;
using MC.Other;

namespace MC
{
    static class Program
    {     
        public static Test Test;
        public static UserSettings UserSettings;
        public static UndoManager UndoManager;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LoadUserSettings();
            Logger.Start();

            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Application.ProductName);

#if DEBUG            
            if (args.Length == 1)
            {
                Application.Run(new Forms.frmMain(args[0])); // Dubble clicked .test-file
            }
            else
            {
                Application.Run(new Forms.frmMain());
            }                   
#else
            try
            {
                if (args.Length == 1)
                {
                    Application.Run(new Forms.frmMain(args[0])); // Dubble clicked .test-file
                }
                else
                {
                    Application.Run(new Forms.frmMain());
                }
            }
            catch(Exception ex)
            {
                Logger.LogHigh("Critical error: " + ex.Message);
                MessageBox.Show("A problem occured: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif
        }

        private static void LoadUserSettings()
        {
            try
            {
                using (var reader = new StreamReader(Settings.UserSettingsFile))
                {
                    var xmlSerial = new XmlSerializer(typeof(UserSettings));
                    Program.UserSettings = (UserSettings)xmlSerial.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                // Don't put this error in the log file, as it occurs at a point the program doesn't know where to put log files
                Logger.LogLow("Error loading user settings. " + ex.Message + " New usersettings created.");
                Program.UserSettings = new UserSettings();
            }
        }
    }
}