using DoctorAppointmentApi.Models;
using DoctorAppointmentApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ISpecialtiesRepository _specialtiesRepository;

        public SpecialtiesController(ISpecialtiesRepository specialtiesRepository)
        {
            _specialtiesRepository = specialtiesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _specialtiesRepository.Get();

            return Ok(results);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _specialtiesRepository.GetById(id);
            return result == null ? NotFound() : Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var results = await _specialtiesRepository.Delete(id);

            return results == 0 ? NotFound() : Ok(results);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Specialties spec)
        {
            if (id != spec.Id) return BadRequest("The route ID does not match the specialty ID.");
            var result = await _specialtiesRepository.Put(id, spec);
            return result == 0 ? NotFound() : Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post(Specialties spec)
        {
            return Ok(await _specialtiesRepository.Post(spec));
        }
    }
}
