using System.Runtime.Serialization;

namespace PASMBTCP.ModbusExceptions
{
    public class SlaveDeviceBusyException : ModbusException
    {
        public SlaveDeviceBusyException()
        {
        }

        public SlaveDeviceBusyException(string message)
            : base(message)
        {
        }

        public SlaveDeviceBusyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SlaveDeviceBusyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
