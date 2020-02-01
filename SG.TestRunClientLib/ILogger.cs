using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunClientLib
{
    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        bool IsEnabled { get; }
    }
}
