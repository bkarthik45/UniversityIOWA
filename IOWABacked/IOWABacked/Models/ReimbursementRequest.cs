using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace IOWABacked.Models
{
    public class ReimbursementRequest
    {
        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public IFormFile? File { get; set; }
    }
}
