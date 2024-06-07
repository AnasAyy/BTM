using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTMBackend.Migrations
{
    /// <inheritdoc />
    public partial class m6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "WhoWeAres",
                newName: "DescriptionEn");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "WhoWeAres",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "WhoWeAres");

            migrationBuilder.RenameColumn(
                name: "DescriptionEn",
                table: "WhoWeAres",
                newName: "Description");
        }
    }
}
