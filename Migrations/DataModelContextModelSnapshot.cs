using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TestAppSysTech;

namespace TestAppSysTech.Migrations
{
    [DbContext(typeof(DataModelContext))]
    partial class DataModelContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.5");

            modelBuilder.Entity("TestAppSysTech.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("TestAppSysTech.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("BaseSalary");

                    b.Property<DateTimeOffset>("DateOfStart");

                    b.Property<int>("GroupId");

                    b.Property<bool>("IsRoot");

                    b.Property<string>("Login")
                        .HasMaxLength(40);

                    b.Property<string>("Name")
                        .HasMaxLength(40);

                    b.Property<string>("Password")
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Persons");
                });

            modelBuilder.Entity("TestAppSysTech.Salary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("CurrentSalary");

                    b.Property<string>("Group");

                    b.Property<string>("Month");

                    b.Property<int>("NumberOfSubordinates");

                    b.Property<int>("PersonId");

                    b.Property<string>("Year");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("Salaries");
                });

            modelBuilder.Entity("TestAppSysTech.Subordinate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Group");

                    b.Property<string>("Name");

                    b.Property<int>("PersonId");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("Subordinates");
                });

            modelBuilder.Entity("TestAppSysTech.Person", b =>
                {
                    b.HasOne("TestAppSysTech.Group", "Group")
                        .WithMany("Persons")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TestAppSysTech.Salary", b =>
                {
                    b.HasOne("TestAppSysTech.Person", "Person")
                        .WithMany("SalaryHistory")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TestAppSysTech.Subordinate", b =>
                {
                    b.HasOne("TestAppSysTech.Person", "Person")
                        .WithMany("Subordinates")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
