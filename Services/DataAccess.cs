using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using System.Data.Common;
using NicolaPIermatteiWec.Models;
using NicolaPIermatteiWec.Models.InsertModels;
using NicolaPIermatteiWec.Models.ViewModels;

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

        public async Task<ResponseModel> DailyInsertion(DailyInsert model)
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

                    //var findPositive = @"SELECT * FROM Positive WHERE DispoId = @DispoContactId";
                    //var queryPositive = await _conn.QueryFirstOrDefaultAsync<Positive>(findPositive, new { model.DispoContactId });
                    //if (queryPositive != null)
                    //{
                    //    var scoreForDate = ScoreForDate(queryPositive.DatePositive, DateContact);
                    //    var scoreForProx = ScoreForProx(model.Prox);
                    //    var Score = scoreForDate + scoreForProx;
                    //    var DateOfRelevation = DateContact;

                    //    return new ResponseModel { StatusCode = 200, Message = "Insertion executed" };
                    //}
                    //return new ResponseModel { StatusCode = 500, Message = "Insertion Not completed during Query" };
                    return new ResponseModel { StatusCode = 200, Message = "Insertion executed" };
                }
                else
                {
                    return new ResponseModel { StatusCode = 500, Message = "Insertion Not completed during Query" };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel { StatusCode = 500, Message = ex.Message};
            }
        }
        public async Task<ResponseModel> PositiveInsertion(PositiveInsert model)
        {
            try
            {
                var query = @"
                    INSERT INTO [Positive] (DispoId, Positive, DatePositive)
                    VALUES (@DispoId, @Positive, @DatePositive)";
                var DatePositive = DateTime.Parse(model.DatePositive);
                var rows = await _conn.ExecuteAsync(query, new { model.DispoId, model.Positive, DatePositive });
                if (rows > 0)
                {
                    return new ResponseModel { StatusCode = 200, Message = "Insertion executed" };
                }
                else
                {
                    return new ResponseModel { StatusCode = 500, Message = "Insertion Not completed during Query" };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel { StatusCode = 500, Message = ex.Message };
            }
        }

        public async Task<List<DistanceByProvince>> GetDistanceByProvinceTableData()
        {
            var LastMonth = DateTime.Now.AddDays(-30).Date;
            var query = @"SELECT DC.Province, 
                            (SELECT COUNT(*)
                            FROM DailyContact as D
                            WHERE D.Prox = 1 AND D.Province = DC.Province) AS LessThanOne,
                            (SELECT COUNT(*)
                            FROM DailyContact as D
                            WHERE D.Prox = 2 AND D.Province = DC.Province) AS FromOneToHalf,
                            (SELECT COUNT(*)
                            FROM DailyContact as D
                            WHERE D.Prox = 3 AND D.Province = DC.Province) AS FromHalfToTwo,
                            (SELECT COUNT(*)
                            FROM DailyContact as D
                            WHERE D.Prox = 4 AND D.Province = DC.Province) AS FromTwoToHalf,
                            (SELECT COUNT(*)
                            FROM DailyContact as D
                            WHERE D.Prox = 5 AND D.Province = DC.Province) AS MoreThanTwoHalf
                            FROM DailyContact AS DC
                            WHERE DC.DateContact >= @LastMonth
                            GROUP BY DC.Province";
            var res = await _conn.QueryAsync<DistanceByProvince>(query, new { LastMonth });
            return res.ToList();
        }

        public async Task<List<TypeOfRelevation>> GetTypeOfRelevationsData()
        {
            var LastMonth = DateTime.Now.AddDays(-30).Date;
            var query = @"SELECT DC.Province, Count(*) AS NumOfContacts, 
                            (SELECT COUNT(*)
                            FROM DailyContact as D
                            JOIN Positive AS P ON P.DispoId = D.DispoContactId
                            WHERE P.Positive = 1 AND P.DatePositive > CAST('2020-06-27' AS datetime2) AND D.Province = DC.Province) AS NumberOfPositives,
                            (SELECT COUNT (*)
                            FROM DailyContact as Dalt
                            JOIN Positive AS Palt ON Palt.DispoId = Dalt.DispoContactId
                            JOIN Scoring AS S ON S.DispoId = Palt.DispoId
                            WHERE Dalt.Province = DC.Province AND S.Score > 0 AND S.DateOfRelevation > @LastMonth) AS Proximity
                            FROM DailyContact AS DC
                            WHERE DateContact >= @LastMonth
                            GROUP BY DC.Province";
            var res = await _conn.QueryAsync<TypeOfRelevation>(query, new { LastMonth });
            return res.ToList();
        }

        private int ScoreForDate(DateTime firstDate, DateTime secondDate)
        {
            int diff = (int)(firstDate.Date - secondDate.Date).TotalDays;
            if (diff <= 5)
            {
                return 10;
            }
            if (diff <= 7)
            {
                return 7;
            }
            if (diff <= 10)
            {
                return 5;
            }
            if (diff <= 15)
            {
                return 3;
            }
            if (diff <= 20)
            {
                return 1;
            }
            return 0;
        }
        private int ScoreForProx(float diff)
        {
            if (diff == 1)
            {
                return 10;
            }
            if (diff == 2)
            {
                return 8;
            }
            if (diff == 3)
            {
                return 5;
            }
            if (diff == 4)
            {
                return 2;
            }
            if (diff == 5)
            {
                return 1;
            }
            return 0;
        }
    }
}
