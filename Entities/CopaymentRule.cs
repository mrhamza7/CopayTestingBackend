using System;

namespace CP1Testing.Entities
{
    public class CopaymentRule : BaseEntity
    {
        public Guid InsuranceProviderId { get; set; }
        public string ServiceType { get; set; } = string.Empty; // Primary Care, Specialist, Emergency, etc.
        public decimal Amount { get; set; }
        public decimal? CoveragePercentage { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual InsuranceProvider InsuranceProvider { get; set; } = null!;
    }
}