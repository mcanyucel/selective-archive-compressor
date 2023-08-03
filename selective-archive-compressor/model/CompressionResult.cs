using System.Collections.Generic;

namespace selective_archive_compressor.model
{
    internal class CompressionResult
    {
        public bool ResultStatus { get; set; }
        public List<string>? ErrorList { get; set; }
    }
}
