using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using System.Data.Common;
using NicolaPIermatteiWec.Models;
using NicolaPIermatteiWec.Models.InsertModels;

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

        public async Task<DailyResponseModel> DailyInsertion(DailyInsert model)
        {
            try
            {
                var query = @"
                    INSERT INTO [DailyContact] (DispoContactId, DispoId, Prox, Province, Latitude, Longitude, Distance, DateContact)
                    VALUES (@DispoContactId, @DispoId, @Prox, @Province, @Latitude, @Longitude, @Distance, @DateContact)";
                var DateContact = DateTime.Parse(model.DateContact);
                var rows = await _conn.ExecuteAsync(query, new { model.DispoContactId, model.DispoId, model.Prox, model.Province, model.Latitude, model.Longitude, model.Distance, DateContact });
                if (rows > 0)
                {
                    return new DailyResponseModel { StatusCode = 200, Message = "Insertion executed" };
                }
                else
                {
                    return new DailyResponseModel { StatusCode = 500, Message = "Insertion Not completed during Query" };
                }
            }
            catch (Exception ex)
            {
                return new DailyResponseModel { StatusCode = 500, Message = ex.Message};
            }
        }
    }
}
