using System;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers
{
    public class RequestController : Controller
    {
        //
        // GET: /Request/Create/

        public ActionResult Create() {
            DropdownController dc = DropdownController.instance;
            var reqContent = new RequestContent() {
                timeOpened = DateTime.Now
            };

            ViewBag.CurrentTime = DateTime.Now;
            ViewBag.RequestorTypes = new SelectList(
                dc.getActiveEntries(Constants.DropdownTable.RequestorType),
                "id", "text");
            ViewBag.Regions = new SelectList(
                dc.getActiveEntries(Constants.DropdownTable.Region),
                "id", "text");

            ViewBag.GenderOptions = new SelectList(Constants.genderOptions);

            return View(reqContent);
        }

        [HttpPost]
        public ActionResult Create(RequestContent reqContent) {
            // TODO: Redirect to request page?
            if (!ModelState.IsValid) {
                return Create();
            } else {
                RequestManagementController rmc =
                    RequestManagementController.instance;
                rmc.create(reqContent);

                return Redirect("/Home/Index");
            }
        }

        public ActionResult NewQuestionResponse(int id) {
            DropdownController dc = DropdownController.instance;
            var qrContent = new QuestionResponseContent();

            // Used to set local question response ID
            ViewBag.LocalId = id;

            // Used to set name of input fields for model binding
            ViewData.TemplateInfo.HtmlFieldPrefix =
                "questionResponseList[" + id + "]";

            ViewBag.QuestionTypes = new SelectList(
                dc.getActiveEntries(Constants.DropdownTable.QuestionType),
                "id", "text");
            ViewBag.TumourGroups = new SelectList(
                dc.getActiveEntries(Constants.DropdownTable.TumourGroup),
                "id", "text");

            return View("Partial/NewQuestionResponse", qrContent);
        }

        public ActionResult NewReference(int id, int refId) {
            var refContent = new ReferenceContent();

            // Used to set local reference ID
            ViewBag.LocalId = id;
            ViewBag.LocalRefId = refId;
            ViewBag.HtmlIdPrefix = "questionResponseList_" + id +
                                   "__referenceList_" + refId + "__";

            // Used to set name of input fields for model binding
            ViewData.TemplateInfo.HtmlFieldPrefix =
                "questionResponseList[" + id +"].referenceList[" +
                refId + "]";

            ViewBag.ReferenceTypes = new SelectList(
                Constants.referenceTypeOptions);

            return View("Partial/NewReference", refContent);
        }
    }
}
