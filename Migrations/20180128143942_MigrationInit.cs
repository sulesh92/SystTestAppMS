using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestAppSysTech.Migrations
{
    public partial class MigrationInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Limit = table.Column<double>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SubNumberCoeff = table.Column<double>(nullable: false),
                    YearsCoefficient = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BaseSalary = table.Column<double>(nullable: false),
                    DateOfStart = table.Column<DateTimeOffset>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    IsRoot = table.Column<bool>(nullable: false),
                    Login = table.Column<string>(maxLength: 40, nullable: true),
                    Name = table.Column<string>(maxLength: 40, nullable: true),
                    Password = table.Column<string>(maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Salaries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CurrentSalary = table.Column<double>(nullable: false),
                    Group = table.Column<string>(nullable: true),
                    Month = table.Column<string>(nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    Year = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salaries_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subordinates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Group = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OwnPersonId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subordinates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subordinates_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_GroupId",
                table: "Persons",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_PersonId",
                table: "Salaries",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Subordinates_PersonId",
                table: "Subordinates",
                column: "PersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Salaries");

            migrationBuilder.DropTable(
                name: "Subordinates");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
