using CP1Testing.DTOs.Appointments;
using CP1Testing.DTOs.Copayments;
using CP1Testing.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CP1Testing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(Guid id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);

            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found" });
            }

            return Ok(appointment);
        }

        [HttpGet("clinic/{clinicId}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> GetAppointmentsByClinic(Guid clinicId)
        {
            var appointments = await _appointmentService.GetAppointmentsByClinicAsync(clinicId);
            return Ok(appointments);
        }

        [HttpGet("staff/{clinicStaffId}")]
        public async Task<IActionResult> GetAppointmentsByClinicStaff(Guid clinicStaffId)
        {
            var appointments = await _appointmentService.GetAppointmentsByClinicStaffAsync(clinicStaffId);
            return Ok(appointments);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAppointmentsByPatient(Guid patientId)
        {
            var appointments = await _appointmentService.GetAppointmentsByPatientAsync(patientId);
            return Ok(appointments);
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetAppointmentsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var appointments = await _appointmentService.GetAppointmentsByDateRangeAsync(startDate, endDate);
            return Ok(appointments);
        }

        [HttpGet("clinic/{clinicId}/date/{date}")]
        public async Task<IActionResult> GetAppointmentsByClinicAndDate(Guid clinicId, DateTime date)
        {
            var appointments = await _appointmentService.GetAppointmentsByClinicAndDateAsync(clinicId, date);
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _appointmentService.CreateAppointmentAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return CreatedAtAction(nameof(GetAppointmentById), new { id = result.Appointment.Id }, result.Appointment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _appointmentService.UpdateAppointmentAsync(id, model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.Appointment);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelAppointment(Guid id, [FromBody] string cancellationReason)
        {
            var result = await _appointmentService.CancelAppointmentAsync(id, cancellationReason);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            var result = await _appointmentService.DeleteAppointmentAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Appointment not found" });
            }

            return NoContent();
        }

        // Copayment related endpoints
        [HttpGet("{appointmentId}/copayment")]
        public async Task<IActionResult> GetAppointmentCopayment(Guid appointmentId)
        {
            var copayment = await _appointmentService.GetAppointmentCopaymentAsync(appointmentId);

            if (copayment == null)
            {
                return NotFound(new { message = "Copayment not found" });
            }

            return Ok(copayment);
        }

        [HttpPost("copayment")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> CreateCopayment([FromBody] CreateCopaymentRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _appointmentService.CreateCopaymentAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.Copayment);
        }

        [HttpPut("copayment/{id}")]
        [Authorize(Roles = "SuperAdmin,ClinicAdmin")]
        public async Task<IActionResult> UpdateCopayment(Guid id, [FromBody] UpdateCopaymentRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _appointmentService.UpdateCopaymentAsync(id, model);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(result.Copayment);
        }
    }
}