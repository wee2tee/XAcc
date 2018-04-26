using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XAcc.Models.MainDB;
using XAcc.Models.Secure;
using XAcc.Models;

namespace XAcc.Models
{
    //public class DBContext : DbContext
    //{
    //    public DBContext(DbContextOptions<DBContext> options) : base(options)
    //    {

    //    }

    //    protected override void OnModelCreating(ModelBuilder mBuilder)
    //    {
    //        mBuilder.Entity<Stmas>().Property(p => p.sellpr1).HasColumnType<decimal>("decimal(18,4)");
    //    }

    //    public DbSet<Stmas> Stmas { get; set; }
    //}

    //public class DBContext2 : DbContext
    //{
    //    public DBContext2(DbContextOptions<DBContext2> options) : base(options)
    //    {

    //    }

    //    protected override void OnModelCreating(ModelBuilder mBuilder)
    //    {
    //        mBuilder.Entity<Stmas>().Property(p => p.sellpr1).HasColumnType<decimal>("decimal(18,4)");
    //    }

    //    public DbSet<Stmas> Stmas { get; set; }
    //}

    public class DBSecureContext : DbContext
    {
        public DBSecureContext(DbContextOptions<DBSecureContext> options) : base(options)
        {

        }

        public DbSet<Scuser> Scuser { get; set; }
        public DbSet<Sccomp> Sccomp { get; set; }
    }

    public class DBAccContext : DbContext
    {
        public DBAccContext(DbContextOptions<DBAccContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder mBuilder)
        {
            mBuilder.Entity<Stmas>().Property(p => p.sellpr1).HasColumnType<decimal>("decimal(18,4)");
        }

        public DbSet<Stmas> Stmas { get; set; }

    }

    public class DBMainContext : DbContext
    {
        public DBMainContext(DbContextOptions<DBMainContext> options) : base(options)
        {

        }

        public DbSet<Customer> Customer { get; set; }
    }
}
