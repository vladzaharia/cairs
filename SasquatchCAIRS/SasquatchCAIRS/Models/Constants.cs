using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models {
    public class Constants {
        public enum Gender {
            None = -1,
            Female = 0,
            Male = 2,
            Other = 3
        }

        public enum RequestStatus {
            Open = 0,
            Completed = 1,
            Invalid = 2
        }

        public enum Priority {
            None = -1,
            Major = 0,
            Moderate = 1,
            Minor = 2
        }

        public enum Consequence {
            None = -1,
            Certain = 0,
            Probable = 1,
            Possible = 2,
            Unlikely = 3
        }

        public enum ReferenceType {
            URL = 0,
            File = 1,
            Text = 2
        }
    }
}