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
using NicolaPIermatteiWec.Models.DbModels;

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

                    var findPositive = @"SELECT * FROM Positive WHERE DispoId = @DispoContactId";
                    var queryPositive = await _conn.QueryFirstOrDefaultAsync<PositiveTable>(findPositive, new { model.DispoContactId });
                    if (queryPositive != null)
                    {
                        var scoreForDate = ScoreForDate(queryPositive.DatePositive, DateContact);
                        var scoreForProx = ScoreForProx(model.Prox);
                        var Score = scoreForDate + scoreForProx;
                        var DateOfRelevation = DateContact;
                        await _conn.ExecuteAsync("INSERT INTO Scoring (Score, DispoId, DateOfRelevation) VALUES (@Score, @DispoId, @DateOfRelevation)", new { Score, model.DispoId, DateOfRelevation});
                        return new ResponseModel { StatusCode = 200, Message = "Insertion executed" };
                    }
                    return new ResponseModel { StatusCode = 500, Message = "Insertion Not completed during Query" };
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
                            WHERE P.Positive = 1 
							AND P.DatePositive > CAST('2020-06-27' AS datetime2) 
							AND D.Province = DC.Province
							AND (SELECT MIN(DatePositive) FROM Positive WHERE D.DispoContactId = DispoId) <= D.DateContact) AS NumberOfPositives,
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

        public async Task<List<TopTenDevicesByContact>> GetTopTenDevicesData()
        {
            var query = @"SELECT TOP(10) DC.DispoId, COUNT (*) AS HowMany
                          FROM DailyContact AS DC
                          GROUP BY DC.DispoId
                          ORDER BY HowMany DESC";
            var res = await _conn.QueryAsync<TopTenDevicesByContact>(query);
            return res.ToList();
        }

        public async Task<List<TopTenByScore>> GetTopTenByScoresData()
        {
            var query = @"SELECT TOP(10) S.DispoId, SUM(S.Score) AS Score
                        FROM Scoring as S
                        GROUP BY S.DispoId
                        ORDER BY Score DESC";
            var res = await _conn.QueryAsync<TopTenByScore>(query);
            return res.ToList();
        }
        public async Task<ResultForTables> ResultForTables()
        {
            var res = new ResultForTables();
            var scores = await _conn.QueryAsync<Scoring>("SELECT * FROM Scoring");
            var Daylies = await _conn.QueryAsync<DailyContact>("SELECT * FROM DailyContact");
            var Positives = await _conn.QueryAsync<PositiveTable>("SELECT * FROM Positive");
            res.Scorings = scores.ToList();
            res.DailyContacts = Daylies.ToList();
            res.PositiveTables = Positives.ToList();
            return res;
        }

        public async Task<bool> ClearData()
        {
            try
            {
                var query = @"DELETE FROM Positive
                          DELETE FROM DailyContact DBCC CHECKIDENT('DailyContact',RESEED, 0)
                          DELETE FROM Scoring DBCC CHECKIDENT('Scoring', RESEED, 0)";
                await _conn.ExecuteAsync(query);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<MapData>> RetreiveForMap()
        {
            var TwoWeeksAgo = DateTime.Now.AddDays(-15).Date;
            var query = @"SELECT COUNT(*) AS HowMany, Province, Latitude, Longitude
                            FROM DailyContact AS DC
                            JOIN Positive AS P ON P.DispoId = DC.DispoContactId
                            JOIN Scoring AS S ON P.DispoId = S.DispoId
                            WHERE S.Score >= 7 AND Dc.DateContact > @TwoWeeksAgo AND P.DatePositive <= DC.DateContact
                            GROUP BY DC.Province, Dc.Latitude, Dc.Longitude";
            var res = await _conn.QueryAsync<MapData>(query, new { TwoWeeksAgo });
            return res.ToList();
        }

        public async Task<List<List<GraphData>>> GetGraphData()
        {
            var minDate = await _conn.QueryFirstOrDefaultAsync<string>("SELECT TOP 1 CAST(DateContact AS nvarchar) FROM DailyContact ORDER BY DateContact");
            var oldestDate = DateTime.Parse(minDate);
            var Provinces = await _conn.QueryAsync<string>("SELECT DISTINCT Province FROM DailyContact");
            var res = new List<List<GraphData>>();
            foreach (var item in Provinces)
            {
                var tmpList = new List<GraphData>();
                var query = @"SELECT CAST(P.DatePositive AS date) AS DateEvent, COUNT (*) AS HowMany
                            FROM DailyContact AS D
                            RIGHT JOIN Positive AS P ON CAST(P.DatePositive AS DATE) = CAST(D.DateContact AS DATE)
                            WHERE Province = @item
                            GROUP BY CAST(P.DatePositive AS date)";
                var resProvince = await _conn.QueryAsync<GraphData>(query, new { item });
                var cntMax = DateTime.Now.Date - oldestDate.Date;
                var cnt = (int)cntMax.TotalDays;
                for (int i = 0; i < cnt; i++)
                {
                    var tmp = new GraphData();
                    tmp.Province = item;
                    tmp.DateEvent = DateTime.Now.AddDays(-i);
                    var howMany = resProvince.FirstOrDefault(x => x.DateEvent.Date == tmp.DateEvent.Date);
                    tmp.HowMany = howMany == null ? 0 : howMany.HowMany;
                    tmpList.Add(tmp);
                }
                res.Add(tmpList);
            }
            return res;
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
