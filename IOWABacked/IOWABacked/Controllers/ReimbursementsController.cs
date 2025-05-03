using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IOWABacked.Data;
using IOWABacked.Models;
using IOWABacked.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IOWABacked.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReimbursementsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IFileValidatorService _fileValidator;
        private readonly ILogger<ReimbursementsController> _logger;

        public ReimbursementsController(AppDbContext context, IWebHostEnvironment env, IFileValidatorService fileValidator, ILogger<ReimbursementsController> logger)
        {
            _context = context;
            _env = env;
            _fileValidator = fileValidator;
            _logger = logger;

        }

    [HttpPost]
        [RequestSizeLimit(5 * 1024 * 1024)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Submit([FromForm] ReimbursementRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Description))
                return BadRequest("Description is required.");

            if (request.PurchaseDate > DateTime.Today)
                return BadRequest("Purchase date cannot be in the future.");

            if (!_fileValidator.IsValid(request.File, out var error))
                return BadRequest(error);

            // Store file
            var ext = Path.GetExtension(request.File.FileName);
           
            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(request.File.FileName)}_{Guid.NewGuid()}{ext}";
            var folderPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "receipts");
            Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var reimbursement = new Reimbursement
            {
                PurchaseDate = request.PurchaseDate,
                Amount = request.Amount,
                Description = request.Description,
                ReceiptFileName = uniqueFileName
            };

            try
            {
                _context.Reimbursements.Add(reimbursement);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                _logger.LogError(ex, "Error in saving reimbursement record to database.");
                return StatusCode(500, "An error occurred while saving the reimbursement.");
            }

            return Ok(new { message = "Reimbursement submitted successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reimbursements = await _context.Reimbursements
                .OrderByDescending(r => r.PurchaseDate)
                .ToListAsync();

            return Ok(reimbursements);
        }
    }
}
