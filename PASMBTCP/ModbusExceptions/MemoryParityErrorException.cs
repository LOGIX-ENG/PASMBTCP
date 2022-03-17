using System.Runtime.Serialization;

namespace PASMBTCP.ModbusExceptions
{
    public class MemoryParityErrorException : ModbusException
    {
        public MemoryParityErrorException()
        {
        }

        public MemoryParityErrorException(string message)
            : base(message)
        {
        }

        public MemoryParityErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MemoryParityErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
