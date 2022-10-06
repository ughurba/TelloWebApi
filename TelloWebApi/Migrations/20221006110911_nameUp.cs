using Microsoft.EntityFrameworkCore.Migrations;

namespace TelloWebApi.Migrations
{
    public partial class nameUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorits_Products_ProductId",
                table: "Favorits");

            migrationBuilder.DropColumn(
                name: "PrdocutId",
                table: "Favorits");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Favorits",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorits_Products_ProductId",
                table: "Favorits",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorits_Products_ProductId",
                table: "Favorits");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Favorits",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "PrdocutId",
                table: "Favorits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorits_Products_ProductId",
                table: "Favorits",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
