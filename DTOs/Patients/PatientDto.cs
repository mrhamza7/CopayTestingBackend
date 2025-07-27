namespace CP1Testing.DTOs.Patients
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string MedicalRecordNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age => CalculateAge(DateOfBirth);
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string EmergencyContactRelationship { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public string FullName => $"{FirstName} {LastName}".Trim();
        
        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}