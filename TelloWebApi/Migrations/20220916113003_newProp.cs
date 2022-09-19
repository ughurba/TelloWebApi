using Microsoft.EntityFrameworkCore.Migrations;

namespace TelloWebApi.Migrations
{
    public partial class newProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "BasketItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Storage",
                table: "BasketItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "BasketItems");

            migrationBuilder.DropColumn(
                name: "Storage",
                table: "BasketItems");
        }
    }
}
