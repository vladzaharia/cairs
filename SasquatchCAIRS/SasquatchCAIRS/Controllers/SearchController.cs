using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SasquatchCAIRS.Models.ServiceSystem;
using SasquatchCAIRS.Models;


namespace SasquatchCAIRS.Controllers
{
    public class SearchController : Controller
    {
        UserProfileController profileController = new UserProfileController();
        private SearchContext db = new SearchContext();

        //
        // GET: /Search/

        public ActionResult Index()
        {
            ViewBag.Profile = profileController.getUserProfile(User.Identity.Name);
            return View(db.SearchResults.ToList());
        }

        //
        // POST: /Search/Create

        [HttpPost]
        public ActionResult Search(String keywords) {
            ViewBag.Profile = profileController.getUserProfile(User.Identity.Name);
            ViewBag.keywords = keywords;
            SearchCriteria sc = new SearchCriteria();
            sc.keywordString = keywords;
            Session["criteria"] = sc;
            List<Request> list = searchCriteriaQuery(sc);
            ViewBag.ResultSetSize = list.Count;
            return View("Results", list);
        }

        public ActionResult Advanced() {
            ViewBag.Profile = profileController.getUserProfile(User.Identity.Name);
            SearchCriteria criteria = new SearchCriteria();
            return View(criteria);
        }

        [HttpPost]
        public ActionResult Results(SearchCriteria criteria, FormCollection form) {
            ViewBag.Profile = profileController.getUserProfile(User.Identity.Name);

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
            IQueryable<Request> list = searchCriteriaQuery(criteria);
            ViewBag.ResultSetSize = list.Count;
            return View();
        }

        [HttpPost]
        public ActionResult Modify() {
            ViewBag.Profile = profileController.getUserProfile(User.Identity.Name);
            SearchCriteria criteria = (SearchCriteria)Session["criteria"];
            return View("Advanced", criteria);
        }





        //
        // GET: /Search/Details/5

        public ActionResult Details(long id = 0)
        {
            Request request = db.SearchResults.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        //
        // GET: /Search/Edit/5

        public ActionResult Edit(long id = 0)
        {
            Request request = db.SearchResults.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        //
        // POST: /Search/Edit/5

        [HttpPost]
        public ActionResult Edit(Request request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(request);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private IQueryable<Request> searchCriteriaQuery(SearchCriteria c) {
            List<Request> searchResults = new List<Request>();



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
         

    }
    

}