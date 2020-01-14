using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SG.TestRunService.Data
{
    public class TestImpactCodeSignature : IEntity
    {
        public int Id { get; set; }
        public string Signature { get; set; }
        public int TestId { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateRemoved { get; set; }
        public bool IsDelelted { get; set; }
    }
}
