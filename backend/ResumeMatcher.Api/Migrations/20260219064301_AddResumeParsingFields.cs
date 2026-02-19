using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeMatcher.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeParsingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ParsedAtUtc",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParsedResumeJson",
                table: "Documents",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParsedAtUtc",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ParsedResumeJson",
                table: "Documents");
        }
    }
}
