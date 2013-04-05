using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Export;
using SasquatchCAIRS.Controllers.Service;
using SasquatchCAIRS.Models.Common;

namespace SasquatchCAIRS.Controllers.ViewControllers {
    /// <summary>
    ///     The Views used to generate new Reports
    /// </summary>
    [Authorize(Roles = Constants.Roles.REPORT_GENERATOR)]
    [ExcludeFromCodeCoverage]
    public class ReportController : Controller {
        /// <summary>
        ///     Default Report Generating page, showing a form with options for the report.
        /// </summary>
        /// <returns>The Reports View</returns>
        /// <request type="GET">/Report</request>
        public ActionResult Reports() {
            return View();
        }

        /// <summary>
        ///     The Page shown when no data is available
        /// </summary>
        /// <returns>The No Data View</returns>
        /// <request type="GET">/Report/NoDataView</request>
        public ActionResult NoDataView() {
            return View();
        }

        /// <summary>
        ///     The Page shown when the report is generated.
        /// </summary>
        /// <param name="form">The FormCollection with the various options chosen</param>
        /// <returns>The Report as an XLS file</returns>
        /// <request type="GET">/Report/Generate</request>
        public ViewResult GeneratingReport(FormCollection form) {
            var rg = new ReportManagementController();
            var eec = new ExcelExportController();

            string templatePath = Path.Combine(HttpRuntime.AppDomainAppPath,
                                               "ReportTemplate.xlsx");
            var markDate = new DateTime(2010, 01, 01, 00, 00, 00, 00);
            TimeSpan dateStamp = DateTime.Now.Subtract(markDate);

            string copiedPath = Path.Combine(HttpRuntime.AppDomainAppPath,
                                             "Report" +
                                             dateStamp.TotalSeconds.ToString() +
                                             ".xlsx");

            var dataTableDictionary = new Dictionary<string, DataTable>();

            List<string> dataTypeStrings =
                form[Constants.ReportFormStrings.DATATYPE].Split(',').ToList();
            List<Constants.DataType> dataTypes =
                dataTypeStrings.Select(
                    dType =>
                    (Constants.DataType)
                    Enum.Parse(typeof (Constants.DataType), dType)).ToList();

            List<string> stratifyOptionStrings =
                form[Constants.ReportFormStrings.STRATIFY_BY].Split(',')
                                                             .ToList();
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
                    DateTime endDate =
                        Convert.ToDateTime(form["todatePicker"]).AddMonths(1);
                    if (rg.checkForDataForMonth(startDate, endDate)) {
                        return View("NoDataView");
                    }
                    foreach (
                        Constants.StratifyOption stratifyOption in
                            stratifyOptions) {
                        temp = rg.generateMonthlyReport(startDate,
                                                        endDate,
                                                        dataTypes,
                                                        stratifyOption);
                        foreach (var keyValuePair in temp) {
                            dataTableDictionary.Add(keyValuePair.Key,
                                                    keyValuePair.Value);
                        }
                    }
                    break;
                case "MonthPerYear":
                    var month =
                        (Constants.Month)
                        Enum.Parse(typeof (Constants.Month), form["MPYMonth"]);
                    int startYear = Convert.ToInt32(form["MPYStartYear"]);
                    int endYear = Convert.ToInt32(form["MPYEndYear"]);
                    if (rg.checkForDataForMpy((int) month, startYear, endYear)) {
                        return View("NoDataView");
                    }
                    foreach (
                        Constants.StratifyOption stratifyOption in
                            stratifyOptions) {
                        temp = rg.generateMonthPerYearReport((int) month,
                                                             startYear,
                                                             endYear,
                                                             dataTypes,
                                                             stratifyOption);
                        foreach (var keyValuePair in temp) {
                            dataTableDictionary.Add(keyValuePair.Key,
                                                    keyValuePair.Value);
                        }
                    }
                    break;
                case "FiscalYear":
                    int start = Convert.ToInt32(form["FYStartYear"]);
                    int end = Convert.ToInt32(form["FYEndYear"]);
                    if (rg.checkForDataForFy(start, end)) {
                        return View("NoDataView");
                    }
                    foreach (
                        Constants.StratifyOption stratifyOption in
                            stratifyOptions) {
                        temp = rg.generateYearlyReport(start, end, dataTypes,
                                                       stratifyOption);
                        foreach (var keyValuePair in temp) {
                            dataTableDictionary.Add(keyValuePair.Key,
                                                    keyValuePair.Value);
                        }
                    }
                    break;
            }

            eec.exportDataTable(Constants.ReportType.Report, dataTableDictionary,
                                templatePath, copiedPath);

            return View();
        }
    }
}