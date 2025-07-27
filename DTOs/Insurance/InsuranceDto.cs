namespace CP1Testing.DTOs.Insurance
{
    public class InsuranceDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid InsuranceProviderId { get; set; }
        public string InsuranceProviderName { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public string GroupNumber { get; set; } = string.Empty;
        public string CardImageUrl { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}