using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class SessionFilterRequest
    {
        public string ProjectName { get; set; }

        public long? StartThreshold { get; set; }

        public long? CompletedThreshold { get; set; }

        public int? Max { get; set; }
    }
}
