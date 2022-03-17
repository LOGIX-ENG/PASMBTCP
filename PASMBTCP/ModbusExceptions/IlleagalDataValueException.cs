using System.Runtime.Serialization;

namespace PASMBTCP.ModbusExceptions
{
    public class IlleagalDataValueException : ModbusException
    {
        public IlleagalDataValueException()
        {
        }

        public IlleagalDataValueException(string message)
            : base(message)
        {
        }

        public IlleagalDataValueException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected IlleagalDataValueException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
