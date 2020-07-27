using NicolaPIermatteiWec.Models;
using NicolaPIermatteiWec.Models.InsertModels;
using System.Data.Common;
using System.Threading.Tasks;

namespace NicolaPIermatteiWec.Services
{
    public interface IDataAccess
    {
        Task<DailyResponseModel> DailyInsertion(DailyInsert model);
        void Dispose();
        DbConnection GetConnection();
    }
}