using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models {
    public static class Constants {
        public enum Gender {
            None = -1,
            Female = 0,
            Male = 2,
            Other = 3
        }

        public enum RequestStatus {
            Open = 0,
            Completed = 1,
            Invalid = 2
        }

        public enum Severity {
            None = -1,
            Major = 0,
            Moderate = 1,
            Minor = 2
        }

        public enum Consequence {
            None = -1,
            Certain = 0,
            Probable = 1,
            Possible = 2,
            Unlikely = 3
        }

        public enum ReferenceType {
            URL = 0,
            File = 1,
            Text = 2
        }

        public enum DropdownTable {
            Keyword,
            QuestionType,
            Region,
            RequestorType,
            TumourGroup,
            UserGroup
        }

        public const string KEYWORD_TBL_ID_ATTR = "KeywordID";
        public const string KEYWORD_TBL_KEYWORD_ATTR = "Keyword";
        public const string KEYWORD_TBL_ACTIVE_ATTR = "Active";

        public const string QUESTIONTYPE_TBL_ID_ATTR = "QuestionTypeID";
        public const string QUESTIONTYPE_TBL_VALUE_ATTR = "Value";
        public const string QUESTIONTYPE_TBL_CODE_ATTR = "Code";
        public const string QUESTIONTYPE_TBL_ACTIVE_ATTR = "Active";

        public const string REGION_TBL_ID_ATTR = "RegionID";
        public const string REGION_TBL_VALUE_ATTR = "Value";
        public const string REGION_TBL_CODE_ATTR = "Code";
        public const string REGION_TBL_ACTIVE_ATTR = "Active";

        public const string REQUESTORTYPE_TBL_ID_ATTR = "RequestorTypeID";
        public const string REQUESTORTYPE_TBL_VALUE_ATTR = "Value";
        public const string REQUESTORTYPE_TBL_CODE_ATTR = "Code";
        public const string REQUESTORTYPE_TBL_ACTIVE_ATTR = "Active";

        public const string TUMOURGROUP_TBL_ID_ATTR = "TumourGroupID";
        public const string TUMOURGROUP_TBL_VALUE_ATTR = "Value";
        public const string TUMOURGROUP_TBL_CODE_ATTR = "Code";
        public const string TUMOURGROUP_TBL_ACTIVE_ATTR = "Active";

        public const string USERGROUP_TBL_ID_ATTR = "GroupID";
        public const string USERGROUP_TBL_VALUE_ATTR = "Value";
        public const string USERGROUP_TBL_CODE_ATTR = "Code";
        public const string USERGROUP_TBL_ACTIVE_ATTR = "Active";

        public static class Roles {
            public const string VIEWER = "Viewer";
            public const string REQUEST_EDITOR = "RequestEditor";
            public const string REPORT_GENERATOR = "ReportGenerator";
            public const string ADMINISTRATOR = "Administrator";
        }

        public static class UIString {
            // Labels used for fields
            public static class FieldLabel {
                // Request Information
                public const string REQUEST_ID = "Request ID";
                public const string CREATED_BY = "Created By";
                public const string COMPLETED_BY = "Closed By";
                public const string START_TIME = "Start Time";
                public const string COMPLETED_TIME = "Completed Time";
                public const string STATUS = "Status";
                public const string TOTAL_TIME_SPENT = "Total Time Spent";

                // Caller Information
                public const string CALLER_NAME = "Name";
                public const string CALLER_FNAME = "First Name";
                public const string CALLER_LNAME = "Last Name";
                public const string CALLER_EMAIL = "Email";
                public const string CALLER_PHONE = "Phone Number";
                public const string CALLER_TYPE = "Caller Type";
                public const string CALLER_REGION = "Region";

                // Patient Information
                public const string PATIENT_NAME = "Name";
                public const string PATIENT_ID = "Patient ID";
                public const string PATIENT_FNAME = "First Name";
                public const string PATIENT_LNAME = "Last Name";
                public const string PATIENT_GENDER = "Gender";
                public const string PATIENT_AGE = "Age";

                // Question Information
                public const string QUESTION = "Question";
                public const string RESPONSE = "Response";
                public const string SPECIAL_NOTES = "Special Notes/Followup";
                public const string TUMOUR_GROUP = "Tumour Group";
                public const string QUESTION_TYPE = "Question Type";
                public const string SEVERITY = "Severity";
                public const string CONSEQUENCE = "Probability of Consequence";
                public const string IMPACT_SCORE = "Impact Score";
                public const string KEYWORDS = "Keywords";
                public const string REFERENCES = "References";
                public const string PARENT_REQUEST = "Parent Request ID";
                public const string TIME_SPENT = "Time Spent";

                // General
                public const string FULL_NAME = "Full Name";
            }

            // Text used in Buttons
            public static class ButtonText {
                public const string ADD_REQUEST = "Add New Request";
                public const string EDIT_REQUEST = "Edit Request";
                public const string UNLOCK_REQUEST = "Unlock Request";
                public const string EXPORT_REQUEST = "Export Request";
                public const string SAVE_DRAFT = "Save Draft";
                public const string MARK_COMPLETE = "Mark as Complete";
                public const string DELETE_REQUEST = "Delete Request";
            }

            // Text used in Page Titles
            public static class TitleText {
                public const string DASHBOARD = "Dashboard";
                public const string VIEW_REQUEST = "View Request";
                public const string REQUEST_NUM = "Request #";
                public const string ERROR = "Error";
            }
        }

        /// <summary>
        /// Gets the string value for the status
        /// </summary>
        /// <param name="status">Status as a Constants.RequestStatus</param>
        /// <returns>String representing the status</returns>
        public static string getStatusString(RequestStatus status) {
            switch (status) {
                case RequestStatus.Open:
                    return "Open";
                case RequestStatus.Completed:
                    return "Completed";
                case RequestStatus.Invalid:
                    return "Invalid";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets the string value for the reference type
        /// </summary>
        /// <param name="type">Reference Type as a Constants.ReferenceType</param>
        /// <returns>String representing the status</returns>
        public static string getReferenceString(ReferenceType type) {
            switch (type) {
                case ReferenceType.URL:
                    return "URL";
                case ReferenceType.File:
                    return "File";
                case ReferenceType.Text:
                    return "Text";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets the string value for the gender.
        /// </summary>
        /// <param name="gender">Gender as a Constants.Gender</param>
        /// <returns>String representing the gender</returns>
        public static string getGenderString(Gender? gender) {
            switch (gender) {
                case Gender.Female:
                    return "Female";
                case Gender.Male:
                    return "Male";
                case Gender.Other:
                    return "Other";
                default:
                    return "None";
            }
        }

        /// <summary>
        /// Gets the Impact score from severity and consequence
        /// </summary>
        /// <param name="severity">Severity as Constants.Severity</param>
        /// <param name="consequence">Consequence as Constants.Consequence</param>
        /// <returns>A string representing the impact score.</returns>
        public static string getImpactScore(Severity? severity, Consequence? consequence) {
            switch (consequence) {
                case Consequence.Certain:
                case Consequence.Probable:
                    switch (severity) {
                        case Severity.Major:
                            return "1";
                        case Severity.Moderate:
                            return "2";
                        case Severity.Minor:
                            return "3";
                        default:
                            return "";
                    }
                case Consequence.Possible:
                    switch (severity) {
                        case Severity.Major:
                        case Severity.Moderate:
                            return "4";
                        case Severity.Minor:
                            return "5";
                        default:
                            return "";
                    }
                case Consequence.Unlikely:
                    return "5";
                default:
                    return "";
            }
        }
    }
}