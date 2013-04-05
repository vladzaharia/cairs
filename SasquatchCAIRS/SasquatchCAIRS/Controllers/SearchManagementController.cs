using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.SearchSystem;

namespace SasquatchCAIRS.Controllers
{
    public class SearchManagementController : Controller
    {
        //
        // GET: /SearchManagement/
        private DropdownController _dropdownController =
            new DropdownController();
        private CAIRSDataContext _db = new CAIRSDataContext();

        private static List<Request> _results;
        private static int _startIndex;

        /// <summary>
        /// Given a SearchCriteria object, this generates a list of strings used to represent all of the fields being filtered on
        /// </summary>
        /// <param name="sc">The SearchCriteria object used to filter on Requests</param>
        /// <returns>A list of strings which represents the values of the SearchCriteria object</returns>
        public List<string> constructCriteriaString(SearchCriteria sc)
        {
            List<string> result = new List<string>();
            if (!String.IsNullOrEmpty(sc.anyKeywordString))
            {
                result.Add(Constants.UIString.FieldLabel.ANY_KEYWORDS + ": " +
                          sc.anyKeywordString.TrimEnd(" ,".ToCharArray()));
            }
            if (!String.IsNullOrEmpty(sc.allKeywordString))
            {
                result.Add(Constants.UIString.FieldLabel.ALL_KEYWORDS + ": " +
                           sc.allKeywordString.TrimEnd(" ,".ToCharArray()));
            }
            if (!String.IsNullOrEmpty(sc.noneKeywordString))
            {
                result.Add(Constants.UIString.FieldLabel.NONE_KEYWORDS + ": " +
                           sc.noneKeywordString.TrimEnd(" ,".ToCharArray()));
            }
            if (sc.startTime.CompareTo(new DateTime()) != 0)
            {
                result.Add(Constants.UIString.FieldLabel.START_TIME + ": " +
                           sc.startTime.ToShortDateString());
            }
            if (sc.completionTime.CompareTo(new DateTime()) != 0)
            {
                result.Add(Constants.UIString.FieldLabel.COMPLETED_TIME + ": " +
                           sc.completionTime.ToShortDateString());
            }
            if (!String.IsNullOrEmpty(sc.requestStatus))
            {
                result.Add(Constants.UIString.FieldLabel.STATUS + ": " +
                          sc.requestStatus);
            }
            if (!String.IsNullOrEmpty(sc.requestorFirstName))
            {
                result.Add(Constants.UIString.FieldLabel.CALLER_FNAME + ": " +
                          sc.requestorFirstName);
            }
            if (!String.IsNullOrEmpty(sc.requestorLastName))
            {
                result.Add(Constants.UIString.FieldLabel.CALLER_LNAME + ": " +
                          sc.requestorLastName);
            }
            if (!String.IsNullOrEmpty(sc.patientFirstName))
            {
                result.Add(Constants.UIString.FieldLabel.PATIENT_FNAME + ": " +
                          sc.patientFirstName);
            }
            if (!String.IsNullOrEmpty(sc.patientLastName))
            {
                result.Add(Constants.UIString.FieldLabel.PATIENT_LNAME + ": " +
                    sc.patientLastName);
            }
            if (!String.IsNullOrEmpty(sc.tumorGroup))
            {
                List<int> ids = typeIDStringtoList(sc.tumorGroup, ",");
                string tumorGroups = String.Join(", ",
                    (from tg in _db.TumourGroups
                     where ids.Contains(tg.TumourGroupID)
                     select tg.Value));

                result.Add(Constants.UIString.FieldLabel.TUMOUR_GROUP + ": " +
                          tumorGroups);
            }
            if (!String.IsNullOrEmpty(sc.questionType))
            {
                List<int> ids = typeIDStringtoList(sc.tumorGroup, ",");
                string questionTypes = String.Join(", ",
                    (from qt in _db.QuestionTypes
                     where ids.Contains(qt.QuestionTypeID)
                     select qt.Value));

                result.Add(Constants.UIString.FieldLabel.QUESTION_TYPE + ": " +
                          questionTypes);
            }
            if (!String.IsNullOrEmpty(sc.severity))
            {
                result.Add(Constants.UIString.FieldLabel.SEVERITY + ": " +
                    sc.severity);
            }
            if (!String.IsNullOrEmpty(sc.consequence))
            {
                result.Add(Constants.UIString.FieldLabel.CONSEQUENCE + ": " +
                          sc.consequence);
            }
            return result;
        }

        /// <summary>
        /// Checks if SearchCriteria objects are empty/set to default values
        /// </summary>
        /// <param name="sc">The SearchCriteria to be checked for not-null values</param>
        /// <returns>True if the SearchCriteria is empty, false otherwise</returns>
        public bool isEmptySearchCriteria(SearchCriteria sc)
        {
            if (!String.IsNullOrEmpty(sc.allKeywordString) || !String.IsNullOrEmpty(sc.noneKeywordString)
                || !String.IsNullOrEmpty(sc.anyKeywordString) || !String.IsNullOrEmpty(sc.consequence)
                || !String.IsNullOrEmpty(sc.patientFirstName) || !String.IsNullOrEmpty(sc.patientLastName)
                || !String.IsNullOrEmpty(sc.questionType) || !String.IsNullOrEmpty(sc.requestStatus)
                || !String.IsNullOrEmpty(sc.requestorFirstName) || !String.IsNullOrEmpty(sc.requestorLastName)
                || !String.IsNullOrEmpty(sc.severity) || !String.IsNullOrEmpty(sc.tumorGroup)
                || sc.startTime.CompareTo(new DateTime()) != 0 || sc.completionTime.CompareTo(new DateTime()) != 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        public bool emptyButValidKeywords(SearchCriteria sc)
        {
            if (getKeywords(sc.anyKeywordString).Any() || getKeywords(sc.allKeywordString).Any()
                || getKeywords(sc.noneKeywordString).Any() || !String.IsNullOrEmpty(sc.consequence)
                || !String.IsNullOrEmpty(sc.patientFirstName) || !String.IsNullOrEmpty(sc.patientLastName)
                || !String.IsNullOrEmpty(sc.questionType) || !String.IsNullOrEmpty(sc.requestStatus)
                || !String.IsNullOrEmpty(sc.requestorFirstName) || !String.IsNullOrEmpty(sc.requestorLastName)
                || !String.IsNullOrEmpty(sc.severity) || !String.IsNullOrEmpty(sc.tumorGroup)
                || sc.startTime.CompareTo(new DateTime()) != 0 || sc.completionTime.CompareTo(new DateTime()) != 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts an input String into a list of Int, used for Question Type ID and Tumour Group ID
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="delimiters">Delimiter inside string</param>
        /// <returns>Corresponding List of Integers</returns>
        public List<int> typeIDStringtoList(string input, string delimiters)
        {
            string[] arr = input.Split(delimiters.ToCharArray());
            return arr.Select(int.Parse).ToList();
        }

        /// <summary>
        /// Converts a given string into a list of Strings, to separate a  keyword string into individual keywords
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="delimiters">Delimiter inside the string</param>
        /// <returns>Corresponding List of Strings</returns>
        public List<String> keywordsToList(string input, string delimiters)
        {
            String[] stringArr = input.Split(delimiters.ToCharArray());
            return stringArr.Select(s => s.Trim()).ToList();
        }

        /// <summary>
        /// Converts a String into List of Intgers based on its Enum value specified in Constants.cs
        /// </summary>
        /// <param name="input">Input String</param>
        /// <param name="type">Enumeration Type</param>
        /// <returns>Corresponding List of Integers for that Type and Input</returns>
        public List<int> enumToIDs(string input, Type type)
        {
            String[] stringArr = input.Split(",".ToCharArray());
            return stringArr.Select(v => (int)Enum.Parse(type, v)).ToList();
        }

        /// <summary>
        /// Returns a list of keywordIDs from the string given
        /// </summary>
        /// <param name="keywordString">The keyword string</param>
        /// <returns>the list of keywordIDs</returns>
        public List<int> getKeywords(string keywordString) {
            // First we grab the keywords
            if (String.IsNullOrEmpty(keywordString))
                return new List<int>();
            return (from k in _db.Keywords
                    where keywordsToList(keywordString, ", ").Contains(k.KeywordValue) 
                    select k.KeywordID).ToList();
        }
        /// <summary>
        /// Gets List of Request ID that contain keywordWordString in a Question
        /// </summary>
        /// <param name="keywordString">Input Search String</param>
        /// <returns>List of Request ID's</returns>
        public List<long> getQuestions(string keywordString)
        {
            // First we grab the keywords
            if (String.IsNullOrEmpty(keywordString))
                return new List<long>();
            List<string> test = keywordsToList(keywordString, " ,");
            List<long> acc = new List<long>();
            foreach (string s in test) {
                acc.Union((from qr in _db.QuestionResponses
                           where qr.Question.Contains(s) && qr.Question!=null
                           select qr.RequestID).ToList());

            }
            return acc;
           /* return (from k in keywordsToList(keywordString, " ,")
                        from q in _db.QuestionResponses
                        where q.Question.Contains(k)
                        select q.RequestID).ToList();*/
        }

        /// <summary>
        /// Gets List of Request ID that contain the keywordString in a Response
        /// </summary>
        /// <param name="keywordString">Input Search String</param>
        /// <returns>List of Request ID's</returns>
        public List<long> getResponse(string keywordString)
        {
            // First we grab the keywords
            if (String.IsNullOrEmpty(keywordString))
                return new List<long>();
            return (from k in _db.QuestionResponses
                    where keywordsToList(keywordString, ", ").Contains(k.Response)
                    select k.RequestID).ToList();
        }

        /// <summary>
        /// Get Requests in Database based on SearchCriteria
        /// </summary>
        /// <param name="criteria">Search criteria that user inputs</param>
        /// <returns>List of Requests that match the input </returns>
        public List<Request> searchCriteriaQuery(SearchCriteria criteria) {
            IQueryable<Request> requests = _db.Requests;

            // Filter on patient first name
            if (!String.IsNullOrEmpty(criteria.patientFirstName)) {
                requests =
                    requests.Where(
                        r => r.PatientFName == criteria.patientFirstName);
            }

            // Filter on patient last name
            if (!String.IsNullOrEmpty(criteria.patientLastName)) {
                requests =
                    requests.Where(
                        r => r.PatientLName == criteria.patientLastName);
            }

            // Filter on requestor first name
            if (!String.IsNullOrEmpty(criteria.requestorFirstName)) {
                requests =
                    requests.Where(
                        r => r.RequestorFName == criteria.requestorFirstName);
            }

            // Filter on requestor last name
            if (!String.IsNullOrEmpty(criteria.requestorLastName)) {
                requests =
                    requests.Where(
                        r => r.RequestorLName == criteria.requestorLastName);
            }

            // Filter on start time
            if (criteria.startTime != DateTime.Parse(Constants.EMPTY_DATE)) {
                requests =
                    requests.Where(
                        r => r.TimeOpened.CompareTo(criteria.startTime) >= 0);
            }

            // Filter on end time
            if (criteria.completionTime != DateTime.Parse(Constants.EMPTY_DATE)) {
                requests =
                    requests.Where(
                        r =>
                        r.TimeClosed != null &&
                        (criteria.completionTime.CompareTo(r.TimeClosed) <= 0));
            }

            // Filter on request status
            if (!String.IsNullOrEmpty(criteria.requestStatus)) {
                requests =
                    requests.Where(
                        r =>
                        enumToIDs(criteria.requestStatus,
                                  typeof(Constants.RequestStatus))
                            .Contains(r.RequestStatus));
            }

            // Filter on Question/Response tuples
            IQueryable<QuestionResponse> questionResponses = _db.QuestionResponses;

            // Filter on QR's Severity
            if (!String.IsNullOrEmpty(criteria.severity)) {
                questionResponses =
                    questionResponses.Where(
                        qr =>
                        enumToIDs(criteria.severity, typeof(Constants.Severity))
                            .Contains((int) qr.Severity));
            }

            // Filter on QR's Consequence
            if (!String.IsNullOrEmpty(criteria.consequence)) {
                questionResponses =
                    questionResponses.Where(
                        qr =>
                        enumToIDs(criteria.consequence,
                                  typeof(Constants.Consequence))
                            .Contains((int) qr.Consequence));
            }

            // Filter on QR's Tumor Group
            if (!String.IsNullOrEmpty(criteria.tumorGroup)) {
                questionResponses =
                    questionResponses.Where(
                        qr =>
                        typeIDStringtoList(criteria.tumorGroup, ",")
                            .Contains(qr.TumourGroup.TumourGroupID));
            }

            // Filter on QR's Question Type
            if (!String.IsNullOrEmpty(criteria.questionType)) {
                questionResponses =
                    questionResponses.Where(
                        qr =>
                        typeIDStringtoList(criteria.questionType, ",")
                            .Contains(qr.QuestionType.QuestionTypeID));
            }
                List<long>  anyq = getQuestions(criteria.anyKeywordString);
                List<long> allq = getQuestions(criteria.anyKeywordString);
                List<long> noneq = getQuestions(criteria.anyKeywordString);
            if ((noneq.Any() || anyq.Any() || allq.Any()) &&
                (criteria.keyQuestResp == "Question")) {
                if (noneq.Any()) {
                    List<long> toRemove =
                        (from qr in questionResponses
                         where noneq.Contains(qr.RequestID)
                         select qr.QuestionResponseID).ToList();

                    questionResponses = (from q in questionResponses
                                         where !toRemove.Contains(q.QuestionResponseID)
                                         select q);
                }



                if (allq.Any()) {
                    IQueryable<long> acc = null;
                    foreach (var id in allq) {
                        if (acc == null) {
                            acc = (from q in questionResponses
                                   where q.RequestID == id
                                   select q.RequestID);
                        } else {
                            acc = acc.Intersect(from r in questionResponses
                                                where r.RequestID == id
                                                select r.RequestID);
                        }
                    }
                }

                if (anyq.Any()) {
                    questionResponses = (from q in questionResponses
                                         where anyq.Contains(q.RequestID)
                                         select q);
                }
            }

            List<long> anyr = getResponse(criteria.anyKeywordString);
            List<long> allr = getResponse(criteria.anyKeywordString);
            List<long> noner = getResponse(criteria.anyKeywordString);
            if ((noner.Any() || anyr.Any() || allr.Any()) &&
                (criteria.keyQuestResp == "Response"))
            {
                if (noner.Any())
                {
                    List<long> toRemove =
                        (from qr in questionResponses
                         where noner.Contains(qr.RequestID)
                         select qr.RequestID).ToList();

                    questionResponses = (from q in questionResponses
                                         where !toRemove.Contains(q.RequestID)
                                         select q).AsQueryable();
                }



                if (allr.Any())
                {
                    IQueryable<long> acc = null;
                    foreach (var id in allr)
                    {
                        if (acc == null)
                        {
                            acc = (from q in questionResponses
                                   where q.RequestID == id
                                   select q.RequestID);
                        }
                        else
                        {
                            acc = acc.Intersect(from r in questionResponses
                                                where r.RequestID == id
                                                select r.RequestID);
                        }
                    }
                    questionResponses = (from q in questionResponses
                               where acc.ToList().Contains(q.RequestID)
                               select q);
                }

                if (anyr.Any())
                {
                    questionResponses = (from q in questionResponses
                                         where anyr.Contains(q.RequestID)
                                         select q);
                }
            }
            List<int> any = getKeywords(criteria.anyKeywordString);
            List<int> none = getKeywords(criteria.noneKeywordString);
            List<int> all = getKeywords(criteria.allKeywordString);

            // Filter QRs based on keywords
            if ((none.Any() || any.Any() || all.Any()) && criteria.keyQuestResp == "Keywords") {
                
                IQueryable<KeywordQuestion> results = _db.KeywordQuestions;

                if (none.Any()) {
                    List<long> toRemove =
                        (from kq in results
                         where none.Contains(kq.KeywordID)
                         select kq.RequestID).ToList();

                    results = (from r in results
                               where !toRemove.Contains(r.RequestID)
                               select r);
                }
                if (all.Any()) {
                    IQueryable<long> acc = null;
                    foreach (int id in all) {
                        if (acc == null) {
                            acc = (from r in results
                                   where r.KeywordID == id
                                   select r.RequestID);
                        } else {
                            acc = acc.Intersect(from r in results
                                                where r.KeywordID == id
                                                select r.RequestID);
                        }
                    }
                    results = (from r in results
                               where acc.ToList().Contains(r.RequestID)
                               select r);
                }
                if (any.Any()) {
                    results = (from kq in results
                               where any.Contains(kq.KeywordID)
                               select kq);
                }


                // Then we intersect Keywords with QuestionResponses through the use of a join
                questionResponses = (from kq in results
                                     join qr in questionResponses
                                         on kq.QuestionResponseID equals qr.QuestionResponseID
                                     select qr);
            }

            return (from r in requests
                    join qr in questionResponses
                        on r.RequestID equals qr.RequestID
                    select r).Distinct()
                          .OrderByDescending(r => r.RequestID)
                          .ToList();
        }

        public List<Request> searchCriteriaQueryRoles(SearchCriteria criteria) {
            // Set Criteria based on Users Role(s)
              if (Roles.IsUserInRole(Constants.Roles.ADMINISTRATOR)) {
              } else if (String.IsNullOrEmpty(criteria.requestStatus) &&
                         Roles.IsUserInRole(Constants.Roles.REQUEST_EDITOR)) {
                  criteria.requestStatus = Enum.GetName(
                      typeof(Constants.RequestStatus),
                      Constants.RequestStatus.Completed)
                                           + "," +
                                           Enum.GetName(
                                               typeof(Constants.RequestStatus
                                                   ),
                                               Constants.RequestStatus.Open);
              } else if (String.IsNullOrEmpty(criteria.requestStatus) &&
                         Roles.IsUserInRole(Constants.Roles.VIEWER)) {
                  criteria.requestStatus =
                      Enum.GetName(typeof(Constants.RequestStatus),
                                   Constants.RequestStatus.Completed);
              }

            return searchCriteriaQuery(criteria);
        }
    
    }
}
