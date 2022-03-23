using PASMBTCP.Events;
using PASMBTCP.SQLite;
using PASMBTCP.Utility;

namespace PASMBTCP.Device
{

    public static class ClientController
    {
        private static readonly Client _client = new();
        private static readonly ClientTable _clientTable = new();
        private static readonly ErrorTag _errorTag = new();

        /// <summary>
        /// Creates and Stores the Appropriate information for the Server
        /// you wish to connect to for data.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="connectionTimeout"></param>
        /// <param name="readWriteTimeout"></param>
        /// <returns></returns>
        public static async Task CreateClient(string Name, string ipAddress, int port, int connectionTimeout, int readWriteTimeout)
        {
            _client.Name = Name;
            _client.IPAddress = ipAddress;
            _client.Port = port;
            _client.ConnectTimeout = connectionTimeout;
            _client.ReadWriteTimeout = readWriteTimeout;

            // Raise Database Exception
            ClientTable.RaiseSQLiteExceptionEvent += ClientDatabase_RaiseSQLiteExceptionEvent;

            await _clientTable.InsertSingleAsync(_client);
        }

        /// <summary>
        /// Delete A Single Client From The Table
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public static async Task DeleteSingleClientAsync(string clientName)
        {
            // Raise Database Exception
            ClientTable.RaiseSQLiteExceptionEvent += ClientDatabase_RaiseSQLiteExceptionEvent;
            await _clientTable.DeleteSingleAsync(clientName);
        }

        /// <summary>
        /// Delete All Clients From The Table
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllClientsAsync()
        {
            // Raise Database Exception
            ClientTable.RaiseSQLiteExceptionEvent += ClientDatabase_RaiseSQLiteExceptionEvent;
            await _clientTable.DeleteAllAsync();
        }

        /// <summary>
        /// Update Client In The Table
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="connectionTimeout"></param>
        /// <param name="readWriteTimeout"></param>
        /// <returns></returns>
        public static async Task UpdateClientAsync(string Name, string ipAddress, int port, int connectionTimeout, int readWriteTimeout)
        {
            _client.Name = Name;
            _client.IPAddress = ipAddress;
            _client.Port = port;
            _client.ConnectTimeout = connectionTimeout;
            _client.ReadWriteTimeout = readWriteTimeout;

            // Raise Database Exception
            ClientTable.RaiseSQLiteExceptionEvent += ClientDatabase_RaiseSQLiteExceptionEvent;
            await _clientTable.UpdateSingleAsync(_client);
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
    }
}
