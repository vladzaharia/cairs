using System;
using System.Linq;
using NUnit.Framework;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.Service;
using SasquatchCAIRS.Models.Common;
using SasquatchCAIRS.Models.Service;

namespace CAIRSTestProject.Unit {
    [TestFixture]
    public class TestRequestManagementController {
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

            foreach (QuestionResponse qr in qrArr) {
                Reference[] rArr =
                    (from r in _db.References
                     where r.RequestID == _reqId &&
                           r.QuestionResponseID ==
                           qr.QuestionResponseID
                     select r)
                        .ToArray();

                foreach (Reference r in rArr) {
                    _db.References.DeleteOnSubmit(r);
                    _db.SubmitChanges();
                }

                KeywordQuestion[] kqArr =
                    (from kq in _db.KeywordQuestions
                     where kq.RequestID == _reqId &&
                           kq.QuestionResponseID == qr.QuestionResponseID
                     select kq)
                        .ToArray();

                foreach (KeywordQuestion kq in kqArr) {
                    int kqId = kq.KeywordID;

                    _db.KeywordQuestions.DeleteOnSubmit(kq);
                    _db.SubmitChanges();

                    if ((from kwQ in _db.KeywordQuestions
                         where kwQ.KeywordID == kqId
                         select kwQ).Any()) {
                        continue;
                    }

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

            var random = new Random();

            _up = new UserProfile {
                UserName = "TRMC" + random.Next(1, 100000000)
            };
            _db.UserProfiles.InsertOnSubmit(_up);
            _db.SubmitChanges();

            _rType = new RequestorType {
                Code = "TRMC" + random.Next(1, 100000),
                Value =
                    "TestRequestManagementController" +
                    random.Next(1, 100000000),
                Active = true
            };
            _db.RequestorTypes.InsertOnSubmit(_rType);
            _db.SubmitChanges();

            _region = new Region {
                Code = "TRMC" + random.Next(1, 100000),
                Value =
                    "TestRequestManagementController" +
                    random.Next(1, 100000000),
                Active = true
            };
            _db.Regions.InsertOnSubmit(_region);
            _db.SubmitChanges();

            _tGroup = new TumourGroup {
                Code = "TRMC" + random.Next(1, 100000),
                Value =
                    "TestRequestManagementController" +
                    random.Next(1, 100000000),
                Active = true
            };
            _db.TumourGroups.InsertOnSubmit(_tGroup);
            _db.SubmitChanges();

            _qType = new QuestionType {
                Code = "TRMC" + random.Next(1, 100000),
                Value =
                    "TestRequestManagementController" +
                    random.Next(1, 100000000),
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

        [Test]
        public void Test_create() {
            DateTime opened = DateTime.Now;

            var rCon = new RequestContent {
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

            var qrCon = new QuestionResponseContent {
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
            qrCon.addKeyword("TRMC_Keyword1");

            _db.ExecuteCommand(
                "INSERT INTO Keyword (KeywordValue, Active) " +
                "VALUES ('TRMC_Keyword0', 'False')");

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

            Assert.That(rCon.timeOpened,
                        Is.EqualTo(req.TimeOpened).Within(1).Seconds);
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
                            (byte)
                            qrCon.referenceList.ElementAt(0).referenceType);
            Assert.AreEqual(rArr[0].ReferenceString,
                            qrCon.referenceList.ElementAt(0).referenceString);

            KeywordQuestion[] kqArr =
                (from kq in _db.KeywordQuestions
                 where kq.RequestID == _reqId &&
                       kq.QuestionResponseID == qr.QuestionResponseID
                 select kq)
                    .ToArray();

            Assert.AreEqual(kqArr.Length, qrCon.keywords.Count);

            foreach (KeywordQuestion t in kqArr) {
                Keyword keyword =
                    (from kw in _db.Keywords
                     where kw.KeywordID == t.KeywordID
                     select kw)
                        .Single();

                Assert.True(qrCon.keywords.Contains(keyword.KeywordValue));
            }
        }

        [Test]
        public void Test_edit() {
            // Cannot use default request ID in RequestContent
            Assert.Throws<Exception>(() => _rmc.edit(new RequestContent()));

            // Cannot use non-existent request ID
            long nonexistent = _db.Requests.Max(x => x.RequestID) + 1;
            Assert.Throws<Exception>(() => _rmc.edit(new RequestContent {
                requestID = nonexistent
            }));

            // Create a new request
            var rCon = new RequestContent {
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
                timeOpened = DateTime.Now,
                regionID = _region.RegionID,
                requestorTypeID = _rType.RequestorTypeID
            };

            var qrCon1 = new QuestionResponseContent {
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
            qrCon1.addKeyword("TRMC_Keyword0");
            qrCon1.addKeyword("TRMC_Keyword1");

            qrCon1.addReference(new ReferenceContent {
                referenceString = "TRMC_Reference0",
                referenceType = Constants.ReferenceType.Text
            });

            rCon.addQuestionResponse(qrCon1);

            var qrCon2 = new QuestionResponseContent {
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
            qrCon2.addKeyword("TRMC_Keyword0");
            qrCon2.addKeyword("TRMC_Keyword2");

            qrCon2.addReference(new ReferenceContent {
                referenceString = "TRMC_Reference1",
                referenceType = Constants.ReferenceType.Text
            });
            qrCon2.addReference(new ReferenceContent {
                referenceString = "TRMC_Reference2",
                referenceType = Constants.ReferenceType.Text
            });

            rCon.addQuestionResponse(qrCon2);

            _reqId = _rmc.create(rCon);

            // Get saved RequestContent details
            rCon = _rmc.getRequestDetails((long) _reqId);

            // Edit request
            rCon.requestStatus = Constants.RequestStatus.Completed;
            rCon.requestorFirstName = "Jane";
            rCon.requestorLastName = "Doe";
            rCon.requestorEmail = "jdoe@gmail.com";
            rCon.requestorPhoneNum = "098-765-4321";
            rCon.requestorPhoneExt = "1111";
            rCon.patientFName = "Bob";
            rCon.patientLName = "Smith";
            rCon.patientGender = Constants.Gender.Male;
            rCon.patientAge = 30;
            rCon.patientAgencyID = "UVWXYZ";
            rCon.timeClosed = DateTime.Now;

            // Remove an existing QuestionResponse
            rCon.questionResponseList.RemoveAt(0);

            // Edit an existing QuestionResponse
            qrCon1 = rCon.questionResponseList.ElementAt(0);

            qrCon1.question = "Question Test";
            qrCon1.response = "Response Test";
            qrCon1.timeSpent = 30;
            qrCon1.specialNotes = "Special Notes Test";
            qrCon1.severity = Constants.Severity.Moderate;
            qrCon1.consequence = Constants.Consequence.Unlikely;

            // Remove an existing keyword from an existing QuestionResponse
            qrCon1.keywords.RemoveAt(0);

            // Add a new keyword to an existing QuestionResponse
            qrCon1.addKeyword("TRMC_Keyword3");

            // Remove an existing reference from an existing QuestionResponse
            qrCon1.referenceList.RemoveAt(0);

            // Edit an existing reference from an existing QuestionResponse
            qrCon1.referenceList.ElementAt(0).referenceString =
                "TRMC_Reference4";

            // Add a new reference to an existing QuestionResponse
            qrCon1.addReference(new ReferenceContent {
                referenceString = "TRMC_Reference5",
                referenceType = Constants.ReferenceType.Text
            });

            // Add a new QuestionResponse
            qrCon2 = new QuestionResponseContent {
                question = "Test Question New",
                response = "Test Response New",
                timeSpent = 10,
                specialNotes = "Test Special Notes New",
                questionTypeID = _qType.QuestionTypeID,
                tumourGroupID = _tGroup.TumourGroupID,
                severity = 0,
                consequence = 0
            };

            qrCon2.addKeyword("TRMC_Keyword0");
            qrCon2.addReference(new ReferenceContent {
                referenceString = "TRMC_Reference3",
                referenceType = Constants.ReferenceType.Text
            });

            rCon.addQuestionResponse(qrCon2);

            _rmc.edit(rCon);

            RequestContent rConEdited = _rmc.getRequestDetails((long) _reqId);

            Assert.NotNull(rCon);
            Assert.AreEqual(rCon.requestStatus, rConEdited.requestStatus);
            Assert.AreEqual(rCon.requestorFirstName,
                            rConEdited.requestorFirstName);
            Assert.AreEqual(rCon.requestorLastName, rConEdited.requestorLastName);
            Assert.AreEqual(rCon.requestorEmail, rConEdited.requestorEmail);
            Assert.AreEqual(rCon.requestorPhoneNum, rConEdited.requestorPhoneNum);
            Assert.AreEqual(rCon.requestorPhoneExt, rConEdited.requestorPhoneExt);
            Assert.AreEqual(rCon.patientFName, rConEdited.patientFName);
            Assert.AreEqual(rCon.patientLName, rConEdited.patientLName);
            Assert.AreEqual(rCon.patientGender, rConEdited.patientGender);
            Assert.AreEqual(rCon.patientAge, rConEdited.patientAge);
            Assert.AreEqual(rCon.patientAgencyID, rConEdited.patientAgencyID);

            Assert.That(rCon.timeOpened,
                        Is.EqualTo(rConEdited.timeOpened).Within(1).Seconds);
            Assert.That(rCon.timeClosed,
                        Is.EqualTo(rConEdited.timeClosed).Within(1).Seconds);

            Assert.AreEqual(rCon.requestorTypeID, rConEdited.requestorTypeID);
            Assert.AreEqual(rCon.regionID, rConEdited.regionID);

            Assert.AreEqual(rCon.questionResponseList.Count,
                            rConEdited.questionResponseList.Count);

            for (int i = 0; i < rCon.questionResponseList.Count; i++) {
                QuestionResponseContent qrConOrig =
                    rCon.questionResponseList.ElementAt(i);
                QuestionResponseContent qrConEdited =
                    rConEdited.questionResponseList.ElementAt(i);

                Assert.AreEqual(qrConOrig.question, qrConEdited.question);
                Assert.AreEqual(qrConOrig.response, qrConEdited.response);
                Assert.AreEqual(qrConOrig.timeSpent, qrConEdited.timeSpent);
                Assert.AreEqual(qrConOrig.specialNotes, qrConEdited.specialNotes);
                Assert.AreEqual(qrConOrig.tumourGroupID,
                                qrConEdited.tumourGroupID);
                Assert.AreEqual(qrConOrig.questionTypeID,
                                qrConEdited.questionTypeID);
                Assert.AreEqual(qrConOrig.severity, qrConEdited.severity);
                Assert.AreEqual(qrConOrig.consequence, qrConEdited.consequence);

                Assert.AreEqual(qrConOrig.referenceList.Count,
                                qrConEdited.referenceList.Count);

                for (int j = 0; j < qrConOrig.referenceList.Count; j++) {
                    Assert.AreEqual(
                        qrConOrig.referenceList.ElementAt(j).referenceType,
                        qrConEdited.referenceList.ElementAt(j)
                                   .referenceType);
                    Assert.AreEqual(
                        qrConOrig.referenceList.ElementAt(j).referenceString,
                        qrConEdited.referenceList.ElementAt(j)
                                   .referenceString);
                }

                Assert.AreEqual(qrConOrig.keywords.Count,
                                qrConEdited.keywords.Count);

                for (int k = 0; k < qrConOrig.keywords.Count; k++) {
                    Assert.True(
                        qrConOrig.keywords.Contains(
                            qrConEdited.keywords.ElementAt(k)));
                }
            }
        }

        [Test]
        public void Test_exists() {
            Assert.False(_rmc.requestExists(-1));

            _reqId = _rmc.create(new RequestContent());
            Assert.True(_rmc.requestExists((long) _reqId));
        }

        [Test]
        public void Test_getRequestDetails() {
            // Should return null
            Assert.Null(_rmc.getRequestDetails(-1));

            // Check request is actually returned
            DateTime opened = DateTime.Now;

            var rCon = new RequestContent {
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

            var qrCon = new QuestionResponseContent {
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
            qrCon.addKeyword("TRMC_Keyword1");

            _db.ExecuteCommand(
                "INSERT INTO Keyword (KeywordValue, Active) " +
                "VALUES ('TRMC_Keyword0', 'False')");

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

            Assert.That(rCon.timeOpened,
                        Is.EqualTo(rCon2.timeOpened).Within(1).Seconds);
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
                Assert.True(qrCon.keywords.Contains(qrCon2.keywords.ElementAt(i)));
            }
        }

        [Test]
        public void Test_invalidate() {
            _reqId = _rmc.create(new RequestContent());
            _rmc.invalidate((long) _reqId);

            RequestContent rCon = _rmc.getRequestDetails((long) _reqId);
            Assert.AreEqual(rCon.requestStatus, Constants.RequestStatus.Invalid);
        }
    }
}