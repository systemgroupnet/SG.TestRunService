using System;
using System.Collections.Generic;
using System.Text;

namespace SG.TestRunService.Common.Models
{
    public class CodeSignature
    {
        public CodeSignature() { }

        public CodeSignature(string path, string signature, CodeSignatureType type)
        {
            Path = path;
            Signature = signature;
            Type = type;
        }

        [Obsolete("Use `Path` property.")]
        public string FileName { get; set; }
        public string Path { get; set; }
        public string Signature { get; set; }
        public CodeSignatureType Type { get; set; }
    }
}
