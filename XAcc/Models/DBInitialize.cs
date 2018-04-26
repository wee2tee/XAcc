using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAcc.Models
{
    public class DBInitialize
    {
        public static void INIT(IServiceProvider sProvider)
        {
            var context_main = new DBMainContext(sProvider.GetRequiredService<DbContextOptions<DBMainContext>>());
            if (context_main.Database.EnsureCreated())
            {
                context_main.Database.ExecuteSqlCommand("Alter Database " + context_main.Database.GetDbConnection().Database + " DEFAULT CHARACTER SET='utf8' COLLATE='utf8_general_ci'");
                context_main.Database.ExecuteSqlCommand("Alter Table " + context_main.Database.GetDbConnection().Database + ".Customer CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");

                context_main.Customer.AddRange(new List<Models.MainDB.Customer>
                {
                    new MainDB.Customer
                    {
                        ID = 1,
                        sernum = "W-10T-000008",
                        prenam = "บริษัท",
                        compnam = "สบายใจ จำกัด",
                        taxid = "1234567890123",
                        orgnum = "00000",
                        server_name = "172.16.2.236",
                        server_port = 50002,
                        mysql_uid = "root",
                        mysql_pwd = "12345",
                        pass_phrase = "xxx"
                    }
                });
                context_main.SaveChanges();
            }

            //var context = new DBContext(sProvider.GetRequiredService<DbContextOptions<DBContext>>());
            //if (context.Database.EnsureCreated())
            //{
            //    context.Database.ExecuteSqlCommand("Alter Database " + context.Database.GetDbConnection().Database + " DEFAULT CHARACTER SET='utf8' COLLATE='utf8_general_ci'");
            //    context.Database.ExecuteSqlCommand("Alter Table " + context.Database.GetDbConnection().Database + ".Stmas CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");

            //    /* Sample data */
            //    context.Stmas.AddRange(new List<Stmas>
            //    {
            //        new Stmas
            //        {
            //            ID = 1,
            //            stkcod = "01-INTL-CL600",
            //            stkdes = "ซีพียูอินเทล เซเลอรอน 600 เมกะเฮิร์ตซ์",
            //            stkdes2 = "Cpu Intel Celeron 600 MHz.",
            //            sellpr1 = 6500
            //        },
            //        new Stmas
            //        {
            //            ID = 2,
            //            stkcod = "01-INTL-P4750",
            //            stkdes = "ซีพียูอินเทล เพนเทียมโฟร์ 750 เมกะเฮิร์ตซ์",
            //            stkdes2 = "Cpu Intel Pentium 4 750 MHz.",
            //            sellpr1 = 8700
            //        }
            //    });
            //    context.SaveChanges();

            //}
        }
    }
}
