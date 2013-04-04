using System;
using System.Linq;
using System.Web.Mvc;
using SasquatchCAIRS.Models.Common;

namespace SasquatchCAIRS.Controllers.Security {
    /// <summary>
    ///     Manages the Locks on Requests.
    /// </summary>
    public class RequestLockManagementController : Controller {
        /// <summary>Data Context for this Controller.</summary>
        private CAIRSDataContext _db = new CAIRSDataContext();

        /// <summary>
        ///     Add lock to existing request.
        /// </summary>
        /// <param name="requestId">The ID of the Request</param>
        /// <param name="userId">The ID of the User</param>
        /// <exception cref="AlreadyLockedException">Exception if request is already locked</exception>
        public void addLock(long requestId, int userId) {
            if (isLocked(requestId)) {
                throw new AlreadyLockedException(requestId);
            }

            // Create a new lock, using the Request ID and User ID provided.
            _db.RequestLocks.InsertOnSubmit(new RequestLock {
                RequestID = requestId,
                UserID = userId,
                StartTime = DateTime.Now
            });

            // Submit to DB.
            _db.SubmitChanges();
        }

        /// <summary>
        ///     Remove lock from existing request.
        /// </summary>
        /// <param name="requestId">The ID of the Request</param>
        public void removeLock(long requestId) {
            // Remove the lock from the DB and submit.
            _db.RequestLocks.DeleteOnSubmit(getRequestLock(requestId));
            _db.SubmitChanges();
        }

        /// <summary>
        ///     Checks if the request is locked
        /// </summary>
        /// <param name="requestId">The ID of the Request</param>
        /// <returns>True if locked, False otherwise</returns>
        public bool isLocked(long requestId) {
            // Try getting a lock, and return whether you got it or not.
            RequestLock requestLock = getRequestLock(requestId);
            return requestLock != null;
        }

        /// <summary>
        ///     Gets the request lock from the database.
        /// </summary>
        /// <param name="requestId">The ID of the Request</param>
        /// <returns>The RequestLock if it exists, null otherwise</returns>
        public RequestLock getRequestLock(long requestId) {
            return _db.RequestLocks.FirstOrDefault(rl =>
                                                   rl.RequestID == requestId);
        }
    }
}