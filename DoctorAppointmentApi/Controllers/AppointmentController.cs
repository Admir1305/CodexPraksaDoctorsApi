using DoctorAppointmentApi.Models;
using DoctorAppointmentApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;

        public AppointmentController(
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository)
        {
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _appointmentRepository.Get());
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            return Ok(await _appointmentRepository.GetStatuses());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _appointmentRepository.GetById(id);
            return appointment == null ? NotFound() : Ok(appointment);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAppointmentsForUser(
            int userId,
            [FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo,
            [FromQuery] int? statusId,
            [FromQuery] int? doctorId,
            [FromQuery] int? patientId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            bool isAdmin = user.TipKorisnik == 0;
            if (!isAdmin && !user.DoctorId.HasValue && !user.PatientId.HasValue)
            {
                return BadRequest("User does not have a valid role or entity link.");
            }

            if (dateFrom.HasValue && dateTo.HasValue && dateFrom.Value.Date > dateTo.Value.Date)
            {
                return BadRequest("Start date cannot be after end date.");
            }

            var appointments = await _appointmentRepository.GetFiltered(
                dateFrom,
                dateTo,
                statusId,
                doctorId,
                patientId,
                user.DoctorId,
                user.PatientId,
                isAdmin);

            return Ok(appointments);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _appointmentRepository.Delete(id);
            return result == 0 ? NotFound() : Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post(Appointments createDto)
        {
            if (!await IsAllowedStatusAsync(createDto.Status_id))
            {
                return BadRequest("Choose Scheduled, In Progress, or Completed.");
            }

            if (createDto.AppointmentTime == default || createDto.AppointmentTime < DateTime.Now)
            {
                ModelState.AddModelError(nameof(createDto.AppointmentTime), "Appointment date and time cannot be in the past.");
                return ValidationProblem(ModelState);
            }

            if (await _appointmentRepository.IsAppointmentOverlapAsync(
                    createDto.DoctorId,
                    createDto.AppointmentTime))
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Appointment conflict",
                    Detail = "The doctor already has an appointment at this time."
                });
            }

            return Ok(await _appointmentRepository.Post(createDto));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Appointments updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("The route ID does not match the appointment ID.");
            }

            if (!await IsAllowedStatusAsync(updateDto.Status_id))
            {
                return BadRequest("Choose Scheduled, In Progress, or Completed.");
            }

            if (updateDto.AppointmentTime == default || updateDto.AppointmentTime < DateTime.Now)
            {
                ModelState.AddModelError(nameof(updateDto.AppointmentTime), "Appointment date and time cannot be in the past.");
                return ValidationProblem(ModelState);
            }

            if (await _appointmentRepository.IsAppointmentOverlapAsync(
                    updateDto.DoctorId,
                    updateDto.AppointmentTime,
                    id))
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Appointment conflict",
                    Detail = "The doctor already has an appointment at this time."
                });
            }

            var result = await _appointmentRepository.Put(id, updateDto);
            return result == 0 ? NotFound() : Ok(result);
        }

        private async Task<bool> IsAllowedStatusAsync(int? statusId)
        {
            if (!statusId.HasValue)
            {
                return false;
            }

            var status = (await _appointmentRepository.GetStatuses())
                .FirstOrDefault(item => item.Id == statusId.Value);
            if (status == null)
            {
                return false;
            }

            var name = status.Name.Trim();
            return name.Contains("schedul", StringComparison.OrdinalIgnoreCase)
                || name.Contains("закаж", StringComparison.OrdinalIgnoreCase)
                || name.Contains("zakaz", StringComparison.OrdinalIgnoreCase)
                || name.Contains("progress", StringComparison.OrdinalIgnoreCase)
                || name.Contains("во тек", StringComparison.OrdinalIgnoreCase)
                || name.Contains("tek", StringComparison.OrdinalIgnoreCase)
                || name.Contains("complete", StringComparison.OrdinalIgnoreCase)
                || name.Contains("заврш", StringComparison.OrdinalIgnoreCase)
                || name.Contains("zavr", StringComparison.OrdinalIgnoreCase);
        }
    }
}
