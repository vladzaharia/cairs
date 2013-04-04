using System.Collections.Generic;

namespace SasquatchCAIRS.Models {
    public static class Constants {
        public enum Gender {
            Female = 0,
            Male = 1,
            Other = 2
        }

        public readonly static Gender[] genderOptions = {
            Gender.Female, Gender.Male, Gender.Other
        };

        public enum RequestStatus {
            Open = 0,
            Completed = 1,
            Invalid = 2
        }

        public enum Severity {
            Major = 0,
            Moderate = 1,
            Minor = 2
        }

        public static readonly Severity[] severityOptions = {
            Severity.Major, Severity.Moderate, Severity.Minor
        };

        public enum Consequence {
            Certain = 0,
            Probable = 1,
            Possible = 2,
            Unlikely = 3
        }

        public static readonly Consequence[] consequenceOptions = {
            Consequence.Certain,
            Consequence.Probable,
            Consequence.Probable,
            Consequence.Unlikely
        };

       

       

        public enum ReferenceType {
            URL = 0,
            File = 1,
            Text = 2
        }

        public static readonly ReferenceType[] referenceTypeOptions = {
            ReferenceType.URL, ReferenceType.File, ReferenceType.Text
        };

        public const int reportHeaderRow = 4;
        public const int dataStartRow = reportHeaderRow + 1;

        public static readonly string[] DATATABLE_TITLES = new string[13] {
            "General Report", "Avg Time Per Request Stratified by Geographical Region", 
            "Avg Time To Complete Stratified by Geographical Region",
            "Total Number of Requests Stratified by Geographical Region", 
            "Total Time Spent Stratified by Geographical Region",
            "Avg Time Per Request Stratified by Caller Type", 
            "Avg Time To Complete Stratified by Caller Type",
            "Total Number of Requests Stratified by Caller Type", 
            "Total Time Spent Stratified by Caller Type",
            "Avg Time Per Question Stratified by Tumour Group", 
            "Avg Time To Complete Stratified by Tumour Group",
            "Total Number of Questions Stratified by Tumour Group", 
            "Total Time Spent Stratified by Tumour Group"
        };

        public enum DropdownTable {
            Keyword,
            QuestionType,
            Region,
            RequestorType,
            TumourGroup,
            UserGroup
        }

        public enum URLStatus {
            None = 0,
            Expired = 1,
            Unlocked = 2,
            Deleted = 3,
            AccessingLocked = 4,
            NotLockedToYou = 5,
            SuccessfulEdit = 6,
            NoRequestEditorRole = 7,
            SuccessfulCreate = 8,
            EditingInvalid = 9
        }

        public static readonly DropdownTable[] DROPDOWN_TABLES = new DropdownTable[6] {
            DropdownTable.Keyword,
            DropdownTable.QuestionType, 
            DropdownTable.Region, 
            DropdownTable.RequestorType, 
            DropdownTable.TumourGroup, 
            DropdownTable.UserGroup
        };

        public const string EMPTY_DATE = "0001-01-01";
        public const string DATE_FORMAT = "yyyy-MM-dd";
        public const int PAGE_SIZE = 20;

        public static class DataTypeStrings {
            public static string DATA_TYPE = "Data Type";
            public static string AVG_TIME = "Avg Time";
            public static string AVG_TIME_TO_COMPLETE = "Avg Time To Complete";
            public static string TOTAL_NUM = "Total Number";
            public static string TOTAL_TIME_SPENT = "Total Time Spent";
        }


        public static class ReportFormStrings {
            public static string REPORT_OPTION = "reportOption";
            public static string DATATYPE = "dataType";
            public static string STRATIFY_BY = "stratifyBy";
        }

        public static class Roles {
            public const string VIEWER = "Viewer";
            public const string REQUEST_EDITOR = "RequestEditor";
            public const string REPORT_GENERATOR = "ReportGenerator";
            public const string ADMINISTRATOR = "Administrator";

            public static readonly string[] ROLE_OPTIONS = {
                VIEWER,
                REQUEST_EDITOR, 
                REPORT_GENERATOR, 
                ADMINISTRATOR
            };
        }

        public static class Export {
            // File Section
            public const string REPORT_TEMPLATE_PATH = "~/ReportTemplate.docx";
            public const string REPORT_TEMP_PATH = "~/Report";
            public const string WORD_CONTENT_TYPE = "application/vnd.ms-word";
            public const string WORD_FILE_EXT = ".docx";
            public const string CONTENT_DISPOSITION = "Content-Disposition";
            public const string REQUEST_ATTACHMENT = "attachment; filename=Request";

            // Request Information Section
            public const string REQUEST_INFORMATION = "Request Information: ";
            public const string REQUEST_ID = "Request ID: ";
            public const string REQUEST_CREATED_BY = "Created By: ";
            public const string REQUEST_CLOSED_BY = "Closed By: ";
            public const string REQUEST_START_TIME = "Start Time: ";
            public const string REQUEST_COMPLETION_TIME = "Completed Time: ";
            public const string REQUEST_TIME_SPENT = "Total Time Spent: ";
            public const string REQUEST_PARENT_ID = "Parent Request ID: ";
            public const string REQUEST_PROPERTIES = "Request Properties";

            // Requestor Information Section
            public const string REQUESTOR_INFORMATION = "Requestor Information:";
            public const string REQUESTOR_NAME = "Requestor Name: ";
            public const string REQUESTOR_EMAIL = "Requestor's Email: ";
            public const string REQUESTOR_PHONE = "Requestor's Phone Number: ";
            public const string REQUESTOR_PHONE_EXT = "Ext: ";
            public const string REQUESTOR_TYPE = "Requestor Type: ";
            public const string REQUSTOR_REGION = "Requestor Region: ";

            // Patient Information Section
            public const string PATIENT_INFORMATION = "Patient Information:";
            public const string PATIENT_NAME ="Patient Name: ";
            public const string PATIENT_ID = "Patient ID: ";
            public const string PATIENT_AGE = "Patient Age: ";
            public const string PATIENT_GENDER = "Patient Gender: ";

            // Question/Response Information Section
            public const string QUESTION_INFORATION = "Question Information:";
            public const string QUESTION_NUMBER = "Question #";
            public const string QUESTION_RESPONSE = "Response: ";
            public const string QUESTION_SPECIAL_NOTES = "Special Notes/Followup: ";
            public const string QUESTION_TYPE = "Question Type: ";
            public const string QUESTION_TUMOUR_GROUP = "Tumour Group: ";
            public const string QUESTION_TIME_SPENT = "Time Spent: ";
            public const string QUESTION_CONSEQUENCE = "Consequence: ";
            public const string QUESTION_IMPACT_SCORE = "Impact Score: ";
            public const string QUESTION_SEVERITY = "Severity: ";
            public const string QUESTION_KEYWORDS = "Keywords: ";
            public const string QUESTION_REFERENCES = "References: ";

            // Random

            public const string TIME_UNITS = " min(s)";
            // Header List
            public static readonly List<string> EXPORT_HEADERS = new List<string>{
                REQUEST_INFORMATION,
                REQUESTOR_INFORMATION,
                PATIENT_INFORMATION,
                QUESTION_INFORATION,
                REQUEST_PROPERTIES
            }; 

        }

        public static class UIString {
            // String Responses
            public static class Response {
                public const string NO_RESULTS = "No results were found.";

                public const string EMPTY_QUERY =
                    "You have not specified any search criteria.";
            }


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
                public const string CALLER_NAME_TABLE = "Caller Name";
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

                // Search Information
                public const string PATIENT_NAME_LABEL = "Patient Name";
                public const string CALLER_NAME_LABEL = "Caller Name";
                

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
                public const string ANY_KEYWORDS = "Any of These Keywords";
                public const string ALL_KEYWORDS = "All of These Keywords";
                public const string NONE_KEYWORDS = "None of These Keywords";
                public const string REFERENCES = "References";
                public const string PARENT_REQUEST = "Parent Request ID";
                public const string TIME_SPENT = "Time Spent";

                // General
                public const string FULL_NAME = "Full Name";
                public const string QUICK_SEARCH = "Quick Search";

                // User
                public const string USERNAME = "Username";
                public const string ROLES = "Roles";
                public const string GROUPS = "Groups";
                public const string USER_EMAIL = "Email Address";

                // Report Information
                public const string USER_CRITERIA = "Username Criteria";
                public const string REQUEST_CRITERIA = "Request ID Criteria";
                public const string SEARCH_CRITERIA = "Select Search Criteria";
                public const string START_DATE = "Start Date";
                public const string CRITERIA_TYPE = "audit criteria type";
                public const string AUDIT_CRITERIA = "Select Audit Criteria";

                // Dropdowns
                public const string DROPDOWN_CODE = "Code";
                public const string DROPDOWN_VALUE = "Value";
                public const string DROPDOWN_KEYWORD = "Keyword";
                public const string DROPDOWN_STATUS = "Status";
                public const string END_DATE = "End Date";


            }

            // Text used in Buttons
            public static class ButtonText {
                public const string CREATE_REQUEST = "Create Request";
                public const string EDIT_REQUEST = "Edit Request";
                public const string UNLOCK_REQUEST = "Unlock Request";
                public const string EXPORT_REQUEST = "Export Request";
                public const string SAVE_DRAFT = "Save Draft";
                public const string MARK_COMPLETE = "Mark as Complete";
                public const string DELETE_REQUEST = "Delete Request";
                public const string ADD_QUESTION = "Add Question";
                public const string DELETE_QUESTION = "Delete Question";
                public const string ADD_REFERENCE = "Add Reference";
                public const string MODIFY_SEARCH = "Modify Search";
                public const string NEW_SEARCH = "New Search";
                public const string SEARCH = "Search!";
                public const string EDIT_USER = "Save Changes";
                public const string EDIT_DROPDOWN = "Save Changes";
                public const string CREATE_DROPDOWN = "Create Dropdown Entry";
                public const string USERS = "Users";
                public const string LOOKUP = "Lookup Fields";
                public const string GEN_AUDIT = "Generate Audit Report";
                public const string CREATE = "Create";
                public const string UPDATE_REQUEST = "Update";
            }

            // Text used in Page Titles
            public static class TitleText {
                public const string DASHBOARD = "Dashboard";
                public const string VIEW_REQUEST = "View Request";
                public const string REQUEST_NUM = "Request #";
                public const string ERROR = "Error";
                public const string ADMIN = "Admin Settings";
                public const string USERS = "Users";
                public const string EDIT_USER = "Edit User";
                public const string DROPDOWN_LISTS = "Dropdown Lists";
                public const string EDIT_DROPDOWN = "Edit Dropdown Entry";
                public const string CREATE_DROPDOWN = "Create Dropdown Entry";
                public const string ADVANCED_SEARCH = "Advanced Search";
                public const string AUDIT_LOG = "Generate Audit Report";
                public const string RESULTS = "Results";
                public const string CREATE_REQUEST = "Create Request";
                public const string REPORTS = "Reports";
                public const string CREATE = "Create";
                public const string EDIT = "Edit";
            }

            public static class GeneralText {
                public const string ACTIVE = "Active";
                public const string DISABLED = "Disabled";
            }

            public static class ItemIDs {
                public const string DASHBOARD = "dashboard";
                public const string CREATE_REQUEST = "create-request";
                public const string REPORTS = "reports";
                public const string ADMIN = "admin";
                public const string SEARCH_DIV = "quick-search-div";
                public const string SEARCH_FIELD = "keywords";
                public const string SEARCH_BUTTON = "search-button";
                public const string ADVANCED_SEARCH = "advanced-search";
                public const string SUBMIT_BUTTON = "submit-button";
            }

            public static class Messages {
                public const string DELETE_REFERENCE_WARNING =
                    "Are you sure you would like to delete this reference?";
                public const string DELETE_QUESTION_WARNING =
                    "Are you sure you would like to delete this question?";
                public const string NO_CONTACT_WARNING =
                    "Are you sure you would like to mark this request as complete without requestor contact information?";
            }
        }

        // For Report Generation
        public enum Month {
            Jan = 1,
            Feb = 2,
            Mar = 3,
            Apr = 4,
            May = 5,
            Jun = 6,
            Jul = 7,
            Aug = 8,
            Sep = 9,
            Oct = 10,
            Nov = 11,
            Dec = 12
        }

        public enum DataType {
            AvgTimePerRequest = 1,
            AvgTimeToComplete = 2,
            TotalNumOfRequests = 3,
            TotalTimeSpent = 4
        }

        public enum StratifyOption {
            None = 0,
            Region = 1,
            CallerType = 2,
            TumorGroup = 3
        }


        public enum CellDataType {
            Number = 0,
            Text = 1
        }

        public enum ReportType {
            Report = 0,
            AuditLog = 1
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
                    return "";
            }
        }

        /// <summary>
        /// Get the string value for a dropdown list
        /// </summary>
        /// <param name="dropdown">Dropdown list as Constants.DropdownTable</param>
        /// <returns>String representing the DropdownTable name</returns>
        public static string getDropdownString(DropdownTable? dropdown) {
            switch (dropdown) {
                case DropdownTable.Keyword:
                    return "Keyword";
                case DropdownTable.QuestionType:
                    return "Question Type";
                case DropdownTable.Region:
                    return "Region";
                case DropdownTable.RequestorType:
                    return "Requestor Type";
                case DropdownTable.TumourGroup:
                    return "Tumour Group";
                case DropdownTable.UserGroup:
                    return "User Group";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Get the DropdownTable for a String
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The DropdownTable for the string</returns>
        public static DropdownTable getTableForString(string s) {
            switch (s) {
                case "Keyword":
                    return DropdownTable.Keyword;
                case "Question Type":
                    return DropdownTable.QuestionType;
                case "Region":
                    return DropdownTable.Region;
                case "Requestor Type":
                    return DropdownTable.RequestorType;
                case "Tumour Group":
                    return DropdownTable.TumourGroup;
                case "User Group":
                    return DropdownTable.UserGroup;
                default:
                    return DropdownTable.Keyword;
            }
        }

        /// <summary>
        /// Gets the Impact score from severity and consequence
        /// </summary>
        /// <param name="severity">Severity as Constants.Severity</param>
        /// <param name="consequence">Consequence as Constants.Consequence</param>
        /// <returns>A string representing the impact score.</returns>
        public static string getImpactScore(Severity? severity,
                                            Consequence? consequence) {
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

        public enum AuditType {
            RequestCreation = 0,
            RequestCompletion = 1,
            RequestDeletion = 2,
            RequestModification = 3,
            RequestView = 4,
            RequestExport = 5,
            RequestUnlock = 6

        }

        public enum AuditCriteriaType {
            user = 0,
            request = 1
        }
    }
}