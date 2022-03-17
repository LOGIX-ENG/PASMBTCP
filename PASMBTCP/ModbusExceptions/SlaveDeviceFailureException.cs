using System.Runtime.Serialization;

namespace PASMBTCP.ModbusExceptions
{
    public class SlaveDeviceFailureException : ModbusException
    {
        public SlaveDeviceFailureException()
        {
        }

        public SlaveDeviceFailureException(string message)
            : base(message)
        {
        }

        public SlaveDeviceFailureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SlaveDeviceFailureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
