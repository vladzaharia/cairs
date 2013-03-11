using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models {
    public class Constants {
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

        public enum Priority {
            None = -1,
            Major = 0,
            Moderate = 1,
            Minor = 2
        }

        public enum Severity {
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
            None = 0,
            Region = 1,
            CallerType = 2,
            TumorGroup = 3
        }
    }
}