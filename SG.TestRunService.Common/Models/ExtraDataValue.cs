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

        public override bool Equals(object obj)
        {
            return obj is ExtraDataValue value &&
                   Value == value.Value &&
                   Url == value.Url;
        }

        public override int GetHashCode()
        {
            int hashCode = 564332472;
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            hashCode = hashCode * -1521134295 + Url.GetHashCode();
            return hashCode;
        }
    }
}
