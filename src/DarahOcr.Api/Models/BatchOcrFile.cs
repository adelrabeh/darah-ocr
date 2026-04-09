using System;
using System.Collections.Generic;

namespace DarahOcr.Api.Models
{
    public class BatchOcrFile
    {
        public int Id { get; set; }
        public int BatchOcrJobId { get; set; }
        public BatchOcrJob BatchOcrJob { get; set; }
        
        public string FileName { get; set; }
        public string FileExtension { get; set; } // .jpg, .png, .tiff, .bmp, .pdf
        public long FileSizeBytes { get; set; }
        public string FilePath { get; set; }
        
        // File Status: Pending, Processing, Completed, Failed
        public string Status { get; set; } = "Pending";
        
        // Preview image data (base64 or path)
        public string PreviewImageBase64 { get; set; }
        public string PreviewImagePath { get; set; }
        
        // Processing details
        public int? OrientationAngle { get; set; } = 0;
        public bool IsSkewed { get; set; } = false;
        public decimal? EstimatedAccuracy { get; set; } // 0-100
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        
        public BatchOcrResult Result { get; set; }
        public string ErrorMessage { get; set; }
    }
}