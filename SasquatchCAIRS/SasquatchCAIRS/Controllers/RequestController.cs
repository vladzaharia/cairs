using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Security;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models.ServiceSystem;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers
{
    public class RequestController : Controller
    {
        //
        // GET: /Request/Get/{id}

        [Authorize (Roles = "Viewer")]
        public ActionResult Details(long id) {
            RequestManagementController rmc = RequestManagementController.instance;
            RequestLockController rlc = RequestLockController.instance;

            RequestContent request = rmc.getRequestDetails(id);
            ViewBag.Title = "View Request - Request #" + request.requestID;

            if (!User.IsInRole(Constants.Roles.REQUEST_EDITOR) &&
                request.requestStatus != Constants.RequestStatus.Completed) {
                request = null;
                ViewBag.Title = "View Request - Error";
            }

            if (rlc.isLocked(id) &&
                !User.IsInRole(Constants.Roles.ADMINISTRATOR)) {
                request = null;
                ViewBag.Title = "View Request - Error";
            }
            
            return View(request);
        }

    }
}
