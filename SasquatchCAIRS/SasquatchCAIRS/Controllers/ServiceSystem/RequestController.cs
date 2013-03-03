using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers.ServiceSystem {
    public sealed class RequestController {
        private static readonly RequestController _instance =
            new RequestController();
        private CAIRSDataContext db = new CAIRSDataContext();

        private RequestController() {
        }

        public static RequestController instance {
            get {
                return _instance;
            }
        }

        /// <summary>
        /// Creates a Request entity based off of the RequestContent.
        /// </summary>
        /// <param name="content">Request content holder.</param>
        /// <returns></returns>
        private Request createRequestEntity(RequestContent content) {
            Request req = new Request();

            if (content.requestID != -1) {
                // Existing Request; has a RequestID
                req.RequestID = content.requestID;
            }

            req.ParentRequestID = content.parentRequestID;

            req.RequestorFName = content.requestorFirstName;
            req.RequestorLName = content.requestorLastName;
            req.RequestorPhone = content.requestorPhoneNum;
            req.RequestorPhoneExt = content.requestorPhoneExt;
            req.RequestorEmail = content.requestorEmail;

            req.PatientFName = content.patientFName;
            req.PatientLName = content.patientLName;

            if (content.patientGender != Constants.Gender.None) {
                req.PatientGender = (byte) content.patientGender;
            } else {
                req.PatientGender = null;
            }

            req.PatientAgencyID = content.patientAgencyID;
            req.PatientAge = content.patientAge;

            req.RequestStatus = (byte) content.requestStatus;
            req.TimeOpened = content.timeOpened;
            req.TimeClosed = content.timeClosed;

            if (content.priority != Constants.Priority.None) {
                req.Priority = (byte) content.priority;
            } else {
                req.Priority = null;
            }

            if (content.consequence != Constants.Consequence.None) {
                req.Consequence = (byte) content.consequence;
            } else {
                req.Consequence = null;
            }

            req.RegionID = content.regionID;
            req.RequestorTypeID = content.requestorTypeID;

            return req;
        }

        /// <summary>
        /// Create a QuestionResponse entity based off of the 
        /// QuestionResponseContent.
        /// </summary>
        /// <param name="content">QuestionResponse content holder.</param>
        /// <returns></returns>
        private QuestionResponse createQuestionResponseEntity(
            QuestionResponseContent content) {

            QuestionResponse qr = new QuestionResponse();
            qr.RequestID = content.requestID;

            if (content.questionResponseID != -1) {
                // Existing QuestionResponse; has a QuestionResponseID
                qr.QuestionResponseID = content.questionResponseID;
            }

            qr.Question = content.question;
            qr.Response = content.response;
            qr.TimeSpent = content.timeSpent;
            qr.SpecialNotes = content.specialNotes;
            qr.QuestionTypeID = content.questionTypeID;
            qr.TumourGroupID = content.tumourGroupID;

            return qr;
        }

        private Reference createReferenceEntity(ReferenceContent content) {
            Reference r = new Reference();

            r.RequestID = content.requestID;
            r.QuestionResponseID = content.questionResponseID;

            if (content.referenceID != -1) {
                // Existing Reference; has a ReferenceID
                r.ReferenceID = content.referenceID;
            }

            r.ReferenceType = (byte) content.referenceType;
            r.ReferenceString = content.referenceString;

            return r;
        }

        /// <summary>
        /// Creates a new Request in the database, with corresponding
        /// QuestionResponses and References
        /// </summary>
        /// <param name="reqContent"></param>
        public void create(RequestContent reqContent) {
            try {
                using (TransactionScope trans = new TransactionScope()) {
                    // Insert new Request entity
                    Request req = createRequestEntity(reqContent);
                    db.Requests.InsertOnSubmit(req);
                    db.SubmitChanges();

                    // For each QuestionResponse associated with the request
                    foreach (QuestionResponseContent qrContent
                        in reqContent.questionResponseList) {

                        // Set the new RequestID
                        qrContent.requestID = req.RequestID;

                        // Insert new QuestionResponse entity
                        QuestionResponse qr =
                            createQuestionResponseEntity(qrContent);
                        db.QuestionResponses.InsertOnSubmit(qr);
                        db.SubmitChanges();

                        // For each Reference associated with the 
                        // QuestionResponse
                        foreach (ReferenceContent refContent
                            in qrContent.referenceList) {

                            // Set the new RequestID and
                            // QuestionResponseID
                            refContent.requestID = req.RequestID;
                            refContent.questionResponseID =
                                qr.QuestionResponseID;

                            // Insert new Reference entity
                            db.References.InsertOnSubmit(
                                createReferenceEntity(refContent));
                            db.SubmitChanges();
                        }
                    }

                    trans.Complete();
                }
            } catch (Exception ex) {
                // Do something
            }
        }

        /// <summary>
        /// Retrieves all of the request information and content from the 
        /// database for a given request ID.
        /// </summary>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public RequestContent view(long requestID) {
            Request reqResult =
                (from reqs in db.Requests
                 where reqs.RequestID == requestID
                 select reqs)
                 .First();

            List<QuestionResponse> qrResults =
                (from qrs in db.QuestionResponses
                 where qrs.RequestID == requestID
                 orderby qrs.QuestionResponseID
                 select qrs)
                 .ToList();

            List<Reference> refResults =
                (from refs in db.References
                 where refs.RequestID == requestID
                 orderby refs.QuestionResponseID
                 select refs)
                 .ToList();

            // Create RequestContent holder
            RequestContent reqContent = new RequestContent(reqResult);

            int refCounter = 0;

            // For each QuestionResponse in the database
            foreach (QuestionResponse qr in qrResults) {

                // Create QuestionResponseContent holder
                QuestionResponseContent qrContent =
                    new QuestionResponseContent(qr);
                reqContent.addQuestionResponse(qrContent);

                // For each Response for the current QuestionResponse
                while (refCounter < refResults.Count &&
                    qr.QuestionResponseID ==
                        refResults[refCounter].QuestionResponseID) {

                    // Create ReferenceContent holder
                    ReferenceContent refContent =
                        new ReferenceContent(refResults[refCounter++]);
                    qrContent.addReference(refContent);
                }
            }

            return reqContent;
        }

        /// <summary>
        /// Modify a request properties, question-response pairs and references
        /// for a given request ID.
        /// </summary>
        /// <param name="reqContent">
        /// Request content to modify in the database.
        /// </param>
        public void edit(RequestContent reqContent) {
            try {
                using (TransactionScope trans = new TransactionScope()) {
                    // Check if the request exists
                    int exists =
                        (from reqs in db.Requests
                         where reqs.RequestID == reqContent.requestID
                         select reqs.RequestID).Count();

                    if (exists != 1 || reqContent.requestID == null) {
                        throw new Exception("Request does not exist.");
                    }

                    // Update the Request entity
                    Request req = createRequestEntity(reqContent);
                    db.SubmitChanges();

                    // Get the list of QuestionResponseIDs in the database
                    // for the given RequestIDs
                    List<long> qrResults =
                        (from qrs in db.QuestionResponses
                         where qrs.RequestID == reqContent.requestID
                         orderby qrs.QuestionResponseID
                         select qrs.QuestionResponseID)
                         .ToList();

                    // Sort the list of QuestionResponseContents to update
                    List<QuestionResponseContent> qrList =
                        reqContent.questionResponseList;
                    qrList.Sort();

                    // Check all QuestionResponses being updated against
                    // those already in the database
                    foreach (QuestionResponseContent qrContent in qrList) {
                        long qrID = qrContent.questionResponseID;

                        if (qrResults.Remove(qrID)) {
                            // QuestionResponse exists in database
                            // Update QuestionResponse and References
                            QuestionResponse qr =
                                createQuestionResponseEntity(qrContent);
                            db.SubmitChanges();
                        } else {
                            // Insert new QuestionResponse
                            QuestionResponse qr =
                                createQuestionResponseEntity(qrContent);
                            db.QuestionResponses.InsertOnSubmit(qr);
                            db.SubmitChanges();

                            // Insert new References
                            foreach (ReferenceContent refContent
                                in qrContent.referenceList) {

                                // Set new QuestionResponseID
                                refContent.questionResponseID = 
                                    qr.QuestionResponseID;

                                Reference r = 
                                    createReferenceEntity(refContent);
                                db.References.InsertOnSubmit(r);
                                db.SubmitChanges();
                            }
                        }
                    }

                    // For each QuestionResponse and associated References
                    // that do not exist anymore
                    foreach (long qrID in qrResults) {
                        // Must delete References before QuestionResponse
                        List<Reference> refList =
                            (from refs in db.References
                             where refs.RequestID == req.RequestID &&
                                   refs.QuestionResponseID == qrID
                             select refs)
                             .ToList();

                        db.References.DeleteAllOnSubmit(refList);
                        db.SubmitChanges();

                        QuestionResponse qr =
                            (from qrs in db.QuestionResponses
                             where qrs.RequestID == req.RequestID &&
                                   qrs.QuestionResponseID == qrID
                             select qrs)
                             .First();

                        db.QuestionResponses.DeleteOnSubmit(qr);
                        db.SubmitChanges();
                    }

                    trans.Complete();
                }
            } catch (Exception ex) {
                // Do something
            }
        }

        /// <summary>
        /// Mark a request with the given request ID as invalid in the database.
        /// </summary>
        /// <param name="requestID"></param>
        public void invalidate(long requestID) {
            try {
                using (TransactionScope trans = new TransactionScope()) {
                    Request req = (from reqs in db.Requests
                                   where reqs.RequestID == requestID
                                   select reqs).First();
                    req.RequestStatus = (byte) Constants.RequestStatus.Invalid;
                    db.SubmitChanges();

                    trans.Complete();
                }
            } catch (Exception ex) {
                // Do something
            }
        }
    }
}