﻿// <auto-generated />
using System;
using DAL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(WebStoreContext))]
    partial class WebStoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebAPI.Model.BasketPosition", b =>
                {
                    b.Property<int>("ProductID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.HasKey("ProductID", "UserID");

                    b.HasIndex("UserID");

                    b.ToTable("BasketPositions");
                });

            modelBuilder.Entity("WebAPI.Model.Order", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("WebAPI.Model.OrderPosition", b =>
                {
                    b.Property<int>("OrderID")
                        .HasColumnType("int");

                    b.Property<int>("ProductID")
                        .HasColumnType("int");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.HasKey("OrderID", "ProductID");

                    b.HasIndex("ProductID");

                    b.ToTable("OrderPositions");
                });

            modelBuilder.Entity("WebAPI.Model.Product", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("GroupID")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int?>("ProductGroupID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ProductGroupID");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("WebAPI.Model.ProductGroup", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentGroupID")
                        .HasColumnType("int");

                    b.Property<int?>("ParentID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ParentGroupID");

                    b.ToTable("ProductGroups");
                });

            modelBuilder.Entity("WebAPI.Model.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("GroupID")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserGroupID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("UserGroupID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebAPI.Model.UserGroup", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("UserGroups");
                });

            modelBuilder.Entity("WebAPI.Model.BasketPosition", b =>
                {
                    b.HasOne("WebAPI.Model.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebAPI.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebAPI.Model.Order", b =>
                {
                    b.HasOne("WebAPI.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebAPI.Model.OrderPosition", b =>
                {
                    b.HasOne("WebAPI.Model.Order", "Order")
                        .WithMany("OrderPositions")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebAPI.Model.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("WebAPI.Model.Product", b =>
                {
                    b.HasOne("WebAPI.Model.ProductGroup", "ProductGroup")
                        .WithMany("Products")
                        .HasForeignKey("ProductGroupID");

                    b.Navigation("ProductGroup");
                });

            modelBuilder.Entity("WebAPI.Model.ProductGroup", b =>
                {
                    b.HasOne("WebAPI.Model.ProductGroup", "ParentGroup")
                        .WithMany()
                        .HasForeignKey("ParentGroupID");

                    b.Navigation("ParentGroup");
                });

            modelBuilder.Entity("WebAPI.Model.User", b =>
                {
                    b.HasOne("WebAPI.Model.UserGroup", "UserGroup")
                        .WithMany("Users")
                        .HasForeignKey("UserGroupID");

                    b.Navigation("UserGroup");
                });

            modelBuilder.Entity("WebAPI.Model.Order", b =>
                {
                    b.Navigation("OrderPositions");
                });

            modelBuilder.Entity("WebAPI.Model.ProductGroup", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("WebAPI.Model.UserGroup", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
