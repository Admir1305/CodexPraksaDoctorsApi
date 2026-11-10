using DoctorAppointmentApi.Models;
using DoctorAppointmentApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientsController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _patientRepository.Get());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientRepository.GetById(id);
            return patient == null ? NotFound() : Ok(patient);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _patientRepository.Delete(id);
            return result == 0 ? NotFound() : Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post(Patients patient)
        {
            if (await _patientRepository.IsEmbgTakenAsync(patient.EMBG))
            {
                ModelState.AddModelError(nameof(patient.EMBG), "A patient with this EMBG already exists.");
                return ValidationProblem(ModelState);
            }

            return Ok(await _patientRepository.Post(patient));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Patients patient)
        {
            if (id != patient.Id)
            {
                return BadRequest("The route ID does not match the patient ID.");
            }

            if (await _patientRepository.IsEmbgTakenAsync(patient.EMBG, id))
            {
                ModelState.AddModelError(nameof(patient.EMBG), "A patient with this EMBG already exists.");
                return ValidationProblem(ModelState);
            }

            var result = await _patientRepository.Put(id, patient);
            return result == 0 ? NotFound() : Ok(result);
        }
    }
}
