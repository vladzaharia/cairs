using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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
        public int? regionID { get; set; }

        [Display(Name = "Requestor Type")]
        public int? requestorTypeID { get; set; }

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
        public QuestionResponseContent(QuestionResponse qr) {
            referenceList = new List<ReferenceContent>();
            keywords = new List<String>();
            requestID = qr.RequestID;
            questionResponseID = qr.QuestionResponseID;

            question = qr.Question;
            response = qr.Response;
            timeSpent = qr.TimeSpent;
            specialNotes = qr.SpecialNotes;
            questionTypeID = qr.QuestionTypeID;
            tumourGroupID = qr.TumourGroupID;

            severity = (Constants.Severity?) qr.Severity;
            consequence = (Constants.Consequence?) qr.Consequence;
        }

        public QuestionResponseContent() {
            consequence = null;
            severity = null;
            referenceList = new List<ReferenceContent>();
            keywords = new List<String>();
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
        [AllowHtml]
        [StringLength(1024)]
        public string question { get; set; }

        [Display(Name = "Response")]
        [AllowHtml]
        public string response { get; set; }

        [Display(Name = "Time Spent")]
        [Range(0, 32767)]
        public short? timeSpent { get; set; }

        [Display(Name = "Special Notes")]
        [AllowHtml]
        [StringLength(1024)]
        public string specialNotes { get; set; }

        [Display(Name = "Question Type")]
        public int? questionTypeID { get; set; }

        [Display(Name = "Tumour Group")]
        public int? tumourGroupID { get; set; }

        [Display(Name = "Severity")]
        public Constants.Severity? severity { get; set; }

        [Display(Name = "Probability of Consequence")]
        public Constants.Consequence? consequence { get; set; }

        [Display(Name = "References")]
        public List<ReferenceContent> referenceList { get; set; }

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
        public List<String> keywords { get; set; }

        public void addKeyword(String newKeyword) {
            keywords.Add(newKeyword);
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

            requestID = -1;
            questionResponseID = -1;
            referenceID = -1;
        }

        public long requestID { get; set; }

        public long questionResponseID { get; set; }

        public long referenceID { get; set; }

        public Constants.ReferenceType referenceType { get; set; }

        public string referenceString { get; set; }
    }
}