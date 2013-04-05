using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.Search;
using SasquatchCAIRS.Controllers.Service;
using SasquatchCAIRS.Models.Common;
using SasquatchCAIRS.Models.Search;
using NUnit.Framework;
using SasquatchCAIRS.Models.Service;
using Assert = NUnit.Framework.Assert;


namespace CAIRSTestProject.Unit
{
    [TestFixture]
    class TestSearchManagementController
    {
        private CAIRSDataContext _dc = new CAIRSDataContext();
        private Request _rq;
        private Request _rq2;
        private Request _rq3;
        private long _randomRequestInt;
        private long _randomRequestInt2;
        private long _randomRequestInt3;
        private Keyword _randomKeyword;
        private Keyword _randomKeyword2;
        private Keyword _randomKeyword3;
        private Keyword _randomKeyword4;
        private QuestionResponse testQrc;
        private QuestionResponse testQrc2;
        private QuestionResponse testQrc3;



        [TestFixtureSetUp]
        public void setUp()
        {
            Random random = new Random();
            _randomRequestInt = random.Next(1, 100000000);
            _randomRequestInt2 = random.Next(1, 10000000);
            _randomRequestInt3 = random.Next(1, 100000000);
            _randomKeyword = new Keyword
            {
                KeywordID = random.Next(1,100000000),
                KeywordValue = ("TSMC" + random.Next(1, 100000000).ToString(CultureInfo.InvariantCulture))
            };
            _randomKeyword2 = new Keyword
            {
                KeywordID = random.Next(1,100000000),
                KeywordValue = ("TSMC" + random.Next(1, 100000000).ToString(CultureInfo.InvariantCulture))
            };
            _randomKeyword3 = new Keyword
            {
                 KeywordValue = ("TSMC" + random.Next(1, 100000000).ToString(CultureInfo.InvariantCulture))
            };
            _randomKeyword4 = new Keyword
            {
                KeywordID = random.Next(1,100000000),
                KeywordValue = ("TSMC" + random.Next(1, 100000000).ToString(CultureInfo.InvariantCulture))
            };

            QuestionResponseContent testQrc = new QuestionResponseContent();
            testQrc.requestID = _randomRequestInt;
            testQrc.keywords.Add(_randomKeyword.KeywordValue);
            testQrc.keywords.Add(_randomKeyword2.KeywordValue);
            testQrc.severity = Constants.Severity.Minor;
            testQrc.consequence = Constants.Consequence.Certain;
            



            QuestionResponseContent testQrc2 = new QuestionResponseContent();
            testQrc2.requestID = _randomRequestInt2;
            testQrc2.keywords.Add(_randomKeyword3.KeywordValue);
            testQrc2.severity = Constants.Severity.Moderate;
            testQrc2.consequence = Constants.Consequence.Possible;


            QuestionResponseContent testQrc3 = new QuestionResponseContent();
            testQrc2.requestID = _randomRequestInt3;
            testQrc2.keywords.Add(_randomKeyword4.KeywordValue);
            testQrc.severity = Constants.Severity.Major;
            testQrc3.consequence = Constants.Consequence.Possible;


            RequestManagementController rmc = new RequestManagementController();
            RequestManagementController rmc2 = new RequestManagementController();
            RequestManagementController rmc3 = new RequestManagementController();

            rmc.create(new RequestContent
            {
                patientFName = "TSMC Test",
                questionResponseList = { testQrc, testQrc3 },
                requestStatus = Constants.RequestStatus.Open,
                requestorFirstName = "10",
                requestorLastName = "100",
                
                



            });

            rmc3.create(new RequestContent
            {
                patientFName = "TSMC Test2",
                requestorFirstName = "20",
                requestorLastName = "100",
                questionResponseList = { testQrc2, testQrc },
                requestStatus = Constants.RequestStatus.Open

            });

            rmc2.create(new RequestContent
            {
                patientFName = "TSMC Test3",
                requestorFirstName = "30",
                requestorLastName = "100",
                questionResponseList = { testQrc3 },
                requestStatus = Constants.RequestStatus.Open

            });

            _rq = _dc.Requests.FirstOrDefault(
                request =>
                request.PatientFName == "TSMC Test");

            _rq2 = _dc.Requests.FirstOrDefault(
                request =>
                request.PatientFName == "TSMC Test2");

            _rq3 = _dc.Requests.FirstOrDefault(
                request =>
                request.PatientFName == "TSMC Test3");


        }

        [TestFixtureTearDown]
        public void TearDown()
        {            _dc.Keywords.Attach(_randomKeyword);
            _dc.Keywords.Attach(_randomKeyword2);
            _dc.Keywords.Attach(_randomKeyword3);
            _dc.Keywords.Attach(_randomKeyword4);
            _dc.Keywords.DeleteOnSubmit(_randomKeyword);
            _dc.Keywords.DeleteOnSubmit(_randomKeyword2);
            _dc.Keywords.DeleteOnSubmit(_randomKeyword3);
            _dc.Keywords.DeleteOnSubmit(_randomKeyword4);
            _dc.QuestionResponses.Attach(testQrc);
            _dc.QuestionResponses.Attach(testQrc2);
            _dc.QuestionResponses.Attach(testQrc3);
            _dc.Requests.Attach(_rq);
            _dc.Requests.Attach(_rq2);
            _dc.Requests.Attach(_rq3);
            _dc.QuestionResponses.DeleteOnSubmit(testQrc);
            _dc.QuestionResponses.DeleteOnSubmit(testQrc2);
            _dc.QuestionResponses.DeleteOnSubmit(testQrc3);
            _dc.Requests.DeleteOnSubmit(_rq);
            _dc.Requests.DeleteOnSubmit(_rq2);
            _dc.Requests.DeleteOnSubmit(_rq3);

            

            _dc.SubmitChanges();
        }


        [Test]
        public void Test_isEmptySearchCriteriaEmpty() {

            SearchManagementController searchCon =
                new SearchManagementController();
            SearchCriteria sc = new SearchCriteria {
                anyKeywordString = null,
                allKeywordString = null,
                noneKeywordString = null,
                patientFirstName = null,
                patientLastName = null,
                questionType = null,
                requestorFirstName = null,
                requestorLastName = null,
                requestStatus = null,
                tumorGroup = null,
                severity = null,
                consequence = null,
            };
            
            bool isCriteriaEmpty = searchCon.isEmptySearchCriteria(sc);
            Assert.IsTrue(isCriteriaEmpty);
        }

        [Test]
        public void Test_isEmptySearchCriteriaNotEmpty(){
            SearchManagementController searchCon = new SearchManagementController();
        

            SearchCriteria s = new SearchCriteria
            {
                anyKeywordString = "Chemotherapy",
                allKeywordString = null,
                noneKeywordString = null,
                patientFirstName = null,
                patientLastName = null,
                questionType = null,
                requestorFirstName = null,
                requestorLastName = null,
                requestStatus = null,
                tumorGroup = null,
                severity = null,
                consequence = null,
            };

            bool criteriaEmpty = searchCon.isEmptySearchCriteria(s);
            Assert.IsFalse(criteriaEmpty);


        }

        [Test]
        public void Test_constructCriteriaString()
        {
            SearchManagementController searchCon = new SearchManagementController();
            SearchCriteria sc = new SearchCriteria
            {
                anyKeywordString = "Chemotherapy, Drug Interaction",
                allKeywordString = "Advil",
                noneKeywordString = "Pain",
                patientFirstName = "Kurt",
                patientLastName = "Eiselt",
                startTime = new DateTime(2013, 2, 1),
                completionTime = new DateTime(2013, 4, 1),
                requestorFirstName = "Jing",
                requestorLastName = "Zhu",
                requestStatus = "Open",
                severity = "Major",
                consequence = "Possible",
            };
            List<string> cs = searchCon.constructCriteriaString(sc);
            Assert.IsNotNull(cs);
            Assert.AreEqual(cs.Count, 12);
            List<string> temp = new List<string>();
            temp.Add("Any of These Keywords: Chemotherapy, Drug Interaction");
            temp.Add("All of These Keywords: Advil");
            temp.Add("None of These Keywords: Pain");
            temp.Add("Start Time: 2/1/2013");
            temp.Add("Completed Time: 4/1/2013");
            temp.Add("Status: Open");
            temp.Add("First Name: Jing");
            temp.Add("Last Name: Zhu");
            temp.Add("First Name: Kurt");
            temp.Add("Last Name: Eiselt");
            temp.Add("Severity: Major");
            temp.Add("Probability of Consequence: Possible");

            Assert.AreEqual(cs.Count, temp.Count);
            for (int i = 0; i < temp.Count; i++)
            {
                Assert.AreEqual(cs[i], temp[i]);
            }





        }

        [Test]
        public void Test_keywordsToList()
        {
            SearchManagementController searchCon = new SearchManagementController();
            String searchString = "Chemotherapy, pain, advil, drug dose";
            List<string> searchList = searchCon.keywordsToList(searchString, ",");
            Assert.IsNotNull(searchList);
            Assert.AreEqual(searchList.Count, 4);
            Assert.IsTrue(String.Equals(searchList[0], "Chemotherapy"));
            Assert.IsTrue(String.Equals(searchList[1], "pain"));
            Assert.IsTrue(String.Equals(searchList[2], "advil"));
            Assert.IsTrue(String.Equals(searchList[3], "drug dose"));


        }

        [Test]
        public void Test_getKeywords()
        {

            SearchManagementController searchCon = new SearchManagementController();

            List<int> kwIDs = searchCon.getKeywords(_randomKeyword.KeywordValue + "," + _randomKeyword2.KeywordValue + ", 57309457");
            Assert.AreEqual(kwIDs[0],  (from k in _dc.Keywords
                                           where k.KeywordValue == _randomKeyword.KeywordValue
                                           select k.KeywordID).First());
            Assert.AreEqual(kwIDs[1], (from k in _dc.Keywords
                                           where k.KeywordValue == _randomKeyword2.KeywordValue
                                           select k.KeywordID).First());
            Assert.AreEqual(kwIDs.Count, 2);



        }

        [Test]
        public void Test_enumToIDs()
        {
            SearchManagementController searchCon = new SearchManagementController();

            List<int> severityIDs = searchCon.enumToIDs("Major, Minor, Moderate",
                                                    typeof(Constants.Severity));
            Assert.AreEqual(severityIDs[0], 0);
            Assert.AreEqual(severityIDs[1], 2);
            Assert.AreEqual(severityIDs[2], 1);

            List<int> consequenceIDs =
                searchCon.enumToIDs("Probable, Unlikely, Certain, Possible", typeof(Constants.Consequence));

            Assert.AreEqual(consequenceIDs[0], 1);
            Assert.AreEqual(consequenceIDs[1], 3);
            Assert.AreEqual(consequenceIDs[2], 0);
            Assert.AreEqual(consequenceIDs[3], 2);
        }

        [Test]
        public void Test_emptyButValidKeywordsValidKw()
        {
            SearchManagementController searchCon = new SearchManagementController();

            SearchCriteria validKey = new SearchCriteria
            {
                anyKeywordString = _randomKeyword.KeywordValue,
                allKeywordString = null,
                noneKeywordString = null,
                patientFirstName = null,
                patientLastName = null,
                questionType = null,
                requestorFirstName = null,
                requestorLastName = null,
                requestStatus = null,
                tumorGroup = null,
                severity = null,
                consequence = null,
            };

            bool keywordValid = searchCon.emptyButValidKeywords(validKey);
            Assert.IsFalse(keywordValid);
        }

        [Test]
        public void Test_emptyButValidKeywordsInvalidKw()
        {
            SearchManagementController searchCon = new SearchManagementController();
            SearchCriteria invalidKey = new SearchCriteria
            {
                anyKeywordString = null,
                allKeywordString = "239048230-948!@#",
                noneKeywordString = null,
                patientFirstName = null,
                patientLastName = null,
                questionType = null,
                requestorFirstName = null,
                requestorLastName = null,
                requestStatus = null,
                tumorGroup = null,
                severity = null,
                consequence = null,
            };

            bool keywordInvalid = searchCon.emptyButValidKeywords(invalidKey);
            Assert.IsTrue(keywordInvalid);
        }

        [Test]
        public void Test_emptyButValidKeywordWithOneValid()
        {

            SearchManagementController searchCon = new SearchManagementController();
            SearchCriteria oneKeyInvalid = new SearchCriteria
            {
                anyKeywordString = _randomKeyword.KeywordValue,
                allKeywordString = "3-09258340598!@#$",
                noneKeywordString = null,
                patientFirstName = null,
                patientLastName = null,
                questionType = null,
                requestorFirstName = null,
                requestorLastName = null,
                requestStatus = null,
                tumorGroup = null,
                severity = null,
                consequence = null,
            };

            bool oneKeywordInvalid =
                searchCon.emptyButValidKeywords(oneKeyInvalid);
            Assert.IsFalse(oneKeywordInvalid);
        }


        [Test]
        public void Test_typeIDStringToList()
        {
            SearchManagementController searchCon = new SearchManagementController();
            List<int> intIDs = searchCon.typeIdStringtoList("1,2,3,40,1000000", ",");
            Assert.AreEqual(intIDs[0], 1);
            Assert.AreEqual(intIDs[1], 2);
            Assert.AreEqual(intIDs[2], 3);
            Assert.AreEqual(intIDs[3], 40);
            Assert.AreEqual(intIDs[4], 1000000);
        }

        [Test]
        public void Test_searchCriteriaQueryAllKeywords()
        {

            SearchManagementController searchCon = new SearchManagementController();

            SearchCriteria s = new SearchCriteria

            {
                anyKeywordString = null,
                allKeywordString = _randomKeyword.KeywordValue + "," + _randomKeyword2.KeywordValue,
                noneKeywordString = null,
                patientFirstName = null,
                patientLastName = null,
                questionType = null,
                requestorFirstName = null,
                requestorLastName = null,
                requestStatus = null,
                tumorGroup = null,
                severity = null,
                consequence = null,
            };
            List<Request> results = searchCon.searchCriteriaQuery(s);
            Assert.AreEqual(results.Count, 2);
            Assert.AreEqual(results[0].PatientFName, "TSMC Test2");
            Assert.AreEqual(results[0].RequestorFName, "20");
            Assert.AreEqual(results[0].RequestorLName, "100");
            Assert.AreEqual(results[1].PatientFName, "TSMC Test");
            Assert.AreEqual(results[1].RequestorFName, "10");
            Assert.AreEqual(results[1].RequestorLName, "100");

        }

        [Test]
        public void Test_searchCriteriaQueryAnyKeywords()
        {
            SearchManagementController searchCon = new SearchManagementController();

            SearchCriteria s = new SearchCriteria

            {
                anyKeywordString = _randomKeyword4.KeywordValue,
                allKeywordString = null,
                noneKeywordString = null,
                patientFirstName = null,
                patientLastName = null,
                questionType = null,
                requestorFirstName = null,
                requestorLastName = null,
                requestStatus = null,
                tumorGroup = null,
                severity = null,
                consequence = null,
            };
            List<Request> results = searchCon.searchCriteriaQuery(s);
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0].PatientFName, "TSMC Test2");
            Assert.AreEqual(results[0].RequestorFName, "20");
            Assert.AreEqual(results[0].RequestorLName, "100");

        }

        [Test]
        public void Test_searchCriteriaQueryCombineKeywords()
        {
            SearchManagementController searchCon = new SearchManagementController();

            SearchCriteria s = new SearchCriteria

            {
                anyKeywordString = null,
                allKeywordString = _randomKeyword.KeywordValue + "," + _randomKeyword2.KeywordValue,
                noneKeywordString = _randomKeyword4.KeywordValue,
                patientFirstName = null,
                patientLastName = null,
                questionType = null,
                requestorFirstName = null,
                requestorLastName = null,
                requestStatus = null,
                tumorGroup = null,
                severity = null,
                consequence = null,
            };
            List<Request> results = searchCon.searchCriteriaQuery(s);
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0].PatientFName, "TSMC Test");
            Assert.AreEqual(results[0].RequestorFName, "10");
            Assert.AreEqual(results[0].RequestorLName, "100");
        }

        [Test]
        public void Test_searchCriteriaQueryPatientName()
        {
            SearchManagementController searchCon = new SearchManagementController();

            SearchCriteria s = new SearchCriteria

            {
                anyKeywordString = null,
                allKeywordString = null,
                noneKeywordString = null,
                patientFirstName = "TSMC Test3",
                patientLastName = null,
                questionType = null,
                requestorFirstName = null,
                requestorLastName = null,
                requestStatus = null,
                tumorGroup = null,
                severity = null,
                consequence = null,
            };
            List<Request> results = searchCon.searchCriteriaQuery(s);
            // Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0].PatientFName, "TSMC Test3");
            Assert.AreEqual(results[0].RequestorFName, "30");
            Assert.AreEqual(results[0].RequestorLName, "100");
        }

        [Test]
        public void Test_searchCriteriaQueryRequestorName()
        {
            SearchManagementController searchCon = new SearchManagementController();

            SearchCriteria s = new SearchCriteria

            {
                anyKeywordString = null,
                allKeywordString = null,
                noneKeywordString = null,
                patientFirstName = null,
                patientLastName = null,
                questionType = null,
                requestorFirstName = "20",
                requestorLastName = "100",
                requestStatus = null,
                tumorGroup = null,
                severity = null,
                consequence = null,
            };
            List<Request> results = searchCon.searchCriteriaQuery(s);
            // Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0].PatientFName, "TSMC Test2");
            Assert.AreEqual(results[0].RequestorFName, "20");
            Assert.AreEqual(results[0].RequestorLName, "100");
        }

        [Test]
        public void Test_searchCriteriaQuerySeverity()
        {
            SearchManagementController searchCon = new SearchManagementController();

            SearchCriteria s = new SearchCriteria

            {
                anyKeywordString = null,
                allKeywordString = null,
                noneKeywordString = null,
                patientFirstName = null,
                patientLastName = null,
                questionType = null,
                requestorFirstName = null,
                requestorLastName = "100",
                requestStatus = null,
                tumorGroup = null,
                severity = "Moderate",
                consequence = null,
            };
            List<Request> results = searchCon.searchCriteriaQuery(s);
            //  Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0].PatientFName, "TSMC Test2");
            Assert.AreEqual(results[0].RequestorFName, "20");
            Assert.AreEqual(results[0].RequestorLName, "100");
            Assert.AreEqual(results[0].QuestionResponses[0].Severity, 1);
        }

        [Test]
        public void Test_searchCriteriaQueryConsequence()
        {
            SearchManagementController searchCon = new SearchManagementController();

            SearchCriteria s = new SearchCriteria

            {
                anyKeywordString = null,
                allKeywordString = null,
                noneKeywordString = null,
                patientFirstName = null,
                patientLastName = null,
                questionType = null,
                requestorFirstName = "10",
                requestorLastName = null,
                requestStatus = null,
                tumorGroup = null,
                severity = null,
                consequence = "Certain",
            };
            List<Request> results = searchCon.searchCriteriaQuery(s);
            //  Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0].PatientFName, "TSMC Test");
            Assert.AreEqual(results[0].RequestorFName, "10");
            Assert.AreEqual(results[0].RequestorLName, "100");
            Assert.AreEqual(results[0].QuestionResponses[0].Consequence, 0);
        }


    }
}