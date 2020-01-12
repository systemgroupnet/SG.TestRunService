using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Models
{
    public class TestRunStatisticItem
    {
        public TestRunOutcome Outcome { get; set; }
        public int Count { get; set; }
    }
}
