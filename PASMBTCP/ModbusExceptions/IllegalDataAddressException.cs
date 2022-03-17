using System.Runtime.Serialization;

namespace PASMBTCP.ModbusExceptions
{
    public class IllegalDataAddressException : ModbusException
    {
        public IllegalDataAddressException()
        {
        }

        public IllegalDataAddressException(string message)
            : base(message)
        {
        }

        public IllegalDataAddressException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected IllegalDataAddressException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
