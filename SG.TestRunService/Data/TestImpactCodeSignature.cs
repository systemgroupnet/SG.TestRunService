using SG.TestRunService.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class TestImpactCodeSignature : IEntity
    {
        public int Id { get; set; }

        public int TestCaseId { get; set; }
        public int AzureProductBuildDefinitionId { get; set; }

        public string Signature { get; set; }

        public DateTime DateAdded { get; set; }
        public DateTime? DateRemoved { get; set; }
        public bool IsDelelted { get; set; }
    }
}
