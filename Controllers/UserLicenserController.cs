
using DataShared;
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
using System.Linq;
using System.Threading.Tasks;

namespace OALicenseWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLicenserController : ControllerBase
    {
        private IWebHostEnvironment _hostingEnvironment;
        private ILogger<UserLicenserController> _logger;
        private readonly string passCode = DataShared.Properties.Resources.licenser;

        #region Constructor
        public UserLicenserController(IWebHostEnvironment environment,
            ILogger<UserLicenserController> logger)
        {
            _hostingEnvironment = environment;
            _logger = logger;
        }
        #endregion

        //Patch api/userlicenser
        [HttpPatch()]
        public ActionResult Patch([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbLicenserFilePath = null;
                string colNameCond = null;
                string colValueCond = null;
                string colName = null;
                string colValue = null;
                if (dicInput.ContainsKey("dbLicenserFilePath"))
                {
                    dbLicenserFilePath = dicInput["dbLicenserFilePath"];
                    dicInput.Remove("dbLicenserFilePath");
                }
                if (dicInput.ContainsKey("colNameCond"))
                {
                    colNameCond = dicInput["colNameCond"];
                    dicInput.Remove("colNameCond");
                }
                if (dicInput.ContainsKey("colValueCond"))
                {
                    colValueCond = dicInput["colValueCond"];
                    dicInput.Remove("colValueCond");
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
                if (dbLicenserFilePath != null)
                {
                    SQLiteHelper.UpdateData(dbLicenserFilePath, passCode,
                                        LicenseData.usersTable,
                                        colNameCond, colValueCond,
                                        colName, colValue,
                                        out errorCode);
                }
                else
                    errorCode = "err_invalidreqres";

                Dictionary<string, string> dicOutput = new Dictionary<string, string>();
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


        //Put api/userlicenser
        [HttpPut()]
        public ActionResult Put([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbLicenserFilePath = null;
                string colNameCond = null;
                string colValueCond = null;
                if (dicInput.ContainsKey("dbLicenserFilePath"))
                {
                    dbLicenserFilePath = dicInput["dbLicenserFilePath"];
                    dicInput.Remove("dbLicenserFilePath");
                }
                if (dicInput.ContainsKey("colNameCond"))
                {
                    colNameCond = dicInput["colNameCond"];
                    dicInput.Remove("colNameCond");
                }
                if (dicInput.ContainsKey("colValueCond"))
                {
                    colValueCond = dicInput["colValueCond"];
                    dicInput.Remove("colValueCond");
                }


                Dictionary<string, string> dicOutput = new Dictionary<string, string>();
                string errorCode = "";
                if (dbLicenserFilePath != null)
                {
                    DataRow userRow = SQLiteHelper.GetDataRow(dbLicenserFilePath, passCode,
                        LicenseData.usersTable, colNameCond, colValueCond, out errorCode);
                    if (errorCode == "")
                    {
                        if (userRow != null)
                        {
                            DataTable dt = DataHelper.ConvertDT(userRow);
                            string dtTemp = JsonConvert.SerializeObject(dt);
                            dicOutput.Add("userRow", dtTemp);
                        }
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

        //Post api/userlicenser/isNew
        [HttpPost("{isNew}")]
        public ActionResult Post(string isNew, [FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                bool IsNew = isNew == "1";
                string dbLicenserFilePath = null;
                DateTime dtNow = DateTime.Now;
                Dictionary<string, object> dicOut = null;
                List<string> stCols = null;
                List<string> stValues = null;
                if (dicInput.ContainsKey("dbLicenserFilePath"))
                {
                    dbLicenserFilePath = dicInput["dbLicenserFilePath"];
                    dicInput.Remove("dbLicenserFilePath");
                }
                if (dicInput.ContainsKey("dtNow"))
                {
                    string dtNowTemp = dicInput["dtNow"];
                    dicInput.Remove("dtNow");
                    dtNow = JsonConvert.DeserializeObject<DateTime>(dtNowTemp);
                }
                if (dicInput.ContainsKey("dicOut"))
                {
                    string dicOutTemp = dicInput["dicOut"];
                    dicInput.Remove("dicOut");
                    dicOut = JsonConvert.DeserializeObject<Dictionary<string, object>>(dicOutTemp);
                }
                if (dicInput.ContainsKey("stCols"))
                {
                    string stColsTemp = dicInput["stCols"];
                    dicInput.Remove("stCols");
                    stCols = JsonConvert.DeserializeObject<List<string>>(stColsTemp);
                }
                if (dicInput.ContainsKey("stValues"))
                {
                    string stValuesTemp = dicInput["stValues"];
                    dicInput.Remove("stValues");
                    stValues = JsonConvert.DeserializeObject<List<string>>(stValuesTemp);
                }

                string errorCode = "";
                if (dbLicenserFilePath != null)
                {
                    if (IsNew)
                    {
                        if (dicOut != null)
                        {
                            AuthEngineHelper.AddLicenserUser(dbLicenserFilePath, 
                                passCode,
                                dtNow, dicOut, out errorCode);
                        }
                    }
                    else
                    {
                        if (dicOut != null && stCols != null && stValues != null)
                        {
                            AuthEngineHelper.EditLicenserUser(dbLicenserFilePath,
                            passCode, stCols, stValues, dtNow, dicOut, out errorCode);
                        }
                    }
                }
                else
                    errorCode = "err_invalidreqres";

                Dictionary<string, string> dicOutput = new Dictionary<string, string>();
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

        //Post api/userlicenser
        [HttpDelete()]
        public ActionResult Delete([FromBody] Dictionary<string, string> dicInput)
        {
            string json = "";
            string errorLog = "";
            try
            {
                string dbLicenserFilePath = null;
                DataTable dtSelected = null;
               
                if (dicInput.ContainsKey("dbLicenserFilePath"))
                {
                    dbLicenserFilePath = dicInput["dbLicenserFilePath"];
                    dicInput.Remove("dbLicenserFilePath");
                }
                if (dicInput.ContainsKey("dtSelected"))
                {
                    string dtSelectedTemp = dicInput["dtSelected"];
                    dicInput.Remove("dtSelected");
                    dtSelected = JsonConvert.DeserializeObject<DataTable>(dtSelectedTemp);
                }
             

                string errorCode = "";
                if (dbLicenserFilePath != null && dtSelected != null)
                {
                    AuthEngineHelper.DeleteLicenserUser(dbLicenserFilePath, passCode,
                            dtSelected.AsEnumerable().ToList(), out errorCode);
                }
                else
                    errorCode = "err_invalidreqres";

                Dictionary<string, string> dicOutput = new Dictionary<string, string>();
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


    }
}
