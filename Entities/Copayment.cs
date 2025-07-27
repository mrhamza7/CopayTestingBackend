using System;

namespace CP1Testing.Entities
{
    public class Copayment : BaseEntity
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid? InsuranceId { get; set; }
        public decimal Amount { get; set; }
        public decimal? InsuranceCoverage { get; set; }
        public decimal PatientResponsibility { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Paid, Partially Paid, Waived
        public DateTime? PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // Credit Card, Cash, Check, etc.
        public string TransactionId { get; set; } = string.Empty;
        public string ReceiptUrl { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual Appointment Appointment { get; set; } = null!;
        public virtual Patient Patient { get; set; } = null!;
        public virtual Insurance? Insurance { get; set; }
    }
}