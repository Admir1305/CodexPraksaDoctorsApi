using DoctorAppointmentApi.Models;

namespace DoctorAppointmentApi.Repository
{
    public interface ISpecialtiesRepository
    {
        Task<IEnumerable<Specialties>> Get();
        Task<Specialties?> GetById(int id);
        Task<int> Post(Specialties spec);
        Task<int> Put(int id, Specialties spec);
        Task<int> Delete(int id);
    }
}
