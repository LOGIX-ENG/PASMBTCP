using System.Text;

namespace PASMBTCP.Utility
{
    internal class DatabaseUtility
    {
        // SQLite Connection String
        public static string path = Environment.CurrentDirectory;
        public static string SqliteConnectionString = $@"Data Source = {path}\AppDataDB.db;";

        /// <summary>
        /// Builds The SQLite Command For Table Creation
        /// </summary>
        /// <returns>String</returns>
        public static string ModbusTagTableCreator(string client)
        {
            StringBuilder createTagTable = new();
            try
            {
                createTagTable.Append($@"CREATE TABLE IF NOT EXISTS {client}_Tag(");
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
        /// Builds The SQLite Command For Table Creation
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
        public static string InsertTagIntoTable(string client)
        {
            StringBuilder insertTag = new();
            try
            {
                insertTag.Append($@"INSERT INTO {client}_Tag");
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
        /// Builds The SQLite Command For Inserting Tag Into The Tag Table
        /// </summary>
        /// <returns>String</returns>
        public static string InsertClientIntoTable()
        {
            StringBuilder insertTag = new();
            try
            {
                insertTag.Append($@"INSERT INTO Client");
                insertTag.Append(@"(Name, IPAddress, Port, ConnectTimeout, ReadWriteTimeout) ");
                insertTag.Append(@"VALUES");
                insertTag.Append(@"(@Name, @IPAddress, @Port, @ConnectTimeout, @ReadWriteTimeout);");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return insertTag.ToString();
        }

        /// <summary>
        /// Builds The SQLite Comand For Inserting Erors Into The Error Table
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string InsertErrorIntoTable()
        {
            StringBuilder insertTag = new();
            try
            {
                insertTag.Append(@"INSERT INTO ERROR");
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
        public static string UpdateTagTable(string client)
        {
            StringBuilder updateTag = new();
            try
            {
                updateTag.Append($@"UPDATE {client}_Tag ");
                updateTag.Append(@"SET ");
                updateTag.Append(@"Value = @Value ");
                updateTag.Append(@"WHERE Name = @Name; ");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return updateTag.ToString();
        }

        /// <summary>
        /// Delete Single Tag
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DeleteTagFromTable(string input)
        {
            StringBuilder deleteTag = new();
            deleteTag.Append($@"DELETE FROM {input}_Tag");
            deleteTag.Append(@"WHERE ");
            deleteTag.Append(@"Name = @Name");
            return deleteTag.ToString();
        }

        /// <summary>
        /// Delete Single Client
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DeleteClientFromTable(string input)
        {
            StringBuilder deleteClient = new();
            deleteClient.Append($@"DELETE FROM {input}");
            deleteClient.Append(@"WHERE ");
            deleteClient.Append(@"Name = @Name");
            return deleteClient.ToString();
        }
    }
}
