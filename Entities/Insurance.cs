using System;

namespace CP1Testing.Entities
{
    public class Insurance : BaseEntity
    {
        public Guid PatientId { get; set; }
        public Guid InsuranceProviderId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public string GroupNumber { get; set; } = string.Empty;
        public string CardImageUrl { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; } = false;
        
        // Navigation properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual InsuranceProvider InsuranceProvider { get; set; } = null!;
    }
}