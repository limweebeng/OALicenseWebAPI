using Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OALicenseWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private ILogger<DataController> _logger;

        #region Constructor
        public DataController(IWebHostEnvironment environment,
            ILogger<DataController> logger)
        {
            _hostingEnvironment = environment;
            _logger = logger;
        }
        #endregion


        [HttpPut("size")]
        public ActionResult GetFileSize([FromBody] Dictionary<string, string> dicInput)
        {
            string errorLog = "";
            try
            {
                string dbFilePath = null;
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
                }
                if (dbFilePath != null)
                {
                    FileInfo fi = new FileInfo(dbFilePath);
                    return Ok(fi.Length.ToString());
                }
                else
                    errorLog = "err_invalidreqres";
            }
            catch (Exception ex)
            {
                errorLog = ex.ToString();
            }
            finally
            {

            } 
            return BadRequest(errorLog);
        }

        [HttpPut()]
        public ActionResult GetDBFile([FromBody] Dictionary<string, string> dicInput)
        {
            string errorLog = "";
            try
            {
                string dbFilePath = null;
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
                }
                if (dbFilePath != null)
                {
                    System.IO.FileStream fs = new System.IO.FileStream(dbFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    fs.CopyTo(ms);
                    fs.Close();
                    var contentType = "APPLICATION/octet-stream";
                    var fileName = Path.GetFileName(dbFilePath);
                    FileStreamResult fsTemp = File(ms, contentType, fileName);
                    fsTemp.FileStream.Position = 0;
                    return fsTemp;
                }
                else
                    errorLog = "err_invalidreqres";
            }
            catch (Exception ex)
            {
                errorLog = ex.ToString();
            }
            finally
            {

            }

            return BadRequest(errorLog);
        }

        [HttpPost("db/{filePath}")]
        [DisableRequestSizeLimit]
        public async System.Threading.Tasks.Task<ActionResult> UploadDBWebEx(string filePath, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Ok("No file is selected.");

            if (!TextHelper.IsMultipartContentType(HttpContext.Request.ContentType))
                return StatusCode(415);

            string errorLog = "";
            try
            {
                if (filePath.Contains("~~"))
                    filePath = filePath.Replace("~~", ".");

                string dbFilePath = "C:";
                List<string> stList = filePath.Split('~').ToList();
                foreach (string st in stList)
                    dbFilePath = Path.Combine(dbFilePath, st);

                string fileName1 = Path.GetFileNameWithoutExtension(dbFilePath);
                if (!Startup.ProgressDBDic.ContainsKey(fileName1))
                {
                    Startup.ProgressDBDic.Add(fileName1, 0);
                }
                Startup.ProgressDBDic[fileName1] = 0;

                long totalBytes = file.Length;
                ContentDispositionHeaderValue contentDispositionHeaderValue =
                        ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                string filename = contentDispositionHeaderValue.FileName.Trim('"');
                byte[] buffer = new byte[16 * 1024];

                using (FileStream output = System.IO.File.Create(dbFilePath))
                {
                    using (Stream input = file.OpenReadStream())
                    {
                        long totalReadBytes = 0;
                        int readBytes;

                        while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            await output.WriteAsync(buffer, 0, readBytes);
                            totalReadBytes += readBytes;
                            int progress = (int)((float)totalReadBytes * 100 / (float)totalBytes);
                            if (Startup.ProgressDBDic[fileName1] != progress && progress > Startup.ProgressDBDic[fileName1])
                            {
                                Startup.ProgressDBDic[fileName1] = progress;
                                await System.Threading.Tasks.Task.Delay(100);
                            }
                        }
                        Startup.ProgressDBDic.Remove(fileName1);
                    }
                }
            }
            catch (Exception ex)
            {
                errorLog = ex.ToString();
            }
            finally
            {

            }

            if (errorLog == "")
                return Ok();
            else
                return BadRequest(errorLog);
        }

        [HttpGet("dbProgress/{filePath}")]
        public ActionResult ProgressDB(string filePath)
        {
            if (filePath.Contains("~~"))
                filePath = filePath.Replace("~~", ".");

            string filePathTemp = "C:";
            List<string> stList = filePath.Split('~').ToList();
            foreach (string st in stList)
                filePathTemp = Path.Combine(filePathTemp, st);
            
            string fileName1 = Path.GetFileNameWithoutExtension(filePathTemp);
            int progress = 0;
            if (Startup.ProgressDBDic.ContainsKey(fileName1))
                progress = Startup.ProgressPatchDic[fileName1];

            return Content(progress.ToString());
        }
    }
}
