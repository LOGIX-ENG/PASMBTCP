using PASMBTCP.Events;
using PASMBTCP.IO;
using PASMBTCP.Message;
using PASMBTCP.SQLite;
using PASMBTCP.Tag;
using PASMBTCP.Utility;
using System.Text.RegularExpressions;

namespace PASMBTCP.Device
{
    public static class ClientDevice
    {
        /// <summary>
        /// Priavte Variables
        /// </summary>
        private static readonly ModbusDatabase database = new();
        private static readonly DataValidation<DataTag> _validation = new();
        private static readonly Converter<DataTag> convert = new();
        private static readonly SocketAdapter socket = new();
        private static readonly ErrorTag _errorTag = new();
        private static string? _pattern;
        private static string? _splitString;
        private static string[]? _cleanString;



        /// <summary>
        /// Properties
        /// </summary>
        public static int Port { get => SocketAdapter.Port; set => SocketAdapter.Port = value; }
        public static string IpAddress { get => SocketAdapter.IpAddress; set => SocketAdapter.IpAddress = value; }
        public static int SocketTimeout { get => SocketAdapter.ConnectTImeout; set => SocketAdapter.ConnectTImeout = value; }
        public static int ReadWriteTimeout { get => SocketAdapter.ReadWriteTimeout; set => SocketAdapter.ReadWriteTimeout = value; }

        /// <summary>
        /// Poll Command
        /// Retreives Data Tags From The Database
        /// </summary>
        /// <returns>Awaitable Task</returns>
        public static async Task PollAsync()
        {
            // Raise The Exeption If Errors Occur
            ModbusDatabase.RaiseGeneralExceptionEvent += ModbusDatabase_RaiseGeneralExceptionEvent;
            ModbusDatabase.RaiseSQLiteExceptionEvent += ModbusDatabase_RaiseSQLiteExceptionEvent;

            // Get All Datatags From Database
            IEnumerable<DataTag> tagsFromDB = await database.GetAllAsync();

            // Raise The Exception If Connection Fails
            SocketAdapter.RaiseGeneralExceptionEvent += SocketAdapter_RaiseGeneralExceptionEvent;

            // Await Connection To Device
            await socket.ConnectAsync();

            List<DataTag> value = new();

            // Iterate Over Each Datatag
            foreach (DataTag tag in tagsFromDB)
            {
                DataTag tagsWithData = await RequestAsync<DataTag>(tag);

                value.Add(tagsWithData);
            }

            // On Complete Update All Tags In Database
            await database.UpdateMultipleAsync(value);

            // Disconnect and Dispose Of The Socket Resource
            SocketAdapter.Disconnect();
            socket.Dispose();

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
            SocketAdapter.RaiseGeneralExceptionEvent += SocketAdapter_RaiseGeneralExceptionEvent;

            // Send Data To Remote Device
            await socket.SendDataAsync(data.ModbusRequest);

            // Receive Data From Remote Device
            data.ModbusResponse = await socket.ReceiveDataAsync();

            // Validate Data From Device
            data = await Task.Run(() => (T)_validation.Validate(data));

            // Raise Conversion Of Data Exception
            Converter<DataTag>.RaiseGeneralExceptionEvent += Converter_RaiseGeneralExceptionEvent;

            // Convert Data To Specified Data Type And Return Tag
            // Switch To Read Each Tag As Its Specific Data Type
            T response = Activator.CreateInstance<T>();
            switch (data.DataType)
            {
                case "Short":
                    response = (T)await Task.Run(() => convert.ShortValue(data));
                    break;
                case "Float":
                    response = (T)await Task.Run(() => convert.Realvalue(data));
                    break;
                case "Long":
                    response = (T)await Task.Run(() => convert.LongValue(data));
                    break;
                case "Bool":
                    response = (T)await Task.Run(() => convert.BoolValue(data));
                    break;
                default:
                    break;
            }
            return response;
        }

        /// <summary>
        /// Create and Insert Into Database, One Single Modbus Tag
        /// </summary>
        /// <param name="name"></param>
        /// <param name="registerAddress"></param>
        /// <param name="unitId"></param>
        /// <param name="functionCode"></param>
        /// <param name="dataType"></param>
        /// <returns>Awaitable Task</returns>
        public static async Task InsertTagIntoDB(string name, string registerAddress, string dataType)
        {
            // Initialize DataTag and ModbusMessage
            DataTag dataTag = new();
            ModbusMessage message = new();

            // Assign Values
            dataTag.Name = name;
            message.RegisterAddress = dataTag.RegisterAddress = (ushort)(GetRegisterAddress(registerAddress) - 1);
            message.UnitId = dataTag.UnitId = 1;
            message.FunctionCode = dataTag.FunctionCode = GetFunctionCode(registerAddress);

            // Switch on DataType
            switch (dataType)
            {
                case "Short":
                    message.Quantity = 1;
                    break;
                case "Float":
                    message.Quantity = 2;
                    break;
                case "Long":
                    message.Quantity = 2;
                    break;
                case "Bool":
                    message.Quantity = 1;
                    break;
                default:
                    break;
            }

            // Assign Values
            dataTag.DataType = dataType;
            dataTag.ExceptionMessage = String.Empty;

            // Modbus Request Is Created Only Once Per Tag.
            // It Is Created Only When Tag Is Created
            // And Inserted Into The Database.
            dataTag.ModbusRequest = message.Frame;

            // Raise Database Exception
            ModbusDatabase.RaiseSQLiteExceptionEvent += ModbusDatabase_RaiseSQLiteExceptionEvent;

            // Insert Tag Into Database.
            await database.InsertSingleAsync(dataTag);
        }

        /// <summary>
        /// Extract Specific Register Address from Input
        /// </summary>
        /// <param name="registerAddress"></param>
        /// <returns></returns>
        public static ushort GetRegisterAddress(string registerAddress)
        {
            // Pattern For Removing Unwated Strings
            _pattern = @"[a-z]+";

            // Split The Unwatnted Strings Using The Pattern.
            _cleanString = Regex.Split(registerAddress, _pattern, RegexOptions.IgnoreCase);

            // Combine the Array Into One String.
            registerAddress = string.Concat(_cleanString);

            // Split The String.
            // Removing Index 0 From The String.
            _splitString = registerAddress.Substring(1, registerAddress.Length - 1);

            //Parse The String Into A USHORT
            bool x = ushort.TryParse(_splitString, out ushort regAdd);

            //If The TyrParse Succeds Return The Parsed Value.
            if (x)
            {
                return regAdd;
            }

            // Else, Return Safe Value
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Extract and Convert Function Code
        /// </summary>
        /// <param name="registerAddress"></param>
        /// <returns></returns>
        public static byte GetFunctionCode(string registerAddress)
        {
            // Pattern For Removing Unwated Chars
            _pattern = @"[a-z]+";

            // Split The Unwatnted Strings Using The Pattern.
            _cleanString = Regex.Split(registerAddress, _pattern, RegexOptions.IgnoreCase);

            // Combine the Array Into One String.
            registerAddress = string.Concat(_cleanString);

            // Split The String.
            // Removing Index 0 From The String.
            _splitString = registerAddress.Substring(0, 1);

            // Parse The String Into A USHORT
            bool x = byte.TryParse(_splitString, out byte fCode);

            // If The TyrParse Succeds Return The Parsed Value.
            if (x)
            {
                // If Function Code Is 0, 1, 3, 4 Return Function Code.
                if(fCode == 0 || fCode == 1 || fCode == 3 || fCode == 4)
                {
                    // Switch On Function Code
                    switch (fCode)
                    {
                        // Read Coils
                        case 0:
                            fCode = 1;
                            break;
                        // Read Discrete Inputs
                        case 1:
                            fCode = 2;
                            break;
                        // Read Input Registers
                        case 3:
                            fCode = 4;
                            break;
                        // Read Holding Registers
                        case 4:
                            fCode = 3;
                            break;
                        default:
                            break;
                    }
                    // Return Function Code.
                    return fCode;
                }
                // Else, Return Safe Function Code.
                else { return 1; }
            }
            // Else, Return Safe Function Code.
            else { return 1; }
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
            Task.Run(() => database.InsertSingleErrorAsync(_errorTag));
            return;
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
            Task.Run(() => database.InsertSingleErrorAsync(_errorTag));
        }

        /// <summary>
        /// Socket Adapter Exception Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SocketAdapter_RaiseGeneralExceptionEvent(object? sender, GeneralExceptionEventArgs e)
        {
            socket.Dispose();
            _errorTag.TimeOfException = e.DateTime;
            _errorTag.ExceptionMessage = e.Exception;
            Task.Run(() => database.InsertSingleErrorAsync(_errorTag));
        }

        /// <summary>
        /// Database Exception Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ModbusDatabase_RaiseGeneralExceptionEvent(object? sender, GeneralExceptionEventArgs e)
        {

            if (e.Exception == "no such table: MODBUSTAG")
            {
                return;
            }
            else
            {
                _errorTag.TimeOfException = e.DateTime;
                _errorTag.ExceptionMessage = e.Exception;
                Task.Run(() => database.InsertSingleErrorAsync(_errorTag));
            }
        }
    }

}
