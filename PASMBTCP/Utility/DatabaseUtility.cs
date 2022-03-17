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
        public static string ModbusTagTableCreator()
        {
            StringBuilder CreateTagTable = new();
            try
            {
                CreateTagTable.Append(@"CREATE TABLE IF NOT EXISTS MODBUSTAG(");
                CreateTagTable.Append("Id INTEGER NOT NULL UNIQUE, ");
                CreateTagTable.Append("Name VARCHAR NOT NULL UNIQUE, ");
                CreateTagTable.Append("RegisterAddress TINYINT NOT NULL UNIQUE, ");
                CreateTagTable.Append("UnitId TINYINT NOT NULL, ");
                CreateTagTable.Append("FunctionCode TINYINT NOT NULL, ");
                CreateTagTable.Append("DataType VARCHAR(10) NOT NULL, ");
                CreateTagTable.Append("Value VARCHAR, ");
                CreateTagTable.Append("ModbusRequest BLOB,");
                CreateTagTable.Append("ModbusResponse BLOB,");
                CreateTagTable.Append("TimeOfException VARCHAR,");
                CreateTagTable.Append("ExceptionMessage VARCHAR,");
                CreateTagTable.Append("PRIMARY KEY(Id AUTOINCREMENT));");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return CreateTagTable.ToString();
        }

        /// <summary>
        /// Builds The SQLite Command For Table Creation
        /// </summary>
        /// <returns>String</returns>
        public static string ModbusErrorTableCreator()
        {
            StringBuilder CreateTagTable = new();
            try
            {
                CreateTagTable.Append(@"CREATE TABLE IF NOT EXISTS ERROR(");
                CreateTagTable.Append("Id INTEGER NOT NULL UNIQUE, ");;
                CreateTagTable.Append("TimeOfException VARCHAR, ");
                CreateTagTable.Append("ExceptionMessage VARCHAR, ");
                CreateTagTable.Append("PRIMARY KEY(Id AUTOINCREMENT));");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return CreateTagTable.ToString();
        }

        /// <summary>
        /// Builds The SQLite Command For Inserting Tag Into The Tag Table
        /// </summary>
        /// <returns>String</returns>
        public static string InsertTagIntoDB()
        {
            StringBuilder InsertTag = new();
            try
            {
                InsertTag.Append(@"INSERT INTO MODBUSTAG");
                InsertTag.Append(@"(Name, RegisterAddress, UnitId, FunctionCode, DataType, Value, ModbusRequest, ModbusResponse, TimeOfException, ExceptionMessage) ");
                InsertTag.Append(@"VALUES");
                InsertTag.Append(@"(@Name, @RegisterAddress, @UnitId, @FunctionCode, @DataType, @Value, @ModbusRequest, @ModbusResponse, @TimeOfException, @ExceptionMessage);");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return InsertTag.ToString();
        }

        /// <summary>
        /// Builds The SQLite Comand For Inserting Erors Into The Error Table
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string InsertErrorIntoDB()
        {
            StringBuilder InsertTag = new();
            try
            {
                InsertTag.Append(@"INSERT INTO ERROR");
                InsertTag.Append(@"(TimeOfException, ExceptionMessage) ");
                InsertTag.Append(@"VALUES");
                InsertTag.Append(@"(@TimeOfException, @ExceptionMessage);");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return InsertTag.ToString();
        }

        /// <summary>
        /// Update Single Entry
        /// </summary>
        /// <returns>String</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string UpdateTagDatabse()
        {
            StringBuilder updateTag = new();
            try
            {
                updateTag.Append(@"UPDATE MODBUSTAG ");
                updateTag.Append(@"SET ");
                updateTag.Append(@"Value = @Value, ");
                updateTag.Append(@"ModbusResponse = @ModbusResponse, ");
                updateTag.Append(@"TimeOfException = @TimeOfException, ");
                updateTag.Append(@"ExceptionMessage = @ExceptionMessage ");
                updateTag.Append(@"WHERE RegisterAddress = @RegisterAddress; ");
            }
            catch (ArgumentOutOfRangeException ex)
            {

                throw new ArgumentOutOfRangeException(ex.Message, ex.InnerException);
            }

            return updateTag.ToString();
        }
    }
}
