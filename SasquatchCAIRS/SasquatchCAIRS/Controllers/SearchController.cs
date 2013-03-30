using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.SearchSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace SasquatchCAIRS.Controllers {
    /// <summary>
    /// Controller responsible for executing searches and returning the corresponding requests
    /// </summary>
    public class SearchController : Controller {

        private DropdownController _dropdownController =
            new DropdownController();
        private CAIRSDataContext _db = new CAIRSDataContext();

        private static List<Request> _results;
        private static int _startIndex;

        /// <summary>
        /// Given a comma delimited string of keywords returns all requests tbat contain one or more of these keywords
        /// </summary>
        /// <param name="keywords">String of comma delimited keywords</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Search(String keywords) {
            Session["criteria"] = null;
            ViewBag.keywords = keywords;
            SearchCriteria sc = new SearchCriteria();
            sc.anyKeywordString = keywords;
            Session["criteria"] = sc;

            long requestId;
            if (long.TryParse(keywords, out requestId)) {
                
                return RedirectToAction("Details", "Request", new {
                    id = requestId
                });
            }

            _startIndex = 0;
            ViewBag.startIndex = _startIndex;

            _results = searchCriteriaQuery(sc);
            fillUpKeywordDict(_results);
            ViewBag.ResultSetSize = _results.Count;
            ViewBag.criteria = constructCriteriaString(sc);

            return View("Results", _results.Take(Constants.PAGE_SIZE));
        }


        /// <summary>
        /// Displays the Advanced Search view and passes in an empty SearchCriteira 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Advanced() {
            SearchCriteria criteria = new SearchCriteria();
            Session["criteria"] = null;

            setDropdownViewbags();

            return View(criteria);
        }

        /// <summary>
        /// Given a SearchCriteria object and the Advanced Search Form it performs 
        /// a search based upon that criteria and displays the results
        /// </summary>
        /// <param name="criteria">The SearchCriteria object that holds the filtering data</param>
        /// <param name="form">The Form on the Advanced Search page and all it's data</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Results(SearchCriteria criteria, FormCollection form) {
           
            DateTime temp;
            if (DateTime.TryParse(form["startTime"], out temp)) {
                criteria.startTime = temp;
            }
            if (DateTime.TryParse(form["completionTime"], out temp)) {
                criteria.completionTime = temp;
            }
            criteria.anyKeywordString = form["anyKeywordString"];
            criteria.allKeywordString = form["allKeywords"];
            criteria.noneKeywordString = form["noneKeywords"];
            criteria.requestStatus = form["status"];
            criteria.severity = form["severity"];
            criteria.consequence = form["consequence"];
            criteria.tumorGroup = form["tumorGroup"];
            criteria.questionType = form["questionType"];
            criteria.requestorFirstName = form["requestorFirst"];
            criteria.requestorLastName = form["requestorLast"];
            criteria.patientFirstName = form["patientFirst"];
            criteria.patientLastName = form["patientLast"];
           
            if (isEmptySearchCriteria(criteria)) {
                ViewBag.emptyForm = true;
                setDropdownViewbags();
                return View("Advanced", criteria);
            }

            Session["criteria"] = criteria;

            
            ViewBag.keywords = criteria.anyKeywordString;
            _results = searchCriteriaQuery(criteria);
            fillUpKeywordDict(_results);

            ViewBag.ResultSetSize = _results.Count;
            ViewBag.startIndex = 0;
            ViewBag.criteria = constructCriteriaString(criteria);
            return View(_results.Take(Constants.PAGE_SIZE));
        }

        /// <summary>
        /// Used for pagination on the results page for requests
        /// </summary>
        /// <param name="id">Page Number</param>
        /// <returns></returns>
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Page(string id) {

            _startIndex = int.Parse(id) > 0 ? int.Parse(id) : 0;

            ViewBag.keywords =
                ((SearchCriteria) Session["criteria"]).anyKeywordString;
            ViewBag.startIndex = _startIndex;
            ViewBag.ResultSetSize = _results.Count;

            fillUpKeywordDict(_results.Skip(_startIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE));

            return View("Results", _results.Skip(_startIndex * Constants.PAGE_SIZE).Take(Constants.PAGE_SIZE));
        }

        /// <summary>
        /// Populates the Advanced Search page with the SearchCriteria stored in the current Session
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Modify() {

            setDropdownViewbags();
            _startIndex = 0;
            ViewBag.startIndex = _startIndex;
            SearchCriteria criteria = (SearchCriteria) Session["criteria"];
            return View("Advanced", criteria);
        }

        /// <summary>
        /// Given a SearchCriteria object, this generates a list of strings used to represent all of the fields being filtered on
        /// </summary>
        /// <param name="sc">The SearchCriteria object used to filter on Requests</param>
        /// <returns>A list of strings which represents the values of the SearchCriteria object</returns>
        private List<string> constructCriteriaString(SearchCriteria sc) {
            List<string> result = new List<string>();
            if (!String.IsNullOrEmpty(sc.anyKeywordString)) {
                result.Add(Constants.UIString.FieldLabel.ANY_KEYWORDS + ": " +
                          sc.anyKeywordString.TrimEnd(" ,".ToCharArray()));
            }
            if (!String.IsNullOrEmpty(sc.allKeywordString)) {
                result.Add(Constants.UIString.FieldLabel.ALL_KEYWORDS + ": " +
                           sc.allKeywordString.TrimEnd(" ,".ToCharArray()));
            }
            if (!String.IsNullOrEmpty(sc.noneKeywordString)) {
                result.Add(Constants.UIString.FieldLabel.NONE_KEYWORDS + ": " +
                           sc.noneKeywordString.TrimEnd(" ,".ToCharArray()));
            }
            if (sc.startTime.CompareTo(new DateTime()) != 0) {
                result.Add(Constants.UIString.FieldLabel.START_TIME + ": " +
                           sc.startTime.ToShortDateString());
            }
            if (sc.completionTime.CompareTo(new DateTime()) != 0) {
                result.Add(Constants.UIString.FieldLabel.COMPLETED_TIME + ": " +
                           sc.completionTime.ToShortDateString());
            }
            if (!String.IsNullOrEmpty(sc.requestStatus)) {
                result.Add(Constants.UIString.FieldLabel.STATUS + ": " +
                          sc.requestStatus);
            }
            if (!String.IsNullOrEmpty(sc.requestorFirstName)) {
                result.Add(Constants.UIString.FieldLabel.CALLER_FNAME + ": " +
                          sc.requestorFirstName);
            }
            if (!String.IsNullOrEmpty(sc.requestorLastName)) {
                result.Add(Constants.UIString.FieldLabel.CALLER_LNAME + ": " +
                          sc.requestorLastName);
            }
            if (!String.IsNullOrEmpty(sc.patientFirstName)) {
                result.Add(Constants.UIString.FieldLabel.PATIENT_FNAME + ": " +
                          sc.patientFirstName);
            }
            if (!String.IsNullOrEmpty(sc.patientLastName)) {
                result.Add(Constants.UIString.FieldLabel.PATIENT_LNAME + ": " +
                    sc.patientLastName);
            }
            if (!String.IsNullOrEmpty(sc.tumorGroup)) {
                List<int> ids = typeIDStringtoList(sc.tumorGroup, ",");
                string tumorGroups = String.Join(", ", 
                    (from tg in _db.TumourGroups
                        where ids.Contains(tg.TumourGroupID)
                        select tg.Value));
                
                result.Add(Constants.UIString.FieldLabel.TUMOUR_GROUP + ": " +
                          tumorGroups);
            }
            if (!String.IsNullOrEmpty(sc.questionType)) {
                List<int> ids = typeIDStringtoList(sc.tumorGroup, ",");
                string questionTypes = String.Join(", ",
                    (from qt in _db.QuestionTypes
                        where ids.Contains(qt.QuestionTypeID)
                        select qt.Value));

                result.Add(Constants.UIString.FieldLabel.QUESTION_TYPE + ": " +
                          questionTypes);
            }
            if (!String.IsNullOrEmpty(sc.severity)) {
                result.Add(Constants.UIString.FieldLabel.SEVERITY + ": " +
                    sc.severity);
            }
            if (!String.IsNullOrEmpty(sc.consequence)) {
                result.Add(Constants.UIString.FieldLabel.CONSEQUENCE + ": " +
                          sc.consequence);
            }
            return result;
        }

        /// <summary>
        /// Checks if SearchCriteria objects are empty/set to default values
        /// </summary>
        /// <param name="sc">The SearchCriteria to be checked for not-null values</param>
        /// <returns>True if the SearchCriteria is empty, false otherwise</returns>
        private bool isEmptySearchCriteria(SearchCriteria sc) {
            if (!String.IsNullOrEmpty(sc.anyKeywordString) || !String.IsNullOrEmpty(sc.noneKeywordString) ||
                !String.IsNullOrEmpty(sc.allKeywordString) ||!String.IsNullOrEmpty(sc.consequence) 
                || !String.IsNullOrEmpty(sc.patientFirstName) || !String.IsNullOrEmpty(sc.patientLastName)
                || !String.IsNullOrEmpty(sc.questionType) || !String.IsNullOrEmpty(sc.requestStatus)
                || !String.IsNullOrEmpty(sc.requestorFirstName) || !String.IsNullOrEmpty(sc.requestorLastName)
                || !String.IsNullOrEmpty(sc.severity) || !String.IsNullOrEmpty(sc.tumorGroup)
                || sc.startTime.CompareTo(new DateTime()) != 0 || sc.completionTime.CompareTo(new DateTime()) != 0) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the ViewBag objects to hold dropdown values
        /// </summary>
        private void setDropdownViewbags() {
            ViewBag.TumorGroups =
                _dropdownController.getEntries(
                Constants.DropdownTable.TumourGroup).OrderBy(tg => tg.value);
            ViewBag.QuestionType =
                _dropdownController.getEntries(
                    Constants.DropdownTable.QuestionType).OrderBy(qt => qt.value);
        }

        /// <summary>
        /// Converts an input String into a list of Int, used for Question Type ID and Tumour Group ID
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="delimiters">Delimiter inside string</param>
        /// <returns>Corresponding List of Integers</returns>
        private List<int> typeIDStringtoList(string input, string delimiters) {
            string[] arr = input.Split(delimiters.ToCharArray());
            return arr.Select(int.Parse).ToList();
        }

        /// <summary>
        /// Converts a given string into a list of Strings, to separate a  keyword string into individual keywords
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="delimiters">Delimiter inside the string</param>
        /// <returns>Corresponding List of Strings</returns>
        private List<String> keywordsToList(string input, string delimiters) {
            String[] stringArr = input.Split(delimiters.ToCharArray());
            return stringArr.Select(s => s.Trim()).ToList();
        }

        /// <summary>
        /// Converts a String into List of Intgers based on its Enum value specified in Constants.cs
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="type">Enumeration Type</param>
        /// <returns>Corresponding List of Integers for that Type and Input</returns>
        private List<int> enumToIDs(string input, Type type) {
            String[] stringArr = input.Split(",".ToCharArray());
            return stringArr.Select(v => (int) Enum.Parse(type, v)).ToList();
        }

        /// <summary>
        /// Given a list of requests fills up the dictionary of each request's keywords
        /// </summary>
        /// <param name="requests">List of requests to be displayed</param>
        private void fillUpKeywordDict(IEnumerable<Request> requests) {
            if (requests == null) {
                return;
            }
            var keywords = new Dictionary<long, List<string>>();
            foreach (Request request in requests) {
                List<string> kw =
                    (from kws in _db.Keywords
                     join kqs in _db.KeywordQuestions
                         on kws.KeywordID equals kqs.KeywordID
                     where kqs.RequestID == request.RequestID
                     select kws.KeywordValue).Distinct()
                        .ToList();
                keywords.Add(request.RequestID, kw);
            }
            ViewBag.keywordDict = keywords;
        }

        private List<int> getKeywords(string keywordString) {
            // First we grab the keywords
            if (String.IsNullOrEmpty(keywordString))
                return new List<int>();
            return (from k in _db.Keywords
                    where keywordsToList( keywordString, ",").Contains(k.KeywordValue)
                    select k.KeywordID).ToList();
        } 



        /// <summary>
        /// Get Requests in Database based on SearchCriteria
        /// </summary>
        /// <param name="criteria">Search criteria that user inputs</param>
        /// <returns>List of Requests that match the input </returns>
        private List<Request> searchCriteriaQuery(SearchCriteria criteria) {
            IQueryable<Request> requests = _db.Requests;

            // Filter on patient first name
            if (!String.IsNullOrEmpty(criteria.patientFirstName)) {
                requests =
                    requests.Where(
                        r => r.PatientFName == criteria.patientFirstName);
            }

            // Filter on patient last name
            if (!String.IsNullOrEmpty(criteria.patientLastName)) {
                requests =
                    requests.Where(
                        r => r.PatientLName == criteria.patientLastName);
            }

            // Filter on requestor first name
            if (!String.IsNullOrEmpty(criteria.requestorFirstName)) {
                requests =
                    requests.Where(
                        r => r.RequestorFName == criteria.requestorFirstName);
            }

            // Filter on requestor last name
            if (!String.IsNullOrEmpty(criteria.requestorLastName)) {
                requests =
                    requests.Where(
                        r => r.RequestorLName == criteria.requestorLastName);
            }

            // Filter on start time
            if (criteria.startTime != DateTime.Parse(Constants.EMPTY_DATE)) {
                requests =
                    requests.Where(
                        r => r.TimeOpened.CompareTo(criteria.startTime) >= 0);
            }

            // Filter on end time
            if (criteria.completionTime != DateTime.Parse(Constants.EMPTY_DATE)) {
                requests =
                    requests.Where(
                        r =>
                        r.TimeClosed != null &&
                        (criteria.completionTime.CompareTo(r.TimeClosed) <= 0));
            }

            // Set Criteria based on Users Role(s)
            if (Roles.IsUserInRole(Constants.Roles.ADMINISTRATOR)) {} else if (String.IsNullOrEmpty(criteria.requestStatus) &&
                                                                               Roles.IsUserInRole(Constants.Roles.REQUEST_EDITOR)) {
                criteria.requestStatus = Enum.GetName(
                    typeof (Constants.RequestStatus),
                    Constants.RequestStatus.Completed)
                                         + "," +
                                         Enum.GetName(
                                             typeof (Constants.RequestStatus
                                                 ),
                                             Constants.RequestStatus.Open);
            } else if (String.IsNullOrEmpty(criteria.requestStatus) &&
                       Roles.IsUserInRole(Constants.Roles.VIEWER)) {
                criteria.requestStatus =
                    Enum.GetName(typeof (Constants.RequestStatus),
                                 Constants.RequestStatus.Completed);
            }
            // Filter on request status
            if (!String.IsNullOrEmpty(criteria.requestStatus)) {
                requests =
                    requests.Where(
                        r =>
                        enumToIDs(criteria.requestStatus,
                                  typeof (Constants.RequestStatus))
                            .Contains(r.RequestStatus));
            }

            // Filter on Question/Response tuples
            IQueryable<QuestionResponse> questionResponses = _db.QuestionResponses;

            // Filter on QR's Severity
            if (!String.IsNullOrEmpty(criteria.severity)) {
                questionResponses =
                    questionResponses.Where(
                        qr =>
                        enumToIDs(criteria.severity, typeof (Constants.Severity))
                            .Contains((int) qr.Severity));
            }

            // Filter on QR's Consequence
            if (!String.IsNullOrEmpty(criteria.consequence)) {
                questionResponses =
                    questionResponses.Where(
                        qr =>
                        enumToIDs(criteria.consequence,
                                  typeof (Constants.Consequence))
                            .Contains((int) qr.Consequence));
            }

            // Filter on QR's Tumor Group
            if (!String.IsNullOrEmpty(criteria.tumorGroup)) {
                questionResponses =
                    questionResponses.Where(
                        qr =>
                        typeIDStringtoList(criteria.tumorGroup, ",")
                            .Contains(qr.TumourGroup.TumourGroupID));
            }

            // Filter on QR's Question Type
            if (!String.IsNullOrEmpty(criteria.questionType)) {
                questionResponses =
                    questionResponses.Where(
                        qr =>
                        typeIDStringtoList(criteria.questionType, ",")
                            .Contains(qr.QuestionType.QuestionTypeID));
            }

            // Filter QRs based on keywords
            if (!String.IsNullOrEmpty(criteria.anyKeywordString + criteria.noneKeywordString + criteria.allKeywordString)) {
                List<int> any = getKeywords(criteria.anyKeywordString);
                List<int> none =  getKeywords(criteria.noneKeywordString);
                List<int> all = getKeywords(criteria.allKeywordString);

                IQueryable<KeywordQuestion> results = _db.KeywordQuestions;

                if (none.Any()) {
                    List<long> toRemove =
                        (from kq in results
                         where none.Contains(kq.KeywordID)
                         select kq.RequestID).ToList();

                    results = (from r in results
                               where !toRemove.Contains(r.RequestID)
                               select r);
                }
                if (all.Any()) {
                    IQueryable<long> acc = null;
                    foreach (int id in all) {
                        if (acc == null) {
                            acc = (from r in results
                                   where r.KeywordID == id
                                   select r.RequestID);
                        } else {
                            acc = acc.Intersect(from r in results
                                                where r.KeywordID == id
                                                select r.RequestID);
                        }
                    }
                    results = (from r in results
                               where acc.ToList().Contains(r.RequestID)
                               select r);
                }
                if (any.Any()) {
                    results = (from kq in results
                                   where any.Contains(kq.KeywordID)
                                   select kq);
                }

                
                // Then we intersect Keywords with QuestionResponses through the use of a join
                questionResponses = (from kq in results
                                     join qr in questionResponses
                                         on kq.QuestionResponseID equals qr.QuestionResponseID
                                     select qr);
            }

            List<Request> final = (from r in requests
                 join qr in questionResponses
                     on r.RequestID equals qr.RequestID
                    select r).Distinct()
                          .OrderByDescending(r => r.RequestID)
                          .ToList();

            return final;
        }
    }
}