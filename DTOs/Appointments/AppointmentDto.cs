namespace CP1Testing.DTOs.Appointments
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public Guid? ClinicStaffId { get; set; }
        public string? ClinicStaffName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty; // Scheduled, Confirmed, Completed, Cancelled, No-Show
        public string Type { get; set; } = string.Empty; // Regular, Follow-up, Emergency, etc.
        public string Notes { get; set; } = string.Empty;
        public string CancellationReason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CopaymentId { get; set; }
        public decimal? CopaymentAmount { get; set; }
        public string? CopaymentStatus { get; set; }
    }
}