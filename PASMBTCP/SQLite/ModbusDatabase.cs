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
    /// <summary>
    /// Modbus Database Class From Database Repository
    /// </summary>
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
        public override async Task DeleteRowAsync(DataTag Entity)
        {
            using (IDbConnection connection = SqlConnection())
            {
                string command = GenerateDeleteQuery();
                //string sqlDeleteCmd = $@"DELETE FROM ModbusTag WHERE NAME = @Name";
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
        }

        /// <summary>
        /// Gets All Rows In The Database
        /// </summary>
        /// <returns>IEnumerable of DataTag</returns>
        public override async Task<IEnumerable<DataTag>> GetAllAsync()
        {
            using (IDbConnection connection = SqlConnection())
            {
                try
                {
                    string sqlQuery = $@"SELECT * FROM MODBUSTAG"; //@"SELECT Name, DataType, ModbusRequest FROM MODBUSTAG";
                    return await connection.QueryAsync<DataTag>(sqlQuery, new DynamicParameters());
                }
                catch (SqliteException ex)
                {

                    _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                    RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
                    return Enumerable.Empty<DataTag>();
                }
            }
        }

        /// <summary>
        /// Inserts Multiple Items Into The Database
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns>Task</returns>
        public override async Task InsertMultipleAsync(List<DataTag> Entity)
        {
            using (SqliteConnection connection = SqlConnection())
            {
                await connection.OpenAsync();

                IDbTransaction transaction = await connection.BeginTransactionAsync();

                string command = GenerateInsertQuery();

                try
                {
                    foreach (DataTag data in Entity)
                    {
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
        }

        /// <summary>
        /// Instert Single Item Into The Database
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns>Task</returns>
        public override async Task InsertSingleAsync(DataTag Entity)
        {
            using (IDbConnection connection = SqlConnection())
            {
                try
                {

                    string command = DatabaseUtility.InsertTagIntoDB();
                    await connection.ExecuteAsync(command, Entity);
                }
                catch (SqliteException ex)
                {

                    if (ex.Message.Contains($"no such table: MODBUSTAG"))
                    {
                        _ = await connection.ExecuteAsync(DatabaseUtility.ModbusTagTableCreator(), Entity);
                    }

                    _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                    RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
                }
            }
        }

        /// <summary>
        /// Inserts General Errors Into The Database Error Table
        /// </summary>
        /// <param name="errorTag"></param>
        /// <returns></returns>
        public async Task InsertSingleErrorAsync(ErrorTag errorTag)
        {
            using (IDbConnection connection = SqlConnection())
            {
                try
                {

                    string command = DatabaseUtility.InsertErrorIntoDB();
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

        /// <summary>
        /// Inserts General Errors Into The Database Error Table
        /// </summary>
        /// <param name="errorTag"></param>
        /// <returns></returns>
        public void InsertSingleErrorSync(ErrorTag errorTag)
        {
            using (IDbConnection connection = SqlConnection())
            {
                try
                {

                    string command = DatabaseUtility.InsertErrorIntoDB();
                    Task.Run(() => connection.ExecuteAsync(command, errorTag));
                }
                catch (SqliteException ex)
                {

                    if (ex.Message.Contains($"no such table: ERROR"))
                    {
                        _ = Task.Run(() => connection.ExecuteAsync(DatabaseUtility.ModbusErrorTableCreator(), errorTag));
                    }
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
            using (SqliteConnection connection = SqlConnection())
            {

                try
                {
                    await connection.OpenAsync();

                    IDbTransaction transaction = await connection.BeginTransactionAsync();

                    string command = GenerateUpdateQuery();


                    foreach (DataTag data in Entity)
                    {
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
        }

        /// <summary>
        /// Update Single Row In The Database
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public override async Task UpdateSingleAsync(DataTag Entity)
        {
            using (IDbConnection connection = SqlConnection())
            {
                try
                {

                    string command = GenerateUpdateQuery();
                    await connection.ExecuteAsync(command, Entity);
                }
                catch (SqliteException ex)
                {
                    _databaseEventArgs = new(GetDateTime(), new SqliteException(ex.Message, ex.ErrorCode).ToString());
                    RaiseSQLiteExceptionEvent?.Invoke(this, _databaseEventArgs);
                }
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

        /// <summary>
        /// Gets The Property Info Of The Class And Puts It Into A List Of String
        /// </summary>
        /// <param name="listOfProperties"></param>
        /// <returns>List of String of Properties</returns>
        private List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            try
            {
                return (from prop in listOfProperties select prop.Name).ToList();
            }
            catch (Exception ex)
            {
                _generalEventArgs = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                RaiseGeneralExceptionEvent?.Invoke(this, _generalEventArgs);
                throw;
            }
        }

        /// <summary>
        /// Generates Insert Query With The Property Values
        /// </summary>
        /// <returns>Sql Command String</returns>
        private string GenerateInsertQuery()
        {
            StringBuilder insertQuery = new($"INSERT INTO MODBUSTAG ");
            List<string> properties = GenerateListOfProperties(GetProperties());

            try
            {
                insertQuery.Append('(');

                properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });

                insertQuery.Remove(insertQuery.Length - 1, 1);
                insertQuery.Append(") VALUES (");

                properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });

                insertQuery.Remove(insertQuery.Length - 1, 1);
                insertQuery.Append(')');
            }
            catch (Exception ex)
            {
                _generalEventArgs = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                RaiseGeneralExceptionEvent?.Invoke(this, _generalEventArgs);
            }

            return insertQuery.ToString();
        }

        /// <summary>
        /// Generates Update Query With The Property Values
        /// </summary>
        /// <returns>Sql Command String</returns>
        private string GenerateUpdateQuery()
        {

            StringBuilder updateQuery = new($"UPDATE MODBUSTAG SET ");
            List<string> properties = GenerateListOfProperties(GetProperties());

            try
            {
                properties.ForEach(prop =>
                {
                    if (!prop.Equals("Name"))
                    {
                        updateQuery.Append($"{prop} = @{prop},");
                    }
                });
            }
            catch (Exception ex)
            {
                _generalEventArgs = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                RaiseGeneralExceptionEvent?.Invoke(this, _generalEventArgs);
            }

            updateQuery.Remove(updateQuery.Length - 1, 1);
            updateQuery.Append(" WHERE Name = @Name");

            return updateQuery.ToString();
        }

        /// <summary>
        /// Generates Delete Query With The Property Values
        /// </summary>
        /// <returns>Sql Command String</returns>
        private string GenerateDeleteQuery()
        {
            StringBuilder deleteQuery = new StringBuilder($"DELETE FROM MODBUSTAG ");
            List<string> properties = GenerateListOfProperties(GetProperties());

            try
            {
                properties.ForEach(prop =>
                {

                    if (prop.Equals("Name"))
                    {
                        deleteQuery.Append($"{prop} = @{prop},");
                    }
                });

                deleteQuery.Remove(deleteQuery.Length - 1, 1);
                deleteQuery.Append(" WHERE Name = @Name");
            }
            catch (Exception ex)
            {
                _generalEventArgs = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                RaiseGeneralExceptionEvent?.Invoke(this, _generalEventArgs);
            }

            return deleteQuery.ToString();
        }

    }
}
