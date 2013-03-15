using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models.SearchSystem;
using SasquatchCAIRS.Models.ServiceSystem;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers
{
    public class SearchController : Controller
    {

        private DropdownController _dropdownController =
            DropdownController.instance;
        private CAIRSDataContext _db = new CAIRSDataContext();

        //
        // POST: /Search/Create

        [HttpPost]
        [Authorize(Roles = "Viewer")]
        public ActionResult Search(String keywords)
        {
            ViewBag.keywords = keywords;
            SearchCriteria sc = new SearchCriteria();
            sc.keywordString = keywords;
            Session["criteria"] = sc;
            List<Request> list = searchCriteriaQuery(sc);
            ViewBag.ResultSetSize = list.Count;
            return View("Results", list);
        }

        [Authorize(Roles = "Viewer")]
        public ActionResult Advanced()
        {
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
        public ActionResult Results(SearchCriteria criteria, FormCollection form)
        {
            DateTime temp;
            if (DateTime.TryParse(form["startTime"], out temp))
            {
                criteria.startTime = temp;
            }
            if (DateTime.TryParse(form["completionTime"], out temp))
            {
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
        public ActionResult Modify(){

            ViewBag.TumorGroups =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.TumourGroup).OrderBy(tg => tg.value);
            ViewBag.QuestionType =
                _dropdownController.getActiveEntries(
                    Constants.DropdownTable.QuestionType).OrderBy(qt => qt.value);

            SearchCriteria criteria = (SearchCriteria)Session["criteria"];
            return View("Advanced", criteria);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        private List<string> stringToList(string input, string delimiters)
        {
            return input.Split(delimiters.ToCharArray()).ToList();
        }

        private List<int> stringToIDs(string input, string delimiters) {
            return input.Split(delimiters.ToCharArray()).Select(int.Parse).ToList();
        } 
        private List<Request> searchCriteriaQuery(SearchCriteria c) {
            /*return (from r in _db.SearchResults.AsEnumerable()
                    where r.RequestStatus.ToString() == c.requestStatus     
                    where r.PatientFName == c.patientFirstName
                    where r.PatientLName == c.patientLastName
                    where r.RequestorFName == c.requestorFirstName
                    where r.RequestorLName == c.requestorLastName
                    from qr in r.QuestionResponses
                    where stringToIDs(c.severity, ",").Contains((int) qr.Severity)
                    where stringToIDs(c.tumorGroup, ",").Contains((int) qr.TumourGroupID)
                    where stringToIDs(c.questionType, ",").Contains((int) qr.QuestionTypeID)

                    select r).ToList(); */
            List<Request> searchRequests =
                (from r in _db.Requests
                 where ((!String.IsNullOrEmpty(c.requestStatus)) && r.RequestStatus.ToString().Contains(c.requestStatus))
                 || ((!String.IsNullOrEmpty(c.patientFirstName)) && r.PatientFName == c.patientFirstName)
                 || ((!String.IsNullOrEmpty(c.patientLastName)) && r.PatientLName == c.patientLastName)
                 || ((!String.IsNullOrEmpty(c.requestorFirstName)) && r.RequestorFName == c.requestorFirstName)
                 || ((!String.IsNullOrEmpty(c.requestorLastName)) && r.RequestorLName == c.requestorLastName)
                 select r).ToList();

            List<Request> searchResults = new List<Request>();
            foreach (var r in searchRequests) {
               searchResults = (from qr in r.QuestionResponses
                 where (qr.Severity != null && (stringToIDs(c.severity, ",").Contains((int) qr.Severity))
                                               ||
                           (qr.TumourGroupID != null &&   stringToIDs(c.tumorGroup, ",")
                                                   .Contains((int) qr.TumourGroupID))
                                               ||
                            (qr.QuestionTypeID != null &&    stringToIDs(c.questionType, ",")
                                                   .Contains((int) qr.QuestionTypeID)))
                 select r).ToList();

                
            }
            return searchRequests;
        }
    }
}