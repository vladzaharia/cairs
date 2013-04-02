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
            Keyword sampleKw1 = new Keyword {
                KeywordID = 0,
                KeywordValue = "test",
                Active = true
            };

            Keyword sampleKw2 = new Keyword
            {
                KeywordID = 1,
                KeywordValue = "test2",
                Active = true
            };

            Keyword sampleKw3 = new Keyword
            {
                KeywordID = 2,
                KeywordValue = "test3",
                Active = true
            };




        }

        [Test]
        public void Test_enumToIDs() {
            
        }

        [Test]
        public void Test_emptyButValidKeywords() {
            
        }

        [Test]
        public void Test_fillUpKeywordDict() {
            
        }

        [Test]
        public void Test_typeIDStringToList() {
            
        }

        [Test]
        public void Test_searchCriteriaQuery() {
            
        }

    }
}
