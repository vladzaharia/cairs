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

        public const string EMPTY_DATE = "0001-01-01";
        public const string DATE_FORMAT = "yyyy-MM-dd";
    }
}