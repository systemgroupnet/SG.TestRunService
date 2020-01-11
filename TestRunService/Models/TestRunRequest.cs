using AutoMapper;
using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Models
{
    [AutoMap(typeof(TestRun))]
    public class TestRunRequest
    {
        public int Id { get; set; }
        public int TestRunId { get; set; }
        public int TestId { get; set; }
    }
}
