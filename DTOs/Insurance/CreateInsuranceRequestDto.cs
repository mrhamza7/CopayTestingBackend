using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Insurance
{
    public class CreateInsuranceRequestDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        public Guid InsuranceProviderId { get; set; }
        
        [Required]
        public string PolicyNumber { get; set; } = string.Empty;
        
        public string GroupNumber { get; set; } = string.Empty;
        
        public string CardImageUrl { get; set; } = string.Empty;
        
        [Required]
        public DateTime EffectiveDate { get; set; }
        
        public DateTime? ExpirationDate { get; set; }
        
        public bool IsPrimary { get; set; } = false;
    }
}