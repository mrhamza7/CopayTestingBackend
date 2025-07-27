using CP1Testing.Entities;
using Microsoft.EntityFrameworkCore;

namespace CP1Testing.Repositories
{
    public class ClinicRepository : Repository<Clinic>, IClinicRepository
    {
        public ClinicRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Clinic?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.ClinicAdmins)
                .ThenInclude(ca => ca.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Clinic>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(c => c.ClinicAdmins)
                .ThenInclude(ca => ca.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicAdmin>> GetClinicAdminsAsync(Guid clinicId)
        {
            return await _context.ClinicAdmins
                .Include(ca => ca.User)
                .Where(ca => ca.ClinicId == clinicId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicStaff>> GetClinicStaffAsync(Guid clinicId)
        {
            return await _context.Set<ClinicStaff>()
                .Include(cs => cs.User)
                .Where(cs => cs.ClinicId == clinicId)
                .ToListAsync();
        }

        public async Task<bool> AddClinicAdminAsync(Guid clinicId, Guid userId)
        {
            var clinic = await GetByIdAsync(clinicId);
            if (clinic == null) return false;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            var existingAdmin = await _context.ClinicAdmins
                .FirstOrDefaultAsync(ca => ca.ClinicId == clinicId && ca.UserId == userId);

            if (existingAdmin != null) return true; // Already an admin

            var clinicAdmin = new ClinicAdmin
            {
                ClinicId = clinicId,
                UserId = userId
            };

            await _context.ClinicAdmins.AddAsync(clinicAdmin);
            return await SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveClinicAdminAsync(Guid clinicId, Guid userId)
        {
            var clinicAdmin = await _context.ClinicAdmins
                .FirstOrDefaultAsync(ca => ca.ClinicId == clinicId && ca.UserId == userId);

            if (clinicAdmin == null) return true; // Not an admin

            _context.ClinicAdmins.Remove(clinicAdmin);
            return await SaveChangesAsync() > 0;
        }
    }
}