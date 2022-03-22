namespace PASMBTCP.Message
{
    public interface IModbusMessage
    {
        byte ExceptionCode { get; set; }
        byte[] Frame { get; }
        byte FunctionCode { get; set; }
        byte[] MbapHeader { get; }
        byte[] ProtocolDataUnit { get; }
        short Quantity { get; set; }
        ushort RegisterAddress { get; set; }
        short TransactionId { get; set; }
        byte UnitId { get; set; }
    }
}