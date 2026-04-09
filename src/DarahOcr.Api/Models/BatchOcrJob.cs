using System;
using System.Collections.Generic;

namespace DarahOcr.Api.Models
{
    public class BatchOcrJob
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        
        public string Name { get; set; }
        public string Description { get; set; }
        
        // Job Status: Pending, Processing, Completed, Failed
        public string Status { get; set; } = "Pending";
        
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; } = 0;
        public int FailedFiles { get; set; } = 0;
        
        // Processing Options
        public bool EnablePreprocessing { get; set; } = true;
        public bool EnableQualityEnhancement { get; set; } = true;
        public int CompressionLevel { get; set; } = 85; // 1-100
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        public List<BatchOcrFile> Files { get; set; } = new List<BatchOcrFile>();
        public List<BatchOcrResult> Results { get; set; } = new List<BatchOcrResult>();
        
        // Error details if job failed
        public string ErrorMessage { get; set; }
        
        public decimal ProgressPercentage => TotalFiles == 0 ? 0 : (ProcessedFiles * 100m) / TotalFiles;
    }
}