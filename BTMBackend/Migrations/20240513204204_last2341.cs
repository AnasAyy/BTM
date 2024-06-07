using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTMBackend.Migrations
{
    /// <inheritdoc />
    public partial class last2341 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CustomerProducts_CustomerId",
                table: "CustomerProducts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProducts_OrderId",
                table: "CustomerProducts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProducts_ProductId",
                table: "CustomerProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProductParts_CustomerProductId",
                table: "CustomerProductParts",
                column: "CustomerProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProductParts_OrderId",
                table: "CustomerProductParts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerProductParts_PartId",
                table: "CustomerProductParts",
                column: "PartId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProductParts_CustomerProducts_CustomerProductId",
                table: "CustomerProductParts",
                column: "CustomerProductId",
                principalTable: "CustomerProducts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProductParts_Orders_OrderId",
                table: "CustomerProductParts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProductParts_Parts_PartId",
                table: "CustomerProductParts",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProducts_Customers_CustomerId",
                table: "CustomerProducts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProducts_Orders_OrderId",
                table: "CustomerProducts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerProducts_Products_ProductId",
                table: "CustomerProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProductParts_CustomerProducts_CustomerProductId",
                table: "CustomerProductParts");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProductParts_Orders_OrderId",
                table: "CustomerProductParts");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProductParts_Parts_PartId",
                table: "CustomerProductParts");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProducts_Customers_CustomerId",
                table: "CustomerProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProducts_Orders_OrderId",
                table: "CustomerProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerProducts_Products_ProductId",
                table: "CustomerProducts");

            migrationBuilder.DropIndex(
                name: "IX_CustomerProducts_CustomerId",
                table: "CustomerProducts");

            migrationBuilder.DropIndex(
                name: "IX_CustomerProducts_OrderId",
                table: "CustomerProducts");

            migrationBuilder.DropIndex(
                name: "IX_CustomerProducts_ProductId",
                table: "CustomerProducts");

            migrationBuilder.DropIndex(
                name: "IX_CustomerProductParts_CustomerProductId",
                table: "CustomerProductParts");

            migrationBuilder.DropIndex(
                name: "IX_CustomerProductParts_OrderId",
                table: "CustomerProductParts");

            migrationBuilder.DropIndex(
                name: "IX_CustomerProductParts_PartId",
                table: "CustomerProductParts");
        }
    }
}
