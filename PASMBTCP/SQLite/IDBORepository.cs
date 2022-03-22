using Microsoft.Data.Sqlite;
using System.Reflection;

namespace PASMBTCP.SQLite
{
    public interface IDBORepository<T> where T : class
    {

        Task DeleteAsync(T Entity);
        Task<IEnumerable<T>> GetAllAsync(string input);
        IEnumerable<PropertyInfo> GetProperties();
        Task InsertMultipleAsync(List<T> Entity);
        Task InsertSingleAsync(T Entity);
        SqliteConnection SqlConnection();
        Task UpdateMultipleAsync(List<T> Entity);
        Task UpdateSingleAsync(T Entity);


    }
}
