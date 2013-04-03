using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SasquatchCAIRS.Controllers;
using SasquatchCAIRS;
using NUnit.Framework;
using SasquatchCAIRS.Controllers.ServiceSystem;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;
using Assert = NUnit.Framework.Assert;

namespace CAIRSTestProject.Unit {
    [TestClass]
    public class TestDropdownController {
        private CAIRSDataContext _dc = new CAIRSDataContext();
        private DropdownController _ddc = new DropdownController();
        DropdownEntry _de = null;
        private static string _testValue = "Dropdown Test Value";
        private static string _testCode = "DRPDWN";
        private static string _editCode = "NEWDRP";
        private static string _editValue = "Edited Dropwdown Test Value";
        private static int _prependor;

        [SetUp]
        public void setUp() {
            Random random = new Random();
            _prependor = random.Next(1, 1000);
            _testValue = _prependor + _testValue;
            _testCode = _prependor + _testCode;
            _editValue = _prependor + _editValue;
            _editCode = _prependor + _editCode;


        }
        

        [TestMethod]
        public void TestCreateActiveKeyword() {
            _ddc.createEntry(Constants.DropdownTable.Keyword, _testCode, _testValue, true);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.Keyword);;
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            deleteTestKeyword(_testValue);
        }

        [TestMethod]
        public void TestCreateInactiveKeyword() {
            _ddc.createEntry(Constants.DropdownTable.Keyword, _testCode, _testValue, false);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.Keyword, false);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            deleteTestKeyword(_testValue);
        }

        //[TestMethod]
        //public void TestActivateKeyword() {
        //    _ddc.createEntry(Constants.DropdownTable.Keyword, testCode, testValue, false);
        //    int keyword = (from key in _dc.Keywords
        //                   where key.KeywordValue == testValue
        //                   select key.KeywordID).FirstOrDefault();
        //    _ddc.editEntryStatus(Constants.DropdownTable.Keyword, keyword, true);

        //}

        [TestMethod]
        public void TestEditKeyword() {
            _ddc.createEntry(Constants.DropdownTable.Keyword, _testCode, _testValue, false);
            int keyword = (from key in _dc.Keywords
                        where key.KeywordValue == _testValue
                        select key.KeywordID).FirstOrDefault();
            _ddc.editEntry(Constants.DropdownTable.Keyword, keyword, _editCode, _editValue, true);    
                    
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.Keyword);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_editValue) && dropdownEntry.active) {
                    _de = dropdownEntry;
                    break;
                }
            }
           
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _editValue);
            deleteTestKeyword(_editValue);
        }


        [TestMethod]
        public void TestAddActiveQuestionType() {
            _ddc.createEntry(Constants.DropdownTable.QuestionType, _testCode, _testValue, true);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.QuestionType);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            Assert.IsTrue(_de.code == _testCode);

            deleteTestQuestionType(_testCode, _testValue);
        }

        [TestMethod]
        public void TestAddInactiveQuestionType() {
            _ddc.createEntry(Constants.DropdownTable.QuestionType, _testCode, _testValue, false);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.QuestionType, false);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            Assert.IsTrue(_de.code == _testCode);

            deleteTestQuestionType(_testCode, _testValue);
        }

        [TestMethod]
        public void TestEditQuestionType() {
            _ddc.createEntry(Constants.DropdownTable.QuestionType, _testCode, _testValue, false);
            int qtypeid = (from qts in _dc.QuestionTypes
                        where qts.Value == _testValue && qts.Code == _testCode
                        select qts.QuestionTypeID).FirstOrDefault();
            _ddc.editEntry(Constants.DropdownTable.QuestionType, qtypeid, _editCode, _editValue, true);

            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.QuestionType);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_editCode) && dropdownEntry.code.Equals(_editValue) && dropdownEntry.active) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _editValue);
            Assert.IsTrue(_de.code == _editCode);

            deleteTestQuestionType(_editCode, _editValue);
        }

        [TestMethod]
        public void TestAddActiveRegion() {
            _ddc.createEntry(Constants.DropdownTable.Region, _testCode, _testValue, true);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.Region);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            Assert.IsTrue(_de.code == _testCode);

            deleteTestRegion(_testCode, _testValue);
        }

        [TestMethod]
        public void TestAddInactiveRegion() {
            _ddc.createEntry(Constants.DropdownTable.Region, _testCode, _testValue, false);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.Region, false);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            Assert.IsTrue(_de.code == _testCode);

            deleteTestRegion(_testCode, _testValue);
        }

        [TestMethod]
        public void TestEditRegion() {
            _ddc.createEntry(Constants.DropdownTable.Region, _testCode, _testValue, false);
            int qtypeid = (from reg in _dc.Regions
                           where reg.Value == _testValue && reg.Code == _testCode
                           select reg.RegionID).FirstOrDefault();
            
            _ddc.editEntry(Constants.DropdownTable.Region, qtypeid, _editCode, _editValue, true);

            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.Region);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_editValue) && dropdownEntry.code.Equals(_editCode) && dropdownEntry.active) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _editValue);
            Assert.IsTrue(_de.code == _editCode);
            Assert.IsTrue(_de.active);

            deleteTestRegion(_editCode, _editValue);
        }

        [TestMethod]
        public void TestAddActiveRequestorType() {
            _ddc.createEntry(Constants.DropdownTable.RequestorType, _testCode, _testValue, true);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.RequestorType);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            Assert.IsTrue(_de.code == _testCode);

            deleteTestRequestorTypes(_testCode, _testValue);
        }

        [TestMethod]
        public void TestAddInactiveRequestorType() {
            _ddc.createEntry(Constants.DropdownTable.RequestorType, _testCode, _testValue, false);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.RequestorType, false);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            Assert.IsTrue(_de.code == _testCode);

            deleteTestRequestorTypes(_testCode, _testValue);
        }

        [TestMethod]
        public void TestEditRequestorType() {
            _ddc.createEntry(Constants.DropdownTable.RequestorType, _testCode, _testValue, false);
            int reqtypeid = (from req in _dc.RequestorTypes
                           where req.Value == _testValue && req.Code == _testCode
                           select req.RequestorTypeID).FirstOrDefault();

            _ddc.editEntry(Constants.DropdownTable.RequestorType, reqtypeid, _editCode, _editValue, true);

            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.RequestorType);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_editValue) && dropdownEntry.code.Equals(_editCode) && dropdownEntry.active) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _editValue);
            Assert.IsTrue(_de.code == _editCode);
            Assert.IsTrue(_de.active);

            deleteTestRequestorTypes(_editCode, _editValue);
        }

        [TestMethod]
        public void TestAddActiveTumourGroup() {
            _ddc.createEntry(Constants.DropdownTable.TumourGroup, _testCode, _testValue, true);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.TumourGroup);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            Assert.IsTrue(_de.code == _testCode);

            deleteTestTumourGroup(_testCode, _testValue);
        }

        [TestMethod]
        public void TestAddInactiveTumourGroup() {
            _ddc.createEntry(Constants.DropdownTable.TumourGroup, _testCode, _testValue, false);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.TumourGroup, false);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            Assert.IsTrue(_de.code == _testCode);

            deleteTestTumourGroup(_testCode, _testValue);
        }

        [TestMethod]
        public void TestEditTumourGroup() {
            _ddc.createEntry(Constants.DropdownTable.TumourGroup, _testCode, _testValue, false);
            int tgid = (from tgs in _dc.TumourGroups
                             where tgs.Value == _testValue && tgs.Code == _testCode
                             select tgs.TumourGroupID).FirstOrDefault();

            _ddc.editEntry(Constants.DropdownTable.TumourGroup, tgid, _editCode, _editValue, true);

            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.TumourGroup);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_editValue) && dropdownEntry.code.Equals(_editCode) && dropdownEntry.active) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _editValue);
            Assert.IsTrue(_de.code == _editCode);
            Assert.IsTrue(_de.active);

            deleteTestTumourGroup(_editCode, _editValue);
        }

        [TestMethod]
        public void TestAddActiveUserGroup() {
            _ddc.createEntry(Constants.DropdownTable.UserGroup, _testCode, _testValue, true);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.UserGroup);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            Assert.IsTrue(_de.code == _testCode);

            deleteTestUserGroup(_testCode, _testValue);
        }

        [TestMethod]
        public void TestAddInactiveUserGroup() {
            _ddc.createEntry(Constants.DropdownTable.UserGroup, _testCode, _testValue, false);
            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.UserGroup, false);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_testValue)) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _testValue);
            Assert.IsTrue(_de.code == _testCode);

            deleteTestUserGroup(_testCode, _testValue);
        }

        [TestMethod]
        public void TestEditUserGroup() {
            _ddc.createEntry(Constants.DropdownTable.UserGroup, _testCode, _testValue, false);
            int tgid = (from ugs in _dc.UserGroups
                        where ugs.Value == _testValue && ugs.Code == _testCode
                        select ugs.GroupID).FirstOrDefault();

            _ddc.editEntry(Constants.DropdownTable.UserGroup, tgid, _editCode, _editValue, true);

            List<DropdownEntry> list = _ddc.getEntries(Constants.DropdownTable.UserGroup);
            foreach (DropdownEntry dropdownEntry in list) {
                if (dropdownEntry.value.Equals(_editValue) && dropdownEntry.code.Equals(_editCode) && dropdownEntry.active) {
                    _de = dropdownEntry;
                    break;
                }
            }
            Assert.NotNull(_de);
            Assert.IsTrue(_de.value == _editValue);
            Assert.IsTrue(_de.code == _editCode);
            Assert.IsTrue(_de.active);

            deleteTestUserGroup(_editCode, _editValue);
        }

        private void deleteTestKeyword(string input) {
            var temp = (from key in _dc.Keywords
                        where key.KeywordValue == input
                        select key).FirstOrDefault();
            _dc.Keywords.DeleteOnSubmit(temp);
            _dc.SubmitChanges();
        }

        private void deleteTestQuestionType(string code, string value) {
            var temp = (from qts in _dc.QuestionTypes
                        where qts.Value == value && qts.Code == code
                        select qts).FirstOrDefault();
            _dc.QuestionTypes.DeleteOnSubmit(temp);
            _dc.SubmitChanges();
        }

        private void deleteTestRegion(string code, string value) {
            var temp = (from reg in _dc.Regions
                        where reg.Value == value && reg.Code == code
                        select reg).FirstOrDefault();
            _dc.Regions.DeleteOnSubmit(temp);
            _dc.SubmitChanges();
        }
        
        private void deleteTestRequestorTypes(string code, string value) {
            var temp = (from req in _dc.RequestorTypes
                        where req.Value == value && req.Code == code
                        select req).FirstOrDefault();
            _dc.RequestorTypes.DeleteOnSubmit(temp);
            _dc.SubmitChanges();
        }

        private void deleteTestTumourGroup(string code, string value) {
            var temp = (from tgs in _dc.TumourGroups
                        where tgs.Value == value && tgs.Code == code
                        select tgs).FirstOrDefault();
            _dc.TumourGroups.DeleteOnSubmit(temp);
            _dc.SubmitChanges();
        }

        private void deleteTestUserGroup(string code, string value) {
            var temp = (from ugs in _dc.UserGroups
                        where ugs.Value == value && ugs.Code == code
                        select ugs).FirstOrDefault();
            _dc.UserGroups.DeleteOnSubmit(temp);
            _dc.SubmitChanges();
        }
    }
}
