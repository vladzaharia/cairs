using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers
{
    public class ReportController : Controller
    {
        private CAIRSDataContext db = new CAIRSDataContext();

        //Generates the monthly report for given period
        //Parameteres: startDate = 0:00:00 of the date selected, endDate = 23:59:59 of selected date
        //  dataToDisplay: list of data that needs to be displayed, stratifyOptions: list of stratifyOptions
        public List<XSLContent> generateMonthlyReport(DateTime startDate, DateTime endDate, List<Constants.DataType> dataToDisplay, List<Constants.StratifyOption> stratifyOptions) {
            if (endDate.Subtract(startDate).Milliseconds < 0)
            {
                // TODO Throw error   
            }

            //create Point representations of month(x) and year(y) to create the columns in the data Table
            int totalNumOfMonths = endDate.Month + (startDate.Year - endDate.Year)*12 - startDate.Month +1 ;
            Point[] pointRepsOfMonths = new Point[totalNumOfMonths];
            DateTime curMonth;
            for (int i = 0; i < totalNumOfMonths; i++)
            {
                curMonth = startDate.AddMonths(i);
                pointRepsOfMonths[i] = new Point(curMonth.Month, curMonth.Year);
            }
            
            List<XSLContent> xslContents = new List<XSLContent>();

            //Retrieve all the requests that fall under the time period
            List<Request> searchResults = (from reqs in db.Requests
                                 where reqs.TimeOpened >= startDate && reqs.TimeOpened <= endDate
                                 orderby reqs.TimeOpened
                                 select reqs).ToList();

            Dictionary<XslContentKey, List<Request>> groupedRequests;
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
                        XSLContent content = new XSLContent();
                        Dictionary<XslContentKey, long> cellValues = new Dictionary<XslContentKey, long>();
                        foreach (KeyValuePair<XslContentKey, List<Request>> groupedRequest in groupedRequests)
                        {
                            switch (dataType)
                            {
                                case Constants.DataType.TotalTimeSpent:
                                    long totalTimeSpent = this.totalTimeSpent(groupedRequest.Value);
                                    cellValues.Add(groupedRequest.Key, totalTimeSpent);
                                    break;
                                case Constants.DataType.TotalNumOfRequests:
                                    long totalNum = this.numOfReqs(groupedRequest.Value);
                                    cellValues.Add(groupedRequest.Key, totalNum);
                                    break;
                                case Constants.DataType.AvgTimeFromStartToComplete:
                                    long avgTime = this.avgTimeFromStartToComplete(groupedRequest.Value);
                                    cellValues.Add(groupedRequest.Key, avgTime);
                                    break;
                                case Constants.DataType.AvgTimePerRequest:
                                    long avgTimeSpent = this.averageTime(groupedRequest.Value);
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
                groupedRequests = new Dictionary<XslContentKey, List<Request>>();
            }

            return xslContents;

        }

        public void generateYearlyReport(int startYear, int endYear, List<Constants.DataType> dataToDisplay, Constants.StratifyOption stratifyBy) {
            //Creates the list of requests for each year
            List<List<Request>> requestsByYear = new List<List<Request>>();
            DateTime start, end;
            List<Request> searchResults;
            for (int i = 0; i < endYear - startYear; i++) {
                //Year Round Sep-Aug
                start = new DateTime(startYear, 9, 1);
                end = new DateTime(startYear + 1, 8, 31);
                searchResults =
                (from reqs in db.Requests
                 where reqs.TimeOpened > start && reqs.TimeOpened <= end
                 orderby reqs.TimeOpened
                 select reqs)
                 .ToList();
                requestsByYear.Add(searchResults);
            }
        }

        public void generateMonthPerYearReport(int month, int startYear, int endYear, List<Constants.DataType> dataToDisplay, Constants.StratifyOption stratifyBy) {
            //Creates the list of requests for each year
            List<List<Request>> requestsForEachYear = new List<List<Request>>();
            DateTime start, end;
            List<Request> searchResults;
            for (int i = 0; i < endYear - startYear; i++) {
                start = new DateTime(startYear + i, month, 1);
                end = new DateTime(startYear + 1 + i, month, 31);
                searchResults =
                (from reqs in db.Requests
                 where reqs.TimeOpened > start && reqs.TimeOpened <= end
                 orderby reqs.TimeOpened
                 select reqs)
                 .ToList();
                requestsForEachYear.Add(searchResults);
            }
        }


        //Take the list of requests as a parameter and group them using LINQ groupby method
        private List<List<Request>> stratify(List<Request> reqList, Constants.StratifyOption stratifyBy)
        {
            List<List<Request>> groupedRequests = new List<List<Request>>();
            switch (stratifyBy) {
                case Constants.StratifyOption.Region:
                    groupedRequests =
                        reqList.GroupBy(r => r.RegionID).Select(grp => grp.ToList()).ToList();
                    break;
                case Constants.StratifyOption.CallerType:
                    groupedRequests =
                        reqList.GroupBy(r => r.RequestorTypeID).Select(grp => grp.ToList()).ToList();
                    break;
                default:
                    //TODO throw error
                    break;
            }

            return groupedRequests;
        }

        //turning into dictionary directly from stratifying
        //NOTE: might get a better performance if we use double hashing. ie. dictionary of dictionary of lists
        private Dictionary<XslContentKey, List<Request>> stratify2(List<Request> reqList, Constants.StratifyOption stratifyBy)
        {
            Dictionary<XslContentKey, List<Request>> groupedRequests = new Dictionary<XslContentKey, List<Request>>();
            List<Request> requestsForKey;
            switch (stratifyBy) {
                case Constants.StratifyOption.Region:
                    foreach (Request req in reqList)
                    {
                        XslContentKey key = new XslContentKey(req.TimeOpened.Month, req.TimeOpened.Year, req.RegionID);
                        if (groupedRequests.ContainsKey(key))
                        {
                            requestsForKey = groupedRequests[key];
                            requestsForKey.Add(req);
                            groupedRequests[key] = requestsForKey;
                        }
                        else
                        {
                            requestsForKey = new List<Request>();
                            requestsForKey.Add(req);
                            groupedRequests[key] = requestsForKey;
                        }
                    }
                    break;
                case Constants.StratifyOption.CallerType:
                    foreach (Request req in reqList)
                    {
                        XslContentKey key = new XslContentKey(req.TimeOpened.Month, req.TimeOpened.Year, req.RequestorTypeID);
                        if (groupedRequests.ContainsKey(key))
                        {
                            requestsForKey = groupedRequests[key];
                            requestsForKey.Add(req);
                            groupedRequests[key] = requestsForKey;
                        }
                        else
                        {
                            requestsForKey = new List<Request>();
                            requestsForKey.Add(req);
                            groupedRequests[key] = requestsForKey;
                        }
                    }
                    break;
                default:
                    //TODO throw error
                    break;
            }

            return groupedRequests;
        }

        //given the stratify option return the total number of types for the group from database 
        //maybe change this to return the list of callerTypes/regions/tumourGroups
        private int numberOfGroups(Constants.StratifyOption stratifyBy)
        {
            int totalNumOfGroups = 0;
            switch (stratifyBy)
            {
                case Constants.StratifyOption.Region:
                    // totalNumOfGroups = TODO when database for dropdowns are set up
                    break;
                case Constants.StratifyOption.CallerType:
                    break;
                case Constants.StratifyOption.TumorGroup:
                    break;
            }
            return totalNumOfGroups;
        }


        // below are for calculating each cell values for each data type given the list of requests to be used 
        private long averageTime(List<Request> reqList) {
            long total = this.totalTimeSpent(reqList);
            return total / reqList.Count;
        }

        //Given the list of requests, calculates the average time from open to close
        private long avgTimeFromStartToComplete(List<Request> reqList) {

            TimeSpan totalTimeSpan = new TimeSpan(0);

            foreach (Request curRequest in reqList) {
                if (curRequest.TimeClosed.HasValue) {
                    totalTimeSpan = totalTimeSpan.Add(curRequest.TimeClosed.Value - curRequest.TimeOpened);
                }
            }
            return totalTimeSpan.Minutes / reqList.Count;
        }

        private long totalTimeSpent(List<Request> reqList) {
            long total = 0;
            foreach (SasquatchCAIRS.Request curRequest in reqList) {
                List<short> qrTimeSpentResults =
                 (from qrs in db.QuestionResponses
                  where qrs.RequestID == curRequest.RequestID && qrs.TimeSpent.HasValue == true
                  orderby qrs.QuestionResponseID
                  select qrs.TimeSpent.Value).
                  ToList();

                total += qrTimeSpentResults.Sum(ts => ts);
            }
            return total;
        }

        private int numOfReqs(List<Request> reqList) {
            return reqList.Count;
        }

        private Dictionary<byte, string> getTypeCodes(Constants.StratifyOption stratifyOption)
        {
            Dictionary<byte, string> codes = null;
            switch (stratifyOption)
            {
                case Constants.StratifyOption.Region:
                    codes = (from region in db.Regions
                             orderby region.RegionCode
                             select region.RegionCode).ToDictionary(region => region.RegionId);
                    break;
                case Constants.StratifyOption.CallerType:
                    codes = (from callerType in db.RequestorTypes
                             orderby callerType.RequestorCode
                             select callerType.RequestorCode).ToDictionary(callerType => callerType.RequestorId);
                    break;
                case Constants.StratifyOption.TumorGroup:
                    codes = (from tumorGroup in db.TumorGroups
                             orderby tumorGroup.TumorGroupCode
                             select tumorGroup.TumorGroupCode).ToDictionary(tumorGroup => tumorGroup.TumorGroupId);
                    break;
                default:
                    //TODO Throw ERROR
                    break;
            }
            return codes;
        }
    }
}
