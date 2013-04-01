using System;
using System.Globalization;
using System.Linq;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers;
using SasquatchCAIRS.Models.SearchSystem;
using SasquatchCAIRS.Models;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CAIRSTestProject.Unit
{
    [TestFixture]
    class TestSearchController
    {
        private CAIRSDataContext _dc = new CAIRSDataContext();

        [Test]
        public void Test_isEmptySearchCriteria() {

            SearchController searchCon = new SearchController();
            SearchCriteria sc = new SearchCriteria {
                keywordString = null,
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
                keywordString = "Chemotherapy",
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

    }
}
