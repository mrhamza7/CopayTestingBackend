using CP1Testing.DTOs.Appointments;
using CP1Testing.DTOs.Copayments;
using CP1Testing.DTOs.HealthRecords;
using CP1Testing.DTOs.Insurance;
using CP1Testing.DTOs.Patients;
using CP1Testing.Entities;
using CP1Testing.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CP1Testing.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Insurance> _insuranceRepository;
        private readonly IRepository<InsuranceProvider> _insuranceProviderRepository;
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IRepository<HealthRecord> _healthRecordRepository;
        private readonly IRepository<Copayment> _copaymentRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public PatientService(
            IPatientRepository patientRepository,
            IClinicRepository clinicRepository,
            IUserRepository userRepository,
            IRepository<Insurance> insuranceRepository,
            IRepository<InsuranceProvider> insuranceProviderRepository,
            IRepository<Appointment> appointmentRepository,
            IRepository<HealthRecord> healthRecordRepository,
            IRepository<Copayment> copaymentRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _patientRepository = patientRepository;
            _clinicRepository = clinicRepository;
            _userRepository = userRepository;
            _insuranceRepository = insuranceRepository;
            _insuranceProviderRepository = insuranceProviderRepository;
            _appointmentRepository = appointmentRepository;
            _healthRecordRepository = healthRecordRepository;
            _copaymentRepository = copaymentRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            var patientDtos = new List<PatientDto>();

            foreach (var patient in patients)
            {
                var user = await _userRepository.GetByIdAsync(patient.UserId);
                var clinic = await _clinicRepository.GetByIdAsync(patient.ClinicId);

                if (user != null && clinic != null)
                {
                    patientDtos.Add(MapToPatientDto(patient, user, clinic.Name));
                }
            }

            return patientDtos;
        }

        public async Task<PatientDto?> GetPatientByIdAsync(Guid id)
        {
            var patient = await _patientRepository.GetByIdWithDetailsAsync(id);
            if (patient == null || patient.User == null || patient.Clinic == null) return null;

            return MapToPatientDto(patient, patient.User, patient.Clinic.Name);
        }

        public async Task<PatientDto?> GetPatientByUserIdAsync(Guid userId)
        {
            var patients = await _patientRepository.FindAsync(p => p.UserId == userId);
            var patient = patients.FirstOrDefault();
            if (patient == null) return null;

            var user = await _userRepository.GetByIdAsync(patient.UserId);
            var clinic = await _clinicRepository.GetByIdAsync(patient.ClinicId);

            if (user == null || clinic == null) return null;

            return MapToPatientDto(patient, user, clinic.Name);
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsByClinicAsync(Guid clinicId)
        {
            var patients = await _patientRepository.GetPatientsByClinicAsync(clinicId);
            var patientDtos = new List<PatientDto>();

            foreach (var patient in patients)
            {
                if (patient.User != null)
                {
                    var clinic = await _clinicRepository.GetByIdAsync(patient.ClinicId);
                    if (clinic != null)
                    {
                        patientDtos.Add(MapToPatientDto(patient, patient.User, clinic.Name));
                    }
                }
            }

            return patientDtos;
        }

        public async Task<(bool Success, string Message, PatientDto? Patient)> CreatePatientAsync(CreatePatientRequestDto model)
        {
            // Check if clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(model.ClinicId);
            if (clinic == null)
            {
                return (false, "Clinic not found", null);
            }

            // Check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return (false, "Email is already registered", null);
            }

            // Create user
            var user = new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                IsActive = true
            };

            // Hash password
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            // Save user
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Add to Patient role
            await _userRepository.AddToRoleAsync(user.Id, "Patient");

            // Generate medical record number
            string mrn = GenerateMedicalRecordNumber(clinic.Name, user.Id);

            // Create patient
            var patient = new Patient
            {
                UserId = user.Id,
                ClinicId = model.ClinicId,
                MedicalRecordNumber = mrn,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                EmergencyContactName = model.EmergencyContactName,
                EmergencyContactPhone = model.EmergencyContactPhone,
                EmergencyContactRelationship = model.EmergencyContactRelationship
            };

            await _patientRepository.AddAsync(patient);
            await _patientRepository.SaveChangesAsync();

            return (true, "Patient created successfully", MapToPatientDto(patient, user, clinic.Name));
        }

        public async Task<(bool Success, string Message, PatientDto? Patient)> UpdatePatientAsync(Guid id, UpdatePatientRequestDto model)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return (false, "Patient not found", null);
            }

            var user = await _userRepository.GetByIdAsync(patient.UserId);
            if (user == null)
            {
                return (false, "User not found", null);
            }

            // Check if email is being changed and if it already exists
            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    return (false, "Email is already registered", null);
                }

                user.Email = model.Email;
            }

            // Update user properties
            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            await _userRepository.UpdateAsync(user);

            // Update patient properties
            if (!string.IsNullOrEmpty(model.Gender))
                patient.Gender = model.Gender;

            if (!string.IsNullOrEmpty(model.EmergencyContactName))
                patient.EmergencyContactName = model.EmergencyContactName;

            if (!string.IsNullOrEmpty(model.EmergencyContactPhone))
                patient.EmergencyContactPhone = model.EmergencyContactPhone;

            if (!string.IsNullOrEmpty(model.EmergencyContactRelationship))
                patient.EmergencyContactRelationship = model.EmergencyContactRelationship;

            await _patientRepository.UpdateAsync(patient);
            await _patientRepository.SaveChangesAsync();

            var clinic = await _clinicRepository.GetByIdAsync(patient.ClinicId);
            if (clinic == null)
            {
                return (true, "Patient updated successfully, but clinic information is missing", MapToPatientDto(patient, user, ""));
            }

            return (true, "Patient updated successfully", MapToPatientDto(patient, user, clinic.Name));
        }

        public async Task<bool> DeletePatientAsync(Guid id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null) return false;

            var user = await _userRepository.GetByIdAsync(patient.UserId);
            if (user == null) return false;

            // Soft delete patient
            await _patientRepository.SoftDeleteAsync(id);
            await _patientRepository.SaveChangesAsync();

            // Soft delete user
            await _userRepository.SoftDeleteAsync(user.Id);
            return await _userRepository.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<InsuranceDto>> GetPatientInsurancesAsync(Guid patientId)
        {
            var insurances = await _patientRepository.GetPatientInsurancesAsync(patientId);
            var insuranceDtos = new List<InsuranceDto>();

            foreach (var insurance in insurances)
            {
                insuranceDtos.Add(MapToInsuranceDto(insurance));
            }

            return insuranceDtos;
        }

        public async Task<InsuranceDto?> GetInsuranceByIdAsync(Guid id)
        {
            var insurance = await _insuranceRepository.GetByIdAsync(id);
            if (insurance == null) return null;

            var provider = await _insuranceProviderRepository.GetByIdAsync(insurance.InsuranceProviderId);
            if (provider == null) return null;

            return MapToInsuranceDto(insurance, provider.Name);
        }

        public async Task<(bool Success, string Message, InsuranceDto? Insurance)> CreateInsuranceAsync(CreateInsuranceRequestDto model)
        {
            // Check if patient exists
            var patient = await _patientRepository.GetByIdAsync(model.PatientId);
            if (patient == null)
            {
                return (false, "Patient not found", null);
            }

            // Check if insurance provider exists
            var provider = await _insuranceProviderRepository.GetByIdAsync(model.InsuranceProviderId);
            if (provider == null)
            {
                return (false, "Insurance provider not found", null);
            }

            // If this is set as primary, update other insurances to not be primary
            if (model.IsPrimary)
            {
                var existingInsurances = await _insuranceRepository.FindAsync(i => i.PatientId == model.PatientId && i.IsPrimary);
                if (existingInsurances != null)
                {
                    foreach (var existing in existingInsurances)
                    {
                        existing.IsPrimary = false;
                        await _insuranceRepository.UpdateAsync(existing);
                    }
                }
            }

            // Create insurance
            var insurance = new Insurance
            {
                PatientId = model.PatientId,
                InsuranceProviderId = model.InsuranceProviderId,
                PolicyNumber = model.PolicyNumber,
                GroupNumber = model.GroupNumber,
                CardImageUrl = model.CardImageUrl,
                EffectiveDate = model.EffectiveDate,
                ExpirationDate = model.ExpirationDate,
                IsPrimary = model.IsPrimary,
                IsVerified = false // New insurances are not verified by default
            };

            await _insuranceRepository.AddAsync(insurance);
            await _insuranceRepository.SaveChangesAsync();

            return (true, "Insurance created successfully", MapToInsuranceDto(insurance, provider.Name));
        }

        public async Task<(bool Success, string Message, InsuranceDto? Insurance)> AddPatientInsuranceAsync(Guid patientId, CreateInsuranceRequestDto model)
        {
            // Override the PatientId in the model with the one from the parameter
            model.PatientId = patientId;
            
            // Check if patient exists
            var patient = await _patientRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                return (false, "Patient not found", null);
            }

            // Use the existing CreateInsuranceAsync method to create the insurance
            return await CreateInsuranceAsync(model);
        }

        public async Task<(bool Success, string Message, InsuranceDto? Insurance)> UpdateInsuranceAsync(Guid id, UpdateInsuranceRequestDto model)
        {
            var insurance = await _insuranceRepository.GetByIdAsync(id);
            if (insurance == null)
            {
                return (false, "Insurance not found", null);
            }

            // Check if insurance provider exists if being updated
            var providerName = string.Empty;
            if (model.InsuranceProviderId.HasValue && model.InsuranceProviderId.Value != Guid.Empty && model.InsuranceProviderId.Value != insurance.InsuranceProviderId)
            {
                var provider = await _insuranceProviderRepository.GetByIdAsync(model.InsuranceProviderId.Value);
                if (provider == null)
                {
                    return (false, "Insurance provider not found", null);
                }
                insurance.InsuranceProviderId = model.InsuranceProviderId.Value;
                providerName = provider.Name;
            }
            else
            {
                var provider = await _insuranceProviderRepository.GetByIdAsync(insurance.InsuranceProviderId);
                if (provider != null)
                {
                    providerName = provider.Name;
                }
            }

            // If this is set as primary, update other insurances to not be primary
            if (model.IsPrimary.HasValue && model.IsPrimary.Value && !insurance.IsPrimary)
            {
                var existingInsurances = await _insuranceRepository.FindAsync(i => 
                    i.PatientId == insurance.PatientId && 
                    i.Id != insurance.Id && 
                    i.IsPrimary);
                
                if (existingInsurances != null)
                {
                    foreach (var existing in existingInsurances)
                    {
                        existing.IsPrimary = false;
                        await _insuranceRepository.UpdateAsync(existing);
                    }
                }
            }

            // Update insurance properties
            if (!string.IsNullOrEmpty(model.PolicyNumber))
                insurance.PolicyNumber = model.PolicyNumber;

            if (!string.IsNullOrEmpty(model.GroupNumber))
                insurance.GroupNumber = model.GroupNumber;

            if (!string.IsNullOrEmpty(model.CardImageUrl))
                insurance.CardImageUrl = model.CardImageUrl;

            if (model.EffectiveDate.HasValue && model.EffectiveDate.Value != DateTime.MinValue)
                insurance.EffectiveDate = model.EffectiveDate.Value;

            if (model.ExpirationDate.HasValue && model.ExpirationDate.Value != DateTime.MinValue)
                insurance.ExpirationDate = model.ExpirationDate.Value;

            if (model.IsPrimary.HasValue)
                insurance.IsPrimary = model.IsPrimary.Value;
                
            if (model.IsVerified.HasValue)
                insurance.IsVerified = model.IsVerified.Value;

            await _insuranceRepository.UpdateAsync(insurance);
            await _insuranceRepository.SaveChangesAsync();

            return (true, "Insurance updated successfully", MapToInsuranceDto(insurance, providerName));
        }

        public async Task<(bool Success, string Message, InsuranceDto? Insurance)> UpdatePatientInsuranceAsync(Guid patientId, Guid insuranceId, UpdateInsuranceRequestDto model)
        {
            var insurance = await _insuranceRepository.GetByIdAsync(insuranceId);
            if (insurance == null)
            {
                return (false, "Insurance not found", null);
            }
            
            if (insurance.PatientId != patientId)
            {
                return (false, "Insurance does not belong to this patient", null);
            }

            // Check if insurance provider exists if being updated
            var providerName = string.Empty;
            if (model.InsuranceProviderId.HasValue && model.InsuranceProviderId.Value != Guid.Empty && model.InsuranceProviderId.Value != insurance.InsuranceProviderId)
            {
                var provider = await _insuranceProviderRepository.GetByIdAsync(model.InsuranceProviderId.Value);
                if (provider == null)
                {
                    return (false, "Insurance provider not found", null);
                }
                insurance.InsuranceProviderId = model.InsuranceProviderId.Value;
                providerName = provider.Name;
            }
            else
            {
                var provider = await _insuranceProviderRepository.GetByIdAsync(insurance.InsuranceProviderId);
                if (provider != null)
                {
                    providerName = provider.Name;
                }
            }

            // If this is set as primary, update other insurances to not be primary
            if (model.IsPrimary.HasValue && model.IsPrimary.Value && !insurance.IsPrimary)
            {
                var existingInsurances = await _insuranceRepository.FindAsync(i => 
                    i.PatientId == patientId && 
                    i.Id != insuranceId && 
                    i.IsPrimary);
                
                if (existingInsurances != null)
                {
                    foreach (var existing in existingInsurances)
                    {
                        existing.IsPrimary = false;
                        await _insuranceRepository.UpdateAsync(existing);
                    }
                }
            }

            // Update insurance properties
            if (!string.IsNullOrEmpty(model.PolicyNumber))
                insurance.PolicyNumber = model.PolicyNumber;

            if (!string.IsNullOrEmpty(model.GroupNumber))
                insurance.GroupNumber = model.GroupNumber;

            if (!string.IsNullOrEmpty(model.CardImageUrl))
                insurance.CardImageUrl = model.CardImageUrl;

            if (model.EffectiveDate.HasValue && model.EffectiveDate.Value != DateTime.MinValue)
                insurance.EffectiveDate = model.EffectiveDate.Value;

            if (model.ExpirationDate.HasValue && model.ExpirationDate.Value != DateTime.MinValue)
                insurance.ExpirationDate = model.ExpirationDate.Value;

            if (model.IsPrimary.HasValue)
                insurance.IsPrimary = model.IsPrimary.Value;
                
            if (model.IsVerified.HasValue)
                insurance.IsVerified = model.IsVerified.Value;

            await _insuranceRepository.UpdateAsync(insurance);
            await _insuranceRepository.SaveChangesAsync();

            return (true, "Insurance updated successfully", MapToInsuranceDto(insurance, providerName));
        }

        public async Task<bool> DeleteInsuranceAsync(Guid id)
        {
            var insurance = await _insuranceRepository.GetByIdAsync(id);
            if (insurance == null) return false;

            await _insuranceRepository.SoftDeleteAsync(id);
            return await _insuranceRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePatientInsuranceAsync(Guid patientId, Guid insuranceId)
        {
            var insurance = await _insuranceRepository.GetByIdAsync(insuranceId);
            if (insurance == null || insurance.PatientId != patientId) return false;

            await _insuranceRepository.SoftDeleteAsync(insuranceId);
            return await _insuranceRepository.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<AppointmentDto>> GetPatientAppointmentsAsync(Guid patientId)
        {
            var appointments = await _patientRepository.GetPatientAppointmentsAsync(patientId);
            var appointmentDtos = new List<AppointmentDto>();

            foreach (var appointment in appointments)
            {
                appointmentDtos.Add(MapToAppointmentDto(appointment));
            }

            return appointmentDtos;
        }

        public async Task<IEnumerable<HealthRecordDto>> GetPatientHealthRecordsAsync(Guid patientId)
        {
            var healthRecords = await _patientRepository.GetPatientHealthRecordsAsync(patientId);
            var healthRecordDtos = new List<HealthRecordDto>();

            foreach (var record in healthRecords)
            {
                healthRecordDtos.Add(MapToHealthRecordDto(record));
            }

            return healthRecordDtos;
        }

        public async Task<IEnumerable<CopaymentDto>> GetPatientCopaymentsAsync(Guid patientId)
        {
            var copayments = await _patientRepository.GetPatientCopaymentsAsync(patientId);
            var copaymentDtos = new List<CopaymentDto>();

            foreach (var copayment in copayments)
            {
                copaymentDtos.Add(MapToCopaymentDto(copayment));
            }

            return copaymentDtos;
        }

        #region Helper Methods

        private string GenerateMedicalRecordNumber(string clinicPrefix, Guid userId)
        {
            // Create a unique MRN based on clinic prefix and user ID
            string prefix = string.Join("", clinicPrefix.Split(' ').Select(s => s.Length > 0 ? s[0] : ' ')).ToUpper();
            if (prefix.Length > 3) prefix = prefix.Substring(0, 3);
            if (prefix.Length < 3) prefix = prefix.PadRight(3, 'X');

            string uniqueId = userId.ToString().Replace("-", "").Substring(0, 8).ToUpper();
            return $"{prefix}-{uniqueId}";
        }

        private PatientDto MapToPatientDto(Patient patient, User user, string clinicName)
        {
            return new PatientDto
            {
                Id = patient.Id,
                UserId = patient.UserId,
                ClinicId = patient.ClinicId,
                ClinicName = clinicName,
                MedicalRecordNumber = patient.MedicalRecordNumber,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactPhone = patient.EmergencyContactPhone,
                EmergencyContactRelationship = patient.EmergencyContactRelationship,
                CreatedAt = patient.CreatedAt,
                UpdatedAt = patient.UpdatedAt
            };
        }

        private InsuranceDto MapToInsuranceDto(Insurance insurance, string? providerName = null)
        {
            return new InsuranceDto
            {
                Id = insurance.Id,
                PatientId = insurance.PatientId,
                InsuranceProviderId = insurance.InsuranceProviderId,
                InsuranceProviderName = providerName ?? (insurance.InsuranceProvider?.Name ?? string.Empty),
                PolicyNumber = insurance.PolicyNumber,
                GroupNumber = insurance.GroupNumber,
                CardImageUrl = insurance.CardImageUrl,
                EffectiveDate = insurance.EffectiveDate,
                ExpirationDate = insurance.ExpirationDate,
                IsPrimary = insurance.IsPrimary,
                IsVerified = insurance.IsVerified,
                CreatedAt = insurance.CreatedAt,
                UpdatedAt = insurance.UpdatedAt
            };
        }

        private AppointmentDto MapToAppointmentDto(Appointment appointment)
        {
            string patientName = string.Empty;
            if (appointment.Patient?.User != null)
            {
                patientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}";
            }
            
            string clinicName = appointment.Clinic?.Name ?? string.Empty;
            string clinicStaffName = string.Empty;

            if (appointment.ClinicStaff?.User != null)
            {
                clinicStaffName = $"{appointment.ClinicStaff.User.FirstName} {appointment.ClinicStaff.User.LastName}";
            }

            decimal? copaymentAmount = null;
            string? copaymentStatus = null;

            if (appointment.Copayment != null)
            {
                copaymentAmount = appointment.Copayment.Amount;
                copaymentStatus = appointment.Copayment.Status;
            }

            return new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = patientName,
                ClinicId = appointment.ClinicId,
                ClinicName = clinicName,
                ClinicStaffId = appointment.ClinicStaffId,
                ClinicStaffName = clinicStaffName,
                AppointmentDate = appointment.AppointmentDate,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status,
                Type = appointment.Type,
                Notes = appointment.Notes,
                CancellationReason = appointment.CancellationReason,
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt,
                CopaymentId = appointment.Copayment?.Id,
                CopaymentAmount = copaymentAmount,
                CopaymentStatus = copaymentStatus
            };
        }

        private HealthRecordDto MapToHealthRecordDto(HealthRecord record)
        {
            string patientName = string.Empty;
            string clinicStaffName = string.Empty;

            if (record.ClinicStaff?.User != null)
            {
                clinicStaffName = $"{record.ClinicStaff.User.FirstName} {record.ClinicStaff.User.LastName}";
            }

            return new HealthRecordDto
            {
                Id = record.Id,
                PatientId = record.PatientId,
                PatientName = patientName,
                AppointmentId = record.AppointmentId,
                AppointmentDate = record.Appointment?.AppointmentDate,
                ClinicStaffId = record.ClinicStaffId,
                ClinicStaffName = clinicStaffName,
                RecordType = record.RecordType,
                Title = record.Title,
                Description = record.Description,
                FileUrl = record.FileUrl,
                IsSharedWithPatient = record.IsSharedWithPatient,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt
            };
        }

        private CopaymentDto MapToCopaymentDto(Copayment copayment)
        {
            string patientName = string.Empty;
            string insuranceProviderName = copayment.Insurance?.InsuranceProvider?.Name ?? string.Empty;
            string policyNumber = copayment.Insurance?.PolicyNumber ?? string.Empty;

            return new CopaymentDto
            {
                Id = copayment.Id,
                AppointmentId = copayment.AppointmentId,
                PatientId = copayment.PatientId,
                PatientName = patientName,
                InsuranceId = copayment.InsuranceId,
                InsuranceProviderName = insuranceProviderName,
                PolicyNumber = policyNumber,
                Amount = copayment.Amount,
                InsuranceCoverage = copayment.InsuranceCoverage,
                PatientResponsibility = copayment.PatientResponsibility,
                Status = copayment.Status,
                PaymentDate = copayment.PaymentDate,
                PaymentMethod = copayment.PaymentMethod,
                TransactionId = copayment.TransactionId,
                ReceiptUrl = copayment.ReceiptUrl,
                CreatedAt = copayment.CreatedAt,
                UpdatedAt = copayment.UpdatedAt
            };
        }

        #endregion
    }
}