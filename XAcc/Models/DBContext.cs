using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XAcc.Models
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder mBuilder)
        {
            mBuilder.Entity<Stmas>().Property(p => p.sellpr1).HasColumnType<decimal>("decimal(18,4)");
        }

        public DbSet<Stmas> Stmas { get; set; }
    }
}
