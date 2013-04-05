using System;

namespace SasquatchCAIRS.Models.Common {
    /// <summary>
    ///     Request is already locked to someone else.
    /// </summary>
    public class AlreadyLockedException : Exception {
        public AlreadyLockedException(long requestId)
            : base("Request #" + requestId + "is already locked!") {}
    }

    /// <summary>
    ///     The provided user never exists.
    /// </summary>
    public class UserDoesNotExistException : Exception {
        public UserDoesNotExistException(int userID)
            : base("User ID " + userID + "does not exist!") {}
    }

    /// <summary>
    ///     The provided request does not exit.
    /// </summary>
    public class RequestDoesNotExistException : Exception {
        public RequestDoesNotExistException(long requestID)
            : base("User ID " + requestID + "does not exist!") {}
    }

    /// <summary>
    ///     Tst to check if Audit Request fails.
    /// </summary>
    public class AuditRequestsDoNotExistException : Exception {
        public AuditRequestsDoNotExistException()
            : base("No Requests were specified for audit report!") {}
    }

    /// <summary>
    ///     Date range provided is invalis.
    /// </summary>
    public class DateRangeInvalidException : Exception {
        public DateRangeInvalidException()
            : base("Start date specified is after end date!") {}
    }
}