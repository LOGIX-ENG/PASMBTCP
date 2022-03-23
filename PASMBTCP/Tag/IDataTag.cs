namespace PASMBTCP.Tag
{
    public interface IDataTag
    {
        string? DataType { get; set; }
        byte FunctionCode { get; set; }
        byte[] ModbusRequest { get; set; }
        byte[] ModbusResponse { get; set; }
        string? Name { get; set; }
        string? Value { get; set; }
        string? ClientName { get; set; }
    }
}