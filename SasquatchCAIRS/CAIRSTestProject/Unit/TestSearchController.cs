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

        [TestFixtureSetUp]
        public void setUp() {
            Random random = new Random();
        long randomRequestInt = random.Next(1, 100000000);
        RequestManagementController rmc = new RequestManagementController();

            
        }
        

        [Test]
        public void Test_isEmptySearchCriteria() {

            SearchController searchCon = new SearchController();
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
        public void Test_constructCriteriaString() {
            SearchController searchCon = new SearchController();
            SearchCriteria sc = new SearchCriteria {
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
            temp.Add( "Status: Open");
            temp.Add("First Name: Vlad");
            temp.Add("Last Name: Zaharia");
            temp.Add("First Name: Kurt");
            temp.Add( "Last Name: Eiselt");
            temp.Add("Severity: Major");
            temp.Add( "Probability of Consequence: Possible");
  //          temp.Add("All of These Keywords: advil");
            Assert.AreEqual(cs.Count,temp.Count);
            for (int i = 0; i < temp.Count;i++ )
            {
                Assert.AreEqual(cs[i],temp[i]);
            }
           




        }

        [Test]
        public void Test_keywordsToList() {
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
        public void Test_getKeywords() {

            SearchController searchCon = new SearchController();
            
            List<int> kwIDs = searchCon.getKeywords("chemotherapy, advil, pain, donkey");
            Assert.AreEqual(kwIDs[0], 94);
            Assert.AreEqual(kwIDs[1], 82);
            Assert.AreEqual(kwIDs[2], 47);
            Assert.AreEqual(kwIDs.Count, 3);



        }

        [Test]
        public void Test_enumToIDs() {
            SearchController searchCon = new SearchController();

            List<int> severityIDs = searchCon.enumToIDs("Major, Minor, Moderate",
                                                    typeof (Constants.Severity));
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
        public void Test_emptyButValidKeywords() {
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
        public void Test_fillUpKeywordDict() {
            
        }

        [Test]
        public void Test_typeIDStringToList() {
            SearchController searchCon = new SearchController();
            List<int> intIDs = searchCon.typeIDStringtoList("1,2,3,40,1000000", ",");
            Assert.AreEqual(intIDs[0], 1);
            Assert.AreEqual(intIDs[1], 2);
            Assert.AreEqual(intIDs[2], 3);
            Assert.AreEqual(intIDs[3], 40);
            Assert.AreEqual(intIDs[4], 1000000);
        }

        [Test]
        public void Test_searchCriteriaQuery() {
            
        }

    }
}
