using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models.ServiceSystem {
    public class RequestContent {
        private Request _request;
        private List<QuestionResponseContent> _questionResponseList
            = new List<QuestionResponseContent>();
        private List<Reference> _referenceList = new List<Reference>();

        public RequestContent(Request req) {
            _request = req;
        }

        public RequestContent() {
            _request = new Request();
        }

        public Request request {
            get {
                return _request;
            }
        }

        public long requestID {
            get {
                return _request.RequestID;
            }
            set {
                _request.RequestID = value;
            }
        }

        public long? parentRequestID {
            get {
                return _request.ParentRequestID;
            }
            set {
                _request.ParentRequestID = value;
            }
        }

        public string requestorFirstName {
            get {
                return _request.RequestorFName;
            }
            set {
                _request.RequestorFName = value;
            }
        }

        public string requestorLastName {
            get {
                return _request.RequestorLName;
            }
            set {
                _request.RequestorLName = value;
            }
        }

        public string requestorPhoneNum {
            get {
                return _request.RequestorPhone;
            }
            set {
                _request.RequestorPhone = value;
            }
        }

        public string requestorPhoneExt {
            get {
                return _request.RequestorPhoneExt;
            }
            set {
                _request.RequestorPhoneExt = value;
            }
        }

        public string requestorEmail {
            get {
                return _request.RequestorEmail;
            }
            set {
                _request.RequestorEmail = value;
            }
        }

        public string patientFirstName {
            get {
                return _request.PatientFName;
            }
            set {
                _request.PatientFName = value;
            }
        }

        public string patientLastName {
            get {
                return _request.PatientLName;
            }
            set {
                _request.PatientLName = value;
            }
        }

        public Constants.Gender patientGender {
            get {
                if (_request.PatientGender != null) {
                    return (Constants.Gender) _request.PatientGender;
                } else {
                    return Constants.Gender.None;
                }
            }
            set {
                if (value != Constants.Gender.None) {
                    _request.PatientGender = (byte) value;
                } else {
                    _request.PatientGender = null;
                }
            }
        }

        public string patientAgencyID {
            get {
                return _request.PatientAgencyID;
            }
            set {
                _request.PatientAgencyID = value;
            }
        }

        public byte? patientAge {
            get {
                return _request.PatientAge;
            }
            set {
                _request.PatientAge = value;
            }
        }

        public Constants.RequestStatus requestStatus {
            get {
                return (Constants.RequestStatus) _request.RequestStatus;
            }
            set {
                _request.RequestStatus = (byte) value;
            }
        }

        public DateTime timeOpened {
            get {
                return _request.TimeOpened;
            }
            set {
                _request.TimeOpened = value;
            }
        }

        public DateTime? timeClosed {
            get {
                return _request.TimeClosed;
            }
            set {
                _request.TimeClosed = value;
            }
        }

        public Constants.Priority priority {
            get {
                if (_request.Priority != null) {
                    return (Constants.Priority) _request.Priority;
                } else {
                    return Constants.Priority.None;
                }
            }
            set {
                if (value != Constants.Priority.None) {
                    _request.Priority = (byte) value;
                } else {
                    _request.Priority = null;
                }
            }
        }

        public Constants.Consequence consequence {
            get {
                if (_request.Consequence != null) {
                    return (Constants.Consequence) _request.Consequence;
                } else {
                    return Constants.Consequence.None;
                }
            }
            set {
                if (value != Constants.Consequence.None) {
                    _request.Consequence = (byte) value;
                } else {
                    _request.Consequence = null;
                }
            }
        }

        public byte? regionID {
            get {
                return _request.RegionID;
            }
            set {
                _request.RegionID = value;
            }
        }

        public byte? requestorTypeID {
            get {
                return _request.RequestorTypeID;
            }
            set {
                _request.RequestorTypeID = value;
            }
        }

        public List<QuestionResponseContent> questionResponseList {
            get {
                return _questionResponseList;
            }
        }

        public void addQuestionResponse(QuestionResponseContent newQR) {
            _questionResponseList.Add(newQR);
        }

        public void removeQuestionResponse(long questionResponseID) {
            foreach (QuestionResponseContent qr in _questionResponseList) {
                if (qr.questionResponseID == questionResponseID) {
                    _questionResponseList.Remove(qr);
                }
            }
        }
    }

    public class QuestionResponseContent : IComparable {
        private QuestionResponse _questionResponse;

        private List<ReferenceContent> _referenceList = 
            new List<ReferenceContent>();

        public QuestionResponseContent(QuestionResponse qr) {
            _questionResponse = qr;
        }

        public QuestionResponseContent() {
            _questionResponse = new QuestionResponse();
        }

        public QuestionResponse questionResponse {
            get {
                return _questionResponse;
            }
        }

        public long requestID {
            get {
                return _questionResponse.RequestID;
            }
            set {
                _questionResponse.RequestID = value;
            }
        }

        public long questionResponseID {
            get {
                return _questionResponse.QuestionResponseID;
            }
            set {
                _questionResponse.QuestionResponseID = value;
            }
        }

        public string question {
            get {
                return _questionResponse.Question;
            }
            set {
                _questionResponse.Question = value;
            }
        }

        public string response {
            get {
                return _questionResponse.Response;
            }
            set {
                _questionResponse.Response = value;
            }
        }

        public short? timeSpent {
            get {
                return _questionResponse.TimeSpent;
            }
            set {
                _questionResponse.TimeSpent = value;
            }
        }

        public string specialNotes {
            get {
                return _questionResponse.SpecialNotes;
            }
            set {
                _questionResponse.SpecialNotes = value;
            }
        }

        public byte? questionTypeID {
            get {
                return _questionResponse.QuestionTypeID;
            }
            set {
                _questionResponse.QuestionTypeID = value;
            }
        }

        public byte? tumourGroupID {
            get {
                return _questionResponse.TumourGroupID;
            }
            set {
                _questionResponse.TumourGroupID = value;
            }
        }

        public List<ReferenceContent> referenceList {
            get {
                return _referenceList;
            }
        }

        public void addReference(ReferenceContent newRef) {
            _referenceList.Add(newRef);
        }

        public void removeReference(long referenceID) {
            foreach (ReferenceContent r in _referenceList) {
                if (r.referenceID == referenceID) {
                    _referenceList.Remove(r);
                }
            }
        }

        /// <summary>
        /// Compares the current instance with another QuestionResponseContent
        /// object and returns an integer that indicates whether the current
        /// instance precedes, follows, or occurs in the same position in the
        /// sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// Less than zero if this instance precedes obj in the sort order.
        /// Zero if this instance occurs in the same position in the sort order
        /// as obj.
        /// Greater than zero if this instance follows obj in the sort order.
        /// </returns>
        public int CompareTo(object obj) {
            if (obj == null) {
                return 1;
            }

            QuestionResponseContent otherQRContent = 
                obj as QuestionResponseContent;
            if (otherQRContent != null) {
                if (requestID == otherQRContent.requestID) {
                    return questionResponseID.CompareTo(
                        otherQRContent.questionResponseID);
                } else {
                    return requestID.CompareTo(otherQRContent.requestID);
                }
            } else {
                throw new ArgumentException(
                    "Object is not a QuestionResponseContent");
            }
        }
    }
   
    public class ReferenceContent {
        private Reference _reference;

        public ReferenceContent(Reference qr) {
            _reference = qr;
        }

        public ReferenceContent() {
            _reference = new Reference();
        }

        public Reference reference {
            get {
                return _reference;
            }
        }

        public long requestID {
            get {
                return _reference.RequestID;
            }
            set {
                _reference.RequestID = value;
            }
        }

        public long questionResponseID {
            get {
                return _reference.QuestionResponseID;
            }
            set {
                _reference.QuestionResponseID = value;
            }
        }

        public long referenceID {
            get {
                return reference.ReferenceID;
            }
            set {
                reference.ReferenceID = value;
            }
        }

        public Constants.ReferenceType referenceType {
            get {
                return (Constants.ReferenceType) _reference.ReferenceType;
            }
            set {
                _reference.ReferenceType = (byte) value;
            }
        }

        public string referenceString {
            get {
                return _reference.ReferenceString;
            }
            set {
                _reference.ReferenceString = value;
            }
        }
    }
}