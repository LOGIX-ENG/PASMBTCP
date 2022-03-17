using System.Runtime.Serialization;

namespace PASMBTCP.ModbusExceptions
{
    public class NegativeAcknowledgeException : ModbusException
    {
        public NegativeAcknowledgeException()
        {
        }

        public NegativeAcknowledgeException(string message)
            : base(message)
        {
        }

        public NegativeAcknowledgeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NegativeAcknowledgeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
