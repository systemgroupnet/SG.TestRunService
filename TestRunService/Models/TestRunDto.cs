using AutoMapper;
using SG.TestRunService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Models
{
    public class TestRunDto
    {
        public int Id { get; set; }
        public int TestRunSessionId { get; set; }
        public int TestId { get; set; }
    }
}
