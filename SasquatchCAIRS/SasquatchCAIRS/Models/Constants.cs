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

        // For Report Generation
        public enum Month {
            None = 0,
            January = 1,
            Feburary = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }

        public enum DataType {
            AvgTimePerRequest = 0,
            AvgTimeFromStartToComplete = 1,
            TotalNumOfRequests = 2,
            TotalTimeSpent = 3
        }

        public enum StratifyOption {
            Region = 0,
            CallerType = 1,
            TumorGroup = 2
        }
        public enum DropdownTable {
            Keyword,
            QuestionType,
            Region,
            RequestorType,
            TumourGroup,
            UserGroup
        }

        public static string KEYWORD_TBL_ID_ATTR = "KeywordID";
        public static string KEYWORD_TBL_KEYWORD_ATTR = "Keyword";
        public static string KEYWORD_TBL_ACTIVE_ATTR = "Active";

        public static string QUESTIONTYPE_TBL_ID_ATTR = "QuestionTypeID";
        public static string QUESTIONTYPE_TBL_VALUE_ATTR = "Value";
        public static string QUESTIONTYPE_TBL_CODE_ATTR = "Code";
        public static string QUESTIONTYPE_TBL_ACTIVE_ATTR = "Active";

        public static string REGION_TBL_ID_ATTR = "RegionID";
        public static string REGION_TBL_VALUE_ATTR = "Value";
        public static string REGION_TBL_CODE_ATTR = "Code";
        public static string REGION_TBL_ACTIVE_ATTR = "Active";

        public static string REQUESTORTYPE_TBL_ID_ATTR = "RequestorTypeID";
        public static string REQUESTORTYPE_TBL_VALUE_ATTR = "Value";
        public static string REQUESTORTYPE_TBL_CODE_ATTR = "Code";
        public static string REQUESTORTYPE_TBL_ACTIVE_ATTR = "Active";

        public static string TUMOURGROUP_TBL_ID_ATTR = "TumourGroupID";
        public static string TUMOURGROUP_TBL_VALUE_ATTR = "Value";
        public static string TUMOURGROUP_TBL_CODE_ATTR = "Code";
        public static string TUMOURGROUP_TBL_ACTIVE_ATTR = "Active";

        public static string USERGROUP_TBL_ID_ATTR = "GroupID";
        public static string USERGROUP_TBL_VALUE_ATTR = "Value";
        public static string USERGROUP_TBL_CODE_ATTR = "Code";
        public static string USERGROUP_TBL_ACTIVE_ATTR = "Active";
    }
}