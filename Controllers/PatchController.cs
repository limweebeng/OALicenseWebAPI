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
                                string json1 = JsonConvert.SerializeObject(dt);
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
                                string dsTemp = JsonConvert.SerializeObject(dataSet);
                                dicOut.Add("dataSet", dsTemp);
                            }
                            else
                            {
                                if (dataSet.Tables.Contains(tableName))
                                {
                                    DataTable dtTemp = dataSet.Tables[tableName];
                                    string dataTable = JsonConvert.SerializeObject(dtTemp);
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

    }
}
