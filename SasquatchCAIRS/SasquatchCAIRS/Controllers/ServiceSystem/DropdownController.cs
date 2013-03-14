﻿using System;
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
        /// <returns>List of dropdown table entries.</returns>
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
                        list.Add(new KeywordEntry(kw.KeywordID,
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
    }
}