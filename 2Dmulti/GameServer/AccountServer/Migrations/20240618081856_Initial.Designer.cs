﻿// <auto-generated />
using AccountServer.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace WebApplication1.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240618081856_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AccountServer.DB.AccountDb", b =>
                {
                    b.Property<int>("AccountDbId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AccountDbId"));

                    b.Property<int>("LoginProviderType")
                        .HasColumnType("int");

                    b.Property<string>("LoginProviderUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AccountDbId");

                    b.HasIndex("LoginProviderUserId", "LoginProviderType")
                        .IsUnique();

                    b.ToTable("Account");
                });
#pragma warning restore 612, 618
        }
    }
}
