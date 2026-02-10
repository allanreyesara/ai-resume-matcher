using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeMatcher.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingUploadAndSizeBytesToDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SizeBytes",
                table: "Documents",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SizeBytes",
                table: "Documents");
        }
    }
}
