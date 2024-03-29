﻿// <auto-generated />
using Gudgeon.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Gudgeon.Data.Migrations
{
    [DbContext(typeof(GudgeonDbContext))]
    [Migration("20230322200430_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Gudgeon.Data.Models.Autorole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("RoleId")
                        .HasColumnType("bigint unsigned");

                    b.Property<int>("UsersType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GuildId", "UsersType")
                        .IsUnique();

                    b.ToTable("Autoroles");
                });
#pragma warning restore 612, 618
        }
    }
}
