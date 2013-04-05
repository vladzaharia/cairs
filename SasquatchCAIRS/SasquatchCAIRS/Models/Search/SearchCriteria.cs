using System;
using System.ComponentModel.DataAnnotations;

namespace SasquatchCAIRS.Models.Search {
    /// <summary>
    ///     The Search Criteria used.
    /// </summary>
    public class SearchCriteria {
        /// <summary>
        ///     "ANY" Keyword
        /// </summary>
        [DataType(DataType.Text)]
        public String anyKeywordString { get; set; }

        /// <summary>
        ///     "ALL" Keyword
        /// </summary>
        [DataType(DataType.Text)]
        public String allKeywordString { get; set; }

        /// <summary>
        ///     "NONE" Keyword
        /// </summary>
        [DataType(DataType.Text)]
        public String noneKeywordString { get; set; }

        /// <summary>
        ///     The Start Time
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime startTime { get; set; }

        /// <summary>
        ///     The Completion Time
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime completionTime { get; set; }

        /// <summary>
        ///     The status of the Request
        /// </summary>
        [DataType(DataType.Text)]
        public String requestStatus { get; set; }

        /// <summary>
        ///     The First Name of the Caller
        /// </summary>
        [DataType(DataType.Text)]
        public String requestorFirstName { get; set; }

        /// <summary>
        ///     The Last Name of the Caller
        /// </summary>
        [DataType(DataType.Text)]
        public String requestorLastName { get; set; }

        /// <summary>
        ///     The First Name of the Patient
        /// </summary>
        [DataType(DataType.Text)]
        public String patientFirstName { get; set; }

        /// <summary>
        ///     The First Name of the Patient
        /// </summary>
        [DataType(DataType.Text)]
        public String patientLastName { get; set; }

        /// <summary>
        ///     The Tumour Group asked about
        /// </summary>
        [DataType(DataType.Text)]
        public String tumorGroup { get; set; }

        /// <summary>
        ///     The Type of Question asked
        /// </summary>
        [DataType(DataType.Text)]
        public String questionType { get; set; }

        /// <summary>
        ///     The Question Severity
        /// </summary>
        [DataType(DataType.Text)]
        public String severity { get; set; }

        /// <summary>
        ///     The Question Consequence
        /// </summary>
        [DataType(DataType.Text)]
        public String consequence { get; set; }
    }
}