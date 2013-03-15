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

        //
        // POST: /Search/Create

        [HttpPost]
        [Authorize(Roles = "Viewer")]
        public ActionResult Search(String keywords) {
            ViewBag.keywords = keywords;
            SearchCriteria sc = new SearchCriteria();
            sc.keywordString = keywords;
            Session["criteria"] = sc;
            List<Request> list = searchCriteriaQuery(sc);
            ViewBag.ResultSetSize = list.Count;
            return View("Results", list);
        }



        [Authorize(Roles = "Viewer")]
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

        [HttpPost]
        [Authorize(Roles = "Viewer")]
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
            return View(list);
        }

        [Authorize(Roles = "Viewer")]
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

        private List<int> stringToList(string input, string delimiters) {
            List<int> results = new List<int>();
            string[] arr = input.Split(delimiters.ToCharArray());
            foreach (var s in arr) {
                results.Add(int.Parse(s));
            }
            return results;
        }

        private List<int> enumToIDs(string input, Type type) {
            String[] stringArr = input.Split(",".ToCharArray());
            List<int> intList = new List<int>();
            foreach (var v in stringArr) {
                intList.Add((int) Enum.Parse(type, v));
            }
            return intList;
        }


        private List<Request> searchCriteriaQuery(SearchCriteria c) {

            IQueryable<Request> matches = _db.Requests;

            // Filter on patient first name
            if (!String.IsNullOrEmpty(c.patientFirstName)) {
                matches =
                    matches.Where(r => r.PatientFName == c.patientFirstName);
            }

            // Filter on patient last name
            if (!String.IsNullOrEmpty(c.patientLastName)) {
                matches =
                    matches.Where(
                        r => r.PatientLName == c.patientLastName);
            }

            // Filter on requestor first name
            if (!String.IsNullOrEmpty(c.requestorFirstName)) {
                matches =
                    matches.Where(
                        r => r.RequestorFName == c.requestorFirstName);
            }

            // Filter on requestor last name
            if (!String.IsNullOrEmpty(c.requestorLastName)) {
                matches =
                    matches.Where(
                        r => r.RequestorLName == c.requestorLastName);
            }

            // Filter on request status
            if (!String.IsNullOrEmpty(c.requestStatus)) {
                matches =
                    matches.Where(
                        r =>
                        enumToIDs(c.requestStatus,
                                  typeof (Constants.RequestStatus))
                            .Contains((int) r.RequestStatus));
            }

            // Filter on Question/Response tuples
            IQueryable<QuestionResponse> qrs = _db.QuestionResponses;

            // Filter on QR's Severity
            if (!String.IsNullOrEmpty(c.severity)) {
                qrs =
                    qrs.Where(
                        qr =>
                        enumToIDs(c.severity, typeof(Constants.Severity))
                            .Contains((int) qr.Severity));
            }

            // Filter on QR's Tumor Group
            if (!String.IsNullOrEmpty(c.tumorGroup)) {
                qrs =
                    qrs.Where(
                        qr =>
                        stringToList(c.tumorGroup, ",")
                            .Contains(qr.TumourGroup.TumourGroupID));
            }

            // Filter on QR's Question Type
            if (!String.IsNullOrEmpty(c.questionType)) {
                qrs =
                    qrs.Where(
                        qr =>
                        stringToList(c.questionType, ",")
                            .Contains(qr.QuestionType.QuestionTypeID));
            }

            // Filter on keywords
            //if (!String.IsNullOrEmpty(c.keywordString)) {
            //    IQueryable<Keyword> kw = _db.Keywords;
            //    kw = kw.Where(key => stringToList(c.keywordString).Contains());
            //}

            // Join based on QR responses
            return (from m in matches
                           join qr in qrs on m.RequestID equals qr.RequestID
                           select m).ToList();
        }
    }
}