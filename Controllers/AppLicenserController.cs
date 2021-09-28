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
    public class AppLicenserController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private ILogger<AppLicenserController> _logger;
        private readonly string passCode = DataShared.Properties.Resources.licenser;

        #region Constructor
        public AppLicenserController(IWebHostEnvironment environment,
            ILogger<AppLicenserController> logger)
        {
            _hostingEnvironment = environment;
            _logger = logger;
        }
        #endregion

        //PUT api/applicenser
        [HttpPut()]
        public ActionResult Put([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbLicenserFilePath = null;
                string tableName = "";
                if (dicInput.ContainsKey("dbLicenserFilePath"))
                {
                    dbLicenserFilePath = dicInput["dbLicenserFilePath"];
                    dicInput.Remove("dbLicenserFilePath");
                }
                if (dicInput.ContainsKey("tableName"))
                {
                    tableName = dicInput["tableName"];
                    dicInput.Remove("tableName");
                }
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                string errorCode = "";
                if (dbLicenserFilePath != null)
                {
                    if (tableName == "tableNameList")
                    {
                        List<string> tableNameList = SQLiteHelper.GetTableNames(dbLicenserFilePath, passCode, out errorCode);
                        string json1 = JsonConvert.SerializeObject(tableNameList);
                        dicOut.Add("tableNameList", json1);
                    }
                    else
                    {
                        DataSet dataSet = SQLiteHelper.GetDataSet(dbLicenserFilePath, passCode, out errorCode);
                        if (tableName != "")
                        {
                            if (tableName == "all")
                            {
                                string json1 = DataHelper.GetJsonDataSet(dataSet);
                                dicOut.Add("dataSet", json1);
                            }
                            else
                            {
                                DataTable dt;
                                dt = dataSet.Tables[tableName];
                                
                                string json1 = DataHelper.GetJsonTable(dt);
                                dicOut.Add(tableName, json1);
                            }
                        }
                    }
                }
                else
                    errorCode = "err_invalidreqres";
                
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

        [HttpPatch()]
        public ActionResult Patch([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbLicenserFilePath = null;
                Dictionary<string, object> dicOut = null;
                List<string> values = null;
                if (dicInput.ContainsKey("dbLicenserFilePath"))
                {
                    dbLicenserFilePath = dicInput["dbLicenserFilePath"];
                    dicInput.Remove("dbLicenserFilePath");
                }
                if (dicInput.ContainsKey("dicOut"))
                {
                    string dicOutTemp = dicInput["dicOut"];
                    dicOut = JsonConvert.DeserializeObject<Dictionary<string, object>>(dicOutTemp); 
                    dicInput.Remove("dicOut");
                }
                if (dicInput.ContainsKey("values"))
                {
                    string valuesTemp = dicInput["values"];
                    values = JsonConvert.DeserializeObject<List<string>>(valuesTemp); 
                    dicInput.Remove("values");
                }

                string errorCode = "";
                if (dbLicenserFilePath != null && dicOut != null && values != null)
                {
                    LicenserEngine.ApplyAppAllocation(dicOut, values, dbLicenserFilePath, out errorCode);
                }
                else 
                    errorCode = "err_invalidreqres";

                Dictionary<string, string> dicTemp = new Dictionary<string, string>();
                dicTemp.Add("errorCode", errorCode);
                json = JsonConvert.SerializeObject(dicTemp);
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
