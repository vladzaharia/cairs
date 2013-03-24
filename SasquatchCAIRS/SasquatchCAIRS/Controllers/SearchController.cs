using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models.SearchSystem;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers {
    /// <summary>
    /// Controller responsible for executing searches and returing the relevant requests
    /// </summary>
    public class SearchController : Controller {

        private DropdownController _dropdownController =
            new DropdownController();
        private CAIRSDataContext _db = new CAIRSDataContext();

        /// <summary>
        /// Given a comma delimited string of keywords returns all requests with one or more of those keywords
        /// </summary>
        /// <param name="keywords">String of comma delimited keywords</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Search(String keywords) {
            Session["criteria"] = null;
            ViewBag.keywords = keywords;
            SearchCriteria sc = new SearchCriteria();
            sc.keywordString = keywords;
            Session["criteria"] = sc;

            long requestId;
            if (long.TryParse(keywords, out requestId)) {
                return RedirectToAction("Details", "Request", new {
                    id = requestId
                });
            }

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
            Session["criteria"] = null;

            setDropdownViewbags();

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
            criteria.keywordString = form["keywords"];
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

            setDropdownViewbags();

            SearchCriteria criteria = (SearchCriteria) Session["criteria"];
            return View("Advanced", criteria);
        }

        /// <summary>
        /// Checks if SearchCriteria objects are empty/set to default values
        /// </summary>
        /// <param name="sc">The SearchCriteria to be checked for not-null values</param>
        /// <returns>True if the SearchCriteria is empty, false otherwise</returns>
        private bool isEmptySearchCriteria(SearchCriteria sc) {
            if (!String.IsNullOrEmpty(sc.keywordString) || !String.IsNullOrEmpty(sc.consequence) ||
                !String.IsNullOrEmpty(sc.patientFirstName) || !String.IsNullOrEmpty(sc.patientLastName)
                || !String.IsNullOrEmpty(sc.questionType) || !String.IsNullOrEmpty(sc.requestStatus)
                || !String.IsNullOrEmpty(sc.requestorFirstName) || !String.IsNullOrEmpty(sc.requestorLastName)
                || !String.IsNullOrEmpty(sc.severity) || !String.IsNullOrEmpty(sc.tumorGroup)) {
                return false;
            }
            if (sc.startTime.CompareTo(new DateTime()) != 0 || sc.completionTime.CompareTo(new DateTime()) != 0) {
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
        /// Clean up managed and unmanaged resources
        /// </summary>
        /// <param name="disposing">Scenario to operate under</param>
        /// <summary>
        /// Converts an input String into a list of Int, used for Question Type ID and Tumour Group ID
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="delimiters">Delimiter inside string</param>
        /// <returns>Corresponding List of Integers</returns>
        private List<int> typeIDStringtoList(string input, string delimiters) {
            string[] arr = input.Split(delimiters.ToCharArray());
            return arr.Select(s => int.Parse(s)).ToList();
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
                     select kws.KeywordValue)
                        .ToList();
                keywords.Add(request.RequestID, kw);
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

            // Set Criteria based on Users Role(s)
            if (Roles.IsUserInRole(Constants.Roles.ADMINISTRATOR)) {
            } else if (String.IsNullOrEmpty(criteria.requestStatus) && Roles.IsUserInRole(Constants.Roles.REQUEST_EDITOR)) {
                criteria.requestStatus = Enum.GetName(typeof(Constants.RequestStatus), Constants.RequestStatus.Completed)
                    + "," + Enum.GetName(typeof(Constants.RequestStatus), Constants.RequestStatus.Open);
            } else if (String.IsNullOrEmpty(criteria.requestStatus) && Roles.IsUserInRole(Constants.Roles.VIEWER)) {
                criteria.requestStatus = Enum.GetName(typeof(Constants.RequestStatus), Constants.RequestStatus.Completed);
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
            if (!String.IsNullOrEmpty(criteria.keywordString)) {


                // First we grab the keywords
                IQueryable<Keyword> keywords = (from k in _db.Keywords
                                                where
                                                    keywordsToList(criteria.keywordString, ",")
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
                    select r).Distinct().ToList();
        }

    }
}