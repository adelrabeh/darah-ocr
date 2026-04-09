using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using DarahOcr.Api.Data;

#nullable disable

namespace DarahOcr.Api.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.0")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity("DarahOcr.Api.Models.User", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd();
            b.Property<string>("Username").IsRequired();
            b.Property<string>("Email").IsRequired();
            b.Property<string>("PasswordHash").IsRequired();
            b.Property<string>("Role").IsRequired().HasDefaultValue("user");
            b.Property<bool>("IsActive").HasDefaultValue(true);
            b.Property<DateTime>("CreatedAt");
            b.Property<DateTime?>("LastLoginAt");
            b.HasKey("Id");
            b.HasIndex("Username").IsUnique();
            b.HasIndex("Email").IsUnique();
            b.ToTable("Users");
        });

        modelBuilder.Entity("DarahOcr.Api.Models.OcrJob", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd();
            b.Property<int>("UserId");
            b.Property<string>("OriginalFilename").IsRequired();
            b.Property<string>("StoredFilename").IsRequired();
            b.Property<string>("FileType").IsRequired();
            b.Property<long>("FileSize");
            b.Property<string>("Status").IsRequired().HasDefaultValue("pending");
            b.Property<string>("ErrorMessage");
            b.Property<DateTime>("CreatedAt");
            b.Property<DateTime?>("StartedAt");
            b.Property<DateTime?>("CompletedAt");
            b.HasKey("Id");
            b.HasIndex("UserId");
            b.ToTable("OcrJobs");
        });

        modelBuilder.Entity("DarahOcr.Api.Models.OcrResult", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd();
            b.Property<int>("JobId");
            b.Property<string>("RawText").IsRequired();
            b.Property<string>("RefinedText");
            b.Property<int>("ConfidenceScore");
            b.Property<string>("QualityLevel").IsRequired();
            b.Property<int>("WordCount");
            b.Property<int>("PageCount");
            b.Property<string>("ProcessingNotes");
            b.Property<string>("OcrEngine").IsRequired().HasDefaultValue("tesseract");
            b.Property<DateTime>("CreatedAt");
            b.HasKey("Id");
            b.HasIndex("JobId").IsUnique();
            b.ToTable("OcrResults");
        });
    }
}
