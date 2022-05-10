using PASMBTCP.Device;
using PASMBTCP.Events;
using PASMBTCP.IO;
using PASMBTCP.SQLite;
using PASMBTCP.Tag;
using PASMBTCP.Utility;

namespace PASMBTCP.Polling
{
    public static class PollingEngine
    {
        /// <summary>
        /// Priavte Variables
        /// </summary>
        private static readonly TagTable _mbDatabase = new();
        private static readonly ClientTable _clientDatabase = new();
        private static readonly DataValidation<DataTag> _validation = new();
        private static readonly Converter<DataTag> _convert = new();
        private static readonly TCPAdapter _tcpAdapter = new();
        private static readonly ErrorTag _errorTag = new();


        /// <summary>
        /// Poll All Clients
        /// </summary>
        /// <returns></returns>
        public static async Task PollAllDevicesAsync()
        {
            // Raise The Exeption If Errors Occur
            TagTable.RaiseGeneralExceptionEvent += ModbusDatabase_RaiseGeneralExceptionEvent;
            TagTable.RaiseSQLiteExceptionEvent += ModbusDatabase_RaiseSQLiteExceptionEvent;
            ClientTable.RaiseGeneralExceptionEvent += ClientDatabase_RaiseGeneralExceptionEvent;
            ClientTable.RaiseSQLiteExceptionEvent += ClientDatabase_RaiseSQLiteExceptionEvent;

            // Get All Clients
            IEnumerable<Client> clients = await _clientDatabase.GetAllAsync("All");
            
            // Raise The Exception If Connection Fails
            TCPAdapter.RaiseGeneralExceptionEvent += SocketAdapter_RaiseGeneralExceptionEvent;

            // List of Polled Tags
            List<DataTag> value = new();

            // Iterate Over Each Client
            foreach (Client client in clients)
            {
                // Initialize The Client
                TCPAdapter.Client = client;

                // Await All Tags For Specific Client
                IEnumerable<DataTag> dataTags = await _mbDatabase.GetAllAsync(client.Name);

                // Await Connection To Device
                await _tcpAdapter.ConnectAsync();
                
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
        /// Polls All Tags In A Single Device
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static async Task PollSingleDeviceAsync(string deviceName)
        {
            // Raise The Exeption If Errors Occur
            TagTable.RaiseGeneralExceptionEvent += ModbusDatabase_RaiseGeneralExceptionEvent;
            TagTable.RaiseSQLiteExceptionEvent += ModbusDatabase_RaiseSQLiteExceptionEvent;
            ClientTable.RaiseGeneralExceptionEvent += ClientDatabase_RaiseGeneralExceptionEvent;
            ClientTable.RaiseSQLiteExceptionEvent += ClientDatabase_RaiseSQLiteExceptionEvent;

            // Get All Clients
            IEnumerable<Client> client = await _clientDatabase.GetSingleAsync(deviceName);

            // Raise The Exception If Connection Fails
            TCPAdapter.RaiseGeneralExceptionEvent += SocketAdapter_RaiseGeneralExceptionEvent;

            // List of Polled Tags
            List<DataTag> value = new();

            // Initialize The Client
            TCPAdapter.Client = client.ElementAt(0);

            // Await All Tags For Specific Client
            IEnumerable<DataTag> dataTags = await _mbDatabase.GetAllAsync(client.ElementAt(0).Name);

            // Await Connection To Device
            await _tcpAdapter.ConnectAsync();

            // Iterate Over Each Tag
            foreach (DataTag tag in dataTags)
            {
                DataTag tagsWithData = await RequestAsync<DataTag>(tag);
                value.Add(tagsWithData);
            }
            
            // On Complete Update All Tags In Database
            await _mbDatabase.UpdateMultipleAsync(value);

            // Disconnect and Dispose Of The Socket Resource
            TCPAdapter.Disconnect();
            _tcpAdapter.Dispose();
        }

        /// <summary>
        /// Polls One Tag In A Specific Device
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static async Task PollSingleTagAsync(string deviceName, string tagName)
        {
            // Raise The Exeption If Errors Occur
            TagTable.RaiseGeneralExceptionEvent += ModbusDatabase_RaiseGeneralExceptionEvent;
            TagTable.RaiseSQLiteExceptionEvent += ModbusDatabase_RaiseSQLiteExceptionEvent;
            ClientTable.RaiseGeneralExceptionEvent += ClientDatabase_RaiseGeneralExceptionEvent;
            ClientTable.RaiseSQLiteExceptionEvent += ClientDatabase_RaiseSQLiteExceptionEvent;

            // Get All Clients
            IEnumerable<Client> client = await _clientDatabase.GetSingleAsync(deviceName);

            // Raise The Exception If Connection Fails
            TCPAdapter.RaiseGeneralExceptionEvent += SocketAdapter_RaiseGeneralExceptionEvent;


            // Initialize The Client
            TCPAdapter.Client = client.ElementAt(0);

            // Await All Tags For Specific Client
            IEnumerable<DataTag> dataTag = await _mbDatabase.GetSingleAsync(deviceName, tagName);

            // Await Connection To Device
            await _tcpAdapter.ConnectAsync();

            DataTag tag = await RequestAsync<DataTag>(dataTag.ElementAt(0));

            // On Complete Update All Tags In Database
            await _mbDatabase.UpdateSingleAsync(tag);

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
