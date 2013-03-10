using System.Collections.Generic;
using System.Linq;

namespace SasquatchCAIRS.Models.ServiceSystem {
    public sealed class RequestManager {
        /// <summary>
        /// Read-only RequestManager singleton
        /// </summary>
        private static readonly RequestManager _instance = new RequestManager();
        private CAIRSDataContext _db = new CAIRSDataContext();

        private RequestManager() {}

        public static RequestManager instance {
            get {
                return _instance;
            }
        }

        /// <summary>
        /// Creates a new Request in the database, with corresponding
        /// QuestionResponses and References
        /// </summary>
        /// <param name="req"></param>
        public void create(RequestContent req) {
            _db.Requests.InsertOnSubmit(req.request);
            _db.SubmitChanges();

            foreach (QuestionResponseContent qr in req.questionResponseList) {
                qr.requestID = req.requestID;

                _db.QuestionResponses.InsertOnSubmit(qr.questionResponse);
                _db.SubmitChanges();

                foreach (ReferenceContent r in qr.referenceList) {
                    r.requestID = req.requestID;
                    r.questionResponseID = qr.questionResponseID;

                    _db.References.InsertOnSubmit(r.reference);
                    _db.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Retrieves all of the request information and content from the 
        /// database for a given request ID.
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public RequestContent view(long requestId) {
            Request reqResult =
                (from reqs in _db.Requests
                 where reqs.RequestID == requestId
                 select reqs)
                 .First();

            List<QuestionResponse> qrResults =
                (from qrs in _db.QuestionResponses
                 where qrs.RequestID == requestId
                 orderby qrs.QuestionResponseID
                 select qrs)
                 .ToList();

            List<Reference> refResults =
                (from refs in _db.References
                 where refs.RequestID == requestId
                 orderby refs.QuestionResponseID
                 select refs)
                 .ToList();

            RequestContent req = new RequestContent(reqResult);

            int refCounter = 0;
            foreach (QuestionResponse qr in qrResults) {
                QuestionResponseContent newQr =
                    new QuestionResponseContent(qr);
                req.addQuestionResponse(newQr);

                while (refCounter < refResults.Count && 
                    qr.QuestionResponseID == 
                    refResults[refCounter].QuestionResponseID) {
                    
                    ReferenceContent newRef =
                        new ReferenceContent(refResults[refCounter++]);
                    newQr.addReference(newRef);
                }
            }

            return req;
        }

        public void edit(RequestContent reqContent) {
            
        }

        /// <summary>
        /// Mark a request with the given request ID as invalid in the database.
        /// </summary>
        /// <param name="requestId"></param>
        public void invalidate(long requestId) {
            Request req = (from reqs in _db.Requests
                           where reqs.RequestID == requestId
                           select reqs).First();
            req.RequestStatus = 2;
            _db.SubmitChanges();
        }
    }
}