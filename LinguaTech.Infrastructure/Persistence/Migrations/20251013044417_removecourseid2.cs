using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaTech.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removecourseid2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseId2",
                table: "Classes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId2",
                table: "Classes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
