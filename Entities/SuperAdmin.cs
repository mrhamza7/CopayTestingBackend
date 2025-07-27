using System;

namespace CP1Testing.Entities
{
    public class SuperAdmin : BaseEntity
    {
        public Guid UserId { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
    }
}