using System;

namespace SasquatchCAIRS.Models {
    public class AlreadyLockedException : Exception {
        public AlreadyLockedException(long requestId)
            : base("Request #" + requestId + "is already locked!") {}
    }

    public class UserDoesNotExistException : Exception {
        public UserDoesNotExistException(int userID)
            : base("User ID " + userID + "does not exist!") {
        }
    }

    public class RequestDoesNotExistException : Exception {
        public RequestDoesNotExistException(long requestID)
            : base("User ID " + requestID + "does not exist!") {
        }
    }

    public class AuditRequestsDoNotExistException : Exception {
        public AuditRequestsDoNotExistException()
            : base("No Requests were specified for audit report!") {
        }
    }
}