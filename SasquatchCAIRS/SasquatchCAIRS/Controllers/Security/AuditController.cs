using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Security;

namespace SasquatchCAIRS.Controllers {
     [Authorize(Roles = "Administrator")]
    public class AuditController : Controller {

        public class AuditReportGenerator {

            [DataType(DataType.Text)]
            public long requestId {
                get;
                set;
            }

            [DataType(DataType.Text)]
            public int userId {
                get;
                set;
            }

            [DataType(DataType.Date)]
            public DateTime startTime {
                get;
                set;
            }

            [DataType(DataType.Date)]
            public DateTime completionTime {
                get;
                set;
            }
        }
    }
}
