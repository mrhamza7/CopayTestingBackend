using System;

namespace CP1Testing.Entities
{
    public class ClinicStaff : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ClinicId { get; set; }
        public string Position { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Clinic Clinic { get; set; } = null!;
    }
}