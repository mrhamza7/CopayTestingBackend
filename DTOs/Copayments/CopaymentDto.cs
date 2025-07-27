namespace CP1Testing.DTOs.Copayments
{
    public class CopaymentDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid? InsuranceId { get; set; }
        public string? InsuranceProviderName { get; set; }
        public string? PolicyNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal? InsuranceCoverage { get; set; }
        public decimal PatientResponsibility { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Paid, Partially Paid, Waived
        public DateTime? PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // Credit Card, Cash, Check, etc.
        public string TransactionId { get; set; } = string.Empty;
        public string ReceiptUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}