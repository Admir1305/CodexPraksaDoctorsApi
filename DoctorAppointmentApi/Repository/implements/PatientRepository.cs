using Dapper;
using DoctorAppointmentApi.Models;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Options;

namespace DoctorAppointmentApi.Repository.implements
{
    public class PatientRepository : IPatientRepository
    {
        private readonly DBSettings _dbSettings;

        public PatientRepository(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        public async Task<int> Delete(int id)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"DELETE FROM Patients WHERE id=@id";
                return await conn.ExecuteAsync(sql, new { id });
            }
        }

        public async Task<IEnumerable<Patients>> Get()
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"SELECT * FROM Patients";
                return await conn.QueryAsync<Patients>(sql);
            }
        }

        public async Task<Patients> GetById(int id)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"SELECT * FROM Patients WHERE id = @id";
                return await conn.QueryFirstOrDefaultAsync<Patients>(sql, new { id });
            }
        }
        public async Task<bool> IsEmbgTakenAsync(string embg, int? excludePatientId = null)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"SELECT COUNT(1)
                               FROM Patients
                               WHERE TRIM(EMBG) = TRIM(@Embg)
                                 AND (@ExcludePatientId IS NULL OR Id <> @ExcludePatientId)";

                int count = await conn.ExecuteScalarAsync<int>(sql, new
                {
                    Embg = embg,
                    ExcludePatientId = excludePatientId
                });

                return count > 0;
            }
        }

        public async Task<int> Post(Patients patient)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"INSERT INTO Patients (FirstName, LastName, EMBG, Phone, Email)
                               VALUES (@FirstName, @LastName, @EMBG, @Phone, @Email)";

                return await conn.ExecuteAsync(sql, patient);
            }
        }

        public async Task<int> Put(int id, Patients patient)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"UPDATE Patients
                               SET FirstName = @FirstName,
                                   LastName = @LastName,
                                   EMBG = @EMBG,
                                   Phone = @Phone,
                                   Email = @Email
                               WHERE Id = @Id";

                return await conn.ExecuteAsync(sql, new
                {
                    Id = id,
                    patient.FirstName,
                    patient.LastName,
                    patient.EMBG,
                    patient.Phone,
                    patient.Email
                });
            }
        }
    }
}
