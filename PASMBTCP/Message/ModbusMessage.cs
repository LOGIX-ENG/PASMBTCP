using PASMBTCP.Utility;
using System.Net;

namespace PASMBTCP.Message
{
    /// <summary>
    /// Concrete Implementor Class
    /// </summary>
    public class ModbusMessage : IModbusMessage
    {
        /// <summary>
        /// Private Variables
        /// </summary>
        private static short _transactionId = GetUniqueId();
        private static int _transactionIdInternal = 0;
        private static short _protocolId = ModbusUtility.ProtocolId;
        private static short _lengthField = ModbusUtility.LengthField;
        private static int _internal = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModbusMessage()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="functionCode"></param>
        /// <param name="registerAddress"></param>
        /// <param name="quantity"></param>
        public ModbusMessage(byte unitId, byte functionCode, ushort registerAddress, short quantity)
        {
            UnitId = unitId;
            FunctionCode = functionCode;
            RegisterAddress = registerAddress;
            Quantity = quantity;
        }

        /// <summary>
        /// Properties for a Modbus Message
        /// </summary>
        public short TransactionId { get => _transactionId; set => _transactionId = value; }
        public short ProtocolId { get => _protocolId; set => _protocolId = value; }
        public byte FunctionCode { get; set; }
        public short LengthField { get => _lengthField; set => _lengthField = value; }
        public byte UnitId { get; set; }
        public ushort RegisterAddress { get; set; }
        public short Quantity { get; set; }

        // Prop for Modbus Exception Codes
        public byte ExceptionCode { get; set; }

        // Prop to create the MbapHeader section of the Modbus Message
        public byte[] MbapHeader
        {
            get
            {
                List<byte> mbap = new();
                mbap.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(_transactionId)));
                mbap.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(_protocolId)));
                mbap.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(_lengthField)));
                mbap.Add(UnitId);
                return mbap.ToArray();
            }
        }

        // Prop to create the PDU section of the Modbus Message
        public byte[] ProtocolDataUnit
        {
            get
            {
                _internal = IPAddress.NetworkToHostOrder(RegisterAddress);
                byte[] bytes = BitConverter.GetBytes((ushort)IPAddress.NetworkToHostOrder(_internal));
                List<byte> pdu = new();
                pdu.Add(FunctionCode);
                pdu.Add(bytes[1]);
                pdu.Add(bytes[0]);
                pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Quantity)));
                return pdu.ToArray();
            }
        }

        // Prop to assemble the Modbus Message Frame
        public byte[] Frame
        {
            get
            {
                List<byte> adu = new();
                adu.AddRange(MbapHeader);
                adu.AddRange(ProtocolDataUnit);
                return adu.ToArray();
            }
        }

        /// <summary>
        /// Get a unique Transatcion ID
        /// </summary>
        /// <returns>Transaciton ID</returns>
        public static short GetUniqueId()
        {
            int transactionIncremented = Interlocked.Increment(ref _transactionIdInternal);
            _transactionId = Convert.ToInt16(transactionIncremented);
            return _transactionId;
        }
    }
}
