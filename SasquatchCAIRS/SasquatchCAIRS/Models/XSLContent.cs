using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models {
    public struct XslContentKey: IEquatable<XslContentKey> {
        private readonly System.Nullable<byte> _typeId;
        private readonly int _month;
        private readonly int _year;

        public System.Nullable<byte> typeId {
            get {
                return _typeId;
            }
        }
        public int month {
            get { return _month; }
        }
        public int year {
            get { return _year; }
        }

        public XslContentKey(int month, int year, System.Nullable<byte> typeId)
        {
            _month = month;
            _year = year;
            _typeId = typeId;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash*31 + _month;
            hash = hash*7 + _year;
            if (_typeId.HasValue)
            {
                hash = hash*23 + _typeId.Value;
            }
            return hash;
        }

        public override bool Equals(object obj) {
            return obj is XslContentKey? Equals((XslContentKey) obj) : false;
        }

        public bool Equals(XslContentKey other)
        {
            if (_typeId.HasValue)
            {
                return _month == other.month && _year == other.year && _typeId.Value == other.typeId;
            }
            else
            {
                return _month == other.month && _year == other.year;
            }
        }
    }

    public class XSLContent
    {
        private string _title;
        private string[] _rowNames;
        private string[] _columnNames;
        private Point[] _columnPointReps;
        private Dictionary<XslContentKey, long> _cellValues;

        public XSLContent XslContent(string tit, string[] rows, Point[] columnPointReps, Dictionary<XslContentKey, long> cellValues)
        {
            _title = tit;
            _rowNames = rows;
            _columnPointReps = columnPointReps;
            _cellValues = cellValues;
            return this;
        }

        public XSLContent XslContent()
        {
            return this;
        }

        public string title
        {
            get { return _title; }
            set { _title = value; }
        }

        public  string[] rowNames
        {
            get { return _rowNames; }
            set { _rowNames = value; }
        }

        //when setting columnPointReps, it also sets the column headers in string representation.
        public Point[] columnPointReps {
            get {
                return _columnPointReps;
            }
            set {
                _columnPointReps = value;
                _columnNames = new string[_columnPointReps.Count()];
                for (int i = 0; i < _columnPointReps.Count(); i++)
                {
                    _columnNames[i] = Enum.GetName(typeof(Constants.Month), _columnPointReps[i].X).ToString() + "/" + _columnPointReps[i].Y;
                }
            }
        }

        public string[] columnNames
        {
            get { return _columnNames; }
        }

        public Dictionary<XslContentKey, long> valueTable {
            get {
                return _cellValues;
            }
            set {
                _cellValues = value;
            }
        }
    }
}