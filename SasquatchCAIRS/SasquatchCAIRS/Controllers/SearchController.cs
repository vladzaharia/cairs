using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.SearchSystem;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers {
    public class SearchController : Controller {

        private DropdownController _dropdownController =
            DropdownController.instance;

        private UserProfileController _profileController =
            new UserProfileController();

        //
                _profileController.getUserProfile(User.Identity.Name);
        // POST: /Search/Create

        [HttpPost]
        [Authorize(Roles = "Viewer")]
        public ActionResult Search(String keywords) {
                _profileController.getUserProfile(User.Identity.Name);
            ViewBag.keywords = keywords;
            var sc = new SearchCriteria();
            sc.keywordString = keywords;
            Session["criteria"] = sc;
            List<Request> list = searchCriteriaQuery(sc);
            ViewBag.ResultSetSize = list.Count;
            return View("Results", list);
        }

        [Authorize(Roles = "Viewer")]
        public ActionResult Advanced() {

                _profileController.getUserProfile(User.Identity.Name);

            ViewBag.TumorGroups =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.TumourGroup);
            ViewBag.QuestionType =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.QuestionType);
            return View(criteria);
        }

        [HttpPost]
        [Authorize(Roles="Viewer")]
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
            var list = searchCriteriaQuery(criteria);
            ViewBag.ResultSetSize = list.Count;
            return View();
        }

        [Authorize(Roles = "Viewer")]
        public ActionResult Modify() {
                _profileController.getUserProfile(User.Identity.Name);
            ViewBag.TumorGroups =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.TumourGroup);
            ViewBag.QuestionType =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.QuestionType);
            var criteria = (SearchCriteria) Session["criteria"];
            return View("Advanced", criteria);
        }

        protected override void Dispose(bool disposing) {
            _db.Dispose();
            base.Dispose(disposing);
        }

        private List<string> stringToList(string input, string delimiters) {
            return input.Split(delimiters.ToCharArray()).ToList();
        } 
        private List<Request> searchCriteriaQuery(SearchCriteria c) {
            return (IQueryable<Request>)
                   (from r in _db.SearchResults.AsEnumerable()
                    where r.QuestionResponses.contains(c.keywordString)
                    where r.RequestStatus == c.requestStatus
                    where r.Severity == c.severity
                    where r.PatientFName == c.patientFirstName
                    where r.PatientLName == c.patientLastName
                    where c.tumorGroup.Contains(r.TumorGroup)
                    where c.questionType.Contains(r.QuestionType)
                    where r.RequestorFName == c.requestorFirstName
                    where r.RequestorLName == c.requestorLastName
                    select r).ToList();
        }
    }
}