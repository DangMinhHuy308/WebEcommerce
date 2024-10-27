using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class SaleProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSale",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSale",
                table: "Products");
        }
    }
}
