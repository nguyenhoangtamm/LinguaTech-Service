using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDetailedDescriptionToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DetailedDescription",
                table: "Courses",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailedDescription",
                table: "Courses");
        }
    }
}
