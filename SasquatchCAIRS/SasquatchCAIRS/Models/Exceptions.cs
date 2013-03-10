using System;

namespace SasquatchCAIRS.Models {
    public class AlreadyLockedException : Exception {
        public AlreadyLockedException(long requestId)
            : base("Request #" + requestId + "is already locked!") {}
    }
}