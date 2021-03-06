﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using SasquatchCAIRS.Models.Common;
using SasquatchCAIRS.Models.Service;

namespace SasquatchCAIRS.Controllers.Service {
    /// <summary>
    ///     Provides Request support to the system
    /// </summary>
    public sealed class RequestManagementController {
        /// <summary>The Data Context for this Controller.</summary>
        private CAIRSDataContext _db = new CAIRSDataContext();

        /// <summary>
        ///     Creates a Request entity based off of the RequestContent.
        /// </summary>
        /// <param name="content">Request content holder.</param>
        /// <returns>Request entity based off content.</returns>
        private Request createRequestEntity(RequestContent content) {
            var req = new Request();

            if (content.requestID != -1) {
                req = (from r in _db.Requests
                       where r.RequestID == content.requestID
                       select r).Single();
            }

            req.ParentRequestID = content.parentRequestID;

            req.RequestorFName = content.requestorFirstName;
            req.RequestorLName = content.requestorLastName;
            req.RequestorPhone = content.requestorPhoneNum;
            req.RequestorPhoneExt = content.requestorPhoneExt;
            req.RequestorEmail = content.requestorEmail;

            req.PatientFName = content.patientFName;
            req.PatientLName = content.patientLName;
            req.PatientGender = (byte?) content.patientGender;
            req.PatientAgencyID = content.patientAgencyID;
            req.PatientAge = content.patientAge;

            req.RequestStatus = (byte) content.requestStatus;
            req.TimeOpened = content.timeOpened;
            req.TimeClosed = content.timeClosed;

            req.RegionID = content.regionID;
            req.RequestorTypeID = content.requestorTypeID;

            return req;
        }

        /// <summary>
        ///     Create a QuestionResponse entity based off of the
        ///     QuestionResponseContent.
        /// </summary>
        /// <param name="content">QuestionResponse content holder.</param>
        /// <returns>QuestionResponse entity based off of content.</returns>
        private QuestionResponse createQuestionResponseEntity(
            QuestionResponseContent content) {
            var qr = new QuestionResponse();

            if (content.questionResponseID != -1) {
                // Existing QuestionResponse; has a QuestionResponseID
                qr = (from q in _db.QuestionResponses
                      where
                          q.QuestionResponseID == content.questionResponseID &&
                          q.RequestID == content.requestID
                      select q).Single();
            }

            qr.RequestID = content.requestID;
            qr.Question = content.question;
            qr.Response = content.response;
            qr.TimeSpent = content.timeSpent;
            qr.SpecialNotes = content.specialNotes;
            qr.QuestionTypeID = content.questionTypeID;
            qr.TumourGroupID = content.tumourGroupID;
            qr.Severity = (byte?) content.severity;
            qr.Consequence = (byte?) content.consequence;

            return qr;
        }

        /// <summary>
        ///     Create a Reference entity based off of the ReferenceContent.
        /// </summary>
        /// <param name="content">Reference content holder.</param>
        /// <returns>Reference entity based off of content.</returns>
        private Reference createReferenceEntity(ReferenceContent content) {
            var r = new Reference();

            if (content.referenceID != -1) {
                // Existing Reference; has a ReferenceID
                r = (from rf in _db.References
                     where rf.ReferenceID == content.referenceID &&
                           rf.QuestionResponseID == content.questionResponseID &&
                           rf.RequestID == content.requestID
                     select rf).Single();
            }

            r.RequestID = content.requestID;
            r.QuestionResponseID = content.questionResponseID;
            r.ReferenceType = (byte) content.referenceType;
            r.ReferenceString = content.referenceString;

            return r;
        }

        /// <summary>
        ///     Creates a new Request in the database, with corresponding
        ///     QuestionResponses and References
        /// </summary>
        /// <param name="reqContent">
        ///     RequestContent containing
        ///     QuestionResponseContents, ReferenceContents and Keywords
        /// </param>
        public long create(RequestContent reqContent) {
            using (var trans = new TransactionScope()) {
                // Insert new Request entity
                Request req = createRequestEntity(reqContent);
                _db.Requests.InsertOnSubmit(req);
                _db.SubmitChanges();

                // For each QuestionResponse associated with the request
                foreach (QuestionResponseContent qrContent
                    in reqContent.questionResponseList) {
                    // Set the new RequestID
                    qrContent.requestID = req.RequestID;

                    // Insert new QuestionResponse entity
                    QuestionResponse qr =
                        createQuestionResponseEntity(qrContent);
                    _db.QuestionResponses.InsertOnSubmit(qr);
                    _db.SubmitChanges();

                    // For each Keyword associated with the
                    // QuestionResponse
                    foreach (String kw in qrContent.keywords) {
                        int kwId = getKeywordIdAndActivate(kw);

                        // Check if already associated with Question
                        var kq =
                            (from kqs in _db.KeywordQuestions
                             where kqs.KeywordID == kwId &&
                                   kqs.RequestID ==
                                   req.RequestID &&
                                   kqs.QuestionResponseID ==
                                   qr.QuestionResponseID
                             select kqs)
                                .SingleOrDefault();

                        if (kq != null) {
                            continue;
                        }

                        kq = new KeywordQuestion {
                            KeywordID = kwId,
                            RequestID = req.RequestID,
                            QuestionResponseID = qr.QuestionResponseID
                        };

                        _db.KeywordQuestions.InsertOnSubmit(kq);
                        _db.SubmitChanges();
                    }

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
                        _db.References.InsertOnSubmit(
                            createReferenceEntity(refContent));
                        _db.SubmitChanges();
                    }
                }

                trans.Complete();

                return req.RequestID;
            }
        }

        /// <summary>
        ///     Create a keyword based off a given keyword string.
        /// </summary>
        /// <param name="keywordStr">Keyword string to create.</param>
        /// <returns>
        ///     Keyword ID of the new keyword, or existing keyword
        ///     ID if already exists.
        /// </returns>
        private int getKeywordIdAndActivate(string keywordStr) {
            // Keyword may not exist
            var kw = new Keyword {
                KeywordValue = keywordStr,
                Active = true
            };

            try {
                _db.Keywords.InsertOnSubmit(kw);
                _db.SubmitChanges();
            } catch (Exception) {
                // Keyword already exists
                // Still marked as pending so must delete from DataContext
                _db.Keywords.DeleteOnSubmit(kw);
                _db.SubmitChanges();

                kw = (from kws in _db.Keywords
                      where kws.KeywordValue == keywordStr
                      select kws)
                    .First();

                kw.Active = true;
                _db.SubmitChanges();
            }

            int id = kw.KeywordID;
            kw = null;

            return id;
        }

        /// <summary>
        ///     Retrieves all of the request information and content from the
        ///     database for a given request ID.
        /// </summary>
        /// <param name="requestId">ID of the specified request.</param>
        /// <returns>
        ///     RequestContent contaning QuestionResponseContents,
        ///     ReferenceContents and Keywords.
        /// </returns>
        public RequestContent getRequestDetails(long requestId) {
            IQueryable<Request> requests = (from reqs in _db.Requests
                                            where reqs.RequestID == requestId
                                            select reqs);

            if (requests.Any()) {
                Request reqResult = requests.First();

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

                // Create RequestContent holder
                var reqContent = new RequestContent(reqResult);

                int refCounter = 0;

                // For each QuestionResponse in the database
                foreach (QuestionResponse qr in qrResults) {
                    // Create QuestionResponseContent holder
                    var qrContent =
                        new QuestionResponseContent(qr);
                    reqContent.addQuestionResponse(qrContent);

                    List<Keyword> kwResults =
                        (from kws in _db.Keywords
                         join kqs in _db.KeywordQuestions
                             on kws.KeywordID equals kqs.KeywordID
                         where kqs.RequestID == requestId &&
                               kqs.QuestionResponseID == qr.QuestionResponseID
                         select kws)
                            .ToList();

                    // For each Keyword for the current QuestionResponse
                    foreach (Keyword kw in kwResults) {
                        qrContent.addKeyword(kw.KeywordValue);
                    }

                    // For each Reference for the current QuestionResponse
                    while (refCounter < refResults.Count &&
                           qr.QuestionResponseID ==
                           refResults[refCounter].QuestionResponseID) {
                        // Create ReferenceContent holder
                        var refContent =
                            new ReferenceContent(refResults[refCounter++]);
                        qrContent.addReference(refContent);
                    }
                }

                return reqContent;
            } else {
                return null;
            }
        }

        /// <summary>
        ///     Edit a request properties, question-response pairs and references
        ///     for a given request ID.
        /// </summary>
        /// <param name="reqContent">
        ///     Request content to modify in the database.
        /// </param>
        public void edit(RequestContent reqContent) {
            if (reqContent.requestID == -1) {
                throw new Exception("Invalid request.");
            }

            using (var trans = new TransactionScope()) {
                // Check if the request exists
                int exists =
                    (from reqs in _db.Requests
                     where reqs.RequestID == reqContent.requestID
                     select reqs.RequestID).Count();

                if (exists < 1) {
                    throw new Exception("Request does not exist.");
                }

                // Update the Request entity
                Request req = createRequestEntity(reqContent);
                _db.SubmitChanges();

                // Retrieve the list of current QuestionResponseIDs for
                // the request
                List<long> currQrIds =
                    getQuestionResponseIds(reqContent.requestID);

                // Check all QuestionResponses being updated against
                // those already in the database
                foreach (QuestionResponseContent qrContent in
                    reqContent.questionResponseList) {
                    qrContent.requestID = reqContent.requestID;

                    if (currQrIds.Remove(qrContent.questionResponseID)) {
                        // QuestionResponse already exists in the database
                        // Update QuestionResponse, References and Keywords
                        QuestionResponse qr =
                            createQuestionResponseEntity(qrContent);
                        _db.SubmitChanges();

                        // Retrieve all Keyword IDs for the
                        // given QuestionResponse
                        List<int> currKwIds = getKeywordIds(
                            reqContent.requestID,
                            qrContent.questionResponseID);

                        // Check all Keywords for the QuestionResponse
                        foreach (String kw in
                            qrContent.keywords) {
                            int kwId = getKeywordIdAndActivate(kw);

                            if (!currKwIds.Remove(kwId)) {
                                // Set Keyword for QuestionResponse

                                // Check if already associated with Question
                                var kq =
                                    (from kqs in _db.KeywordQuestions
                                     where kqs.KeywordID == kwId &&
                                           kqs.RequestID ==
                                           req.RequestID &&
                                           kqs.QuestionResponseID ==
                                           qr.QuestionResponseID
                                     select kqs)
                                        .SingleOrDefault();

                                if (kq != null) {
                                    continue;
                                }

                                kq = new KeywordQuestion {
                                    KeywordID = kwId,
                                    RequestID = reqContent.requestID,
                                    QuestionResponseID =
                                        qrContent.questionResponseID
                                };

                                _db.KeywordQuestions.InsertOnSubmit(kq);
                                _db.SubmitChanges();
                            }
                        }

                        // Remaining Keywords not associated anymore
                        foreach (int kwId in currKwIds) {
                            KeywordQuestion kq =
                                getKeywordQuestionEntity(
                                    kwId,
                                    reqContent.requestID,
                                    qrContent.questionResponseID);

                            _db.KeywordQuestions.DeleteOnSubmit(kq);
                            _db.SubmitChanges();
                        }

                        // Retrieve all Reference IDs for the given
                        // QuestionResponse
                        List<long> currRefIds = getReferenceIds(
                            reqContent.requestID,
                            qrContent.questionResponseID);

                        // Check all References for the QuestionResponse
                        foreach (ReferenceContent refContent in
                            qrContent.referenceList) {
                            refContent.requestID = req.RequestID;
                            refContent.questionResponseID =
                                qr.QuestionResponseID;

                            Reference r =
                                createReferenceEntity(refContent);

                            if (refContent.referenceID == -1) {
                                // Insert new Reference into database
                                _db.References.InsertOnSubmit(r);
                            } else {
                                currRefIds.Remove(refContent.referenceID);
                            }

                            _db.SubmitChanges();
                        }

                        // Remaining References do not exist anymore
                        foreach (long refId in currRefIds) {
                            Reference r = getReferenceEntity(
                                refId,
                                reqContent.requestID,
                                qrContent.questionResponseID);

                            _db.References.DeleteOnSubmit(r);
                            _db.SubmitChanges();
                        }
                    } else {
                        // Add new QuestionResponse 
                        QuestionResponse qr =
                            createQuestionResponseEntity(qrContent);

                        _db.QuestionResponses.InsertOnSubmit(qr);
                        _db.SubmitChanges();

                        qrContent.questionResponseID =
                            qr.QuestionResponseID;

                        // Add new Keywords to QuestionResponse
                        foreach (String kw in qrContent.keywords) {
                            int kwId = getKeywordIdAndActivate(kw);

                            // Check if already associated with Question
                            var kq =
                                (from kqs in _db.KeywordQuestions
                                 where kqs.KeywordID == kwId &&
                                       kqs.RequestID ==
                                       req.RequestID &&
                                       kqs.QuestionResponseID ==
                                       qr.QuestionResponseID
                                 select kqs)
                                    .SingleOrDefault();

                            if (kq != null) {
                                continue;
                            }

                            // Set Keyword for QuestionResponse
                            kq = new KeywordQuestion {
                                KeywordID = kwId,
                                RequestID = req.RequestID,
                                QuestionResponseID = qr.QuestionResponseID
                            };

                            _db.KeywordQuestions.InsertOnSubmit(kq);
                            _db.SubmitChanges();
                        }

                        // Add all References for the QuestionResponse
                        foreach (ReferenceContent refContent in
                            qrContent.referenceList) {
                            refContent.requestID = req.RequestID;
                            refContent.questionResponseID =
                                qr.QuestionResponseID;

                            Reference r =
                                createReferenceEntity(refContent);

                            _db.References.InsertOnSubmit(r);
                            _db.SubmitChanges();
                        }
                    }
                }

                // Remove QuestionResponses that no longer exist
                foreach (long qrId in currQrIds) {
                    // Delete all Keyword associations
                    List<int> currKwIds = getKeywordIds(
                        reqContent.requestID, qrId);

                    foreach (int kwId in currKwIds) {
                        KeywordQuestion kq = getKeywordQuestionEntity(
                            kwId, reqContent.requestID, qrId);

                        _db.KeywordQuestions.DeleteOnSubmit(kq);
                        _db.SubmitChanges();
                    }

                    // Delete all References
                    List<long> currRefIds = getReferenceIds(
                        reqContent.requestID, qrId);

                    foreach (long refId in currRefIds) {
                        Reference r = getReferenceEntity(
                            refId, reqContent.requestID, qrId);

                        _db.References.DeleteOnSubmit(r);
                        _db.SubmitChanges();
                    }

                    // Delete QuestionResponse
                    QuestionResponse qr = getQuestionResponseEntity(
                        reqContent.requestID, qrId);

                    _db.QuestionResponses.DeleteOnSubmit(qr);
                    _db.SubmitChanges();
                }

                trans.Complete();
            }
        }

        /// <summary>
        ///     Retrieve the question response IDs associated with a given Request.
        /// </summary>
        /// <param name="reqId">Request ID.</param>
        /// <returns>List of question response IDs.</returns>
        private List<long> getQuestionResponseIds(long reqId) {
            return (from qrs in _db.QuestionResponses
                    where qrs.RequestID == reqId
                    orderby qrs.QuestionResponseID
                    select qrs.QuestionResponseID)
                .ToList();
        }

        /// <summary>
        ///     Retrieve the keyword IDs associated with a given QuestionResponse
        ///     in a specific Request.
        /// </summary>
        /// <param name="reqId">Request ID.</param>
        /// <param name="qrId">QuestionResponse ID.</param>
        /// <returns>List of keyword IDs.</returns>
        private List<int> getKeywordIds(long reqId, long qrId) {
            return (from kws in _db.KeywordQuestions
                    where kws.RequestID == reqId &&
                          kws.QuestionResponseID == qrId
                    select kws.KeywordID)
                .ToList();
        }

        /// <summary>
        ///     Return the reference IDs associated with a given QuestionResponse
        ///     in a specific Request.
        /// </summary>
        /// <param name="reqId">Request ID.</param>
        /// <param name="qrId">QuestionResponse ID.</param>
        /// <returns>List of reference IDs.</returns>
        private List<long> getReferenceIds(long reqId, long qrId) {
            return (from refs in _db.References
                    where refs.RequestID == reqId &&
                          refs.QuestionResponseID == qrId
                    select refs.ReferenceID)
                .ToList();
        }

        /// <summary>
        ///     Returns the QuestionResponse entity for a given QuestionResponse.
        /// </summary>
        /// <param name="reqId">Request ID.</param>
        /// <param name="qrId">QuestionResponse ID.</param>
        /// <returns>
        ///     Specified QuestionResponse entity, or null if does not
        ///     exist.
        /// </returns>
        private QuestionResponse getQuestionResponseEntity(long reqId,
                                                           long qrId) {
            return (from qrs in _db.QuestionResponses
                    where qrs.RequestID == reqId &&
                          qrs.QuestionResponseID == qrId
                    select qrs)
                .SingleOrDefault();
        }

        /// <summary>
        ///     Returns the KeywordQuestion entity for a given Keyword and
        ///     QuestionResponse.
        /// </summary>
        /// <param name="kwId">Keyword ID.</param>
        /// <param name="reqId">Request ID.</param>
        /// <param name="qrId">QuestionResponse ID.</param>
        /// <returns>
        ///     Specified KeywordQuestion entity, or null if does
        ///     not exist.
        /// </returns>
        private KeywordQuestion getKeywordQuestionEntity(int kwId,
                                                         long reqId,
                                                         long qrId) {
            return (from kqs in _db.KeywordQuestions
                    where kqs.KeywordID == kwId &&
                          kqs.RequestID == reqId &&
                          kqs.QuestionResponseID == qrId
                    select kqs)
                .SingleOrDefault();
        }

        /// <summary>
        ///     Returns the Reference entity for a given Reference and
        ///     QuestionResponse.
        /// </summary>
        /// <param name="refId">Reference ID.</param>
        /// <param name="reqId">Request ID.</param>
        /// <param name="qrId">QuestionResponse ID.</param>
        /// <returns>
        ///     Specific Reference entity, or null if does not exist.
        /// </returns>
        private Reference getReferenceEntity(long refId,
                                             long reqId,
                                             long qrId) {
            return (from refs in _db.References
                    where refs.RequestID == reqId &&
                          refs.QuestionResponseID == qrId &&
                          refs.ReferenceID == refId
                    select refs)
                .SingleOrDefault();
        }

        /// <summary>
        ///     Mark a request with the given request ID as invalid in the database.
        /// </summary>
        /// <param name="requestId">Request ID.</param>
        public void invalidate(long requestId) {
            Request req = (from reqs in _db.Requests
                           where reqs.RequestID == requestId
                           select reqs).First();
            req.RequestStatus = (byte) Constants.RequestStatus.Invalid;
            _db.SubmitChanges();
        }

        /// <summary>
        ///     Checks if a request ID exists in the database.
        /// </summary>
        /// <param name="requestId">Unique ID for a request.</param>
        /// <returns>
        ///     True if a request exists with the specified ID,
        ///     false otherwise.
        /// </returns>
        public Boolean requestExists(long requestId) {
            int exists = (from reqs in _db.Requests
                          where reqs.RequestID == requestId
                          select reqs).Count();

            return (exists == 1);
        }
    }
}