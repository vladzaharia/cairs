namespace SasquatchCAIRS.Models.ServiceSystem {
    public class DropdownEntry {
        protected int _id = -1;
        protected string _code = null;
        protected string _value = null;
        protected bool _active = true;

        public DropdownEntry() {
        }

        public DropdownEntry(string code, string value) {
            _code = code;
            _value = value;
        }

        public DropdownEntry(int id,
                             string code,
                             string value,
                             bool active) {
            _id = id;
            _code = code;
            _value = value;
            _active = active;
        }

        public int id {
            get {
                return _id;
            }
        }

        public string code {
            get {
                return _code;
            }
        }

        public string value {
            get {
                return _value;
            }
        }

        public bool active {
            get {
                return _active;
            }
        }

        public string text {
            get {
                return _code + " - " + _value;
            }
        }
    }

    public class KeywordEntry : DropdownEntry {
        public KeywordEntry(int id, string value, bool active) {
            _id = id;
            _value = value;
            _active = active;

            _code = null;
        }
    }
}