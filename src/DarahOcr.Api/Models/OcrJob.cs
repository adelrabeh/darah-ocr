namespace DarahOcr.Api.Models;

public class OcrJob
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string OriginalFilename { get; set; } = "";
    public string StoredFilename { get; set; } = "";
    public string FileType { get; set; } = "";
    public long FileSize { get; set; }
    public string Status { get; set; } = "pending";
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public OcrResult? Result { get; set; }
}
