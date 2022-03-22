using PASMBTCP.Utility;

namespace PASMBTCP.Message
{
    public class SlaveException : ModbusMessage
    {
        /// <summary>
        /// Priavte Variables
        /// </summary>
        private static readonly Dictionary<byte, string> _exceptions = CreateExceptionLookup();
        private static byte _slaveExceptionCode;

        /// <summary>
        /// Constructor
        /// </summary>
        public SlaveException()
        {
        }

        /// <summary>
        /// Constructor : Takes In The Exception Code From Remote Device
        /// </summary>
        /// <param name="slaveExceptionCode"></param>
        public SlaveException(byte slaveExceptionCode)
        {
            _slaveExceptionCode = slaveExceptionCode;
        }

        // Exception Code 
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        public static byte ExceptionCode { get => _slaveExceptionCode; set => _slaveExceptionCode = value; }

#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

        /// <summary>
        /// Get The Exception Message From The Exception Code
        /// </summary>
        /// <returns>Key Value Pair</returns>
        private static Dictionary<byte, string> CreateExceptionLookup()
        {
            Dictionary<byte, string> valuePairs = new();
            valuePairs.Add(1, ModbusExceptionMessages.IllegalFunction);
            valuePairs.Add(2, ModbusExceptionMessages.IllegalDataAddress);
            valuePairs.Add(3, ModbusExceptionMessages.IllegalDataValue);
            valuePairs.Add(4, ModbusExceptionMessages.SlaveDeviceFailure);
            valuePairs.Add(5, ModbusExceptionMessages.Acknowledge);
            valuePairs.Add(6, ModbusExceptionMessages.SlaveDeviceBusy);
            valuePairs.Add(7, ModbusExceptionMessages.NegativeAcknowledge);
            valuePairs.Add(8, ModbusExceptionMessages.MemoryParityError);
            valuePairs.Add(10, ModbusExceptionMessages.GatewayPathUnavailable);
            valuePairs.Add(11, ModbusExceptionMessages.GatewayTargetDeviceFailedToRespond);
            return valuePairs;
        }

        /// <summary>
        /// Gets The Message From The Dictionary And Formats It To A String
        /// </summary>
        /// <returns>Exception Message</returns>
        public override string ToString()
        {
            string exceptionMessage = _exceptions.ContainsKey(_slaveExceptionCode)
                ? _exceptions[ExceptionCode]
                : ModbusExceptionMessages.Unknown;
            return exceptionMessage;
        }
    }
}
