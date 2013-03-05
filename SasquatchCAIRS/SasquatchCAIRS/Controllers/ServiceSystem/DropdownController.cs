using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SasquatchCAIRS.Models.ServiceSystem {
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

        public List<DropdownEntry> getActiveEntries(Constants.DropdownTable table) {
            List<DropdownEntry> list = new List<DropdownEntry>();

            switch (table) {
                case Constants.DropdownTable.Keyword:
                    List<Keyword> keywords =
                        (from kw in _db.Keywords
                         where kw.Active == true
                         select kw)
                         .ToList();

                    foreach (Keyword kw in keywords) {
                        list.Add(new DropdownEntry(kw.KeywordID,
                                                   null,
                                                   kw.KeywordValue,
                                                   kw.Active));
                    }

                    break;
                case Constants.DropdownTable.QuestionType:
                    List<QuestionType> qTypes =
                        (from qType in _db.QuestionTypes
                         where qType.Active == true
                         select qType)
                         .ToList();

                    foreach (QuestionType qType in qTypes) {
                        list.Add(new DropdownEntry(qType.QuestionTypeID,
                                                   qType.Code,
                                                   qType.Value,
                                                   qType.Active));
                    }

                    break;
                case Constants.DropdownTable.Region:
                    List<Region> regions =
                        (from region in _db.Regions
                         where region.Active == true
                         select region)
                         .ToList();

                    foreach (Region region in regions) {
                        list.Add(new DropdownEntry(region.RegionID,
                                                   region.Code,
                                                   region.Value,
                                                   region.Active));
                    }

                    break;
                case Constants.DropdownTable.RequestorType:
                    List<RequestorType> rTypes =
                        (from rType in _db.RequestorTypes
                         where rType.Active == true
                         select rType)
                         .ToList();

                    foreach (RequestorType rType in rTypes) {
                        list.Add(new DropdownEntry(rType.RequestorTypeID,
                                                   rType.Code,
                                                   rType.Value,
                                                   rType.Active));
                    }

                    break;
                case Constants.DropdownTable.TumourGroup:
                    List<TumourGroup> tGroups =
                        (from tGroup in _db.TumourGroups
                         where tGroup.Active == true
                         select tGroup)
                         .ToList();

                    foreach (TumourGroup tGroup in tGroups) {
                        list.Add(new DropdownEntry(tGroup.TumourGroupID,
                                                   tGroup.Code,
                                                   tGroup.Value,
                                                   tGroup.Active));
                    }

                    break;
                case Constants.DropdownTable.UserGroup:
                    List<UserGroup> uGroups =
                        (from uGroup in _db.UserGroups
                         where uGroup.Active == true
                         select uGroup)
                         .ToList();

                    foreach (UserGroup uGroup in uGroups) {
                        list.Add(new DropdownEntry(uGroup.GroupID,
                                                   uGroup.Code,
                                                   uGroup.Value,
                                                   uGroup.Active));
                    }

                    break;
            }

            return list;
        }

        /// <summary>
        /// Add a new keyword to the database.
        /// </summary>
        /// <param name="value">The keyword string.</param>
        public void addKeyword(string value) {
            addEntry(Constants.DropdownTable.Keyword, null, value);
        }

        /// <summary>
        /// Add a new entry to one of the dropdown tables in the database.
        /// </summary>
        /// <param name="table">Table to add a new entry to.</param>
        /// <param name="code">
        /// The code associated with the entry. Keyword entries have no code.
        /// </param>
        /// <param name="value">The value associated with the entry.</param>
        public void addEntry(Constants.DropdownTable table,
                             string code,
                             string value) {
            switch (table) {
                case Constants.DropdownTable.Keyword:
                    Keyword kw = new Keyword();
                    kw.KeywordValue = value;

                    _db.Keywords.InsertOnSubmit(kw);
                    break;
                case Constants.DropdownTable.QuestionType:
                    QuestionType qType = new QuestionType();
                    qType.Code = code;
                    qType.Value = value;

                    _db.QuestionTypes.InsertOnSubmit(qType);

                    break;
                case Constants.DropdownTable.Region:
                    Region region = new Region();
                    region.Code = code;
                    region.Value = value;

                    _db.Regions.InsertOnSubmit(region);

                    break;
                case Constants.DropdownTable.RequestorType:
                    RequestorType rType = new RequestorType();
                    rType.Code = code;
                    rType.Value = value;

                    _db.RequestorTypes.InsertOnSubmit(rType);

                    break;
                case Constants.DropdownTable.TumourGroup:
                    TumourGroup tGroup = new TumourGroup();
                    tGroup.Code = code;
                    tGroup.Value = value;

                    _db.TumourGroups.InsertOnSubmit(tGroup);

                    break;
                case Constants.DropdownTable.UserGroup:
                    UserGroup uGroup = new UserGroup();
                    uGroup.Code = code;
                    uGroup.Value = value;

                    _db.UserGroups.InsertOnSubmit(uGroup);

                    break;
            }

            _db.SubmitChanges();
        }

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
    }
}