using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Models.SearchSystem;
using SasquatchCAIRS.Models.ServiceSystem;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers {
    public class SearchController : Controller {
        UserProfileController _profileController = new UserProfileController();

        private DropdownController _dropdownController =
            DropdownController.instance;
        private SearchContext _db = new SearchContext();

        //
        // POST: /Search/Create

        [HttpPost]
        public ActionResult Search(String keywords) {
            ViewBag.Profile = _profileController.getUserProfile(User.Identity.Name);
            ViewBag.keywords = keywords;
            SearchCriteria sc = new SearchCriteria();
            sc.keywordString = keywords;
            Session["criteria"] = sc;
            List<Request> list = searchCriteriaQuery(sc);
            ViewBag.ResultSetSize = list.Count;
            return View("Results", list);
        }

        public ActionResult Advanced() {
            ViewBag.Profile = _profileController.getUserProfile(User.Identity.Name);

            SearchCriteria criteria = new SearchCriteria();

            ViewBag.TumorGroups =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.TumourGroup);
            ViewBag.QuestionType =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.QuestionType);
            return View(criteria);
        }

        [HttpPost]
        public ActionResult Results(SearchCriteria criteria, FormCollection form) {
            ViewBag.Profile = _profileController.getUserProfile(User.Identity.Name);
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

        public ActionResult Modify() {
            ViewBag.Profile = _profileController.getUserProfile(User.Identity.Name);
            ViewBag.TumorGroups =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.TumourGroup);
            ViewBag.QuestionType =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.QuestionType);
            SearchCriteria criteria = (SearchCriteria) Session["criteria"];
            return View("Advanced", criteria);
        }

        protected override void Dispose(bool disposing) {
            _db.Dispose();
            base.Dispose(disposing);
        }


        private List<Request> searchCriteriaQuery(SearchCriteria c) {
            return new List<Request>();
        }
    }
}