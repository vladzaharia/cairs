using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models.ServiceSystem {
    public class Request {
        private int _requestID = -1;

        private string _requestorFirstName;
        private string _requestorLastName;
        private string _requestorPhoneNum;
        private string _requestorPhoneExt;
        private string _requestorEmail;

        private string _patientFirstName;
        private string _patientLastName;
        private Constants.Gender _patientGender;
        private string _patientAgencyID;
        private ushort _patientAge;

        

        public Request(int id) {
            _requestID = id;
        }
    
        public int requestID {
            get {
                return _requestID;
            }
            set {
                _requestID = value;
            }
        }

        public string requestorFirstName {
            get {
                return _requestorFirstName;
            }
            set {
                _requestorFirstName = value;
            }
        }

        private string requestorLastName {
            get;
            set;
        }

        private string requestorPhone {
            get;
            set;
        }

        private string requestorPhoneExt {
            get;
            set;
        }

        private string requestorEmail {
            get;
            set;
        }

        private string patientFirstName {
            get;
            set;
        }

        private string patientLastName {
            get;
            set;
        }

        private string patientGender {
            get;
            set;
        }

        private int patientAge {
            get;
            set;
        }

        private string patientAgencyID {
            get;
            set;
        }
    }

    public class QuestionResponse {

    }
}