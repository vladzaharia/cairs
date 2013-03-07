using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models {

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
        public String patientLastName {
            get;
            set;
        }

        public String tumorGroup {
            get;
            set;
        }
        public Int16 probability {
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