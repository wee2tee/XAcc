﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Storage.Internal;
using System;
using XAcc.Models;

namespace XAcc.Migrations
{
    [DbContext(typeof(DBContext))]
    [Migration("25610402095819_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011");

            modelBuilder.Entity("XAcc.Models.Stmas", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("sellpr1")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("stkcod");

                    b.Property<string>("stkdes");

                    b.Property<string>("stkdes2");

                    b.HasKey("ID");

                    b.ToTable("Stmas");
                });
#pragma warning restore 612, 618
        }
    }
}
