using System.Data;

namespace TugasAsinkron2.Interfaces
{
    public interface IDataAccess
    {
        Task<List<T>> Read<T, U>(string sql, U parameters);
        Task Write<T>(string sql, T parameters);
        Task<string> ExecuteTransaction<T>(List<KeyValuePair<string, T>> sqlDict);
        Task<string> BulkInsert(DataTable data, string tableName);
    }
}
