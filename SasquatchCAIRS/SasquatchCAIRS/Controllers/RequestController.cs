using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers
{
    public class RequestController : Controller
    {
        //
        // GET: /Request/Get/{id}

        [Authorize (Roles = "Viewer")]
        public ActionResult Details(long id) {
            RequestManagementController rm = RequestManagementController.instance;

            RequestContent request = rm.getRequestDetails(id);

            return View(request);
        }

    }
}
