using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models.ServiceSystem {
    public sealed class DropdownController {
        private static readonly DropdownController _instance =
            new DropdownController();
        private CAIRSDataContext _db = new CAIRSDataContext();

        public DropdownController() {
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
        /// <param name="activeOnly">True to check only active entries, False to check all</param>
        /// <returns>List of dropdown table entries.</returns>
        public List<DropdownEntry> getEntries(Constants.DropdownTable table, bool activeOnly = true) {
            List<DropdownEntry> list = new List<DropdownEntry>();
            

            switch (table) {
                case Constants.DropdownTable.Keyword:
                    List<Keyword> keywords = activeOnly ? _db.Keywords.Where(kw => kw.Active).ToList() : _db.Keywords.ToList();

                    list.AddRange(keywords.Select(kw => 
                                                  new KeywordEntry(kw.KeywordID, 
                                                                   kw.KeywordValue, 
                                                                   kw.Active)));

                    break;
                case Constants.DropdownTable.QuestionType:
                    List<QuestionType> qTypes = activeOnly ? _db.QuestionTypes.Where(qType => qType.Active).ToList() : _db.QuestionTypes.ToList();

                    list.AddRange(qTypes.Select(qType => 
                        new DropdownEntry(qType.QuestionTypeID, 
                                          qType.Code, 
                                          qType.Value, 
                                          qType.Active)));

                    break;
                case Constants.DropdownTable.Region:
                    List<Region> regions = activeOnly ? _db.Regions.Where(region => region.Active).ToList() : _db.Regions.ToList();

                    list.AddRange(regions.Select(region => 
                        new DropdownEntry(region.RegionID, 
                                          region.Code, 
                                          region.Value, 
                                          region.Active)));

                    break;
                case Constants.DropdownTable.RequestorType:
                    List<RequestorType> rTypes = activeOnly ? _db.RequestorTypes.Where(rType => rType.Active).ToList() : _db.RequestorTypes.ToList();

                    list.AddRange(rTypes.Select(rType => 
                        new DropdownEntry(rType.RequestorTypeID, 
                                          rType.Code, 
                                          rType.Value, 
                                          rType.Active)));

                    break;
                case Constants.DropdownTable.TumourGroup:
                    List<TumourGroup> tGroups = activeOnly ? _db.TumourGroups.Where(tGroup => tGroup.Active).ToList() : _db.TumourGroups.ToList();

                    list.AddRange(tGroups.Select(tGroup => 
                        new DropdownEntry(tGroup.TumourGroupID, 
                                          tGroup.Code, 
                                          tGroup.Value, 
                                          tGroup.Active)));

                    break;
                case Constants.DropdownTable.UserGroup:
                    List<UserGroup> uGroups = activeOnly ? _db.UserGroups.Where(uGroup => uGroup.Active).ToList() : _db.UserGroups.ToList();

                    list.AddRange(uGroups.Select(uGroup => 
                        new DropdownEntry(uGroup.GroupID, 
                                          uGroup.Code, 
                                          uGroup.Value, 
                                          uGroup.Active)));

                    break;
            }

            return list.OrderBy(dd => dd.code).ToList();
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
                    Keyword kw = new Keyword();
                    kw.KeywordValue = entry.value;

                    _db.Keywords.InsertOnSubmit(kw);
                    break;
                case Constants.DropdownTable.QuestionType:
                    QuestionType qType = new QuestionType();
                    qType.Code = entry.code;
                    qType.Value = entry.value;

                    _db.QuestionTypes.InsertOnSubmit(qType);

                    break;
                case Constants.DropdownTable.Region:
                    Region region = new Region();
                    region.Code = entry.code;
                    region.Value = entry.value;

                    _db.Regions.InsertOnSubmit(region);

                    break;
                case Constants.DropdownTable.RequestorType:
                    RequestorType rType = new RequestorType();
                    rType.Code = entry.code;
                    rType.Value = entry.value;

                    _db.RequestorTypes.InsertOnSubmit(rType);

                    break;
                case Constants.DropdownTable.TumourGroup:
                    TumourGroup tGroup = new TumourGroup();
                    tGroup.Code = entry.code;
                    tGroup.Value = entry.value;

                    _db.TumourGroups.InsertOnSubmit(tGroup);

                    break;
                case Constants.DropdownTable.UserGroup:
                    UserGroup uGroup = new UserGroup();
                    uGroup.Code = entry.code;
                    uGroup.Value = entry.value;

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
            catch (InvalidOperationException ioEx) {
                // No such entry
                // TODO: Do something
            }
        }

        /// <summary>
        /// Edit an already-existing dropdown entry
        /// </summary>
        /// <param name="table">The table the entry is under</param>
        /// <param name="code">The new code of the entry</param>
        /// <param name="value">The new value of the entry</param>
        /// <param name="active">The new status of the entry</param>
        public void createEntry(Constants.DropdownTable table, string code,
                                string value, string active) {
            switch (table) {
                case Constants.DropdownTable.Keyword:
                    _db.Keywords.InsertOnSubmit(new Keyword {
                        KeywordValue = value,
                        Active = Convert.ToBoolean(active)
                    });
                    break;
                case Constants.DropdownTable.QuestionType:
                    _db.QuestionTypes.InsertOnSubmit(new QuestionType {
                        Code = code,
                        Value = value,
                        Active = Convert.ToBoolean(active)
                    });
                    break;
                case Constants.DropdownTable.Region:
                    _db.Regions.InsertOnSubmit(new Region {
                        Code = code,
                        Value = value,
                        Active = Convert.ToBoolean(active)
                    });
                    break;
                case Constants.DropdownTable.RequestorType:
                    _db.RequestorTypes.InsertOnSubmit(new RequestorType {
                        Code = code,
                        Value = value,
                        Active = Convert.ToBoolean(active)
                    });
                    break;
                case Constants.DropdownTable.TumourGroup:
                    _db.TumourGroups.InsertOnSubmit(new TumourGroup {
                        Code = code,
                        Value = value,
                        Active = Convert.ToBoolean(active)
                    });
                    break;
                case Constants.DropdownTable.UserGroup:
                    _db.UserGroups.InsertOnSubmit(new UserGroup {
                        Code = code,
                        Value = value,
                        Active = Convert.ToBoolean(active)
                    });
                    break;
            }

            _db.SubmitChanges();
        }

        /// <summary>
        /// Edit an already-existing dropdown entry
        /// </summary>
        /// <param name="table">The table the entry is under</param>
        /// <param name="id">The ID of the entry</param>
        /// <param name="code">The new code of the entry</param>
        /// <param name="value">The new value of the entry</param>
        /// <param name="active">The new status of the entry</param>
        public void editEntry(Constants.DropdownTable table, int id, string code, 
            string value, string active) {
            switch (table) {
                case Constants.DropdownTable.Keyword:
                    Keyword keyword =
                        _db.Keywords.First(kw => kw.KeywordID == id);
                    keyword.KeywordValue = value;
                    keyword.Active = Convert.ToBoolean(active);
                    break;
                case Constants.DropdownTable.QuestionType:
                    QuestionType qt =
                        _db.QuestionTypes.First(q => q.QuestionTypeID == id);
                    qt.Code = code;
                    qt.Value = value;
                    qt.Active = Convert.ToBoolean(active);
                    break;
                case Constants.DropdownTable.Region:
                    Region r = _db.Regions.First(reg => reg.RegionID == id);
                    r.Code = code;
                    r.Value = value;
                    r.Active = Convert.ToBoolean(active);
                    break;
                case Constants.DropdownTable.RequestorType:
                    RequestorType rt =
                        _db.RequestorTypes.First(
                            req => req.RequestorTypeID == id);
                    rt.Code = code;
                    rt.Value = value;
                    rt.Active = Convert.ToBoolean(active);
                    break;
                case Constants.DropdownTable.TumourGroup:
                    TumourGroup tg =
                        _db.TumourGroups.First(tum => tum.TumourGroupID == id);
                    tg.Code = code;
                    tg.Value = value;
                    tg.Active = Convert.ToBoolean(active);
                    break;
                case Constants.DropdownTable.UserGroup:
                    UserGroup ug = _db.UserGroups.First(usg => usg.GroupID == id);
                    ug.Code = code;
                    ug.Value = value;
                    ug.Active = Convert.ToBoolean(active);
                    break;
            }

            _db.SubmitChanges();
        }
    }
}