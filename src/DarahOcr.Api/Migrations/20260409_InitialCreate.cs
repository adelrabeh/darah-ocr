using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DarahOcr.Api.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Username = table.Column<string>(nullable: false),
                Email = table.Column<string>(nullable: false),
                PasswordHash = table.Column<string>(nullable: false),
                Role = table.Column<string>(nullable: false, defaultValue: "user"),
                IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                CreatedAt = table.Column<DateTime>(nullable: false),
                LastLoginAt = table.Column<DateTime>(nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_Users", x => x.Id));

        migrationBuilder.CreateIndex("IX_Users_Username", "Users", "Username", unique: true);
        migrationBuilder.CreateIndex("IX_Users_Email", "Users", "Email", unique: true);

        migrationBuilder.CreateTable(
            name: "OcrJobs",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<int>(nullable: false),
                OriginalFilename = table.Column<string>(nullable: false),
                StoredFilename = table.Column<string>(nullable: false),
                FileType = table.Column<string>(nullable: false),
                FileSize = table.Column<long>(nullable: false),
                Status = table.Column<string>(nullable: false, defaultValue: "pending"),
                ErrorMessage = table.Column<string>(nullable: true),
                CreatedAt = table.Column<DateTime>(nullable: false),
                StartedAt = table.Column<DateTime>(nullable: true),
                CompletedAt = table.Column<DateTime>(nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OcrJobs", x => x.Id);
                table.ForeignKey("FK_OcrJobs_Users", x => x.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_OcrJobs_UserId", "OcrJobs", "UserId");

        migrationBuilder.CreateTable(
            name: "OcrResults",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false).Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                JobId = table.Column<int>(nullable: false),
                RawText = table.Column<string>(nullable: false),
                RefinedText = table.Column<string>(nullable: true),
                ConfidenceScore = table.Column<int>(nullable: false),
                QualityLevel = table.Column<string>(nullable: false),
                WordCount = table.Column<int>(nullable: false),
                PageCount = table.Column<int>(nullable: false),
                ProcessingNotes = table.Column<string>(nullable: true),
                OcrEngine = table.Column<string>(nullable: false, defaultValue: "tesseract"),
                CreatedAt = table.Column<DateTime>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OcrResults", x => x.Id);
                table.ForeignKey("FK_OcrResults_Jobs", x => x.JobId, "OcrJobs", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_OcrResults_JobId", "OcrResults", "JobId", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("OcrResults");
        migrationBuilder.DropTable("OcrJobs");
        migrationBuilder.DropTable("Users");
    }
}
