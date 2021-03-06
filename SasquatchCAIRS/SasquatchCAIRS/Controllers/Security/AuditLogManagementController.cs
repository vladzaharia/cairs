﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SasquatchCAIRS.Controllers.Export;
using SasquatchCAIRS.Models.Common;

namespace SasquatchCAIRS.Controllers.Security {
    /// <summary>
    ///     Manages the Audit Logs for Requests.
    /// </summary>
    public class AuditLogManagementController : Controller {
        /// <summary>Data Context for this Controller.</summary>
        private CAIRSDataContext _db = new CAIRSDataContext();

        /// <summary>
        ///     Constructor with default Controller Context
        /// </summary>
        public AuditLogManagementController() : this(new ControllerContext()) {}

        /// <summary>
        ///     Constructor with custom Controller Context
        /// </summary>
        /// <param name="ctx"></param>
        public AuditLogManagementController(ControllerContext ctx) {
            ControllerContext = ctx;
        }

        /// <summary>
        ///     Add entry to audit log table when an AuditType action is performed on a request.
        /// </summary>
        /// <param name="requestId">The ID of the Request</param>
        /// <param name="userId">The ID of the User</param>
        /// <param name="type">The type of audit action</param>
        public void addEntry(long requestId, int userId,
                             Constants.AuditType type) {
            // Check if userID is valid
            bool userFound = ((from up in _db.UserProfiles
                               where up.UserId == userId
                               select up.UserId).ToList()).Count != 0;

            // Check if requestID is valid
            bool idFound = (_db.Requests.Where(u => u.RequestID == requestId)
                               .Select(u => u.RequestID)).Count() != 0;

            if (!userFound) {
                throw new UserDoesNotExistException(userId);
            }

            if (!idFound) {
                throw new RequestDoesNotExistException(requestId);
            }

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
            // Check if requestID is valid
            bool userFound = ((from up in _db.UserProfiles
                               where up.UserId == userId
                               select up.UserId).ToList()).Count != 0;

            // Check if userID is valid
            bool idFound = (_db.Requests.Where(u => u.RequestID == requestId)
                               .Select(u => u.RequestID)).Count() != 0;

            // Check if datetime is valid
            bool dateValid = DateTime.MinValue < dateTime &&
                             dateTime < DateTime.MaxValue;

            if (!userFound) {
                throw new UserDoesNotExistException(userId);
            }

            if (!idFound) {
                throw new RequestDoesNotExistException(requestId);
            }

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
        ///     If report will have data based on Requests specified, call to ExcelExporter and return true, or produce false if there is no data in the report
        /// </summary>
        /// <param name="auditRequests">The request(s) to track activitiy for</param>
        public bool createReportForRequest(List<Request> auditRequests) {
            // Check if auditRequests were correctly passed
            if (auditRequests == null) {
                throw new AuditRequestsDoNotExistException();
            }

            // Create a new Dictionary, each entry will be an XLS sheet
            var tableExport = new Dictionary<string, DataTable>();

            // Create a blank list of DataTables, each DataTable will correspond to user ID
            var xlsExports = new List<DataTable>();

            foreach (Request request in auditRequests) {
                // Create blank list of AuditLogs and fill with all AuditLogs for the given request ID
                List<AuditLog> requestLogs =
                    _db.AuditLogs.Where(r => r.RequestID == request.RequestID)
                       .OrderBy(ars => ars.AuditDate).ToList();

                // Create DataTable with requestLogs to send to XLSExporter
                string sheetName = "Audit Log for Request " + request.RequestID;

                var xlsTable =
                    new DataTable(sheetName);

                // Create required columns
                xlsTable.Columns.Add("User Action", typeof (string));
                xlsTable.Columns.Add("Action Timestamp", typeof (DateTime));
                xlsTable.Columns.Add("User", typeof (string));

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
            XLSExporterHelper(tableExport);

            return true;
        }

        /// <summary>
        ///     Create an audit report with all AuditLog entries for a specified user in a specified date range.
        /// </summary>
        /// <param name="userID">The User ID to track activity for</param>
        /// <param name="startDate"> The start of the specified date range</param>
        /// <param name="endDate">The end of the specified date range</param>
        public bool createReportForUser(int userID, DateTime startDate,
                                        DateTime endDate) {
            // Create Dictionary to exports, each DataTable will correspond to user ID
            var tableExport = new Dictionary<string, DataTable>();
            var xlsExports = new List<DataTable>();

            // check date validity
            bool dateRangeValid = startDate <= endDate;

            if (!dateRangeValid) {
                throw new DateRangeInvalidException();
            }

            // check user validity
            bool userFound = ((from up in _db.UserProfiles
                               where up.UserId == userID
                               select up.UserId).ToList()).Count != 0;

            if (!userFound) {
                throw new UserDoesNotExistException(userID);
            }

            // Create blank list of AuditLogs and fill with all AuditLogs for user ID
            List<AuditLog> requestLogs =
                _db.AuditLogs.Where(r => r.UserID == userID &&
                                         r.AuditDate.Date >= startDate.Date &&
                                         r.AuditDate.Date <= endDate.Date)
                   .OrderBy(ars => ars.AuditDate)
                   .ToList();

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
            xlsTable.Columns.Add("User Action", typeof (string));
            xlsTable.Columns.Add("Request ID", typeof (long));
            xlsTable.Columns.Add("Action Timestamp", typeof (DateTime));

            // Populate table rows
            requestLogs.ForEach(
                a =>
                xlsTable.Rows.Add(
                    Enum.GetName(typeof (Constants.AuditType), a.AuditType),
                    a.RequestID.ToString(), a.AuditDate));

            // add DataTable for this ID to xlsExports
            tableExport.Add(sheetName, xlsTable);
            xlsExports.Add(xlsTable);

            // check if data in report, if not return false
            bool reportData = xlsTable.Rows.Count > 0;

            if (reportData == false) {
                return false;
            }

            // Call XLSExporter with table(s)

            XLSExporterHelper(tableExport);

            return true;
        }

        /// <summary>
        ///     Isolate call to ExcelExportController
        /// </summary>
        /// <param name="exportTable">Dictionary to create in excel</param>
        public void XLSExporterHelper(Dictionary<string, DataTable> exportTable) {
            var markDate = new DateTime(2010, 01, 01, 00, 00, 00, 00);
            TimeSpan dateStamp = DateTime.Now.Subtract(markDate);
            string fromPath = Path.Combine(HttpRuntime.AppDomainAppPath,
                                           "AuditLogTemplate.xlsx");
            string toPath = Path.Combine(HttpRuntime.AppDomainAppPath,
                                         "AuditLogTemplate" +
                                         dateStamp.TotalSeconds.ToString() +
                                         ".xlsx");

            var eeController = new ExcelExportController();

            eeController.exportDataTable(Constants.ReportType.AuditLog,
                                         exportTable, fromPath, toPath);
        }
    }
}