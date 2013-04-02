using System;
using System.Collections.Generic;
using System.Data;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.ServiceSystem;
using NUnit.Framework;
using Rhino.Mocks;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;
using Assert = NUnit.Framework.Assert;

namespace CAIRSTestProject {
    
    [TestFixture]
    public class ReportControllerTest {
        private IDataContext _iDataContext;
        private List<Constants.DataType> _dataTypes;

        [TestFixtureSetUp]
        public void setUp() {
            _iDataContext =
                MockRepository.GenerateStub<IDataContext>();
            _iDataContext.Stub(x => x.Repository<Request>())
                         .Return(Samples.getSampleRequests());
            _iDataContext.Stub(x => x.Repository<QuestionResponse>())
                         .Return(Samples.getSampleQs());
            _iDataContext.Stub(x => x.Repository<Region>())
                         .Return(Samples.getSampleRegions());
            _iDataContext.Stub(x => x.Repository<TumourGroup>())
                         .Return(Samples.getSampleTumourGroups());
            _iDataContext.Stub(x => x.Repository<RequestorType>())
                         .Return(Samples.getSampleCallerTypes());

            _dataTypes = new List<Constants.DataType> {
                Constants.DataType.AvgTimePerRequest,
                Constants.DataType.AvgTimeToComplete,
                Constants.DataType.TotalNumOfRequests,
                Constants.DataType.TotalTimeSpent
            };
        }

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

        [Test]
        public void generateMonthlyStratifiedByRegionTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
                reportC.generateMonthlyReport(new DateTime(2013, 02, 01),
                                              new DateTime(2013, 06, 01),
                                              _dataTypes,
                                              Constants.StratifyOption.Region);

            //DataTable expectedAvgTimeByRegion = new DataTable();
            //string stratifyGroupString = Enum.GetName(typeof(Constants.StratifyOption),
            //                 Constants.StratifyOption.Region);
            //expectedAvgTimeByRegion.Columns.Add(stratifyGroupString);
            //MonthYearPair startMonthYearPair = new MonthYearPair(02, 2013);
            //for (int i = 0; i < 4; i++) {
            //    DataColumn monthColumn = new DataColumn(startMonthYearPair.ToString(), typeof(Int64)) {
            //        DefaultValue = 0
            //    };
            //    expectedAvgTimeByRegion.Columns.Add(monthColumn);
            //    startMonthYearPair.addmonth(1);
            //}

            //DataRow noRegionRow = expectedAvgTimeByRegion.NewRow();
            //noRegionRow[stratifyGroupString] = "No " + stratifyGroupString;
            //noRegionRow["Feb/2013"] = 50;
            //noRegionRow["Mar/2013"] = 110;
            //expectedAvgTimeByRegion.Rows.Add(noRegionRow);

            //DataRow bcRow = expectedAvgTimeByRegion.NewRow();
            //bcRow[stratifyGroupString] = "BC";
            //bcRow["Feb/2013"] = 70;
            //bcRow["Mar/2013"] = 60;
            //bcRow["Apr/2013"] = 50;
            //expectedAvgTimeByRegion.Rows.Add(bcRow);

            //DataRow onRow = expectedAvgTimeByRegion.NewRow();
            //onRow[stratifyGroupString] = "ON";
            //onRow["Feb/2013"] = 90;
            //onRow["Apr/2013"] = 40;
            //expectedAvgTimeByRegion.Rows.Add(onRow);
            
            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[1]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[2]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[3]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[4]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeByRegion = results[Constants.DATATABLE_TITLES[1]];
            Assert.AreEqual(60, avgTimeByRegion.Rows[1][2]);
            Assert.AreEqual(40, avgTimeByRegion.Rows[2][4]);
            //Tests the table size:
            Assert.AreEqual(3, avgTimeByRegion.Rows.Count);
            Assert.AreEqual(5, avgTimeByRegion.Columns.Count);

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeToCompleteByRegion = results[Constants.DATATABLE_TITLES[2]];
            Assert.AreEqual(5, avgTimeToCompleteByRegion.Rows[1][1]);
            Assert.AreEqual(0, avgTimeToCompleteByRegion.Rows[0][4]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalRequestsByRegion = results[Constants.DATATABLE_TITLES[3]];
            Assert.AreEqual(1, totalRequestsByRegion.Rows[1][2]);
            Assert.AreEqual(0, totalRequestsByRegion.Rows[2][3]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalTimeSpentByRegion = results[Constants.DATATABLE_TITLES[4]];
            Assert.AreEqual(0, totalTimeSpentByRegion.Rows[2][2]);
            Assert.AreEqual(110, totalTimeSpentByRegion.Rows[0][2]);
        }

        [Test]
        public void generateMonthlyStratifiedByCallerTypeTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
                reportC.generateMonthlyReport(new DateTime(2013, 02, 01),
                                              new DateTime(2013, 06, 01),
                                              _dataTypes,
                                              Constants.StratifyOption.CallerType);

            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[5]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[6]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[7]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[8]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeByCallerType = results[Constants.DATATABLE_TITLES[5]];
            Assert.AreEqual(85, avgTimeByCallerType.Rows[3][2]);
            Assert.AreEqual(70, avgTimeByCallerType.Rows[2][1]);
            Assert.AreEqual(0, avgTimeByCallerType.Rows[0][1]);
            //Tests the table size:
            Assert.AreEqual(4, avgTimeByCallerType.Rows.Count);
            Assert.AreEqual(5, avgTimeByCallerType.Columns.Count);

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeToCompleteByCallerType = results[Constants.DATATABLE_TITLES[6]];
            Assert.AreEqual(5, avgTimeToCompleteByCallerType.Rows[0][3]);
            Assert.AreEqual(0, avgTimeToCompleteByCallerType.Rows[0][4]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalRequestsByCallerType = results[Constants.DATATABLE_TITLES[7]];
            Assert.AreEqual(2, totalRequestsByCallerType.Rows[2][1]);
            Assert.AreEqual(0, totalRequestsByCallerType.Rows[2][3]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalTimeSpentByCallerType = results[Constants.DATATABLE_TITLES[8]];
            Assert.AreEqual(40, totalTimeSpentByCallerType.Rows[1][4]);
            Assert.AreEqual(0, totalTimeSpentByCallerType.Rows[3][3]);
        }

        [Test]
        public void generateMonthlyStratifiedByTumourGroupTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
                reportC.generateMonthlyReport(new DateTime(2013, 02, 01),
                                              new DateTime(2013, 06, 01),
                                              _dataTypes,
                                              Constants.StratifyOption.TumorGroup);

            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[9]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[10]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[11]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[12]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeByTumourGroup = results[Constants.DATATABLE_TITLES[9]];
            Assert.AreEqual(75, avgTimeByTumourGroup.Rows[1][2]);
            Assert.AreEqual(40, avgTimeByTumourGroup.Rows[2][4]);
            Assert.AreEqual(0, avgTimeByTumourGroup.Rows[1][1]);
            //Tests the table size:
            Assert.AreEqual(3, avgTimeByTumourGroup.Rows.Count);
            Assert.AreEqual(5, avgTimeByTumourGroup.Columns.Count);

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeToCompleteByTumourGroup = results[Constants.DATATABLE_TITLES[10]];
            Assert.AreEqual(5, avgTimeToCompleteByTumourGroup.Rows[1][1]);
            Assert.AreEqual(0, avgTimeToCompleteByTumourGroup.Rows[2][2]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalQuestionsByTumourGroup = results[Constants.DATATABLE_TITLES[11]];
            Assert.AreEqual(2, totalQuestionsByTumourGroup.Rows[2][1]);
            Assert.AreEqual(0, totalQuestionsByTumourGroup.Rows[2][3]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalTimeSpentByTumourGroup = results[Constants.DATATABLE_TITLES[12]];
            Assert.AreEqual(40, totalTimeSpentByTumourGroup.Rows[2][4]);
            Assert.AreEqual(0, totalTimeSpentByTumourGroup.Rows[0][4]);
        }

        [Test]
        public void generateMonthlyNoStratificationTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
                reportC.generateMonthlyReport(new DateTime(2013, 02, 01),
                                              new DateTime(2013, 06, 01),
                                              _dataTypes,
                                              Constants.StratifyOption.None);

            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[0]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable generalReport = results[Constants.DATATABLE_TITLES[0]];
            Assert.AreEqual(85, generalReport.Rows[0][2]);
            Assert.AreEqual(3, generalReport.Rows[2][1]);
            Assert.AreNotEqual(0, generalReport.Rows[1][1]);
            //Tests the table size:
            Assert.AreEqual(4, generalReport.Rows.Count);
            Assert.AreEqual(5, generalReport.Columns.Count);
        }

        [Test]
        public void generateMpyStratifiedByRegionTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
                reportC.generateMonthPerYearReport(2,
                                              2012, 2013,
                                              _dataTypes,
                                              Constants.StratifyOption.Region);

            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[1]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[2]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[3]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[4]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeByRegion = results[Constants.DATATABLE_TITLES[1]];
            Assert.AreEqual(70, avgTimeByRegion.Rows[1][2]);
            Assert.AreEqual(0, avgTimeByRegion.Rows[2][1]);
            //Tests the table size:
            Assert.AreEqual(3, avgTimeByRegion.Rows.Count);
            Assert.AreEqual(3, avgTimeByRegion.Columns.Count);

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeToCompleteByRegion = results[Constants.DATATABLE_TITLES[2]];
            Assert.AreEqual(5, avgTimeToCompleteByRegion.Rows[1][2]);
            Assert.AreEqual(0, avgTimeToCompleteByRegion.Rows[0][1]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalRequestsByRegion = results[Constants.DATATABLE_TITLES[3]];
            Assert.AreEqual(1, totalRequestsByRegion.Rows[1][2]);
            Assert.AreEqual(0, totalRequestsByRegion.Rows[0][1]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalTimeSpentByRegion = results[Constants.DATATABLE_TITLES[4]];
            Assert.AreEqual(90, totalTimeSpentByRegion.Rows[2][2]);
            Assert.AreEqual(0, totalTimeSpentByRegion.Rows[1][1]);
        }

        [Test]
        public void generateMpyStratifiedByCallerTypeTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
                reportC.generateMonthPerYearReport(2,
                                              2013, 2014,
                                              _dataTypes,
                                              Constants.StratifyOption.CallerType);

            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[5]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[6]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[7]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[8]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeByCallerType = results[Constants.DATATABLE_TITLES[5]];
            Assert.AreEqual(70, avgTimeByCallerType.Rows[2][1]);
            Assert.AreEqual(0, avgTimeByCallerType.Rows[0][2]);
            //Tests the table size:
            Assert.AreEqual(4, avgTimeByCallerType.Rows.Count);
            Assert.AreEqual(3, avgTimeByCallerType.Columns.Count);

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeToCompleteByCallerType = results[Constants.DATATABLE_TITLES[6]];
            Assert.AreEqual(5, avgTimeToCompleteByCallerType.Rows[1][1]);
            Assert.AreEqual(0, avgTimeToCompleteByCallerType.Rows[1][2]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalRequestsByCallerType = results[Constants.DATATABLE_TITLES[7]];
            Assert.AreEqual(2, totalRequestsByCallerType.Rows[2][1]);
            Assert.AreEqual(0, totalRequestsByCallerType.Rows[2][2]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalTimeSpentByCallerType = results[Constants.DATATABLE_TITLES[8]];
            Assert.AreEqual(70, totalTimeSpentByCallerType.Rows[1][1]);
            Assert.AreEqual(0, totalTimeSpentByCallerType.Rows[3][2]);
        }

        [Test]
        public void generateMpyStratifiedByTumourGroupTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
                reportC.generateMonthPerYearReport(2,
                                              2012, 2013,
                                              _dataTypes,
                                              Constants.StratifyOption.TumorGroup);

            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[9]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[10]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[11]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[12]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeByTumourGroup = results[Constants.DATATABLE_TITLES[9]];
            Assert.AreEqual(65, avgTimeByTumourGroup.Rows[0][2]);
            Assert.AreEqual(40, avgTimeByTumourGroup.Rows[2][2]);
            Assert.AreEqual(0, avgTimeByTumourGroup.Rows[1][1]);
            //Tests the table size:
            Assert.AreEqual(3, avgTimeByTumourGroup.Rows.Count);
            Assert.AreEqual(3, avgTimeByTumourGroup.Columns.Count);

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeToCompleteByTumourGroup = results[Constants.DATATABLE_TITLES[10]];
            Assert.AreEqual(5, avgTimeToCompleteByTumourGroup.Rows[1][2]);
            Assert.AreEqual(0, avgTimeToCompleteByTumourGroup.Rows[2][1]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalQuestionsByTumourGroup = results[Constants.DATATABLE_TITLES[11]];
            Assert.AreEqual(2, totalQuestionsByTumourGroup.Rows[0][2]);
            Assert.AreEqual(0, totalQuestionsByTumourGroup.Rows[0][1]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalTimeSpentByTumourGroup = results[Constants.DATATABLE_TITLES[12]];
            Assert.AreEqual(80, totalTimeSpentByTumourGroup.Rows[2][2]);
            Assert.AreEqual(0, totalTimeSpentByTumourGroup.Rows[2][1]);
        }

        [Test]
        public void generateMpyNoStratificationTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
               reportC.generateMonthPerYearReport(2,
                                              2013, 2014,
                                              _dataTypes,
                                              Constants.StratifyOption.None);

            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[0]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable generalReport = results[Constants.DATATABLE_TITLES[0]];
            Assert.AreEqual(70, generalReport.Rows[0][1]);
            Assert.AreEqual(0, generalReport.Rows[2][2]);
            Assert.AreNotEqual(0, generalReport.Rows[1][1]);
            //Tests the table size:
            Assert.AreEqual(4, generalReport.Rows.Count);
            Assert.AreEqual(3, generalReport.Columns.Count);
        }

        [Test]
        public void generateYearlyStratifiedByRegionTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
                reportC.generateYearlyReport(2011, 2013,
                                              _dataTypes,
                                              Constants.StratifyOption.Region);

            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[1]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[2]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[3]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[4]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeByRegion = results[Constants.DATATABLE_TITLES[1]];
            Assert.AreEqual(65, avgTimeByRegion.Rows[1][2]);
            Assert.AreEqual(40, avgTimeByRegion.Rows[2][3]);
            Assert.AreEqual(0, avgTimeByRegion.Rows[0][1]);
            //Tests the table size:
            Assert.AreEqual(3, avgTimeByRegion.Rows.Count);
            Assert.AreEqual(4, avgTimeByRegion.Columns.Count);

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeToCompleteByRegion = results[Constants.DATATABLE_TITLES[2]];
            Assert.AreEqual(5, avgTimeToCompleteByRegion.Rows[1][3]);
            Assert.AreEqual(0, avgTimeToCompleteByRegion.Rows[0][1]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalRequestsByRegion = results[Constants.DATATABLE_TITLES[3]];
            Assert.AreEqual(2, totalRequestsByRegion.Rows[1][2]);
            Assert.AreEqual(1, totalRequestsByRegion.Rows[2][3]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalTimeSpentByRegion = results[Constants.DATATABLE_TITLES[4]];
            Assert.AreEqual(0, totalTimeSpentByRegion.Rows[0][3]);
            Assert.AreEqual(130, totalTimeSpentByRegion.Rows[1][2]);
        }

        [Test]
        public void generateYearlyStratifiedByCallerTypeTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
                reportC.generateYearlyReport(2011, 2013,
                                              _dataTypes,
                                              Constants.StratifyOption.CallerType);

            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[5]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[6]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[7]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[8]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeByCallerType = results[Constants.DATATABLE_TITLES[5]];
            Assert.AreEqual(85, avgTimeByCallerType.Rows[3][2]);
            Assert.AreEqual(70, avgTimeByCallerType.Rows[2][2]);
            Assert.AreEqual(0, avgTimeByCallerType.Rows[0][1]);
            //Tests the table size:
            Assert.AreEqual(4, avgTimeByCallerType.Rows.Count);
            Assert.AreEqual(4, avgTimeByCallerType.Columns.Count);

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeToCompleteByCallerType = results[Constants.DATATABLE_TITLES[6]];
            Assert.AreEqual(5, avgTimeToCompleteByCallerType.Rows[0][3]);
            Assert.AreEqual(0, avgTimeToCompleteByCallerType.Rows[1][1]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalRequestsByCallerType = results[Constants.DATATABLE_TITLES[7]];
            Assert.AreEqual(1, totalRequestsByCallerType.Rows[1][3]);
            Assert.AreEqual(0, totalRequestsByCallerType.Rows[2][3]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalTimeSpentByCallerType = results[Constants.DATATABLE_TITLES[8]];
            Assert.AreEqual(40, totalTimeSpentByCallerType.Rows[1][3]);
            Assert.AreEqual(0, totalTimeSpentByCallerType.Rows[3][3]);
        }

        [Test]
        public void generateYearlyStratifiedByTumourGroupTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
                reportC.generateYearlyReport(2011, 2013,
                                              _dataTypes,
                                              Constants.StratifyOption.TumorGroup);

            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[9]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[10]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[11]));
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[12]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeByTumourGroup = results[Constants.DATATABLE_TITLES[9]];
            Assert.AreEqual(50, avgTimeByTumourGroup.Rows[0][2]);
            Assert.AreEqual(30, avgTimeByTumourGroup.Rows[1][3]);
            Assert.AreEqual(0, avgTimeByTumourGroup.Rows[2][1]);
            //Tests the table size:
            Assert.AreEqual(3, avgTimeByTumourGroup.Rows.Count);
            Assert.AreEqual(4, avgTimeByTumourGroup.Columns.Count);

            //Tests the values of random cells created to the expected value of the cell
            DataTable avgTimeToCompleteByTumourGroup = results[Constants.DATATABLE_TITLES[10]];
            Assert.AreEqual(5, avgTimeToCompleteByTumourGroup.Rows[1][2]);
            Assert.AreEqual(0, avgTimeToCompleteByTumourGroup.Rows[0][3]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalQuestionsByTumourGroup = results[Constants.DATATABLE_TITLES[11]];
            Assert.AreEqual(2, totalQuestionsByTumourGroup.Rows[2][2]);
            Assert.AreEqual(0, totalQuestionsByTumourGroup.Rows[2][1]);

            //Tests the values of random cells created to the expected value of the cell
            DataTable totalTimeSpentByTumourGroup = results[Constants.DATATABLE_TITLES[12]];
            Assert.AreEqual(30, totalTimeSpentByTumourGroup.Rows[1][3]);
            Assert.AreEqual(0, totalTimeSpentByTumourGroup.Rows[1][1]);
        }

        [Test]
        public void generateYearlyNoStratificationTest() {
            ReportController reportC = new ReportController(_iDataContext);
            Dictionary<string, DataTable> results =
                reportC.generateYearlyReport(2011, 2013,
                                              _dataTypes,
                                              Constants.StratifyOption.None);

            //Tests if all dataTable is included
            Assert.True(results.ContainsKey(Constants.DATATABLE_TITLES[0]));

            //Tests the values of random cells created to the expected value of the cell
            DataTable generalReport = results[Constants.DATATABLE_TITLES[0]];
            Assert.AreEqual(76, generalReport.Rows[0][2]);
            Assert.AreEqual(0, generalReport.Rows[2][1]);
            Assert.AreNotEqual(0, generalReport.Rows[1][3]);
            //Tests the table size:
            Assert.AreEqual(4, generalReport.Rows.Count);
            Assert.AreEqual(4, generalReport.Columns.Count);
        }
    }
}
