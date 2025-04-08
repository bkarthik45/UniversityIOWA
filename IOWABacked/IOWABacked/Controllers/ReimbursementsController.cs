using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IOWABacked.Data;
using IOWABacked.Models;

namespace IOWABacked.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReimbursementsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ReimbursementsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Submit([FromForm] ReimbursementRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("File is required.");

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(request.File.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                return BadRequest("Only PDF, JPG, JPEG, PNG files are allowed.");

            if (request.File.Length > 5 * 1024 * 1024)
                return BadRequest("File size cannot exceed 5MB.");

            var folderPath = Path.Combine(_env.WebRootPath ?? "wwwroot", "receipts");
            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, request.File.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var reimbursement = new Reimbursement
            {
                PurchaseDate = request.PurchaseDate,
                Amount = request.Amount,
                Description = request.Description,
                ReceiptFileName = request.File.FileName
            };

            _context.Reimbursements.Add(reimbursement);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reimbursement submitted successfully" });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var reimbursements = _context.Reimbursements.OrderByDescending(r => r.PurchaseDate).ToList();
            return Ok(reimbursements);
        }
    }
}
