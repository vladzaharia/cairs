using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;
using System.Data;
using System.IO;


namespace SasquatchCAIRS.Controllers
{
    public class ServiceSystemController : Controller
    {
        //
        // GET: /ServiceSystem/Reports
        //[Authorize (Roles = "ReportGenerator")]
        public ActionResult Reports()
        {
            return View();
        }

        public ViewResult GenaratingReport(FormCollection form) {
            //string path = Server.MapPath("~/Report.xlsx").ToString();
            string templatePath = Server.MapPath("~/ReportTemplate.xlsx");
            DateTime markDate = new DateTime(2010,01,01, 00, 00, 00, 00);
            TimeSpan dateStamp = DateTime.Now.Subtract(markDate);

            string copiedPath = Server.MapPath("~/Report"+dateStamp.TotalSeconds.ToString()+".xlsx");
            ViewBag.form = templatePath;
            List<DataTable> dataTables = new List<DataTable>();

            List<string> dataTypeStrings = form["dataType"].Split(',').ToList();
            List<Constants.DataType> dataTypes =
                dataTypeStrings.Select(
                    dType =>
                    (Constants.DataType)
                    Enum.Parse(typeof (Constants.DataType), dType)).ToList();

            List<string> stratifyOptionStrings = form["stratigyBy"].Split(',').ToList();
            List<Constants.StratifyOption> stratifyOptions =
                stratifyOptionStrings.Select(
                    stratify =>
                    (Constants.StratifyOption)
                    Enum.Parse(typeof (Constants.StratifyOption), stratify))
                                     .ToList();

            ReportController rg = new ReportController();
            ExcelExportController eec = new ExcelExportController();

            switch (form["reportOption"]) {
                case "Monthly":
                    DateTime startDate =
                        Convert.ToDateTime(form["fromdatePicker"]);
                    DateTime endDate = Convert.ToDateTime(form["todatePicker"]);
                    foreach (Constants.StratifyOption stratifyOption in stratifyOptions) {
                        dataTables.AddRange(rg.generateMonthlyReport(startDate, endDate,
                                                          dataTypes,
                                                          stratifyOption));
                    }
                    break;
                case "MonthPerYear":
                    Constants.Month month =
                        (Constants.Month)
                        Enum.Parse(typeof(Constants.Month), form["MPYMonth"]);
                    int startYear = Convert.ToInt32(form["MPYStartYear"]);
                    int endYear = Convert.ToInt32(form["MPYEndYear"]);
                    foreach (Constants.StratifyOption stratifyOption in stratifyOptions) {
                        dataTables.AddRange(rg.generateMonthPerYearReport((int) month,
                                                               startYear,
                                                               endYear,
                                                               dataTypes,
                                                               stratifyOption));
                    }
                    break;
                case "FiscalYear":
                    int start = Convert.ToInt32(form["FYStartYear"]);
                    int end = Convert.ToInt32(form["FYEndYear"]);
                    foreach (Constants.StratifyOption stratifyOption in stratifyOptions) {
                        dataTables.AddRange(rg.generateYearlyReport(start, end, dataTypes,
                                                         stratifyOption));
                    }
                    break;
            }
            
            eec.exportDataTable(Constants.ReportType.Report,dataTables, templatePath,copiedPath);

            return View();
        }
    }
}
