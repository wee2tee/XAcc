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
                    });

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
                    dbacc_context.SaveChanges();
                }
                return dbacc_context;
            }
            else
            {
                return null;
            }
        }

        //public static DBSecureContext EnsureDbSecureCreated(this Controller controller, DBMainContext dbmain_context, IConfiguration configuration)
        //{
        //    if (controller.User == null)
        //        return null;


        //    var identity = controller.User.Identity as ClaimsIdentity;
        //    var sernum = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;

        //    Customer customer = dbmain_context.Customer.Where(c => c.sernum.Trim() == sernum.Trim()).FirstOrDefault();

        //    if (customer != null)
        //    {
        //        string sec_conn_str = configuration.GetSection("ConnectionStrings")["AccConnection"];
        //        sec_conn_str = sec_conn_str.Replace("{ServerName}", customer.server_name);
        //        sec_conn_str = sec_conn_str.Replace("{ServerPort}", customer.server_port.ToString());
        //        sec_conn_str = sec_conn_str.Replace("{Uid}", customer.mysql_uid);
        //        sec_conn_str = sec_conn_str.Replace("{Pwd}", customer.mysql_pwd);
        //        sec_conn_str = sec_conn_str.Replace("{DbName}", "accsecure");


        //        DbContextOptionsBuilder<DBSecureContext> optionsBuilder = new DbContextOptionsBuilder<DBSecureContext>().UseMySQL(sec_conn_str);

        //        DBSecureContext dbsecure_context = new DBSecureContext(optionsBuilder.Options);

        //        if (dbsecure_context.Database.EnsureCreated())
        //        {
        //            dbsecure_context.Database.ExecuteSqlCommand("Alter Database " + dbsecure_context.Database.GetDbConnection().Database + " DEFAULT CHARACTER SET='utf8' COLLATE='utf8_general_ci'");
        //            dbsecure_context.Database.ExecuteSqlCommand("Alter Table " + dbsecure_context.Database.GetDbConnection().Database + ".Scuser CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");
        //            dbsecure_context.Database.ExecuteSqlCommand("Alter Table " + dbsecure_context.Database.GetDbConnection().Database + ".Sccomp CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");

        //            /* Initial data */
        //            dbsecure_context.Scuser.AddRange(new List<Scuser>
        //            {
        //                new Scuser
        //                {
        //                    ID = 1,
        //                    authlev = 9,
        //                    connectgrp = "*",
        //                    language = "T",
        //                    laspwd = null,
        //                    lasusedat = null,
        //                    lasusetim = string.Empty,
        //                    laswrk = null,
        //                    prnnum = "0",
        //                    reccod = "BIT9",
        //                    recdes = "รหัสประจำตัวผู้ใช้ระดับ 9",
        //                    rectyp = "U",
        //                    resumedat = null,
        //                    revokedat = null,
        //                    rwttxt = string.Empty,
        //                    secure = "0",
        //                    status = "N",
        //                    userattr = 0,
        //                    userpwd = "BIT9"
        //                }
        //            });

        //            dbsecure_context.Sccomp.AddRange(new List<Sccomp>
        //            {
        //                new Sccomp
        //                {
        //                    ID = 1,
        //                    candel = "Y",
        //                    compcod = "DATAT",
        //                    compnam = "ข้อมูลทดสอบ",
        //                    dbname = "test",
        //                    credat = DateTime.Now
        //                },
        //                new Sccomp
        //                {
        //                    ID = 2,
        //                    candel = "N",
        //                    compcod = "DATAZ",
        //                    compnam = "ข้อมูลเปล่า",
        //                    dbname = "dat",
        //                    credat = DateTime.Now
        //                }
        //            });
        //            dbsecure_context.SaveChanges();

        //            /* Create dat, test DB */
        //            dbsecure_context.Sccomp.ToList().ForEach(s => controller.EnsureDbAccContextCreated(dbmain_context, configuration, s));
        //        }
        //        return dbsecure_context;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //public static DBAccContext EnsureDbAccContextCreated(this Controller controller, DBMainContext dbmain_context, IConfiguration configuration, Sccomp sccomp)
        //{
        //    if (controller.User == null)
        //        return null;

        //    var identity = controller.User.Identity as ClaimsIdentity;
        //    var sernum = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;

        //    Customer customer = dbmain_context.Customer.Where(c => c.sernum.Trim() == sernum.Trim()).FirstOrDefault();

        //    if (sccomp != null)
        //    {
        //        string acc_conn_str = configuration.GetSection("ConnectionStrings")["AccConnection"];
        //        acc_conn_str = acc_conn_str.Replace("{ServerName}", customer.server_name);
        //        acc_conn_str = acc_conn_str.Replace("{ServerPort}", customer.server_port.ToString());
        //        acc_conn_str = acc_conn_str.Replace("{Uid}", customer.mysql_uid);
        //        acc_conn_str = acc_conn_str.Replace("{Pwd}", customer.mysql_pwd);
        //        acc_conn_str = acc_conn_str.Replace("{DbName}", sccomp.dbname);

        //        DbContextOptionsBuilder<DBAccContext> optionsBuilder = new DbContextOptionsBuilder<DBAccContext>().UseMySQL(acc_conn_str);

        //        DBAccContext dbacc_context = new DBAccContext(optionsBuilder.Options);

        //        if (dbacc_context.Database.EnsureCreated())
        //        {
        //            dbacc_context.Database.ExecuteSqlCommand("Alter Database " + dbacc_context.Database.GetDbConnection().Database + " DEFAULT CHARACTER SET='utf8' COLLATE='utf8_general_ci'");
        //            dbacc_context.Database.ExecuteSqlCommand("Alter Table " + dbacc_context.Database.GetDbConnection().Database + ".Stmas CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");

        //            /* Initial data */
        //            if (sccomp.dbname != "test")
        //                return dbacc_context;

        //            dbacc_context.Stmas.AddRange(new List<Stmas>
        //                {
        //                    new Stmas
        //                    {
        //                        ID = 1,
        //                        stkcod = "01-INTL-CL600",
        //                        stkdes = "ซีพียูอินเทล เซเลอรอน 600 เมกะเฮิร์ตซ์",
        //                        stkdes2 = "Cpu Intel Celeron 600 MHz.",
        //                        sellpr1 = 6500,
        //                    },
        //                    new Stmas
        //                    {
        //                        ID = 2,
        //                        stkcod = "01-INTL-P4750",
        //                        stkdes = "ซีพียูอินเทล เพนเทียมโฟร์ 750 เมกะเฮิร์ตซ์",
        //                        stkdes2 = "Cpu Intel Pentium 4 750 MHz.",
        //                        sellpr1 = 8700
        //                    }
        //                });
        //            dbacc_context.SaveChanges();
        //        }
        //        return dbacc_context;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}
