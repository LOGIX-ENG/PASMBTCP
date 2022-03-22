using PASMBTCP.Events;
using PASMBTCP.Message;
using PASMBTCP.ModbusExceptions;
using PASMBTCP.SQLite;
using PASMBTCP.Utility;
using System.Globalization;

namespace PASMBTCP.Tag
{
    /// <summary>
    /// Data Validation Class For Modbus Messages
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataValidation<T> where T : DataTag
    {
        /// <summary>
        /// Private Variables
        /// </summary>
        private static DataTag dataTag = Activator.CreateInstance<T>();
        private static SlaveException? _slaveException;
        private static ModbusExceptionsEventArgs _args = new();
        private static ModbusDatabase _database = new();

        /// <summary>
        /// Constructor
        /// </summary>
        public DataValidation()
        {
        }

        /// <summary>
        /// Formats Date Time With Culture Info
        /// </summary>
        /// <returns>Date Time Formated String</returns>
        private static string GetDateTime()
        {
            DateTime dateTime = DateTime.Now;
            CultureInfo cultureInfo = new("en-US");
            string formatspecifier = "dd/MMMM/yyyy, hh:mm:ss tt";
            return dateTime.ToString(formatspecifier, cultureInfo);
        }

        /// <summary>
        /// Receives A DataTag And Performs A Check On The Function Code.
        /// If The Sent Function Code Equals The Response Functionc Code Then It Returns The DataTag.
        /// If The Sent Function Code Is Not Equal To The Response Function Code It Checkes The Error Code
        /// And Replies With A Zerored Out Byte[] In The Modbus.Resoponse Field Of The DataTag Object.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>DataTag</returns>
        public DataTag Validate(DataTag data)
        {
            dataTag = data;

            if ((dataTag.ModbusRequest[7] == dataTag.ModbusResponse[7] && dataTag.ModbusResponse.Length <= 13))
            {
                return dataTag;
            }

            else if (dataTag.ModbusResponse[7] == (dataTag.ModbusRequest[7] + ModbusUtility.ExceptionCodeOffset))
            {
                byte exceptionCode = dataTag.ModbusResponse[8];
                switch (exceptionCode)
                {
                    case 1:
                        _slaveException = new(1);
                        _args = new(GetDateTime(), new IllegalFunctionException(_slaveException.ToString()).ToString());
                        ErrorTag _error = new();
                        _error.TimeOfException = _args.DateTime;
                        _error.ExceptionMessage = _args.Exception;
                        Task.Run(() => _database.InsertSingleErrorAsync(_error));
                        return OnException(dataTag);
                    case 2:
                        _slaveException = new(2);
                        _args = new(GetDateTime(), new IllegalDataAddressException(_slaveException.ToString()).ToString());
                        ErrorTag _error2 = new();
                        _error2.TimeOfException = _args.DateTime;
                        _error2.ExceptionMessage = _args.Exception;
                        Task.Run(() => _database.InsertSingleErrorAsync(_error2));
                        return OnException(dataTag);
                    case 3:
                        _slaveException = new(3);
                        _args = new(GetDateTime(), new IlleagalDataValueException(_slaveException.ToString()).ToString());
                        ErrorTag _error3 = new();
                        _error3.TimeOfException = _args.DateTime;
                        _error3.ExceptionMessage = _args.Exception;
                        Task.Run(() => _database.InsertSingleErrorAsync(_error3));
                        return OnException(dataTag);

                    case 4:
                        _slaveException = new(4);
                        _args = new(GetDateTime(), new SlaveDeviceFailureException(_slaveException.ToString()).ToString());
                        ErrorTag _error4 = new();
                        _error4.TimeOfException = _args.DateTime;
                        _error4.ExceptionMessage = _args.Exception;
                        Task.Run(() => _database.InsertSingleErrorAsync(_error4));
                        return OnException(dataTag);

                    case 5:
                        _slaveException = new(5);
                        _args = new(GetDateTime(), new AcknowledgeException(_slaveException.ToString()).ToString());
                        ErrorTag _error5 = new();
                        _error5.TimeOfException = _args.DateTime;
                        _error5.ExceptionMessage = _args.Exception;
                        Task.Run(() => _database.InsertSingleErrorAsync(_error5));
                        return OnException(dataTag);

                    case 6:
                        _slaveException = new(6);
                        _args = new(GetDateTime(), new SlaveDeviceBusyException(_slaveException.ToString()).ToString());
                        ErrorTag _error6 = new();
                        _error6.TimeOfException = _args.DateTime;
                        _error6.ExceptionMessage = _args.Exception;
                        Task.Run(() => _database.InsertSingleErrorAsync(_error6));
                        return OnException(dataTag);

                    case 7:
                        _slaveException = new(7);
                        _args = new(GetDateTime(), new NegativeAcknowledgeException(_slaveException.ToString()).ToString());
                        ErrorTag _error7 = new();
                        _error7.TimeOfException = _args.DateTime;
                        _error7.ExceptionMessage = _args.Exception;
                        Task.Run(() => _database.InsertSingleErrorAsync(_error7));
                        return OnException(dataTag);

                    case 8:
                        _slaveException = new(8);
                        _args = new(GetDateTime(), new MemoryParityErrorException(_slaveException.ToString()).ToString());
                        ErrorTag _error8 = new();
                        _error8.TimeOfException = _args.DateTime;
                        _error8.ExceptionMessage = _args.Exception;
                        Task.Run(() => _database.InsertSingleErrorAsync(_error8));
                        return OnException(dataTag);

                    case 10:
                        _slaveException = new(10);
                        _args = new(GetDateTime(), new GatewayPathUnavailableException(_slaveException.ToString()).ToString());
                        ErrorTag _error10 = new();
                        _error10.TimeOfException = _args.DateTime;
                        _error10.ExceptionMessage = _args.Exception;
                        Task.Run(() => _database.InsertSingleErrorAsync(_error10));
                        return OnException(dataTag);

                    case 11:
                        _slaveException = new(11);
                        _args = new(GetDateTime(), new GatewayTargetDeviceFailedRespondException(_slaveException.ToString()).ToString());
                        ErrorTag _error11 = new();
                        _error11.TimeOfException = _args.DateTime;
                        _error11.ExceptionMessage = _args.Exception;
                        Task.Run(() => _database.InsertSingleErrorAsync(_error11));
                        return OnException(dataTag);

                    default:
                        return dataTag;
                }

            }
            else if (dataTag.ModbusResponse.Length > 13)
            {
                return OnException(dataTag);
            }
            return dataTag;
        }

        /// <summary>
        /// Zeros Out The Modbus.Response Of The DataTag Object
        /// So The Database Is Updated With A Zero.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>DataTag</returns>
        protected virtual DataTag OnException(DataTag data)
        {
            int len = 0;

            switch (dataTag.DataType)
            {
                case "Float":
                    len = 13;
                    break;
                case "Short":
                    len = 11;
                    break;
                case "Long":
                    len = 13;
                    break;
                case "Bool":
                    len = 11;
                    break;
                default:
                    break;
            }

            dataTag.ModbusResponse = new byte[len];

            return dataTag;
        }

    }
}
