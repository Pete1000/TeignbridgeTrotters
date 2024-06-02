using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeignbridgeTrotters.ResultDownloader
{
    public class ProgramSettings
    {
        public int WaitTimeSeconds { get; set; }
        public string? SourceFileName { get; set; }
        public string? DestinationFileName { get; set; }
    }
}
