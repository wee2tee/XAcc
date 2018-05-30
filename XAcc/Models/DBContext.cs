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

        protected override void OnModelCreating(ModelBuilder mBuilder)
        {
            /* Scuser */
            mBuilder.Entity<Scuser>().Property(p => p.connectgrp).HasColumnType("CHAR(10)");
            mBuilder.Entity<Scuser>().Property(p => p.language).HasColumnType("CHAR(2)");
            mBuilder.Entity<Scuser>().Property(p => p.lasusetim).HasColumnType("CHAR(8)");
            mBuilder.Entity<Scuser>().Property(p => p.prnnum).HasColumnType("CHAR(4)");
            mBuilder.Entity<Scuser>().Property(p => p.reccod).HasColumnType("CHAR(10)");
            mBuilder.Entity<Scuser>().Property(p => p.recdes).HasColumnType("VARCHAR(50)");
            mBuilder.Entity<Scuser>().Property(p => p.rectyp).HasColumnType("CHAR(1)");
            mBuilder.Entity<Scuser>().Property(p => p.rwttxt).HasColumnType("VARCHAR(20)");
            mBuilder.Entity<Scuser>().Property(p => p.secure).HasColumnType("CHAR(1)");
            mBuilder.Entity<Scuser>().Property(p => p.status).HasColumnType("CHAR(1)");
            mBuilder.Entity<Scuser>().Property(p => p.userattr).HasColumnType("int(1)");
            mBuilder.Entity<Scuser>().Property(p => p.userpwd).HasColumnType("CHAR(50)");

            /* Sccomp */
            mBuilder.Entity<Sccomp>().Property(p => p.candel).HasColumnType("char(1)");
            mBuilder.Entity<Sccomp>().Property(p => p.compcod).HasColumnType("char(8)");
            mBuilder.Entity<Sccomp>().Property(p => p.compnam).HasColumnType("varchar(50)");
            mBuilder.Entity<Sccomp>().Property(p => p.dbname).HasColumnType("char(30)");

            /* Scmodul */
            mBuilder.Entity<Scmodul>().Property(p => p.action_name).HasColumnType("varchar(20)");
            mBuilder.Entity<Scmodul>().Property(p => p.controller_name).HasColumnType("varchar(20)");
            mBuilder.Entity<Scmodul>().Property(p => p.desc_en).HasColumnType("varchar(50)");
            mBuilder.Entity<Scmodul>().Property(p => p.desc_th).HasColumnType("varchar(50)");
            mBuilder.Entity<Scmodul>().Property(p => p.doctyp).HasColumnType("char(2)");
            mBuilder.Entity<Scmodul>().Property(p => p.modul).HasColumnType("char(30)");
            mBuilder.Entity<Scmodul>().Property(p => p.parent_modul).HasColumnType("char(30)");
        }

        public DbSet<Scuser> Scuser { get; set; }
        public DbSet<Sccomp> Sccomp { get; set; }
        public DbSet<Scmodul> Scmodul { get; set; }
    }

    public class DBAccContext : DbContext
    {
        public DBAccContext(DbContextOptions<DBAccContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder mBuilder)
        {
            /* Glacc */
            mBuilder.Entity<Glacc>().Property(p => p.accnam).HasColumnType("varchar(50)");
            mBuilder.Entity<Glacc>().Property(p => p.accnam2).HasColumnType("varchar(50)");
            mBuilder.Entity<Glacc>().Property(p => p.accnum).HasColumnType("char(15)");
            mBuilder.Entity<Glacc>().Property(p => p.acctyp).HasColumnType("varchar(1)");
            mBuilder.Entity<Glacc>().Property(p => p.chgby).HasColumnType("varchar(10)");
            mBuilder.Entity<Glacc>().Property(p => p.consol).HasColumnType("varchar(15)");
            mBuilder.Entity<Glacc>().Property(p => p.creby).HasColumnType("varchar(10)");
            mBuilder.Entity<Glacc>().Property(p => p.group).HasColumnType("varchar(1)");
            mBuilder.Entity<Glacc>().Property(p => p.level).HasColumnType("int(3)");
            mBuilder.Entity<Glacc>().Property(p => p.nature).HasColumnType("varchar(1)");
            mBuilder.Entity<Glacc>().Property(p => p.parent).HasColumnType("varchar(15)");
            mBuilder.Entity<Glacc>().Property(p => p.status).HasColumnType("varchar(1)");
            mBuilder.Entity<Glacc>().Property(p => p.usedep).HasColumnType("varchar(1)");
            mBuilder.Entity<Glacc>().Property(p => p.usejob).HasColumnType("varchar(1)");

            mBuilder.Entity<Stmas>().Property(p => p.sellpr1).HasColumnType<decimal>("decimal(18,4)");
        }

        public DbSet<Stmas> Stmas { get; set; }
        public DbSet<Glacc> Glacc { get; set; }

    }

    public class DBMainContext : DbContext
    {
        public DBMainContext(DbContextOptions<DBMainContext> options) : base(options)
        {

        }

        public DbSet<Customer> Customer { get; set; }
    }
}
