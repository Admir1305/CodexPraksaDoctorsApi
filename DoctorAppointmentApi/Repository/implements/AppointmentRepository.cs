using Dapper;
using DoctorAppointmentApi.Models;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Options;
using System.Text;

namespace DoctorAppointmentApi.Repository.implements
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly DBSettings _dbSettings;
        private const string AppointmentSelectSql = @"
            SELECT a.id, a.doctorid, a.patientid, a.appointmenttime, a.notes,
                   a.status_id, st.name AS status_name,
                   p.firstname AS PatientFirstName, p.lastname AS PatientLastName,
                   p.embg AS PatientEmbg,
                   d.firstname AS DoctorFirstName, d.lastname AS DoctorLastName,
                   sp.name AS SpecialtyName
            FROM appointments a
            JOIN statuses st ON st.id = a.status_id
            LEFT JOIN patients p ON p.id = a.patientid
            LEFT JOIN doctors d ON d.id = a.doctorid
            LEFT JOIN specialties sp ON sp.id = d.specialtyid";

        public AppointmentRepository(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        public async Task<int> Delete(int id)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"DELETE FROM Appointments WHERE Id=@id";
                return await conn.ExecuteAsync(sql, new { id });
            }
        }

        public async Task<IEnumerable<Appointments>> Get()
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = AppointmentSelectSql + " ORDER BY a.appointmenttime";
                return await conn.QueryAsync<Appointments>(sql);
            }
        }

        public async Task<IEnumerable<AppointmentStatus>> GetStatuses()
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                const string sql = @"SELECT id, name FROM statuses ORDER BY id";
                return await conn.QueryAsync<AppointmentStatus>(sql);
            }
        }

        public async Task<Appointments> GetById(int id)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = AppointmentSelectSql + " WHERE a.id = @id";
                return await conn.QueryFirstOrDefaultAsync<Appointments>(sql, new { id });
            }
        }

        public async Task<int> Post(Appointments createDto)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"INSERT INTO Appointments
                               (DoctorId, PatientId, AppointmentTime, Status_id, Notes)
                               VALUES
                               (@DoctorId, @PatientId, @AppointmentTime, @Status, @Notes)";

                return await conn.ExecuteAsync(sql, new
                {
                    AppointmentTime = createDto.AppointmentTime,
                    Status = createDto.Status_id,
                    Notes = createDto.Notes,
                    DoctorId = createDto.DoctorId,
                    PatientId = createDto.PatientId
                });
            }
        }

        public async Task<int> Put(int id, Appointments updateDto)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"UPDATE Appointments
                               SET DoctorId = @DoctorId,
                                   PatientId = @PatientId,
                                   AppointmentTime = @AppointmentTime,
                                   Status_id = @Status,
                                   Notes = @Notes
                               WHERE id = @id";

                return await conn.ExecuteAsync(sql, new
                {
                    id,
                    AppointmentTime = updateDto.AppointmentTime,
                    Status = updateDto.Status_id,
                    Notes = updateDto.Notes,
                    DoctorId = updateDto.DoctorId,
                    PatientId = updateDto.PatientId
                });
            }
        }
        public async Task<bool> IsAppointmentOverlapAsync(int doctorId, DateTime appointmentTime, int? excludeAppointmentId = null)
        {
            var slotStart = new DateTime(
                appointmentTime.Year,
                appointmentTime.Month,
                appointmentTime.Day,
                appointmentTime.Hour,
                appointmentTime.Minute,
                0,
                appointmentTime.Kind);
            var slotEnd = slotStart.AddMinutes(1);

            using (FbConnection connection = new(_dbSettings.DbConnectionString))
            {
                string sql = @"SELECT COUNT(1)
                               FROM Appointments a
                               LEFT JOIN Statuses st ON st.Id = a.Status_id
                               WHERE a.DoctorId = @DoctorId
                                 AND a.AppointmentTime >= @SlotStart
                                 AND a.AppointmentTime < @SlotEnd
                                 AND (@ExcludeId IS NULL OR a.Id <> @ExcludeId)
                                 AND (st.Name IS NULL OR NOT
                                      (st.Name CONTAINING 'cancel' OR st.Name CONTAINING @CancelledMacedonian))";

                var count = await connection.ExecuteScalarAsync<int>(sql, new
                {
                    DoctorId = doctorId,
                    SlotStart = slotStart,
                    SlotEnd = slotEnd,
                    ExcludeId = excludeAppointmentId,
                    CancelledMacedonian = new string(new[] { (char)0x043E, (char)0x0442, (char)0x043A, (char)0x0430, (char)0x0436 })
                });

                return count > 0;
            }
        }

        public async Task<IEnumerable<Appointments>> GetByDoctorId(int doctorId)
        {
            using (FbConnection connection = new(_dbSettings.DbConnectionString))
            {
                string sql = AppointmentSelectSql + " WHERE a.doctorid = @DoctorId";
                return await connection.QueryAsync<Appointments>(sql, new { DoctorId = doctorId });
            }
        }

        public async Task<IEnumerable<Appointments>> GetByPatientId(int patientId)
        {
            using (FbConnection connection = new(_dbSettings.DbConnectionString))
            {
                string sql = AppointmentSelectSql + " WHERE a.patientid = @PatientId";
                return await connection.QueryAsync<Appointments>(sql, new { PatientId = patientId });
            }
        }

        public async Task<IEnumerable<Appointments>> GetFiltered(
            DateTime? dateFrom,
            DateTime? dateTo,
            int? statusId,
            int? doctorId,
            int? patientId,
            int? userDoctorId,
            int? userPatientId,
            bool isAdmin)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                StringBuilder sql = new(AppointmentSelectSql + " WHERE 1 = 1");
                var parameters = new DynamicParameters();

                if (dateFrom.HasValue)
                {
                    sql.Append(" AND a.appointmenttime >= @DateFrom");
                    parameters.Add("DateFrom", dateFrom.Value.Date);
                }

                if (dateTo.HasValue)
                {
                    sql.Append(" AND a.appointmenttime < @DateTo");
                    parameters.Add("DateTo", dateTo.Value.Date.AddDays(1));
                }

                if (statusId.HasValue)
                {
                    sql.Append(" AND a.status_id = @StatusId");
                    parameters.Add("StatusId", statusId.Value);
                }

                if (doctorId.HasValue)
                {
                    sql.Append(" AND a.doctorid = @DoctorId");
                    parameters.Add("DoctorId", doctorId.Value);
                }

                if (patientId.HasValue)
                {
                    sql.Append(" AND a.patientid = @PatientId");
                    parameters.Add("PatientId", patientId.Value);
                }

                if (!isAdmin)
                {
                    if (userDoctorId.HasValue)
                    {
                        sql.Append(" AND a.doctorid = @UserDoctorId");
                        parameters.Add("UserDoctorId", userDoctorId.Value);
                    }
                    else if (userPatientId.HasValue)
                    {
                        sql.Append(" AND a.patientid = @UserPatientId");
                        parameters.Add("UserPatientId", userPatientId.Value);
                    }
                }

                sql.Append(" ORDER BY a.appointmenttime");
                return await conn.QueryAsync<Appointments>(sql.ToString(), parameters);
            }
        }
    }
}
