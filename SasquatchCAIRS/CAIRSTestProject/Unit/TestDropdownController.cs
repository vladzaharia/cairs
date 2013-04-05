using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SasquatchCAIRS;
using SasquatchCAIRS.Controllers.Service;
using SasquatchCAIRS.Models.Common;
using SasquatchCAIRS.Models.Service;

namespace CAIRSTestProject.Unit {
    [TestFixture]
    public class TestDropdownController {
        [TearDown]
        public void TearDown() {
            switch (_table) {
                case Constants.DropdownTable.Keyword:
                    deleteTestKeyword(_testValue);
                    deleteTestKeyword(_editValue);
                    break;
                case Constants.DropdownTable.QuestionType:
                    deleteTestQuestionType(_testCode, _testValue);
                    deleteTestQuestionType(_editCode, _editValue);
                    break;
                case Constants.DropdownTable.Region:
                    deleteTestRegion(_testCode, _testValue);
                    deleteTestRegion(_editCode, _editValue);
                    break;
                case Constants.DropdownTable.RequestorType:
                    deleteTestRequestorType(_testCode, _testValue);
                    deleteTestRequestorType(_editCode, _editValue);
                    break;
                case Constants.DropdownTable.TumourGroup:
                    deleteTestTumourGroup(_testCode, _testValue);
                    deleteTestTumourGroup(_editCode, _editValue);
                    break;
                case Constants.DropdownTable.UserGroup:
                    deleteTestUserGroup(_testCode, _testValue);
                    deleteTestUserGroup(_editCode, _editValue);
                    break;
                default:
                    break;
            }
        }

        private CAIRSDataContext _dc = new CAIRSDataContext();

        private DropdownManagementController _ddc =
            new DropdownManagementController();

        private static Constants.DropdownTable _table;
        private DropdownEntry _de;
        private string _testValue, _testCode, _editCode, _editValue;
        private static int _prependor;

        [TestFixtureSetUp]
        public void setUp() {
            var random = new Random();
            _prependor = random.Next(1, 1000);
            _testValue = _prependor + "DRP Test Value";
            _testCode = _prependor + "DRP";
            _editValue = _prependor + "DRP Edit Value";
            _editCode = _prependor + "EDT";
        }

        public void TestGetActiveEntries(Constants.DropdownTable table) {
            _table = table;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            List<DropdownEntry> list = _ddc.getEntries(_table);
            DropdownEntry toCheck = (from keys in list
                                     where keys.value == _testValue
                                     select keys).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.value.Trim(), _testValue);
            Assert.True(toCheck.active);
        }

        public void TestGetInactiveEntries(Constants.DropdownTable table) {
            _table = table;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            List<DropdownEntry> list = _ddc.getEntries(_table, false);
            DropdownEntry toCheck = (from keys in list
                                     where keys.value == _testValue
                                     select keys).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.value.Trim(), _testValue);
            Assert.False(toCheck.active);
        }

        private void deleteTestKeyword(string input) {
            Keyword temp = (from key in _dc.Keywords
                            where key.KeywordValue == input
                            select key).FirstOrDefault();
            if (temp != null) {
                _dc.Keywords.DeleteOnSubmit(temp);
                _dc.SubmitChanges();
            }
        }

        private void deleteTestQuestionType(string code, string value) {
            QuestionType temp = (from qts in _dc.QuestionTypes
                                 where qts.Value == value && qts.Code == code
                                 select qts).FirstOrDefault();
            if (temp != null) {
                _dc.QuestionTypes.DeleteOnSubmit(temp);
                _dc.SubmitChanges();
            }
        }

        private void deleteTestRegion(string code, string value) {
            Region temp = (from reg in _dc.Regions
                           where reg.Value == value && reg.Code == code
                           select reg).FirstOrDefault();
            if (temp != null) {
                _dc.Regions.DeleteOnSubmit(temp);
                _dc.SubmitChanges();
            }
        }

        private void deleteTestRequestorType(string code, string value) {
            RequestorType temp = (from req in _dc.RequestorTypes
                                  where req.Value == value && req.Code == code
                                  select req).FirstOrDefault();
            if (temp != null) {
                _dc.RequestorTypes.DeleteOnSubmit(temp);
                _dc.SubmitChanges();
            }
        }

        private void deleteTestTumourGroup(string code, string value) {
            TumourGroup temp = (from tgs in _dc.TumourGroups
                                where tgs.Value == value && tgs.Code == code
                                select tgs).FirstOrDefault();
            if (temp != null) {
                _dc.TumourGroups.DeleteOnSubmit(temp);
                _dc.SubmitChanges();
            }
        }

        private void deleteTestUserGroup(string code, string value) {
            UserGroup temp = (from ugs in _dc.UserGroups
                              where ugs.Value == value && ugs.Code == code
                              select ugs).FirstOrDefault();
            if (temp != null) {
                _dc.UserGroups.DeleteOnSubmit(temp);
                _dc.SubmitChanges();
            }
        }

        [Test]
        public void TestActivateKeyword() {
            _table = Constants.DropdownTable.Keyword;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int keyid = (from keys in _dc.Keywords
                         where keys.KeywordValue == _testValue
                         select keys.KeywordID).FirstOrDefault();
            _ddc.editEntryStatus(_table, keyid, true);
            Keyword toCheck = (from u in _dc.Keywords
                               where u.KeywordID == keyid
                               select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.KeywordValue.Trim(), _testValue);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestActivateQuestionType() {
            _table = Constants.DropdownTable.QuestionType;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int qtid = (from qts in _dc.QuestionTypes
                        where qts.Value == _testValue
                        select qts.QuestionTypeID).FirstOrDefault();
            _ddc.editEntryStatus(_table, qtid, true);
            QuestionType toCheck = (from u in _dc.QuestionTypes
                                    where u.QuestionTypeID == qtid
                                    select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestActivateRegion() {
            _table = Constants.DropdownTable.Region;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int regid = (from regs in _dc.Regions
                         where regs.Value == _testValue
                         select regs.RegionID).FirstOrDefault();
            _ddc.editEntryStatus(_table, regid, true);
            Region toCheck = (from u in _dc.Regions
                              where u.RegionID == regid
                              select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestActivateRequestorType() {
            _table = Constants.DropdownTable.RequestorType;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int rtid = (from rts in _dc.RequestorTypes
                        where rts.Value == _testValue
                        select rts.RequestorTypeID).FirstOrDefault();
            _ddc.editEntryStatus(_table, rtid, true);
            RequestorType toCheck = (from u in _dc.RequestorTypes
                                     where u.RequestorTypeID == rtid
                                     select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestActivateTumourGroup() {
            _table = Constants.DropdownTable.TumourGroup;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int tgid = (from tgs in _dc.TumourGroups
                        where tgs.Value == _testValue
                        select tgs.TumourGroupID).FirstOrDefault();
            _ddc.editEntryStatus(_table, tgid, true);
            TumourGroup toCheck = (from u in _dc.TumourGroups
                                   where u.TumourGroupID == tgid
                                   select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestActivateUserGroup() {
            _table = Constants.DropdownTable.UserGroup;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int ugid = (from ugs in _dc.UserGroups
                        where ugs.Value == _testValue
                        select ugs.GroupID).FirstOrDefault();
            _ddc.editEntryStatus(_table, ugid, true);
            UserGroup toCheck = (from u in _dc.UserGroups
                                 where u.GroupID == ugid
                                 select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestCreateActiveKeyword() {
            _table = Constants.DropdownTable.Keyword;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            Keyword toCheck = (from keys in _dc.Keywords
                               where keys.KeywordValue == _testValue
                               select keys).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.KeywordValue.Trim(), _testValue);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestCreateActiveQuestionType() {
            _table = Constants.DropdownTable.QuestionType;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            QuestionType toCheck = (from qts in _dc.QuestionTypes
                                    where qts.Value == _testValue
                                    select qts).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestCreateActiveRegion() {
            _table = Constants.DropdownTable.Region;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            Region toCheck = (from regs in _dc.Regions
                              where regs.Value == _testValue
                              select regs).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestCreateActiveRequestorType() {
            _table = Constants.DropdownTable.RequestorType;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            RequestorType toCheck = (from rts in _dc.RequestorTypes
                                     where rts.Value == _testValue
                                     select rts).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestAddKeywordEntity() {
            _table = Constants.DropdownTable.Keyword;
            _de = new DropdownEntry(_testCode, _testValue);
            _ddc.addEntry(_table, _de);
            Keyword toCheck = (from keys in _dc.Keywords
                               where keys.KeywordValue == _testValue
                               select keys).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.KeywordValue.Trim(), _testValue);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestAddQuestionTypeEntity() {
            _table = Constants.DropdownTable.QuestionType;
            _de = new DropdownEntry(_testCode, _testValue);
            _ddc.addEntry(_table, _de);
            QuestionType toCheck = (from qts in _dc.QuestionTypes
                                    where qts.Value == _testValue
                                    select qts).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestAddRegionEntity() {
            _table = Constants.DropdownTable.Region;
            _de = new DropdownEntry(_testCode, _testValue);
            _ddc.addEntry(_table, _de);
            Region toCheck = (from regs in _dc.Regions
                              where regs.Value == _testValue
                              select regs).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestAddRequestorTypeEntity() {
            _table = Constants.DropdownTable.RequestorType;
            _de = new DropdownEntry(_testCode, _testValue);
            _ddc.addEntry(_table, _de);
            RequestorType toCheck = (from rts in _dc.RequestorTypes
                                     where rts.Value == _testValue
                                     select rts).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestAddTumourGroupEntity() {
            _table = Constants.DropdownTable.TumourGroup;
            _de = new DropdownEntry(_testCode, _testValue);
            _ddc.addEntry(_table, _de);
            TumourGroup toCheck = (from tgs in _dc.TumourGroups
                                   where tgs.Value == _testValue
                                   select tgs).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestAddUserGroupEntity() {
            _table = Constants.DropdownTable.UserGroup;
            _de = new DropdownEntry(_testCode, _testValue);
            _ddc.addEntry(_table, _de);
            UserGroup toCheck = (from ugs in _dc.UserGroups
                                 where ugs.Value == _testValue
                                 select ugs).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestCreateActiveTumourGroup() {
            _table = Constants.DropdownTable.TumourGroup;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            TumourGroup toCheck = (from tgs in _dc.TumourGroups
                                   where tgs.Value == _testValue
                                   select tgs).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestCreateActiveUserGroup() {
            _table = Constants.DropdownTable.UserGroup;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            UserGroup toCheck = (from ugs in _dc.UserGroups
                                 where ugs.Value == _testValue
                                 select ugs).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestCreateInactiveKeyword() {
            _table = Constants.DropdownTable.Keyword;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            Keyword toCheck = (from keys in _dc.Keywords
                               where keys.KeywordValue == _testValue
                               select keys).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.KeywordValue.Trim(), _testValue);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestCreateInactiveQuestionType() {
            _table = Constants.DropdownTable.QuestionType;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            QuestionType toCheck = (from qts in _dc.QuestionTypes
                                    where qts.Value == _testValue
                                    select qts).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestCreateInactiveRegion() {
            _table = Constants.DropdownTable.Region;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            Region toCheck = (from regs in _dc.Regions
                              where regs.Value == _testValue
                              select regs).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestCreateInactiveRequestorType() {
            _table = Constants.DropdownTable.RequestorType;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            RequestorType toCheck = (from rts in _dc.RequestorTypes
                                     where rts.Value == _testValue
                                     select rts).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestCreateInactiveTumourGroup() {
            _table = Constants.DropdownTable.TumourGroup;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            TumourGroup toCheck = (from tgs in _dc.TumourGroups
                                   where tgs.Value == _testValue
                                   select tgs).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestCreateInactiveUserGroup() {
            _table = Constants.DropdownTable.UserGroup;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            UserGroup toCheck = (from ugs in _dc.UserGroups
                                 where ugs.Value == _testValue
                                 select ugs).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.AreEqual(toCheck.Code.Trim(), _testCode);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestDeactiveKeyword() {
            _table = Constants.DropdownTable.Keyword;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            int keyid = (from keys in _dc.Keywords
                         where keys.KeywordValue == _testValue
                         select keys.KeywordID).FirstOrDefault();
            _ddc.editEntryStatus(_table, keyid, false);
            Keyword toCheck = (from u in _dc.Keywords
                               where u.KeywordID == keyid
                               select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.KeywordValue.Trim(), _testValue);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestDeactiveQuestionType() {
            _table = Constants.DropdownTable.QuestionType;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            int qtid = (from qts in _dc.QuestionTypes
                        where qts.Value == _testValue
                        select qts.QuestionTypeID).FirstOrDefault();
            _ddc.editEntryStatus(_table, qtid, false);
            QuestionType toCheck = (from u in _dc.QuestionTypes
                                    where u.QuestionTypeID == qtid
                                    select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestDeactiveRegion() {
            _table = Constants.DropdownTable.Region;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            int regid = (from regs in _dc.Regions
                         where regs.Value == _testValue
                         select regs.RegionID).FirstOrDefault();
            _ddc.editEntryStatus(_table, regid, false);
            Region toCheck = (from u in _dc.Regions
                              where u.RegionID == regid
                              select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestDeactiveRequestorType() {
            _table = Constants.DropdownTable.RequestorType;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            int rtid = (from rts in _dc.RequestorTypes
                        where rts.Value == _testValue
                        select rts.RequestorTypeID).FirstOrDefault();
            _ddc.editEntryStatus(_table, rtid, false);
            RequestorType toCheck = (from u in _dc.RequestorTypes
                                     where u.RequestorTypeID == rtid
                                     select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestDeactiveTumourGroup() {
            _table = Constants.DropdownTable.TumourGroup;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            int tgid = (from tgs in _dc.TumourGroups
                        where tgs.Value == _testValue
                        select tgs.TumourGroupID).FirstOrDefault();
            _ddc.editEntryStatus(_table, tgid, false);
            TumourGroup toCheck = (from u in _dc.TumourGroups
                                   where u.TumourGroupID == tgid
                                   select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestDeactiveUserGroup() {
            _table = Constants.DropdownTable.UserGroup;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            int ugid = (from ugs in _dc.UserGroups
                        where ugs.Value == _testValue
                        select ugs.GroupID).FirstOrDefault();
            _ddc.editEntryStatus(_table, ugid, false);
            UserGroup toCheck = (from u in _dc.UserGroups
                                 where u.GroupID == ugid
                                 select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _testValue);
            Assert.False(toCheck.Active);
        }

        [Test]
        public void TestEditKeyword() {
            _table = Constants.DropdownTable.Keyword;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int keyid = (from keys in _dc.Keywords
                         where keys.KeywordValue == _testValue
                         select keys.KeywordID).FirstOrDefault();
            _ddc.editEntry(_table, keyid, _editCode, _editValue, true);
            Keyword toCheck = (from u in _dc.Keywords
                               where u.KeywordID == keyid
                               select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.KeywordValue.Trim(), _editValue);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestEditQuestionType() {
            _table = Constants.DropdownTable.QuestionType;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int qtid = (from qts in _dc.QuestionTypes
                        where qts.Value == _testValue && qts.Code == _testCode
                        select qts.QuestionTypeID).FirstOrDefault();
            _ddc.editEntry(_table, qtid, _editCode, _editValue, true);
            QuestionType toCheck = (from u in _dc.QuestionTypes
                                    where u.QuestionTypeID == qtid
                                    select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _editValue);
            Assert.AreEqual(toCheck.Code.Trim(), _editCode);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestEditRegion() {
            _table = Constants.DropdownTable.Region;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int regid = (from regs in _dc.Regions
                         where
                             regs.Value == _testValue && regs.Code == _testCode
                         select regs.RegionID).FirstOrDefault();
            _ddc.editEntry(_table, regid, _editCode, _editValue, true);
            Region toCheck = (from u in _dc.Regions
                              where u.RegionID == regid
                              select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _editValue);
            Assert.AreEqual(toCheck.Code.Trim(), _editCode);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestEditRequestorType() {
            _table = Constants.DropdownTable.RequestorType;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int rtid = (from rts in _dc.RequestorTypes
                        where rts.Value == _testValue && rts.Code == _testCode
                        select rts.RequestorTypeID).FirstOrDefault();
            _ddc.editEntry(_table, rtid, _editCode, _editValue, true);
            RequestorType toCheck = (from u in _dc.RequestorTypes
                                     where u.RequestorTypeID == rtid
                                     select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _editValue);
            Assert.AreEqual(toCheck.Code.Trim(), _editCode);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestEditTumourGroup() {
            _table = Constants.DropdownTable.TumourGroup;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int tgid = (from tgs in _dc.TumourGroups
                        where tgs.Value == _testValue && tgs.Code == _testCode
                        select tgs.TumourGroupID).FirstOrDefault();
            _ddc.editEntry(_table, tgid, _editCode, _editValue, true);
            TumourGroup toCheck = (from u in _dc.TumourGroups
                                   where u.TumourGroupID == tgid
                                   select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _editValue);
            Assert.AreEqual(toCheck.Code.Trim(), _editCode);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestEditUserGroup() {
            _table = Constants.DropdownTable.UserGroup;
            _ddc.createEntry(_table, _testCode, _testValue, false);
            int ugid = (from ugs in _dc.UserGroups
                        where ugs.Value == _testValue && ugs.Code == _testCode
                        select ugs.GroupID).FirstOrDefault();
            _ddc.editEntry(_table, ugid, _editCode, _editValue, true);
            UserGroup toCheck = (from u in _dc.UserGroups
                                 where u.GroupID == ugid
                                 select u).FirstOrDefault();
            Assert.NotNull(toCheck);
            Assert.AreEqual(toCheck.Value.Trim(), _editValue);
            Assert.AreEqual(toCheck.Code.Trim(), _editCode);
            Assert.True(toCheck.Active);
        }

        [Test]
        public void TestGetActiveKeywords() {
            TestGetActiveEntries(Constants.DropdownTable.Keyword);
        }

        [Test]
        public void TestGetActiveQuestionTypes() {
            TestGetActiveEntries(Constants.DropdownTable.QuestionType);
        }

        [Test]
        public void TestGetActiveRegions() {
            TestGetActiveEntries(Constants.DropdownTable.Region);
        }

        [Test]
        public void TestGetActiveRequestorTypes() {
            TestGetActiveEntries(Constants.DropdownTable.RequestorType);
        }

        [Test]
        public void TestGetActiveTumourGroups() {
            TestGetActiveEntries(Constants.DropdownTable.TumourGroup);
        }

        [Test]
        public void TestGetActiveUserGroups() {
            TestGetActiveEntries(Constants.DropdownTable.UserGroup);
        }

        [Test]
        public void TestGetEmptyMatchingKeyword() {
            _table = Constants.DropdownTable.Keyword;
            List<string> list = _ddc.getMatchingKeywords(_testValue);
            Assert.NotNull(list);
            Assert.False(list.Contains(_testValue));
        }

        [Test]
        public void TestGetInactiveKeywords() {
            TestGetInactiveEntries(Constants.DropdownTable.Keyword);
        }

        [Test]
        public void TestGetInactiveQuestionTypes() {
            TestGetInactiveEntries(Constants.DropdownTable.QuestionType);
        }

        [Test]
        public void TestGetInactiveRegions() {
            TestGetInactiveEntries(Constants.DropdownTable.Region);
        }

        [Test]
        public void TestGetInactiveRequestorTypes() {
            TestGetInactiveEntries(Constants.DropdownTable.RequestorType);
        }

        [Test]
        public void TestGetInactiveTumourGroups() {
            TestGetInactiveEntries(Constants.DropdownTable.TumourGroup);
        }

        [Test]
        public void TestGetInactiveUserGroups() {
            TestGetInactiveEntries(Constants.DropdownTable.UserGroup);
        }

        [Test]
        public void TestGetMatchingKeyword() {
            _table = Constants.DropdownTable.Keyword;
            _ddc.createEntry(_table, _testCode, _testValue, true);
            List<string> list = _ddc.getMatchingKeywords(_testValue);
            Assert.NotNull(list);
            Assert.True(list.Contains(_testValue));
        }
    }
}