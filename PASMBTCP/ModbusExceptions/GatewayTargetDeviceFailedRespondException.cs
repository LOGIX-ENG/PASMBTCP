using System.Runtime.Serialization;

namespace PASMBTCP.ModbusExceptions
{
    public class GatewayTargetDeviceFailedRespondException : ModbusException
    {
        public GatewayTargetDeviceFailedRespondException()
        {
        }

        public GatewayTargetDeviceFailedRespondException(string message)
            : base(message)
        {
        }

        public GatewayTargetDeviceFailedRespondException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected GatewayTargetDeviceFailedRespondException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
