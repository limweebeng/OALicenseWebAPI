using DataShared;
using Helper;
using Microsoft.AspNetCore.Hosting;
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
    public class LicenserController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private ILogger<LicenserController> _logger;
        private readonly string passCode = DataShared.Properties.Resources.licenser;

        #region Constructor
        public LicenserController(IWebHostEnvironment environment,
            ILogger<LicenserController> logger)
        {
            _hostingEnvironment = environment;
            _logger = logger;
        }
        #endregion

        //PATCH api/licenser/modifiedDate
        [HttpPatch("{modifiedDate}")]
        public ActionResult Patch(string modifiedDate, [FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string name = null;
                string password = null;
                string dbLicenserFilePath = null;
                DateTime dtNow = DateTime.Now;
                bool modifiedDateTemp = modifiedDate == "1";
                if (dicInput.ContainsKey("name"))
                {
                    name = dicInput["name"];
                    dicInput.Remove("name");
                }
                if (dicInput.ContainsKey("password"))
                {
                    password = dicInput["password"];
                    dicInput.Remove("password");
                }
                if (dicInput.ContainsKey("dbLicenserFilePath"))
                {
                    dbLicenserFilePath = dicInput["dbLicenserFilePath"];
                    dicInput.Remove("dbLicenserFilePath");
                }
                if (dicInput.ContainsKey("dtNow"))
                {
                    string dtNowTemp = dicInput["dtNow"];
                    dtNow = JsonConvert.DeserializeObject<DateTime>(dtNowTemp);
                    dicInput.Remove("dtNow");
                }

                string errorCode = "";
                DataRow dataRow = null;
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbLicenserFilePath != null)
                {
                    DataTable dataTable = SQLiteHelper.GetDataTable(dbLicenserFilePath, passCode,
                        LicenseData.usersTable, out errorCode);
                    if (errorCode == "" && dataTable != null)
                    { 
                        if (name != null && password != null)
                        {
                            dataRow = LicenserEngine.GetValidDataRow(dataTable,
                                          name, password,
                                          dtNow,
                                          out errorCode);
                        }
                        else
                        {
                            errorCode = "err_invalidreqres";
                        }
                        if (errorCode == "" && dataRow != null)
                        {
                            if (modifiedDateTemp)
                            {
                                SQLiteHelper.UpdateModifyDateTime(dbLicenserFilePath, passCode,
                                    LicenseData.usersTable, LicenseData.UserName, name,
                                    dtNow,
                                    out errorCode);
                            }
                            DataTable dt = DataHelper.ConvertDT(dataRow);
                            string json1 = JsonConvert.SerializeObject(dt);
                            dicOut.Add("userRow", json1);
                        }
                    }
                    else
                        errorCode = "err_invalidreqres";
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

        //PATCH api/licenser
        [HttpPatch()]
        public ActionResult Patch([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbLicenserFilePath = null;
                Dictionary<string, object> dicColCond = null;
                Dictionary<string, object> dicValue = null;
                if (dicInput.ContainsKey("dbLicenserFilePath"))
                {
                    dbLicenserFilePath = dicInput["dbLicenserFilePath"];
                    dicInput.Remove("dbLicenserFilePath");
                }
                if (dicInput.ContainsKey("dicColCond"))
                {
                    string dicColCondTemp = dicInput["dicColCond"];
                    dicInput.Remove("dicColCond");
                    dicColCond = JsonConvert.DeserializeObject<Dictionary<string, object>>(dicColCondTemp);
                }
                if (dicInput.ContainsKey("dicValue"))
                {
                    string dicValueTemp = dicInput["dicValue"];
                    dicInput.Remove("dicValue");
                    dicValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(dicValueTemp);
                }
                string errorCode = "";
                if (dbLicenserFilePath != null && dicColCond != null && dicValue != null)
                {
                    SQLiteHelper.UpdateData(dbLicenserFilePath, passCode, LicenseData.usersTable,
                                           dicColCond, dicValue, out errorCode);
                }
                else
                {
                    errorCode = "err_invalidreqres";
                }
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

        //PUT api/licenser/table
        [HttpPut("table/{tableName}")]
        public ActionResult Put(string tableName, [FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbLicenserFilePath = null;
                if (dicInput.ContainsKey("dbLicenserFilePath"))
                {
                    dbLicenserFilePath = dicInput["dbLicenserFilePath"];
                    dicInput.Remove("dbLicenserFilePath");
                }
                
                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbLicenserFilePath != null)
                {
                    DataSet dataSet = SQLiteHelper.GetDataSet(dbLicenserFilePath,
                        passCode, out errorCode);
                    if (errorCode == "" && dataSet != null)
                    {
                        if (tableName == "all")
                        {
                            string dsTemp = JsonConvert.SerializeObject(dataSet);
                            dicOut.Add("dataSet", dsTemp);
                        }
                        else
                        {
                            if (tableName != "")
                            {
                                if (dataSet.Tables.Contains(tableName))
                                {
                                    DataTable dtTemp = dataSet.Tables[tableName];
                                    string dataTable = JsonConvert.SerializeObject(dtTemp);
                                    dicOut.Add(tableName, dataTable);
                                }
                            }
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


    }
}
