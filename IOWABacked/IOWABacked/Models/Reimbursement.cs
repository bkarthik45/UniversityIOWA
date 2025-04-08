using System;
using System.ComponentModel.DataAnnotations;

namespace IOWABacked.Models
{
    public class Reimbursement
    {
        public int Id { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; }

        public string? ReceiptFileName { get; set; }
    }
}
