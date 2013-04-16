using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;

namespace CAIRSMigrationTool {
    public class MdbToMssqlTool {
        static void Main(string[] args) {
            if (args.Length != 1) {
                Console.WriteLine("Usage: $0 <mdb_file>");
                return;
            }

            var conStr = "Provider=Microsoft.JET.OLEDB.4.0;" +
                         "Data Source=" +
                         args[0];

            OleDbConnection con;
            try {
                con = new OleDbConnection(conStr);
                con.Open();
            } catch (Exception e) {
                Console.WriteLine(
                    "Error: Failed to create a database connection. \n{0}",
                    e.Message);
                return;
            }

            var db = new CAIRSDataContext();

            const string reqSelect = "SELECT * FROM [di requests]";

            var reqCmd = new OleDbCommand(reqSelect, con);
            var reqReader = reqCmd.ExecuteReader();

            if (reqReader == null) {
                Console.WriteLine("Error: Failed to create a data reader to " +
                                  "migrate existing requests.");
                return;
            }

            var up = (from uProfile in db.UserProfiles
                        where uProfile.UserName == "System"
                        select uProfile)
                        .SingleOrDefault();

            if (up == null) {
                // Create a dummy user to assign the requests to
                up = new UserProfile {
                    UserName = "System",
                    UserFullName = "System"
                };

                db.UserProfiles.InsertOnSubmit(up);
                db.SubmitChanges();
            }

            long numReqs = 0, numSuccessfulReqs = 0;
            while (reqReader.Read()) {
                long reqId = -1;

                try {
                    // Contents should follow the order of:
                    // values[0] = RequestID
                    // values[1] = TimeOpened
                    // values[2] = RequestorTypeCode
                    // values[3] = RequestorFName
                    // values[4] = RegionCode
                    // values[5] = TumourGroupCode
                    // values[6] = RequestorEmail/RequestorPhone (parse?)
                    // values[7] = Question
                    // values[8] = Response
                    // values[9] = TimeSpent
                    // values[10] = ReferenceString
                    // values[14] = SpecialNotes
                    // values[15] = UserID
                    // values[16] = QuestionTypeCode
                    // values[17] = Keyword (multiple; comma-delimited)
                    // values[18] = Impact score (reverse compute?)

                    using (var scope = new TransactionScope()) {
                        reqId = Convert.ToInt64(reqReader.GetValue(0));

                        Console.WriteLine("Migrating request #{0}.", reqId);

                        // Check if request ID already exists
                        var req = (from r in db.Requests
                                   where r.RequestID == reqId
                                   select r)
                                  .SingleOrDefault();

                        if (req != null) {
                            Console.WriteLine(
                                "Request #{0} already exists, skipping.", reqId);
                            continue;
                        }

                        var columns = new List<string>();
                        var values = new List<string>();

                        // Set request ID, status, time opened
                        columns.Add("RequestID");
                        values.Add(Convert.ToString(reqId));
                        columns.Add("RequestStatus");
                        values.Add(Convert.ToString(1));
                        columns.Add("TimeOpened");
                        values.Add("'" +
                                   Convert.ToString(reqReader.GetValue(1)) +
                                   "'");

                        DateTime now = DateTime.Now;
                        columns.Add("TimeClosed");
                        values.Add("'" +
                                   Convert.ToString(now) +
                                   "'");

                        // Set requestor type, if possible
                        if (!reqReader.IsDBNull(2)) {
                            // Check if code exists
                            var code = reqReader.GetString(2);

                            var rt = (from rType in db.RequestorTypes
                                      where rType.Code == code
                                      select rType)
                                     .SingleOrDefault();

                            if (rt == null) {
                                // Add new requestor type to database
                                var rtSelect = "SELECT * FROM requestor " +
                                               "WHERE code=" + code;

                                var rtCmd = new OleDbCommand(rtSelect, con);
                                var rtReader = rtCmd.ExecuteReader();

                                // Use value if exists in requestor table
                                var value =
                                    rtReader != null && rtReader.Read() ?
                                    rtReader.GetString(1) : code;

                                rt = new RequestorType {
                                    Code = code,
                                    Value = value,
                                    Active = true
                                };

                                db.RequestorTypes.InsertOnSubmit(rt);
                                db.SubmitChanges();
                            }

                            columns.Add("RequestorTypeID");
                            values.Add(Convert.ToString(rt.RequestorTypeID));
                        }

                        // Set requestor name (as first), if possible
                        if (!reqReader.IsDBNull(3)) {
                            columns.Add("RequestorFName");
                            values.Add(
                                "'" + 
                                Convert.ToString(reqReader.GetString(3)) + 
                                "'");
                        }

                        // Set region, if possible
                        if (!reqReader.IsDBNull(4)) {
                            // Check if code exists
                            var code = reqReader.GetString(4);

                            var reg = (from region in db.Regions
                                       where region.Code == code
                                       select region)
                                      .SingleOrDefault();

                            if (reg == null) {
                                // Add new region to database
                                var regSelect = "SELECT * FROM region" +
                                                "WHERE state_prov=" + code;

                                var regCmd = new OleDbCommand(regSelect, con);
                                var regReader = regCmd.ExecuteReader();

                                // Use value if exists in region table
                                var value =
                                    regReader != null && regReader.Read() ?
                                    regReader.GetString(1) : code;

                                reg = new Region {
                                    Code = code,
                                    Value = value,
                                    Active = true
                                };

                                db.Regions.InsertOnSubmit(reg);
                                db.SubmitChanges();
                            }

                            columns.Add("RegionID");
                            values.Add(Convert.ToString(reg.RegionID));
                        }

                        // Set requestor email/phone number
                        if (!reqReader.IsDBNull(6)) {
                            var str = reqReader.GetString(6);

                            var regex = new Regex(
                                @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                            var match = regex.Match(str);

                            if (match.Success || str.Length > 20) {
                                columns.Add("RequestorEmail");
                                values.Add(
                                    "'" + Convert.ToString(str) + "'");
                            } else {
                                columns.Add("RequestorPhone");
                                values.Add(
                                    "'" + Convert.ToString(str) + "'");
                            }
                        }

                        // Insert new Request entity
                        db.ExecuteCommand("SET IDENTITY_INSERT Request ON");

                        var reqInsert =
                            "INSERT INTO Request (" +
                            string.Join(", ", columns.ToArray()) +
                            ") VALUES (" +
                            string.Join(", ", values.ToArray()) +
                            ");";
                        db.ExecuteCommand(reqInsert);

                        db.ExecuteCommand("SET IDENTITY_INSERT Request OFF");

                        // Create QuestionResponse entity
                        var qr = new QuestionResponse {
                            RequestID = reqId
                        };

                        var emptyStr = "(empty)";

                        // Set tumour group, if possible
                        if (!reqReader.IsDBNull(5)) {
                            // Check if code exists
                            var code = reqReader.GetString(5);

                            var tg = (from tGroup in db.TumourGroups
                                      where tGroup.Code == code
                                      select tGroup)
                                     .SingleOrDefault();

                            if (tg == null) {
                                // Add new tumour group to database
                                var tgSelect = "SELECT * FROM region " +
                                                "WHERE state_prov=" + code;

                                var tgCmd = new OleDbCommand(tgSelect, con);
                                var tgReader = tgCmd.ExecuteReader();

                                // Use value if exists in tumour group table
                                var value =
                                    tgReader != null && tgReader.Read() ?
                                    tgReader.GetString(1) : code;

                                tg = new TumourGroup {
                                    Code = code,
                                    Value = value,
                                    Active = true
                                };

                                db.TumourGroups.InsertOnSubmit(tg);
                                db.SubmitChanges();
                            }

                            qr.TumourGroupID = tg.TumourGroupID;
                        } else {
                            var tgId =
                                (from tGroup in db.TumourGroups
                                 where tGroup.Active
                                 select tGroup.TumourGroupID).SingleOrDefault();

                            qr.TumourGroupID = tgId;
                        }

                        // Set question, if possible
                        qr.Question = !reqReader.IsDBNull(7) ? reqReader.GetString(7) : emptyStr;

                        // Set response, if possible
                        qr.Response = !reqReader.IsDBNull(8) ? reqReader.GetString(8) : emptyStr;

                        // Set time spent, if possible
                        qr.TimeSpent = (short) (!reqReader.IsDBNull(9) ? Convert.ToInt16(reqReader.GetValue(9)) : 0);

                        // Add special notes
                        qr.SpecialNotes = !reqReader.IsDBNull(14) ? reqReader.GetString(14) : emptyStr;

                        // Set question type ID
                        if (!reqReader.IsDBNull(16)) {
                            // Check if code exists
                            var code = reqReader.GetString(16);

                            var qt = (from qTypes in db.QuestionTypes
                                      where qTypes.Code == code
                                      select qTypes)
                                     .SingleOrDefault();

                            if (qt == null) {
                                // Add new question type to database
                                var qtSelect =
                                    "SELECT * FROM [question type] " +
                                    "WHERE code=" + code;

                                var qtCmd = new OleDbCommand(qtSelect, con);
                                var qtReader = qtCmd.ExecuteReader();

                                // Use value if exists in tumour group table
                                var value =
                                    qtReader != null && qtReader.Read() ?
                                    qtReader.GetString(0) : code;

                                qt = new QuestionType {
                                    Code = code,
                                    Value = value,
                                    Active = true
                                };

                                db.QuestionTypes.InsertOnSubmit(qt);
                                db.SubmitChanges();
                            }
                        } else {
                            var qtId =
                                (from qType in db.QuestionTypes
                                 where qType.Active
                                 select qType.QuestionTypeID).SingleOrDefault();

                            qr.QuestionTypeID = qtId;
                        }

                        // Set impact score
                        if (!reqReader.IsDBNull(18)) {
                            var score =
                                Convert.ToInt32(reqReader.GetValue(18));

                            switch (score) {
                                case 1:
                                    qr.Severity = 0;
                                    qr.Consequence = 1;
                                    break;
                                case 2:
                                    qr.Severity = 1;
                                    qr.Consequence = 1;
                                    break;
                                case 3:
                                    qr.Severity = 2;
                                    qr.Consequence = 1;
                                    break;
                                case 4:
                                    qr.Severity = 1;
                                    qr.Consequence = 2;
                                    break;
                                case 5:
                                    qr.Severity = 2;
                                    qr.Consequence = 3;
                                    break;
                                default:
                                    qr.Severity = 2;
                                    qr.Consequence = 3;
                            }
                        } else {
                            qr.Severity = 2;
                            qr.Consequence = 3;
                        }

                        // Insert new QuestionResponse entity
                        db.QuestionResponses.InsertOnSubmit(qr);
                        db.SubmitChanges();

                        var importedKw =
                            (from keyword in db.Keywords
                             where keyword.KeywordValue == "imported"
                             select keyword)
                                .SingleOrDefault();

                        if (importedKw == null) {
                            importedKw = new Keyword {
                                KeywordValue = "imported",
                                Active = true
                            };

                            db.Keywords.InsertOnSubmit(importedKw);
                            db.SubmitChanges();
                        }

                        var importedKq = new KeywordQuestion {
                            KeywordID = importedKw.KeywordID,
                            RequestID = reqId,
                            QuestionResponseID =
                                qr.QuestionResponseID
                        };

                        db.KeywordQuestions.InsertOnSubmit(importedKq);
                        db.SubmitChanges();

                        // Set keyword(s), if possibe
                        if (!reqReader.IsDBNull(17)) {
                            var keywords =
                                reqReader.GetString(17).Split(
                                    new Char[] { ',' });

                            foreach (var str in keywords) {
                                var kwStr = str.Trim();

                                // TODO: how to handle keywords
                                if (!String.IsNullOrEmpty(kwStr)) {
                                    var kw = (from keyword in db.Keywords
                                              where keyword.KeywordValue == kwStr
                                              select keyword)
                                             .SingleOrDefault();

                                    if (kw == null) {
                                        continue;
                                    }

                                    var kq = new KeywordQuestion {
                                        KeywordID = kw.KeywordID,
                                        RequestID = reqId,
                                        QuestionResponseID =
                                            qr.QuestionResponseID
                                    };

                                    db.KeywordQuestions.InsertOnSubmit(kq);
                                    db.SubmitChanges();
                                }
                            }
                        }

                        var reference = new Reference {
                            ReferenceString = !reqReader.IsDBNull(10) ? reqReader.GetString(10) : emptyStr,
                            ReferenceType = 2, // Text
                            RequestID = reqId,
                            QuestionResponseID = qr.QuestionResponseID
                        };

                        db.References.InsertOnSubmit(reference);
                        db.SubmitChanges();

                        var al = new AuditLog {
                            UserID = up.UserId,
                            RequestID = reqId,
                            AuditDate =
                                Convert.ToDateTime(reqReader.GetValue(1)),
                            AuditType = 0
                        };

                        db.AuditLogs.InsertOnSubmit(al);
                        db.SubmitChanges();

                        al = new AuditLog {
                            UserID = up.UserId,
                            RequestID = reqId,
                            AuditDate = (DateTime) now,
                            AuditType = 1
                        };

                        db.AuditLogs.InsertOnSubmit(al);
                        db.SubmitChanges();
                        
                        scope.Complete();
                        numSuccessfulReqs++;
                    }
                } catch (Exception e) {
                    Console.WriteLine(
                        "Error: Failed to migrate request #{0}.\n{1}",
                        reqId, e.Message);
                    break;
                } finally {
                    db.ExecuteCommand("SET IDENTITY_INSERT Request OFF");
                    numReqs++;
                }
            }

            Console.WriteLine("Migration completed! " +
                              "Successfully migrated {0} out of {1} requests.",
                              numSuccessfulReqs, numReqs);
            con.Close();
        }
    }
}