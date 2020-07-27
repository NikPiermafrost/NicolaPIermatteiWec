using NicolaPIermatteiWec.Models;
using NicolaPIermatteiWec.Models.DbModels;
using NicolaPIermatteiWec.Models.InsertModels;
using NicolaPIermatteiWec.Models.ViewModels;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Services
{
    public interface IDataAccess
    {
        Task<bool> ClearData();
        Task<ResponseModel> DailyInsertion(DailyInsert model);
        void Dispose();
        DbConnection GetConnection();
        Task<List<DistanceByProvince>> GetDistanceByProvinceTableData();
        Task<List<TopTenByScore>> GetTopTenByScoresData();
        Task<List<TopTenDevicesByContact>> GetTopTenDevicesData();
        Task<List<TypeOfRelevation>> GetTypeOfRelevationsData();
        Task<ResponseModel> PositiveInsertion(PositiveInsert model);
        Task<ResultForTables> ResultForTables();
        Task<List<MapData>> RetreiveForMap();
    }
}