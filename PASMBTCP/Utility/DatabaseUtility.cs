using System.Text;

namespace PASMBTCP.Utility
{
    internal class DatabaseUtility
    {
        // SQLite Connection String
        public static string path = Environment.CurrentDirectory;
        public static string SqliteConnectionString = $@"Data Source = {path}\AppDataDB.db;";

        /// <summary>
        /// Builds The SQLite Command For Modbus Tag Table Creation
        /// </summary>
        /// <returns>String</returns>
        public static string ModbusTagTableCreator(string clientName)
        {
            StringBuilder createTagTable = new();
            try
            {
                createTagTable.Append($@"CREATE TABLE IF NOT EXISTS {clientName}_Tag(");
                createTagTable.Append("Id INTEGER NOT NULL UNIQUE, ");
                createTagTable.Append("Name VARCHAR NOT NULL UNIQUE, ");
                createTagTable.Append("DataType VARCHAR(10) NOT NULL, ");
                createTagTable.Append("Value VARCHAR, ");
                createTagTable.Append("ModbusRequest BLOB,");
                createTagTable.Append("ClientName VARCHAR NOT NULL, ");
                createTagTable.Append("PRIMARY KEY(Id AUTOINCREMENT));");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return createTagTable.ToString();
        }

        /// <summary>
        /// Builds The SQLite Command For The Client Table Creation
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ModbusClientTableCreator()
        {
            StringBuilder createClientTable = new();
            try
            {
                createClientTable.Append($@"CREATE TABLE IF NOT EXISTS Client(");
                createClientTable.Append("Id INTEGER NOT NULL UNIQUE, ");
                createClientTable.Append("Name VARCHAR NOT NULL UNIQUE, ");
                createClientTable.Append("IPAddress VARCHAR NOT NULL, ");
                createClientTable.Append("Port INT NOT NULL, ");
                createClientTable.Append("ConnectTimeout INT NOT NULL, ");
                createClientTable.Append("ReadWriteTimeout INT NOT NULL, ");
                createClientTable.Append("PRIMARY KEY(Id AUTOINCREMENT));");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return createClientTable.ToString();
        }

        /// <summary>
        /// Builds The SQLite Command For The Error Table Creation
        /// </summary>
        /// <returns>String</returns>
        public static string ModbusErrorTableCreator()
        {
            StringBuilder createTagTable = new();
            try
            {
                createTagTable.Append(@"CREATE TABLE IF NOT EXISTS Error(");
                createTagTable.Append("Id INTEGER NOT NULL UNIQUE, ");
                createTagTable.Append("TimeOfException VARCHAR, ");
                createTagTable.Append("ExceptionMessage VARCHAR, ");
                createTagTable.Append("PRIMARY KEY(Id AUTOINCREMENT));");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return createTagTable.ToString();
        }

        /// <summary>
        /// Builds The SQLite Command For Inserting Tag Into The Tag Table
        /// </summary>
        /// <returns>String</returns>
        public static string InsertTagIntoTable(string clientName)
        {
            StringBuilder insertTag = new();
            try
            {
                insertTag.Append($@"INSERT INTO {clientName}_Tag");
                insertTag.Append(@"(Name, DataType, Value, ModbusRequest, ClientName) ");
                insertTag.Append(@"VALUES");
                insertTag.Append(@"(@Name, @DataType, @Value, @ModbusRequest, @ClientName);");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return insertTag.ToString();
        }

        /// <summary>
        /// Builds The SQLite Command For Inserting A Client Into The Client Table
        /// </summary>
        /// <returns>String</returns>
        public static string InsertClientIntoTable()
        {
            StringBuilder insertClient = new();
            try
            {
                insertClient.Append($@"INSERT INTO Client");
                insertClient.Append(@"(Name, IPAddress, Port, ConnectTimeout, ReadWriteTimeout) ");
                insertClient.Append(@"VALUES");
                insertClient.Append(@"(@Name, @IPAddress, @Port, @ConnectTimeout, @ReadWriteTimeout);");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return insertClient.ToString();
        }

        /// <summary>
        /// Builds The SQLite Comand For Inserting Errors Into The Error Table
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string InsertErrorIntoTable()
        {
            StringBuilder insertTag = new();
            try
            {
                insertTag.Append(@"INSERT INTO Error");
                insertTag.Append(@"(TimeOfException, ExceptionMessage) ");
                insertTag.Append(@"VALUES");
                insertTag.Append(@"(@TimeOfException, @ExceptionMessage);");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return insertTag.ToString();
        }

        /// <summary>
        /// Update Single Entry
        /// </summary>
        /// <returns>String</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string UpdateTagTable(string clientName)
        {
            StringBuilder updateTag = new();
            try
            {
                updateTag.Append($@"UPDATE {clientName}_Tag ");
                updateTag.Append(@"SET ");
                updateTag.Append(@"Value = @Value ");
                updateTag.Append(@"WHERE Name = @Name;");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return updateTag.ToString();
        }

        /// <summary>
        /// Update Single Entry
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string UpdateClientTable()
        {
            StringBuilder updateClient = new();
            try
            {
                updateClient.Append($@"UPDATE Device");
                updateClient.Append(@"SET ");
                updateClient.Append(@"Name = @Name ");
                updateClient.Append(@"IPAddress = @IPAddress ");
                updateClient.Append(@"Port = @Port ");
                updateClient.Append(@"ConnectTimeout = @ConnectTimeout ");
                updateClient.Append(@"ReadWriteTimeout = @ReadWriteTimeout ");
                updateClient.Append(@"WHERE Name = @Name;");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return updateClient.ToString();
        }

        /// <summary>
        /// Delete Single Tag
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DeleteTagFromTable(string clientName, string tagName)
        {
            StringBuilder deleteTag = new();
            deleteTag.Append($@"DELETE FROM {clientName}_Tag");
            deleteTag.Append(@"WHERE ");
            deleteTag.Append($@"{tagName} = @Name;");
            return deleteTag.ToString();
        }

        /// <summary>
        /// Delete Single Client
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DeleteClientFromTable(string clientName)
        {
            StringBuilder deleteClient = new();
            deleteClient.Append(@"DELETE FROM Client");
            deleteClient.Append(@"WHERE ");
            deleteClient.Append($@"{clientName} = @Name;");
            return deleteClient.ToString();
        }

        /// <summary>
        /// Delete All Tags For A Specific Client
        /// </summary>
        /// <returns></returns>
        public static string DeleteAllTags(string clientName)
        {
            StringBuilder deleteAllTags = new();
            deleteAllTags.Append($@"DELETE FROM {clientName}_Tag;");
            return deleteAllTags.ToString();
        }

        /// <summary>
        /// Delete All Clients
        /// </summary>
        /// <returns></returns>
        public static string DeleteAllClients()
        {
            StringBuilder deleteAllClients = new();
            deleteAllClients.Append(@"DELETE FROM Device;");
            return deleteAllClients.ToString();
        }

        /// <summary>
        /// Get All Tables In Database
        /// </summary>
        /// <returns></returns>
        public static string GetAllTables()
        {
            StringBuilder allTables = new();
            allTables.Append(@"SELECT name FROM sqlite_master");
            allTables.Append(@"WHERE type='table'");
            allTables.Append(@"ORDER BY name;");
            return allTables.ToString();
        }
    }
}
