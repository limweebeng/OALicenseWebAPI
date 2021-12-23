using Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OA.AuthLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace OALicenseWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatchController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private ILogger<LicenseController> _logger;
        private readonly string passCode = DataShared.Properties.Resources.licenser;

        #region Constructor
        public PatchController(IWebHostEnvironment environment,
            ILogger<LicenseController> logger)
        {
            _hostingEnvironment = environment;
            _logger = logger;
        }
        #endregion

        //Put api/put
        [HttpPut()]
        public ActionResult Put([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string planCode = null;
                string dbAutoPatchFilePath = null;
                if (dicInput.ContainsKey("planCode"))
                {
                    planCode = dicInput["planCode"];
                    dicInput.Remove("planCode");
                }
                if (dicInput.ContainsKey("dbAutoPatchFilePath"))
                {
                    dbAutoPatchFilePath = dicInput["dbAutoPatchFilePath"];
                    dicInput.Remove("dbAutoPatchFilePath");
                }
                string stMessage = "";
                DataRow patchRow = null;
                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbAutoPatchFilePath != null)
                {
                    DataSet dataSet = SQLiteHelper.GetDataSet(dbAutoPatchFilePath, passCode,
                     out errorCode);

                    if (errorCode == "")
                    {
                        if (planCode != null)
                        {
                            patchRow = AuthEngineHelper.CheckUpdateStatus(dataSet, planCode, out stMessage);
                            if (patchRow != null)
                            {
                                DataTable dt = DataHelper.ConvertDT(patchRow);
                                string json1 = DataHelper.GetJsonTable(dt);
                                dicOut.Add("patchRow", json1);
                            }
                        }
                        else
                        {
                            errorCode = "err_invalidreqres";
                        }
                    }
                }
                else
                {
                    errorCode = "err_invalidreqres";
                }
                dicOut.Add("stMessage", stMessage);
                dicOut.Add("errorCode", errorCode);
                json = JsonConvert.SerializeObject(dicOut);
            }
            catch (Exception ex)
            {
                errorLog = ex.ToString();
            }
            finally
            {

            }
            if (errorLog == "")
                return Ok(json);
            else
                return BadRequest(errorLog);
        }

        //Put api/patch/tableName
        [HttpPut("{tableName}")]
        public ActionResult Put(string tableName, [FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbAutoPatchFilePath = null;
                if (dicInput.ContainsKey("dbAutoPatchFilePath"))
                {
                    dbAutoPatchFilePath = dicInput["dbAutoPatchFilePath"];
                    dicInput.Remove("dbAutoPatchFilePath");
                }

                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbAutoPatchFilePath != null)
                {
                    DataSet dataSet = SQLiteHelper.GetDataSet(dbAutoPatchFilePath, passCode,
                     out errorCode);

                    if (errorCode == "" && dataSet != null)
                    {
                        if (tableName != "")
                        {
                            if (tableName == "all")
                            {
                                string dsTemp = DataHelper.GetJsonDataSet(dataSet);
                                dicOut.Add("dataSet", dsTemp);
                            }
                            else
                            {
                                if (dataSet.Tables.Contains(tableName))
                                {
                                    DataTable dtTemp = dataSet.Tables[tableName];
                                    string dataTable = DataHelper.GetJsonTable(dtTemp);
                                    dicOut.Add(tableName, dataTable);
                                }
                            }
                        }
                        else
                        {
                            errorCode = "err_invalidreqres";
                        }
                    }
                }
                else
                {
                    errorCode = "err_invalidreqres";
                }
                dicOut.Add("errorCode", errorCode);
                json = JsonConvert.SerializeObject(dicOut);
            }
            catch (Exception ex)
            {
                errorLog = ex.ToString();
            }
            finally
            {

            }
            if (errorLog == "")
                return Ok(json);
            else
                return BadRequest(errorLog);
        }

        //Delete api/patch
        [HttpDelete()]
        public ActionResult Delete([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbAutoPatchFilePath = null;
                string planCode = null;
                List<DataRow> dataRowList = null;
                if (dicInput.ContainsKey("dbAutoPatchFilePath"))
                {
                    dbAutoPatchFilePath = dicInput["dbAutoPatchFilePath"];
                    dicInput.Remove("dbAutoPatchFilePath");
                }

                if (dicInput.ContainsKey("planCode"))
                {
                    planCode = dicInput["planCode"];
                    dicInput.Remove("planCode");
                }
                if (dicInput.ContainsKey("dataTable"))
                {
                    string dataTableTemp = dicInput["dataTable"];
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(dataTableTemp);
                    dataRowList = DataHelper.GetDataRowList(dt);
                    dicInput.Remove("dataTable");
                }

                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbAutoPatchFilePath != null
                    && planCode != null)
                {
                    if (dataRowList != null)
                    {
                        AuthEngineHelper.DeletePatches(dataRowList, passCode,
                            dbAutoPatchFilePath, planCode, out errorCode);
                    }
                }
                else
                {
                    errorCode = "err_invalidreqres";
                }
                dicOut.Add("errorCode", errorCode);
                json = JsonConvert.SerializeObject(dicOut);
            }
            catch (Exception ex)
            {
                errorLog = ex.ToString();
            }
            finally
            {

            }
            if (errorLog == "")
                return Ok(json);
            else
                return BadRequest(errorLog);
        }

        //Delete api/patch/tableName
        [HttpDelete("{tableName}")]
        public ActionResult DeleteEx(string tableName, [FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbAutoPatchFilePath = null;
                string planCode = null;
                if (dicInput.ContainsKey("dbAutoPatchFilePath"))
                {
                    dbAutoPatchFilePath = dicInput["dbAutoPatchFilePath"];
                    dicInput.Remove("dbAutoPatchFilePath");
                }

                if (dicInput.ContainsKey("planCode"))
                {
                    planCode = dicInput["planCode"];
                    dicInput.Remove("planCode");
                }

                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbAutoPatchFilePath != null
                    && planCode != null &&
                    !string.IsNullOrEmpty(tableName))
                {
                    AuthEngineHelper.DeletePatches(tableName, dbAutoPatchFilePath,
                        passCode, planCode, out errorCode);
                }
                else
                {
                    errorCode = "err_invalidreqres";
                }
                dicOut.Add("errorCode", errorCode);
                json = JsonConvert.SerializeObject(dicOut);
            }
            catch (Exception ex)
            {
                errorLog = ex.ToString();
            }
            finally
            {

            }
            if (errorLog == "")
                return Ok(json);
            else
                return BadRequest(errorLog);
        }

        //Post api/patch
        [HttpPost()]
        public ActionResult Post([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbAutoPatchFilePath = null;
                string colName = null;
                string colValue = null;
                Dictionary<string, object> dicValue = null;
                if (dicInput.ContainsKey("dbAutoPatchFilePath"))
                {
                    dbAutoPatchFilePath = dicInput["dbAutoPatchFilePath"];
                    dicInput.Remove("dbAutoPatchFilePath");
                }
                if (dicInput.ContainsKey("colName"))
                {
                    colName = dicInput["colName"];
                    dicInput.Remove("colName");
                }
                if (dicInput.ContainsKey("colValue"))
                {
                    colValue = dicInput["colValue"];
                    dicInput.Remove("colValue");
                }
                if (dicInput.ContainsKey("dicValue"))
                {
                    string dicValueTemp = dicInput["dicValue"];
                    dicValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(dicValueTemp);
                    dicInput.Remove("dicValue");
                }

                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbAutoPatchFilePath != null)
                {
                    if (colName != null
                            && colValue != null
                            && dicValue != null)
                    {
                        AuthEngineHelper.AddEditPatches(
                             passCode,
                             colName,
                             colValue, dicValue,
                             dbAutoPatchFilePath,
                             out errorCode);
                    }
                    else
                    {
                        errorCode = "err_invalidreqres";
                    }
                }
                else
                {
                    errorCode = "err_invalidreqres";
                }
                dicOut.Add("errorCode", errorCode);
                json = JsonConvert.SerializeObject(dicOut);
            }
            catch (Exception ex)
            {
                errorLog = ex.ToString();
            }
            finally
            {

            }
            if (errorLog == "")
                return Ok(json);
            else
                return BadRequest(errorLog);
        }

        //Post api/patch/download
        [Route("download")]
        [HttpPost()]
        public ActionResult PostDownload([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbAutoPatchFilePath = null;
                Dictionary<string, object> dicValue = null;
                if (dicInput.ContainsKey("dbAutoPatchFilePath"))
                {
                    dbAutoPatchFilePath = dicInput["dbAutoPatchFilePath"];
                    dicInput.Remove("dbAutoPatchFilePath");
                }
                if (dicInput.ContainsKey("dicValue"))
                {
                    string dicValueTemp = dicInput["dicValue"];
                    dicValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(dicValueTemp);
                    dicInput.Remove("dicValue");

                }
                string errorCode = "";
                if (dbAutoPatchFilePath != null && dicValue != null)
                    LicenserEngine.AddDownloadRecord(dicValue, dbAutoPatchFilePath, out errorCode);
                else
                    errorCode = "err_invalidreqres";

                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                dicOut.Add("errorCode", errorCode);
                json = JsonConvert.SerializeObject(dicOut);
            }
            catch (Exception ex)
            {
                errorLog = ex.ToString();
            }
            finally
            {

            }
            if (errorLog == "")
                return Ok(json);
            else
                return BadRequest(errorLog);
        }

        //Post api/patch/maintenance
        [HttpPost("maintenance/{isNew}")]
        public ActionResult PostMaintenance(string isNew,
            [FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbAutoPatchFilePath = null;
                Dictionary<string, object> dicValue = null;
                DateTime dtNow = DateTime.Now;
                bool IsNew = isNew == "1";
                string userName = null;
                string selectedPlan = null;
                if (dicInput.ContainsKey("dbAutoPatchFilePath"))
                {
                    dbAutoPatchFilePath = dicInput["dbAutoPatchFilePath"];
                    dicInput.Remove("dbAutoPatchFilePath");
                }
                if (dicInput.ContainsKey("dicValue"))
                {
                    string dicValueTemp = dicInput["dicValue"];
                    dicValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(dicValueTemp);
                    dicInput.Remove("dicValue");
                }
                if (dicInput.ContainsKey("dtNow"))
                {
                    string dtNowTemp = dicInput["dtNow"];
                    dtNow = JsonConvert.DeserializeObject<DateTime>(dtNowTemp);
                    dicInput.Remove("dtNow");
                }
                if (dicInput.ContainsKey("userName"))
                {
                    userName = dicInput["userName"];
                    dicInput.Remove("userName");
                }

                if (dicInput.ContainsKey("selectedPlan"))
                {
                    selectedPlan = dicInput["selectedPlan"];
                    dicInput.Remove("selectedPlan");
                }

                string errorCode = "";

                if (dbAutoPatchFilePath != null && dicValue != null
                    && userName != null)
                {
                    AuthEngineHelper.AddEditMaintenance(dtNow, userName,
                        dicValue, IsNew, selectedPlan, dbAutoPatchFilePath,
                        passCode, out errorCode);
                }
                else
                    errorCode = "err_invalidreqres";

                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                dicOut.Add("errorCode", errorCode);
                json = JsonConvert.SerializeObject(dicOut);
            }
            catch (Exception ex)
            {
                errorLog = ex.ToString();
            }
            finally
            {

            }
            if (errorLog == "")
                return Ok(json);
            else
                return BadRequest(errorLog);
        }

        [HttpPost("upload/{filePath}")]
        [DisableRequestSizeLimit]
        public async System.Threading.Tasks.Task<ActionResult> UploadPatchFile(string filePath, IFormFile file)
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

                string filePathTemp = "C:";
                List<string> stList = filePath.Split('~').ToList();
                foreach (string st in stList)
                    filePathTemp = Path.Combine(filePathTemp, st);

                string fileName1 = Path.GetFileNameWithoutExtension(filePathTemp);
                if (!Startup.ProgressPatchDic.ContainsKey(fileName1))
                {
                    Startup.ProgressPatchDic.Add(fileName1, 0);
                }
                Startup.ProgressPatchDic[fileName1] = 0;

                long totalBytes = file.Length;
                ContentDispositionHeaderValue contentDispositionHeaderValue =
                        ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                string filename = contentDispositionHeaderValue.FileName.Trim('"');
                byte[] buffer = new byte[16 * 1024];

                string directory = Path.GetDirectoryName(filePathTemp);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                using (FileStream output = System.IO.File.Create(filePathTemp))
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
                            if (Startup.ProgressPatchDic[fileName1] != progress && progress > Startup.ProgressPatchDic[fileName1])
                            {
                                Startup.ProgressPatchDic[fileName1] = progress;
                                await System.Threading.Tasks.Task.Delay(100);
                            }
                        }
                    }
                }
                Startup.ProgressPatchDic.Remove(fileName1);
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

        [HttpGet("patchProgress/{filePath}")]
        public ActionResult ProgressPatch(string filePath)
        {
            if (filePath.Contains("~~"))
                filePath = filePath.Replace("~~", ".");

            string filePathTemp = "C:";
            List<string> stList = filePath.Split('~').ToList();
            foreach (string st in stList)
                filePathTemp = Path.Combine(filePathTemp, st);

            string fileName1 = Path.GetFileNameWithoutExtension(filePathTemp);
            int progress = 0;
            if (Startup.ProgressPatchDic.ContainsKey(fileName1))
                progress = Startup.ProgressPatchDic[fileName1];
            return Content(progress.ToString());
        }

        [HttpGet("fileSize/{filePath}")]
        public ActionResult FileSize(string filePath)
        {
            string errorLog = "";
            string json = "";
            try
            {
                List<string> stList = filePath.Split('~').ToList();
                string filePathTemp = "C:";
                foreach (string st in stList)
                {
                    filePathTemp = Path.Combine(filePathTemp, st);
                }

                bool isExists = System.IO.File.Exists(filePathTemp);
                string fileSize = "-1";
                if (isExists)
                {
                    FileInfo fi = new FileInfo(filePathTemp);
                    fileSize = fi.Length.ToString();
                }
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                dicOut.Add("fileSize", fileSize);
                json = JsonConvert.SerializeObject(dicOut);
            }
            catch (Exception ex)
            {
                errorLog = ex.ToString();
            }
            finally
            {

            }

            if (errorLog == "")
                return Ok(json);
            else
                return BadRequest(errorLog);
        }

        [HttpPut("downloadFile")]
        public ActionResult DownloadFile([FromBody] Dictionary<string, string> dicInput)
        {
            string errorLog = "";
            try
            {
                string filePath = null;
                if (dicInput.ContainsKey("filePath"))
                {
                    filePath = dicInput["filePath"];
                    dicInput.Remove("filePath");
                }
                if (filePath != null)
                {
                    System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    fs.CopyTo(ms);
                    fs.Close();
                    var contentType = "APPLICATION/octet-stream";
                    var fileName = Path.GetFileName(filePath);
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

        [HttpPut("deleteFile")]
        public ActionResult DeleteFile([FromBody] Dictionary<string, string> dicInput)
        {
            string errorLog = "";
            string json = "";
          
            try
            {
                string filePath = null;
                if (dicInput.ContainsKey("filePath"))
                {
                    filePath = dicInput["filePath"];
                    dicInput.Remove("filePath");
                }
                if (filePath != null)
                {
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                    else
                    {
                        Dictionary<string, string> dicOut = new Dictionary<string, string>();
                        dicOut.Add("errorCode", "err_invalidfile");
                        json = JsonConvert.SerializeObject(dicOut);
                    }
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
            if (errorLog == "")
                return Ok(json);
            else 
                return BadRequest(errorLog);
        }

        [HttpGet("fileList/{directoryPath}")]
        public ActionResult GetFileList(string directoryPath)
        {
            string errorLog = "";
            string json = "";
            try
            {
                List<string> stList = directoryPath.Split('~').ToList();
                string directoryPathTemp = "C:";
                foreach (string st in stList)
                {
                    directoryPathTemp = Path.Combine(directoryPathTemp, st);
                }

                if (directoryPathTemp != null)
                {
                    List<string> stList1 = Directory.GetFiles(directoryPathTemp).ToList();
                    string stList2 = JsonConvert.SerializeObject(stList1);
                    Dictionary<string, string> dicOut = new Dictionary<string, string>();
                    dicOut.Add("fileList", stList2);
                    json = JsonConvert.SerializeObject(dicOut);
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
                return Ok(json);
            else
                return BadRequest(errorLog);
        }

        //[HttpGet("directoryList/{directoryPath}")]
        //public ActionResult GetDirectoryList(string directoryPath)
        //{
        //    string errorLog = "";
        //    string json = "";
        //    try
        //    {
        //        List<string> stList = directoryPath.Split('~').ToList();
        //        string directoryPathTemp = "C:";
        //        foreach (string st in stList)
        //        {
        //            directoryPathTemp = Path.Combine(directoryPathTemp, st);
        //        }

        //        if (directoryPathTemp != null)
        //        {
        //            List<string> stList1 = Directory.GetDirectories(directoryPathTemp).ToList();
        //            string stList2 = JsonConvert.SerializeObject(stList1);
        //            Dictionary<string, string> dicOut = new Dictionary<string, string>();
        //            dicOut.Add("directoryList", stList2);
        //            json = JsonConvert.SerializeObject(dicOut);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        errorLog = ex.ToString();
        //    }
        //    finally
        //    {

        //    }

        //    if (errorLog == "")
        //        return Ok(json);
        //    else
        //        return BadRequest(errorLog);
        //}

        [HttpPut("deleteFiles")]
        public ActionResult DeleteFiles([FromBody] Dictionary<string, string> dicInput)
        {
            string errorLog = "";
            string json = "";

            try
            {
                List<string> filePaths = null;
                if (dicInput.ContainsKey("filePaths"))
                {
                    string files = dicInput["filePaths"];
                    dicInput.Remove("filePaths");
                    filePaths = JsonConvert.DeserializeObject<List<string>>(files);
                }
                if (filePaths != null)
                {
                    string errorCode = "";
                    foreach (string stFilePath in filePaths)
                    {
                        if (System.IO.File.Exists(stFilePath))
                            System.IO.File.Delete(stFilePath);
                        else
                        {
                            if (errorCode == "")
                                errorCode = "err_invalidfile";
                            
                        }
                    }
                    Dictionary<string, string> dicOut = new Dictionary<string, string>();
                    dicOut.Add("errorCode", errorCode);
                    json = JsonConvert.SerializeObject(dicOut);
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
            if (errorLog == "")
                return Ok(json);
            else
                return BadRequest(errorLog);
        }

    }
}
