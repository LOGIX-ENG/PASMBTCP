using Microsoft.Data.Sqlite;
using System.Reflection;

namespace PASMBTCP.SQLite
{
    public abstract class DBOBase<T> : IDBORepository<T> where T : class
    {
        public abstract Task DeleteAsync(T Entity);
        public abstract Task<IEnumerable<T>> GetAllAsync(string input);
        public abstract IEnumerable<PropertyInfo> GetProperties();
        public abstract Task InsertMultipleAsync(List<T> Entity);
        public abstract Task InsertSingleAsync(T Entity);
        public abstract SqliteConnection SqlConnection();
        public abstract Task UpdateMultipleAsync(List<T> Entity);
        public abstract Task UpdateSingleAsync(T Entity);
    }

}
