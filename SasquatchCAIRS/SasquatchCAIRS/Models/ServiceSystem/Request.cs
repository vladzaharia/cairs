using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models.ServiceSystem {
    public class RequestContent {
        private long _requestID = -1;
        private long? _parentRequestID = null;

        private string _requestorFName = null;
        private string _requestorLName = null;
        private string _requestorPhoneNum = null;
        private string _requestorPhoneExt = null;
        private string _requestorEmail = null;

        private string _patientFName = null;
        private string _patientLName = null;
        private Constants.Gender _patientGender = Constants.Gender.None;
        private string _patientAgencyID = null;
        private byte? _patientAge = null;

        private Constants.RequestStatus _requestStatus =
            Constants.RequestStatus.Open;
        private DateTime _timeOpened = new DateTime();
        private DateTime? _timeClosed = null;

        private Constants.Priority _priority = Constants.Priority.None;
        private Constants.Consequence _consequence =
            Constants.Consequence.None;
        private byte? _regionID = null;
        private byte? _requestorTypeID = null;

        private List<QuestionResponseContent> _questionResponseList
            = new List<QuestionResponseContent>();
        private List<Reference> _referenceList = new List<Reference>();

        public RequestContent(Request req) {
            _requestID = req.RequestID;
            _parentRequestID = req.ParentRequestID;

            _requestorFName = req.RequestorFName;
            _requestorLName = req.RequestorLName;
            _requestorPhoneNum = req.RequestorPhone;
            _requestorPhoneExt = req.RequestorPhoneExt;
            _requestorEmail = req.RequestorEmail;

            _patientFName = req.PatientFName;
            _patientLName = req.PatientLName;

            if (req.PatientGender != null) {
                _patientGender = (Constants.Gender) req.PatientGender;
            }

            _patientAgencyID = req.PatientAgencyID;
            _patientAge = req.PatientAge;

            _requestStatus = (Constants.RequestStatus) req.RequestStatus;
            _timeOpened = req.TimeOpened;
            _timeClosed = req.TimeClosed;

            if (req.Priority != null) {
                _priority = (Constants.Priority) req.Priority;
            }

            if (req.Consequence != null) {
                _consequence = (Constants.Consequence) req.Consequence;
            }

            _regionID = req.RegionID;
            _requestorTypeID = req.RequestorTypeID;
        }

        public RequestContent() {}

        public long requestID {
            get {
                return _requestID;
            }
            set {
                _requestID = value;
            }
        }

        public long? parentRequestID {
            get {
                return _parentRequestID;
            }
            set {
                _parentRequestID = value;
            }
        }

        public string requestorFirstName {
            get {
                return _requestorFName;
            }
            set {
                _requestorFName = value;
            }
        }

        public string requestorLastName {
            get {
                return _requestorLName;
            }
            set {
                _requestorLName = value;
            }
        }

        public string requestorPhoneNum {
            get {
                return _requestorPhoneNum;
            }
            set {
                _requestorPhoneNum = value;
            }
        }

        public string requestorPhoneExt {
            get {
                return _requestorPhoneExt;
            }
            set {
                _requestorPhoneExt = value;
            }
        }

        public string requestorEmail {
            get {
                return _requestorEmail;
            }
            set {
                _requestorEmail = value;
            }
        }

        public string patientFName {
            get {
                return _patientFName;
            }
            set {
                _patientFName = value;
            }
        }

        public string patientLName {
            get {
                return _patientLName;
            }
            set {
                _patientLName = value;
            }
        }

        public Constants.Gender patientGender {
            get {
                return _patientGender;
            }
            set {
                _patientGender = value;
            }
        }

        public string patientAgencyID {
            get {
                return _patientAgencyID;
            }
            set {
                _patientAgencyID = value;
            }
        }

        public byte? patientAge {
            get {
                return _patientAge;
            }
            set {
                _patientAge = value;
            }
        }

        public Constants.RequestStatus requestStatus {
            get {
                return _requestStatus;
            }
            set {
                _requestStatus = value;
            }
        }

        public DateTime timeOpened {
            get {
                return _timeOpened;
            }
            set {
                _timeOpened = value;
            }
        }

        public DateTime? timeClosed {
            get {
                return _timeClosed;
            }
            set {
                _timeClosed = value;
            }
        }

        public Constants.Priority priority {
            get {
                return _priority;
            }
            set {
                _priority = value;
            }
        }

        public Constants.Consequence consequence {
            get {
                return _consequence;
            }
            set {
                _consequence = value;
            }
        }

        public byte? regionID {
            get {
                return _regionID;
            }
            set {
                _regionID = value;
            }
        }

        public byte? requestorTypeID {
            get {
                return _requestorTypeID;
            }
            set {
                _requestorTypeID = value;
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
        private long _requestID = -1;
        private long _questionResponseID = -1;

        private string _question = null;
        private string _response = null;
        private short? _timeSpent = null;
        private string _specialNotes = null;
        private byte? _questionTypeID = null;
        private byte? _tumourGroupID = null;

        private List<ReferenceContent> _referenceList =
            new List<ReferenceContent>();

        public QuestionResponseContent(QuestionResponse qr) {
            _requestID = qr.RequestID;
            _questionResponseID = qr.QuestionResponseID;

            _question = qr.Question;
            _response = qr.Response;
            _timeSpent = qr.TimeSpent;
            _specialNotes = qr.SpecialNotes;
            _questionTypeID = qr.QuestionTypeID;
            _tumourGroupID = qr.TumourGroupID;
        }

        public QuestionResponseContent() {}

        public long requestID {
            get {
                return _requestID;
            }
            set {
                _requestID = value;
            }
        }

        public long questionResponseID {
            get {
                return _questionResponseID;
            }
            set {
                _questionResponseID = value;
            }
        }

        public string question {
            get {
                return _question;
            }
            set {
                _question = value;
            }
        }

        public string response {
            get {
                return _response;
            }
            set {
                _response = value;
            }
        }

        public short? timeSpent {
            get {
                return _timeSpent;
            }
            set {
                _timeSpent = value;
            }
        }

        public string specialNotes {
            get {
                return _specialNotes;
            }
            set {
                _specialNotes = value;
            }
        }

        public byte? questionTypeID {
            get {
                return _questionTypeID;
            }
            set {
                _questionTypeID = value;
            }
        }

        public byte? tumourGroupID {
            get {
                return _tumourGroupID;
            }
            set {
                _tumourGroupID = value;
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
        private long _requestID;
        private long _questionResponseID;
        private long _referenceID;

        private Constants.ReferenceType _referenceType =
            Constants.ReferenceType.Text;
        private string _referenceString = "";

        public ReferenceContent(Reference r) {
            _requestID = r.RequestID;
            _questionResponseID = r.QuestionResponseID;
            _referenceID = r.ReferenceID;

            _referenceType = (Constants.ReferenceType) r.ReferenceType;
            _referenceString = r.ReferenceString;
        }

        public ReferenceContent() {}

        public long requestID {
            get {
                return _requestID;
            }
            set {
                _requestID = value;
            }
        }

        public long questionResponseID {
            get {
                return _questionResponseID;
            }
            set {
                _questionResponseID = value;
            }
        }

        public long referenceID {
            get {
                return _referenceID;
            }
            set {
                _referenceID = value;
            }
        }

        public Constants.ReferenceType referenceType {
            get {
                return _referenceType;
            }
            set {
                _referenceType = value;
            }
        }

        public string referenceString {
            get {
                return _referenceString;
            }
            set {
                _referenceString = value;
            }
        }
    }
}