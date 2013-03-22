using System;
using System.ComponentModel.DataAnnotations;

namespace SasquatchCAIRS.Models.SearchSystem {

    public class SearchCriteria {

        [DataType(DataType.Text)]
        public String keywordString {
            get;
            set;
        }

        [DataType(DataType.Date)]
        public DateTime startTime { 
            get;
            set;
        }

        [DataType(DataType.Date)]
        public DateTime completionTime {
            get;
            set;
        }

        [DataType(DataType.Text)]
        public String requestStatus {
            get;
            set;
        }

        [DataType(DataType.Text)]
        public String requestorFirstName {
            get;
            set;
        }

        [DataType(DataType.Text)]
        public String requestorLastName {
            get;
            set;
        }

        [DataType(DataType.Text)]
        public String patientFirstName { 
            get;
            set;
        }
        [DataType(DataType.Text)]
        public String patientLastName {
            get;
            set;
        }

        [DataType(DataType.Text)]
        public String tumorGroup {
            get;
            set;
        }

        [DataType(DataType.Text)]
        public String questionType {
            get;
            set;
        }

        [DataType(DataType.Text)]
        public String severity {
            get;
            set;
        }

        [DataType(DataType.Text)]
        public String consequence {
            get;
            set;
        }
    }
}