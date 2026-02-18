using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeMatcher.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentNormalizedText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedExtractedText",
                table: "Documents",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizedExtractedText",
                table: "Documents");
        }
    }
}
