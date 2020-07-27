using System.Data.Common;

namespace NicolaPIermatteiWec.Services
{
    public interface IDataAccess
    {
        void Dispose();
        DbConnection GetConnection();
    }
}