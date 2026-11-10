using DoctorAppointmentApi.Models;

namespace DoctorAppointmentApi.Repository
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patients>> Get();

        Task<Patients> GetById(int id);

        Task<int> Post(Patients patient);

        Task<int> Put(int id, Patients patient);

        Task<int> Delete(int id);
        Task<bool> IsEmbgTakenAsync(string embg, int? excludePatientId = null);
    }
}
