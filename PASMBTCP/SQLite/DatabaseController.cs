using PASMBTCP.Device;
using PASMBTCP.Events;
using PASMBTCP.Tag;
using PASMBTCP.Utility;

namespace PASMBTCP.SQLite
{
    public static class DatabaseController
    {
        private static readonly ClientTable _clientTable = new();
        private static readonly Client _client = new();
        private static readonly TagTable _tagTable = new();
        private static readonly ErrorTag _errorTag = new();
        private static readonly DataTag _dataTag = new();

        /// <summary>
        /// Gets All Table Names In The Database
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> GetAllTables()
        {
            return await _clientTable.GetAllTables();
        }

        /// <summary>
        /// Removes the Client Table From The Database
        /// </summary>
        /// <returns></returns>
        public static async Task DropTable(string tableName)
        {
            await _clientTable.DropTable(tableName);
        }

        /// <summary>
        /// Modbus Database Exception Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ClientDatabase_RaiseSQLiteExceptionEvent(object? sender, DatabaseExceptionEventArgs e)
        {
            _errorTag.TimeOfException = e.DateTime;
            _errorTag.ExceptionMessage = e.Exception;
            Task.Run(() => _clientTable.InsertSingleErrorAsync(_errorTag));
            return;
        }

        /// <summary>
        /// Modbus Database Exception Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ModbusDatabase_RaiseSQLiteExceptionEvent(object? sender, DatabaseExceptionEventArgs e)
        {
            _errorTag.TimeOfException = e.DateTime;
            _errorTag.ExceptionMessage = e.Exception;
            Task.Run(() => _tagTable.InsertSingleErrorAsync(_errorTag));
            return;
        }
    }
}
