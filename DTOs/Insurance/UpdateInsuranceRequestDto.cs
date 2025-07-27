using System.ComponentModel.DataAnnotations;

namespace CP1Testing.DTOs.Insurance
{
    public class UpdateInsuranceRequestDto
    {
        public Guid? InsuranceProviderId { get; set; }
        
        public string? PolicyNumber { get; set; }
        
        public string? GroupNumber { get; set; }
        
        public string? CardImageUrl { get; set; }
        
        public DateTime? EffectiveDate { get; set; }
        
        public DateTime? ExpirationDate { get; set; }
        
        public bool? IsPrimary { get; set; }
        
        public bool? IsVerified { get; set; }
    }
}