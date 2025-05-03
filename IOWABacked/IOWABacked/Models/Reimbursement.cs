using System;
using System.ComponentModel.DataAnnotations;

namespace IOWABacked.Models
{
    public class Reimbursement
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Purchase date is required.")]
        public DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01,500, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Receipt file name is required.")]
        public string ReceiptFileName { get; set; } = string.Empty;

    }
}
