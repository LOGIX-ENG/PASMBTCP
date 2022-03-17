using System.Runtime.Serialization;

namespace PASMBTCP.ModbusExceptions
{
    public class AcknowledgeException : ModbusException
    {
        public AcknowledgeException()
        {
        }

        public AcknowledgeException(string message)
            : base(message)
        {
        }

        public AcknowledgeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AcknowledgeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
