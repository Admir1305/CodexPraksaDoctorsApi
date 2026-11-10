using Dapper;
using DoctorAppointmentApi.Models;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Options;

namespace DoctorAppointmentApi.Repository.implements
{
    public class UserRepository : IUserRepository
    {
        private readonly DBSettings _dbSettings;

        public UserRepository(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        public async Task<Users> AuthenticateAsync(string Korisnik, string Lozinka)
        {


            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"SELECT *
                               FROM Users
                               WHERE UPPER(Korisnik) = UPPER(@Korisnik)
                                 AND Lozinka = @Lozinka
                         AND Aktiven = 1";

                return await conn.QueryFirstOrDefaultAsync<Users>(sql, new { Korisnik = Korisnik, Lozinka = Lozinka });
            }
        }

        public async Task<Users> GetByUsernameAsync(string Korisnik)
        {
            using (FbConnection conn = new(_dbSettings.DbConnectionString))
            {
                string sql = @"SELECT * 
                               FROM Users 
                               WHERE Korisnik = @Korisnik";

                return await conn.QueryFirstOrDefaultAsync<Users>(sql, new { Korisnik = Korisnik });
            }
        }
        public async Task<Users> GetByIdAsync(int id)
        {
            using (FbConnection connection = new(_dbSettings.DbConnectionString))
            {
                string sql = "SELECT * FROM USERS WHERE ID = @Id";
                return await connection.QueryFirstOrDefaultAsync<Users>(sql, new { Id = id });
            }
        }
    }
}
