using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class ExtraDataValue
    {
        public ExtraDataValue() { }

        public ExtraDataValue(string value, string url = null)
        {
            Value = value;
            Url = url;
        }

        public string Value { get; set; }
        public string Url { get; set; }
    }
}
