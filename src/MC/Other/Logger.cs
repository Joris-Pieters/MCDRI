using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace MC.Other
{
    public static class Logger
    {
        static StreamWriter writer;
        static string fileName;

        public static void Start()
        {

            string path = "";

#if !DEBUG
            path = Program.UserSettings.logDirectory + "\\";
#endif

            fileName = path +
                DateTime.Now.Year + "-" +
                DateTime.Now.Month + "-" +
                DateTime.Now.Day + " " +
                DateTime.Now.Hour + "-" +
                DateTime.Now.Minute + "-" +
                DateTime.Now.Second + ".log";

            writer = new StreamWriter(fileName);
        }

        public static void Stop()
        {
            writer.AutoFlush = true;
            long length = writer.BaseStream.Length;
            writer.Dispose();
            if (length == 0) // Delete empty log files (will still be there in case of program crash however)
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    Logger.LogLow(ex.Message);
                }
                    Logger.LogLow("log: " + length);
            }
        }

        public static void LogLow(object o)
        {
#if DEBUG
                Debug.WriteLine("| " + o.ToString());
#endif
        }

        public static void LogHigh(object o)
        {
            LogLow(o);

            writer.WriteLine(
                DateTime.Now.Year   + "\t" +
                DateTime.Now.Month  + "\t" +
                DateTime.Now.Day    + "\t" +
                DateTime.Now.Hour   + "\t" +
                DateTime.Now.Minute + "\t" +
                DateTime.Now.Second + "\t" +
                o.ToString());
        }

    }
}
