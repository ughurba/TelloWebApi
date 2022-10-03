using Microsoft.EntityFrameworkCore.Migrations;

namespace TelloWebApi.Migrations
{
    public partial class addFiledOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Storage",
                table: "OrderItems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Storage",
                table: "OrderItems");
        }
    }
}
