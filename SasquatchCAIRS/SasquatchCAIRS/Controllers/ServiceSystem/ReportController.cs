using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers
{
    public class ReportController : Controller
    {
        private static readonly ReportController _instance =
            new ReportController();
        private CAIRSDataContext _db = new CAIRSDataContext();

        private ReportController() {
        }

        public static ReportController instance {
            get {
                return _instance;
            }
        }

        /// <summary>
        /// creates list of dataTables to be exported
        /// </summary>
        /// <param name="startDate">start date, selected by the user</param>
        /// <param name="endDate">end date, selected by the user</param>
        /// <param name="dataToDisplay">date Types to represent, selected by the user</param>
        /// <param name="stratifyBy">stratify option, selected by the user</param>
        /// <returns></returns>
        public IEnumerable<DataTable> generateMonthlyReport(DateTime startDate, DateTime endDate,
                                                      IEnumerable<Constants.DataType> dataToDisplay,
                                                      Constants.StratifyOption stratifyBy)
        {
            var dataTablesForReport = new List<DataTable>();

            //executes different methods depending on the stratify options selected
            switch (stratifyBy) {
                case Constants.StratifyOption.Region:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the region
                    Dictionary<byte?, List<Request>> regionDictionary = (from reqs in _db.Requests
                                                                         where
                                                                             reqs.TimeOpened > startDate &&
                                                                             reqs.TimeOpened <= endDate 
                                                                         group reqs by reqs.RegionID
                                                                             into regionGroups
                                                                             select regionGroups).ToDictionary(r => r.Key,
                                                                                                           r =>
                                                                                                           r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<byte?, Dictionary<MonthYearPair, List<Request>>> regionAndYear =
                        regionDictionary.ToDictionary(keyValuePair => keyValuePair.Key,
                                                      keyValuePair =>
                                                      keyValuePair.Value.GroupBy(r => new MonthYearPair(r.TimeOpened.Year, r.TimeOpened.Month))
                                                                  .Select(grp => grp)
                                                                  .ToDictionary(grp => grp.Key, grp => grp.ToList()));

                    //creates dataTable for each dataType and add them to the dataTable list
                    dataTablesForReport.AddRange(
                        dataToDisplay.Select(
                            dataType => createDtForEachMonth(startDate, endDate, stratifyBy, dataType, regionAndYear)));

                    break;
                case Constants.StratifyOption.CallerType:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the callerType
                    Dictionary<byte?, List<Request>> callerDictionary = (from reqs in _db.Requests
                                                                         where
                                                                             reqs.TimeOpened > startDate &&
                                                                             reqs.TimeOpened <= endDate
                                                                         group reqs by reqs.RequestorTypeID
                                                                             into callerGroups
                                                                             select callerGroups).ToDictionary(r => r.Key,
                                                                                                           r =>
                                                                                                           r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<byte?, Dictionary<MonthYearPair, List<Request>>> callerAndYear =
                        callerDictionary.ToDictionary(keyValuePair => keyValuePair.Key,
                                                      keyValuePair =>
                                                      keyValuePair.Value.GroupBy(r => new MonthYearPair(r.TimeOpened))
                                                                  .Select(grp => grp)
                                                                  .ToDictionary(grp => grp.Key, grp => grp.ToList()));

                    ////creates dataTable for each dataType and add them to the dataTable list
                    dataTablesForReport.AddRange(
                        dataToDisplay.Select(
                            dataType => createDtForEachMonth(startDate, endDate, stratifyBy, dataType, callerAndYear)));
                    break;
                case Constants.StratifyOption.TumorGroup:
                    //Retrieves the QuestionResponse from the database which opened within the given timeFrame,
                    //adds the open, close timestamps, then group them by the tumourGroup
                    Dictionary<byte?, List<QandRwithTimestamp>> qrTumourGrpDic =
                        (from reqs in _db.Requests
                         where
                             reqs.TimeOpened > startDate &&
                             reqs.TimeOpened <= endDate
                         join qr in _db.QuestionResponses on reqs.RequestID
                             equals qr.RequestID
                         select
                             new QandRwithTimestamp(qr, reqs.TimeOpened,
                                                    reqs.TimeClosed)).GroupBy(
                                                        q => q.qr.TumourGroupID)
                                                                     .ToDictionary
                            (grp => grp.Key, grp => grp.ToList());

                    //Sub-groups the regionGroups by the year the question(request) is opened.
                    Dictionary<byte?, Dictionary<MonthYearPair, List<QandRwithTimestamp>>> tgAndYear =
                        qrTumourGrpDic.ToDictionary(keyValuePair => keyValuePair.Key,
                                                      keyValuePair =>
                                                      keyValuePair.Value.GroupBy(r => new MonthYearPair(r.timeOpened))
                                                                  .Select(grp => grp)
                                                                  .ToDictionary(grp => grp.Key, grp => grp.ToList()));

                    dataTablesForReport.AddRange(
                        dataToDisplay.Select(
                            dataType => createDtForEachMonth(startDate, endDate, stratifyBy, dataType, tgAndYear)));

                    break;
                case Constants.StratifyOption.None:
                default:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the year
                    Dictionary<MonthYearPair, List<Request>> dictionaryByMonth = (from reqs in _db.Requests
                                                                       where
                                                                           reqs.TimeOpened > startDate &&
                                                                           reqs.TimeOpened <= endDate
                                                                       group reqs by new MonthYearPair(reqs.TimeOpened)
                                                                           into listByYear
                                                                           select listByYear).ToDictionary(r => r.Key,
                                                                                                           r =>
                                                                                                           r.ToList());
                    DataTable dt = new DataTable();
                    dt.Clear();

                    //Finds the string representation of stratifyBy option and create a column
                    DataColumn dataTypeColumn = new DataColumn("DataType", typeof(string));
                    dt.Columns.Add(dataTypeColumn);

                    //create column for each month
                    int totalNumOfMonths = endDate.Month +
                                           (endDate.Year - startDate.Year) * 12 -
                                           startDate.Month + 1;
                    MonthYearPair startMonthYearPair = new MonthYearPair(startDate.Month, startDate.Year);
                    for (int i = 0; i < totalNumOfMonths; i++) {
                        DataColumn monthColumn = new DataColumn(startMonthYearPair.ToString(), typeof(Int64)) {
                            DefaultValue = 0
                        };
                        dt.Columns.Add(monthColumn);
                        startMonthYearPair.addmonth(1);
                    }

                    foreach (Constants.DataType dataType in dataToDisplay) {
                        //adds a row for each dataType in the table
                        DataRow newRow = dt.NewRow();

                        //depending what data we need, the following enters the correct value for the the data cell.
                        switch (dataType) {
                            case Constants.DataType.AvgTimePerRequest:
                                newRow["DataType"] = "Avg Time Per Request";
                                foreach (KeyValuePair<MonthYearPair, List<Request>> keyValue in dictionaryByMonth) {
                                    newRow[keyValue.Key.ToString()] =
                                        averageTime(keyValue.Value);
                                }
                                break;
                            case Constants.DataType.AvgTimeFromStartToComplete:
                                newRow["DataType"] = "Avg Time From Start To Complete";
                                foreach (KeyValuePair<MonthYearPair, List<Request>> keyValue in dictionaryByMonth) {
                                    newRow[keyValue.Key.ToString()] =
                                        averageTime(keyValue.Value);
                                }
                                break;
                            case Constants.DataType.TotalNumOfRequests:
                                newRow["DataType"] = "Total Number";
                                foreach (KeyValuePair<MonthYearPair, List<Request>> keyValue in dictionaryByMonth) {
                                    newRow[keyValue.Key.ToString()] =
                                        averageTime(keyValue.Value);
                                }
                                break;
                            case Constants.DataType.TotalTimeSpent:
                                newRow["DataType"] = "Total Time Spent";
                                foreach (KeyValuePair<MonthYearPair, List<Request>> keyValue in dictionaryByMonth) {
                                    newRow[keyValue.Key.ToString()] =
                                        averageTime(keyValue.Value);
                                }
                                break;
                        }
                        dt.Rows.Add(newRow);
                    }
                    break;
            }

            return dataTablesForReport;
        }

        /// <summary>
        /// Returns the list of DataTables with proper data filled in, to be exported 
        /// </summary>
        /// <param name="startYear">start year selected by user</param>
        /// <param name="endYear">end year selected by user</param>
        /// <param name="dataToDisplay">date Types to represent, selected by the user</param>
        /// <param name="stratifyBy">stratify option, selected by the user</param>
        /// <returns></returns>
        public IEnumerable<DataTable> generateYearlyReport(int startYear, int endYear, IEnumerable<Constants.DataType> dataToDisplay,
                                         Constants.StratifyOption stratifyBy)
        {
            var dataTablesForReport = new List<DataTable>();

            var start = new DateTime(startYear, 4, 1, 0, 0, 0);
            var end = new DateTime(endYear, 3, DateTime.DaysInMonth(endYear, 3), 23, 59, 59);

            switch (stratifyBy) {
                case Constants.StratifyOption.Region:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the region
                    Dictionary<byte?, List<Request>> regionDictionary = (from reqs in _db.Requests
                                                                         where
                                                                             reqs.TimeOpened > start &&
                                                                             reqs.TimeOpened <= end
                                                                         group reqs by reqs.RegionID
                                                                             into regionGroups
                                                                             select regionGroups).ToDictionary(r => r.Key,
                                                                                                           r =>
                                                                                                           r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<byte?, Dictionary<FiscalYear, List<Request>>> regionAndYear =
                        regionDictionary.ToDictionary(keyValuePair => keyValuePair.Key,
                                                      keyValuePair =>
                                                      keyValuePair.Value.GroupBy(r => new FiscalYear(r.TimeOpened))
                                                                  .Select(grp => grp)
                                                                  .ToDictionary(grp => grp.Key, grp => grp.ToList()));

                    //creates dataTable for each data and adds it to the list of dataTables
                    dataTablesForReport.AddRange(
                        dataToDisplay.Select(
                            dataType => createDtForEachFiscalYear(startYear, endYear, stratifyBy, dataType, regionAndYear)));

                    break;
                case Constants.StratifyOption.CallerType:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the callerType
                    Dictionary<byte?, List<Request>> callerDictionary = (from reqs in _db.Requests
                                                                         where
                                                                             reqs.TimeOpened > start &&
                                                                             reqs.TimeOpened <= end
                                                                         group reqs by reqs.RequestorTypeID
                                                                             into callerGroups
                                                                             select callerGroups).ToDictionary(r => r.Key,
                                                                                                           r =>
                                                                                                           r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<byte?, Dictionary<FiscalYear, List<Request>>> callerAndYear =
                        callerDictionary.ToDictionary(keyValuePair => keyValuePair.Key,
                                                      keyValuePair =>
                                                      keyValuePair.Value.GroupBy(r => new FiscalYear(r.TimeOpened))
                                                                  .Select(grp => grp)
                                                                  .ToDictionary(grp => grp.Key, grp => grp.ToList()));

                    dataTablesForReport.AddRange(
                        dataToDisplay.Select(
                            dataType => createDtForEachFiscalYear(startYear, endYear, stratifyBy, dataType, callerAndYear)));
                    break;
                case Constants.StratifyOption.TumorGroup:
                    Dictionary<byte?, List<QandRwithTimestamp>> qrTumourGrpDic =
                        (from reqs in _db.Requests
                         where
                             reqs.TimeOpened > start &&
                             reqs.TimeOpened <= end
                         join qr in _db.QuestionResponses on reqs.RequestID
                             equals qr.RequestID
                         select
                             new QandRwithTimestamp(qr, reqs.TimeOpened,
                                                    reqs.TimeClosed)).GroupBy(
                                                        q => q.qr.TumourGroupID)
                                                                     .ToDictionary
                            (grp => grp.Key, grp => grp.ToList());

                    Dictionary<byte?, Dictionary<FiscalYear, List<QandRwithTimestamp>>> tgAndYear =
                        qrTumourGrpDic.ToDictionary(keyValuePair => keyValuePair.Key,
                                                      keyValuePair =>
                                                      keyValuePair.Value.GroupBy(r => new FiscalYear(r.timeOpened))
                                                                  .Select(grp => grp)
                                                                  .ToDictionary(grp => grp.Key, grp => grp.ToList()));

                    dataTablesForReport.AddRange(
                        dataToDisplay.Select(
                            dataType => createDtForEachFiscalYear(startYear, endYear, stratifyBy, dataType, tgAndYear)));

                    break;
                case Constants.StratifyOption.None:
                default:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the year
                    Dictionary<int, List<Request>> dictionaryByYear = (from reqs in _db.Requests
                                                                       where
                                                                           reqs.TimeOpened > start &&
                                                                           reqs.TimeOpened <= end
                                                                       group reqs by reqs.TimeOpened.Year
                                                                           into listByYear
                                                                           select listByYear).ToDictionary(r => r.Key,
                                                                                                           r =>
                                                                                                           r.ToList());
                    DataTable dt = new DataTable();
                    dt.Clear();

                    //Finds the string representation of stratifyBy option and create a column
                    DataColumn dataTypeColumn = new DataColumn("DataType", typeof(string));
                    dt.Columns.Add(dataTypeColumn);

                    //create column for each month
                    int totalNumOfMonths = endYear - startYear + 1;
                    FiscalYear startFiscalYear = new FiscalYear(startYear);
                    for (int i = 0; i < totalNumOfMonths; i++) {
                        DataColumn monthColumn = new DataColumn(startFiscalYear.ToString(), typeof(Int64)) {
                            DefaultValue = 0
                        };
                        dt.Columns.Add(monthColumn);
                        startFiscalYear.addYear(1);
                    }

                    foreach (Constants.DataType dataType in dataToDisplay) {
                        //adds a row for each dataType in the table
                        DataRow newRow = dt.NewRow();

                        switch (dataType) {
                            case Constants.DataType.AvgTimePerRequest:
                                newRow["DataType"] = "Avg Time Per Request";
                                foreach (KeyValuePair<int, List<Request>> keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key] =
                                        averageTime(keyValue.Value);
                                }
                                break;
                            case Constants.DataType.AvgTimeFromStartToComplete:
                                newRow["DataType"] = "Avg Time From Start To Complete";
                                foreach (KeyValuePair<int, List<Request>> keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key] =
                                        averageTime(keyValue.Value);
                                }
                                break;
                            case Constants.DataType.TotalNumOfRequests:
                                newRow["DataType"] = "Total Number";
                                foreach (KeyValuePair<int, List<Request>> keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key] =
                                        averageTime(keyValue.Value);
                                }
                                break;
                            case Constants.DataType.TotalTimeSpent:
                                newRow["DataType"] = "Total Time Spent";
                                foreach (KeyValuePair<int, List<Request>> keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key] =
                                        averageTime(keyValue.Value);
                                }
                                break;
                        }
                        dt.Rows.Add(newRow);
                    }
                    break;
            }

            return dataTablesForReport;
        }

        public IEnumerable<DataTable> generateMonthPerYearReport(int month, int startYear, int endYear,
                                                          IEnumerable<Constants.DataType> dataToDisplay,
                                                          Constants.StratifyOption stratifyBy)
        {
            var dataTablesForReport = new List<DataTable>();

            var start = new DateTime(startYear, month, 1, 0, 0, 0);
            var end = new DateTime(endYear, month, DateTime.DaysInMonth(endYear, month), 23, 59, 59);

            switch (stratifyBy)
            {
                case Constants.StratifyOption.Region:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the region
                    Dictionary<byte?, List<Request>> regionDictionary = (from reqs in _db.Requests
                                                                         where
                                                                             reqs.TimeOpened > start &&
                                                                             reqs.TimeOpened <= end &&
                                                                             reqs.TimeOpened.Month == month
                                                                         group reqs by reqs.RegionID
                                                                         into regionGroups
                                                                         select regionGroups).ToDictionary(r => r.Key,
                                                                                                           r =>
                                                                                                           r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<byte?, Dictionary<int, List<Request>>> regionAndYear =
                        regionDictionary.ToDictionary(keyValuePair => keyValuePair.Key,
                                                      keyValuePair =>
                                                      keyValuePair.Value.GroupBy(r => r.TimeOpened.Year)
                                                                  .Select(grp => grp)
                                                                  .ToDictionary(grp => grp.Key, grp => grp.ToList()));

                    //foreach (Constants.DataType dataType in dataToDisplay) {
                    //    DataTable dt = createDtForEachYear(startYear, endYear, stratifyBy, dataType, regionAndYear);
                    //    dataTablesForReport.Add(dt);
                    //}
                    dataTablesForReport.AddRange(
                        dataToDisplay.Select(
                            dataType => createDtForEachYear(startYear, endYear, stratifyBy, dataType, regionAndYear)));

                    break;
                case Constants.StratifyOption.CallerType:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the callerType
                    Dictionary<byte?, List<Request>> callerDictionary = (from reqs in _db.Requests
                                                                         where
                                                                             reqs.TimeOpened > start &&
                                                                             reqs.TimeOpened <= end &&
                                                                             reqs.TimeOpened.Month == month
                                                                         group reqs by reqs.RequestorTypeID
                                                                         into callerGroups
                                                                         select callerGroups).ToDictionary(r => r.Key,
                                                                                                           r =>
                                                                                                           r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<byte?, Dictionary<int, List<Request>>> callerAndYear =
                        callerDictionary.ToDictionary(keyValuePair => keyValuePair.Key,
                                                      keyValuePair =>
                                                      keyValuePair.Value.GroupBy(r => r.TimeOpened.Year)
                                                                  .Select(grp => grp)
                                                                  .ToDictionary(grp => grp.Key, grp => grp.ToList()));

                    dataTablesForReport.AddRange(
                        dataToDisplay.Select(
                            dataType => createDtForEachYear(startYear, endYear, stratifyBy, dataType, callerAndYear)));
                    break;
                case Constants.StratifyOption.TumorGroup:
                    Dictionary<byte?, List<QandRwithTimestamp>> qrTumourGrpDic =
                        (from reqs in _db.Requests
                         where
                             reqs.TimeOpened > start &&
                             reqs.TimeOpened <= end
                         join qr in _db.QuestionResponses on reqs.RequestID
                             equals qr.RequestID
                         select
                             new QandRwithTimestamp(qr, reqs.TimeOpened,
                                                    reqs.TimeClosed)).GroupBy(
                                                        q => q.qr.TumourGroupID)
                                                                     .ToDictionary
                            (grp => grp.Key, grp => grp.ToList());

                    Dictionary<byte?, Dictionary<int, List<QandRwithTimestamp>>> tgAndYear =
                        qrTumourGrpDic.ToDictionary(keyValuePair => keyValuePair.Key,
                                                      keyValuePair =>
                                                      keyValuePair.Value.GroupBy(r => r.timeOpened.Year)
                                                                  .Select(grp => grp)
                                                                  .ToDictionary(grp => grp.Key, grp => grp.ToList()));

                    dataTablesForReport.AddRange(
                        dataToDisplay.Select(
                            dataType => createDtForEachYear(startYear, endYear, stratifyBy, dataType, tgAndYear)));

                    break;
                case Constants.StratifyOption.None:
                //default:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the year
                    Dictionary<int, List<Request>> dictionaryByYear = (from reqs in _db.Requests
                                                                         where
                                                                             reqs.TimeOpened > start &&
                                                                             reqs.TimeOpened <= end &&
                                                                             reqs.TimeOpened.Month == month
                                                                         group reqs by reqs.TimeOpened.Year
                                                                             into listByYear
                                                                           select listByYear).ToDictionary(r => r.Key,
                                                                                                           r =>
                                                                                                           r.ToList());
                    DataTable dt = new DataTable();
                    dt.Clear();

                    //Finds the string representation of stratifyBy option and create a column
                    DataColumn dataTypeColumn = new DataColumn("DataType", typeof(string));
                    dt.Columns.Add(dataTypeColumn);

                    //create column for each year
                    for (int i = startYear; i <= endYear; i++) {
                        DataColumn yearColumn = new DataColumn(i.ToString(), typeof(Int64));
                        yearColumn.DefaultValue = 0;
                        dt.Columns.Add(yearColumn);
                    }

                    foreach (Constants.DataType dataType in dataToDisplay) {
                        //adds a row for each dataType in the table
                        DataRow newRow = dt.NewRow();

                        switch (dataType) {
                                case Constants.DataType.AvgTimePerRequest:
                                    newRow["DataType"] = "Avg Time Per Request";
                                    foreach ( KeyValuePair<int, List<Request>> keyValue in dictionaryByYear) {
                                        newRow[keyValue.Key] =
                                            averageTime(keyValue.Value);
                                    }
                                break;
                                case Constants.DataType.AvgTimeFromStartToComplete:
                                    newRow["DataType"] = "Avg Time From Start To Complete";
                                    foreach (KeyValuePair<int, List<Request>> keyValue in dictionaryByYear) {
                                        newRow[keyValue.Key] =
                                            averageTime(keyValue.Value);
                                    }
                                break;
                                case Constants.DataType.TotalNumOfRequests:
                                    newRow["DataType"] = "Total Number";
                                    foreach (KeyValuePair<int, List<Request>> keyValue in dictionaryByYear) {
                                        newRow[keyValue.Key] =
                                            averageTime(keyValue.Value);
                                    }
                                break;
                                case Constants.DataType.TotalTimeSpent:
                                    newRow["DataType"] = "Total Time Spent";
                                    foreach (KeyValuePair<int, List<Request>> keyValue in dictionaryByYear) {
                                        newRow[keyValue.Key] =
                                            averageTime(keyValue.Value);
                                    }
                                break;
                        }
                        dt.Rows.Add(newRow);
                    }
                    break;
            }

            return dataTablesForReport;
        }

        /// <summary>
        /// Creates a dataTable using given parameters for generateMonthPerYearReport method
        /// </summary>
        /// <param name="startYear">start year for the first column</param>
        /// <param name="endYear">end year for the last column</param>
        /// <param name="stratifyBy">strafityBy option is passed on to get the stratifyBy Name</param>
        /// <param name="dataType">dataType option passed on to get the cell daa</param>
        /// <param name="dictionary">dictionary grouped by stratify groups for rows and 
        ///                     each stratify group sub-grouped into their years for columns</param>
        /// <returns>DataTable with proper data filled in.</returns>
        private DataTable createDtForEachYear(int startYear, int endYear,
                                                 Constants.StratifyOption
                                                     stratifyBy,
                                                 Constants.DataType dataType,
                                                 Dictionary
                                                     <byte?,
                                                     Dictionary<int, List<Request>>>
                                                     dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups =
                Enum.GetName(typeof (Constants.StratifyOption), stratifyBy);
            DataColumn stratifyGrpColum = new DataColumn(stratifyGroups,
                                                         typeof (string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each year
            for (int i = startYear; i <= endYear; i++) {
                DataColumn yearColumn = new DataColumn(i.ToString(),
                                                       typeof (Int64)) {
                                                           DefaultValue = 0
                                                       };
                dt.Columns.Add(yearColumn);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<byte, string> idToName = getTypeNames(stratifyBy);


            foreach (var keyValuePair in dictionary) {
                //adds a row for each stratify groups in the table
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key.HasValue) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key.Value];
                } else {
                    newRow[stratifyGroups] = "No " + stratifyGroups;
                }

                //now visit each year which the requests are sub-grouped by year
                //and adds the proper value for the cell in the dataTable
                foreach (var valuePair in keyValuePair.Value) {
                    switch (dataType) {
                        case Constants.DataType.AvgTimePerRequest:
                            newRow[valuePair.Key.ToString()] =
                                averageTime(valuePair.Value);
                            break;
                        case Constants.DataType.AvgTimeFromStartToComplete:
                            newRow[valuePair.Key.ToString()] =
                                avgTimeFromStartToComplete(valuePair.Value);
                            break;
                        case Constants.DataType.TotalNumOfRequests:
                            newRow[valuePair.Key.ToString()] =
                                valuePair.Value.Count;
                            break;
                        case Constants.DataType.TotalTimeSpent:
                            newRow[valuePair.Key.ToString()] =
                                totalTimeSpent(valuePair.Value);
                            break;
                    }
                }
                dt.Rows.Add(newRow);
            }

            return dt;
        }

        /// <summary>
        /// Creates a dataTable using given parameters for generateMonthPerYearReport method
        /// </summary>
        /// <param name="startYear">start year for the first column</param>
        /// <param name="endYear">end year for the last column</param>
        /// <param name="stratifyBy">strafityBy option is passed on to get the stratifyBy Name</param>
        /// <param name="dataType">dataType option passed on to get the cell daa</param>
        /// <param name="dictionary">dictionary grouped by stratify groups for rows and 
        ///                     each stratify group sub-grouped into their years for columns</param>
        /// <returns>DataTable with proper data filled in.</returns>
        private DataTable createDtForEachYear(int startYear, int endYear,
                                                 Constants.StratifyOption
                                                     stratifyBy,
                                                 Constants.DataType dataType,
                                                 Dictionary
                                                     <byte?,
                                                     Dictionary<int, List<QandRwithTimestamp>>>
                                                     dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups =
                Enum.GetName(typeof(Constants.StratifyOption), stratifyBy);
            DataColumn stratifyGrpColum = new DataColumn(stratifyGroups,
                                                         typeof(string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each year
            for (int i = startYear; i <= endYear; i++) {
                DataColumn yearColumn = new DataColumn(i.ToString(),
                                                       typeof(Int64)) {
                                                           DefaultValue = 0
                                                       };
                dt.Columns.Add(yearColumn);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<byte, string> idToName = getTypeNames(stratifyBy);


            foreach (var keyValuePair in dictionary) {
                //adds a row for each stratify groups in the table
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key.HasValue) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key.Value];
                } else {
                    newRow[stratifyGroups] = "No " + stratifyGroups;
                }

                //now visit each year which the requests are sub-grouped by year
                //and adds the proper value for the cell in the dataTable
                foreach (var valuePair in keyValuePair.Value) {
                    switch (dataType) {
                        case Constants.DataType.AvgTimePerRequest:
                            newRow[valuePair.Key.ToString()] =
                                averageTime(valuePair.Value);
                            break;
                        case Constants.DataType.AvgTimeFromStartToComplete:
                            newRow[valuePair.Key.ToString()] =
                                avgTimeFromStartToComplete(valuePair.Value);
                            break;
                        case Constants.DataType.TotalNumOfRequests:
                            newRow[valuePair.Key.ToString()] =
                                valuePair.Value.Count;
                            break;
                        case Constants.DataType.TotalTimeSpent:
                            newRow[valuePair.Key.ToString()] =
                                totalTimeSpent(valuePair.Value);
                            break;
                    }
                }
                dt.Rows.Add(newRow);
            }

            return dt;
        }

        /// <summary>
        /// creates a dataTable fore each fiscal year for given stratify grouping
        /// </summary>
        /// <param name="startYear">report start year</param>
        /// <param name="endYear">report end year</param>
        /// <param name="stratifyBy">stratifyOption to retireve group names</param>
        /// <param name="dataType">data to be displayed</param>
        /// <param name="dictionary">requests grouped into stratify group then in to their fiscal year</param>
        /// <returns>dataTable fore each fiscal year for given stratify grouping</returns>
        private DataTable createDtForEachFiscalYear(int startYear, int endYear, Constants.StratifyOption stratifyBy,
                                              Constants.DataType dataType,
                                              Dictionary<byte?, Dictionary<FiscalYear, List<Request>>> dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups = Enum.GetName(typeof(Constants.StratifyOption), stratifyBy);
            DataColumn stratifyGrpColum = new DataColumn(stratifyGroups, typeof(string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each month
            int totalNumOfMonths = endYear - startYear + 1;
            FiscalYear startFiscalYear = new FiscalYear(startYear);
            for (int i = 0; i < totalNumOfMonths; i++) {
                DataColumn monthColumn = new DataColumn(startFiscalYear.ToString(), typeof(Int64)) {
                    DefaultValue = 0
                };
                dt.Columns.Add(monthColumn);
                startFiscalYear.addYear(1);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<byte, string> idToName = getTypeNames(stratifyBy);

            //adds a row for each startify groups with perper data filled in
            foreach (var keyValuePair in dictionary) {
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key.HasValue) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key.Value];
                } else {
                    newRow[stratifyGroups] = "No " + stratifyGroups;
                }

                //now visit each year which the requests are sub-grouped by year
                //and adds the proper value for the cell in the dataTable
                foreach (var valuePair in keyValuePair.Value) {
                    switch (dataType) {
                        case Constants.DataType.AvgTimePerRequest:
                            newRow[valuePair.Key.ToString()] = averageTime(valuePair.Value);
                            break;
                        case Constants.DataType.AvgTimeFromStartToComplete:
                            newRow[valuePair.Key.ToString()] = avgTimeFromStartToComplete(valuePair.Value);
                            break;
                        case Constants.DataType.TotalNumOfRequests:
                            newRow[valuePair.Key.ToString()] = valuePair.Value.Count;
                            break;
                        case Constants.DataType.TotalTimeSpent:
                            newRow[valuePair.Key.ToString()] = totalTimeSpent(valuePair.Value);
                            break;
                    }
                }
                dt.Rows.Add(newRow);
            }

            return dt;
        }

        /// <summary>
        /// creates a dataTable fore each fiscal year for given stratify grouping
        /// </summary>
        /// <param name="startYear">report start year</param>
        /// <param name="endYear">report end year</param>
        /// <param name="stratifyBy">stratifyOption to retireve group names</param>
        /// <param name="dataType">data to be displayed</param>
        /// <param name="dictionary">QandRwithTimestamp grouped into stratify group then in to their fiscal year</param>
        /// <returns>dataTable fore each fiscal year for given stratify grouping</returns>
        private DataTable createDtForEachFiscalYear(int startYear, int endYear, Constants.StratifyOption stratifyBy,
                                              Constants.DataType dataType,
                                              Dictionary<byte?, Dictionary<FiscalYear, List<QandRwithTimestamp>>> dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups = Enum.GetName(typeof(Constants.StratifyOption), stratifyBy);
            DataColumn stratifyGrpColum = new DataColumn(stratifyGroups, typeof(string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each month
            int totalNumOfMonths = endYear - startYear + 1;
            FiscalYear startFiscalYear = new FiscalYear(startYear);
            for (int i = 0; i < totalNumOfMonths; i++) {
                DataColumn monthColumn = new DataColumn(startFiscalYear.ToString(), typeof(Int64)) {
                    DefaultValue = 0
                };
                dt.Columns.Add(monthColumn);
                startFiscalYear.addYear(1);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<byte, string> idToName = getTypeNames(stratifyBy);

            //adds a row for each startify groups with perper data filled in
            foreach (var keyValuePair in dictionary) {
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key.HasValue) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key.Value];
                } else {
                    newRow[stratifyGroups] = "No " + stratifyGroups;
                }

                //now visit each year which the requests are sub-grouped by year
                //and adds the proper value for the cell in the dataTable
                foreach (var valuePair in keyValuePair.Value) {
                    switch (dataType) {
                        case Constants.DataType.AvgTimePerRequest:
                            newRow[valuePair.Key.ToString()] = averageTime(valuePair.Value);
                            break;
                        case Constants.DataType.AvgTimeFromStartToComplete:
                            newRow[valuePair.Key.ToString()] = avgTimeFromStartToComplete(valuePair.Value);
                            break;
                        case Constants.DataType.TotalNumOfRequests:
                            newRow[valuePair.Key.ToString()] = valuePair.Value.Count;
                            break;
                        case Constants.DataType.TotalTimeSpent:
                            newRow[valuePair.Key.ToString()] = totalTimeSpent(valuePair.Value);
                            break;
                    }
                }
                dt.Rows.Add(newRow);
            }

            return dt;
        }

        private DataTable createDtForEachMonth(DateTime startTime, DateTime endTime, Constants.StratifyOption stratifyBy,
                                              Constants.DataType dataType,
                                              Dictionary<byte?, Dictionary<MonthYearPair, List<Request>>> dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups = Enum.GetName(typeof(Constants.StratifyOption), stratifyBy);
            DataColumn stratifyGrpColum = new DataColumn(stratifyGroups, typeof(string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each month
            int totalNumOfMonths = endTime.Month +
                                   (endTime.Year - startTime.Year)*12 -
                                   startTime.Month + 1;
            MonthYearPair startMonthYearPair = new MonthYearPair(startTime.Month, startTime.Year);
            for (int i = 0; i < totalNumOfMonths; i++) {
                DataColumn monthColumn = new DataColumn(startMonthYearPair.ToString(), typeof(Int64)) {
                    DefaultValue = 0
                };
                dt.Columns.Add(monthColumn);
                startMonthYearPair.addmonth(1);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<byte, string> idToName = getTypeNames(stratifyBy);


            foreach (var keyValuePair in dictionary) {
                //adds a row for each stratify groups in the table
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key.HasValue) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key.Value];
                } else {
                    newRow[stratifyGroups] = "No " + stratifyGroups;
                }

                //now visit each year which the requests are sub-grouped by year
                //and adds the proper value for the cell in the dataTable
                foreach (var valuePair in keyValuePair.Value) {
                    switch (dataType) {
                        case Constants.DataType.AvgTimePerRequest:
                            newRow[valuePair.Key.ToString()] = averageTime(valuePair.Value);
                            break;
                        case Constants.DataType.AvgTimeFromStartToComplete:
                            newRow[valuePair.Key.ToString()] = avgTimeFromStartToComplete(valuePair.Value);
                            break;
                        case Constants.DataType.TotalNumOfRequests:
                            newRow[valuePair.Key.ToString()] = valuePair.Value.Count;
                            break;
                        case Constants.DataType.TotalTimeSpent:
                            newRow[valuePair.Key.ToString()] = totalTimeSpent(valuePair.Value);
                            break;
                    }
                }
                dt.Rows.Add(newRow);
            }

            return dt;
        }

        private DataTable createDtForEachMonth(DateTime startTime, DateTime endTime, Constants.StratifyOption stratifyBy,
                                              Constants.DataType dataType,
                                              Dictionary<byte?, Dictionary<MonthYearPair, List<QandRwithTimestamp>>> dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups = Enum.GetName(typeof(Constants.StratifyOption), stratifyBy);
            DataColumn stratifyGrpColum = new DataColumn(stratifyGroups, typeof(string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each month
            int totalNumOfMonths = endTime.Month +
                                   (endTime.Year - startTime.Year) * 12 -
                                   startTime.Month + 1;
            MonthYearPair startMonthYearPair = new MonthYearPair(startTime.Month, startTime.Year);
            for (int i = 0; i < totalNumOfMonths; i++) {
                DataColumn monthColumn = new DataColumn(startMonthYearPair.ToString(), typeof(Int64)) {
                    DefaultValue = 0
                };
                dt.Columns.Add(monthColumn);
                startMonthYearPair.addmonth(1);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<byte, string> idToName = getTypeNames(stratifyBy);


            foreach (var keyValuePair in dictionary) {
                //adds a row for each stratify groups in the table
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key.HasValue) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key.Value];
                } else {
                    newRow[stratifyGroups] = "No " + stratifyGroups;
                }

                //now visit each year which the requests are sub-grouped by year
                //and adds the proper value for the cell in the dataTable
                foreach (var valuePair in keyValuePair.Value) {
                    switch (dataType) {
                        case Constants.DataType.AvgTimePerRequest:
                            newRow[valuePair.Key.ToString()] = averageTime(valuePair.Value);
                            break;
                        case Constants.DataType.AvgTimeFromStartToComplete:
                            newRow[valuePair.Key.ToString()] = avgTimeFromStartToComplete(valuePair.Value);
                            break;
                        case Constants.DataType.TotalNumOfRequests:
                            newRow[valuePair.Key.ToString()] = valuePair.Value.Count;
                            break;
                        case Constants.DataType.TotalTimeSpent:
                            newRow[valuePair.Key.ToString()] = totalTimeSpent(valuePair.Value);
                            break;
                    }
                }
                dt.Rows.Add(newRow);
            }

            return dt;
        }

        // below are for calculating each cell values for each data type given the list of requests to be used 
        private Int32 averageTime(List<Request> requestsList) {
            Int32 total = totalTimeSpent(requestsList);
            return total / requestsList.Count;
        }

        // below are for calculating each cell values for each data type given the list of requests to be used 
        private Int32 averageTime(List<QandRwithTimestamp> qrList) {
            Int32 total = totalTimeSpent(qrList);
            return total / qrList.Count;
        }

        //Given the list of requests, calculates the average time from open to close
        private Int32 avgTimeFromStartToComplete(List<Request> reqList) {
            var totalTimeSpan = new TimeSpan(0);
            totalTimeSpan = reqList.Aggregate(totalTimeSpan,
                                              (current, curRequest) =>
                                              curRequest.TimeClosed.HasValue
                                                  ? current.Add(
                                                      curRequest.TimeClosed
                                                                .Value -
                                                      curRequest.TimeOpened)
                                                  : current.Add(DateTime.Now -
                                                                curRequest
                                                                    .TimeOpened));
            return totalTimeSpan.Minutes/reqList.Count;
        }

        //Given the list of requests, calculates the average time from open to close
        private Int32 avgTimeFromStartToComplete(List<QandRwithTimestamp> qrList) {
            var totalTimeSpan = new TimeSpan(0);

            totalTimeSpan = qrList.Aggregate(totalTimeSpan,
                                             (current, curQr) =>
                                             curQr.timeClosed.HasValue
                                                 ? current.Add(
                                                     curQr.timeClosed.Value -
                                                     curQr.timeOpened)
                                                 : current.Add(DateTime.Now -
                                                               curQr.timeOpened));

            return totalTimeSpan.Minutes/qrList.Count;
        }

        //calculates the total time spent for all Q&R pair in each requestin the given list
        private Int32 totalTimeSpent(IEnumerable<Request> reqList) {
           
            return
                reqList.Select(curRequest => (from qrs in _db.QuestionResponses
                                              where
                                                  qrs.RequestID ==
                                                  curRequest.RequestID &&
                                                  qrs.TimeSpent.HasValue
                                              orderby qrs.QuestionResponseID
                                              select qrs.TimeSpent.Value).ToList
                                                 ())
                       .Select(
                           qrTimeSpentResults =>
                           qrTimeSpentResults.Sum(ts => ts))
                       .Sum();
        }

        //calculates the total time spent for all the Q&A pair in the given list
        private Int32 totalTimeSpent(IEnumerable<QandRwithTimestamp> qrList) {
            return qrList.Where(qr => qr.qr.TimeSpent.HasValue)
                         .Aggregate(0,
                                    (current, qr) =>
                                    qr.qr.TimeSpent != null
                                        ? current + qr.qr.TimeSpent.Value
                                        : 0);
        }

        //creates dictionary for the names of stratify groups to be used in the dataTable 
        private Dictionary<byte, string> getTypeNames(Constants.StratifyOption stratifyOption)
        {
            Dictionary<byte, string> codes = null;
            switch (stratifyOption)
            {
                case Constants.StratifyOption.Region:
                    codes = (from region in _db.Regions
                             orderby region.Value
                             select region).ToDictionary(region => region.RegionID, r => r.Code);
                    break;
                case Constants.StratifyOption.CallerType:
                    codes = (from callerType in _db.RequestorTypes
                             orderby callerType.Value
                             select callerType).ToDictionary(ct => ct.RequestorTypeID, ct => ct.Code);
                    break;
                case Constants.StratifyOption.TumorGroup:
                    codes = (from tumorGroup in _db.TumourGroups
                             orderby tumorGroup.Value
                             select tumorGroup).ToDictionary(tg => tg.TumourGroupID, tg => tg.Code);
                    break;
                default:
            
                    //silent fail
                    break;
            }
            return codes;
        }

    }

    public struct QandRwithTimestamp {
        private QuestionResponse _qr;
        private DateTime _timeOpened;
        private DateTime? _timeClosed;

        public QandRwithTimestamp(QuestionResponse qandR, DateTime openTime, DateTime? closeTime) {
            _qr = qandR;
            _timeOpened = openTime;
            _timeClosed = closeTime;
        }

        public QuestionResponse qr {
            get {
                return _qr;
            }
        }

        public DateTime timeOpened {
            get {
                return _timeOpened;
            }
        }

        public DateTime? timeClosed {
            get {
                return _timeClosed;
            }
        }
    }

    public struct FiscalYear: IEquatable<FiscalYear> {
        private int _fiscalYeaNum;

        public int fiscalYeaNum {
            get {
                return _fiscalYeaNum;
            }
        }

        public FiscalYear(DateTime date) {
            int[] previousFiscalYear = new[]{1, 2, 3};
            if (previousFiscalYear.Contains(date.Month)) {
                _fiscalYeaNum = date.Year - 1;
            } else {
                _fiscalYeaNum = date.Year;
            }
        }

        public FiscalYear(int year) {
            _fiscalYeaNum = year;
        }

        public void addYear(int i) {
            _fiscalYeaNum = +i;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 31 + _fiscalYeaNum;
            return hash;
        }

        public override string ToString() {
            return _fiscalYeaNum.ToString() + " - " + (_fiscalYeaNum+1).ToString();
        }

        public override bool Equals(object obj) {
            return obj is FiscalYear && Equals((FiscalYear) obj);
        }

        public bool Equals(FiscalYear other) {
            return _fiscalYeaNum == other._fiscalYeaNum;
        }
    }

    public struct MonthYearPair : IEquatable<MonthYearPair> {
        private int _year;
        private int _month;

        private int year {
            get {
                return _year;
            }
        }

        private int month {
            get {
                return _month;
            }
        }

        public MonthYearPair(int month, int year) {
            _month = month;
            _year = year;
        }

        public MonthYearPair(DateTime date) {
            _month = date.Month;
            _year = date.Year;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 31 + _month;
            hash = hash*7 + _year;
            return hash;
        }

        public void addmonth(int monthToAdd) {
            _month = (_month + monthToAdd)%12;
            if (_month == 0) {
                _month = 12;
            }
            _year = _year + (_month + monthToAdd)/12;
        }

        public override string ToString() {
            return Enum.GetName(typeof(Constants.Month), _month) + "/" + _year;
        }

        public override bool Equals(object obj) {
            return obj is MonthYearPair && Equals((MonthYearPair) obj);
        }

        public bool Equals(MonthYearPair other) {
            return _month == other.month && _year == other.year;
        }
        
    }
}