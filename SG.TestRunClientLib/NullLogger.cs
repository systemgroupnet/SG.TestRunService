using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClientLib
{
    public class NullLogger : ILogger
    {
        public void Debug(string message) { } 
        public void Info(string message) { } 
        public void Warn(string message) { } 
        public void Error(string message) { }

        public bool IsEnabled => false;
    }
}
