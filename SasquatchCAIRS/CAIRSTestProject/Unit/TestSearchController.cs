using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models.SearchSystem;
using SasquatchCAIRS.Models;
using NUnit.Framework;
using SasquatchCAIRS.Models.ServiceSystem;
using Assert = NUnit.Framework.Assert;


namespace CAIRSTestProject.Unit
{
    [TestFixture]
    class TestSearchController
    {
        private CAIRSDataContext _dc = new CAIRSDataContext();
        private Request rq;
        private Request rq2;
        private Request rq3;

        [TestFixtureSetUp]
        public void setUp()
        {
            Random random = new Random();
            long randomRequestInt = random.Next(1, 100000000);
            long randomRequestInt2 = random.Next(1, 10000000);
            long randomRequestInt3 = random.Next(1, 100000000);
            long randomKeyword = random.Next(1, 100000000);
            long randomKeyword2 = random.Next(1, 100000000);
            long randomKeyword3 = random.Next(1, 100000000);
            long randomKeyword4 = random.Next(1, 100000000);

            QuestionResponseContent testQrc = new QuestionResponseContent();
            testQrc.requestID = randomRequestInt;
            testQrc.keywords.Add("TSMC abc");
            testQrc.keywords.Add("TSMC abcd");

            QuestionResponseContent testQrc2 = new QuestionResponseContent();
            testQrc2.requestID = randomRequestInt2;
            testQrc2.keywords.Add("TSMC abcde");
           // testQrc2.keywords.Add(randomKeyword2.ToString(CultureInfo.InvariantCulture));
           // testQrc2.keywords.Add(randomKeyword3.ToString(CultureInfo.InvariantCulture));

            QuestionResponseContent testQrc3 = new QuestionResponseContent();
            testQrc2.requestID = randomRequestInt3;
          //  testQrc2.keywords.Add(randomKeyword.ToString(CultureInfo.InvariantCulture));
            testQrc2.keywords.Add("TSMC abcd");
            testQrc2.keywords.Add("TSMC aabc" );

            RequestManagementController rmc = new RequestManagementController();
            RequestManagementController rmc2 = new RequestManagementController();
            RequestManagementController rmc3 = new RequestManagementController();

            rmc.create(new RequestContent
            {
                patientFName = "Jing's Test",
                questionResponseList = { testQrc },
                requestStatus = Constants.RequestStatus.Open,
                requestorFirstName = "vlad",
                requestorLastName = "zaharia",
                
                

            });

            rmc3.create(new RequestContent
            {
                patientFName = "Jing's Test2",

                questionResponseList = { testQrc2 },
                requestStatus = Constants.RequestStatus.Open

            });

            rmc2.create(new RequestContent
            {
                patientFName = "Jing's Test3",

                questionResponseList = { testQrc3 },
                requestStatus = Constants.RequestStatus.Open

            });

            rq = _dc.Requests.FirstOrDefault(
                request =>
                request.PatientLName == "Jing's Test");

            rq2 = _dc.Requests.FirstOrDefault(
                request =>
                request.PatientLName == "Jing's Test2");

            rq3 = _dc.Requests.FirstOrDefault(
                request =>
                request.PatientLName == "Jing's Test3");


        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _dc.Requests.DeleteOnSubmit(rq);
            _dc.Requests.DeleteOnSubmit(rq2);
            _dc.Requests.DeleteOnSubmit(rq3);
            _dc.SubmitChanges();
        }


        [Test]
        public void Test_isEmptySearchCriteria()
        {

            SearchController searchCon = new SearchController();
            SearchCriteria sc = new SearchCriteria
            {
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
            SearchController searchCon = new SearchController();
            SearchCriteria sc = new SearchCriteria
            {
                anyKeywordString = "Chemotherapy, Drug Interaction",
                allKeywordString = "Advil",
                noneKeywordString = "Pain",
                patientFirstName = "Kurt",
                patientLastName = "Eiselt",
                startTime = new DateTime(0),
                completionTime = new DateTime(0),
                //questionType = "ADMIN",
                requestorFirstName = "Vlad",
                requestorLastName = "Zaharia",
                requestStatus = "Open",
                //tumorGroup = "ME",
                severity = "Major",
                consequence = "Possible",
            };
            List<string> cs = searchCon.constructCriteriaString(sc);
            Assert.IsNotNull(cs);
            Assert.AreEqual(cs.Count, 10);
            List<string> temp = new List<string>();
            temp.Add("Any of These Keywords: Chemotherapy, Drug Interaction");
            temp.Add("All of These Keywords: Advil");
            temp.Add("None of These Keywords: Pain");
            temp.Add("Status: Open");
            temp.Add("First Name: Vlad");
            temp.Add("Last Name: Zaharia");
            temp.Add("First Name: Kurt");
            temp.Add("Last Name: Eiselt");
            temp.Add("Severity: Major");
            temp.Add("Probability of Consequence: Possible");
            //          temp.Add("All of These Keywords: advil");
            Assert.AreEqual(cs.Count, temp.Count);
            for (int i = 0; i < temp.Count; i++)
            {
                Assert.AreEqual(cs[i], temp[i]);
            }





        }

        [Test]
        public void Test_keywordsToList()
        {
            SearchController searchCon = new SearchController();
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

            SearchController searchCon = new SearchController();

            List<int> kwIDs = searchCon.getKeywords("chemotherapy, advil, pain, donkey");
            foreach (var kwID in kwIDs) {
                Console.Write(kwID);
            }
            Assert.AreEqual(kwIDs[0], 82);
            Assert.AreEqual(kwIDs[1], 94);
            Assert.AreEqual(kwIDs[2], 47);
            Assert.AreEqual(kwIDs.Count, 3);



        }

        [Test]
        public void Test_enumToIDs()
        {
            SearchController searchCon = new SearchController();

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
        public void Test_emptyButValidKeywordsWithValidKw()
        {
            SearchController searchCon = new SearchController();

            SearchCriteria validKey = new SearchCriteria
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

            bool keywordValid = searchCon.emptyButValidKeywords(validKey);
            Assert.IsFalse(keywordValid);
        }

        [Test]
        public void Test_emptyButValidKeywordsWithInvalidKw()
        {
            SearchController searchCon = new SearchController();
            SearchCriteria invalidKey = new SearchCriteria
            {
                anyKeywordString = null,
                allKeywordString = "donkey",
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

            SearchController searchCon = new SearchController();
            SearchCriteria oneKeyInvalid = new SearchCriteria
            {
                anyKeywordString = "Chemotherapy",
                allKeywordString = "donkey",
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
        public void Test_fillUpKeywordDict()
        {

        }

        [Test]
        public void Test_typeIDStringToList()
        {
            SearchController searchCon = new SearchController();
            List<int> intIDs = searchCon.typeIDStringtoList("1,2,3,40,1000000", ",");
            Assert.AreEqual(intIDs[0], 1);
            Assert.AreEqual(intIDs[1], 2);
            Assert.AreEqual(intIDs[2], 3);
            Assert.AreEqual(intIDs[3], 40);
            Assert.AreEqual(intIDs[4], 1000000);
        }

        [Test]
        public void Test_searchCriteriaQueryWithNoResults()
        {
            
            SearchController searchCon = new SearchController();
            
            SearchCriteria s = new SearchCriteria
            
            {
                anyKeywordString = " ",
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
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void Test_searchCriteriaQueryWithOneResult() {
            SearchController searchCon = new SearchController();
            
            SearchCriteria s = new SearchCriteria

            {
                anyKeywordString = null,
                allKeywordString = null,
                noneKeywordString = null,
                patientFirstName = "Jing's Test",
                patientLastName = null,
                questionType = null,
                requestorFirstName = "vlad",
                requestorLastName = null,
                requestStatus = "Open",
                tumorGroup = null,
                severity = "Major",
                consequence = null,
            };
            List<Request> results = searchCon.searchCriteriaQuery(s);
            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0].PatientFName, "Jing's Test");
            Assert.AreEqual(results[0].RequestorFName, "vlad");
            Assert.AreEqual(results[0].RequestorFName, "zaharia");
        }

    }
}
