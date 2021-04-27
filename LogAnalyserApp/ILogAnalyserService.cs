using LogAnalyzer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogAnalyserApp
{
    public interface ILogAnalyserService
    {
        ReturnObject ArchiveFiles(string searchPattern, DateTimeOffset startDate, DateTimeOffset endDate);
        ReturnObject CalculateLogs(string searchPattern);
        void DeleteArchiveFilesRecursive(DateTimeOffset startDate, DateTimeOffset endDate);
        ReturnObject DeleteFiles(FileInfo[] allfiles, DateTimeOffset startDate, DateTimeOffset endDate);
        int FileCount(FileInfo[] allfiles);
        int FileCount(FileInfo[] allfiles, DateTimeOffset startDate, DateTimeOffset endDate);
        (int DuplicateLogs, int UniqueLogs) FileTotalLogs(FileInfo item);
        (bool status, DateTimeOffset date) IsNewLine(string line);
        (FileInfo[] FileResult, string PathOfFile) LogFiles(string SearchPattern);
        (FileInfo[] FileResult, string PathOfFile) LogFiles(string SearchPattern, DateTimeOffset startDate, DateTimeOffset endDate);
        ReturnObject LogFiles(string SearchPattern, long startSize, long endSize);
        ReturnObject TotalLogs(string SearchPattern);
        ReturnObject TotalLogs(string SearchPattern, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
