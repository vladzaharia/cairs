using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SasquatchCAIRS.Models.ServiceSystem {
    public class RequestContent {
        private List<QuestionResponseContent> _questionResponseList
            = new List<QuestionResponseContent>();

        public RequestContent(Request req) {
            requestID = req.RequestID;
            parentRequestID = req.ParentRequestID;

            requestorFirstName = req.RequestorFName;
            requestorLastName = req.RequestorLName;
            requestorPhoneNum = req.RequestorPhone;
            requestorPhoneExt = req.RequestorPhoneExt;
            requestorEmail = req.RequestorEmail;

            patientFName = req.PatientFName;
            patientLName = req.PatientLName;

            if (req.PatientGender != null) {
                patientGender = (Constants.Gender) req.PatientGender;
            } else {
                patientGender = null;
            }

            patientAgencyID = req.PatientAgencyID;
            patientAge = req.PatientAge;

            requestStatus = (Constants.RequestStatus) req.RequestStatus;
            timeOpened = req.TimeOpened;
            timeClosed = req.TimeClosed;

            regionID = req.RegionID;
            requestorTypeID = req.RequestorTypeID;
        }

        public RequestContent() {
            requestorTypeID = null;
            regionID = null;
            timeClosed = null;
            timeOpened = new DateTime(1753, 1, 1);
            requestStatus = Constants.RequestStatus.Open;
            patientAge = null;
            patientAgencyID = null;
            patientGender = null;
            patientLName = null;
            patientFName = null;
            requestID = -1;
            parentRequestID = null;
            requestorEmail = null;
            requestorPhoneExt = null;
            requestorPhoneNum = null;
            requestorLastName = null;
            requestorFirstName = null;
        }

        [Display(Name = "Request ID")]
        public long requestID { get; set; }

        [Display(Name = "Parent Request ID")]
        public long? parentRequestID { get; set; }

        [Display(Name = "First Name"), StringLength(64)]
        public string requestorFirstName { get; set; }

        [Display(Name = "Last Name"), StringLength(64)]
        public string requestorLastName { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string requestorPhoneNum { get; set; }

        [Display(Name = "Phone Ext"), StringLength(15)]
        public string requestorPhoneExt { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string requestorEmail { get; set; }

        [Display(Name = "First Name"), StringLength(64)]
        public string patientFName { get; set; }

        [Display(Name = "Last Name"), StringLength(64)]
        public string patientLName { get; set; }

        [Display(Name = "Gender")]
        public Constants.Gender? patientGender { get; set; }

        [Display(Name = "Agency ID")]
        [StringLength(15)]
        public string patientAgencyID { get; set; }

        [Display(Name = "Patient Age")]
        [Range(0, 255)]
        public byte? patientAge { get; set; }

        [Display(Name = "Status")]
        public Constants.RequestStatus requestStatus { get; set; }

        [Display(Name = "Time Opened")]
        [DataType(DataType.DateTime)]
        public DateTime timeOpened { get; set; }

        [Display(Name = "Time Closed")]
        [DataType(DataType.DateTime)]
        public DateTime? timeClosed { get; set; }

        [Display(Name = "Region")]
        public byte? regionID { get; set; }

        [Display(Name = "Requestor Type")]
        public byte? requestorTypeID { get; set; }

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

        public readonly QuestionResponseContent qrContent = null;
    }

    public class QuestionResponseContent : IComparable {
        private Constants.Severity _severity = Constants.Severity.None;
        private Constants.Consequence _consequence =
            Constants.Consequence.None;

        public QuestionResponseContent(QuestionResponse qr) {
            referenceList = new List<ReferenceContent>();
            keywords = new List<KeywordContent>();
            requestID = qr.RequestID;
            questionResponseID = qr.QuestionResponseID;

            question = qr.Question;
            response = qr.Response;
            timeSpent = qr.TimeSpent;
            specialNotes = qr.SpecialNotes;
            questionTypeID = qr.QuestionTypeID;
            tumourGroupID = qr.TumourGroupID;

            if (qr.Severity != null) {
                _severity = (Constants.Severity) qr.Severity;
            }

            if (qr.Consequence != null) {
                _consequence = (Constants.Consequence) qr.Consequence;
            }
        }

        public QuestionResponseContent() {
            referenceList = new List<ReferenceContent>();
            keywords = new List<KeywordContent>();
            tumourGroupID = null;
            questionTypeID = null;
            timeSpent = null;
            specialNotes = null;
            response = null;
            question = null;
            questionResponseID = -1;
            requestID = -1;
        }

        public long requestID { get; set; }

        public long questionResponseID { get; set; }

        [Display(Name = "Question")]
        [StringLength(1024)]
        public string question { get; set; }

        [Display(Name = "Response")]
        public string response { get; set; }

        [Display(Name = "Time Spent")]
        [Range(0, 32767)]
        public short? timeSpent { get; set; }

        [Display(Name = "Special Notes")]
        [StringLength(1024)]
        public string specialNotes { get; set; }

        [Display(Name = "Question Type")]
        public byte? questionTypeID { get; set; }

        [Display(Name = "Tumour Group")]
        public byte? tumourGroupID { get; set; }

        public Constants.Severity severity {
            get {
                return _severity;
            }
            set {
                _severity = value;
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

        [Display(Name = "References")]
        public List<ReferenceContent> referenceList { get; private set; }

        public void addReference(ReferenceContent newRef) {
            referenceList.Add(newRef);
        }

        public void removeReference(long referenceID) {
            foreach (ReferenceContent r in referenceList) {
                if (r.referenceID == referenceID) {
                    referenceList.Remove(r);
                }
            }
        }

        [Display(Name = "Keywords")]
        public List<KeywordContent> keywords { get; private set; }

        public void addKeyword(KeywordContent newKeyword) {
            keywords.Add(newKeyword);
        }

        public void removeKeyword(int keywordId) {
            foreach (KeywordContent kw in keywords) {
                if (kw.keywordId == keywordId) {
                    keywords.Remove(kw);
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
        public ReferenceContent(Reference r) {
            requestID = r.RequestID;
            questionResponseID = r.QuestionResponseID;
            referenceID = r.ReferenceID;

            referenceType = (Constants.ReferenceType) r.ReferenceType;
            referenceString = r.ReferenceString;
        }

        public ReferenceContent() {
            referenceString = "";
            referenceType = Constants.ReferenceType.Text;
        }

        public long requestID { get; set; }

        public long questionResponseID { get; set; }

        public long referenceID { get; set; }

        public Constants.ReferenceType referenceType { get; set; }

        public string referenceString { get; set; }
    }

    public class KeywordContent {
        public KeywordContent(Keyword kw) {
            keywordId = kw.KeywordID;
            keywordStr = kw.KeywordValue;
        }

        public KeywordContent() {
            keywordStr = null;
            keywordId = -1;
        }

        public int keywordId { get; set; }

        public string keywordStr { get; set; }
    }
}