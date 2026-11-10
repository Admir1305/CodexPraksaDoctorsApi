using Dapper;
using DoctorAppointmentApi.Models;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Options;
using System.Data.Common;

namespace DoctorAppointmentApi.Repository.implements
{
    public class SpecialtiesRepository : ISpecialtiesRepository
    {
        private readonly DBSettings _dbSettings;

        public SpecialtiesRepository(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }
        public async Task<int> Delete(int id)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"DELETE FROM Specialties WHERE Id=@id";

                var results = await conn.ExecuteAsync(sql, new {id});

                return results;
            } 
        }

        public async Task<IEnumerable<Specialties>> Get()
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"SELECT Id, Name FROM Specialties";

                var results = await conn.QueryAsync<Specialties>(sql);

                return results;
            }
        }

        public async Task<Specialties?> GetById(int id)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"SELECT Id, Name FROM Specialties WHERE id=@id";

                var results = await conn.QueryFirstOrDefaultAsync<Specialties>(sql, new { id });

                return results;
            }
        }

        public async Task<int> Post(Specialties spec)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"INSERT INTO Specialties (Name) values(@Name)";

                var results = await conn.ExecuteAsync(sql, new
                {
                    Name = spec.Name
                });

                return results;
            }
        }

        public async Task<int> Put(int id, Specialties spec)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"UPDATE Specialties s SET s.Name = @Name WHERE s.Id = @Id";

                var results = await conn.ExecuteAsync(sql, new
                {
                    Id = id,
                    Name = spec.Name
                });
                return results;
            }
        }
    }
}
