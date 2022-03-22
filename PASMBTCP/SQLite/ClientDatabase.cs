using Dapper;
using Microsoft.Data.Sqlite;
using PASMBTCP.Device;
using PASMBTCP.Events;
using PASMBTCP.Utility;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace PASMBTCP.SQLite
{
    public class ClientDatabase : DBOBase<Client>
    {
        /// <summary>
        /// Private Variables
        /// </summary>
        private static DatabaseExceptionEventArgs _databaseEventArgs = new();
        private static GeneralExceptionEventArgs _generalEventArgs = new();
        public static event EventHandler<DatabaseExceptionEventArgs>? RaiseSQLiteExceptionEvent;
        public static event EventHandler<GeneralExceptionEventArgs>? RaiseGeneralExceptionEvent;


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
        public override async Task DeleteSingleAsync(Client Entity)
        {
            using IDbConnection connection = SqlConnection();
            string command = DatabaseUtility.DeleteClientFromTable(Entity.Name);
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
        /// <returns>IEnumerable of Client</returns>
        public override async Task<IEnumerable<Client>> GetAllAsync(string input)
        {
            using IDbConnection connection = SqlConnection();
            try
            {
                string sqlQuery = $@"SELECT * FROM Client";
                return await connection.QueryAsync<Client>(sqlQuery, new DynamicParameters());
            }
            catch (SqliteException ex)
            {

                _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
                return Enumerable.Empty<Client>();
            }
        }

        /// <summary>
        /// Gets Single In The Database
        /// </summary>
        /// <returns>IEnumerable of Client</returns>
        public async Task<IEnumerable<Client>> GetSingleAsync(string input)
        {
            using IDbConnection connection = SqlConnection();
            try
            {
                string sqlQuery = $@"SELECT {input} FROM Client";
                return await connection.QueryAsync<Client>(sqlQuery, new DynamicParameters());
            }
            catch (SqliteException ex)
            {

                _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
                return Enumerable.Empty<Client>();
            }
        }


        /// <summary>
        /// Gets The Property Info Of The Class And Puts It Into A List Of String
        /// </summary>
        /// <param name="listOfProperties"></param>
        /// <returns>List of String of Properties</returns>
        public override IEnumerable<PropertyInfo> GetProperties()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts Multiple Items Into The Database
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns>Task</returns>
        public override Task InsertMultipleAsync(List<Client> Entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Instert Single Item Into The Database
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns>Task</returns>
        public override async Task InsertSingleAsync(Client Entity)
        {
            using IDbConnection connection = SqlConnection();
            try
            {

                string command = DatabaseUtility.InsertClientIntoTable();
                await connection.ExecuteAsync(command, Entity);
            }
            catch (SqliteException ex)
            {

                if (ex.Message.Contains($"no such table: Client"))
                {
                    _ = await connection.ExecuteAsync(DatabaseUtility.ModbusClientTableCreator(), Entity);
                    await InsertSingleAsync(Entity);
                    return;
                }

                _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
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
        public override async Task UpdateMultipleAsync(List<Client> Entity)
        {
            using SqliteConnection connection = SqlConnection();

            try
            {
                await connection.OpenAsync();

                IDbTransaction transaction = await connection.BeginTransactionAsync();


                foreach (Client data in Entity)
                {
                    string command = DatabaseUtility.UpdateTagTable(data.Name);
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
        public override async Task UpdateSingleAsync(Client Entity)
        {
            using IDbConnection connection = SqlConnection();
            try
            {

                string command = DatabaseUtility.UpdateTagTable(Entity.Name);
                await connection.ExecuteAsync(command, Entity);
            }
            catch (SqliteException ex)
            {
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

                if (ex.Message.Contains($"no such table: ERROR"))
                {
                    _ = await connection.ExecuteAsync(DatabaseUtility.ModbusErrorTableCreator(), errorTag);
                }
            }
        }
    }
}
