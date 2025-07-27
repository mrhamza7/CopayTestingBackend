namespace CP1Testing.DTOs.HealthRecords
{
    public class HealthRecordDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid? AppointmentId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public Guid? ClinicStaffId { get; set; }
        public string? ClinicStaffName { get; set; }
        public string RecordType { get; set; } = string.Empty; // Medical Note, Test Result, Prescription, etc.
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public bool IsSharedWithPatient { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}