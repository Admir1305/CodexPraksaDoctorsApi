using DoctorAppointmentApi.Models;

namespace DoctorAppointmentApi.Repository
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointments>> Get();

        Task<IEnumerable<AppointmentStatus>> GetStatuses();

        Task<Appointments> GetById(int id);

        Task<int> Post(Appointments createDto);

        Task<int> Put(int id, Appointments updateDto);

        Task<int> Delete(int id);
        Task<bool> IsAppointmentOverlapAsync(int doctorId, DateTime appointmentTime, int? excludeAppointmentId = null);

        Task<IEnumerable<Appointments>> GetByDoctorId(int doctorId);

        Task<IEnumerable<Appointments>> GetByPatientId(int patientId);

        Task<IEnumerable<Appointments>> GetFiltered(
            DateTime? dateFrom,
            DateTime? dateTo,
            int? statusId,
            int? doctorId,
            int? patientId,
            int? userDoctorId,
            int? userPatientId,
            bool isAdmin);
    }
}
