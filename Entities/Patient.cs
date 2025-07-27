using System;

namespace CP1Testing.Entities
{
    public class Patient : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ClinicId { get; set; }
        public string MedicalRecordNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string EmergencyContactRelationship { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Clinic Clinic { get; set; } = null!;
        public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<HealthRecord> HealthRecords { get; set; } = new List<HealthRecord>();
        public virtual ICollection<Copayment> Copayments { get; set; } = new List<Copayment>();
    }
}