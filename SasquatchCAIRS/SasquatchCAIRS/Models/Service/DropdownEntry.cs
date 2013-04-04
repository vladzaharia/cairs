namespace SasquatchCAIRS.Models.Service {
    /// <summary>
    ///     A Dropdown Entry
    /// </summary>
    public class DropdownEntry {
        protected bool _active = true;
        protected string _code = null;
        protected int _id = -1;
        protected string _value = null;

        /// <summary>
        ///     Create new Dropdown Entry with no information
        /// </summary>
        public DropdownEntry() {}

        /// <summary>
        ///     Create new Dropdown entry with code and value
        /// </summary>
        /// <param name="code">The code for the dropdown</param>
        /// <param name="value">The value of the dropdown</param>
        public DropdownEntry(string code, string value) {
            _code = code;
            _value = value;
        }

        /// <summary>
        ///     Create new Dropdown entry with all fields
        /// </summary>
        /// <param name="id">The ID for the dropdown</param>
        /// <param name="code">The code for the dropdown</param>
        /// <param name="value">The value for the dropdown</param>
        /// <param name="active">Whether the entry is active or not</param>
        public DropdownEntry(int id,
                             string code,
                             string value,
                             bool active) {
            _id = id;
            _code = code;
            _value = value;
            _active = active;
        }

        /// <summary>
        ///     Dropdown Entry ID
        /// </summary>
        public int id {
            get { return _id; }
        }

        /// <summary>
        ///     Dropdown Entry Code
        /// </summary>
        public string code {
            get { return _code; }
        }

        /// <summary>
        ///     Dropdown Entry Value
        /// </summary>
        public string value {
            get { return _value; }
        }

        /// <summary>
        ///     Whether the Dropdown Entry is active or not
        /// </summary>
        public bool active {
            get { return _active; }
        }

        /// <summary>
        ///     A combination of the Code and Value
        /// </summary>
        public string text {
            get { return _code + " - " + _value; }
        }
    }

    /// <summary>
    ///     A Keyword Entry, which doesn't include a code
    /// </summary>
    public class KeywordEntry : DropdownEntry {
        /// <summary>
        ///     Create a new Keyword Entry with all field values
        /// </summary>
        /// <param name="id">ID of the Keyword Entry</param>
        /// <param name="value">Keyword's value</param>
        /// <param name="active">Whether or not the keyword is valid</param>
        public KeywordEntry(int id, string value, bool active) {
            _id = id;
            _value = value;
            _active = active;

            _code = null;
        }
    }
}