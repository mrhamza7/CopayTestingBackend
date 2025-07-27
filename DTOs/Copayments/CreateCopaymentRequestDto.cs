using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Copayments
{
    public class CreateCopaymentRequestDto
    {
        [Required]
        public Guid AppointmentId { get; set; }
        
        [Required]
        public Guid PatientId { get; set; }
        
        public Guid? InsuranceId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        public decimal? InsuranceCoverage { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Patient responsibility must be 0 or greater")]
        public decimal PatientResponsibility { get; set; }
    }
}