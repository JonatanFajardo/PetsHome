using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace PetsHome.DataAccess
{
    public class PetsHomeDbContext : DbContext
    {
        public static string ConnectionString { get; set; }

        public static void BuildConnectionString(string connectionString)
        {
            var connString = new SqlConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

            ConnectionString = connString.ConnectionString;
        }
    }
}