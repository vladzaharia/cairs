using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers
{
    public class ReportController : Controller
    {
        private readonly CAIRSDataContext _db = new CAIRSDataContext();

        //Generates the monthly report for given period
        //Parameteres: startDate = 0:00:00 of the date selected, endDate = 23:59:59 of selected date
        //  dataToDisplay: list of data that needs to be displayed, stratifyOptions: list of stratifyOptions
        public List<XLSContent> generateMonthlyReport(DateTime startDate, DateTime endDate,
                                                      List<Constants.DataType> dataToDisplay,
                                                      List<Constants.StratifyOption> stratifyOptions)
        {
            if (endDate.Subtract(startDate).Milliseconds < 0)
            {
                // TODO Throw error   
            }

            //create Point representations of month(x) and year(y) to create the columns in the data Table
            int totalNumOfMonths = endDate.Month + (startDate.Year - endDate.Year)*12 - startDate.Month + 1;
            var pointRepsOfMonths = new Point[totalNumOfMonths];
            DateTime curMonth;
            for (int i = 0; i < totalNumOfMonths; i++)
            {
                curMonth = startDate.AddMonths(i);
                pointRepsOfMonths[i] = new Point(curMonth.Month, curMonth.Year);
            }

            var xslContents = new List<XLSContent>();

            //Retrieve all the requests that fall under the time period
            List<Request> searchResults = (from reqs in _db.Requests
                                           where reqs.TimeOpened >= startDate && reqs.TimeOpened <= endDate
                                           orderby reqs.TimeOpened
                                           select reqs).ToList();

            Dictionary<XlsReportContentKey, List<Request>> groupedRequests;
            //List<List<Request>> groupedRequests;

            //if any stratify options are selected, the searchRequests needs to be grouped beforeextracting any data.
            if (stratifyOptions.Any())
            {
                //Code only for callerType and requestorType for now
                foreach (Constants.StratifyOption stratifyBy in stratifyOptions)
                {
                    //stratify and return dictionary of List<Requests> that has correct xslContentKey
                    groupedRequests = stratify2(searchResults, stratifyBy);
                    // Now that requests are grouped, put them in the proper spot of 2D array of requests
                    //Dictionary<XslContentKey,List<Request>> requestsPerSubType = new Dictionary<XslContentKey, List<Request>>();
                    foreach (Constants.DataType dataType in dataToDisplay)
                    {
                        //create XSLContent per stratify option per data
                        //add it to xlsContents.add()
                        var content = new XLSContent();
                        var cellValues = new Dictionary<XlsReportContentKey, long>();
                        foreach (var groupedRequest in groupedRequests)
                        {
                            switch (dataType)
                            {
                                case Constants.DataType.TotalTimeSpent:
                                    long totalTimeSpent = this.totalTimeSpent(groupedRequest.Value);
                                    cellValues.Add(groupedRequest.Key, totalTimeSpent);
                                    break;
                                case Constants.DataType.TotalNumOfRequests:
                                    long totalNum = numOfReqs(groupedRequest.Value);
                                    cellValues.Add(groupedRequest.Key, totalNum);
                                    break;
                                case Constants.DataType.AvgTimeFromStartToComplete:
                                    long avgTime = avgTimeFromStartToComplete(groupedRequest.Value);
                                    cellValues.Add(groupedRequest.Key, avgTime);
                                    break;
                                case Constants.DataType.AvgTimePerRequest:
                                    long avgTimeSpent = averageTime(groupedRequest.Value);
                                    cellValues.Add(groupedRequest.Key, avgTimeSpent);
                                    break;
                                default:
                                    //TODO Throw Error
                                    break;
                            }
                        }
                        content.valueTable = cellValues;
                        content.rowNames = getTypeCodes(stratifyBy).Values.ToArray();
                        content.columnPointReps = pointRepsOfMonths;
                        //content.title = 
                        xslContents.Add(content);
                    }
                }
            }
                //if there's no stratify option selected, only 1 xslContent is needed
                //as different data types will be displayed in the same table.
            else
            {
                groupedRequests = new Dictionary<XlsReportContentKey, List<Request>>();
            }

            return xslContents;
        }

        public void generateYearlyReport(int startYear, int endYear, List<Constants.DataType> dataToDisplay,
                                         Constants.StratifyOption stratifyBy)
        {
            //Creates the list of requests for each year
            var requestsByYear = new List<List<Request>>();
            DateTime start, end;
            List<Request> searchResults;
            for (int i = 0; i < endYear - startYear; i++)
            {
                //Year Round Sep-Aug
                start = new DateTime(startYear, 4, 1, 0, 0, 0);
                end = new DateTime(startYear + 1, 3, 31, 23, 59, 59);
                searchResults =
                    (from reqs in _db.Requests
                     where reqs.TimeOpened > start && reqs.TimeOpened <= end
                     orderby reqs.TimeOpened
                     select reqs)
                        .ToList();
                requestsByYear.Add(searchResults);
            }
        }

        public List<DataTable> generateMonthPerYearReport(int month, int startYear, int endYear,
                                                          List<Constants.DataType> dataToDisplay,
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
                    Dictionary<byte?, List<QuestionResponse>> qrTumoutGrpDic = (from reqs in _db.Requests
                               where
                                   reqs.TimeOpened > start &&
                                   reqs.TimeOpened <= end &&
                                   reqs.TimeOpened.Month == month
                               join qr in _db.QuestionResponses on reqs.RequestID equals qr.RequestID
                               group qr by qr.TumourGroupID
                               into tumourGroups
                               select tumourGroups).ToDictionary(r => r.Key, r => r.ToList());

                    Dictionary<byte?, Dictionary<int, List<QuestionResponse>>> tgAndYear =
                        callerDictionary.ToDictionary(keyValuePair => keyValuePair.Key,
                                                      keyValuePair =>
                                                      keyValuePair.Value.GroupBy(r => r.TimeOpened.Year)
                                                                  .Select(grp => grp)
                                                                  .ToDictionary(grp => grp.Key, grp => grp.ToList()));

                    break;
                default:
                    break;
            }

            return dataTablesForReport;
        }

        private DataTable createDtForEachYear(int startYear, int endYear, Constants.StratifyOption stratifyBy,
                                              Constants.DataType dataType,
                                              Dictionary<byte?, Dictionary<int, List<Request>>> dictionary)
        {
            var dt = new DataTable();
            dt.Clear();
            string stratifyGroups = Enum.GetName(typeof (Constants.StratifyOption), stratifyBy);
            dt.Columns.Add(stratifyGroups);

            for (int i = startYear; i <= endYear; i++)
            {
                dt.Columns.Add(i.ToString());
            }

            Dictionary<byte, string> idToName = getTypeCodes(stratifyBy);
            foreach (var keyValuePair in dictionary)
            {
                DataRow _newRow = dt.NewRow();
                if (keyValuePair.Key.HasValue)
                {
                    _newRow[stratifyGroups] = idToName[keyValuePair.Key.Value];
                }
                else
                {
                    _newRow[stratifyGroups] = "No " + stratifyGroups;
                }

                foreach (var valuePair in keyValuePair.Value)
                {
                    switch (dataType)
                    {
                        case Constants.DataType.AvgTimePerRequest:
                            _newRow[valuePair.Key.ToString()] = averageTime(valuePair.Value);
                            break;
                        case Constants.DataType.AvgTimeFromStartToComplete:
                            _newRow[valuePair.Key.ToString()] = avgTimeFromStartToComplete(valuePair.Value);
                            break;
                        case Constants.DataType.TotalNumOfRequests:
                            _newRow[valuePair.Key.ToString()] = valuePair.Value.Count;
                            break;
                        case Constants.DataType.TotalTimeSpent:
                            _newRow[valuePair.Key.ToString()] = totalTimeSpent(valuePair.Value);
                            break;
                    }
                }
                dt.Rows.Add(_newRow);
            }

            return dt;
        }


        // below are for calculating each cell values for each data type given the list of requests to be used 
        private long averageTime(List<Request> reqList)
        {
            long total = totalTimeSpent(reqList);
            return total/reqList.Count;
        }

        //Given the list of requests, calculates the average time from open to close
        private long avgTimeFromStartToComplete(List<Request> reqList)
        {
            var totalTimeSpan = new TimeSpan(0);

            foreach (Request curRequest in reqList)
            {
                if (curRequest.TimeClosed.HasValue)
                {
                    totalTimeSpan = totalTimeSpan.Add(curRequest.TimeClosed.Value - curRequest.TimeOpened);
                }
            }
            return totalTimeSpan.Minutes/reqList.Count;
        }

        private long totalTimeSpent(List<Request> reqList)
        {
            long total = 0;
            foreach (Request curRequest in reqList)
            {
                List<short> qrTimeSpentResults =
                    (from qrs in _db.QuestionResponses
                     where qrs.RequestID == curRequest.RequestID && qrs.TimeSpent.HasValue
                     orderby qrs.QuestionResponseID
                     select qrs.TimeSpent.Value).
                        ToList();

                total += qrTimeSpentResults.Sum(ts => ts);
            }
            return total;
        }

        private int numOfReqs(List<Request> reqList)
        {
            return reqList.Count;
        }

        private Dictionary<byte, string> getTypeCodes(Constants.StratifyOption stratifyOption)
        {
            Dictionary<byte, string> codes = null;
            switch (stratifyOption)
            {
                case Constants.StratifyOption.Region:
                    codes = (from region in _db.Regions
                             orderby region.RegionId
                             select region).ToDictionary(region => region.RegionId, r => r.RegionValue);
                    break;
                case Constants.StratifyOption.CallerType:
                    codes = (from callerType in _db.RequestorTypes
                             orderby callerType.RequestorCode
                             select callerType.RequestorCode).ToDictionary(ct => ct.RequestorId, ct => ct.Value);
                    break;
                case Constants.StratifyOption.TumorGroup:
                    codes = (from tumorGroup in _db.TumorGroups
                             orderby tumorGroup.TumorGroupCode
                             select tumorGroup.TumorGroupCode).ToDictionary(tg => tg.TumorGroupId, tg => tg.Value);
                    break;
                default:
                    //TODO Throw ERROR
                    break;
            }
            return codes;
        }
    }
}