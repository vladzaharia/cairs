using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SasquatchCAIRS.Models;

namespace SasquatchCAIRS.Controllers {
    [Authorize(Roles = Constants.Roles.ADMINISTRATOR)]
    public class AuditLogManagementController : Controller {
        private CAIRSDataContext _db = new CAIRSDataContext();
  
        /// <summary>
        ///     Add entry to audit log table when an AuditType action is performed on a request.
        /// </summary>
        /// <param name="requestId">The ID of the Request</param>
        /// <param name="userId">The ID of the User</param>
        /// <param name="type">The type of audit action</param>
        public void addEntry(long requestId, int userId,
                             Constants.AuditType type) {
            // Create a new entry in the log, using the Request ID, user ID, and action provided.
            // Set to the current datetime
            _db.AuditLogs.InsertOnSubmit(new AuditLog {
                RequestID = requestId,
                UserID = userId,
                AuditType = (byte) type,
                AuditDate = DateTime.Now
            });

            // Submit to DB.
            _db.SubmitChanges();
        }

        /// <summary>
        ///     Add entry to audit log table when an AuditType action is performed on a request.
        /// </summary>
        /// <param name="requestId">The ID of the Request</param>
        /// <param name="userId">The ID of the User</param>
        /// <param name="type">The type of audit action</param>
        /// <param name="dateTime"></param>
        public void addEntry(long requestId, int userId,
                             Constants.AuditType type, DateTime dateTime) {
            // Create a new entry in the log, using the Request ID, user ID, and action provided.
            // Set to the current datetime
            _db.AuditLogs.InsertOnSubmit(new AuditLog {
                RequestID = requestId,
                UserID = userId,
                AuditType = (byte) type,
                AuditDate = dateTime
            });

            // Submit to DB.
            _db.SubmitChanges();
        }

        /// <summary>
        ///     Create an audit report with all AuditLog entries for a specified request ID.
        /// </summary>
        /// <param name="auditRequests">The request to track activitiy for</param>
        public bool createReportForRequest(List<Request> auditRequests) {

            // Create a new Dictionary, each entry will be an XLS sheet
            Dictionary<string, DataTable> tableExport = new Dictionary<string, DataTable>();
            
            // Create a blank list of DataTables, each DataTable will correspond to user ID
            List<DataTable> xlsExports = new List<DataTable>();

            foreach (Request request in auditRequests) {

                // Create blank list of AuditLogs and fill with all AuditLogs for the given request ID
                List<AuditLog> requestLogs =
                    (from r in _db.AuditLogs
                     where r.RequestID == request.RequestID
                     select r).ToList();

                // Create DataTable with requestLogs to send to XLSExporter
                string sheetName = "Audit Log for Request " + request.RequestID;
                
                var xlsTable =
                    new DataTable(sheetName);

                // Create required columns
                xlsTable.Columns.Add("User Action", typeof(string));
                xlsTable.Columns.Add("Action Timestamp", typeof(DateTime));
                xlsTable.Columns.Add("User", typeof(string));

                // Populate table rows
                requestLogs.ForEach(
                    a =>
                    xlsTable.Rows.Add(
                        Enum.GetName(typeof (Constants.AuditType), a.AuditType),
                        a.AuditDate, a.UserProfile.UserName
                        ));

                // add DataTable to xlsExports
                tableExport.Add(sheetName, xlsTable);
                xlsExports.Add(xlsTable);
            }

            // check if data in report, if not return false
            bool reportData = false;

            foreach (DataTable dt in xlsExports) {
                if (dt.Rows.Count > 0) {
                    reportData = true;
                }
            }

            if (reportData == false) {
                return false;
            }
            
            // Call XLSExporter
            DateTime markDate = new DateTime(2010, 01, 01, 00, 00, 00, 00);
            TimeSpan dateStamp = DateTime.Now.Subtract(markDate);
            string fromPath = Path.Combine(HttpRuntime.AppDomainAppPath,
                                           "AuditLogTemplate.xlsx");
            string toPath = Path.Combine(HttpRuntime.AppDomainAppPath, "AuditLogTemplate" + dateStamp.TotalSeconds.ToString() + ".xlsx");

            ExcelExportController eeController = new ExcelExportController();

            eeController.exportDataTable(Constants.ReportType.AuditLog, tableExport, fromPath, toPath);

            return true;
        }

        /// <summary>
        ///     Create an audit report with all AuditLog entries for a specified user in a specified date range.
        /// </summary>
        /// <param name="userIDs">The user ID to track activitiy for</param>
        /// <param name="startDate"> The start of the specified date range</param>
        /// <param name="endDate">The end of the specified date range</param>
        public bool createReportForUser(IEnumerable<long> userIDs, DateTime startDate,
                                        DateTime endDate) {
            
            // Create Dictionary to exports, each DataTable will correspond to user ID
            Dictionary<string, DataTable> tableExport = new Dictionary<string, DataTable>();
            List<DataTable> xlsExports = new List<DataTable>();

            // Create blank list of AuditLogs and fill with all AuditLogs for each user ID
           foreach (long userID in userIDs) {
               
               var requestLogs = (_db.AuditLogs.Where(r => r.UserID == userID && r.AuditDate.Date >= startDate.Date && r.AuditDate.Date <= endDate.Date)).ToList();

               // Create DataTable with requestLogs to send to XLSExporter
               string userName = (from u in _db.UserProfiles
                                  where userID == u.UserId
                                  select u.UserName).First();
               string sheetName = "Audit Log for User " + userName +
                                  " Between "
                                  + startDate.Date.ToLongDateString() + " and " +
                                  endDate.Date.ToLongDateString();
               
               var xlsTable =
                   new DataTable(sheetName);

               // Create required columns
               xlsTable.Columns.Add("User Action", typeof(string));
               xlsTable.Columns.Add("Request ID", typeof(long));
               xlsTable.Columns.Add("Action Timestamp", typeof(DateTime));

               // Populate table rows
               requestLogs.ForEach(
                   a =>
                   xlsTable.Rows.Add(
                       Enum.GetName(typeof (Constants.AuditType), a.AuditType),
                       a.RequestID.ToString(), a.AuditDate));

               // add DataTable for this ID to xlsExports
              tableExport.Add(sheetName, xlsTable);
               xlsExports.Add(xlsTable);
           }

           // check if data in report, if not return false
           bool reportData = false;

           foreach (DataTable dt in xlsExports) {
               if (dt.Rows.Count > 0) {
                   reportData = true;
               }
           }

            if (reportData == false) {
                return false;
            }
            
            // Call XLSExporter with table(s)
            DateTime markDate = new DateTime(2010, 01, 01, 00, 00, 00, 00);
            TimeSpan dateStamp = DateTime.Now.Subtract(markDate);
            string fromPath = Path.Combine(HttpRuntime.AppDomainAppPath,
                                           "AuditLogTemplate.xlsx");
            string toPath = Path.Combine(HttpRuntime.AppDomainAppPath, "AuditLogTemplate" + dateStamp.TotalSeconds.ToString() + ".xlsx");
            
            ExcelExportController eeController = new ExcelExportController();
            
            eeController.exportDataTable(Constants.ReportType.AuditLog, tableExport, fromPath, toPath);

            return true;
        }
    }
}
