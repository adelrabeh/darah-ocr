namespace DarahOcr.Api.Models;

public class OcrResult
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public OcrJob Job { get; set; } = null!;
    public string RawText { get; set; } = "";
    public string? RefinedText { get; set; }
    public int ConfidenceScore { get; set; }
    public string QualityLevel { get; set; } = "";
    public int WordCount { get; set; }
    public int PageCount { get; set; }
    public string? ProcessingNotes { get; set; }
    public string OcrEngine { get; set; } = "tesseract";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
