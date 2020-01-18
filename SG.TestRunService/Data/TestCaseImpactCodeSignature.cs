﻿using SG.TestRunService.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;

namespace SG.TestRunService.Data
{
    public class TestCaseImpactCodeSignature : IEntity
    {
        public int Id { get; set; }

        public int TestCaseId { get; set; }
        public int AzureProductBuildDefinitionId { get; set; }

        [Required]
        public string Signature { get; set; }

        public DateTime DateAdded { get; set; }
        public DateTime? DateRemoved { get; set; }
        public bool IsDelelted { get; set; }

        [OnDelete(DeleteBehavior.Cascade)]
        public TestCase TestCase { get; set; }
    }
}
