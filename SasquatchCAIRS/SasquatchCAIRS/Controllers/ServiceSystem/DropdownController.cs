using System;
using System.Collections.Generic;
using System.Linq;
using SasquatchCAIRS.Models;
using SasquatchCAIRS.Models.ServiceSystem;

namespace SasquatchCAIRS.Controllers.ServiceSystem {
    public sealed class DropdownController {
        private static readonly DropdownController _instance =
            new DropdownController();
        private CAIRSDataContext _db = new CAIRSDataContext();

        private DropdownController() {
        }

        public static DropdownController instance {
            get {
                return _instance;
            }
        }

        /// <summary>
        /// Get all active dropdown entries from a specific table.
        /// </summary>
        /// <param name="table">Table containing the entries.</param>
        /// <returns>List of dropdown table entries.</returns>
        public List<DropdownEntry> getActiveEntries(Constants.DropdownTable table) {
            List<DropdownEntry> list = new List<DropdownEntry>();

            switch (table) {
                case Constants.DropdownTable.Keyword:
                    List<Keyword> keywords =
                        (from kw in _db.Keywords
                         where kw.Active
                         select kw)
                         .ToList();

                    list.AddRange(keywords.Select(
                        kw => new KeywordEntry(kw.KeywordID,
                                               kw.KeywordValue,
                                               kw.Active)));

                    break;
                case Constants.DropdownTable.QuestionType:
                    List<QuestionType> qTypes =
                        (from qType in _db.QuestionTypes
                         where qType.Active
                         select qType)
                         .ToList();

                    list.AddRange(qTypes.Select(
                        qType => new DropdownEntry(qType.QuestionTypeID,
                                                   qType.Code,
                                                   qType.Value,
                                                   qType.Active)));

                    break;
                case Constants.DropdownTable.Region:
                    List<Region> regions =
                        (from region in _db.Regions
                         where region.Active
                         select region)
                         .ToList();

                    list.AddRange(regions.Select(
                        region => new DropdownEntry(region.RegionID,
                                                    region.Code,
                                                    region.Value,
                                                    region.Active)));

                    break;
                case Constants.DropdownTable.RequestorType:
                    List<RequestorType> rTypes =
                        (from rType in _db.RequestorTypes
                         where rType.Active
                         select rType)
                         .ToList();

                    list.AddRange(rTypes.Select(
                        rType => new DropdownEntry(rType.RequestorTypeID,
                                                   rType.Code,
                                                   rType.Value,
                                                   rType.Active)));

                    break;
                case Constants.DropdownTable.TumourGroup:
                    List<TumourGroup> tGroups =
                        (from tGroup in _db.TumourGroups
                         where tGroup.Active
                         select tGroup)
                         .ToList();

                    list.AddRange(tGroups.Select(
                        tGroup => new DropdownEntry(tGroup.TumourGroupID,
                                                    tGroup.Code,
                                                    tGroup.Value,
                                                    tGroup.Active)));

                    break;
                case Constants.DropdownTable.UserGroup:
                    List<UserGroup> uGroups =
                        (from uGroup in _db.UserGroups
                         where uGroup.Active
                         select uGroup)
                         .ToList();

                    list.AddRange(uGroups.Select(
                        uGroup => new DropdownEntry(uGroup.GroupID,
                                                    uGroup.Code,
                                                    uGroup.Value,
                                                    uGroup.Active)));

                    break;
            }

            return list;
        }

        /// <summary>
        /// Add a new entry to one of the dropdown tables in the database.
        /// </summary>
        /// <param name="table">Table to add a new entry to.</param>
        /// <param name="entry">DropdownEntry containing the value and code,
        /// if exists.</param>
        public void addEntry(Constants.DropdownTable table,
                             DropdownEntry entry) {
            switch (table) {
                case Constants.DropdownTable.Keyword:
                    Keyword kw = new Keyword {
                        KeywordValue = entry.value
                    };

                    _db.Keywords.InsertOnSubmit(kw);
                    break;
                case Constants.DropdownTable.QuestionType:
                    QuestionType qType = new QuestionType {
                        Code = entry.code,
                        Value = entry.value
                    };

                    _db.QuestionTypes.InsertOnSubmit(qType);

                    break;
                case Constants.DropdownTable.Region:
                    Region region = new Region {
                        Code = entry.code,
                        Value = entry.value
                    };

                    _db.Regions.InsertOnSubmit(region);

                    break;
                case Constants.DropdownTable.RequestorType:
                    RequestorType rType = new RequestorType {
                        Code = entry.code,
                        Value = entry.value
                    };

                    _db.RequestorTypes.InsertOnSubmit(rType);

                    break;
                case Constants.DropdownTable.TumourGroup:
                    TumourGroup tGroup = new TumourGroup {
                        Code = entry.code,
                        Value = entry.value
                    };

                    _db.TumourGroups.InsertOnSubmit(tGroup);

                    break;
                case Constants.DropdownTable.UserGroup:
                    UserGroup uGroup = new UserGroup {
                        Code = entry.code,
                        Value = entry.value
                    };

                    _db.UserGroups.InsertOnSubmit(uGroup);

                    break;
            }

            _db.SubmitChanges();
        }

        /// <summary>
        /// Edit a dropdown entry in a given table.
        /// </summary>
        /// <param name="table">Dropdown table enum.</param>
        /// <param name="id">Dropdown entry ID.</param>
        /// <param name="active">True for active, false for inactive.</param>
        public void editEntryStatus(Constants.DropdownTable table,
                                    int id,
                                    bool active) {
            try {
                switch (table) {
                    case Constants.DropdownTable.Keyword:
                        Keyword keyword =
                            (from kw in _db.Keywords
                             where kw.KeywordID == id
                             select kw)
                             .First();

                        keyword.Active = active;
                        break;
                    case Constants.DropdownTable.QuestionType:
                        QuestionType qType =
                            (from qt in _db.QuestionTypes
                             where qt.QuestionTypeID == id
                             select qt)
                             .First();

                        qType.Active = active;
                        break;
                    case Constants.DropdownTable.Region:
                        Region region =
                            (from reg in _db.Regions
                             where reg.RegionID == id
                             select reg)
                             .First();

                        region.Active = active;
                        break;
                    case Constants.DropdownTable.RequestorType:
                        RequestorType rType =
                            (from rt in _db.RequestorTypes
                             where rt.RequestorTypeID == id
                             select rt)
                             .First();

                        rType.Active = active;
                        break;
                    case Constants.DropdownTable.TumourGroup:
                        TumourGroup tGroup =
                            (from tg in _db.TumourGroups
                             where tg.TumourGroupID == id
                             select tg)
                             .First();

                        tGroup.Active = active;
                        break;
                    case Constants.DropdownTable.UserGroup:
                        UserGroup uGroup =
                            (from ug in _db.UserGroups
                             where ug.GroupID == id
                             select ug)
                             .First();

                        uGroup.Active = active;
                        break;
                }

                _db.SubmitChanges();
            }
            catch (InvalidOperationException) {
                // No such entry
                // TODO: Do something
            }
        }
    }
}