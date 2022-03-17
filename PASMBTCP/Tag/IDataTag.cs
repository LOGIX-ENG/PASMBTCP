namespace PASMBTCP.Tag
{
    public interface IDataTag
    {
        string? DataType { get; set; }
        byte FunctionCode { get; set; }
        byte[] ModbusRequest { get; set; }
        byte[] ModbusResponse { get; set; }
        string? Name { get; set; }
        ushort RegisterAddress { get; set; }
        byte UnitId { get; set; }
        string? Value { get; set; }
        string? TimeOfException { get; set; }
        string? ExceptionMessage { get; set; }
    }
}