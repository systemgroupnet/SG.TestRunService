using SG.TestRunService.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SG.TestRunService.Data
{
    public class CodeSignature : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string Signature { get; set; }
        public string Path { get; set; }
    }
}
