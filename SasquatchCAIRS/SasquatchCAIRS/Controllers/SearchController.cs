using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models.SearchSystem;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers {
    public class SearchController : Controller {

        private DropdownController _dropdownController =
            DropdownController.instance;
        private CAIRSDataContext _db = new CAIRSDataContext();

        /// <summary>
        /// Given a list of keywords returns all requests with one or more of those keywords
        /// </summary>
        /// <param name="keywords">String of comma delimited keywords</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Search(String keywords) {
            ViewBag.keywords = keywords;
            SearchCriteria sc = new SearchCriteria();
            sc.keywordString = keywords;
            Session["criteria"] = sc;
            List<Request> list = searchCriteriaQuery(sc);
            fillUpKeywordDict(list);
            ViewBag.ResultSetSize = list.Count;
            return View("Results", list);
        }


        /// <summary>
        /// Displays the Advanced Search view and passes in an empty SearchCriteira 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Advanced() {
            SearchCriteria criteria = new SearchCriteria();

            ViewBag.TumorGroups =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.TumourGroup).OrderBy(tg => tg.value);
            ViewBag.QuestionType =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.QuestionType).OrderBy(qt => qt.value);

            return View(criteria);
        }

        /// <summary>
        /// Given a SearchCriteria object and the Adv. Search Form it preforms 
        /// a search based upon that criteria and displays the results
        /// </summary>
        /// <param name="criteria">The SearchCriteria object that hold the filtering data</param>
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
            criteria.requestStatus = form["status"];
            criteria.severity = form["severity"];
            criteria.consequence = form["consequence"];
            criteria.tumorGroup = form["tumorGroup"];
            criteria.questionType = form["questionType"];
            criteria.requestorFirstName = form["requestorFirst"];
            criteria.requestorLastName = form["requestorLast"];
            criteria.patientFirstName = form["patientFirst"];
            criteria.patientLastName = form["patientLast"];
            Session["criteria"] = criteria;


            ViewBag.keywords = criteria.keywordString;
            List<Request> list = searchCriteriaQuery(criteria);
            fillUpKeywordDict(list);
            ViewBag.ResultSetSize = list.Count;
            return View(list);
        }

        /// <summary>
        /// Populates the Advanced Search page with the SearchCriteria stored in the current Session
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Modify() {

            ViewBag.TumorGroups =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.TumourGroup).OrderBy(tg => tg.value);
            ViewBag.QuestionType =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.QuestionType).OrderBy(qt => qt.value);

            SearchCriteria criteria = (SearchCriteria) Session["criteria"];
            return View("Advanced", criteria);
        }

        protected override void Dispose(bool disposing) {
            _db.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Converts an input String into a list of Int, used for Question Type ID and Tumour Group ID
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="delimiters">Delimiter inside string</param>
        /// <returns>Corresponding List of Integers</returns>
        private List<int> stringToList(string input, string delimiters) {
            List<int> results = new List<int>();
            string[] arr = input.Split(delimiters.ToCharArray());
            foreach (var s in arr) {
                results.Add(int.Parse(s));
            }
            return results;
        }

        /// <summary>
        /// Converts a given string into a list of Strings, to separate a  keyword string into individual keywords
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="delimiters">Delimiter inside the string</param>
        /// <returns>Corresponding List of Strings</returns>
        private List<String> stringToSList(string input, string delimiters) {
            return input.Split(delimiters.ToCharArray()).ToList();
        }

        /// <summary>
        /// Converts a String into List of Intgers based on its Enum value specified in Constants.cs
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="type">Specific Type (Severity or Consequence)</param>
        /// <returns>Corresponding List of Integers for that Type and Input</returns>
        private List<int> enumToIDs(string input, Type type) {
            String[] stringArr = input.Split(",".ToCharArray());
            List<int> intList = new List<int>();
            foreach (var v in stringArr) {
                intList.Add((int) Enum.Parse(type, v));
            }
            return intList;
        }

        /// <summary>
        /// Given a list of requests fills up the dictionary of each request's keywords
        /// </summary>
        /// <param name="requests">List of requests to be displayed</param>
        private void fillUpKeywordDict(IEnumerable<Request> requests) {
            var keywords = new Dictionary<long, List<string>>();
            foreach (Request request in requests) {
                IQueryable<KeywordQuestion> keywordQuestions =
                    from keywordQuestion in _db.KeywordQuestions
                    where keywordQuestion.RequestID == request.RequestID
                    select keywordQuestion;
                keywords.Add(request.RequestID, (from keyword in _db.Keywords
                                                 join keywordQuestion in keywordQuestions
                                                 on keyword.KeywordID equals keywordQuestion.KeywordID
                                                 select keyword.KeywordValue).ToList());
            }
            ViewBag.keywordDict = keywords;
        }

        /// <summary>
        /// Get Requests in Database based on SearchCriteria
        /// </summary>
        /// <param name="criteria">Search criteria that users inputs</param>
        /// <returns>List of Requests that match the input </returns>
        private List<Request> searchCriteriaQuery(SearchCriteria criteria) {
            IQueryable<Request> requests = _db.Requests;

            // Filter on patient first name
            if (!String.IsNullOrEmpty(criteria.patientFirstName)) {
                requests =
                    requests.Where(r => r.PatientFName == criteria.patientFirstName);
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
                requests = requests.Where(r => r.TimeOpened.CompareTo(criteria.startTime) >= 0);
            }

            // Filter on end time
            if (criteria.completionTime != DateTime.Parse(Constants.EMPTY_DATE)) {
                requests =
                    requests.Where(r => r.TimeClosed != null && (criteria.completionTime.CompareTo(r.TimeClosed) <= 0));
            }

            // Filter on request status
            if (!String.IsNullOrEmpty(criteria.requestStatus)) {
                requests =
                    requests.Where(
                        r =>
                        enumToIDs(criteria.requestStatus,
                                  typeof(Constants.RequestStatus))
                            .Contains(r.RequestStatus));
            }

            // Filter on Question/Response tuples
            IQueryable<QuestionResponse> questionResponses = _db.QuestionResponses;

            // Filter on QR's Severity
            if (!String.IsNullOrEmpty(criteria.severity)) {
                questionResponses =
                    questionResponses.Where(
                        qr =>
                        enumToIDs(criteria.severity, typeof(Constants.Severity))
                            .Contains((int) qr.Severity));
            }

            // Filter on QR's Tumor Group
            if (!String.IsNullOrEmpty(criteria.tumorGroup)) {
                questionResponses =
                    questionResponses.Where(
                        qr =>
                        stringToList(criteria.tumorGroup, ",")
                            .Contains(qr.TumourGroup.TumourGroupID));
            }

            // Filter on QR's Question Type
            if (!String.IsNullOrEmpty(criteria.questionType)) {
                questionResponses =
                    questionResponses.Where(
                        qr =>
                        stringToList(criteria.questionType, ",")
                            .Contains(qr.QuestionType.QuestionTypeID));
            }

            // Filter QRs based on keywords
            if (!String.IsNullOrEmpty(criteria.keywordString)) {


                // First we grab the keywords
                IQueryable<Keyword> keywords = (from k in _db.Keywords
                                                where
                                                    stringToSList(criteria.keywordString, ",")
                                                    .Contains(k.KeywordValue)
                                                select k);

                // Then we select the Keyword Question pairs with the same keywords
                IQueryable<KeywordQuestion> keywordQuestions =
                     (from kqs in _db.KeywordQuestions
                      from k in keywords
                      where k.KeywordID == kqs.KeywordID
                      select kqs);

                // Then we intersect Keywords with QuestionResponses through the use of a join
                questionResponses = from key in keywordQuestions
                                    join qr in questionResponses
                                    on key.QuestionResponseID equals qr.QuestionResponseID
                                    select qr;
            }
            //Finally we intersect our requests with the question responses and get our results
            return (from r in requests
                    join qr in questionResponses
                    on r.RequestID equals qr.RequestID
                    select r).ToList();
        }

    }
}