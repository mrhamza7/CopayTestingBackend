using Microsoft.EntityFrameworkCore;
using System;

namespace CP1Testing.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<SuperAdmin> SuperAdmins { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<ClinicAdmin> ClinicAdmins { get; set; }
        public DbSet<ClinicStaff> ClinicStaff { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<InsuranceProvider> InsuranceProviders { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<CopaymentRule> CopaymentRules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Copayment> Copayments { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure soft delete query filter for all entities that inherit from BaseEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                    var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var falseConstant = System.Linq.Expressions.Expression.Constant(false);
                    var comparison = System.Linq.Expressions.Expression.Equal(property, falseConstant);
                    var lambda = System.Linq.Expressions.Expression.Lambda(comparison, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
            
            // Configure relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
                
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
                
            modelBuilder.Entity<SuperAdmin>()
                .HasOne(sa => sa.User)
                .WithOne()
                .HasForeignKey<SuperAdmin>(sa => sa.UserId);
                
            modelBuilder.Entity<ClinicAdmin>()
                .HasOne(ca => ca.User)
                .WithMany(u => u.ClinicAdmins)
                .HasForeignKey(ca => ca.UserId);
                
            modelBuilder.Entity<ClinicAdmin>()
                .HasOne(ca => ca.Clinic)
                .WithMany(c => c.ClinicAdmins)
                .HasForeignKey(ca => ca.ClinicId);
                
            modelBuilder.Entity<ClinicStaff>()
                .HasOne(cs => cs.User)
                .WithMany()
                .HasForeignKey(cs => cs.UserId);
                
            modelBuilder.Entity<ClinicStaff>()
                .HasOne(cs => cs.Clinic)
                .WithMany(c => c.ClinicStaff)
                .HasForeignKey(cs => cs.ClinicId);
                
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);
                
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Clinic)
                .WithMany(c => c.Patients)
                .HasForeignKey(p => p.ClinicId);
                
            modelBuilder.Entity<Insurance>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Insurances)
                .HasForeignKey(i => i.PatientId);
                
            modelBuilder.Entity<Insurance>()
                .HasOne(i => i.InsuranceProvider)
                .WithMany(ip => ip.Insurances)
                .HasForeignKey(i => i.InsuranceProviderId);
                
            modelBuilder.Entity<CopaymentRule>()
                .HasOne(cr => cr.InsuranceProvider)
                .WithMany(ip => ip.CopaymentRules)
                .HasForeignKey(cr => cr.InsuranceProviderId);
                
            modelBuilder.Entity<CopaymentRule>()
                .Property(cr => cr.CoveragePercentage)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<CopaymentRule>()
                .Property(cr => cr.Amount)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.NoAction);
                
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Clinic)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClinicId);
                
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.ClinicStaff)
                .WithMany()
                .HasForeignKey(a => a.ClinicStaffId);
                
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Copayment)
                .WithOne(c => c.Appointment)
                .HasForeignKey<Copayment>(c => c.AppointmentId);
                
            modelBuilder.Entity<Copayment>()
                .HasOne(c => c.Patient)
                .WithMany(p => p.Copayments)
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.NoAction);
                
            modelBuilder.Entity<Copayment>()
                .HasOne(c => c.Insurance)
                .WithMany()
                .HasForeignKey(c => c.InsuranceId);
                
            modelBuilder.Entity<Copayment>()
                .Property(c => c.PatientResponsibility)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<Copayment>()
                .Property(c => c.Amount)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<Copayment>()
                .Property(c => c.InsuranceCoverage)
                .HasPrecision(18, 2);
                
            modelBuilder.Entity<HealthRecord>()
                .HasOne(hr => hr.Patient)
                .WithMany(p => p.HealthRecords)
                .HasForeignKey(hr => hr.PatientId)
                .OnDelete(DeleteBehavior.NoAction);
                
            modelBuilder.Entity<HealthRecord>()
                .HasOne(hr => hr.Appointment)
                .WithMany()
                .HasForeignKey(hr => hr.AppointmentId)
                .OnDelete(DeleteBehavior.NoAction);
                
            modelBuilder.Entity<HealthRecord>()
                .HasOne(hr => hr.ClinicStaff)
                .WithMany()
                .HasForeignKey(hr => hr.ClinicStaffId);
                
            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = Guid.Parse("8d04dce2-969a-435d-bba4-df3f325983dc"), Name = "SuperAdmin", NormalizedName = "SUPERADMIN", Description = "Super Administrator with full system access", CreatedAt = new DateTime(2023, 1, 1) },
                new Role { Id = Guid.Parse("cfd242d3-2107-4563-b2a4-15383e683964"), Name = "ClinicAdmin", NormalizedName = "CLINICADMIN", Description = "Clinic Administrator with clinic-level access", CreatedAt = new DateTime(2023, 1, 1) },
                new Role { Id = Guid.Parse("4fd681b6-5dd2-4a5a-aa9e-b2806c61d4ca"), Name = "Patient", NormalizedName = "PATIENT", Description = "Patient with limited access to personal data", CreatedAt = new DateTime(2023, 1, 1) }
            );
        }
        
        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChangesAsync(cancellationToken);
        }
        
        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity baseEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            baseEntity.CreatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Modified:
                            baseEntity.UpdatedAt = DateTime.UtcNow;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            baseEntity.IsDeleted = true;
                            baseEntity.DeletedAt = DateTime.UtcNow;
                            break;
                    }
                }
            }
        }
    }
}