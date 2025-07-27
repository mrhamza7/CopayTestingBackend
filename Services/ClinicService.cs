using CP1Testing.DTOs.Clinics;
using CP1Testing.DTOs.Users;
using CP1Testing.Entities;
using CP1Testing.Repositories;

namespace CP1Testing.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public ClinicService(
            IClinicRepository clinicRepository,
            IUserRepository userRepository,
            IUserService userService)
        {
            _clinicRepository = clinicRepository;
            _userRepository = userRepository;
            _userService = userService;
        }

        public async Task<IEnumerable<ClinicDto>> GetAllClinicsAsync()
        {
            var clinics = await _clinicRepository.GetAllWithDetailsAsync();
            return clinics.Select(MapToClinicDto);
        }

        public async Task<ClinicDto?> GetClinicByIdAsync(Guid id)
        {
            var clinic = await _clinicRepository.GetByIdWithDetailsAsync(id);
            return clinic != null ? MapToClinicDto(clinic) : null;
        }

        public async Task<(bool Success, string Message, ClinicDto? Clinic)> CreateClinicAsync(CreateClinicRequestDto model)
        {
            var clinic = new Clinic
            {
                Name = model.Name,
                Address = model.Address,
                City = model.City,
                State = model.State,
                ZipCode = model.ZipCode,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Website = model.Website,
                TaxId = model.TaxId,
                IsActive = true
            };

            await _clinicRepository.AddAsync(clinic);
            await _clinicRepository.SaveChangesAsync();

            return (true, "Clinic created successfully", MapToClinicDto(clinic));
        }

        public async Task<(bool Success, string Message, ClinicDto? Clinic)> UpdateClinicAsync(Guid id, UpdateClinicRequestDto model)
        {
            var clinic = await _clinicRepository.GetByIdAsync(id);
            if (clinic == null)
            {
                return (false, "Clinic not found", null);
            }

            // Update properties
            if (model.Name != null) clinic.Name = model.Name;
            if (model.Address != null) clinic.Address = model.Address;
            if (model.City != null) clinic.City = model.City;
            if (model.State != null) clinic.State = model.State;
            if (model.ZipCode != null) clinic.ZipCode = model.ZipCode;
            if (model.PhoneNumber != null) clinic.PhoneNumber = model.PhoneNumber;
            if (model.Email != null) clinic.Email = model.Email;
            if (model.Website != null) clinic.Website = model.Website;
            if (model.TaxId != null) clinic.TaxId = model.TaxId;
            if (model.IsActive.HasValue) clinic.IsActive = model.IsActive.Value;

            await _clinicRepository.UpdateAsync(clinic);
            await _clinicRepository.SaveChangesAsync();

            return (true, "Clinic updated successfully", MapToClinicDto(clinic));
        }

        public async Task<bool> DeleteClinicAsync(Guid id)
        {
            var clinic = await _clinicRepository.GetByIdAsync(id);
            if (clinic == null) return false;

            await _clinicRepository.SoftDeleteAsync(id);
            return await _clinicRepository.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<UserDto>> GetClinicAdminsAsync(Guid clinicId)
        {
            var clinicAdmins = await _clinicRepository.GetClinicAdminsAsync(clinicId);
            var userDtos = new List<UserDto>();

            foreach (var admin in clinicAdmins)
            {
                var user = await _userService.GetUserByIdAsync(admin.UserId);
                if (user != null)
                {
                    userDtos.Add(user);
                }
            }

            return userDtos;
        }

        public async Task<IEnumerable<UserDto>> GetClinicStaffAsync(Guid clinicId)
        {
            var clinicStaff = await _clinicRepository.GetClinicStaffAsync(clinicId);
            var userDtos = new List<UserDto>();

            foreach (var staff in clinicStaff)
            {
                var user = await _userService.GetUserByIdAsync(staff.UserId);
                if (user != null)
                {
                    userDtos.Add(user);
                }
            }

            return userDtos;
        }

        public async Task<(bool Success, string Message)> AssignClinicAdminAsync(AssignClinicAdminRequestDto model)
        {
            // Check if clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(model.ClinicId);
            if (clinic == null)
            {
                return (false, "Clinic not found");
            }

            // Check if user exists
            var user = await _userRepository.GetByIdAsync(model.UserId);
            if (user == null)
            {
                return (false, "User not found");
            }

            // Add user to ClinicAdmin role if not already
            if (!await _userRepository.IsInRoleAsync(model.UserId, "ClinicAdmin"))
            {
                await _userRepository.AddToRoleAsync(model.UserId, "ClinicAdmin");
            }

            // Assign user as clinic admin
            var result = await _clinicRepository.AddClinicAdminAsync(model.ClinicId, model.UserId);
            return result ? (true, "User assigned as clinic admin successfully") : (false, "Failed to assign user as clinic admin");
        }

        public async Task<(bool Success, string Message)> RemoveClinicAdminAsync(Guid clinicId, Guid userId)
        {
            // Check if clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(clinicId);
            if (clinic == null)
            {
                return (false, "Clinic not found");
            }

            // Check if user exists
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return (false, "User not found");
            }

            // Remove user from clinic admin
            var result = await _clinicRepository.RemoveClinicAdminAsync(clinicId, userId);

            // Check if user is admin for any other clinics
            var isAdminForOtherClinics = await _userRepository.FindAsync(u => 
                u.Id == userId && 
                u.ClinicAdmins.Any(ca => ca.ClinicId != clinicId));

            // If not admin for any other clinics, remove from ClinicAdmin role
            if (isAdminForOtherClinics == null && await _userRepository.IsInRoleAsync(userId, "ClinicAdmin"))
            {
                await _userRepository.RemoveFromRoleAsync(userId, "ClinicAdmin");
            }

            return result ? (true, "User removed as clinic admin successfully") : (false, "Failed to remove user as clinic admin");
        }

        private ClinicDto MapToClinicDto(Clinic clinic)
        {
            return new ClinicDto
            {
                Id = clinic.Id,
                Name = clinic.Name,
                Address = clinic.Address,
                City = clinic.City,
                State = clinic.State,
                ZipCode = clinic.ZipCode,
                PhoneNumber = clinic.PhoneNumber,
                Email = clinic.Email,
                Website = clinic.Website,
                TaxId = clinic.TaxId,
                IsActive = clinic.IsActive,
                CreatedAt = clinic.CreatedAt,
                UpdatedAt = clinic.UpdatedAt
            };
        }
    }
}