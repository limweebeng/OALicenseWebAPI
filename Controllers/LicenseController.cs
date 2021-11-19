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
using System.Linq;

namespace OALicenseWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private ILogger<LicenseController> _logger;
        private readonly string passCode = DataShared.Properties.Resources.licenser;

        #region Constructor
        public LicenseController(IWebHostEnvironment environment,
            ILogger<LicenseController> logger)
        {
            _hostingEnvironment = environment;
            _logger = logger;
        }
        #endregion

        //Get api/license
        [HttpGet()]
        public ActionResult Get()
        {
            return Ok("Testing");
        }


        //Post api/license
        [HttpPost()]
        public ActionResult Post([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string fullName = "";
                string email = "";
                string company = "";
                string msgDisplay = "";

                string dbFilePath = null;
                string dbLicenserFilePath = null;
                string colName = null;
                string colValue = null;
                string compNameEx = null;
                string appFullName = null;
                DateTime dtNow = DateTime.Now;
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
                }
                if (dicInput.ContainsKey("dbLicenserFilePath"))
                {
                    dbLicenserFilePath = dicInput["dbLicenserFilePath"];
                    dicInput.Remove("dbLicenserFilePath");
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
                if (dicInput.ContainsKey("companyNameEx"))
                {
                    compNameEx = dicInput["companyNameEx"];
                    dicInput.Remove("companyNameEx");
                }
                if (dicInput.ContainsKey("appFullName"))
                {
                    appFullName = dicInput["appFullName"];
                    dicInput.Remove("appFullName");
                }
                if (dicInput.ContainsKey("dtNow"))
                {
                    string dtNowTemp = dicInput["dtNow"];
                    dtNow = JsonConvert.DeserializeObject<DateTime>(dtNowTemp);
                    dicInput.Remove("dtNow");
                }
                string errorCode = "";
                if (dbFilePath != null)
                {
                    if (colName != null && colValue != null)
                    {
                        DataRow userRow = AuthEngineHelper.RegisterLicense(dicInput,
                            colName, colValue,
                            dbFilePath,
                            passCode,
                            out fullName, out email,
                            out company, out msgDisplay, out errorCode);
                    }
                    else
                        errorCode = "err_invalidreqres";
                }
                else
                    errorCode = "err_invalidreqres";

                if (errorCode == "")
                {
                    if (dbLicenserFilePath != null)
                    {
                        DataSet dataSet = SQLiteHelper.GetDataSet(dbLicenserFilePath, passCode, out errorCode);
                        if (errorCode == "" && dataSet != null)
                        {
                            if (compNameEx != null && appFullName != null)
                            {
                                AuthEngineHelper.NotifyLicenser(
                                    dataSet, compNameEx, appFullName,
                                        fullName,
                                        company, email, dtNow);
                            }
                        }
                    }
                    else
                        errorCode = "err_invalidreqres";
                }
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                dicOut.Add("msgDisplay", msgDisplay);
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

        //Delete api/license
        [HttpDelete()]
        public ActionResult Delete([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            string dbFilePath = null;
            string colName = null;
            string colValue = null;
            DataTable dtSelected = null;
            bool isWebLicense = false;
            try
            {
                string errorCode = "";
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
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
                if (dicInput.ContainsKey("dtSelected"))
                {
                    string dtSelectedTemp = dicInput["dtSelected"];
                    dtSelected = JsonConvert.DeserializeObject<DataTable>(dtSelectedTemp);
                    dicInput.Remove("dtSelected");
                }
                if (dicInput.ContainsKey("webLicense"))
                {
                    string stWebLicense = dicInput["webLicense"];
                    isWebLicense = stWebLicense == "1";
                    dicInput.Remove("webLicense");
                }
                if (dbFilePath != null)
                {
                    if (dtSelected != null)
                    {
                        AuthEngineHelper.DeleteDBUser(dbFilePath, passCode,
                            dtSelected.AsEnumerable().ToList(),
                            isWebLicense,
                            out errorCode);
                    }
                    else
                    {
                        DataSet dataSet = SQLiteHelper.GetDataSet(dbFilePath, passCode, out errorCode);
                        if (errorCode == "")
                        {
                            DataRow userRow = AuthEngineHelper.Deactivate(dbFilePath,
                                passCode, colName, colValue, out errorCode);
                            if (errorCode == "")
                            {
                                if (userRow != null)
                                {
                                    SQLiteHelper.DeleteData(dbFilePath, passCode, LicenseData.usersTable, colName, colValue, out errorCode);
                                }
                                else
                                {
                                    errorCode = "err_mid";
                                }
                            }
                        }
                    }
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

        //Patch api/license/update
        [HttpPatch("{update}")]
        public ActionResult Patch(string update, [FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbFilePath = null;
                string colName = null;
                string colValue = null;
                DateTime dtNow = DateTime.Now;
                string patchNo = "";
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
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
                if (dicInput.ContainsKey("dtNow"))
                {
                    string dtNowTemp = dicInput["dtNow"];
                    dtNow = JsonConvert.DeserializeObject<DateTime>(dtNowTemp);
                    dicInput.Remove("dtNow");
                }
                if (dicInput.ContainsKey("patchNo"))
                {
                    patchNo = dicInput["patchNo"];
                    dicInput.Remove("patchNo");
                }

                string errorCode = "";
                DataRow userRow = null;
                if (dbFilePath != null &&
                    colName != null && colValue != null)
                {
                    userRow = SQLiteHelper.GetDataRow(dbFilePath, passCode,
                        LicenseData.usersTable, colName, colValue, out errorCode);
                }
                else
                {
                    errorCode = "err_invalidreqres";
                }

                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (errorCode == "" && userRow != null)
                {
                    if (update == "0" || update == "1")
                    {
                        if (update == "1")
                            AuthEngineHelper.GetLicenseOnline(dbFilePath, passCode,
                                dtNow, ref userRow, out errorCode);
                        DataTable dt = DataHelper.ConvertDT(userRow);
                        string json1 = JsonConvert.SerializeObject(dt);
                        dicOut.Add("userRow", json1);
                    }
                    else if (update == "2")
                    {
                        SQLiteHelper.UpdateData(dbFilePath, passCode,
                            LicenseData.usersTable, colName, colValue,
                            AppPatchEngine.patchNumber, patchNo, out errorCode);
                    }
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

        //Put api/license
        [HttpPut()]
        public ActionResult Put([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbFilePath = null;
                Dictionary<string, object> dicColCond = null;
                Dictionary<string, object> dicValue = null;
                string planCode = null;
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
                }
                if (dicInput.ContainsKey("planCode"))
                {
                    planCode = dicInput["planCode"];
                    dicInput.Remove("planCode");
                }
                if (dicInput.ContainsKey("dicColCond"))
                {
                    string dicColCondTemp = dicInput["dicColCond"];
                    dicColCond = JsonConvert.DeserializeObject<Dictionary<string, object>>(dicColCondTemp);
                    dicInput.Remove("dicColCond");
                }
                if (dicInput.ContainsKey("dicValue"))
                {
                    string dicValueTemp = dicInput["dicValue"];
                    dicValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(dicValueTemp);
                    dicInput.Remove("dicValue");
                }


                string errorCode = "";
                if (dbFilePath != null && dicColCond != null &&
                    dicValue != null)
                {
                    SQLiteHelper.UpdateData(dbFilePath, passCode,
                                                 LicenseData.usersTable,
                                                 dicColCond, dicValue,
                                                 out errorCode);
                    if (errorCode == "")
                    {
                        SQLiteHelper.UpdateData(dbFilePath, passCode,
                                           LicenseData.usersTable,
                                           AppPatchEngine.planCode,
                                           planCode, AppPatchEngine.planCode, "", out errorCode);
                    }
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

        //Post api/license/user
        [HttpPost("user/{isNew}")]
        public ActionResult PostUser(string isNew, [FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                bool IsNew = isNew == "1";
                string dbFilePath = null;
                Dictionary<string, object> dicOut = null;
                string colCond = null;
                string colValueCond = null;
                Dictionary<string, string> infoDic = null;
                List<string> stCols = null;
                List<string> stValues = null;
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
                }
                if (dicInput.ContainsKey("dicOut"))
                {
                    string dicOutTemp = dicInput["dicOut"];
                    dicOut = JsonConvert.DeserializeObject<Dictionary<string, object>>(dicOutTemp);
                    dicInput.Remove("dicOut");
                }
                if (dicInput.ContainsKey("colCond"))
                {
                    colCond = dicInput["colCond"];
                    dicInput.Remove("colCond");
                }
                if (dicInput.ContainsKey("colValueCond"))
                {
                    colValueCond = dicInput["colValueCond"];
                    dicInput.Remove("colValueCond");
                }
                if (dicInput.ContainsKey("infoDic"))
                {
                    string infoDicTemp = dicInput["infoDic"];
                    infoDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(infoDicTemp);
                    dicInput.Remove("infoDic");
                }
                if (dicInput.ContainsKey("stCols"))
                {
                    string stColsTemp = dicInput["stCols"];
                    stCols = JsonConvert.DeserializeObject<List<string>>(stColsTemp);
                    dicInput.Remove("stCols");
                }
                if (dicInput.ContainsKey("stValues"))
                {
                    string stValuesTemp = dicInput["stValues"];
                    stValues = JsonConvert.DeserializeObject<List<string>>(stValuesTemp);
                    dicInput.Remove("stValues");
                }

                string errorCode = "";
                Dictionary<string, string> dicOutput = new Dictionary<string, string>();
                if (dbFilePath != null)
                {
                    if (IsNew)
                    {
                        string objValue = AuthEngineHelper.AddLicenseUser(dbFilePath,
                                            passCode, colCond, colValueCond, infoDic, dicOut, out errorCode);
                        if (errorCode != "")
                        {
                            dicOutput.Add("objValue", objValue);
                        }
                    }
                    else
                    {
                        AuthEngineHelper.EditLicenseUser(dbFilePath,
                                            passCode, stCols, stValues, dicOut, out errorCode);
                    }
                }
                else
                    errorCode = "err_invalidreqres";

                dicOutput.Add("errorCode", errorCode);
                json = JsonConvert.SerializeObject(dicOutput);
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


        //Put api/license/table
        [HttpPut("table/{tableName}")]
        public ActionResult PutTable(string tableName, [FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbFilePath = null;
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
                }

                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbFilePath != null) 
                {
                    DataSet ds = SQLiteHelper.GetDataSet(dbFilePath, passCode, out errorCode);
                    if (errorCode == "")
                    {
                        if (tableName != "")
                        {
                            if (tableName == "all")
                            {
                                string dsTemp = DataHelper.GetJsonDataSet(ds);
                                dicOut.Add("dataSet", dsTemp);
                            }
                            else
                            {
                                DataTable dt = ds.Tables[tableName];
                                if (dt != null)
                                {
                                    string dtTemp = DataHelper.GetJsonTable(dt);
                                    dicOut.Add(tableName, dtTemp);
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


        //Put api/license/group
        [Route("group")]
        [HttpPut()]
        public ActionResult PutGroup([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbFilePath = null;
                
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
                }

                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbFilePath != null)
                {
                    DataTable dtGroup = AuthEngineHelper.GetDataTableEx(dbFilePath,
                           passCode, LicenseData.Group, out errorCode);
                    if (errorCode == "" && dtGroup != null)
                    {
                        string json1 = DataHelper.GetJsonTable(dtGroup);
                        dicOut.Add("dtGroup", json1);
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

        //Post api/license/group
        [Route("group")]
        [HttpPost()]
        public ActionResult PostGroup([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbFilePath = null;
                DataTable dtGroup = null;
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
                }
                if (dicInput.ContainsKey("dtGroup"))
                {
                    string dtGroupTemp = dicInput["dtGroup"];
                    dtGroup = JsonConvert.DeserializeObject<DataTable>(dtGroupTemp);
                    dicInput.Remove("dtGroup");
                }

                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbFilePath != null)
                {
                    DataSet dataSet = SQLiteHelper.GetDataSet(dbFilePath, passCode, out errorCode);
                    if (errorCode == "")
                    {
                        if (dtGroup != null)
                        {
                            SQLiteHelper.ModifyTable(dbFilePath, passCode, dtGroup, LicenseData.Group, out errorCode);
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

        //Post api/license/scheme/isNew
        [HttpPost("scheme/{isNew}")]
        public ActionResult PostScheme(string isNew, [FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                bool IsNew = isNew == "1";
                string dbFilePath = null;
                Dictionary<string, object> dicObj = null;
                string colCondName= null;
                string colCondValue = null;
                string colName = null;
                string colValue = null;
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
                }
                if (dicInput.ContainsKey("dicObj"))
                {
                    string dicObjTemp = dicInput["dicObj"];
                    dicObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(dicObjTemp);
                    dicInput.Remove("dicObj");
                }
                if (dicInput.ContainsKey("colCondName"))
                {
                    colCondName = dicInput["colCondName"];
                    dicInput.Remove("colCondName");
                }
                if (dicInput.ContainsKey("colCondValue"))
                {
                    colCondValue = dicInput["colCondValue"];
                    dicInput.Remove("colCondValue");
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

                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbFilePath != null)
                {
                    string tableName = LicenseData.Scheme;
                    DataTable dtScheme = SQLiteHelper.GetDataTable(dbFilePath, 
                        passCode, tableName,
                        out errorCode);
                    if (errorCode == "")
                    {
                        if (dtScheme != null)
                        {
                            if (IsNew)
                            {
                                if (dicObj != null)
                                    SQLiteHelper.InsertNewData(dbFilePath, passCode, dtScheme, dicObj, out errorCode);
                            }
                            else
                            {
                                if (colCondName != null &&
                                    colCondValue != null &&
                                    colName != null &&
                                    colValue != null)
                                {
                                    SQLiteHelper.UpdateData(dbFilePath,
                                            passCode,
                                            LicenseData.Scheme,
                                            colCondName,
                                            colCondValue,
                                            colName,
                                            colValue,
                                            out errorCode);
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


        //Put api/license/scheme
        [Route("scheme")]
        [HttpPatch()]
        public ActionResult PatchScheme([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbFilePath = null;
                string colCondName = null;
                string colCondValue = null;
                string colName = null;
                string colValue = null;
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
                }
                if (dicInput.ContainsKey("colCondName"))
                {
                    colCondName = dicInput["colCondName"];
                    dicInput.Remove("colCondName");
                }
                if (dicInput.ContainsKey("colCondValue"))
                {
                    colCondValue = dicInput["colCondValue"];
                    dicInput.Remove("colCondValue");
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

                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbFilePath != null)
                {
                    if (colCondName != null && colCondValue != null
                        && colName != null && colValue != null)
                    {
                        SQLiteHelper.UpdateData(dbFilePath, 
                            passCode, LicenseData.Scheme,
                            colCondName, colCondValue,
                            colName , colValue,
                            out errorCode);
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

        //Put api/license/scheme
        [Route("scheme")]
        [HttpPut()]
        public ActionResult PutScheme([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbFilePath = null;
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
                }

                string errorCode = "";
                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                if (dbFilePath != null)
                {
                    DataTable dtScheme = AuthEngineHelper.GetDataTableEx(dbFilePath,
                            passCode, LicenseData.Scheme, out errorCode);
                    if (errorCode == "" && dtScheme != null)
                    {
                        string json1 = DataHelper.GetJsonTable(dtScheme);
                        dicOut.Add("dtScheme", json1);
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

        //Delete api/license/scheme
        [Route("scheme")]
        [HttpDelete()]
        public ActionResult DeleteScheme([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            string dbFilePath = null;
            string colName = null;
            string colValue = null;
            string errorCode = "";
            try
            {
                if (dicInput.ContainsKey("dbFilePath"))
                {
                    dbFilePath = dicInput["dbFilePath"];
                    dicInput.Remove("dbFilePath");
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
               
                if (dbFilePath != null)
                {
                    SQLiteHelper.DeleteData(dbFilePath, passCode, LicenseData.Scheme, colName, colValue, out errorCode);
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
    
    }
}

