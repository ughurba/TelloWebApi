using Microsoft.EntityFrameworkCore.Migrations;

namespace TelloWebApi.Migrations
{
    public partial class ordeup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Building",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Cash",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Building",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Cash",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "Orders");
        }
    }
}
