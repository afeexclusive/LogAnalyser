using System;
using System.Collections.Generic;
using System.Text;

namespace LogAnalyserApp
{
    public class FileError
    {
        public string NameOfFile { get; set; }
        public int DuplicateErrorCount { get; set; }
        public int UniqueErrorCount { get; set; }
    }
}
