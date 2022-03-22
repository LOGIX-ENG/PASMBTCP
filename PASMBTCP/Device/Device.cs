using PASMBTCP.Device;
using PASMBTCP.Events;
using PASMBTCP.SQLite;
using PASMBTCP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASMBTCP.Device
{

    public static class Device
    {
        private static readonly Client _client = new();
        private static readonly ClientDatabase _clientDatabase = new();
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
            ClientDatabase.RaiseSQLiteExceptionEvent += ModbusDatabase_RaiseSQLiteExceptionEvent;

            await _clientDatabase.InsertSingleAsync(_client);
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
            Task.Run(() => _clientDatabase.InsertSingleErrorAsync(_errorTag));
            return;
        }
    }
}
