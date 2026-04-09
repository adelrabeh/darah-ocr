using DarahOcr.Api.Data;
using DarahOcr.Api.Models;
using Microsoft.EntityFrameworkCore;
using Tesseract;

namespace DarahOcr.Api.Services;

public class OcrService(AppDbContext db, IConfiguration config, ILogger<OcrService> logger)
{
    private readonly string _uploadsPath = config["Storage:UploadsPath"] ?? Path.Combine(AppContext.BaseDirectory, "uploads");
    private readonly string _tessDataPath = config["Tesseract:DataPath"] ?? Path.Combine(AppContext.BaseDirectory, "tessdata");

    public async Task ProcessJobAsync(int jobId)
    {
        var job = await db.OcrJobs.FindAsync(jobId);
        if (job is null) return;

        job.Status = "processing";
        job.StartedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        try
        {
            var filePath = Path.Combine(_uploadsPath, job.StoredFilename);
            var (rawText, pageCount, confidence) = await ExtractTextAsync(filePath, job.FileType);

            var wordCount = rawText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            var quality = confidence >= 70 ? "high" : confidence >= 40 ? "medium" : "low";

            var result = new OcrResult
            {
                JobId = jobId,
                RawText = rawText,
                ConfidenceScore = confidence,
                QualityLevel = quality,
                WordCount = wordCount,
                PageCount = pageCount,
                OcrEngine = "tesseract",
                ProcessingNotes = quality == "low" ? "جودة منخفضة — يُنصح بمراجعة النص" : null
            };

            db.OcrResults.Add(result);
            job.Status = "completed";
            job.CompletedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            logger.LogInformation("Job {JobId} completed: {Words} words, {Pages} pages", jobId, wordCount, pageCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Job {JobId} failed", jobId);
            job.Status = "failed";
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }

    private async Task<(string text, int pages, int confidence)> ExtractTextAsync(string filePath, string fileType)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();

        if (ext == ".pdf")
            return await ProcessPdfAsync(filePath);
        else
            return await ProcessImageAsync(filePath);
    }

    private async Task<(string text, int pages, int confidence)> ProcessPdfAsync(string pdfPath)
    {
        // تحويل PDF إلى صور باستخدام ImageMagick
        var tmpDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tmpDir);

        try
        {
            var settings = new ImageMagick.MagickReadSettings
            {
                Density = new ImageMagick.Density(150)
            };

            using var images = new ImageMagick.MagickImageCollection();
            await Task.Run(() => images.Read(pdfPath, settings));

            var texts = new List<string>();
            var totalConfidence = 0;
            var pageNum = 0;

            foreach (var image in images)
            {
                pageNum++;
                var imgPath = Path.Combine(tmpDir, $"page_{pageNum:D4}.png");
                await Task.Run(() => image.Write(imgPath));
                var (text, conf) = OcrImage(imgPath);
                texts.Add(text);
                totalConfidence += conf;
            }

            var avgConfidence = pageNum > 0 ? totalConfidence / pageNum : 0;
            return (string.Join("\n\n", texts), pageNum, avgConfidence);
        }
        finally
        {
            Directory.Delete(tmpDir, true);
        }
    }

    private async Task<(string text, int pages, int confidence)> ProcessImageAsync(string imagePath)
    {
        var (text, confidence) = await Task.Run(() => OcrImage(imagePath));
        return (text, 1, confidence);
    }

    private (string text, int confidence) OcrImage(string imagePath)
    {
        try
        {
            using var engine = new TesseractEngine(_tessDataPath, "ara+eng", EngineMode.Default);
            using var img = Pix.LoadFromFile(imagePath);
            using var page = engine.Process(img);

            var text = page.GetText() ?? "";
            var confidence = (int)(page.GetMeanConfidence() * 100);
            return (text.Trim(), confidence);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Tesseract failed for {Image}", imagePath);
            return ("", 0);
        }
    }
}
