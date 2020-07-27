using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using System.Data.Common;

namespace NicolaPIermatteiWec.Services
{
    public class DataAccess : IDisposable, IDataAccess
    {
        private readonly string _connectionString;
        private DbConnection _conn;
        public DataAccess(string connectionString)
        {
            _connectionString = connectionString;
            GetConnection();
        }

        public DbConnection GetConnection()
        {
            _conn = new SqlConnection(_connectionString);
            _conn.Open();
            return _conn;
        }

        public void Dispose()
        {
            _conn.Dispose();
        }
    }
}
