using DoctorAppointmentApi.Models;
using DoctorAppointmentApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DoctorAppointmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userRepository.AuthenticateAsync(loginDto.Korisnik, loginDto.Lozinka);

            if (user == null)
            {
                return Unauthorized("Invalid username or password, or the profile is inactive.");
            }

            return Ok(user);
        }
    }
}