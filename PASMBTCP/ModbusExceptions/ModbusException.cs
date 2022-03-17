using System.Runtime.Serialization;

namespace PASMBTCP.ModbusExceptions
{
    public class ModbusException : Exception
    {
        public ModbusException()
        {
        }

        public ModbusException(string message)
            : base(message)
        {
        }

        public ModbusException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ModbusException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
