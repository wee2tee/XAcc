using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using XAcc.Controllers;
using XAcc.Models;
using XAcc.Models.MainDB;
using XAcc.Models.Secure;

namespace XAcc.Models
{
    public static class Helper
    {
        public static string GetIdentityClaimValue(this ControllerExtend controller, string claimtype)
        {
            if (controller.User.Identity.IsAuthenticated)
            {
                var claim = controller.User.Identities.First().Claims.Where(c => c.Type == claimtype/*System.Security.Claims.ClaimTypes.SerialNumber*/).FirstOrDefault();
                return claim != null ? claim.Value.ToString() : string.Empty;
            }
            else
            {
                return null;
            }
        }

        public static Customer GetCustomerIdentity(this Controller controller, DBMainContext dbmain_context)
        {
            if (controller.User.Identity.IsAuthenticated)
            {
                var identity = controller.User.Identity as ClaimsIdentity;
                var sernum = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;

                Customer customer = dbmain_context.Customer.Where(c => c.sernum.Trim() == sernum.Trim()).FirstOrDefault();
                return customer;
            }
            else
            {
                return null;
            }
        }

        public static Customer GetCustomerIdentity(this ControllerExtend controller)
        {
            if (controller.User.Identity.IsAuthenticated)
            {
                var identity = controller.User.Identity as ClaimsIdentity;
                var sernum = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;

                Customer customer = controller.dbmain_context.Customer.Where(c => c.sernum.Trim() == sernum.Trim()).FirstOrDefault();
                return customer;
            }
            else
            {
                return null;
            }
        }

        public static bool PrepareDbContext(this ControllerExtend controller)
        {
            if (controller.User.Identity.IsAuthenticated)
            {
                controller.dbsecure_context = controller.GetCustomerIdentity().EnsureDbSecureCreated(controller.configuration);

                var dbname = controller.GetIdentityClaimValue(ClaimTypes.UserData);
                if (!string.IsNullOrEmpty(dbname))
                {
                    var sccomp = controller.dbsecure_context.Sccomp.Where(s => s.dbname == dbname).FirstOrDefault();
                    if(sccomp != null)
                    {
                        controller.dbacc_context = controller.GetCustomerIdentity().EnsureDbAccContextCreated(controller.configuration, sccomp);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static DBSecureContext EnsureDbSecureCreated(this Customer customer, IConfiguration configuration)
        {
            if (customer == null)
                return null;

            string sec_conn_str = configuration.GetSection("ConnectionStrings")["AccConnection"];
            sec_conn_str = sec_conn_str.Replace("{ServerName}", customer.server_name);
            sec_conn_str = sec_conn_str.Replace("{ServerPort}", customer.server_port.ToString());
            sec_conn_str = sec_conn_str.Replace("{Uid}", customer.mysql_uid);
            sec_conn_str = sec_conn_str.Replace("{Pwd}", customer.mysql_pwd);
            sec_conn_str = sec_conn_str.Replace("{DbName}", "accsecure");


            DbContextOptionsBuilder<DBSecureContext> optionsBuilder = new DbContextOptionsBuilder<DBSecureContext>().UseMySQL(sec_conn_str);

            DBSecureContext dbsecure_context = new DBSecureContext(optionsBuilder.Options);

            if (dbsecure_context.Database.EnsureCreated())
            {
                dbsecure_context.Database.ExecuteSqlCommand("Alter Database " + dbsecure_context.Database.GetDbConnection().Database + " DEFAULT CHARACTER SET='utf8' COLLATE='utf8_general_ci'");
                dbsecure_context.Database.ExecuteSqlCommand("Alter Table " + dbsecure_context.Database.GetDbConnection().Database + ".Scuser CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");
                dbsecure_context.Database.ExecuteSqlCommand("Alter Table " + dbsecure_context.Database.GetDbConnection().Database + ".Sccomp CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");
                dbsecure_context.Database.ExecuteSqlCommand("Alter Table " + dbsecure_context.Database.GetDbConnection().Database + ".Scmodul CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");

                /* Initial data */
                dbsecure_context.Scuser.AddRange(new List<Scuser>
                    {
                        new Scuser
                        {
                            ID = 1,
                            authlev = 9,
                            connectgrp = "*",
                            language = "T",
                            laspwd = null,
                            lasusedat = null,
                            lasusetim = string.Empty,
                            laswrk = null,
                            prnnum = "0",
                            reccod = "BIT9",
                            recdes = "รหัสประจำตัวผู้ใช้ระดับ 9",
                            rectyp = "U",
                            resumedat = null,
                            revokedat = null,
                            rwttxt = string.Empty,
                            secure = "0",
                            status = "N",
                            userattr = 0,
                            userpwd = "BIT9"
                        }
                    }
                );

                dbsecure_context.Sccomp.AddRange(new List<Sccomp>
                    {
                        new Sccomp
                        {
                            ID = 1,
                            candel = "Y",
                            compcod = "DATAT",
                            compnam = "ข้อมูลทดสอบ",
                            dbname = "test",
                            credat = DateTime.Now
                        },
                        new Sccomp
                        {
                            ID = 2,
                            candel = "N",
                            compcod = "DATAZ",
                            compnam = "ข้อมูลเปล่า",
                            dbname = "dat",
                            credat = DateTime.Now
                        }
                    }
                );

                dbsecure_context.Scmodul.AddRange(new List<Scmodul>
                {
                    new Scmodul
                    {
                        modul = "Account",
                        parent_modul = "",
                        desc_th = "บัญชี",
                        desc_en = "Account",
                        doctyp = "",
                        controller_name = "",
                        action_name = ""
                    },
                    new Scmodul
                    {
                        modul = "Account|1",
                        parent_modul = "Account",
                        desc_th = "บันทึกรายการประจำวัน",
                        desc_en = "Journal Entries",
                        doctyp = "",
                        controller_name = "",
                        action_name = ""
                    },
                    new Scmodul
                    {
                        modul = "Account|2",
                        parent_modul = "Account",
                        desc_th = "ผังบัญชี",
                        desc_en = "Chart of Accounts",
                        doctyp = "",
                        controller_name = "Glacc",
                        action_name = "Index"
                    },
                    new Scmodul
                    {
                        modul = "Account|1|1",
                        parent_modul = "Account|1",
                        desc_th = "รายการที่ยังไม่ผ่านบัญชี",
                        desc_en = "Unpost Transaction",
                        doctyp = "",
                        controller_name = "Gljnl",
                        action_name = "IndexUnpost"
                    },
                    new Scmodul
                    {
                        modul = "Account|1|2",
                        parent_modul = "Account|1",
                        desc_th = "รายการที่ผ่านบัญชีแล้ว",
                        desc_en = "Posted Transaction",
                        doctyp = "",
                        controller_name = "Gljnl",
                        action_name = "IndexPosted"
                    }
                });

                dbsecure_context.SaveChanges();

                /* Create dat, test DB */
                dbsecure_context.Sccomp.ToList().ForEach(s => customer.EnsureDbAccContextCreated(configuration, s));
            }
            return dbsecure_context;

        }

        public static DBAccContext EnsureDbAccContextCreated(this Customer customer, IConfiguration configuration, Sccomp sccomp)
        {
            if (sccomp != null)
            {
                string acc_conn_str = configuration.GetSection("ConnectionStrings")["AccConnection"];
                acc_conn_str = acc_conn_str.Replace("{ServerName}", customer.server_name);
                acc_conn_str = acc_conn_str.Replace("{ServerPort}", customer.server_port.ToString());
                acc_conn_str = acc_conn_str.Replace("{Uid}", customer.mysql_uid);
                acc_conn_str = acc_conn_str.Replace("{Pwd}", customer.mysql_pwd);
                acc_conn_str = acc_conn_str.Replace("{DbName}", sccomp.dbname);

                DbContextOptionsBuilder<DBAccContext> optionsBuilder = new DbContextOptionsBuilder<DBAccContext>().UseMySQL(acc_conn_str);

                DBAccContext dbacc_context = new DBAccContext(optionsBuilder.Options);

                if (dbacc_context.Database.EnsureCreated())
                {
                    dbacc_context.Database.ExecuteSqlCommand("Alter Database " + dbacc_context.Database.GetDbConnection().Database + " DEFAULT CHARACTER SET='utf8' COLLATE='utf8_general_ci'");
                    dbacc_context.Database.ExecuteSqlCommand("Alter Table " + dbacc_context.Database.GetDbConnection().Database + ".Stmas CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");
                    dbacc_context.Database.ExecuteSqlCommand("Alter Table " + dbacc_context.Database.GetDbConnection().Database + ".Glacc CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");
                    dbacc_context.Database.ExecuteSqlCommand("Alter Table " + dbacc_context.Database.GetDbConnection().Database + ".Gljnl CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");
                    dbacc_context.Database.ExecuteSqlCommand("Alter Table " + dbacc_context.Database.GetDbConnection().Database + ".Gljnlit CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");
                    dbacc_context.Database.ExecuteSqlCommand("Alter Table " + dbacc_context.Database.GetDbConnection().Database + ".Istab CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");

                    /* Initial data */
                    if (sccomp.dbname != "test")
                        return dbacc_context;

                    dbacc_context.Stmas.AddRange(new List<Stmas>
                    {
                        new Stmas
                        {
                            id = 1,
                            stkcod = "01-INTL-CL600",
                            stkdes = "ซีพียูอินเทล เซเลอรอน 600 เมกะเฮิร์ตซ์",
                            stkdes2 = "Cpu Intel Celeron 600 MHz.",
                            sellpr1 = 6500,
                        },
                        new Stmas
                        {
                            id = 2,
                            stkcod = "01-INTL-P4750",
                            stkdes = "ซีพียูอินเทล เพนเทียมโฟร์ 750 เมกะเฮิร์ตซ์",
                            stkdes2 = "Cpu Intel Pentium 4 750 MHz.",
                            sellpr1 = 8700
                        }
                    });

                    dbacc_context.Glacc.AddRange(new List<Glacc>
                    {
                        new Glacc
                        {
                            id = 1,
                            accnum = "1000-00",
                            accnam = "สินทรัพย์",
                            accnam2 = "Assets",
                            level = 1,
                            parent = null,
                            group = "1",
                            acctyp = "1",
                            usedep = "N",
                            usejob = "N",
                            nature = "0",
                            consol = "",
                            status = "A",
                            creby = "BIT9",
                            credat = DateTime.Now,
                            chgby = null,
                            chgdat = null
                        },
                        new Glacc
                        {
                            id = 2,
                            accnum = "1100-00",
                            accnam = "สินทรัพย์หมุนเวียน",
                            accnam2 = "Current Assets",
                            level = 2,
                            parent = "1000-00",
                            group = "1",
                            acctyp = "1",
                            usedep = "N",
                            usejob = "N",
                            nature = "0",
                            consol = "",
                            status = "A",
                            creby = "BIT9",
                            credat = DateTime.Now,
                            chgby = null,
                            chgdat = null
                        },
                        new Glacc
                        {
                            id = 3,
                            accnum = "1110-00",
                            accnam = "เงินสดและเงินฝากธนาคาร",
                            accnam2 = "Cash on Hand and at Banks",
                            level = 3,
                            parent = "1100-00",
                            group = "1",
                            acctyp = "1",
                            usedep = "N",
                            usejob = "N",
                            nature = "0",
                            consol = "",
                            status = "A",
                            creby = "BIT9",
                            credat = DateTime.Now,
                            chgby = null,
                            chgdat = null
                        },
                        new Glacc
                        {
                            id = 4,
                            accnum = "1111-00",
                            accnam = "เงินสด",
                            accnam2 = "Cash",
                            level = 4,
                            parent = "1110-00",
                            group = "1",
                            acctyp = "0",
                            usedep = "N",
                            usejob = "N",
                            nature = "0",
                            consol = "",
                            status = "A",
                            creby = "BIT9",
                            credat = DateTime.Now,
                            chgby = null,
                            chgdat = null
                        },
                        new Glacc
                        {
                            id = 5,
                            accnum = "1130-00",
                            accnam = "ลูกหนี้การค้าและตั๋วเงินรับ",
                            accnam2 = "Accounts and Notes Receivable",
                            level = 3,
                            parent = "1100-00",
                            group = "1",
                            acctyp = "1",
                            usedep = "N",
                            usejob = "N",
                            nature = "0",
                            consol = "",
                            status = "A",
                            creby = "BIT9",
                            credat = DateTime.Now,
                            chgby = null,
                            chgdat = null
                        },
                        new Glacc
                        {
                            id = 6,
                            accnum = "1130-01",
                            accnam = "ลูกหนี้การค้า",
                            accnam2 = "Accounts Receivable - Trade",
                            level = 4,
                            parent = "1130-00",
                            group = "1",
                            acctyp = "0",
                            usedep = "N",
                            usejob = "N",
                            nature = "0",
                            consol = "",
                            status = "A",
                            creby = "BIT9",
                            credat = DateTime.Now,
                            chgby = null,
                            chgdat = null
                        },
                        new Glacc
                        {
                            id = 7,
                            accnum = "1130-02",
                            accnam = "เช็ครับลงวันที่ล่วงหน้า",
                            accnam2 = "Post-dated cheques Received",
                            level = 4,
                            parent = "1130-00",
                            group = "1",
                            acctyp = "0",
                            usedep = "N",
                            usejob = "N",
                            nature = "0",
                            consol = "",
                            status = "A",
                            creby = "BIT9",
                            credat = DateTime.Now,
                            chgby = null,
                            chgdat = null
                        },
                        new Glacc
                        {
                            id = 8,
                            accnum = "2000-00",
                            accnam = "หนี้สิน",
                            accnam2 = "Liabilities and Shareholders' Equity",
                            level = 1,
                            parent = null,
                            group = "2",
                            acctyp = "1",
                            usedep = "N",
                            usejob = "N",
                            nature = "1",
                            consol = "",
                            status = "A",
                            creby = "BIT9",
                            credat = DateTime.Now,
                            chgby = null,
                            chgdat = null
                        },
                        new Glacc
                        {
                            id = 9,
                            accnum = "2100-00",
                            accnam = "หนี้สินหมุนเวียน",
                            accnam2 = "Current Liabilities",
                            level = 2,
                            parent = "2000-00",
                            group = "2",
                            acctyp = "1",
                            usedep = "N",
                            usejob = "N",
                            nature = "1",
                            consol = "",
                            status = "A",
                            creby = "BIT9",
                            credat = DateTime.Now,
                            chgby = null,
                            chgdat = null
                        },
                        new Glacc
                        {
                            id = 10,
                            accnum = "2120-00",
                            accnam = "เจ้าหนี้การค้าและตั๋วเงินจ่าย",
                            accnam2 = "Accounts and Notes Payable",
                            level = 3,
                            parent = "2100-00",
                            group = "2",
                            acctyp = "1",
                            usedep = "N",
                            usejob = "N",
                            nature = "1",
                            consol = "",
                            status = "A",
                            creby = "BIT9",
                            credat = DateTime.Now,
                            chgby = null,
                            chgdat = null
                        },
                        new Glacc
                        {
                            id = 11,
                            accnum = "2120-01",
                            accnam = "เจ้าหนี้การค้า",
                            accnam2 = "Accounts Payable",
                            level = 4,
                            parent = "2120-00",
                            group = "2",
                            acctyp = "0",
                            usedep = "N",
                            usejob = "N",
                            nature = "1",
                            consol = "",
                            status = "A",
                            creby = "BIT9",
                            credat = DateTime.Now,
                            chgby = null,
                            chgdat = null
                        }
                    });

                    dbacc_context.Istab.AddRange(new List<Istab>
                    {
                        new Istab
                        {
                            chgby = null,
                            chgdat = null,
                            creby = "BIT9",
                            credat = DateTime.Now,
                            depcod = string.Empty,
                            fld01 = string.Empty,
                            fld02 = 0,
                            shortnam = "ธ.กรุงเทพ",
                            shortnam2 = "Bangkok Bank",
                            status = string.Empty,
                            tabtyp = Istab.TABTYP.BNKCOD.GetTabtypCode(),
                            typcod = "BBL",
                            typdes = "บมจ.ธนาคารกรุงเทพ",
                            typdes2 = "Bangkok Bank PLC."
                        },
                        new Istab
                        {
                            chgby = null,
                            chgdat = null,
                            creby = "BIT9",
                            credat = DateTime.Now,
                            depcod = string.Empty,
                            fld01 = string.Empty,
                            fld02 = 0,
                            shortnam = "ธ.กสิกรไทย",
                            shortnam2 = "Kasikorn Thai Bank",
                            status = string.Empty,
                            tabtyp = Istab.TABTYP.BNKCOD.GetTabtypCode(),
                            typcod = "KBANK",
                            typdes = "บมจ.ธนาคารกสิกรไทย",
                            typdes2 = "Kasikorn Thai Bank PLC."
                        },
                        new Istab
                        {
                            chgby = null,
                            chgdat = null,
                            creby = "BIT9",
                            credat = DateTime.Now,
                            depcod = string.Empty,
                            fld01 = string.Empty,
                            fld02 = 0,
                            shortnam = "ธ.กรุงเทพ",
                            shortnam2 = "Bangkok Bank",
                            status = string.Empty,
                            tabtyp = Istab.TABTYP.BNKCOD.GetTabtypCode(),
                            typcod = "BBL",
                            typdes = "บมจ.ธนาคารกรุงเทพ",
                            typdes2 = "Bangkok Bank PLC."
                        },
                        new Istab
                        {
                            chgby = null,
                            chgdat = null,
                            creby = "BIT9",
                            credat = DateTime.Now,
                            depcod = string.Empty,
                            fld01 = string.Empty,
                            fld02 = 0,
                            shortnam = "ธ.กสิกรไทย",
                            shortnam2 = "Kasikorn Thai Bank",
                            status = string.Empty,
                            tabtyp = Istab.TABTYP.BNKCOD.GetTabtypCode(),
                            typcod = "KBANK",
                            typdes = "บมจ.ธนาคารกสิกรไทย",
                            typdes2 = "Kasikorn Thai Bank PLC."
                        },
                        new Istab
                        {
                            chgby = null,
                            chgdat = null,
                            creby = "BIT9",
                            credat = DateTime.Now,
                            depcod = string.Empty,
                            fld01 = string.Empty,
                            fld02 = 0,
                            shortnam = "ธ.กรุงเทพ",
                            shortnam2 = "Bangkok Bank",
                            status = string.Empty,
                            tabtyp = Istab.TABTYP.BNKCOD.GetTabtypCode(),
                            typcod = "BBL",
                            typdes = "บมจ.ธนาคารกรุงเทพ",
                            typdes2 = "Bangkok Bank PLC."
                        },
                        new Istab
                        {
                            chgby = null,
                            chgdat = null,
                            creby = "BIT9",
                            credat = DateTime.Now,
                            depcod = string.Empty,
                            fld01 = string.Empty,
                            fld02 = 0,
                            shortnam = "ธ.กสิกรไทย",
                            shortnam2 = "Kasikorn Thai Bank",
                            status = string.Empty,
                            tabtyp = Istab.TABTYP.BNKCOD.GetTabtypCode(),
                            typcod = "KBANK",
                            typdes = "บมจ.ธนาคารกสิกรไทย",
                            typdes2 = "Kasikorn Thai Bank PLC."
                        },
                        new Istab
                        {
                            chgby = null,
                            chgdat = null,
                            creby = "BIT9",
                            credat = DateTime.Now,
                            depcod = string.Empty,
                            fld01 = string.Empty,
                            fld02 = 0,
                            shortnam = "ธ.กรุงเทพ",
                            shortnam2 = "Bangkok Bank",
                            status = string.Empty,
                            tabtyp = Istab.TABTYP.BNKCOD.GetTabtypCode(),
                            typcod = "BBL",
                            typdes = "บมจ.ธนาคารกรุงเทพ",
                            typdes2 = "Bangkok Bank PLC."
                        },
                        new Istab
                        {
                            chgby = null,
                            chgdat = null,
                            creby = "BIT9",
                            credat = DateTime.Now,
                            depcod = string.Empty,
                            fld01 = string.Empty,
                            fld02 = 0,
                            shortnam = "ธ.กสิกรไทย",
                            shortnam2 = "Kasikorn Thai Bank",
                            status = string.Empty,
                            tabtyp = Istab.TABTYP.BNKCOD.GetTabtypCode(),
                            typcod = "KBANK",
                            typdes = "บมจ.ธนาคารกสิกรไทย",
                            typdes2 = "Kasikorn Thai Bank PLC."
                        },
                        new Istab
                        {
                            chgby = null,
                            chgdat = null,
                            creby = "BIT9",
                            credat = DateTime.Now,
                            depcod = string.Empty,
                            fld01 = string.Empty,
                            fld02 = 0,
                            shortnam = "ธ.กรุงเทพ",
                            shortnam2 = "Bangkok Bank",
                            status = string.Empty,
                            tabtyp = Istab.TABTYP.BNKCOD.GetTabtypCode(),
                            typcod = "BBL",
                            typdes = "บมจ.ธนาคารกรุงเทพ",
                            typdes2 = "Bangkok Bank PLC."
                        },
                        new Istab
                        {
                            chgby = null,
                            chgdat = null,
                            creby = "BIT9",
                            credat = DateTime.Now,
                            depcod = string.Empty,
                            fld01 = string.Empty,
                            fld02 = 0,
                            shortnam = "ธ.กสิกรไทย",
                            shortnam2 = "Kasikorn Thai Bank",
                            status = string.Empty,
                            tabtyp = Istab.TABTYP.BNKCOD.GetTabtypCode(),
                            typcod = "KBANK",
                            typdes = "บมจ.ธนาคารกสิกรไทย",
                            typdes2 = "Kasikorn Thai Bank PLC."
                        }
                    });

                    dbacc_context.SaveChanges();
                }
                return dbacc_context;
            }
            else
            {
                return null;
            }
        }

        public static string GetTabtypCode(this Istab.TABTYP tabtyp)
        {
            switch (tabtyp)
            {
                case Istab.TABTYP.BNKCOD:
                    return "01";
                case Istab.TABTYP.CHQSTAT:
                    return "02";
                case Istab.TABTYP.QUCOD:
                    return "20";
                case Istab.TABTYP.BRANCH:
                    return "21";
                case Istab.TABTYP.STKGRP:
                    return "22";
                case Istab.TABTYP.STKLEVEL:
                    return "23";
                case Istab.TABTYP.AREA:
                    return "40";
                case Istab.TABTYP.DLVBY:
                    return "41";
                case Istab.TABTYP.CANCEL_SO_REASON:
                    return "42";
                case Istab.TABTYP.CANCEL_PO_REASON:
                    return "43";
                case Istab.TABTYP.QT_STATUS:
                    return "44";
                case Istab.TABTYP.CUSTYP:
                    return "45";
                case Istab.TABTYP.SUPTYP:
                    return "46";
                case Istab.TABTYP.SLMTYP:
                    return "47";
                case Istab.TABTYP.DEPARTMENT:
                    return "50";
                case Istab.TABTYP.FASGRP:
                    return "52";
                case Istab.TABTYP.PCASH_STATUS:
                    return "53";
                case Istab.TABTYP.TAXGRP:
                    return "55";
                default:
                    return "01";
            }
        }

        public static string GetTabtypDesc(this Istab.TABTYP tabtyp)
        {
            switch (tabtyp)
            {
                case Istab.TABTYP.BNKCOD:
                    return "รหัสธนาคาร";
                case Istab.TABTYP.CHQSTAT:
                    return "สถานะเช็ค";
                case Istab.TABTYP.QUCOD:
                    return "หน่วยนับ";
                case Istab.TABTYP.BRANCH:
                    return "สาขา";
                case Istab.TABTYP.STKGRP:
                    return "หมวดสินค้า";
                case Istab.TABTYP.STKLEVEL:
                    return "ระดับสินค้า";
                case Istab.TABTYP.AREA:
                    return "เขตการขาย";
                case Istab.TABTYP.DLVBY:
                    return "ขนส่งโดย";
                case Istab.TABTYP.CANCEL_SO_REASON:
                    return "เหตุผลการยกเลิกใบสั่งขาย";
                case Istab.TABTYP.CANCEL_PO_REASON:
                    return "เหตุผลการยกเลิกใบสั่งซื้อ";
                case Istab.TABTYP.QT_STATUS:
                    return "สถานะใบเสนอราคา";
                case Istab.TABTYP.CUSTYP:
                    return "ประเภทลูกค้า";
                case Istab.TABTYP.SUPTYP:
                    return "ประเภทผู้จำหน่าย";
                case Istab.TABTYP.SLMTYP:
                    return "ประเภทพนักงานขาย";
                case Istab.TABTYP.DEPARTMENT:
                    return "แผนก";
                case Istab.TABTYP.FASGRP:
                    return "หมวดทรัพย์สิน";
                case Istab.TABTYP.PCASH_STATUS:
                    return "สถานะใบเบิกเงินสดย่อย";
                case Istab.TABTYP.TAXGRP:
                    return "หมวดภาษีหัก ณ ที่จ่าย";
                default:
                    return "รหัสธนาคาร";
            }
        }
    }
}
