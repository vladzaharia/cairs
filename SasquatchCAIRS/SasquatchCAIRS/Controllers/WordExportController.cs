using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SasquatchCAIRS.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

namespace SasquatchCAIRS.Controllers {
    /// <summary>
    /// Deals with exporting a given request to a .docx file
    /// </summary>
    public class WordExportController : Controller {
        private CAIRSDataContext _db = new CAIRSDataContext();

        /// <summary>
        /// Given a Request, returns a list of strings that represent all of the Requests data
        /// </summary>
        /// <param name="request">The Request to decompose into a list of strings</param>
        /// <returns>List of strings representing the Request</returns>
        public IEnumerable<string> requestToStrings(Request request) {
            List<string> list = new List<string>();

            // ----- Request Information -----
            list.Add(Constants.Export.REQUEST_INFORMATION);

            // Request ID
            list.Add(Constants.Export.REQUEST_ID + request.RequestID);

            // Created By
            AuditLog auditLog = (from al in _db.AuditLogs
                                 where
                                     (int) al.AuditType ==
                                     (int) Constants.AuditType.RequestCreation &&
                                     al.RequestID == request.RequestID
                                 select al).FirstOrDefault();
            if (auditLog != null && auditLog.UserProfile != null) {
                list.Add(Constants.Export.REQUEST_CREATED_BY +
                         auditLog.UserProfile.UserName);
            }

            // Closed By
            auditLog = (from al in _db.AuditLogs
                                        where
                                            (int) al.AuditType ==
                                            (int) Constants.AuditType.RequestCompletion &&
                                            al.RequestID == request.RequestID
                                        select al).FirstOrDefault();
            if (auditLog != null && auditLog.UserProfile != null) {
                list.Add(Constants.Export.REQUEST_CLOSED_BY + auditLog.UserProfile.UserName);
            }

            // Time Opened
            list.Add(Constants.Export.REQUEST_START_TIME + request.TimeOpened);

            // Time Closed
            if (request.TimeClosed != null) {
                list.Add(Constants.Export.REQUEST_COMPLETION_TIME + request.TimeClosed);
            }
            // Time Spent placeholder
            list.Add(Constants.Export.REQUEST_TIME_SPENT);

            // Empty line for readability
            list.Add(" ");

            // ----- Requestor Information -----
            list.Add(Constants.Export.REQUESTOR_INFORMATION);
            Boolean requestorInfo = false;

            // Requestor Name
            if (!String.IsNullOrEmpty(request.RequestorFName +
                                      request.RequestorLName)) {
                list.Add(Constants.Export.REQUESTOR_NAME + request.RequestorFName + " " +
                         request.RequestorLName);
                requestorInfo = true;
            }

            // Requestor Email
            if (!String.IsNullOrEmpty(request.RequestorEmail)) {
                list.Add(Constants.Export.REQUESTOR_EMAIL + request.RequestorEmail);
                requestorInfo = true;
            }

            // Requestor Phone
            if (!String.IsNullOrEmpty(request.RequestorPhone)) {
                string phone = request.RequestorPhone;
                if (!String.IsNullOrEmpty(request.RequestorPhoneExt)) {
                    phone += Constants.Export.REQUESTOR_PHONE_EXT + request.RequestorPhoneExt;
                }
                list.Add(Constants.Export.REQUESTOR_PHONE + phone);
                requestorInfo = true;
            }

            // Requestor Type
            if (request.RequestorType != null) {
                list.Add(Constants.Export.REQUESTOR_TYPE + request.RequestorType.Value);
                requestorInfo = true;
            }

            // Requestor Region
            if (request.Region != null) {
                list.Add(Constants.Export.REQUSTOR_REGION + request.Region.Value);
                requestorInfo = true;
            }

            if (!requestorInfo) {
                list.Add("N/A");
            }

            // Empty line for readability
            list.Add(" ");

            // ----- Patient Information -----
            list.Add(Constants.Export.PATIENT_INFORMATION);
            Boolean patientInfo = false;

            // Patient Name
            if (request.PatientFName != null || request.PatientLName != null) {
                list.Add(Constants.Export.PATIENT_NAME +
                    request.PatientFName + " " + request.PatientLName);
            }

            // Patient Agency ID
            if (!String.IsNullOrEmpty(request.PatientAgencyID)) {
                list.Add(Constants.Export.PATIENT_ID + request.PatientAgencyID);
                patientInfo = true;
            }

            // Patient Age
            if (request.PatientAge != null) {
                list.Add(Constants.Export.PATIENT_AGE + request.PatientAge);
                patientInfo = true;
            }

            // Patient Gender
            if (request.PatientGender != null) {
                list.Add(Constants.Export.PATIENT_GENDER + request.PatientGender);
                patientInfo = true;
            }

            if (!patientInfo) {
                list.Add("N/A");
            }

            // Empty line for readability
            list.Add(" ");
            // ----- Question/Response Information -----
            list.Add(Constants.Export.QUESTION_INFORATION);

            var qrs = from qr in _db.QuestionResponses
                      where qr.RequestID == request.RequestID
                      select qr;

            // Time Spent Accumulator
            int timeSpent = 0;

            // Request Counter
            int counter = 1;
            foreach (QuestionResponse questionResponse in qrs) {
                // Question Number & Question Content
                list.Add(Constants.Export.QUESTION_NUMBER + (counter++) + ":<br>"
                    + HttpUtility.HtmlDecode(questionResponse.Question));

                // Question Response
                if (!String.IsNullOrEmpty(questionResponse.Response)) {
                    list.Add(Constants.Export.QUESTION_RESPONSE + "<br>" +
                        HttpUtility.HtmlDecode(questionResponse.Response));
                }

                // Question Special Notes
                if (!String.IsNullOrEmpty(questionResponse.SpecialNotes)) {
                    list.Add(Constants.Export.QUESTION_SPECIAL_NOTES + "<br>" +
                        HttpUtility.HtmlDecode(questionResponse.SpecialNotes));
                }

                // Question Type
                if (questionResponse.QuestionType != null) {
                    list.Add(Constants.Export.QUESTION_TYPE +
                        questionResponse.QuestionType.Value);
                }

                // Question Tumor Group
                if (questionResponse.TumourGroup != null) {
                    list.Add(Constants.Export.QUESTION_TUMOUR_GROUP +
                        questionResponse.TumourGroup.Value);
                }

                // Question Time Spent
                if (questionResponse.TimeSpent != null) {
                    list.Add(Constants.Export.QUESTION_TIME_SPENT +
                        questionResponse.TimeSpent +
                        Constants.Export.TIME_UNITS);
                    timeSpent += (int) questionResponse.TimeSpent;
                }

                // Question Severity
                if (questionResponse.Severity != null) {
                    list.Add(Constants.Export.QUESTION_SEVERITY +
                        questionResponse.Severity);
                }

                // Question Consequence
                if (questionResponse.Consequence != null) {
                    list.Add(Constants.Export.QUESTION_CONSEQUENCE +
                        questionResponse.Consequence);
                }

                // Question Impact Score
                if (questionResponse.Consequence != null &&
                    questionResponse.Severity != null) {
                    string impactScore = Constants.getImpactScore(
                        (Constants.Severity?) questionResponse.Severity,
                        (Constants.Consequence?) questionResponse.Consequence);

                    list.Add(Constants.Export.QUESTION_IMPACT_SCORE +
                        impactScore);
                }

                // Question Keywords
                List<string> keywords =
                    (from kq in _db.KeywordQuestions
                     join k in _db.Keywords on kq.KeywordID equals k.KeywordID
                     where kq.QuestionResponseID ==
                               questionResponse.QuestionResponseID
                     select k.KeywordValue).ToList();

                if (keywords.Count > 0) {
                    string accumulator = string.Join(", ", keywords.ToList());
                    list.Add(Constants.Export.QUESTION_KEYWORDS + accumulator);
                }

                // Question References
                if (questionResponse.References.Any()) {
                    list.Add(Constants.Export.QUESTION_REFERENCES);
                    foreach (Reference r in questionResponse.References) {
                        string refStr;

                        if (r.ReferenceType ==
                            (byte) Constants.ReferenceType.URL) {
                            refStr = "<a href='" + r.ReferenceString + "'>" +
                                     r.ReferenceString + "</a>";
                        } else {
                            refStr = r.ReferenceString;
                        }
                        list.Add(refStr);
                    }
                }

                // Empty line for readability
                list.Add(" ");
            }

            // Total Time Spent on Request
            list.Insert(list.IndexOf(Constants.Export.REQUEST_TIME_SPENT),
                Constants.Export.REQUEST_TIME_SPENT + timeSpent + Constants.Export.TIME_UNITS);
            list.RemoveAt(list.IndexOf(Constants.Export.REQUEST_TIME_SPENT));
            
            list.Add(Constants.Export.REQUEST_PROPERTIES);
            // ----- Properties -----
            if (request.ParentRequestID != null) {
                // Parent Request ID
                list.Add(Constants.Export.REQUEST_PARENT_ID +
                         request.ParentRequestID);
            } else {
                list.Add("N/A");
            }

            return list;
        }


        /// <summary>
        /// Given the input writes out a docx file with the request's data
        /// </summary>
        /// <param name="input">The list of strings to turn into paragraphs</param>
        /// <param name="templatePath">The location of the template on the server</param>
        /// <param name="destinationPath">Temporary path of file to write on server</param>
        /// <param name="requestId">The ID of the request to be exported</param>
        public void generateDocument(IEnumerable<string> input, string templatePath, string destinationPath, long requestId) {
            System.IO.File.Copy(templatePath, destinationPath);

            using (WordprocessingDocument document = WordprocessingDocument.Open(destinationPath, true)) {
                MainDocumentPart mainPart = document.MainDocumentPart;
                int altChunkIdCounter = 1;
                int blockLevelCounter = 1;

                foreach (string s in input) {
                    string altChunkId = String.Format("AltChunkId{0}",
                                                      altChunkIdCounter++);

                    // Import data as html content using Altchunk
                    AlternativeFormatImportPart chunk =
                        mainPart.AddAlternativeFormatImportPart(
                            AlternativeFormatImportPartType.Html, altChunkId);

                    using (Stream chunkStream = chunk.GetStream(
                        FileMode.Create, FileAccess.Write)) {
                        
                        using (StreamWriter stringWriter =
                               new StreamWriter(chunkStream, Encoding.UTF8)) {
                            // Requires Encoding.UTF8 to remove special characters
                            String content;

                            if (Constants.Export.EXPORT_HEADERS.Contains(s)) {
                                content = "<h3>" + s + "</h3>";
                            } else {
                                content = s;
                            }

                            stringWriter.Write("<html><body>" + content + "</html></body>");
;                        }
                    }

                    AltChunk altChunk = new AltChunk {Id = altChunkId};

                    mainPart.Document.Body.InsertAt(altChunk,
                                                    blockLevelCounter++);
                    mainPart.Document.Save();
                }

                document.Close();
            }

            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = Constants.Export.WORD_CONTENT_TYPE;
            response.AddHeader(Constants.Export.CONTENT_DISPOSITION, Constants.Export.REQUEST_ATTACHMENT
                + requestId + Constants.Export.WORD_FILE_EXT);
            response.TransmitFile(destinationPath);
            response.Flush();
            response.End();

            if (System.IO.File.Exists(destinationPath)) {
                System.IO.File.Delete(destinationPath);
            }
        }
    }
}
