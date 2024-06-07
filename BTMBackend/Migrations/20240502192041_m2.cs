using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTMBackend.Migrations
{
    /// <inheritdoc />
    public partial class m2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ExternalLinks");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ExternalLinks",
                newName: "NameEn");

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "ExternalLinks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AdSliders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AdSliders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "OTPMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExpirationDatetime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPMessages", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OTPMessages");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "ExternalLinks");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AdSliders");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AdSliders");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "ExternalLinks",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ExternalLinks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
