using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models.ServiceSystem {
    public struct QandRwithTimestamp {
        private QuestionResponse _qr;
        private DateTime _timeOpened;
        private DateTime? _timeClosed;

        public QandRwithTimestamp(QuestionResponse qandR, DateTime openTime, DateTime? closeTime) {
            _qr = qandR;
            _timeOpened = openTime;
            _timeClosed = closeTime;
        }

        public QuestionResponse qr {
            get {
                return _qr;
            }
        }

        public DateTime timeOpened {
            get {
                return _timeOpened;
            }
        }

        public DateTime? timeClosed {
            get {
                return _timeClosed;
            }
        }
    }

    public struct FiscalYear : IEquatable<FiscalYear> {
        private int _fiscalYeaNum;

        public int fiscalYeaNum {
            get {
                return _fiscalYeaNum;
            }
        }

        public FiscalYear(DateTime date) {
            int[] previousFiscalYear = new[] { 1, 2, 3 };
            if (previousFiscalYear.Contains(date.Month)) {
                _fiscalYeaNum = date.Year - 1;
            } else {
                _fiscalYeaNum = date.Year;
            }
        }

        public FiscalYear(int year) {
            _fiscalYeaNum = year;
        }

        public void addYear(int i) {
            _fiscalYeaNum += i;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 31 + _fiscalYeaNum;
            return hash;
        }

        public override string ToString() {
            return _fiscalYeaNum.ToString() + " - " + (_fiscalYeaNum + 1).ToString();
        }

        public override bool Equals(object obj) {
            return obj is FiscalYear && Equals((FiscalYear) obj);
        }

        public bool Equals(FiscalYear other) {
            return _fiscalYeaNum == other._fiscalYeaNum;
        }
    }

    public struct MonthYearPair : IEquatable<MonthYearPair> {
        private int _year;
        private int _month;

        private int year {
            get {
                return _year;
            }
        }

        private int month {
            get {
                return _month;
            }
        }

        public MonthYearPair(int month, int year) {
            _month = month;
            _year = year;
        }

        public MonthYearPair(DateTime date) {
            _month = date.Month;
            _year = date.Year;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 31 + _month;
            hash = hash * 7 + _year;
            return hash;
        }

        public void addmonth(int monthToAdd) {
            int addedYear = (_month + monthToAdd - 1) / 12;
            _month = (_month + monthToAdd) % 12;
            if (_month == 0) {
                _month = 12;
            }
            _year = _year + addedYear;
        }

        public override string ToString() {
            return Enum.GetName(typeof(Constants.Month), _month) + "/" + _year;
        }

        public override bool Equals(object obj) {
            return obj is MonthYearPair && Equals((MonthYearPair) obj);
        }

        public bool Equals(MonthYearPair other) {
            return _month == other.month && _year == other.year;
        }

    }
}