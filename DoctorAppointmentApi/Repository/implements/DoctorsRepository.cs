using Dapper;
using DoctorAppointmentApi.Models;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using System.Numerics;

namespace DoctorAppointmentApi.Repository.implements
{
    public class DoctorsRepository : IDoctorsRepository
    {
        private readonly DBSettings _dbSettings;
        public DoctorsRepository(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        public async Task<int> Delete(int id)
        {
            using (FbConnection conn = new (_dbSettings.DbConnectionString))
            {
                string sql = @"DELETE FROM Doctors WHERE Id = @id";
                var results = await conn.ExecuteAsync(sql, new {id});

                return results;
            }
        }

        public async Task<IEnumerable<Doctors>> Get()
        {
            using (FbConnection connection = new(_dbSettings.DbConnectionString))
            {
                string sql = @"SELECT DOC.ID, DOC.FIRSTNAME, DOC.LASTNAME, DOC.SPECIALTYID,
                                      DOC.PHONE, DOC.ISACTIVE, SP.NAME AS SPECIALTYNAME
                               FROM DOCTORS DOC
                               LEFT JOIN SPECIALTIES SP ON SP.ID = DOC.SPECIALTYID";
                var results = await connection.QueryAsync<Doctors>(sql);
                return results;
            }
        }

        public async Task<Doctors> GetById(int id)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString)) // tuka go zemam string za konekcija so baza
            {
                string sql = @"SELECT DOC.ID, DOC.FIRSTNAME, DOC.LASTNAME, DOC.SPECIALTYID,
                                      DOC.PHONE, DOC.ISACTIVE, SP.NAME AS SPECIALTYNAME
                               FROM DOCTORS DOC
                               LEFT JOIN SPECIALTIES SP ON SP.ID = DOC.SPECIALTYID
                               WHERE DOC.ID = @id";
                var results = await conn.QueryFirstOrDefaultAsync<Doctors>(sql, new { id });

                return results;
            }
        }
        // kreiranje zapis
        public async Task<int> Post(Doctors createDto)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"INSERT INTO Doctors (FirstName,LastName,SpecialtyId,Phone,IsActive) 
                               values (@FirstName,@LastName,@SpecialtyId,@Phone,@IsActive)";

                var result = await conn.ExecuteAsync(sql, new
                {
                    FirstName = createDto.FirstName,
                    LastName = createDto.LastName,
                    SpecialtyId = createDto.SpecialtyId,
                    Phone = createDto.Phone,
                    IsActive = createDto.IsActive
                });
                return result;
            }

        }
        // azhuriranje nov zapis
        public async Task<int> Put(int Id, Doctors updateDto)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"UPDATE Doctors  
                             set
                             FirstName = @FirstName,
                             LastName = @LastName,
                             SpecialtyId = @SpecialtyId,
                             Phone = @Phone,
                             IsActive = @IsActive
                             WHERE Id = @Id";
                var result = await conn.ExecuteAsync(sql, new
                {
                    Id = Id,
                    FirstName = updateDto.FirstName,
                    LastName = updateDto.LastName,
                    SpecialtyId = updateDto.SpecialtyId,
                    Phone = updateDto.Phone,
                    IsActive = updateDto.IsActive
                });
                return result;
            }
        }
    }
}
