using CP1Testing.Entities;
using Microsoft.EntityFrameworkCore;

namespace CP1Testing.Repositories
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Patient?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.Clinic)
                .Include(p => p.Insurances)
                .ThenInclude(i => i.InsuranceProvider)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Patient>> GetPatientsByClinicAsync(Guid clinicId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => p.ClinicId == clinicId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Insurance>> GetPatientInsurancesAsync(Guid patientId)
        {
            return await _context.Insurances
                .Include(i => i.InsuranceProvider)
                .Where(i => i.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(Guid patientId)
        {
            return await _context.Appointments
                .Include(a => a.Clinic)
                .Include(a => a.ClinicStaff)
                .ThenInclude(cs => cs != null ? cs.User : null)
                .Include(a => a.Copayment)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<HealthRecord>> GetPatientHealthRecordsAsync(Guid patientId)
        {
            return await _context.HealthRecords
                .Include(hr => hr.Appointment)
                .Include(hr => hr.ClinicStaff)
                .ThenInclude(cs => cs != null ? cs.User : null)
                .Where(hr => hr.PatientId == patientId && hr.IsSharedWithPatient)
                .OrderByDescending(hr => hr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Copayment>> GetPatientCopaymentsAsync(Guid patientId)
        {
            return await _context.Copayments
                .Include(c => c.Appointment)
                .Include(c => c.Insurance)
                .ThenInclude(i => i != null ? i.InsuranceProvider : null)
                .Where(c => c.PatientId == patientId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}