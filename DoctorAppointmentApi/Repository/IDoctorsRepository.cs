using DoctorAppointmentApi.Models;

namespace DoctorAppointmentApi.Repository
{
    public interface IDoctorsRepository
    {
        Task<IEnumerable<Doctors>> Get();

        Task<Doctors> GetById(int id);

        Task<int> Post(Doctors createDto);

        Task<int> Put(int id, Doctors updateDto);

        Task<int> Delete(int id);
    }
}
