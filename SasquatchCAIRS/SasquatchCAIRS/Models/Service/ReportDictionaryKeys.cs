using System;
using System.Linq;
using SasquatchCAIRS.Models.Common;

namespace SasquatchCAIRS.Models.Service {
    /// <summary>
    ///     QuestionResponse with a timestamp
    /// </summary>
    public struct QandRwithTimestamp {
        private QuestionResponse _qr;
        private DateTime? _timeClosed;
        private DateTime _timeOpened;

        /// <summary>
        ///     Create a new QuestionResponse with open and closed time
        /// </summary>
        /// <param name="qandR">QuestionResponse</param>
        /// <param name="openTime">Time opened</param>
        /// <param name="closeTime">Time closed</param>
        public QandRwithTimestamp(QuestionResponse qandR, DateTime openTime,
                                  DateTime? closeTime) {
            _qr = qandR;
            _timeOpened = openTime;
            _timeClosed = closeTime;
        }

        /// <summary>
        ///     QuestionResponse object
        /// </summary>
        public QuestionResponse qr {
            get { return _qr; }
        }

        /// <summary>
        ///     The Time QR was opened
        /// </summary>
        public DateTime timeOpened {
            get { return _timeOpened; }
        }

        /// <summary>
        ///     The Time QR was Closed
        /// </summary>
        public DateTime? timeClosed {
            get { return _timeClosed; }
        }
    }

    /// <summary>
    ///     A BCCA Fiscal Year
    /// </summary>
    public struct FiscalYear : IEquatable<FiscalYear> {
        private int _fiscalYeaNum;

        /// <summary>
        ///     Create a new FiscalYear for a date
        /// </summary>
        /// <param name="date">The DateTime to create a fiscal year for</param>
        public FiscalYear(DateTime date) {
            var previousFiscalYear = new[] {1, 2, 3};
            if (previousFiscalYear.Contains(date.Month)) {
                _fiscalYeaNum = date.Year - 1;
            } else {
                _fiscalYeaNum = date.Year;
            }
        }

        /// <summary>
        ///     Create a new FiscalYear for a year
        /// </summary>
        /// <param name="year">The year to create this FiscalYear for</param>
        public FiscalYear(int year) {
            _fiscalYeaNum = year;
        }

        /// <summary>
        ///     The Number of the Fiscal Year
        /// </summary>
        public int fiscalYeaNum {
            get { return _fiscalYeaNum; }
        }

        public bool Equals(FiscalYear other) {
            return _fiscalYeaNum == other._fiscalYeaNum;
        }

        /// <summary>
        ///     Add a year to the Fiscal Year
        /// </summary>
        /// <param name="i">The number of years to add</param>
        public void addYear(int i) {
            _fiscalYeaNum += i;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash*31 + _fiscalYeaNum;
            return hash;
        }

        public override string ToString() {
            return _fiscalYeaNum.ToString() + " - " +
                   (_fiscalYeaNum + 1).ToString();
        }

        public override bool Equals(object obj) {
            return obj is FiscalYear && Equals((FiscalYear) obj);
        }
    }

    /// <summary>
    ///     A combination of Month and Year for Reporting
    /// </summary>
    public struct MonthYearPair : IEquatable<MonthYearPair> {
        private int _month;
        private int _year;

        /// <summary>
        ///     Create a new Pair by Month and Year
        /// </summary>
        /// <param name="month">The month value</param>
        /// <param name="year">The year value</param>
        public MonthYearPair(int month, int year) {
            _month = month;
            _year = year;
        }

        /// <summary>
        ///     Create a new Pair by DateTime
        /// </summary>
        /// <param name="date">The date to use</param>
        public MonthYearPair(DateTime date) {
            _month = date.Month;
            _year = date.Year;
        }

        private int year {
            get { return _year; }
        }

        private int month {
            get { return _month; }
        }

        public bool Equals(MonthYearPair other) {
            return _month == other.month && _year == other.year;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash*31 + _month;
            hash = hash*7 + _year;
            return hash;
        }

        /// <summary>
        ///     Add a number of months to the pair
        /// </summary>
        /// <param name="monthToAdd">Number of months to add</param>
        public void addmonth(int monthToAdd) {
            int addedYear = (_month + monthToAdd - 1)/12;
            _month = (_month + monthToAdd)%12;
            if (_month == 0) {
                _month = 12;
            }
            _year = _year + addedYear;
        }

        public override string ToString() {
            return Enum.GetName(typeof (Constants.Month), _month) + "/" + _year;
        }

        public override bool Equals(object obj) {
            return obj is MonthYearPair && Equals((MonthYearPair) obj);
        }
    }
}