using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SasquatchCAIRS.Models.Common;

namespace SasquatchCAIRS.Models.Service {
    /// <summary>
    ///     A wrapper for Requests
    /// </summary>
    public class RequestContent {
        private List<QuestionResponseContent> _questionResponseList
            = new List<QuestionResponseContent>();

        /// <summary>
        ///     Create a new Wrapper for a Request
        /// </summary>
        /// <param name="req">The Request to Create a Wrapper For</param>
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

        /// <summary>
        ///     Create a blank Request Content
        /// </summary>
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

        /// <summary>
        ///     The Request ID
        /// </summary>
        [Display(Name = "Request ID")]
        public long requestID { get; set; }

        /// <summary>
        ///     The Request ID of the Parent
        /// </summary>
        [Display(Name = "Parent Request ID")]
        public long? parentRequestID { get; set; }

        /// <summary>
        ///     First name of Requestor
        /// </summary>
        [Display(Name = "First Name"), StringLength(64)]
        public string requestorFirstName { get; set; }

        /// <summary>
        ///     Last Name of Requestor
        /// </summary>
        [Display(Name = "Last Name"), StringLength(64)]
        public string requestorLastName { get; set; }

        /// <summary>
        ///     Requestor's Phone Number
        /// </summary>
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string requestorPhoneNum { get; set; }

        /// <summary>
        ///     Requestor's Phone Extension
        /// </summary>
        [Display(Name = "Phone Ext"), StringLength(15)]
        public string requestorPhoneExt { get; set; }

        /// <summary>
        ///     Requestor's Email Address
        /// </summary>
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string requestorEmail { get; set; }

        /// <summary>
        ///     Patient's First Name
        /// </summary>
        [Display(Name = "First Name"), StringLength(64)]
        public string patientFName { get; set; }

        /// <summary>
        ///     Patient's Last Name
        /// </summary>
        [Display(Name = "Last Name"), StringLength(64)]
        public string patientLName { get; set; }

        /// <summary>
        ///     Patient Gender
        /// </summary>
        [Display(Name = "Gender")]
        public Constants.Gender? patientGender { get; set; }

        /// <summary>
        ///     Patient's Agency ID
        /// </summary>
        [Display(Name = "Agency ID")]
        [StringLength(15)]
        public string patientAgencyID { get; set; }

        /// <summary>
        ///     Patient's Age
        /// </summary>
        [Display(Name = "Patient Age")]
        [Range(0, 255)]
        public byte? patientAge { get; set; }

        /// <summary>
        ///     Request Status
        /// </summary>
        [Display(Name = "Status")]
        public Constants.RequestStatus requestStatus { get; set; }

        /// <summary>
        ///     Time Opened
        /// </summary>
        [Display(Name = "Time Opened")]
        [DataType(DataType.DateTime)]
        public DateTime timeOpened { get; set; }

        /// <summary>
        ///     Time Closed
        /// </summary>
        [Display(Name = "Time Closed")]
        [DataType(DataType.DateTime)]
        public DateTime? timeClosed { get; set; }

        /// <summary>
        ///     ID of the Region
        /// </summary>
        [Display(Name = "Region")]
        public int? regionID { get; set; }

        /// <summary>
        ///     ID of the Requestor Type
        /// </summary>
        [Display(Name = "Requestor Type")]
        public int? requestorTypeID { get; set; }

        /// <summary>
        ///     List of QuestionResponseContent
        /// </summary>
        public List<QuestionResponseContent> questionResponseList {
            get { return _questionResponseList; }
        }

        /// <summary>
        ///     Add a QuestionResponseContent to the List
        /// </summary>
        /// <param name="newQr">The new QuestionResponse</param>
        public void addQuestionResponse(QuestionResponseContent newQr) {
            _questionResponseList.Add(newQr);
        }
    }

    /// <summary>
    ///     A wrapper for QuestionResponse
    /// </summary>
    public class QuestionResponseContent {
        /// <summary>
        ///     Create a new QuestionResponse wrapper around a QuestionResponse
        /// </summary>
        /// <param name="qr">The QuestionResponse</param>
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

        /// <summary>
        ///     Create new blank wrapper
        /// </summary>
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

        /// <summary>
        ///     The ID of the Request
        /// </summary>
        public long requestID { get; set; }

        /// <summary>
        ///     The ID of the QuestionResponse
        /// </summary>
        public long questionResponseID { get; set; }

        /// <summary>
        ///     The Question
        /// </summary>
        [Display(Name = "Question")]
        [AllowHtml]
        [StringLength(1024)]
        public string question { get; set; }

        /// <summary>
        ///     The Response
        /// </summary>
        [Display(Name = "Response")]
        [AllowHtml]
        public string response { get; set; }

        /// <summary>
        ///     Time Spent answering the Question
        /// </summary>
        [Display(Name = "Time Spent")]
        [Range(0, 32767)]
        public short? timeSpent { get; set; }

        /// <summary>
        ///     Any Special notes for this Question
        /// </summary>
        [Display(Name = "Special Notes")]
        [AllowHtml]
        [StringLength(1024)]
        public string specialNotes { get; set; }

        /// <summary>
        ///     The ID of the QuestionType
        /// </summary>
        [Display(Name = "Question Type")]
        public int? questionTypeID { get; set; }

        /// <summary>
        ///     The ID of the TumourGroup
        /// </summary>
        [Display(Name = "Tumour Group")]
        public int? tumourGroupID { get; set; }

        /// <summary>
        ///     The Severity for the Question
        /// </summary>
        [Display(Name = "Severity")]
        public Constants.Severity? severity { get; set; }

        /// <summary>
        ///     The Probability of Consequence for the Question
        /// </summary>
        [Display(Name = "Probability of Consequence")]
        public Constants.Consequence? consequence { get; set; }

        /// <summary>
        ///     The List of References
        /// </summary>
        [Display(Name = "References")]
        public List<ReferenceContent> referenceList { get; set; }

        /// <summary>
        ///     List of all Keywords
        /// </summary>
        [Display(Name = "Keywords")]
        public List<String> keywords { get; set; }

        /// <summary>
        ///     Add a new Reference to the QR
        /// </summary>
        /// <param name="newRef">The new ReferenceContent to add</param>
        public void addReference(ReferenceContent newRef) {
            referenceList.Add(newRef);
        }

        /// <summary>
        ///     Add a new Keyword to the list
        /// </summary>
        /// <param name="newKeyword"></param>
        public void addKeyword(String newKeyword) {
            keywords.Add(newKeyword);
        }
    }

    /// <summary>
    ///     A wrapper for References
    /// </summary>
    public class ReferenceContent {
        /// <summary>
        ///     Create a new wrapper around a Reference
        /// </summary>
        /// <param name="r">The Reference to wrap around</param>
        public ReferenceContent(Reference r) {
            requestID = r.RequestID;
            questionResponseID = r.QuestionResponseID;
            referenceID = r.ReferenceID;

            referenceType = (Constants.ReferenceType) r.ReferenceType;
            referenceString = r.ReferenceString;
        }

        /// <summary>
        ///     Create a new blank wrapper
        /// </summary>
        public ReferenceContent() {
            referenceString = "";
            referenceType = Constants.ReferenceType.Text;

            requestID = -1;
            questionResponseID = -1;
            referenceID = -1;
        }

        /// <summary>
        ///     The Request ID
        /// </summary>
        public long requestID { get; set; }

        /// <summary>
        ///     The QuestionResponse ID
        /// </summary>
        public long questionResponseID { get; set; }

        /// <summary>
        ///     The Reference ID
        /// </summary>
        public long referenceID { get; set; }

        /// <summary>
        ///     The Type of Reference
        /// </summary>
        public Constants.ReferenceType referenceType { get; set; }

        /// <summary>
        ///     The Content of the Reference
        /// </summary>
        public string referenceString { get; set; }
    }
}