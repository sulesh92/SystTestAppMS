using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TestAppSysTech;

namespace TestAppSysTech.Migrations
{
    [DbContext(typeof(DataModelContext))]
    [Migration("20180124224631_FirstMigration")]
    partial class FirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<int?>("GroupId");

                    b.Property<bool>("IsRoot");

                    b.Property<string>("Login")
                        .HasMaxLength(40);

                    b.Property<string>("Name")
                        .HasMaxLength(40);

                    b.Property<string>("Password")
                        .HasMaxLength(40);

                    b.Property<string>("Supervisor");

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

                    b.Property<int?>("PersonId");

                    b.Property<string>("Year");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("Salaries");
                });

            modelBuilder.Entity("TestAppSysTech.Person", b =>
                {
                    b.HasOne("TestAppSysTech.Group", "Group")
                        .WithMany("Persons")
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("TestAppSysTech.Salary", b =>
                {
                    b.HasOne("TestAppSysTech.Person", "Person")
                        .WithMany("SalaryHistory")
                        .HasForeignKey("PersonId");
                });
        }
    }
}
