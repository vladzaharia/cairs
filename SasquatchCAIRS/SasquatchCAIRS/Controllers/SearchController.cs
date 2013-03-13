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
        // GET: /Search/

        public ActionResult Index() {
            ViewBag.Profile = _profileController.getUserProfile(User.Identity.Name);
            return View(_db.SearchResults.ToList());
        }

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
        /// <summary>
        /// This takes all the data from the form and dumps it into the criteria object
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Results(SearchCriteria criteria, FormCollection form) {
            ViewBag.Profile = _profileController.getUserProfile(User.Identity.Name);

            criteria.requestStatus = form["status"];
            criteria.severity = form["severity"];
            criteria.consequence = form["consequence"];
            criteria.tumorGroup = form["tumorGroup"];
            criteria.questionType = form["questionType"];
            criteria.requestorFirstName = form["requestorFirst"];
            criteria.requestorLastName = form["requestorLast"];
            criteria.patientFirstName = form["patientFirst"];
            criteria.patientLastName = form["patientLast"]; //why twice?
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
            return View(viewName: "Advanced", model: criteria);
        }

        //
        // GET: /Search/Details/5

        public ActionResult Details(long id = 0) {
            Request request = _db.SearchResults.Find(id);
            if (request == null) {
                return HttpNotFound();
            }
            return View(request);
        }

        //
        // GET: /Search/Edit/5

        public ActionResult Edit(long id = 0) {
            Request request = _db.SearchResults.Find(id);
            if (request == null) {
                return HttpNotFound();
            }
            return View(request);
        }

        //
        // POST: /Search/Edit/5

        [HttpPost]
        public ActionResult Edit(Request request) {
            if (ModelState.IsValid) {
                _db.Entry(request).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(request);
        }

        protected override void Dispose(bool disposing) {
            _db.Dispose();
            base.Dispose(disposing);
        }



            return (IQueryable<Request>)
                   (from r in db.SearchResults
                    where r.requestStatus == c.requestStatus
                    where r.severity == c.severity
                    where r.patientFirstName == c.patientFirstName
                    where r.patientLastName == c.patientLastName
                    where c.tumorGroup.Contains(r.tumorGroup)
                    where c.questionType.Contains(r.questionType)
                    where r.requestorFirstName == c.requestorFirstName
                    where r.requestorLastName == c.requestorLastName
                    select r).toList();
        }
         

        private List<Request> searchCriteriaQuery(SearchCriteria c) {
            return new List<Request>();
    }
        //        if (!string.IsNullOrEmpty(c.keywordString)) {
    
        //            List<SasquatchCAIRS.Request> searchResults =
        //                (db.SearchResults()
        //                    from r in db.SearchResults()
        //                   .Where(r => c.keywordString.Contains(r.keywordString))
        //                   .Where(r => r.requestStatus == c.requestStatus)
        //                   .Where(r => r.severity == c.severity)
        //                   .Where(r => r.patientFirstName == c.patientFirstName)
        //                   .Where(r => r.patientLastName == c.patientLastName)
        //                   .Where(r => c.tumorGroup.Contains(r.tumorGroup))
        //                   .Where(r => c.questionType.Contains(r.questionType))
        //                   .Where(r => r.requestorFName == c.requestorFirstName)
        //                   .Where(r => r.requestorLName == c.requestorLastName)).ToList();
        //        }
        //        foreach (r in searchResults)
        //            display(r);
        //    }
}