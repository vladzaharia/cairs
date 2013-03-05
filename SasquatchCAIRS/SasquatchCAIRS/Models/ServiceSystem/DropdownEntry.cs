using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models.ServiceSystem {
    public class DropdownEntry {
        private int _id = -1;
        private string _code = null;
        private string _value = null;
        private bool _active = true;

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
    }
}