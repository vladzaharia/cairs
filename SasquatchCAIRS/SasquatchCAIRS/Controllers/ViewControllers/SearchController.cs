using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Search;
using SasquatchCAIRS.Controllers.Service;
using SasquatchCAIRS.Models.Common;
using SasquatchCAIRS.Models.Search;

namespace SasquatchCAIRS.Controllers.ViewControllers {
    /// <summary>
    ///     Controller responsible for executing searches and returning the corresponding requests
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SearchController : Controller {
        /// <summary>The Results of the search..</summary>
        private static List<Request> _results;

        /// <summary>The start index, for Pagination.</summary>
        private static int _startIndex;

        /// <summary>Data Context for this Controller.</summary>
        private CAIRSDataContext _db = new CAIRSDataContext();

        /// <summary>Dropdown Management Controller for this Controller.</summary>
        private DropdownManagementController _dropdownController =
            new DropdownManagementController();

        /// <summary>Search Management Controller for this Controller.</summary>
        private SearchManagementController _smc =
            new SearchManagementController();

        /// <summary>
        ///  Given a comma delimited string of keywords returns all requests that contain one or more of these keywords
        /// </summary>
        /// <param name="keywords">String of comma delimited keywords</param>
        /// <returns>The Search Results</returns>
        /// <request type="GET">/Search/Search</request>
        [HttpPost]
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Search(String keywords) {
            Session["criteria"] = new SearchCriteria();
            ViewBag.keywords = keywords;
            var sc = new SearchCriteria();
            sc.anyKeywordString = keywords;

            long requestId;
            if (long.TryParse(keywords, out requestId)) {
                return RedirectToAction("Details", "Request", new {
                    id = requestId
                });
            }
            
            ViewBag.criteria = _smc.constructCriteriaString(sc);
            _startIndex = 0;
            ViewBag.startIndex = _startIndex;
            Session["criteria"] = sc;

            if (!_smc.getKeywords(sc.anyKeywordString).Any()) {
                ViewBag.ResultSetSize = 0;
                return View("Results", new List<Request>());
            }

            _results = _smc.searchCriteriaQueryRoles(sc);
            fillUpKeywordDict(_results);
            ViewBag.ResultSetSize = _results.Count;

            return View("Results", _results.Take(Constants.PAGE_SIZE));
        }

        /// <summary>
        ///     Displays the Advanced Search view and passes in an empty SearchCriteira
        /// </summary>
        /// <returns>The Advanced Search View</returns>
        /// <request type="GET">/Search/Advanced</request>
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Advanced() {
            var criteria = new SearchCriteria();

            setDropdownViewbags();

            return View(criteria);
        }

        /// <summary>
        ///     Given a SearchCriteria object and the Advanced Search Form it performs
        ///     a search based upon that criteria and displays the results
        /// </summary>
        /// <param name="criteria">The SearchCriteria object that holds the filtering data</param>
        /// <param name="form">The Form on the Advanced Search page and all it's data</param>
        /// <returns>The Results Page</returns>
        /// <request type="POST">/Search/Results</request>
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


            if (criteria.startTime > criteria.completionTime) {
                ViewBag.invalidDate = true;
                setDropdownViewbags();
                return View("Advanced", criteria);

            }
            if (_smc.isEmptySearchCriteria(criteria)) {
                ViewBag.emptyForm = true;
                setDropdownViewbags();
                return View("Advanced", criteria);
            }

            Session["criteria"] = criteria;
            ViewBag.startIndex = 0;
            ViewBag.criteria = _smc.constructCriteriaString(criteria);
            if (_smc.emptyButValidKeywords(criteria)) {
                ViewBag.ResultSetSize = 0;
                return View("Results", new List<Request>());
            }

            _results = _smc.searchCriteriaQueryRoles(criteria);
            fillUpKeywordDict(_results);
            ViewBag.ResultSetSize = _results.Count;

            return View(_results.Take(Constants.PAGE_SIZE));
        }

        /// <summary>
        ///     Used for pagination on the results page for requests
        /// </summary>
        /// <param name="id">Page Number</param>
        /// <returns>The Results, paginated</returns>
        /// <request type="GET">/Search/Page</request>
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Page(string id) {
            _startIndex = int.Parse(id) > 0 ? int.Parse(id) : 0;

            ViewBag.criteria = _smc.constructCriteriaString((SearchCriteria) Session["criteria"]);
            ViewBag.startIndex = _startIndex;
            ViewBag.ResultSetSize = _results.Count;

            fillUpKeywordDict(
                _results.Skip(_startIndex*Constants.PAGE_SIZE)
                        .Take(Constants.PAGE_SIZE));

            return View("Results",
                        _results.Skip(_startIndex*Constants.PAGE_SIZE)
                                .Take(Constants.PAGE_SIZE));
        }

        /// <summary>
        ///     Populates the Advanced Search page with the SearchCriteria stored in the current Session
        /// </summary>
        /// <returns>Modify an existing SearchCriteria</returns>
        /// <request type="GET">/Search/Modify</request>
        [Authorize(Roles = Constants.Roles.VIEWER)]
        public ActionResult Modify() {
            setDropdownViewbags();
            _startIndex = 0;
            ViewBag.startIndex = _startIndex;
            var criteria = (SearchCriteria) Session["criteria"];
            return View("Advanced", criteria);
        }

        /// <summary>
        ///     Sets the ViewBag objects to hold dropdown values
        /// </summary>
        private void setDropdownViewbags() {
            ViewBag.TumorGroups =
                _dropdownController.getEntries(
                    Constants.DropdownTable.TumourGroup).OrderBy(tg => tg.value);
            ViewBag.QuestionType =
                _dropdownController.getEntries(
                    Constants.DropdownTable.QuestionType)
                                   .OrderBy(qt => qt.value);
        }

        /// <summary>
        ///     Given a list of requests fills up the dictionary of each request's keywords
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
    }
}