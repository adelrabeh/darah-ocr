using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DarahOcr.Api.Data;
using DarahOcr.Api.Models;
using DarahOcr.Api.Services;

namespace DarahOcr.Api.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize]
public class JobsController(AppDbContext db, OcrService ocrService, IConfiguration config) : ControllerBase
{
    private readonly string _uploadsPath = config["Storage:UploadsPath"] ?? Path.Combine(AppContext.BaseDirectory, "uploads");
    private readonly string[] _allowedTypes = [".pdf", ".png", ".jpg", ".jpeg", ".tiff", ".tif"];

    [HttpGet]
    public async Task<IActionResult> GetJobs()
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("admin") || User.IsInRole("operator");

        var query = db.OcrJobs
            .Include(j => j.Result)
            .Include(j => j.User)
            .AsQueryable();

        if (!isAdmin) query = query.Where(j => j.UserId == userId);

        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new
            {
                j.Id, j.OriginalFilename, j.FileType, j.FileSize,
                j.Status, j.ErrorMessage, j.CreatedAt, j.StartedAt, j.CompletedAt,
                username = j.User.Username,
                hasResult = j.Result != null,
                wordCount = j.Result != null ? j.Result.WordCount : 0,
                pageCount = j.Result != null ? j.Result.PageCount : 0,
                quality = j.Result != null ? j.Result.QualityLevel : null
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJob(int id)
    {
        var userId = GetUserId();
        var job = await db.OcrJobs
            .Include(j => j.Result)
            .Include(j => j.User)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null) return NotFound();
        if (job.UserId != userId && !User.IsInRole("admin") && !User.IsInRole("operator"))
            return Forbid();

        return Ok(new
        {
            job.Id, job.OriginalFilename, job.FileType, job.FileSize,
            job.Status, job.ErrorMessage, job.CreatedAt, job.StartedAt, job.CompletedAt,
            username = job.User.Username,
            result = job.Result is null ? null : new
            {
                job.Result.RawText, job.Result.RefinedText,
                job.Result.ConfidenceScore, job.Result.QualityLevel,
                job.Result.WordCount, job.Result.PageCount,
                job.Result.OcrEngine, job.Result.ProcessingNotes,
                job.Result.CreatedAt
            }
        });
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "لم يتم اختيار ملف" });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedTypes.Contains(ext))
            return BadRequest(new { message = "نوع الملف غير مدعوم" });

        if (file.Length > 50 * 1024 * 1024)
            return BadRequest(new { message = "حجم الملف يتجاوز 50 ميجابايت" });

        Directory.CreateDirectory(_uploadsPath);
        var storedName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(_uploadsPath, storedName);

        using (var stream = System.IO.File.Create(filePath))
            await file.CopyToAsync(stream);

        var job = new OcrJob
        {
            UserId = GetUserId(),
            OriginalFilename = file.FileName,
            StoredFilename = storedName,
            FileType = ext.TrimStart('.').ToUpper(),
            FileSize = file.Length
        };
        db.OcrJobs.Add(job);
        await db.SaveChangesAsync();

        // معالجة OCR في الخلفية
        _ = Task.Run(() => ocrService.ProcessJobAsync(job.Id));

        return Ok(new { job.Id, job.OriginalFilename, job.Status, message = "تم رفع الملف وبدأت المعالجة" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var job = await db.OcrJobs.Include(j => j.Result).FirstOrDefaultAsync(j => j.Id == id);
        if (job is null) return NotFound();
        if (job.UserId != userId && !User.IsInRole("admin")) return Forbid();

        var filePath = Path.Combine(_uploadsPath, job.StoredFilename);
        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

        db.OcrJobs.Remove(job);
        await db.SaveChangesAsync();
        return Ok(new { message = "تم حذف المهمة" });
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}
