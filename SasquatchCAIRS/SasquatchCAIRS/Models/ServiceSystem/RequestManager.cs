using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models.ServiceSystem {
    public sealed class RequestManager {
        /// <summary>
        /// Read-only RequestManager singleton
        /// </summary>
        private static readonly RequestManager _instance = new RequestManager();
        private CAIRSDataContext db = new CAIRSDataContext();

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
            db.Requests.InsertOnSubmit(req.request);
            db.SubmitChanges();

            foreach (QuestionResponseContent qr in req.questionResponseList) {
                qr.requestID = req.requestID;

                db.QuestionResponses.InsertOnSubmit(qr.questionResponse);
                db.SubmitChanges();

                foreach (ReferenceContent r in qr.referenceList) {
                    r.requestID = req.requestID;
                    r.questionResponseID = qr.questionResponseID;

                    db.References.InsertOnSubmit(r.reference);
                    db.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Retrieves all of the request information and content from the 
        /// database for a given request ID.
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public RequestContent view(long requestID) {
            SasquatchCAIRS.Request reqResult =
                (from reqs in db.Requests
                 where reqs.RequestID == requestID
                 select reqs)
                 .First();

            List<SasquatchCAIRS.QuestionResponse> qrResults =
                (from qrs in db.QuestionResponses
                 where qrs.RequestID == requestID
                 orderby qrs.QuestionResponseID
                 select qrs)
                 .ToList();

            List<SasquatchCAIRS.Reference> refResults =
                (from refs in db.References
                 where refs.RequestID == requestID
                 orderby refs.QuestionResponseID
                 select refs)
                 .ToList();

            RequestContent req = new RequestContent(reqResult);

            int refCounter = 0;
            foreach (QuestionResponse qr in qrResults) {
                QuestionResponseContent newQR =
                    new QuestionResponseContent(qr);
                req.addQuestionResponse(newQR);

                while (refCounter < refResults.Count && 
                    qr.QuestionResponseID == 
                    refResults[refCounter].QuestionResponseID) {
                    
                    ReferenceContent newRef =
                        new ReferenceContent(refResults[refCounter++]);
                    newQR.addReference(newRef);
                }
            }

            return req;
        }

        public void edit(RequestContent reqContent) {
            Request req = (from reqs in db.Requests
                           where reqs.RequestID == reqContent.requestID
                           select reqs).First();

        }

        /// <summary>
        /// Mark a request with the given request ID as invalid in the database.
        /// </summary>
        /// <param name="requestID"></param>
        public void invalidate(long requestID) {
            Request req = (from reqs in db.Requests
                           where reqs.RequestID == requestID
                           select reqs).First();
            req.RequestStatus = 2;
            db.SubmitChanges();
        }
    }
}