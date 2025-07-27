using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Copayments
{
    public class UpdateCopaymentRequestDto
    {
        public Guid? InsuranceId { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal? Amount { get; set; }
        
        public decimal? InsuranceCoverage { get; set; }
        
        [Range(0, double.MaxValue, ErrorMessage = "Patient responsibility must be 0 or greater")]
        public decimal? PatientResponsibility { get; set; }
        
        public string? Status { get; set; } // Pending, Paid, Partially Paid, Waived
        
        public DateTime? PaymentDate { get; set; }
        
        public string? PaymentMethod { get; set; } // Credit Card, Cash, Check, etc.
        
        public string? TransactionId { get; set; }
        
        public string? ReceiptUrl { get; set; }
    }
}