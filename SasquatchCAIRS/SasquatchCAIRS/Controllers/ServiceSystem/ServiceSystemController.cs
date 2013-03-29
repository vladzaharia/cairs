﻿using System;
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
    [Authorize (Roles = Constants.Roles.REPORT_GENERATOR)]
    public class ServiceSystemController : Controller
    {
        //
        // GET: /ServiceSystem/Reports
        public ActionResult Reports()
        {
            return View();
        }

        public ActionResult NoDataView() {
            return View();
        }

        public ViewResult GeneratingReport(FormCollection form) {

            ReportController rg = new ReportController();
            ExcelExportController eec = new ExcelExportController();


            string templatePath = Path.Combine(HttpRuntime.AppDomainAppPath, "ReportTemplate.xlsx");
            DateTime markDate = new DateTime(2010,01,01, 00, 00, 00, 00);
            TimeSpan dateStamp = DateTime.Now.Subtract(markDate);

            string copiedPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Report" + dateStamp.TotalSeconds.ToString() + ".xlsx");
            
            Dictionary<string, DataTable> dataTableDictionary = new Dictionary<string, DataTable>();

            List<string> dataTypeStrings = form[Constants.ReportFormStrings.DATATYPE].Split(',').ToList();
            List<Constants.DataType> dataTypes =
                dataTypeStrings.Select(
                    dType =>
                    (Constants.DataType)
                    Enum.Parse(typeof (Constants.DataType), dType)).ToList();

            List<string> stratifyOptionStrings = form[Constants.ReportFormStrings.STRATIFY_BY].Split(',').ToList();
            List<Constants.StratifyOption> stratifyOptions =
                stratifyOptionStrings.Select(
                    stratify =>
                    (Constants.StratifyOption)
                    Enum.Parse(typeof (Constants.StratifyOption), stratify))
                                     .ToList();

            Dictionary<string, DataTable> temp;

            switch (form[Constants.ReportFormStrings.REPORT_OPTION]) {
                case "Monthly":
                    DateTime startDate =
                        Convert.ToDateTime(form["fromdatePicker"]);
                    DateTime endDate = Convert.ToDateTime(form["todatePicker"]).AddMonths(1);
                    if (rg.checkForDataForMonth(startDate, endDate)) {
                        return View("NoDataView");
                    }
                    foreach (Constants.StratifyOption stratifyOption in stratifyOptions) {
                        temp = rg.generateMonthlyReport(startDate,
                                                        endDate,
                                                        dataTypes,
                                                        stratifyOption);
                        foreach (KeyValuePair<string, DataTable> keyValuePair in temp) {
                            dataTableDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                    break;
                case "MonthPerYear":
                    Constants.Month month =
                        (Constants.Month)
                        Enum.Parse(typeof(Constants.Month), form["MPYMonth"]);
                    int startYear = Convert.ToInt32(form["MPYStartYear"]);
                    int endYear = Convert.ToInt32(form["MPYEndYear"]);
                    if (rg.checkForDataForMpy((int)month, startYear, endYear)) {
                        return View("NoDataView");
                    }
                    foreach (Constants.StratifyOption stratifyOption in stratifyOptions) {
                        temp = rg.generateMonthPerYearReport((int) month,
                                                             startYear,
                                                             endYear,
                                                             dataTypes,
                                                             stratifyOption);
                        foreach (KeyValuePair<string, DataTable> keyValuePair in temp) {
                            dataTableDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                    break;
                case "FiscalYear":
                    int start = Convert.ToInt32(form["FYStartYear"]);
                    int end = Convert.ToInt32(form["FYEndYear"]);
                    if (rg.checkForDataForFy(start, end)) {
                        return View("NoDataView");
                    }
                    foreach (Constants.StratifyOption stratifyOption in stratifyOptions) {
                        temp = rg.generateYearlyReport(start, end, dataTypes,
                                                        stratifyOption);
                        foreach (KeyValuePair<string, DataTable> keyValuePair in temp) {
                            dataTableDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                    break;
            }

            eec.exportDataTable(Constants.ReportType.Report, dataTableDictionary, templatePath, copiedPath);

            return View();
        }
    }
}
