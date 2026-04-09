using System;

namespace DarahOcr.Api.Models
{
    public class BatchOcrResult
    {
        public int Id { get; set; }
        public int BatchOcrJobId { get; set; }
        public BatchOcrJob BatchOcrJob { get; set; }
        
        public int BatchOcrFileId { get; set; }
        public BatchOcrFile BatchOcrFile { get; set; }
        
        // Extracted text
        public string ExtractedText { get; set; }
        
        // Confidence score from Tesseract
        public decimal ConfidenceScore { get; set; } // 0-100
        
        // Processing details
        public int ProcessingTimeMs { get; set; }
        public string Language { get; set; } = "ara"; // Arabic
        
        // Quality metrics
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public decimal? TextDensity { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}