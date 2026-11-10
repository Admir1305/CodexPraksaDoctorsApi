using DoctorAppointmentApi.Models;
using DoctorAppointmentApi.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorsRepository _doctorsRepository;
        private readonly ISpecialtiesRepository _specialtiesRepository;

        public DoctorsController(IDoctorsRepository doctorsRepository, ISpecialtiesRepository specialtiesRepository)
        {
            _doctorsRepository = doctorsRepository;
            _specialtiesRepository = specialtiesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var results = await _doctorsRepository.Get();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _doctorsRepository.GetById(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Doctors createDto)
        {
            if (await _specialtiesRepository.GetById(createDto.SpecialtyId!.Value) == null)
            {
                ModelState.AddModelError(nameof(createDto.SpecialtyId), "Select a specialty that exists.");
                return ValidationProblem(ModelState);
            }

            return Ok(await _doctorsRepository.Post(createDto));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _doctorsRepository.Delete(id);
            return result == 0 ? NotFound() : Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Doctors updateDto)
        {
            if (id != updateDto.Id) return BadRequest("The route ID does not match the doctor ID.");
            if (await _specialtiesRepository.GetById(updateDto.SpecialtyId!.Value) == null)
            {
                ModelState.AddModelError(nameof(updateDto.SpecialtyId), "Select a specialty that exists.");
                return ValidationProblem(ModelState);
            }

            var result = await _doctorsRepository.Put(id, updateDto);
            return result == 0 ? NotFound() : Ok(result);
        } 
    }
}
