using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClientLib
{
    public class ConsoleLogger : ILogger
    {
        public bool IsEnabled { get; set; } = true;

        private static string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyy/MM/dd - HH:mm:ss.fff");
        }

        public void Debug(string message)
        {
            Console.WriteLine(GetTimeStamp() + " DEBUG: " + message);
        }

        public void Error(string message)
        {
            Console.WriteLine(GetTimeStamp() + "ERROR: " + message);
        }

        public void Info(string message)
        {
            Console.WriteLine(GetTimeStamp() + " " + message);
        }

        public void Warn(string message)
        {
            Console.WriteLine(GetTimeStamp() + " !! " + message);
        }
    }
}
