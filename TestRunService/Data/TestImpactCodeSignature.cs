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
        [Required]
        public string Signature { get; set; }
        [Required]
        public int TestId { get; set; }
        [Required]
        public DateTime DateAdded { get; set; }
        public DateTime DateRemoved { get; set; }
        public bool IsDelelted { get; set; }
    }
}
