using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguaTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubmissionAndAnswerEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Score",
                table: "Submissions",
                type: "double precision",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AddColumn<DateTime>(
                name: "GradedAt",
                table: "Submissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Submissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Submissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "Answers",
                type: "boolean",
                nullable: true,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "Answers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "Answers",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelectedOptionId",
                table: "Answers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_SelectedOptionId",
                table: "Answers",
                column: "SelectedOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_QuestionOptions_SelectedOptionId",
                table: "Answers",
                column: "SelectedOptionId",
                principalTable: "QuestionOptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_QuestionOptions_SelectedOptionId",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_SelectedOptionId",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "GradedAt",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "SelectedOptionId",
                table: "Answers");

            migrationBuilder.AlterColumn<double>(
                name: "Score",
                table: "Submissions",
                type: "double precision",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "Answers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true,
                oldDefaultValue: false);
        }
    }
}
