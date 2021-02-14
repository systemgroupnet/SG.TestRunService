using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class SessionFilterRequest
    {
        public string ProjectName { get; set; }

        public DateTime? StartedBefore { get; set; }

        public DateTime? StartedAfter { get; set; }

        public DateTime? CompletedBefore { get; set; }

        public DateTime? CompletedAfter { get; set; }

        public int? Top { get; set; }

        public int? Skip { get; set; }
    }
}
