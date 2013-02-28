using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public String callerFirstName {
            set;
        }
        public String requestorLName {
            get;
            set;
        }
        public String requestorFName
        {
            get;
            set;
        }
        public String patientLName
        {
            get;
            set;
        }
        public String patientFName
        {
            get;
            set;
        }


        [DataType(DataType.Text)]
        public String callerLastName {
            get;
            set;
        }

        public Int16 severity {
            get;
            set;
        }

            get;
            set;
        }

        public Int16 status {
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