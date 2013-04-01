using System;
using System.Collections.Generic;
using System.Linq;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.ServiceSystem;
using NUnit.Framework;
using Rhino.Mocks;
using Assert = NUnit.Framework.Assert;

namespace CAIRSTestProject {
    
    [TestFixture]
    public class ReportControllerTest {
        private IDataContext _iDataContext;

        [TestFixtureSetUp]
        public void setUp() {
            _iDataContext =
                MockRepository.GenerateStub<IDataContext>();
            _iDataContext.Stub(x => x.Repository<Request>())
                         .Return(Samples.getSampleRequests());
        }

        //private IQueryable<Request> getSampleRequests() {
        //    Request _request1 = new Request();
        //    _request1.TimeOpened = new DateTime(2013, 03, 01);
        //    Request _request2 = new Request();
        //    _request2.TimeOpened = new DateTime(2012, 04, 01);
        //    Request _request3 = new Request();
        //    _request3.TimeOpened = new DateTime(2013, 04, 05);
        //    List<Request> sampleRequests = new List<Request>();
        //    sampleRequests.Add(_request1);
        //    sampleRequests.Add(_request2);
        //    sampleRequests.Add(_request3);

        //    return sampleRequests.AsQueryable();
        //}


        [Test]
        public void checkForDataForFyTest() {
            ReportController reportC = new ReportController(_iDataContext);
            bool shouldBeTrue = reportC.checkForDataForFy(1980, 1982);
            bool shouldBeFalse = reportC.checkForDataForFy(2012, 2014);

            Assert.AreEqual(true, shouldBeTrue, "Data found for 1980-1982? Data shouldn't exist");
            Assert.AreEqual(false, shouldBeFalse, "Data not found for 2012-2014! Data should exist");
        }

        [Test]
        public void checkForDataForMonthTest() {
            ReportController reportC = new ReportController(_iDataContext);
            bool shouldBeFalse = reportC.checkForDataForMonth(new DateTime(2013, 01, 01),
                                         new DateTime(2013, 08, 01));
            bool shouldBeTrue = reportC.checkForDataForMonth(new DateTime(1980, 01, 01),
                                         new DateTime(1981, 08, 01));
            Assert.AreEqual(true, shouldBeTrue, "Data found for 1980 Jan-Apr?! Data shouldn't exist");
            Assert.AreEqual(false, shouldBeFalse, "Data is not found for 2013 Jan-Apr! Data should exist");
        }

        [Test]
        public void checkForDataForMpyTest() {
            ReportController reportC = new ReportController(_iDataContext);
            bool shouldBeFalse = reportC.checkForDataForMpy(4, 2011, 2014);
            bool shouldBeTrue = reportC.checkForDataForMpy(4, 1980, 1981);

            Assert.AreEqual(true, shouldBeTrue, "Data found for Mar 1980-1981?! Data shouldn't exist");
            Assert.AreEqual(false, shouldBeFalse,
                            "Data is not found for Mar 2011-2014! Data should exist");
        }
    }
}
