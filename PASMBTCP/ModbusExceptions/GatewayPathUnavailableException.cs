using System.Runtime.Serialization;

namespace PASMBTCP.ModbusExceptions
{
    public class GatewayPathUnavailableException : ModbusException
    {
        public GatewayPathUnavailableException()
        {
        }

        public GatewayPathUnavailableException(string message)
            : base(message)
        {
        }

        public GatewayPathUnavailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected GatewayPathUnavailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
