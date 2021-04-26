using LogAnalyserApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LogAnalyzer.Controllers
{

    /// <summary>
    /// Logging methods
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogAnalyserService logAnalyserService;
        private readonly IWebHostEnvironment hostEnvironment;


        /// <summary>
        /// Dependency Injections
        /// </summary>
        /// <param name="logAnalyserService"></param>
        /// <param name="hostEnvironment"></param>
        public LogController(ILogAnalyserService logAnalyserService, IWebHostEnvironment hostEnvironment)
        {
            this.logAnalyserService = logAnalyserService;
            this.hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// This endpoint gets the total logs in all the files
        /// </summary>
        /// <returns>Returns int TotalLogs</returns>
        [HttpGet]
        [Route("getAllLogs")]
        public IActionResult GetTotalLogs()
        {
            return Ok(logAnalyserService.TotalLogs("*.log"));
        }

        /// <summary>
        /// This endpoint gets total available logs in a period
        /// </summary>
        /// <returns>Returns int TotalLogs for time range</returns>
        [HttpGet]
        [Route("getPeriodicLogs")]
        public IActionResult GetTotalLogs(DateTime startDay, DateTime endDate)
        {
            return Ok(logAnalyserService.TotalLogs("*.log", startDay, endDate));
        }

        /// <summary>
        /// This endpoint gets total available logs based on size range
        /// </summary>
        /// <returns>Returns object Dictionary TKey, TValue of files for size range</returns>
        [HttpGet]
        [Route("getLogsBasedOnSize")]
        public IActionResult GetTotalLogs(long startSize, long endSize)
        {
            return Ok(logAnalyserService.LogFiles("*.log", startSize, endSize));
        }

        /// <summary>
        /// This endpoint gets error logs per log file
        /// </summary>
        /// <returns>Returns a list FileError object</returns>
        [HttpGet]
        [Route("getLogCountPerFile")]
        public IActionResult UniqueAndDuplicateLogsPerFile()
        {
            return Ok(logAnalyserService.CalculateLogs("*.log"));
        }

        /// <summary>
        /// This endpoint archives logs from a period
        /// </summary>
        /// <returns>Returns int Number of all archieved files</returns>
        [HttpGet]
        [Route("archiveLogPeriodic")]
        public IActionResult ArchiveErrors(DateTime startDay, DateTime endDate)
        {
            return Ok(logAnalyserService.ArchiveFiles(("*.log"), startDay, endDate));
        }

        /// <summary>
        /// This endpoint deletes logs from a period
        /// </summary>
        /// <returns>Returns int Number of all deleted files</returns>
        [HttpGet]
        [Route("DeleteLogFilePeriodic")]
        public IActionResult DeleteErrors(DateTime startDay, DateTime endDate)
        {
            return Ok(logAnalyserService.DeleteFiles(logAnalyserService.LogFiles("*.log").FileResult, startDay, endDate));
        }

        /// <summary>
        /// This endpoint deletes archives from a period
        /// </summary>
        /// <returns>Returns int Number of all deleted files</returns>
        [HttpGet]
        [Route("DeleteArchivePeriodic")]
        public IActionResult DeleteArchives(DateTime startDay, DateTime endDate)
        {
            logAnalyserService.DeleteArchiveFilesRecursive(startDay, endDate);

            return Ok();
        }

        /// <summary>
        /// This endpoint saves log files to remote server
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadLogFile")]
        public IActionResult SaveToServer(IFormFile form)
        {
                //string uniqueFileName = null;
                if (form != null)
                {
                var webRoot = hostEnvironment.WebRootPath;
                var uploadFolder = webRoot + Path.DirectorySeparatorChar
                                               + Path.DirectorySeparatorChar
                                               + "Logfiles";
                    //uniqueFileName = Guid.NewGuid().ToString() + "_" + form.FileName;
                    string filePath = Path.Combine(uploadFolder, form.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        form.CopyTo(fileStream);
                    }

                }

                return Ok();
            
        }


    }
}
