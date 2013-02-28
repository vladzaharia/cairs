using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models {

    public class SearchCriteria {

        public String keywordString {
            get;
            set;
        }
        public Int16 questionTypeId {
            get;
            set;
        }
        public Int16 tumorGroupId {
            get;
            set;
        }
        public Int16 requestStatus {
            get;
            set;
        }
        public DateTime startTime {
            get;
            set;
        }
        public DateTime completionTime {
            get;
            set;
        }
        public String RequestorLName {
            get;
            set;
        }
        public String RequestorFName
        {
            get;
            set;
        }
        public String PatientLName
        {
            get;
            set;
        }
        public String PatientFName
        {
            get;
            set;
        }
        public Int16 severity {
            get;
            set;
        }
        public Int16 probability {
            get;
            set;
        }
    }

    public class SearchCriteriaContext : DbContext {
        public SearchCriteriaContext()
            : base("DefaultConnection") {
        }

        public DbSet<SearchCriteria> SearchResults {
            get;
            set;
        }
    }
}