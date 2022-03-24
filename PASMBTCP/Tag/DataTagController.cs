using PASMBTCP.Events;
using PASMBTCP.Message;
using PASMBTCP.SQLite;
using PASMBTCP.Utility;
using System.Text.RegularExpressions;

namespace PASMBTCP.Tag
{
    public static class DataTagController
    {
        /// <summary>
        /// Priavate Variables
        /// </summary>
        private static readonly TagTable _database = new();
        private static readonly ErrorTag _errorTag = new();
        private static readonly DataTag _dataTag = new();
        private static readonly ModbusMessage _message = new();
        private static string _pattern = String.Empty;
        private static string _splitString = String.Empty;
        private static string[]? _cleanString;


        /// <summary>
        /// Create and Insert Into Database, One Single Modbus Tag
        /// </summary>
        /// <param name="name"></param>
        /// <param name="registerAddress"></param>
        /// <param name="dataType"></param>
        /// <param name="server"></param>
        /// <returns>Awaitable Task</returns>
        public static async Task CreateTag(string name, string registerAddress, string dataType, string clientName)
        { 
            // Assign Values
            _dataTag.Name = name;
            _message.RegisterAddress = (ushort)(GetRegisterAddress(registerAddress) - 1);
            _message.UnitId = 1;
            _message.FunctionCode = GetFunctionCode(registerAddress);
            _dataTag.ClientName = clientName;

            // Switch on DataType
            switch (dataType)
            {
                case "Short":
                    _message.Quantity = 1;
                    _message.TransactionId = ModbusUtility.ShortCode;
                    break;
                case "Float":
                    _message.Quantity = 2;
                    _message.TransactionId = ModbusUtility.FloatCode;
                    break;
                case "Long":
                    _message.Quantity = 2;
                    _message.TransactionId = ModbusUtility.LongCode;
                    break;
                case "Bool":
                    _message.Quantity = 1;
                    _message.TransactionId = ModbusUtility.BoolCode;
                    break;
                default:
                    break;
            }

            // Assign Values
            _dataTag.DataType = dataType;

            // Modbus Request Is Created Only Once Per Tag.
            // It Is Created Only When Tag Is Created
            // And Inserted Into The Database.
            _dataTag.ModbusRequest = _message.Frame;

            // Raise Database Exception
            TagTable.RaiseSQLiteExceptionEvent += ModbusDatabase_RaiseSQLiteExceptionEvent;

            // Insert Tag Into Database.
            await _database.InsertSingleAsync(_dataTag);
        }

        /// <summary>
        /// Extract Specific Register Address from Input
        /// </summary>
        /// <param name="registerAddress"></param>
        /// <returns></returns>
        private static ushort GetRegisterAddress(string registerAddress)
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
        private static byte GetFunctionCode(string registerAddress)
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
                if (fCode == 0 || fCode == 1 || fCode == 3 || fCode == 4)
                {
                    // Switch On Function Code
                    switch (fCode)
                    {
                        case 0:
                            fCode = 1;
                            break;
                        case 1:
                            fCode = 2;
                            break;
                        case 3:
                            fCode = 4;
                            break;
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
        /// Delete A Single Tag From The Table
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public static async Task DeleteSingleTagAsync(string clientName, string tagName)
        {
            // Raise Database Exception
            ClientTable.RaiseSQLiteExceptionEvent += ModbusDatabase_RaiseSQLiteExceptionEvent;
            await _database.DeleteSingleAsync(clientName, tagName);
        }

        /// <summary>
        /// Delete All Tags From The Table
        /// </summary>
        /// <returns></returns>
        public static async Task DeleteAllTagsAsync(string clientName)
        {
            // Raise Database Exception
            ClientTable.RaiseSQLiteExceptionEvent += ModbusDatabase_RaiseSQLiteExceptionEvent;
            await _database.DeleteAllAsync(clientName);
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
            Task.Run(() => _database.InsertSingleErrorAsync(_errorTag));
            return;
        }
    }
}
