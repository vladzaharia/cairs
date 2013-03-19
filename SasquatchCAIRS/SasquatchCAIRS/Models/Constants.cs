using System.Collections.Generic;

namespace SasquatchCAIRS.Models {
    public static class Constants {
        public enum Gender {
            Female = 0,
            Male = 1,
            Other = 2
        }

        public readonly static Gender[] genderOptions = {
            Gender.Female, Gender.Male, Gender.Other
        };

        public enum RequestStatus {
            Open = 0,
            Completed = 1,
            Invalid = 2
        }

        public enum Severity {
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

        public static readonly ReferenceType[] referenceTypeOptions = {
            ReferenceType.URL, ReferenceType.File, ReferenceType.Text
        };

        public enum DropdownTable {
            Keyword,
            QuestionType,
            Region,
            RequestorType,
            TumourGroup,
            UserGroup
        }
    }
}