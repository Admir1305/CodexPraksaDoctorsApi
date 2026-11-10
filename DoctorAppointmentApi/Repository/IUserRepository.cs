using DoctorAppointmentApi.Models;

namespace DoctorAppointmentApi.Repository
{
    public interface IUserRepository
    {
        Task<Users> AuthenticateAsync(string username, string password);
        Task<Users> GetByUsernameAsync(string username);

        Task<Users> GetByIdAsync(int id);

    }
}
