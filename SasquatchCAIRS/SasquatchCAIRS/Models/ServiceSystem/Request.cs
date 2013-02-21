using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models.ServiceSystem {
    public class RequestContent {
        private SasquatchCAIRS.Request _request;
        private List<QuestionResponseContent> _questionResponseList
            = new List<QuestionResponseContent>();
        private List<SasquatchCAIRS.Reference> _referenceList
            = new List<SasquatchCAIRS.Reference>();

        public RequestContent(SasquatchCAIRS.Request req) {
            _request = req;
        }

        public RequestContent() {
            _request = new SasquatchCAIRS.Request();
        }

        public SasquatchCAIRS.Request request {
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

        public System.Nullable<long> parentRequestID {
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
                if (value != null) {
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

        public System.Nullable<byte> patientAge {
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

        public Constants.Priority priority {
            get {
                if (_request.Priority != null) {
                    return (Constants.Priority) _request.Priority;
                } else {
                    return Constants.Priority.None;
                }
            }
            set {
                if (value != null) {
                    _request.Priority = (byte) value;
                } else {
                    _request.Priority = null;
                }
            }
        }

        public Constants.Severity severity {
            get {
                if (_request.Severity != null) {
                    return (Constants.Severity) _request.Severity;
                } else {
                    return Constants.Severity.None;
                }
            }
            set {
                if (value != null) {
                    _request.Severity = (byte) value;
                } else {
                    _request.Severity = null;
                }
            }
        }

        public System.Nullable<byte> regionID {
            get {
                return _request.RegionID;
            }
            set {
                _request.RegionID = value;
            }
        }

        public System.Nullable<byte> requestorTypeID {
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

        public Boolean addQuestionResponse(QuestionResponseContent newQR) {
            if (newQR.requestID != requestID) {
                return false;
            }

            foreach (QuestionResponseContent qr in _questionResponseList) {
                if (qr.questionResponseID == newQR.questionResponseID) {
                    return false;
                }
            }

            _questionResponseList.Add(newQR);
            return true;
        }

        public Boolean removeQuestionResponse(long questionResponseID) {
            foreach (QuestionResponseContent qr in _questionResponseList) {
                if (qr.questionResponseID == questionResponseID) {
                    _questionResponseList.Remove(qr);
                    return true;
                }
            }

            return false;
        }
    }

    public class QuestionResponseContent {
        private SasquatchCAIRS.QuestionResponse _questionResponse;

        private List<ReferenceContent> _referenceList = new List<ReferenceContent>();

        public QuestionResponseContent(SasquatchCAIRS.QuestionResponse qr) {
            _questionResponse = qr;
        }

        public QuestionResponseContent() {
            _questionResponse = new SasquatchCAIRS.QuestionResponse();
        }

        public SasquatchCAIRS.QuestionResponse questionResponse {
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

        public System.Nullable<short> timeSpent {
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

        public System.Nullable<byte> questionTypeID {
            get {
                return _questionResponse.QuestionTypeID;
            }
            set {
                _questionResponse.QuestionTypeID = value;
            }
        }

        public System.Nullable<byte> tumourGroupID {
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

        public Boolean addReference(ReferenceContent newRef) {
            if (newRef.requestID != requestID ||
                newRef.questionResponseID != questionResponseID) {
                return false;
            }

            foreach (ReferenceContent r in _referenceList) {
                if (r.questionResponseID == newRef.questionResponseID) {
                    return false;
                }
            }

            _referenceList.Add(newRef);
            return true;
        }

        public Boolean removeReference(long referenceID) {
            foreach (ReferenceContent r in _referenceList) {
                if (r.questionResponseID == questionResponseID) {
                    _referenceList.Remove(r);
                    return true;
                }
            }

            return false;
        }
    }
   
    public class ReferenceContent {
        private SasquatchCAIRS.Reference _reference;

        public ReferenceContent(SasquatchCAIRS.Reference qr) {
            _reference = qr;
        }

        public ReferenceContent() {
            _reference = new SasquatchCAIRS.Reference();
        }

        public SasquatchCAIRS.Reference reference {
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

        public int referenceID {
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

/**
    public class Request {
        private long _requestID;
        private long _parentRequestID;

        private string _requestorFirstName;
        private string _requestorLastName;
        private string _requestorPhoneNum;
        private string _requestorPhoneExt;
        private string _requestorEmail;

        private string _patientFirstName;
        private string _patientLastName;
        private Constants.Gender _patientGender;
        private string _patientAgencyID;
        private byte _patientAge;

        private Constants.RequestStatus _requestStatus;
        private DateTime _timeOpened;
        private DateTime _timeCompleted;

        private Constants.Priority _priority;
        private Constants.Severity _severity;

        private byte _regionID;
        private byte _requestorTypeID;

        private List<QuestionResponse> _questionResponseList =
            new List<QuestionResponse>();

        public Request() {}
        
        public Request(int id) {
            _requestID = id;
        }
    
        public long requestID {
            get {
                return _requestID;
            }
            set {
                _requestID = value;
            }
        }

        public long parentRequestID {
            get {
                return _parentRequestID;
            }
            set{
                _parentRequestID = value;
            }
        }

        public string requestorFirstName {
            get {
                return _requestorFirstName;
            }
            set {
                _requestorFirstName = value;
            }
        }

        public string requestorLastName {
            get {
                return _requestorLastName;
            }
            set {
                _requestorLastName = value;
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
            set{
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

        public string patientFirstName {
            get {
                return _patientFirstName;
            }
            set {
                _patientFirstName = value;
            }
        }

        public string patientLastName {
            get {
                return _patientLastName;
            }
            set {
                _patientLastName = value;
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

        public byte patientAge {
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

        public Constants.Priority priority {
            get {
                return _priority;
            }
            set {
                _priority = value;
            }
        }

        public Constants.Severity severity {
            get {
                return _severity;
            }
            set {
                _severity = value;
            }
        }

        public byte regionID {
            get {
                return _regionID;
            }
            set {
                _regionID = value;
            }
        }

        public byte requestorTypeID {
            get {
                return _requestorTypeID;
            }
            set {
                _requestorTypeID = value;
            }
        }

        public List<QuestionResponse> questionResponseList {
            get {
                return _questionResponseList;
            }
        }

        public void addQuestionResponse(QuestionResponse qr) {
            _questionResponseList.Add(qr);
        }
    }

    public class QuestionResponse {
        private long _questionResponseID;

        private string _question;
        private string _response;
        private ushort _timeSpent;
        private string _specialNotes;

        private byte _questionTypeID;
        private byte _tumourGroupID;

        private List<Reference> _refList = new List<Reference>();

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

        public ushort timeSpent {
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

        public byte questionTypeID {
            get {
                return _questionTypeID;
            }
            set {
                _questionTypeID = value;
            }
        }

        public byte tumourGroupID {
            get {
                return _tumourGroupID;
            }
            set {
                _tumourGroupID = value;
            }
        }

        public List<Reference> refList {
            get {
                return refList;
            }
        }

        public void addReference(Reference reference) {
            _refList.Add(reference);
        }
    }

    public class Reference {
        private int _referenceID;

        private byte _referenceType;
        private string _referenceString;

        public int referenceID {
            get {
                return _referenceID;
            }
            set {
                _referenceID = value;
            }
        }

        public byte referenceType {
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
 **/
}