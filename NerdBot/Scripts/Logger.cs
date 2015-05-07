using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NerdBot
{
    public class Logger
    {
        private StreamWriter _logger;
        private string _location;

        public bool DebugLogOn { get; set; }
        public bool ErrorLogOn { get; set; }

        public enum LogType
        {
            Debug,
            Error
        }

        public Logger(string loc)
        {
            _location = loc;
            DebugLogOn = true;
            ErrorLogOn = true;
        }

        public void Log(string msg, LogType type)
        {
            if (msg == null)
                return;

            if (type == LogType.Debug && !DebugLogOn)
                return;

            if (type == LogType.Error && !ErrorLogOn)
                return;

            string errorType = type.ToString().ToUpper();
            string date = DateTime.Now.Date.Month + "-" + DateTime.Now.Date.Day + "-" + DateTime.Now.Date.Year;

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Logs\\"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Logs\\");
            }

            _logger = new StreamWriter(Directory.GetCurrentDirectory() + "\\Logs\\" + errorType + "_" + date + ".txt", true);

            _logger.WriteLine(errorType.ToString().ToUpper() + " < " + DateTime.Now + " > " + _location + " : " + msg);
            _logger.Close();

        }
    }
}
