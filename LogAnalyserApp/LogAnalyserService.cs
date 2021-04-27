using LogAnalyzer;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace LogAnalyserApp
{
   public class LogAnalyserService : ILogAnalyserService
    {
        public (FileInfo[] FileResult, string PathOfFile) LogFiles(string SearchPattern)
        {
            DirectoryInfo temporaryFileSearch = new DirectoryInfo(@"C:\AmadeoLogs").Exists ? new DirectoryInfo(@"C:\AmadeoLogs") : new DirectoryInfo(@"D:\AmadeoLogs");
            

            FileInfo[] fileList;

            fileList = temporaryFileSearch.GetFiles(SearchPattern, SearchOption.AllDirectories);
            var residentDir = fileList.FirstOrDefault().Directory;
            return (fileList, residentDir.FullName);
        }
        public int FileCount(FileInfo[] allfiles)
        {
            return allfiles.Count();
        }


        public int FileCount(FileInfo[] allfiles, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            int countLogFile = 0;
            var trimBy = new char[] { '.', 'l', 'o', 'g' };
            var fileNames = allfiles.Select(x => x.Name.Trim(trimBy)).ToList();


            foreach (var item in fileNames)
            {
                var computablefilename = string.Join("", item.TakeLast(10).ToList());

                var compareDate = DateTimeOffset.Parse(computablefilename);
                if (compareDate >= startDate && compareDate <= endDate)
                {
                    countLogFile++;
                }
            }
            return countLogFile;
        }


        public (FileInfo[] FileResult, string PathOfFile) LogFiles(string SearchPattern, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var fileList = LogFiles(SearchPattern).FileResult;
            var trimBy = new char[] { '.', 'l', 'o', 'g' };

            List<FileInfo> searchFiles = new List<FileInfo>();
            foreach (var file in fileList)
            {
                var nameOfFile = file.Name.Trim(trimBy);
                var computablefilename = string.Join("", nameOfFile.TakeLast(10).ToList());

                var compareDate = DateTimeOffset.Parse(computablefilename);
                if (compareDate >= startDate && compareDate <= endDate)
                {
                    searchFiles.Add(file);
                }
            }

            return (searchFiles.ToArray(), SearchPattern);
        }

        public ReturnObject LogFiles(string SearchPattern, long startSize, long endSize)
        {
            var fileList = LogFiles(SearchPattern).FileResult;
            var trimBy = new char[] { '.', 'l', 'o', 'g' };

            Dictionary<string, string> searchFiles = new Dictionary<string, string>();
            foreach (var file in fileList)
            {
                var nameOfFile = file.Name.Trim(trimBy);
                var computablefilename = string.Join("", nameOfFile.TakeLast(10).ToList());

                var compareSize = file.Length / 1024;
                if (compareSize >= startSize && compareSize <= endSize)
                {
                    var displaysize = Math.Round(Convert.ToDecimal(file.Length) / Convert.ToDecimal(1024), 2);
                    searchFiles.Add(file.Name, displaysize+" kb");
                }
            }

            return new ReturnObject {  Response = "Successful", returnObject = new { searchFiles}, Status = true };
        }


        public ReturnObject DeleteFiles(FileInfo[] allfiles, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            int countLogFile = 0;
            var trimBy = new char[] { '.', 'l', 'o', 'g' };

            foreach (var file in allfiles)
            {
                var nameOfFile = file.Name.Trim(trimBy);
                var computablefilename = string.Join("", nameOfFile.TakeLast(10).ToList());

                var compareDate = DateTimeOffset.Parse(computablefilename);
                if (compareDate >= startDate && compareDate <= endDate)
                {
                    file.Delete();
                    countLogFile++;
                }
            }

            return new ReturnObject { Response = $"Successful. {countLogFile} files are deleted", returnObject = null, Status = true };
        }

        public void DeleteArchiveFilesRecursive(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var temporaryFileSearch = new DirectoryInfo(@"C:\Users\DELL\Documents\logging lab");
            var fileList = temporaryFileSearch.GetFiles("*.zip", SearchOption.AllDirectories);
            List<FileInfo> filesToDelete = new List<FileInfo>();

            foreach (var item in fileList)
            {
                var filename = item.Name.Replace('_', '.').Trim(new char[] { '.', 'z', 'i', 'p' });
                var one = filename.Split('-')[0];
                var two = filename.Split('-')[1];
                var filenameparts1 = DateParser(one);
                var filenameparts2 = DateParser(two);
                if (filenameparts1 >= startDate && filenameparts2 <= endDate)
                {
                    item.Delete();
                }
            }
        }

        public ReturnObject ArchiveFiles(string searchPattern, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            string patternPath = LogFiles(searchPattern).PathOfFile;

            string startPath = patternPath;
            string startDay = startDate.Day < 10 ? "0" + startDate.Day.ToString() : startDate.Day.ToString();
            string startMonth = startDate.Month < 10 ? "0" + startDate.Month.ToString() : startDate.Month.ToString();

            string endDay = endDate.Day < 10 ? "0" + endDate.Day.ToString() : endDate.Day.ToString();
            string endMonth = endDate.Month < 10 ? "0" + endDate.Month.ToString() : endDate.Month.ToString();


            string folderToAch = patternPath + $"\\\\{startDay}_{startMonth}_{startDate.Year}-{endDay}_{endMonth}_{endDate.Year}";
            string zipPath = patternPath + $"\\\\{startDay}_{startMonth}_{startDate.Year}-{endDay}_{endMonth}_{endDate.Year}.zip";

            try
            {
                Directory.CreateDirectory(folderToAch);
            }
            catch (Exception)
            {


            }
            var allfiles = LogFiles(searchPattern, startDate, endDate).FileResult;
            foreach (var item in allfiles)
            {
                var desF = folderToAch + "\\\\" + item.Name;
                File.Move(item.FullName, desF, true);
            }

            ZipFile.CreateFromDirectory(folderToAch, zipPath);

            int countLogFile = 0;
            try
            {
                var temporaryFileSearch = new DirectoryInfo(folderToAch);
                var fileList = temporaryFileSearch.GetFiles(searchPattern, SearchOption.AllDirectories);
                foreach (var item in fileList)
                {
                    item.Delete();
                    countLogFile++;
                }

                Directory.Delete(folderToAch);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }


            return new ReturnObject { Response = $"Successful. {countLogFile} files are archived", returnObject = null, Status = true };
        }

        public ReturnObject TotalLogs(string SearchPattern)
        {
            int countLogs = 0;
            var fileList = LogFiles(SearchPattern).FileResult;

            foreach (var item in fileList)
            {
                var filecontent = File.ReadLines(item.FullName);

                foreach (var line in filecontent)
                {
                    if (IsNewLine(line).status)
                    {
                        countLogs++;
                    }

                }
            }

            return new ReturnObject {  Response = "Successful", returnObject = countLogs, Status = true };
        }

        public ReturnObject TotalLogs(string SearchPattern, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            int countLogs = 0;
            var fileList = LogFiles(SearchPattern).FileResult;

            foreach (var item in fileList)
            {
                var filecontent = File.ReadLines(item.FullName);

                foreach (var line in filecontent)
                {
                    var theline = IsNewLine(line);
                    if (theline.status && theline.date >= startDate && theline.date <= endDate)
                    {

                        countLogs++;
                    }

                }
            }

            return new ReturnObject { Response = "Successful", returnObject = new  { total = countLogs }, Status = true };
        }

        public ReturnObject CalculateLogs(string searchPattern)
        {
            var files = LogFiles(searchPattern).FileResult.ToList();
            List<FileError> fileErrors = new List<FileError>();
            foreach (var item in files)
            {
                var result = FileTotalLogs(item);
                fileErrors.Add(new FileError()
                {
                    DuplicateErrorCount = result.DuplicateLogs,
                    UniqueErrorCount = result.UniqueLogs,
                    NameOfFile = item.Name
                });
            }
            
            return new ReturnObject { Response = "Successful", returnObject = new { FileErrors = fileErrors }, Status = true };
        }

        public (int DuplicateLogs, int UniqueLogs) FileTotalLogs(FileInfo item)
        {
            int countUnique = 0;
            int countDuplicate = 0;
            //bool isUsed = false;

            var filecontent = File.ReadLines(item.FullName);
            List<string> comparableLines = new List<string>();

            foreach (var line in filecontent)
            {
                var theline = IsNewLine(line);
                if (theline.status)
                {
                    if (line.Substring(0, 20).Contains("--->"))
                    {
                        comparableLines.Add(line.Remove(0, 25));
                    }
                    else
                    {
                        comparableLines.Add(line.Remove(0, 32));
                    }

                }
            }

            var categories = comparableLines.GroupBy(x => x.Length);

            var emi = categories.ToList();

            foreach (var cat in categories)
            {
                if (cat.Count() > 1)
                {
                    countDuplicate++;
                }
                else
                {
                    countUnique++;
                }
            }

            return (countDuplicate, countUnique);

        }

        public (bool status, DateTimeOffset date) IsNewLine(string line)
        {
            if (line.Length < 80)
            {
                return (false, DateTimeOffset.Now);
            }

            List<string> resultword = new List<string>();
            var wordArr = line.Split(" ").ToList();
            foreach (var wor in wordArr)
            {
                if (wor.Contains(".") || wor.Contains(":"))
                {
                    string properword;
                    if (wor.Contains(">"))
                    {
                        properword = string.Join("", wor.TakeLast(10).ToList());
                        resultword.Add(properword);
                    }
                    else
                    {
                        resultword.Add(wor);
                    }

                }
            }

            foreach (var result in resultword)
            {
                DateTime date;
                string[] format = new string[] { "dd.MM.yyyy" };
                var trythetime = DateTime.TryParseExact(result, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out date);

                if (trythetime)
                {
                    return (true, date);
                }
            }
            return (false, DateTimeOffset.Now);
        }

        private static DateTime DateParser(string dateString)
        {
            DateTime date;
            string[] format = new string[] { "dd.MM.yyyy" };
            var trythetime = DateTime.TryParseExact(dateString, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out date);

            if (trythetime)
            {
                return date;
            }
            return default;
        }

        

    }
}
