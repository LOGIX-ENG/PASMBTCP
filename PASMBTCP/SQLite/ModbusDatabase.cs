using Dapper;
using Microsoft.Data.Sqlite;
using PASMBTCP.Events;
using PASMBTCP.Tag;
using PASMBTCP.Utility;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
namespace PASMBTCP.SQLite
{
    public class ModbusDatabase : DBOBase<DataTag>
    {
        /// <summary>
        /// Private Variables
        /// </summary>
        private static DatabaseExceptionEventArgs _databaseEventArgs = new();
        private static GeneralExceptionEventArgs _generalEventArgs = new();
        public static event EventHandler<DatabaseExceptionEventArgs>? RaiseSQLiteExceptionEvent;
        public static event EventHandler<GeneralExceptionEventArgs>? RaiseGeneralExceptionEvent;


        /// <summary>
        /// Constructor
        /// </summary>
        public ModbusDatabase()
        {
        }


        /// <summary>
        /// Formats Date Time With Culture Info
        /// </summary>
        /// <returns>Date Time Formated String</returns>
        private static string GetDateTime()
        {
            DateTime dateTime = DateTime.Now;
            CultureInfo cultureInfo = new("en-US");
            string formatspecifier = "dd/MMMM/yyyy, hh:mm:ss tt";
            return dateTime.ToString(formatspecifier, cultureInfo);
        }

        /// <summary>
        /// Delete Row From Database
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public override async Task DeleteAsync(DataTag Entity)
        {
            using IDbConnection connection = SqlConnection();
            string command = DatabaseUtility.DeleteTagFromTable(Entity.Name);
            try
            {
                await connection.ExecuteAsync(command, Entity);
            }
            catch (SqliteException ex)
            {
                _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
            }
        }

        /// <summary>
        /// Gets All Rows In The Database
        /// </summary>
        /// <returns>IEnumerable of DataTag</returns>
        public override async Task<IEnumerable<DataTag>> GetAllAsync(string input)
        {
            using IDbConnection connection = SqlConnection();
            try
            {
                string sqlQuery = $@"SELECT * FROM {input}_Tag";
                // IEnumerable<Client> result = await connection.QueryAsync<Client>(sqlQuery, new DynamicParameters());
                return await connection.QueryAsync<DataTag>(sqlQuery, new DynamicParameters()); // result.AsParallel<Client>();
            }
            catch (SqliteException ex)
            {

                _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
                return Enumerable.Empty<DataTag>();
            }
        }

        /// <summary>
        /// Gets Single In The Database
        /// </summary>
        /// <returns>IEnumerable of Client</returns>
        public async Task<IEnumerable<DataTag>> GetSingleAsync(string deviceName, string tagName)
        {
            using IDbConnection connection = SqlConnection();
            try
            {
                string sqlQuery = $@"SELECT {tagName} FROM {deviceName}_Tag";
                return await connection.QueryAsync<DataTag>(sqlQuery, new DynamicParameters());
            }
            catch (SqliteException ex)
            {

                _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
                return Enumerable.Empty<DataTag>();
            }
        }

        /// <summary>
        /// Inserts Multiple Items Into The Database
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns>Task</returns>
        public override async Task InsertMultipleAsync(List<DataTag> Entity)
        {
            using SqliteConnection connection = SqlConnection();
            await connection.OpenAsync();

            IDbTransaction transaction = await connection.BeginTransactionAsync();

            //string command = DatabaseUtility.InsertTagIntoTable();

            try
            {
                foreach (DataTag data in Entity)
                {
                    string command = DatabaseUtility.InsertTagIntoTable(data.ClientName);
                    await connection.ExecuteAsync(command, data);
                }
            }
            catch (SqliteException ex)
            {
                _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                transaction.Rollback();
                await connection.CloseAsync();
                await connection.DisposeAsync();
                RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
            }

            transaction.Commit();
            await connection.CloseAsync();
            await connection.DisposeAsync();
        }

        /// <summary>
        /// Instert Single Item Into The Database
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns>Task</returns>
        public override async Task InsertSingleAsync(DataTag Entity)
        {
            using IDbConnection connection = SqlConnection();
            try
            {

                string command = DatabaseUtility.InsertTagIntoTable(Entity.ClientName);
                await connection.ExecuteAsync(command, Entity);
            }
            catch (SqliteException ex)
            {

                if (ex.Message.Contains($"no such table: {Entity.ClientName}_Tag"))
                {
                    _ = await connection.ExecuteAsync(DatabaseUtility.ModbusTagTableCreator(Entity.ClientName), Entity);
                    await InsertSingleAsync(Entity);
                    return;
                }

                _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
            }
        }

        /// <summary>
        /// Inserts General Errors Into The Database Error Table
        /// </summary>
        /// <param name="errorTag"></param>
        /// <returns></returns>
        public async Task InsertSingleErrorAsync(ErrorTag errorTag)
        {
            using IDbConnection connection = SqlConnection();
            try
            {

                string command = DatabaseUtility.InsertErrorIntoTable();
                await connection.ExecuteAsync(command, errorTag);
            }
            catch (SqliteException ex)
            {

                if (ex.Message.Contains($"no such table: Error"))
                {
                    _ = await connection.ExecuteAsync(DatabaseUtility.ModbusErrorTableCreator(), errorTag);
                }
            }
        }


        /// <summary>
        /// Generates The Connection String
        /// </summary>
        /// <returns>SqliteConnection</returns>
        public override SqliteConnection SqlConnection()
        {
            SqliteConnection conn = new(DatabaseUtility.SqliteConnectionString);
            return conn;
        }

        /// <summary>
        /// Update Multiple Rows In The Database
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public override async Task UpdateMultipleAsync(List<DataTag> Entity)
        {
            using SqliteConnection connection = SqlConnection();

            try
            {
                await connection.OpenAsync();

                IDbTransaction transaction = await connection.BeginTransactionAsync();


                foreach (DataTag data in Entity)
                {
                    string command = DatabaseUtility.UpdateTagTable(data.ClientName);
                    await connection.ExecuteAsync(command, data);
                }
                try
                {
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    _generalEventArgs = new(GetDateTime(), new Exception(e.Message, e.InnerException).ToString());
                    RaiseGeneralExceptionEvent?.Invoke(this, _generalEventArgs);
                }
                await connection.CloseAsync();
            }
            catch (SqliteException ex)
            {
                _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
            }
            catch (Exception e)
            {
                _generalEventArgs = new(GetDateTime(), new Exception(e.Message, e.InnerException).ToString());
                RaiseGeneralExceptionEvent?.Invoke(this, _generalEventArgs);
            }
        }

        /// <summary>
        /// Update Single Row In The Database
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public override async Task UpdateSingleAsync(DataTag Entity)
        {
            using IDbConnection connection = SqlConnection();
            try
            {

                string command = DatabaseUtility.UpdateTagTable(Entity.ClientName);
                await connection.ExecuteAsync(command, Entity);
            }
            catch (SqliteException ex)
            {
                _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
            }
        }

        /// <summary>
        /// Gets The Property Information From The Class
        /// </summary>
        /// <returns>IEnumberable of PropertyInfo</returns>
        public override IEnumerable<PropertyInfo> GetProperties()
        {
            return typeof(DataTag).GetProperties();
        }

    }
}
