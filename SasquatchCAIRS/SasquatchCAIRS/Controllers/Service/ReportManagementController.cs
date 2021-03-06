﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Models.Common;
using SasquatchCAIRS.Models.Service;

namespace SasquatchCAIRS.Controllers.Service {
    /// <summary>
    ///     Controls the Reports in the system.
    /// </summary>
    public class ReportManagementController : Controller {
        //private CAIRSDataContext _db = new CAIRSDataContext();
        /// <summary>Data Context for this Controller.</summary>
        private readonly IDataContext _db;

        /// <summary>
        ///     Create a new RMC with a custom Data Context
        /// </summary>
        /// <param name="context">A custom IDataContext</param>
        public ReportManagementController(IDataContext context) {
            _db = context;
        }

        /// <summary>
        ///     Create a new RMC with its own CAIRSDataContext
        /// </summary>
        public ReportManagementController() {
            _db = new CAIRSDataContext();
        }

        /// <summary>
        ///     Checks if any data exists for the given period
        /// </summary>
        /// <param name="start">Start date stamp</param>
        /// <param name="end">End date stamp</param>
        /// <returns>Returns true if no data is found</returns>
        public bool checkForDataForMonth(DateTime start, DateTime end) {
            List<Request> requests = (from reqs in _db.Repository<Request>()
                                      where
                                          reqs.TimeOpened > start &&
                                          reqs.TimeOpened <= end
                                      select reqs).ToList();
            return !requests.Any();
        }

        /// <summary>
        ///     Checks if any data exists for the given period
        /// </summary>
        /// <param name="month">Month user selected</param>
        /// <param name="startYear">Start year selected by user</param>
        /// <param name="endYear">End year selected by user</param>
        /// <returns>Returns true if no data is found</returns>
        public bool checkForDataForMpy(int month, int startYear, int endYear) {
            List<Request> requests = (from reqs in _db.Repository<Request>()
                                      where
                                          reqs.TimeOpened.Year >= startYear &&
                                          reqs.TimeOpened.Year <= endYear &&
                                          reqs.TimeOpened.Month == month
                                      select reqs).ToList();
            return !requests.Any();
        }

        /// <summary>
        ///     Checks if any data exists for the given period
        /// </summary>
        /// <param name="startYear">Start year selected by user</param>
        /// <param name="endYear">End year selected by user</param>
        /// <returns>Returns true if no data is found</returns>
        public bool checkForDataForFy(int startYear, int endYear) {
            List<Request> requests = (from reqs in _db.Repository<Request>()
                                      where
                                          reqs.TimeOpened.Year >= startYear &&
                                          reqs.TimeOpened.Year <= endYear
                                      select reqs).ToList();
            return !requests.Any();
        }

        /// <summary>
        ///     Creates list of dataTables for monthly report, to be exported based on the month and criteria specified
        /// </summary>
        /// <param name="startDate">Start date, selected by the user</param>
        /// <param name="endDate">End date, selected by the user</param>
        /// <param name="dataToDisplay">Date Types to represent, selected by the user</param>
        /// <param name="stratifyBy">Stratify option, selected by the user</param>
        /// <returns>The list of data tables, one table for each data type chosen</returns>
        public Dictionary<string, DataTable> generateMonthlyReport(
            DateTime startDate, DateTime endDate,
            IEnumerable<Constants.DataType> dataToDisplay,
            Constants.StratifyOption stratifyBy) {
            var dataTablesForReport = new Dictionary<string, DataTable>();

            //executes different methods depending on the stratify options selected
            switch (stratifyBy) {
                case Constants.StratifyOption.Region:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the region
                    Dictionary<int, List<Request>> regionDictionary = (from reqs
                                                                           in
                                                                           _db
                                                                           .Repository
                                                                           <
                                                                           Request
                                                                           >()
                                                                       where
                                                                           reqs
                                                                               .TimeOpened >
                                                                           startDate &&
                                                                           reqs
                                                                               .TimeOpened <=
                                                                           endDate
                                                                       group
                                                                           reqs
                                                                           by
                                                                           reqs
                                                                           .RegionID
                                                                       into
                                                                           regionGroups
                                                                       select
                                                                           regionGroups)
                        .ToDictionary(r => nullableToInt(r.Key),
                                      r =>
                                      r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<int, Dictionary<MonthYearPair, List<Request>>>
                        regionAndYear =
                            regionDictionary.ToDictionary(
                                keyValuePair => keyValuePair.Key,
                                keyValuePair =>
                                keyValuePair.Value.GroupBy(
                                    r => new MonthYearPair(r.TimeOpened))
                                            .Select(grp => grp)
                                            .ToDictionary(grp => grp.Key,
                                                          grp => grp.ToList()));

                    //creates dataTable for each data and adds it to the dictionary of dataTables
                    foreach (Constants.DataType dataType in dataToDisplay) {
                        int titleIndex = ((int) stratifyBy - 1)*4 +
                                         (int) dataType;
                        dataTablesForReport.Add(
                            Constants.DATATABLE_TITLES[titleIndex],
                            createDtForEachMonth(startDate, endDate, stratifyBy,
                                                 dataType, regionAndYear));
                    }

                    break;
                case Constants.StratifyOption.RequestorType:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the callerType
                    Dictionary<int, List<Request>> callerDictionary = (from reqs
                                                                           in
                                                                           _db
                                                                           .Repository
                                                                           <
                                                                           Request
                                                                           >()
                                                                       where
                                                                           reqs
                                                                               .TimeOpened >
                                                                           startDate &&
                                                                           reqs
                                                                               .TimeOpened <=
                                                                           endDate
                                                                       group
                                                                           reqs
                                                                           by
                                                                           reqs
                                                                           .RequestorTypeID
                                                                       into
                                                                           callerGroups
                                                                       select
                                                                           callerGroups)
                        .ToDictionary(r => nullableToInt(r.Key),
                                      r =>
                                      r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<int, Dictionary<MonthYearPair, List<Request>>>
                        callerAndYear =
                            callerDictionary.ToDictionary(
                                keyValuePair => keyValuePair.Key,
                                keyValuePair =>
                                keyValuePair.Value.GroupBy(
                                    r => new MonthYearPair(r.TimeOpened))
                                            .Select(grp => grp)
                                            .ToDictionary(grp => grp.Key,
                                                          grp => grp.ToList()));

                    //creates dataTable for each data and adds it to the dictionary of dataTables
                    foreach (Constants.DataType dataType in dataToDisplay) {
                        int titleIndex = ((int) stratifyBy - 1)*4 +
                                         (int) dataType;
                        dataTablesForReport.Add(
                            Constants.DATATABLE_TITLES[titleIndex],
                            createDtForEachMonth(startDate, endDate, stratifyBy,
                                                 dataType, callerAndYear));
                    }
                    break;
                case Constants.StratifyOption.TumorGroup:
                    //Retrieves the QuestionResponse from the database which opened within the given timeFrame,
                    //adds the open, close timestamps, then group them by the tumourGroup
                    Dictionary<int, List<QandRwithTimestamp>> qrTumourGrpDic =
                        (from reqs in _db.Repository<Request>()
                         where
                             reqs.TimeOpened > startDate &&
                             reqs.TimeOpened <= endDate
                         join qr in _db.Repository<QuestionResponse>() on
                             reqs.RequestID
                             equals qr.RequestID
                         select
                             new QandRwithTimestamp(qr, reqs.TimeOpened,
                                                    reqs.TimeClosed)).ToList()
                                                                     .GroupBy(
                                                                         q =>
                                                                         q.qr
                                                                          .TumourGroupID)
                                                                     .Select(
                                                                         grp =>
                                                                         grp)
                                                                     .ToDictionary
                            (grp => nullableToInt(grp.Key), grp => grp.ToList());

                    //Sub-groups the regionGroups by the year the question(request) is opened.
                    Dictionary
                        <int,
                            Dictionary<MonthYearPair, List<QandRwithTimestamp>>>
                        tgAndYear =
                            qrTumourGrpDic.ToDictionary(
                                keyValuePair => keyValuePair.Key,
                                keyValuePair =>
                                keyValuePair.Value.GroupBy(
                                    r => new MonthYearPair(r.timeOpened))
                                            .Select(grp => grp)
                                            .ToDictionary(grp => grp.Key,
                                                          grp => grp.ToList()));

                    //creates dataTable for each data and adds it to the dictionary of dataTables
                    foreach (Constants.DataType dataType in dataToDisplay) {
                        int titleIndex = ((int) stratifyBy - 1)*4 +
                                         (int) dataType;
                        dataTablesForReport.Add(
                            Constants.DATATABLE_TITLES[titleIndex],
                            createDtForEachMonth(startDate, endDate, stratifyBy,
                                                 dataType, tgAndYear));
                    }
                    break;
                default:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the year
                    Dictionary<MonthYearPair, List<Request>> dictionaryByMonth = (from
                                                                                      reqs
                                                                                      in
                                                                                      _db
                                                                                      .Repository
                                                                                      <
                                                                                      Request
                                                                                      >
                                                                                      ()
                                                                                  where
                                                                                      reqs
                                                                                          .TimeOpened >
                                                                                      startDate &&
                                                                                      reqs
                                                                                          .TimeOpened <=
                                                                                      endDate
                                                                                  group
                                                                                      reqs
                                                                                      by
                                                                                      new {
                                                                                          reqs
                                                                                      .TimeOpened
                                                                                      .Month,
                                                                                          reqs
                                                                                      .TimeOpened
                                                                                      .Year
                                                                                      }
                                                                                  into
                                                                                      listByYear
                                                                                  select
                                                                                      listByYear)
                        .ToDictionary(
                            r => new MonthYearPair(r.Key.Month, r.Key.Year),
                            r =>
                            r.ToList());
                    var dt = new DataTable();
                    dt.Clear();

                    //Finds the string representation of stratifyBy option and create a column
                    var dataTypeColumn =
                        new DataColumn(Constants.DataTypeStrings.DATA_TYPE,
                                       typeof (string));
                    dt.Columns.Add(dataTypeColumn);

                    //create column for each month
                    int totalNumOfMonths = endDate.Month +
                                           (endDate.Year - startDate.Year)*12 -
                                           startDate.Month;
                    var startMonthYearPair = new MonthYearPair(startDate.Month,
                                                               startDate.Year);
                    for (int i = 0; i < totalNumOfMonths; i++) {
                        var monthColumn =
                            new DataColumn(startMonthYearPair.ToString(),
                                           typeof (Int64)) {
                                               DefaultValue = 0
                                           };
                        dt.Columns.Add(monthColumn);
                        startMonthYearPair.addmonth(1);
                    }
                    foreach (
                        Constants.DataType dataType in
                            dataToDisplay.OrderByDescending(x => x).Reverse()) {
                        //adds a row for each dataType in the table
                        DataRow newRow = dt.NewRow();

                        //depending what data we need, the following enters the correct value for the the data cell.
                        switch (dataType) {
                            case Constants.DataType.AvgTimePerRequest:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings.AVG_TIME;
                                foreach (var keyValue in dictionaryByMonth) {
                                    newRow[keyValue.Key.ToString()] =
                                        averageTime(keyValue.Value);
                                }
                                break;
                            case Constants.DataType.AvgTimeToComplete:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings
                                             .AVG_TIME_TO_COMPLETE;
                                foreach (var keyValue in dictionaryByMonth) {
                                    newRow[keyValue.Key.ToString()] =
                                        avgTimeFromStartToComplete(
                                            keyValue.Value);
                                }
                                break;
                            case Constants.DataType.TotalNumOfRequests:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings.TOTAL_NUM;
                                foreach (var keyValue in dictionaryByMonth) {
                                    newRow[keyValue.Key.ToString()] =
                                        keyValue.Value.Count;
                                }
                                break;
                            case Constants.DataType.TotalTimeSpent:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings.TOTAL_TIME_SPENT;
                                foreach (var keyValue in dictionaryByMonth) {
                                    newRow[keyValue.Key.ToString()] =
                                        totalTimeSpent(keyValue.Value);
                                }
                                break;
                        }
                        dt.Rows.Add(newRow);
                    }
                    dataTablesForReport.Add(Constants.DATATABLE_TITLES[0], dt);
                    break;
            }

            return dataTablesForReport;
        }

        /// <summary>
        ///     Creates list of dataTables for monthly report, to be exported based on the years and criteria specified
        /// </summary>
        /// <param name="startYear">Start year selected by user</param>
        /// <param name="endYear">End year selected by user</param>
        /// <param name="dataToDisplay">Date Types to represent, selected by the user</param>
        /// <param name="stratifyBy">Stratify option, selected by the user</param>
        /// <returns>List of DataTables for each data type chosen</returns>
        public Dictionary<string, DataTable> generateYearlyReport(int startYear,
                                                                  int endYear,
                                                                  IEnumerable
                                                                      <
                                                                      Constants.
                                                                      DataType>
                                                                      dataToDisplay,
                                                                  Constants.
                                                                      StratifyOption
                                                                      stratifyBy) {
            var dataTablesForReport = new Dictionary<string, DataTable>();

            var startDate = new DateTime(startYear, 4, 1, 0, 0, 0);
            var enDated = new DateTime(endYear + 1, 3,
                                       DateTime.DaysInMonth(endYear, 3), 23, 59,
                                       59);

            switch (stratifyBy) {
                case Constants.StratifyOption.Region:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the region
                    Dictionary<int, List<Request>> regionDictionary = (from reqs
                                                                           in
                                                                           _db
                                                                           .Repository
                                                                           <
                                                                           Request
                                                                           >()
                                                                       where
                                                                           reqs
                                                                               .TimeOpened >
                                                                           startDate &&
                                                                           reqs
                                                                               .TimeOpened <=
                                                                           enDated
                                                                       group
                                                                           reqs
                                                                           by
                                                                           reqs
                                                                           .RegionID
                                                                       into
                                                                           regionGroups
                                                                       select
                                                                           regionGroups)
                        .ToDictionary(r => nullableToInt(r.Key),
                                      r =>
                                      r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<int, Dictionary<FiscalYear, List<Request>>>
                        regionAndYear =
                            regionDictionary.ToDictionary(
                                keyValuePair => keyValuePair.Key,
                                keyValuePair =>
                                keyValuePair.Value.GroupBy(
                                    r => new FiscalYear(r.TimeOpened))
                                            .Select(grp => grp)
                                            .ToDictionary(grp => grp.Key,
                                                          grp => grp.ToList()));

                    //creates dataTable for each data and adds it to the list of dataTables
                    foreach (Constants.DataType dataType in dataToDisplay) {
                        int titleIndex = ((int) stratifyBy - 1)*4 +
                                         (int) dataType;
                        dataTablesForReport.Add(
                            Constants.DATATABLE_TITLES[titleIndex],
                            createDtForEachFiscalYear(startYear, endYear,
                                                      stratifyBy, dataType,
                                                      regionAndYear));
                    }
                    break;
                case Constants.StratifyOption.RequestorType:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the callerType
                    Dictionary<int, List<Request>> callerDictionary = (from reqs
                                                                           in
                                                                           _db
                                                                           .Repository
                                                                           <
                                                                           Request
                                                                           >()
                                                                       where
                                                                           reqs
                                                                               .TimeOpened >
                                                                           startDate &&
                                                                           reqs
                                                                               .TimeOpened <=
                                                                           enDated
                                                                       group
                                                                           reqs
                                                                           by
                                                                           reqs
                                                                           .RequestorTypeID
                                                                       into
                                                                           callerGroups
                                                                       select
                                                                           callerGroups)
                        .ToDictionary(r => nullableToInt(r.Key),
                                      r =>
                                      r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<int, Dictionary<FiscalYear, List<Request>>>
                        callerAndYear =
                            callerDictionary.ToDictionary(
                                keyValuePair => keyValuePair.Key,
                                keyValuePair =>
                                keyValuePair.Value.GroupBy(
                                    r => new FiscalYear(r.TimeOpened))
                                            .Select(grp => grp)
                                            .ToDictionary(grp => grp.Key,
                                                          grp => grp.ToList()));

                    //creates dataTable for each data and adds it to the list of dataTables
                    foreach (Constants.DataType dataType in dataToDisplay) {
                        int titleIndex = ((int) stratifyBy - 1)*4 +
                                         (int) dataType;
                        dataTablesForReport.Add(
                            Constants.DATATABLE_TITLES[titleIndex],
                            createDtForEachFiscalYear(startYear, endYear,
                                                      stratifyBy, dataType,
                                                      callerAndYear));
                    }
                    break;
                case Constants.StratifyOption.TumorGroup:
                    Dictionary<int, List<QandRwithTimestamp>> qrTumourGrpDic =
                        (from reqs in _db.Repository<Request>()
                         where
                             reqs.TimeOpened > startDate &&
                             reqs.TimeOpened <= enDated
                         join qr in _db.Repository<QuestionResponse>() on
                             reqs.RequestID
                             equals qr.RequestID
                         select
                             new QandRwithTimestamp(qr, reqs.TimeOpened,
                                                    reqs.TimeClosed)).ToList()
                                                                     .GroupBy(
                                                                         q =>
                                                                         q.qr
                                                                          .TumourGroupID)
                                                                     .Select(
                                                                         grp =>
                                                                         grp)
                                                                     .ToDictionary
                            (grp => nullableToInt(grp.Key), grp => grp.ToList());

                    Dictionary
                        <int, Dictionary<FiscalYear, List<QandRwithTimestamp>>>
                        tgAndYear =
                            qrTumourGrpDic.ToDictionary(
                                keyValuePair => keyValuePair.Key,
                                keyValuePair =>
                                keyValuePair.Value.GroupBy(
                                    r => new FiscalYear(r.timeOpened))
                                            .Select(grp => grp)
                                            .ToDictionary(grp => grp.Key,
                                                          grp => grp.ToList()));

                    //creates dataTable for each data and adds it to the dictionary of dataTables
                    foreach (Constants.DataType dataType in dataToDisplay) {
                        int titleIndex = ((int) stratifyBy - 1)*4 +
                                         (int) dataType;
                        dataTablesForReport.Add(
                            Constants.DATATABLE_TITLES[titleIndex],
                            createDtForEachFiscalYear(startYear, endYear,
                                                      stratifyBy, dataType,
                                                      tgAndYear));
                    }

                    break;
                default:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the year
                    Dictionary<FiscalYear, List<Request>> dictionaryByYear = (from
                                                                                  reqs
                                                                                  in
                                                                                  _db
                                                                                  .Repository
                                                                                  <
                                                                                  Request
                                                                                  >
                                                                                  ()
                                                                              where
                                                                                  reqs
                                                                                      .TimeOpened >
                                                                                  startDate &&
                                                                                  reqs
                                                                                      .TimeOpened <=
                                                                                  enDated
                                                                              select
                                                                                  reqs)
                        .ToList().GroupBy(r => new FiscalYear(r.TimeOpened))
                        .Select(grp => grp)
                        .ToDictionary(grp => grp.Key, grp => grp.ToList());

                    //Dictionary<FiscalYear, List<Request>> dictionaryByYear = (from reqs in _db.Requests
                    //                                                   where
                    //                                                       reqs.TimeOpened > start &&
                    //                                                       reqs.TimeOpened <= end
                    //                                                   group reqs by new FiscalYear(reqs.TimeOpened)
                    //                                                       into listByYear
                    //                                                              select listByYear).ToDictionary(r => r.Key,
                    //                                                                                       r =>
                    //                                                                                       r.ToList());
                    var dt = new DataTable();
                    dt.Clear();

                    //Finds the string representation of stratifyBy option and create a column
                    var dataTypeColumn =
                        new DataColumn(Constants.DataTypeStrings.DATA_TYPE,
                                       typeof (string));
                    dt.Columns.Add(dataTypeColumn);

                    //create column for each month
                    int totalNumOfYears = endYear - startYear + 1;
                    var startFiscalYear = new FiscalYear(startYear);
                    for (int i = 0; i < totalNumOfYears; i++) {
                        var monthColumn =
                            new DataColumn(startFiscalYear.ToString(),
                                           typeof (Int64)) {
                                               DefaultValue = 0
                                           };
                        dt.Columns.Add(monthColumn);
                        startFiscalYear.addYear(1);
                    }

                    foreach (
                        Constants.DataType dataType in
                            dataToDisplay.OrderByDescending(x => x).Reverse()) {
                        //adds a row for each dataType in the table
                        DataRow newRow = dt.NewRow();
                        switch (dataType) {
                            case Constants.DataType.AvgTimePerRequest:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings.AVG_TIME;
                                foreach (var keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key.ToString()] =
                                        averageTime(keyValue.Value);
                                }
                                break;
                            case Constants.DataType.AvgTimeToComplete:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings
                                             .AVG_TIME_TO_COMPLETE;
                                foreach (var keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key.ToString()] =
                                        avgTimeFromStartToComplete(
                                            keyValue.Value);
                                }
                                break;
                            case Constants.DataType.TotalNumOfRequests:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings.TOTAL_NUM;
                                foreach (var keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key.ToString()] =
                                        keyValue.Value.Count;
                                }
                                break;
                            case Constants.DataType.TotalTimeSpent:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings.TOTAL_TIME_SPENT;
                                foreach (var keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key.ToString()] =
                                        totalTimeSpent(keyValue.Value);
                                }
                                break;
                        }
                        dt.Rows.Add(newRow);
                    }
                    dataTablesForReport.Add(Constants.DATATABLE_TITLES[0], dt);
                    break;
            }

            return dataTablesForReport;
        }

        /// <summary>
        ///     Creates list of dataTables for monthly report, to be exported based on the month,the year and criteria specified
        /// </summary>
        /// <param name="month">Month of the interest</param>
        /// <param name="startYear">Start year of the report</param>
        /// <param name="endYear">End year of the report</param>
        /// <param name="dataToDisplay">List of dataTypes selected by the user</param>
        /// <param name="stratifyBy">Stratify option selected by the user</param>
        /// <returns>A list of DataTables of month per year for the data provided</returns>
        public Dictionary<string, DataTable> generateMonthPerYearReport(
            int month, int startYear, int endYear,
            IEnumerable<Constants.DataType> dataToDisplay,
            Constants.StratifyOption stratifyBy) {
            var dataTablesForReport = new Dictionary<string, DataTable>();

            var start = new DateTime(startYear, month, 1, 0, 0, 0);
            var end = new DateTime(endYear, month,
                                   DateTime.DaysInMonth(endYear, month), 23, 59,
                                   59);

            switch (stratifyBy) {
                case Constants.StratifyOption.Region:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the region
                    Dictionary<int, List<Request>> regionDictionary = (from reqs
                                                                           in
                                                                           _db
                                                                           .Repository
                                                                           <
                                                                           Request
                                                                           >()
                                                                       where
                                                                           reqs
                                                                               .TimeOpened >
                                                                           start &&
                                                                           reqs
                                                                               .TimeOpened <=
                                                                           end &&
                                                                           reqs
                                                                               .TimeOpened
                                                                               .Month ==
                                                                           month
                                                                       group
                                                                           reqs
                                                                           by
                                                                           reqs
                                                                           .RegionID
                                                                       into
                                                                           regionGroups
                                                                       select
                                                                           regionGroups)
                        .ToDictionary(r => nullableToInt(r.Key),
                                      r =>
                                      r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<int, Dictionary<int, List<Request>>>
                        regionAndYear =
                            regionDictionary.ToDictionary(
                                keyValuePair => keyValuePair.Key,
                                keyValuePair =>
                                keyValuePair.Value.GroupBy(
                                    r => r.TimeOpened.Year)
                                            .Select(grp => grp)
                                            .ToDictionary(grp => grp.Key,
                                                          grp => grp.ToList()));

                    //creates dataTable for each data and adds it to the dictionary of dataTables
                    foreach (Constants.DataType dataType in dataToDisplay) {
                        int titleIndex = ((int) stratifyBy - 1)*4 +
                                         (int) dataType;
                        dataTablesForReport.Add(
                            Constants.DATATABLE_TITLES[titleIndex],
                            createDtForEachYear(startYear, endYear, stratifyBy,
                                                dataType, regionAndYear));
                    }

                    break;
                case Constants.StratifyOption.RequestorType:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the callerType
                    Dictionary<int, List<Request>> callerDictionary = (from reqs
                                                                           in
                                                                           _db
                                                                           .Repository
                                                                           <
                                                                           Request
                                                                           >()
                                                                       where
                                                                           reqs
                                                                               .TimeOpened >
                                                                           start &&
                                                                           reqs
                                                                               .TimeOpened <=
                                                                           end &&
                                                                           reqs
                                                                               .TimeOpened
                                                                               .Month ==
                                                                           month
                                                                       group
                                                                           reqs
                                                                           by
                                                                           reqs
                                                                           .RequestorTypeID
                                                                       into
                                                                           callerGroups
                                                                       select
                                                                           callerGroups)
                        .ToDictionary(r => nullableToInt(r.Key),
                                      r =>
                                      r.ToList());

                    //Sub-groups the regionGroups by the year the request is opened.
                    Dictionary<int, Dictionary<int, List<Request>>>
                        callerAndYear =
                            callerDictionary.ToDictionary(
                                keyValuePair => keyValuePair.Key,
                                keyValuePair =>
                                keyValuePair.Value.GroupBy(
                                    r => r.TimeOpened.Year)
                                            .Select(grp => grp)
                                            .ToDictionary(grp => grp.Key,
                                                          grp => grp.ToList()));

                    //creates dataTable for each data and adds it to the dictionary of dataTables
                    foreach (Constants.DataType dataType in dataToDisplay) {
                        int titleIndex = ((int) stratifyBy - 1)*4 +
                                         (int) dataType;
                        dataTablesForReport.Add(
                            Constants.DATATABLE_TITLES[titleIndex],
                            createDtForEachYear(startYear, endYear, stratifyBy,
                                                dataType, callerAndYear));
                    }
                    break;
                case Constants.StratifyOption.TumorGroup:
                    Dictionary<int, List<QandRwithTimestamp>> qrTumourGrpDic =
                        (from reqs in _db.Repository<Request>()
                         where
                             reqs.TimeOpened > start &&
                             reqs.TimeOpened <= end
                         join qr in _db.Repository<QuestionResponse>() on
                             reqs.RequestID
                             equals qr.RequestID
                         select
                             new QandRwithTimestamp(qr, reqs.TimeOpened,
                                                    reqs.TimeClosed)).ToList()
                                                                     .GroupBy(
                                                                         q =>
                                                                         q.qr
                                                                          .TumourGroupID)
                                                                     .Select(
                                                                         grp =>
                                                                         grp)
                                                                     .ToDictionary
                            (grp => nullableToInt(grp.Key), grp => grp.ToList());

                    Dictionary<int, Dictionary<int, List<QandRwithTimestamp>>>
                        tgAndYear =
                            qrTumourGrpDic.ToDictionary(
                                keyValuePair => keyValuePair.Key,
                                keyValuePair =>
                                keyValuePair.Value.GroupBy(
                                    r => r.timeOpened.Year)
                                            .Select(grp => grp)
                                            .ToDictionary(grp => grp.Key,
                                                          grp => grp.ToList()));

                    //creates dataTable for each data and adds it to the dictionary of dataTables
                    foreach (Constants.DataType dataType in dataToDisplay) {
                        int titleIndex = ((int) stratifyBy - 1)*4 +
                                         (int) dataType;
                        dataTablesForReport.Add(
                            Constants.DATATABLE_TITLES[titleIndex],
                            createDtForEachYear(startYear, endYear, stratifyBy,
                                                dataType, tgAndYear));
                    }

                    break;
                default:
                    //Retrieves the requests from the database which opened within the given timeFrame
                    //then group them by the year
                    Dictionary<int, List<Request>> dictionaryByYear = (from reqs
                                                                           in
                                                                           _db
                                                                           .Repository
                                                                           <
                                                                           Request
                                                                           >()
                                                                       where
                                                                           reqs
                                                                               .TimeOpened >
                                                                           start &&
                                                                           reqs
                                                                               .TimeOpened <=
                                                                           end &&
                                                                           reqs
                                                                               .TimeOpened
                                                                               .Month ==
                                                                           month
                                                                       group
                                                                           reqs
                                                                           by
                                                                           reqs
                                                                           .TimeOpened
                                                                           .Year
                                                                       into
                                                                           listByYear
                                                                       select
                                                                           listByYear)
                        .ToDictionary(r => r.Key,
                                      r =>
                                      r.ToList());
                    var dt = new DataTable();
                    dt.Clear();

                    //Finds the string representation of stratifyBy option and create a column
                    var dataTypeColumn =
                        new DataColumn(Constants.DataTypeStrings.DATA_TYPE,
                                       typeof (string));
                    dt.Columns.Add(dataTypeColumn);

                    //create column for each year
                    for (int i = startYear; i <= endYear; i++) {
                        var yearColumn = new DataColumn(i.ToString(),
                                                        typeof (Int32)) {
                                                            DefaultValue = 0
                                                        };
                        dt.Columns.Add(yearColumn);
                    }

                    foreach (
                        Constants.DataType dataType in
                            dataToDisplay.OrderByDescending(x => x).Reverse()) {
                        //adds a row for each dataType in the table
                        DataRow newRow = dt.NewRow();

                        switch (dataType) {
                            case Constants.DataType.AvgTimePerRequest:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings.AVG_TIME;
                                foreach (var keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key.ToString()] =
                                        averageTime(keyValue.Value);
                                }
                                break;
                            case Constants.DataType.AvgTimeToComplete:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings
                                             .AVG_TIME_TO_COMPLETE;
                                foreach (var keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key.ToString()] =
                                        avgTimeFromStartToComplete(
                                            keyValue.Value);
                                }
                                break;
                            case Constants.DataType.TotalNumOfRequests:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings.TOTAL_NUM;
                                foreach (var keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key.ToString()] =
                                        keyValue.Value.Count;
                                }
                                break;
                            case Constants.DataType.TotalTimeSpent:
                                newRow[Constants.DataTypeStrings.DATA_TYPE] =
                                    Constants.DataTypeStrings.TOTAL_TIME_SPENT;
                                foreach (var keyValue in dictionaryByYear) {
                                    newRow[keyValue.Key.ToString()] =
                                        totalTimeSpent(keyValue.Value);
                                }
                                break;
                        }
                        dt.Rows.Add(newRow);
                    }

                    dataTablesForReport.Add(Constants.DATATABLE_TITLES[0], dt);
                    break;
            }

            return dataTablesForReport;
        }

        /// <summary>
        ///     Creates a dataTable using given parameters for generateMonthPerYearReport method
        /// </summary>
        /// <param name="startYear">Start year for the first column</param>
        /// <param name="endYear">End year for the last column</param>
        /// <param name="stratifyBy">StrafityBy option is passed on to get the stratifyBy Name</param>
        /// <param name="dataType">DataType option passed on to get the cell daa</param>
        /// <param name="dictionary">
        ///     Dictionary grouped by stratify groups for rows and
        ///     each stratify group sub-grouped into their years for columns
        /// </param>
        /// <returns>DataTable with proper data filled in.</returns>
        private DataTable createDtForEachYear(int startYear, int endYear,
                                              Constants.StratifyOption
                                                  stratifyBy,
                                              Constants.DataType dataType,
                                              Dictionary
                                                  <int,
                                                  Dictionary<int, List<Request>>
                                                  >
                                                  dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups =
                Enum.GetName(typeof (Constants.StratifyOption), stratifyBy);
            var stratifyGrpColum = new DataColumn(stratifyGroups,
                                                  typeof (string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each year
            for (int i = startYear; i <= endYear; i++) {
                var yearColumn = new DataColumn(i.ToString(),
                                                typeof (Int64)) {
                                                    DefaultValue = 0
                                                };
                dt.Columns.Add(yearColumn);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<int, string> idToName = getTypeNames(stratifyBy);

            //Even if the dictionary does not contain the no group data, table should still show them.
            if (!dictionary.ContainsKey(-1)) {
                DataRow noGroupRow = dt.NewRow();
                noGroupRow[stratifyGroups] = "No " + stratifyGroups;
                dt.Rows.Add(noGroupRow);
            }

            foreach (
                var keyValuePair in
                    dictionary.OrderByDescending(x => x.Key).Reverse()) {
                //adds a row for each stratify groups in the table
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key != -1) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key];
                    idToName.Remove(keyValuePair.Key);
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
                        case Constants.DataType.AvgTimeToComplete:
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

            DataRow groupRow;
            foreach (var group in idToName) {
                groupRow = dt.NewRow();
                groupRow[stratifyGroups] = group.Value;
                dt.Rows.Add(groupRow);
            }

            return dt;
        }

        /// <summary>
        ///     Creates a dataTable using given parameters for generateMonthPerYearReport method
        /// </summary>
        /// <param name="startYear">Start year for the first column</param>
        /// <param name="endYear">End year for the last column</param>
        /// <param name="stratifyBy">StrafityBy option is passed on to get the stratifyBy Name</param>
        /// <param name="dataType">DataType option passed on to get the cell daa</param>
        /// <param name="dictionary">
        ///     Dictionary grouped by stratify groups for rows and
        ///     each stratify group sub-grouped into their years for columns
        /// </param>
        /// <returns>DataTable with proper data filled in.</returns>
        private DataTable createDtForEachYear(int startYear, int endYear,
                                              Constants.StratifyOption
                                                  stratifyBy,
                                              Constants.DataType dataType,
                                              Dictionary
                                                  <int,
                                                  Dictionary
                                                  <int, List<QandRwithTimestamp>
                                                  >>
                                                  dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups =
                Enum.GetName(typeof (Constants.StratifyOption), stratifyBy);
            var stratifyGrpColum = new DataColumn(stratifyGroups,
                                                  typeof (string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each year
            for (int i = startYear; i <= endYear; i++) {
                var yearColumn = new DataColumn(i.ToString(),
                                                typeof (Int64)) {
                                                    DefaultValue = 0
                                                };
                dt.Columns.Add(yearColumn);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<int, string> idToName = getTypeNames(stratifyBy);

            //Even if the dictionary does not contain the no group data, table should still show them.
            if (!dictionary.ContainsKey(-1)) {
                DataRow noGroupRow = dt.NewRow();
                noGroupRow[stratifyGroups] = "No " + stratifyGroups;
                dt.Rows.Add(noGroupRow);
            }

            foreach (
                var keyValuePair in
                    dictionary.OrderByDescending(x => x.Key).Reverse()) {
                //adds a row for each stratify groups in the table
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key != -1) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key];
                    idToName.Remove(keyValuePair.Key);
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
                        case Constants.DataType.AvgTimeToComplete:
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

            DataRow groupRow;
            foreach (var group in idToName) {
                groupRow = dt.NewRow();
                groupRow[stratifyGroups] = group.Value;
                dt.Rows.Add(groupRow);
            }

            return dt;
        }

        /// <summary>
        ///     Creates a dataTable fore each fiscal year for given stratify grouping
        /// </summary>
        /// <param name="startYear">Report start year</param>
        /// <param name="endYear">Report end year</param>
        /// <param name="stratifyBy">StratifyOption to retireve group names</param>
        /// <param name="dataType">Data to be displayed</param>
        /// <param name="dictionary">Requests grouped into stratify group then in to their fiscal year</param>
        /// <returns>DataTable fore each fiscal year for given stratify grouping</returns>
        private DataTable createDtForEachFiscalYear(int startYear, int endYear,
                                                    Constants.StratifyOption
                                                        stratifyBy,
                                                    Constants.DataType dataType,
                                                    Dictionary
                                                        <int,
                                                        Dictionary
                                                        <FiscalYear,
                                                        List<Request>>>
                                                        dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups =
                Enum.GetName(typeof (Constants.StratifyOption), stratifyBy);
            var stratifyGrpColum = new DataColumn(stratifyGroups,
                                                  typeof (string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each month
            int totalNumOfMonths = endYear - startYear + 1;
            var startFiscalYear = new FiscalYear(startYear);
            for (int i = 0; i < totalNumOfMonths; i++) {
                var monthColumn = new DataColumn(startFiscalYear.ToString(),
                                                 typeof (Int64)) {
                                                     DefaultValue = 0
                                                 };
                dt.Columns.Add(monthColumn);
                startFiscalYear.addYear(1);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<int, string> idToName = getTypeNames(stratifyBy);

            //Even if the dictionary does not contain the no group data, table should still show them.
            if (!dictionary.ContainsKey(-1)) {
                DataRow noGroupRow = dt.NewRow();
                noGroupRow[stratifyGroups] = "No " + stratifyGroups;
                dt.Rows.Add(noGroupRow);
            }

            //adds a row for each startify groups with proper data filled in
            foreach (
                var keyValuePair in
                    dictionary.OrderByDescending(x => x.Key).Reverse()) {
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key != -1) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key];
                    idToName.Remove(keyValuePair.Key);
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
                        case Constants.DataType.AvgTimeToComplete:
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

            DataRow groupRow;
            foreach (var group in idToName) {
                groupRow = dt.NewRow();
                groupRow[stratifyGroups] = group.Value;
                dt.Rows.Add(groupRow);
            }

            return dt;
        }

        /// <summary>
        ///     Creates a dataTable fore each fiscal year for given stratify grouping
        /// </summary>
        /// <param name="startYear">Report start year</param>
        /// <param name="endYear">Report end year</param>
        /// <param name="stratifyBy">StratifyOption to retireve group names</param>
        /// <param name="dataType">Data to be displayed</param>
        /// <param name="dictionary">QandRwithTimestamp grouped into stratify group then in to their fiscal year</param>
        /// <returns>DataTable fore each fiscal year for given stratify grouping</returns>
        private DataTable createDtForEachFiscalYear(int startYear, int endYear,
                                                    Constants.StratifyOption
                                                        stratifyBy,
                                                    Constants.DataType dataType,
                                                    Dictionary
                                                        <int,
                                                        Dictionary
                                                        <FiscalYear,
                                                        List<QandRwithTimestamp>
                                                        >> dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups =
                Enum.GetName(typeof (Constants.StratifyOption), stratifyBy);
            var stratifyGrpColum = new DataColumn(stratifyGroups,
                                                  typeof (string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each month
            int totalNumOfMonths = endYear - startYear + 1;
            var startFiscalYear = new FiscalYear(startYear);
            for (int i = 0; i < totalNumOfMonths; i++) {
                var monthColumn = new DataColumn(startFiscalYear.ToString(),
                                                 typeof (Int64)) {
                                                     DefaultValue = 0
                                                 };
                dt.Columns.Add(monthColumn);
                startFiscalYear.addYear(1);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<int, string> idToName = getTypeNames(stratifyBy);

            //Even if the dictionary does not contain the no group data, table should still show them.
            if (!dictionary.ContainsKey(-1)) {
                DataRow noGroupRow = dt.NewRow();
                noGroupRow[stratifyGroups] = "No " + stratifyGroups;
                dt.Rows.Add(noGroupRow);
            }

            //adds a row for each startify groups with perper data filled in
            foreach (
                var keyValuePair in
                    dictionary.OrderByDescending(x => x.Key).Reverse()) {
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key != -1) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key];
                    idToName.Remove(keyValuePair.Key);
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
                        case Constants.DataType.AvgTimeToComplete:
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

            DataRow groupRow;
            foreach (var group in idToName) {
                groupRow = dt.NewRow();
                groupRow[stratifyGroups] = group.Value;
                dt.Rows.Add(groupRow);
            }

            return dt;
        }

        /// <summary>
        ///     Creates a dataTable fore each fiscal year for given stratify grouping
        /// </summary>
        /// <param name="startTime">Report start date</param>
        /// <param name="endTime">Report end date</param>
        /// <param name="stratifyBy">StratifyOption to retireve group names</param>
        /// <param name="dataType">Data to be displayed</param>
        /// <param name="dictionary">Requests grouped into stratify group then in to their month/year pair</param>
        /// <returns>DataTable fore each  month for given stratify grouping</returns>
        private DataTable createDtForEachMonth(DateTime startTime,
                                               DateTime endTime,
                                               Constants.StratifyOption
                                                   stratifyBy,
                                               Constants.DataType dataType,
                                               Dictionary
                                                   <int,
                                                   Dictionary
                                                   <MonthYearPair, List<Request>
                                                   >> dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups =
                Enum.GetName(typeof (Constants.StratifyOption), stratifyBy);
            var stratifyGrpColum = new DataColumn(stratifyGroups,
                                                  typeof (string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each month
            int totalNumOfMonths = endTime.Month +
                                   (endTime.Year - startTime.Year)*12 -
                                   startTime.Month;
            var startMonthYearPair = new MonthYearPair(startTime.Month,
                                                       startTime.Year);
            for (int i = 0; i < totalNumOfMonths; i++) {
                var monthColumn = new DataColumn(startMonthYearPair.ToString(),
                                                 typeof (Int64)) {
                                                     DefaultValue = 0
                                                 };
                dt.Columns.Add(monthColumn);
                startMonthYearPair.addmonth(1);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<int, string> idToName = getTypeNames(stratifyBy);

            //Even if the dictionary does not contain the no group data, table should still show them.
            if (!dictionary.ContainsKey(-1)) {
                DataRow noGroupRow = dt.NewRow();
                noGroupRow[stratifyGroups] = "No " + stratifyGroups;
                dt.Rows.Add(noGroupRow);
            }

            foreach (
                var keyValuePair in
                    dictionary.OrderByDescending(x => x.Key).Reverse()) {
                //adds a row for each stratify groups in the table
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key != -1) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key];
                    idToName.Remove(keyValuePair.Key);
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
                        case Constants.DataType.AvgTimeToComplete:
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

            DataRow groupRow;
            foreach (var group in idToName) {
                groupRow = dt.NewRow();
                groupRow[stratifyGroups] = group.Value;
                dt.Rows.Add(groupRow);
            }

            return dt;
        }

        /// <summary>
        ///     Creates a DataTable fore each fiscal year for given stratify grouping
        /// </summary>
        /// <param name="startTime">Report start date</param>
        /// <param name="endTime">Report end date</param>
        /// <param name="stratifyBy">StratifyOption to retireve group names</param>
        /// <param name="dataType">Data to be displayed</param>
        /// <param name="dictionary">QandRwithTimestamp grouped into stratify group then in to their month</param>
        /// <returns>DataTable for each month for given stratify grouping</returns>
        private DataTable createDtForEachMonth(DateTime startTime,
                                               DateTime endTime,
                                               Constants.StratifyOption
                                                   stratifyBy,
                                               Constants.DataType dataType,
                                               Dictionary
                                                   <int,
                                                   Dictionary
                                                   <MonthYearPair,
                                                   List<QandRwithTimestamp>>>
                                                   dictionary) {
            var dt = new DataTable();
            dt.Clear();

            //Finds the string representation of stratifyBy option and create a column
            string stratifyGroups =
                Enum.GetName(typeof (Constants.StratifyOption), stratifyBy);
            var stratifyGrpColum = new DataColumn(stratifyGroups,
                                                  typeof (string));
            dt.Columns.Add(stratifyGrpColum);

            //create column for each month
            int totalNumOfMonths = endTime.Month +
                                   (endTime.Year - startTime.Year)*12 -
                                   startTime.Month;
            var startMonthYearPair = new MonthYearPair(startTime.Month,
                                                       startTime.Year);
            for (int i = 0; i < totalNumOfMonths; i++) {
                var monthColumn = new DataColumn(startMonthYearPair.ToString(),
                                                 typeof (Int64)) {
                                                     DefaultValue = 0
                                                 };
                dt.Columns.Add(monthColumn);
                startMonthYearPair.addmonth(1);
            }

            //gets the names of the stratify groups. ie, callerType,region or tumourGroup Codes
            Dictionary<int, string> idToName = getTypeNames(stratifyBy);

            //Even if the dictionary does not contain the no group data, table should still show them.
            if (!dictionary.ContainsKey(-1)) {
                DataRow noGroupRow = dt.NewRow();
                noGroupRow[stratifyGroups] = "No " + stratifyGroups;
                dt.Rows.Add(noGroupRow);
            }

            foreach (
                var keyValuePair in
                    dictionary.OrderByDescending(x => x.Key).Reverse()) {
                //adds a row for each stratify groups in the table
                DataRow newRow = dt.NewRow();

                //if the key is null then it should create a row for 'No group assigned' requests
                if (keyValuePair.Key != -1) {
                    newRow[stratifyGroups] = idToName[keyValuePair.Key];
                    idToName.Remove(keyValuePair.Key);
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
                        case Constants.DataType.AvgTimeToComplete:
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

            DataRow groupRow;
            foreach (var group in idToName) {
                groupRow = dt.NewRow();
                groupRow[stratifyGroups] = group.Value;
                dt.Rows.Add(groupRow);
            }

            return dt;
        }

        /// <summary>
        ///     Calculates each cell values for each data type given the list of requests to be used
        /// </summary>
        /// <param name="requestsList">List of requests to find the average time</param>
        /// <returns>Average time spent for the given list of requests</returns>
        private Int32 averageTime(List<Request> requestsList) {
            Int32 total = totalTimeSpent(requestsList);
            return total/requestsList.Count;
        }

        /// <summary>
        ///     Calculates each cell values for each data type given the list of QnR to be used
        /// </summary>
        /// <param name="qrList">List of QandRs to find the average time</param>
        /// <returns>Average time spent for the given list of requests</returns>
        private Int32 averageTime(List<QandRwithTimestamp> qrList) {
            Int32 total = totalTimeSpent(qrList);
            return total/qrList.Count;
        }

        /// <summary>
        ///     Given the list of requests, calculates the average time from open to close
        /// </summary>
        /// <param name="reqList">List of requests</param>
        /// <returns>Average time from start to complete for given list of requests</returns>
        private Int32 avgTimeFromStartToComplete(List<Request> reqList) {
            var totalTimeSpan = new TimeSpan(0);
            foreach (Request current in reqList) {
                if (current.TimeClosed.HasValue) {
                    totalTimeSpan += (current.TimeClosed.Value -
                                      current.TimeOpened);
                } else {
                    totalTimeSpan += (DateTime.Now - current.TimeOpened);
                }
            }

            return (int) (totalTimeSpan.TotalMinutes/reqList.Count);
        }

        /// <summary>
        ///     Given the list of requests, calculates the average time from open to close
        /// </summary>
        /// <param name="qrList">List of QandRs with time stamps</param>
        /// <returns>Average time from start to complete for given list of questions</returns>
        private Int32 avgTimeFromStartToComplete(List<QandRwithTimestamp> qrList) {
            var totalTimeSpan = new TimeSpan(0);

            foreach (QandRwithTimestamp current in qrList) {
                if (current.timeClosed.HasValue) {
                    totalTimeSpan += (current.timeClosed.Value -
                                      current.timeOpened);
                } else {
                    totalTimeSpan += (DateTime.Now - current.timeOpened);
                }
            }

            return (int) (totalTimeSpan.TotalMinutes/qrList.Count);
        }

        /// <summary>
        ///     Calculates the total time spent for all Q&R pair in each requestin the given list
        /// </summary>
        /// <param name="reqList">List of requests</param>
        /// <returns>Total time spent on the list given</returns>
        private Int32 totalTimeSpent(IEnumerable<Request> reqList) {
            return
                reqList.Select(
                    curRequest =>
                    (from qrs in _db.Repository<QuestionResponse>()
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

        /// <summary>
        ///     Calculates the total time spent for all the Q&A pair in the given list
        /// </summary>
        /// <param name="qrList">List of questions</param>
        /// <returns>Total time spent on the given list of questions</returns>
        private Int32 totalTimeSpent(IEnumerable<QandRwithTimestamp> qrList) {
            return qrList.Where(qr => qr.qr.TimeSpent.HasValue)
                         .Aggregate(0,
                                    (current, qr) =>
                                    qr.qr.TimeSpent != null
                                        ? current + qr.qr.TimeSpent.Value
                                        : 0);
        }

        /// <summary>
        ///     Creates dictionary for the names of stratify groups to be used in the dataTable
        /// </summary>
        /// <param name="stratifyOption">Stratify option selected</param>
        /// <returns>Returns the dictionary of subgroup codes for the stratify option selected</returns>
        private Dictionary<int, string> getTypeNames(
            Constants.StratifyOption stratifyOption) {
            Dictionary<int, string> codes = null;
            switch (stratifyOption) {
                case Constants.StratifyOption.Region:
                    codes = (from region in _db.Repository<Region>()
                             orderby region.Value
                             select region).ToDictionary(
                                 region => region.RegionID, r => r.Code);
                    break;
                case Constants.StratifyOption.RequestorType:
                    codes = (from callerType in _db.Repository<RequestorType>()
                             orderby callerType.Value
                             select callerType).ToDictionary(
                                 ct => ct.RequestorTypeID, ct => ct.Code);
                    break;
                case Constants.StratifyOption.TumorGroup:
                    codes = (from tumorGroup in _db.Repository<TumourGroup>()
                             orderby tumorGroup.Value
                             select tumorGroup).ToDictionary(
                                 tg => tg.TumourGroupID, tg => tg.Code);
                    break;
            }
            return codes;
        }

        /// <summary>
        ///     Currently if the key is null (ie. if the request/question does not have assigned stratify group,
        ///     the key gets set to the highest value
        /// </summary>
        /// <param name="key">StratifyGroupID</param>
        /// <returns>Returns the value of the key, or -1 if the key is null</returns>
        private int nullableToInt(int? key) {
            if (key.HasValue) {
                return key.Value;
            } else {
                return -1;
            }
        }
    }
}