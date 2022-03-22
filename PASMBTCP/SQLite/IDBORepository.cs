using Microsoft.Data.Sqlite;
using System.Reflection;

namespace PASMBTCP.SQLite
{
    public interface IDBORepository<T> where T : class
    {
        /// <summary>
        /// Delete Row
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        Task DeleteAsync(T Entity);

        /// <summary>
        /// Get All Rows
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync(string input);

        /// <summary>
        /// Get Props
        /// </summary>
        /// <returns></returns>
        IEnumerable<PropertyInfo> GetProperties();

        /// <summary>
        /// Insert Multiple Rows
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        Task InsertMultipleAsync(List<T> Entity);

        /// <summary>
        /// Insert Single Row
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        Task InsertSingleAsync(T Entity);

        /// <summary>
        /// SQLite Connection
        /// </summary>
        /// <returns></returns>
        SqliteConnection SqlConnection();

        /// <summary>
        /// Update Multiple Rows
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        Task UpdateMultipleAsync(List<T> Entity);

        /// <summary>
        /// Update Single Row
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        Task UpdateSingleAsync(T Entity);

    }
}
