using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderToSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Sections",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Sections");
        }
    }
}
