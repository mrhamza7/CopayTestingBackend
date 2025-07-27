using CP1Testing.DTOs.Appointments;
using CP1Testing.DTOs.Copayments;
using CP1Testing.Entities;
using CP1Testing.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CP1Testing.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<Appointment> _appointmentRepository;
        private readonly IRepository<Copayment> _copaymentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IRepository<ClinicStaff> _clinicStaffRepository;
        private readonly IRepository<Insurance> _insuranceRepository;
        private readonly IRepository<InsuranceProvider> _insuranceProviderRepository;
        private readonly IUserRepository _userRepository;

        public AppointmentService(
            IRepository<Appointment> appointmentRepository,
            IRepository<Copayment> copaymentRepository,
            IPatientRepository patientRepository,
            IClinicRepository clinicRepository,
            IRepository<ClinicStaff> clinicStaffRepository,
            IRepository<Insurance> insuranceRepository,
            IRepository<InsuranceProvider> insuranceProviderRepository,
            IUserRepository userRepository)
        {
            _appointmentRepository = appointmentRepository;
            _copaymentRepository = copaymentRepository;
            _patientRepository = patientRepository;
            _clinicRepository = clinicRepository;
            _clinicStaffRepository = clinicStaffRepository;
            _insuranceRepository = insuranceRepository;
            _insuranceProviderRepository = insuranceProviderRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            var appointmentDtos = new List<AppointmentDto>();

            foreach (var appointment in appointments)
            {
                appointmentDtos.Add(await MapToAppointmentDtoAsync(appointment));
            }

            return appointmentDtos;
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(Guid id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null) return null;

            return await MapToAppointmentDtoAsync(appointment);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByClinicAsync(Guid clinicId)
        {
            var appointments = await _appointmentRepository.FindAsync(a => a.ClinicId == clinicId);
            var appointmentDtos = new List<AppointmentDto>();

            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    appointmentDtos.Add(await MapToAppointmentDtoAsync(appointment));
                }
            }

            return appointmentDtos;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByClinicStaffAsync(Guid clinicStaffId)
        {
            var appointments = await _appointmentRepository.FindAsync(a => a.ClinicStaffId == clinicStaffId);
            var appointmentDtos = new List<AppointmentDto>();

            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    appointmentDtos.Add(await MapToAppointmentDtoAsync(appointment));
                }
            }

            return appointmentDtos;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientAsync(Guid patientId)
        {
            var appointments = await _appointmentRepository.FindAsync(a => a.PatientId == patientId);
            var appointmentDtos = new List<AppointmentDto>();

            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    appointmentDtos.Add(await MapToAppointmentDtoAsync(appointment));
                }
            }

            return appointmentDtos;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var appointments = await _appointmentRepository.FindAsync(a => 
                a.AppointmentDate >= startDate.Date && 
                a.AppointmentDate <= endDate.Date);
            
            var appointmentDtos = new List<AppointmentDto>();

            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    appointmentDtos.Add(await MapToAppointmentDtoAsync(appointment));
                }
            }

            return appointmentDtos;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByClinicAndDateAsync(Guid clinicId, DateTime date)
        {
            var appointments = await _appointmentRepository.FindAsync(a => 
                a.ClinicId == clinicId && 
                a.AppointmentDate.Date == date.Date);
            
            var appointmentDtos = new List<AppointmentDto>();

            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    appointmentDtos.Add(await MapToAppointmentDtoAsync(appointment));
                }
            }

            return appointmentDtos;
        }

        public async Task<(bool Success, string Message, AppointmentDto? Appointment)> CreateAppointmentAsync(CreateAppointmentRequestDto model)
        {
            // Validate patient
            var patient = await _patientRepository.GetByIdAsync(model.PatientId);
            if (patient == null)
            {
                return (false, "Patient not found", null);
            }

            // Validate clinic
            var clinic = await _clinicRepository.GetByIdAsync(model.ClinicId);
            if (clinic == null)
            {
                return (false, "Clinic not found", null);
            }

            // Validate clinic staff if provided
            if (model.ClinicStaffId.HasValue)
            {
                var clinicStaff = await _clinicStaffRepository.GetByIdAsync(model.ClinicStaffId.Value);
                if (clinicStaff == null)
                {
                    return (false, "Clinic staff not found", null);
                }

                // Ensure clinic staff belongs to the clinic
                if (clinicStaff.ClinicId != model.ClinicId)
                {
                    return (false, "Clinic staff does not belong to the selected clinic", null);
                }
            }

            // Check for scheduling conflicts
            var conflictingAppointments = await _appointmentRepository.FindAsync(a =>
                a.ClinicId == model.ClinicId &&
                a.AppointmentDate.Date == model.AppointmentDate.Date &&
                ((a.StartTime <= model.StartTime && a.EndTime > model.StartTime) ||
                (a.StartTime < model.EndTime && a.EndTime >= model.EndTime) ||
                (a.StartTime >= model.StartTime && a.EndTime <= model.EndTime)) &&
                (model.ClinicStaffId == null || a.ClinicStaffId == model.ClinicStaffId) &&
                a.Status != "Cancelled");

            if (conflictingAppointments != null && conflictingAppointments.Any())
            {
                return (false, "The selected time slot is not available", null);
            }

            // Create appointment
            var appointment = new Appointment
            {
                PatientId = model.PatientId,
                ClinicId = model.ClinicId,
                ClinicStaffId = model.ClinicStaffId,
                AppointmentDate = model.AppointmentDate.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Status = "Scheduled",
                Type = model.Type,
                Notes = model.Notes
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return (true, "Appointment created successfully", await MapToAppointmentDtoAsync(appointment));
        }

        public async Task<(bool Success, string Message, AppointmentDto? Appointment)> UpdateAppointmentAsync(Guid id, UpdateAppointmentRequestDto model)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                return (false, "Appointment not found", null);
            }

            // Validate clinic staff if provided
            if (model.ClinicStaffId.HasValue)
            {
                var clinicStaff = await _clinicStaffRepository.GetByIdAsync(model.ClinicStaffId.Value);
                if (clinicStaff == null)
                {
                    return (false, "Clinic staff not found", null);
                }

                // Ensure clinic staff belongs to the clinic
                if (clinicStaff.ClinicId != appointment.ClinicId)
                {
                    return (false, "Clinic staff does not belong to the appointment's clinic", null);
                }

                appointment.ClinicStaffId = model.ClinicStaffId;
            }

            // Check for scheduling conflicts if date or time is being changed
            if ((model.AppointmentDate.HasValue && model.AppointmentDate.Value.Date != appointment.AppointmentDate.Date) ||
                (model.StartTime.HasValue && model.StartTime.Value != appointment.StartTime) ||
                (model.EndTime.HasValue && model.EndTime.Value != appointment.EndTime))
            {
                var appointmentDate = model.AppointmentDate ?? appointment.AppointmentDate;
                var startTime = model.StartTime ?? appointment.StartTime;
                var endTime = model.EndTime ?? appointment.EndTime;
                
                var conflictingAppointments = await _appointmentRepository.FindAsync(a =>
                    a.Id != id &&
                    a.ClinicId == appointment.ClinicId &&
                    a.AppointmentDate.Date == appointmentDate.Date &&
                    ((a.StartTime <= startTime && a.EndTime > startTime) ||
                    (a.StartTime < endTime && a.EndTime >= endTime) ||
                    (a.StartTime >= startTime && a.EndTime <= endTime)) &&
                    (appointment.ClinicStaffId == null || a.ClinicStaffId == appointment.ClinicStaffId) &&
                    a.Status != "Cancelled");

                if (conflictingAppointments != null && conflictingAppointments.Any())
                {
                    return (false, "The selected time slot is not available", null);
                }

                if (model.AppointmentDate.HasValue)
                    appointment.AppointmentDate = model.AppointmentDate.Value.Date;
                if (model.StartTime.HasValue)
                    appointment.StartTime = model.StartTime.Value;
                if (model.EndTime.HasValue)
                    appointment.EndTime = model.EndTime.Value;
            }

            // Update other properties
            if (!string.IsNullOrEmpty(model.Status))
                appointment.Status = model.Status;
            if (!string.IsNullOrEmpty(model.Type))
                appointment.Type = model.Type;
            if (!string.IsNullOrEmpty(model.Notes))
                appointment.Notes = model.Notes;

            if (!string.IsNullOrEmpty(model.CancellationReason) && model.Status == "Cancelled")
            {
                appointment.CancellationReason = model.CancellationReason;
            }

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return (true, "Appointment updated successfully", await MapToAppointmentDtoAsync(appointment));
        }

        public async Task<(bool Success, string Message)> CancelAppointmentAsync(Guid id, string cancellationReason)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                return (false, "Appointment not found");
            }

            if (appointment.Status == "Completed")
            {
                return (false, "Cannot cancel a completed appointment");
            }

            appointment.Status = "Cancelled";
            appointment.CancellationReason = cancellationReason;

            await _appointmentRepository.UpdateAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();

            return (true, "Appointment cancelled successfully");
        }

        public async Task<bool> DeleteAppointmentAsync(Guid id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null) return false;

            await _appointmentRepository.SoftDeleteAsync(id);
            return await _appointmentRepository.SaveChangesAsync() > 0;
        }

        public async Task<CopaymentDto?> GetAppointmentCopaymentAsync(Guid appointmentId)
        {
            var copayment = await _copaymentRepository.FindAsync(c => c.AppointmentId == appointmentId);
            if (copayment == null || !copayment.Any()) return null;

            return await MapToCopaymentDtoAsync(copayment.First());
        }

        public async Task<(bool Success, string Message, CopaymentDto? Copayment)> CreateCopaymentAsync(CreateCopaymentRequestDto model)
        {
            // Validate appointment
            var appointment = await _appointmentRepository.GetByIdAsync(model.AppointmentId);
            if (appointment == null)
            {
                return (false, "Appointment not found", null);
            }

            // Validate patient
            var patient = await _patientRepository.GetByIdAsync(model.PatientId);
            if (patient == null)
            {
                return (false, "Patient not found", null);
            }

            // Ensure patient matches appointment
            if (appointment.PatientId != model.PatientId)
            {
                return (false, "Patient does not match the appointment", null);
            }

            // Check if copayment already exists
            var existingCopayment = await _copaymentRepository.FindAsync(c => c.AppointmentId == model.AppointmentId);
            if (existingCopayment != null && existingCopayment.Any())
            {
                return (false, "Copayment already exists for this appointment", null);
            }

            // Validate insurance if provided
            if (model.InsuranceId.HasValue)
            {
                var insurance = await _insuranceRepository.GetByIdAsync(model.InsuranceId.Value);
                if (insurance == null)
                {
                    return (false, "Insurance not found", null);
                }

                // Ensure insurance belongs to the patient
                if (insurance.PatientId != model.PatientId)
                {
                    return (false, "Insurance does not belong to the patient", null);
                }
            }

            // Create copayment
            var copayment = new Copayment
            {
                AppointmentId = model.AppointmentId,
                PatientId = model.PatientId,
                InsuranceId = model.InsuranceId,
                Amount = model.Amount,
                InsuranceCoverage = model.InsuranceCoverage,
                PatientResponsibility = model.PatientResponsibility,
                Status = "Pending"
            };

            await _copaymentRepository.AddAsync(copayment);
            await _copaymentRepository.SaveChangesAsync();

            return (true, "Copayment created successfully", await MapToCopaymentDtoAsync(copayment));
        }

        public async Task<(bool Success, string Message, CopaymentDto? Copayment)> UpdateCopaymentAsync(Guid id, UpdateCopaymentRequestDto model)
        {
            var copayment = await _copaymentRepository.GetByIdAsync(id);
            if (copayment == null)
            {
                return (false, "Copayment not found", null);
            }

            // Validate insurance if provided
            if (model.InsuranceId.HasValue)
            {
                var insurance = await _insuranceRepository.GetByIdAsync(model.InsuranceId.Value);
                if (insurance == null)
                {
                    return (false, "Insurance not found", null);
                }

                // Ensure insurance belongs to the patient
                if (insurance.PatientId != copayment.PatientId)
                {
                    return (false, "Insurance does not belong to the patient", null);
                }

                copayment.InsuranceId = model.InsuranceId;
            }

            // Update properties
            if (model.Amount.HasValue && model.Amount.Value > 0)
                copayment.Amount = model.Amount.Value;

            if (model.InsuranceCoverage.HasValue && model.InsuranceCoverage.Value >= 0)
                copayment.InsuranceCoverage = model.InsuranceCoverage;

            if (model.PatientResponsibility.HasValue && model.PatientResponsibility.Value >= 0)
                copayment.PatientResponsibility = model.PatientResponsibility.Value;

            if (!string.IsNullOrEmpty(model.Status))
                copayment.Status = model.Status;

            if (model.PaymentDate.HasValue)
                copayment.PaymentDate = model.PaymentDate;

            if (!string.IsNullOrEmpty(model.PaymentMethod))
                copayment.PaymentMethod = model.PaymentMethod;

            if (!string.IsNullOrEmpty(model.TransactionId))
                copayment.TransactionId = model.TransactionId;

            if (!string.IsNullOrEmpty(model.ReceiptUrl))
                copayment.ReceiptUrl = model.ReceiptUrl;

            await _copaymentRepository.UpdateAsync(copayment);
            await _copaymentRepository.SaveChangesAsync();

            return (true, "Copayment updated successfully", await MapToCopaymentDtoAsync(copayment));
        }

        #region Helper Methods

        private async Task<AppointmentDto> MapToAppointmentDtoAsync(Appointment appointment)
        {
            string patientName = string.Empty;
            string clinicName = string.Empty;
            string clinicStaffName = string.Empty;

            // Get patient name
            var patient = await _patientRepository.GetByIdAsync(appointment.PatientId);
            if (patient != null)
            {
                var user = await _userRepository.GetByIdAsync(patient.UserId);
                if (user != null)
                {
                    patientName = $"{user.FirstName} {user.LastName}";
                }
            }

            // Get clinic name
            var clinic = await _clinicRepository.GetByIdAsync(appointment.ClinicId);
            if (clinic != null)
            {
                clinicName = clinic.Name;
            }

            // Get clinic staff name
            if (appointment.ClinicStaffId.HasValue)
            {
                var clinicStaff = await _clinicStaffRepository.GetByIdAsync(appointment.ClinicStaffId.Value);
                if (clinicStaff != null)
                {
                    var user = await _userRepository.GetByIdAsync(clinicStaff.UserId);
                    if (user != null)
                    {
                        clinicStaffName = $"{user.FirstName} {user.LastName}";
                    }
                }
            }

            // Get copayment info
            var copayment = await _copaymentRepository.FindAsync(c => c.AppointmentId == appointment.Id);
            Guid? copaymentId = null;
            decimal? copaymentAmount = null;
            string? copaymentStatus = null;

            if (copayment != null && copayment.Any())
            {
                var firstCopayment = copayment.First();
                copaymentId = firstCopayment.Id;
                copaymentAmount = (decimal?)firstCopayment.Amount;
                copaymentStatus = firstCopayment.Status;
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
                CopaymentId = copaymentId,
                CopaymentAmount = copaymentAmount,
                CopaymentStatus = copaymentStatus
            };
        }

        private async Task<CopaymentDto> MapToCopaymentDtoAsync(Copayment copayment)
        {
            string patientName = string.Empty;
            string insuranceProviderName = string.Empty;
            string policyNumber = string.Empty;

            // Get patient name
            var patient = await _patientRepository.GetByIdAsync(copayment.PatientId);
            if (patient != null)
            {
                var user = await _userRepository.GetByIdAsync(patient.UserId);
                if (user != null)
                {
                    patientName = $"{user.FirstName} {user.LastName}";
                }
            }

            // Get insurance info
            if (copayment.InsuranceId.HasValue)
            {
                var insurance = await _insuranceRepository.GetByIdAsync(copayment.InsuranceId.Value);
                if (insurance != null)
                {
                    policyNumber = insurance.PolicyNumber;

                    var provider = await _insuranceProviderRepository.GetByIdAsync(insurance.InsuranceProviderId);
                    if (provider != null)
                    {
                        insuranceProviderName = provider.Name;
                    }
                }
            }

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