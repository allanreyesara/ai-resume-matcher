using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeMatcher.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentExtractedTextSaved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtractedText",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtractionErrorMessage",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtractionStatus",
                table: "Documents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtractedText",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ExtractionErrorMessage",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ExtractionStatus",
                table: "Documents");
        }
    }
}
