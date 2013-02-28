using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers
{
    public class SearchController : Controller
    {
        private SearchContext db = new SearchContext();

        //
        // GET: /Search/

        public ActionResult Index()
        {
            return View(db.SearchResults.ToList());
        }

        //
        // POST: /Search/Create

        [HttpPost]
        public ActionResult Search(String searchText) {
            ViewBag.searchText = searchText;
            return View();
        }

        public ActionResult Advanced() {
            return View();
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
    }
}