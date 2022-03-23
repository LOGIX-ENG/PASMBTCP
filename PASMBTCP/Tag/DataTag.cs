using PASMBTCP.Utility;

namespace PASMBTCP.Tag
{
    public class DataTag : IDataTag
    {
        // Proerties For Data Tags
        public string? DataType { get; set; } = null;
        public byte FunctionCode { get; set; }
        public byte[] ModbusRequest { get; set; } = Array.Empty<byte>();
        public byte[] ModbusResponse { get; set; } = Array.Empty<byte>();
        public string? Name { get; set; } = null;
        public string? Value { get; set; } = null;
        public string? ClientName { get; set; } = null;
    }
}
