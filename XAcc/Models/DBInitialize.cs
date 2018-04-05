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
            var context = new DBContext(sProvider.GetRequiredService<DbContextOptions<DBContext>>());

            if (context.Database.EnsureCreated())
            {
                context.Database.ExecuteSqlCommand("Alter Database " + context.Database.GetDbConnection().Database + " DEFAULT CHARACTER SET='utf8' COLLATE='utf8_general_ci'");
                context.Database.ExecuteSqlCommand("Alter Table " + context.Database.GetDbConnection().Database + ".Stmas CONVERT TO CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'");

                /* Sample data */
                context.Stmas.AddRange(new List<Stmas>
                {
                    new Stmas
                    {
                        ID = 1,
                        stkcod = "01-INTL-CL600",
                        stkdes = "ซีพียูอินเทล เซเลอรอน 600 เมกะเฮิร์ตซ์",
                        stkdes2 = "Cpu Intel Celeron 600 MHz.",
                        sellpr1 = 6500
                    },
                    new Stmas
                    {
                        ID = 2,
                        stkcod = "01-INTL-P4750",
                        stkdes = "ซีพียูอินเทล เพนเทียมโฟร์ 750 เมกะเฮิร์ตซ์",
                        stkdes2 = "Cpu Intel Pentium 4 750 MHz.",
                        sellpr1 = 8700
                    }
                });
                context.SaveChanges();

            }
        }
    }
}
