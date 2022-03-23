using Microsoft.Data.Sqlite;
using System.Reflection;

namespace PASMBTCP.SQLite
{
    public abstract class DBOBase<T> : IDBORepository<T> where T : class
    {
        /// <summary>
        /// Get All Rows
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public abstract Task<IEnumerable<T>> GetAllAsync(string input);

        /// <summary>
        /// Get Props
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<PropertyInfo> GetProperties();

        /// <summary>
        /// Insert Multiple Rows
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public abstract Task InsertMultipleAsync(List<T> Entity);

        /// <summary>
        /// Insert Single Row
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public abstract Task InsertSingleAsync(T Entity);

        /// <summary>
        /// SQLite Connection
        /// </summary>
        /// <returns></returns>
        public abstract SqliteConnection SqlConnection();

        /// <summary>
        /// Update Multiple Rows
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public abstract Task UpdateMultipleAsync(List<T> Entity);

        /// <summary>
        /// Update Single Row
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public abstract Task UpdateSingleAsync(T Entity);
    }

}
