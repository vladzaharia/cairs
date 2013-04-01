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
            sc.keywordString = keywords;
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
            criteria.keywordString = form["keywords"];
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

            ViewBag.keywords = criteria.keywordString;
            _results = searchCriteriaQuery(criteria);
            fillUpKeywordDict(_results);

            ViewBag.ResultSetSize = _results.Count;
            ViewBag.startIndex = 0;
            return View(_results.Take(Constants.PAGE_SIZE));
        }

        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Page(string id) {

            _startIndex = int.Parse(id) > 0 ? int.Parse(id) : 0;

            ViewBag.keywords =
                ((SearchCriteria) Session["criteria"]).keywordString;
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
        /// Checks if SearchCriteria objects are empty/set to default values
        /// </summary>
        /// <param name="sc">The SearchCriteria to be checked for not-null values</param>
        /// <returns>True if the SearchCriteria is empty, false otherwise</returns>
        public bool isEmptySearchCriteria(SearchCriteria sc) {
            if (!String.IsNullOrEmpty(sc.keywordString) || !String.IsNullOrEmpty(sc.consequence) 
                || !String.IsNullOrEmpty(sc.patientFirstName) || !String.IsNullOrEmpty(sc.patientLastName)
                || !String.IsNullOrEmpty(sc.questionType) || !String.IsNullOrEmpty(sc.requestStatus)
                || !String.IsNullOrEmpty(sc.requestorFirstName) || !String.IsNullOrEmpty(sc.requestorLastName)
                || !String.IsNullOrEmpty(sc.severity) || !String.IsNullOrEmpty(sc.tumorGroup)
                || !String.IsNullOrEmpty(sc.allKeywordString) || !String.IsNullOrEmpty(sc.noneKeywordString)
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
            IQueryable<QuestionResponse> questionResponses =
                _db.QuestionResponses;
            IQueryable<QuestionResponse> noneQuestionResponses =
                _db.QuestionResponses;
            IQueryable<QuestionResponse> allQuestionResponses =
                _db.QuestionResponses;


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
            if (!String.IsNullOrEmpty(criteria.keywordString)) {


                // First we grab the keywords
                IQueryable<Keyword> keywords = (from k in _db.Keywords
                                                where
                                                    keywordsToList(
                                                        criteria.keywordString,
                                                        ",")
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
                                        on key.QuestionResponseID equals
                                        qr.QuestionResponseID
                                    select qr;



            }
            if (!String.IsNullOrEmpty(criteria.noneKeywordString)) {
            IQueryable<Keyword> noneKeywords = (from k in _db.Keywords
                                                where
                                                    (keywordsToList(
                                                        criteria.noneKeywordString,
                                                        ",")
                                                    .Contains(k.KeywordValue))
                                                select k);

            IQueryable<KeywordQuestion> noneKeywordQuestions =
                (from kqs in _db.KeywordQuestions
                 from k in noneKeywords
                 where k.KeywordID == kqs.KeywordID
                 select kqs);

            noneQuestionResponses = from key in noneKeywordQuestions
                                    join qr in questionResponses
                                        on key.QuestionResponseID equals
                                        qr.QuestionResponseID
                                    select qr;
        }

            if (!String.IsNullOrEmpty(criteria.allKeywordString))
            {
                IQueryable<Keyword> allKeywords = (from k in _db.Keywords
                                                    where
                                                        (keywordsToList(
                                                            criteria.allKeywordString,
                                                            ",")
                                                        .Contains(k.KeywordValue))
                                                    select k);

                IQueryable<KeywordQuestion> allKeywordQuestions =
                    (from kqs in _db.KeywordQuestions
                     from k in allKeywords
                     where k.KeywordID == kqs.KeywordID
                     select kqs);

                long var = allKeywordQuestions.FirstOrDefault().QuestionResponseID;
                allKeywordQuestions = (from key in allKeywordQuestions
                                    where key.QuestionResponseID == var
                                    select key);

                allQuestionResponses = from key in allKeywordQuestions
                                        join qr in questionResponses
                                            on key.QuestionResponseID equals
                                            qr.QuestionResponseID
                                        select qr;
            }

        //Finally we intersect our requests with the question responses and get our results
            IQueryable<Request> searchResults = (from r in requests
                                        join qr in questionResponses
                                            on r.RequestID equals qr.RequestID

                                        select r).Distinct()
                                                 .OrderByDescending(
                                                     r => r.RequestID);
            IQueryable<Request> noneSearchResults = (from r in requests
                                                join qr in noneQuestionResponses
                                                    on r.RequestID equals qr.RequestID

                                                select r).Distinct()
                                                 .OrderByDescending(
                                                     r => r.RequestID);
            IQueryable<Request> allSearchResults = (from r in requests
                                              join qr in allQuestionResponses
                                                  on r.RequestID equals qr.RequestID

                                              select r).Distinct()
                                                 .OrderByDescending(
                                                     r => r.RequestID);
            List<Request> final = new List<Request>();
            if (!String.IsNullOrEmpty(criteria.keywordString) &&
                !String.IsNullOrEmpty(criteria.allKeywordString) && !String.IsNullOrEmpty(criteria.noneKeywordString))
                final = searchResults.Intersect(allSearchResults).Except(noneSearchResults).ToList();
            else if (!String.IsNullOrEmpty(criteria.keywordString) &&
                     !String.IsNullOrEmpty(criteria.allKeywordString) &&
                     String.IsNullOrEmpty(criteria.noneKeywordString))
                final = searchResults.Intersect(allSearchResults).ToList();
            else if (!String.IsNullOrEmpty(criteria.keywordString) &&
                     String.IsNullOrEmpty(criteria.allKeywordString) &&
                     !String.IsNullOrEmpty(criteria.noneKeywordString))
                final = searchResults.Except(noneSearchResults).ToList();
            else if (String.IsNullOrEmpty(criteria.keywordString) &&
                     !String.IsNullOrEmpty(criteria.allKeywordString) &&
                     !String.IsNullOrEmpty(criteria.noneKeywordString))
                final =  allSearchResults.Except(noneSearchResults).ToList();
            else if (!String.IsNullOrEmpty(criteria.keywordString) &&
                String.IsNullOrEmpty(criteria.allKeywordString) && String.IsNullOrEmpty(criteria.noneKeywordString))
                final = searchResults.ToList();
            else if (String.IsNullOrEmpty(criteria.keywordString) &&
                !String.IsNullOrEmpty(criteria.allKeywordString) && String.IsNullOrEmpty(criteria.noneKeywordString))
                final = allSearchResults.ToList();
            else if (String.IsNullOrEmpty(criteria.keywordString) &&
                String.IsNullOrEmpty(criteria.allKeywordString) && !String.IsNullOrEmpty(criteria.noneKeywordString))
                final = new List<Request>((_db.Requests.Except(noneSearchResults))).ToList();
            else if (String.IsNullOrEmpty(criteria.keywordString) &&
                     String.IsNullOrEmpty(criteria.allKeywordString) &&
                     String.IsNullOrEmpty(criteria.noneKeywordString))
                final = searchResults.ToList();
            return final;
        }

    }
}