using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models.ServiceSystem {
    public class AuditCriteria {
        protected string _criteriaType = null;
        protected string _userName = null;
        protected DateTime? _startDate = null;
        protected DateTime? _endDate = null;
        protected string _requestID = null;

        [Required]
        [DisplayName(Constants.UIString.FieldLabel.CRITERIA_TYPE)]
        public string criteriaType {
            get {
                return _criteriaType;
            }
            set {
                 _criteriaType = value;
                 uCriteria = (_criteriaType ?? "").Equals("uCriteria", StringComparison.CurrentCultureIgnoreCase);
                 rCriteria = (_criteriaType ?? "").Equals("rCriteria", StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public bool? uCriteria {
            get;
            set;
        }
        public bool? rCriteria {
            get;
            set;
        }

    [DataType(DataType.Text)]
    public string userName {
            get;
            set;
        }

        public DateTime? startDate {
            get;
            set;
        }

        public DateTime? endDate {
            get;
            set;
        }

        [DataType(DataType.Text)]
        public string requestID {
            get;
            set;
        }
    }
}
