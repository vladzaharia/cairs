using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SasquatchCAIRS.Models.Common;

namespace SasquatchCAIRS.Models.Security {
    /// <summary>
    ///     The Audit Criteria Used
    /// </summary>
    public class AuditCriteria {
        /// <summary>The criteria type.</summary>
        private string _criteriaType;

        /// <summary>
        ///     The Criteria Type
        /// </summary>
        [Required]
        [DisplayName(Constants.UIString.FieldLabel.CRITERIA_TYPE)]
        public string criteriaType {
            get { return _criteriaType; }
            set {
                _criteriaType = value;
                uCriteria = (_criteriaType ?? "").Equals("uCriteria",
                                                         StringComparison
                                                             .CurrentCultureIgnoreCase);
                rCriteria = (_criteriaType ?? "").Equals("rCriteria",
                                                         StringComparison
                                                             .CurrentCultureIgnoreCase);
            }
        }

        /// <summary>
        ///     User Criteria
        /// </summary>
        public bool? uCriteria { get; set; }

        /// <summary>
        ///     Request Criteria
        /// </summary>
        public bool? rCriteria { get; set; }

        /// <summary>
        ///     Username
        /// </summary>
        [DataType(DataType.Text)]
        public string userName { get; set; }

        /// <summary>
        ///     Start Date
        /// </summary>
        public DateTime? startDate { get; set; }

        /// <summary>
        ///     End Date
        /// </summary>
        public DateTime? endDate { get; set; }

        /// <summary>
        ///     Request ID
        /// </summary>
        [DataType(DataType.Text)]
        public string requestID { get; set; }
    }
}