using System.Runtime.Serialization;

namespace PASMBTCP.ModbusExceptions
{
    public class IllegalFunctionException : ModbusException
    {
        public IllegalFunctionException()
        {
        }

        public IllegalFunctionException(string message)
            : base(message)
        {
        }

        public IllegalFunctionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected IllegalFunctionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
