using PASMBTCP.Device;
using PASMBTCP.Events;
using PASMBTCP.IO;
using PASMBTCP.SQLite;
using PASMBTCP.Tag;
using PASMBTCP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PASMBTCP.Polling
{
    public static class PollingEngine
    {
        /// <summary>
        /// Priavte Variables
        /// </summary>
        private static readonly ModbusDatabase _mbDatabase = new();
        private static readonly ClientDatabase _clientDatabase = new();
        private static readonly DataValidation<DataTag> _validation = new();
        private static readonly Converter<DataTag> _convert = new();
        private static readonly TCPAdapter _tcpAdapter = new();
        private static readonly ErrorTag _errorTag = new();


        /// <summary>
        /// Poll All Servers
        /// </summary>
        /// <returns></returns>
        public static async Task PollAllAsync()
        {
            // Raise The Exeption If Errors Occur
            ModbusDatabase.RaiseGeneralExceptionEvent += ModbusDatabase_RaiseGeneralExceptionEvent;
            ModbusDatabase.RaiseSQLiteExceptionEvent += ModbusDatabase_RaiseSQLiteExceptionEvent;
            ClientDatabase.RaiseGeneralExceptionEvent += ClientDatabase_RaiseGeneralExceptionEvent;
            ClientDatabase.RaiseSQLiteExceptionEvent += ClientDatabase_RaiseSQLiteExceptionEvent;

            // Get All Clients
            IEnumerable<Client> clients = await _clientDatabase.GetAllAsync("All");
            
            // Raise The Exception If Connection Fails
            TCPAdapter.RaiseGeneralExceptionEvent += SocketAdapter_RaiseGeneralExceptionEvent;

            // List of Polled Tags
            List<DataTag> value = new();

            // Iterate Over Each Client
            foreach(Client client in clients)
            {
                TCPAdapter.IpAddress = client.IPAddress;
                TCPAdapter.Port = client.Port;
                TCPAdapter.ConnectTimeout = client.ConnectTimeout;
                TCPAdapter.ReadWriteTimeout = client.ReadWriteTimeout;
                
                // Await Connection To Device
                await _tcpAdapter.ConnectAsync();
                
                IEnumerable<DataTag> dataTags = await _mbDatabase.GetAllAsync(client.Name);
                
                // Iterate Over Each Tag
                foreach(DataTag tag in dataTags)
                {
                    DataTag tagsWithData = await RequestAsync<DataTag>(tag);
                    value.Add(tagsWithData);
                }
                // On Complete Update All Tags In Database
                await _mbDatabase.UpdateMultipleAsync(value);
            }

            // Disconnect and Dispose Of The Socket Resource
            TCPAdapter.Disconnect();
            _tcpAdapter.Dispose();
        }

        /// <summary>
        /// Read, Validate, Convert Data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        private static async Task<T> RequestAsync<T>(T data)
            where T : DataTag
        {
            // Raise Exception If Write Fails
            TCPAdapter.RaiseGeneralExceptionEvent += SocketAdapter_RaiseGeneralExceptionEvent;

            // Send Data To Remote Device
            await _tcpAdapter.SendDataAsync(data.ModbusRequest);

            // Receive Data From Remote Device
            data.ModbusResponse = await _tcpAdapter.ReceiveDataAsync();

            // Validate Data From Device
            data = await Task.Run(() => (T)_validation.Validate(data));

            // Raise Conversion Of Data Exception
            Converter<DataTag>.RaiseGeneralExceptionEvent += Converter_RaiseGeneralExceptionEvent;

            // Convert Data To Specified Data Type And Return Tag
            // Switch To Read Each Tag As Its Specific Data Type
            T response = Activator.CreateInstance<T>();
            switch (data.ModbusRequest[1])
            {
                case 1:
                    response = (T)await Task.Run(() => _convert.BoolValue(data));
                    break;
                case 2:
                    response = (T)await Task.Run(() => _convert.ShortValue(data));
                    break;
                case 3:
                    response = (T)await Task.Run(() => _convert.Realvalue(data));
                    break;
                case 4:
                    response = (T)await Task.Run(() => _convert.LongValue(data));
                    break;
                default:
                    break;
            }
            return response;
        }

        /// <summary>
        /// Database Exception Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ModbusDatabase_RaiseSQLiteExceptionEvent(object? sender, DatabaseExceptionEventArgs e)
        {
            _errorTag.TimeOfException = e.DateTime;
            _errorTag.ExceptionMessage = e.Exception;
            Task.Run(() => _mbDatabase.InsertSingleErrorAsync(_errorTag));
            return;
        }

        /// <summary>
        /// Database Exception Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ModbusDatabase_RaiseGeneralExceptionEvent(object? sender, GeneralExceptionEventArgs e)
        {
            _errorTag.TimeOfException = e.DateTime;
            _errorTag.ExceptionMessage = e.Exception;
            Task.Run(() => _mbDatabase.InsertSingleErrorAsync(_errorTag));
        }

        /// <summary>
        /// Database Exception Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ClientDatabase_RaiseSQLiteExceptionEvent(object? sender, Events.DatabaseExceptionEventArgs e)
        {
            _errorTag.TimeOfException = e.DateTime;
            _errorTag.ExceptionMessage = e.Exception;
            Task.Run(() => _mbDatabase.InsertSingleErrorAsync(_errorTag));
            return;
        }

        /// <summary>
        /// Database Exception Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ClientDatabase_RaiseGeneralExceptionEvent(object? sender, Events.GeneralExceptionEventArgs e)
        {
            _errorTag.TimeOfException = e.DateTime;
            _errorTag.ExceptionMessage = e.Exception;
            Task.Run(() => _mbDatabase.InsertSingleErrorAsync(_errorTag));
            return;
        }

        /// <summary>
        /// Socket Adapter Exception Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SocketAdapter_RaiseGeneralExceptionEvent(object? sender, GeneralExceptionEventArgs e)
        {
            _tcpAdapter.Dispose();
            _errorTag.TimeOfException = e.DateTime;
            _errorTag.ExceptionMessage = e.Exception;
            Task.Run(() => _mbDatabase.InsertSingleErrorAsync(_errorTag));
        }

        /// <summary>
        /// Converter Exception Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Converter_RaiseGeneralExceptionEvent(object? sender, GeneralExceptionEventArgs e)
        {

            _errorTag.TimeOfException = e.DateTime;
            _errorTag.ExceptionMessage = e.Exception;
            Task.Run(() => _mbDatabase.InsertSingleErrorAsync(_errorTag));
        }
    }
}
