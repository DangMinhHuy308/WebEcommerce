using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShippingName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "Shippings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DistrictName",
                table: "Shippings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WardName",
                table: "Shippings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityName",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "DistrictName",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "WardName",
                table: "Shippings");
        }
    }
}
