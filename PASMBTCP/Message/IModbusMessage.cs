namespace PASMBTCP.Message
{
    public interface IModbusMessage
    {
        byte ExceptionCode { get; set; }
        byte[] Frame { get; }
        byte FunctionCode { get; set; }
        short LengthField { get; set; }
        byte[] MbapHeader { get; }
        byte[] ProtocolDataUnit { get; }
        short ProtocolId { get; set; }
        short Quantity { get; set; }
        ushort RegisterAddress { get; set; }
        short TransactionId { get; set; }
        byte UnitId { get; set; }
    }
}