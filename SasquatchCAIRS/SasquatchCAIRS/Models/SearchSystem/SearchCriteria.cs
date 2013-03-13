using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

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

        public String tumorGroup {
            get;
            set;
        }

        public String questionType {
            get;
            set;
        }

        public String severity {
            get;
            set;
        }

        public String consequence {
            get;
            set;
        }
    }

    public class SearchCriteriaContext : DbContext {
        public SearchCriteriaContext()
            : base("sasquatchConnectionString") {
        }

        public DbSet<SearchCriteria> SearchCriterias {
            get;
            set;
        }
    }
}