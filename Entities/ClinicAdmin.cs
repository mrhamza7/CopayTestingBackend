using System;

namespace CP1Testing.Entities
{
    public class ClinicAdmin : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid ClinicId { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Clinic Clinic { get; set; } = null!;
    }
}