using System;
using System.Linq;
using NUnit.Framework;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;
using Assert = NUnit.Framework.Assert;

namespace CAIRSTestProject.Unit {
    [TestFixture]
    public class TestRequestManagementController {
        private CAIRSDataContext _db;
        private UserProfile _up;
        private long? _reqId; 
        private RequestManagementController _rmc;

        private RequestorType _rType;
        private Region _region;
        private TumourGroup _tGroup;
        private QuestionType _qType;

        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            _db = new CAIRSDataContext();
            _rmc = new RequestManagementController();

            Random random = new Random();

            _up = new UserProfile {
                UserName = "TRMC" + random.Next(1, 100000000)
            };
            _db.UserProfiles.InsertOnSubmit(_up);
            _db.SubmitChanges();

            _rType = new RequestorType {
                Code = "TRMC" + random.Next(1, 100000),
                Value = "TestRequestManagementController" + random.Next(1, 100000000),
                Active = true
            };
            _db.RequestorTypes.InsertOnSubmit(_rType);
            _db.SubmitChanges();

            _region = new Region {
                Code = "TRMC" + random.Next(1, 100000),
                Value = "TestRequestManagementController" + random.Next(1, 100000000),
                Active = true
            };
            _db.Regions.InsertOnSubmit(_region);
            _db.SubmitChanges();

            _tGroup = new TumourGroup {
                Code = "TRMC" + random.Next(1, 100000),
                Value = "TestRequestManagementController" + random.Next(1, 100000000),
                Active = true
            };
            _db.TumourGroups.InsertOnSubmit(_tGroup);
            _db.SubmitChanges();

            _qType = new QuestionType {
                Code = "TRMC" + random.Next(1, 100000),
                Value = "TestRequestManagementController" + random.Next(1, 100000000),
                Active = true
            };
            _db.QuestionTypes.InsertOnSubmit(_qType);
            _db.SubmitChanges();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown() {
            _db.UserProfiles.DeleteOnSubmit(_up);
            _db.RequestorTypes.DeleteOnSubmit(_rType);
            _db.Regions.DeleteOnSubmit(_region);
            _db.TumourGroups.DeleteOnSubmit(_tGroup);
            _db.QuestionTypes.DeleteOnSubmit(_qType);
            _db.SubmitChanges();
        }

        [TearDown]
        public void TearDown() {
            if (_reqId == null) {
                return;
            }

            Request req = (from r in _db.Requests
                           where r.RequestID == _reqId
                           select r)
                           .SingleOrDefault();

            if (req == null) {
                return;
            }

            QuestionResponse[] qrArr =
                (from q in _db.QuestionResponses
                 where q.RequestID == _reqId
                 select q)
                                       .ToArray();

            foreach (var qr in qrArr) {
                Reference[] rArr =
                    (from r in _db.References
                     where r.RequestID == _reqId &&
                           r.QuestionResponseID ==
                           qr.QuestionResponseID
                     select r)
                        .ToArray();

                foreach (var r in rArr) {
                    _db.References.DeleteOnSubmit(r);
                    _db.SubmitChanges();
                }

                KeywordQuestion[] kqArr = 
                    (from kq in _db.KeywordQuestions
                     where kq.RequestID == _reqId &&
                           kq.QuestionResponseID == qr.QuestionResponseID
                     select kq)
                     .ToArray();

                foreach (var kq in kqArr) {
                    int kqId = kq.KeywordID;

                    _db.KeywordQuestions.DeleteOnSubmit(kq);
                    _db.SubmitChanges();

                    Keyword kw = (from k in _db.Keywords
                                  where k.KeywordID == kqId
                                  select k)
                        .Single();

                    _db.Keywords.DeleteOnSubmit(kw);
                    _db.SubmitChanges();
                }

                _db.QuestionResponses.DeleteOnSubmit(qr);
                _db.SubmitChanges();
            }

            _db.Requests.DeleteOnSubmit(req);
            _db.SubmitChanges();

            _reqId = null;
        }

        [Test]
        public void Test_create() {
            DateTime opened = DateTime.Now;

            RequestContent rCon = new RequestContent {
                requestStatus = Constants.RequestStatus.Open,
                requestorFirstName = "Bob",
                requestorLastName = "Smith",
                requestorEmail = "bsmith@gmail.com",
                requestorPhoneNum = "123-456-7890",
                requestorPhoneExt = "0000",
                patientFName = "Jane",
                patientLName = "Doe",
                patientGender = Constants.Gender.Female,
                patientAge = 20,
                patientAgencyID = "ABCDE",
                timeOpened = opened,
                regionID = _region.RegionID,
                requestorTypeID = _rType.RequestorTypeID
            };

            QuestionResponseContent qrCon = new QuestionResponseContent {
                question = "Test Question",
                response = "Test Response",
                timeSpent = 10,
                specialNotes = "Test Special Notes",
                questionTypeID = _qType.QuestionTypeID,
                tumourGroupID = _tGroup.TumourGroupID,
                severity = 0,
                consequence = 0
            };

            // Test with new and existing keyword
            qrCon.addKeyword("TRMC_Keyword0");
            //qrCon.addKeyword("TRMC_Keyword1");

            //_db.Keywords.InsertOnSubmit(new Keyword {
            //    Active = false,
            //    KeywordValue = "TRMC_Keyword0"
            //});
            //_db.SubmitChanges();

            qrCon.addReference(new ReferenceContent {
                referenceString = "TRMC_Reference1",
                referenceType = Constants.ReferenceType.Text
            });

            rCon.addQuestionResponse(qrCon);

            _reqId = _rmc.create(rCon);

            Request req = (from r in _db.Requests
                           where r.RequestID == _reqId
                           select r)
                           .Single();

            Assert.AreEqual(req.RequestStatus, (byte) rCon.requestStatus);
            Assert.AreEqual(req.RequestorFName, rCon.requestorFirstName);
            Assert.AreEqual(req.RequestorLName, rCon.requestorLastName);
            Assert.AreEqual(req.RequestorEmail, rCon.requestorEmail);
            Assert.AreEqual(req.RequestorPhone, rCon.requestorPhoneNum);
            Assert.AreEqual(req.RequestorPhoneExt, rCon.requestorPhoneExt);
            Assert.AreEqual(req.PatientFName, rCon.patientFName);
            Assert.AreEqual(req.PatientLName, rCon.patientLName);
            Assert.AreEqual(req.PatientGender, (byte) rCon.patientGender);
            Assert.AreEqual(req.PatientAge, rCon.patientAge);
            Assert.AreEqual(req.PatientAgencyID, rCon.patientAgencyID);

            Assert.That(rCon.timeOpened, Is.EqualTo(req.TimeOpened).Within(1).Seconds);
            Assert.Null(req.TimeClosed);

            Assert.AreEqual(req.RequestorTypeID, rCon.requestorTypeID);
            Assert.AreEqual(req.RegionID, rCon.regionID);

            QuestionResponse qr = (from q in _db.QuestionResponses
                                   where q.RequestID == _reqId
                                   select q)
                                   .Single();

            Assert.AreEqual(qr.Question, qrCon.question);
            Assert.AreEqual(qr.Response, qrCon.response);
            Assert.AreEqual(qr.TimeSpent, qrCon.timeSpent);
            Assert.AreEqual(qr.SpecialNotes, qrCon.specialNotes);
            Assert.AreEqual(qr.TumourGroupID, qrCon.tumourGroupID);
            Assert.AreEqual(qr.QuestionTypeID, qrCon.questionTypeID);
            Assert.AreEqual(qr.Severity, (byte) qrCon.severity);
            Assert.AreEqual(qr.Consequence, (byte) qrCon.consequence);

            Reference[] rArr =
                (from r in _db.References
                 where r.RequestID == _reqId &&
                       r.QuestionResponseID == qr.QuestionResponseID
                 select r)
                    .ToArray();

            Assert.AreEqual(rArr.Length, qrCon.referenceList.Count);
            Assert.AreEqual(rArr[0].ReferenceType,
                            (byte) qrCon.referenceList.ElementAt(0).referenceType);
            Assert.AreEqual(rArr[0].ReferenceString,
                            qrCon.referenceList.ElementAt(0).referenceString);

            KeywordQuestion[] kqArr =
                (from kq in _db.KeywordQuestions
                 where kq.RequestID == _reqId &&
                       kq.QuestionResponseID == qr.QuestionResponseID
                 select kq)
                    .ToArray();

            Assert.AreEqual(kqArr.Length, qrCon.keywords.Count);

            Keyword keyword =
                (from kw in _db.Keywords
                 where kw.KeywordID == kqArr[0].KeywordID
                 select kw)
                    .Single();
            Assert.AreEqual(keyword.KeywordValue,
                            qrCon.keywords.ElementAt(0));

            //keyword =
            //    (from kw in _db.Keywords
            //     where kw.KeywordID == kqArr[1].KeywordID
            //     select kw)
            //        .Single();
            //Assert.AreEqual(keyword.KeywordValue,
            //                qrCon.keywords.ElementAt(1));
        }

        [Test]
        public void Test_getRequestDetails() {
            // Should return null
            Assert.Null(_rmc.getRequestDetails(-1));

            // Check request is actually returned
            DateTime opened = DateTime.Now;

            RequestContent rCon = new RequestContent {
                requestStatus = Constants.RequestStatus.Open,
                requestorFirstName = "Bob",
                requestorLastName = "Smith",
                requestorEmail = "bsmith@gmail.com",
                requestorPhoneNum = "123-456-7890",
                requestorPhoneExt = "0000",
                patientFName = "Jane",
                patientLName = "Doe",
                patientGender = Constants.Gender.Female,
                patientAge = 20,
                patientAgencyID = "ABCDE",
                timeOpened = opened,
                regionID = _region.RegionID,
                requestorTypeID = _rType.RequestorTypeID
            };

            QuestionResponseContent qrCon = new QuestionResponseContent {
                question = "Test Question",
                response = "Test Response",
                timeSpent = 10,
                specialNotes = "Test Special Notes",
                questionTypeID = _qType.QuestionTypeID,
                tumourGroupID = _tGroup.TumourGroupID,
                severity = 0,
                consequence = 0
            };

            qrCon.addKeyword("TRMC_Keyword1");
            qrCon.addKeyword("TRMC_Keyword2");

            qrCon.addReference(new ReferenceContent {
                referenceString = "TRMC_Reference1",
                referenceType = Constants.ReferenceType.Text
            });

            rCon.addQuestionResponse(qrCon);

            _reqId = _rmc.create(rCon);

            RequestContent rCon2 = _rmc.getRequestDetails((long) _reqId);

            Assert.NotNull(rCon);
            Assert.AreEqual(rCon.requestStatus, rCon2.requestStatus);
            Assert.AreEqual(rCon.requestorFirstName, rCon2.requestorFirstName);
            Assert.AreEqual(rCon.requestorLastName, rCon2.requestorLastName);
            Assert.AreEqual(rCon.requestorEmail, rCon2.requestorEmail);
            Assert.AreEqual(rCon.requestorPhoneNum, rCon2.requestorPhoneNum);
            Assert.AreEqual(rCon.requestorPhoneExt, rCon2.requestorPhoneExt);
            Assert.AreEqual(rCon.patientFName, rCon2.patientFName);
            Assert.AreEqual(rCon.patientLName, rCon2.patientLName);
            Assert.AreEqual(rCon.patientGender, rCon2.patientGender);
            Assert.AreEqual(rCon.patientAge, rCon2.patientAge);
            Assert.AreEqual(rCon.patientAgencyID, rCon2.patientAgencyID);

            Assert.That(rCon.timeOpened, Is.EqualTo(rCon2.timeOpened).Within(1).Seconds);
            Assert.Null(rCon2.timeClosed);

            Assert.AreEqual(rCon.requestorTypeID, rCon2.requestorTypeID);
            Assert.AreEqual(rCon.regionID, rCon2.regionID);

            Assert.AreEqual(rCon.questionResponseList.Count,
                            rCon2.questionResponseList.Count);

            QuestionResponseContent qrCon2 =
                rCon2.questionResponseList.ElementAt(0);

            Assert.AreEqual(qrCon.question, qrCon2.question);
            Assert.AreEqual(qrCon.response, qrCon2.response);
            Assert.AreEqual(qrCon.timeSpent, qrCon2.timeSpent);
            Assert.AreEqual(qrCon.specialNotes, qrCon2.specialNotes);
            Assert.AreEqual(qrCon.tumourGroupID, qrCon2.tumourGroupID);
            Assert.AreEqual(qrCon.questionTypeID, qrCon2.questionTypeID);
            Assert.AreEqual(qrCon.severity, qrCon2.severity);
            Assert.AreEqual(qrCon.consequence, qrCon2.consequence);

            Assert.AreEqual(qrCon.referenceList.Count,
                            qrCon2.referenceList.Count);
            Assert.AreEqual(qrCon.referenceList.ElementAt(0).referenceType,
                            qrCon2.referenceList.ElementAt(0).referenceType);
            Assert.AreEqual(qrCon.referenceList.ElementAt(0).referenceString,
                            qrCon2.referenceList.ElementAt(0).referenceString);

            Assert.AreEqual(qrCon.keywords.Count, qrCon2.keywords.Count);

            for (int i = 0; i < qrCon.keywords.Count; i++) {
                Assert.AreEqual(qrCon.keywords.ElementAt(i),
                                qrCon2.keywords.ElementAt(i));
            }
        }
    }
}
