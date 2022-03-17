namespace PASMBTCP.Events
{
    public class ModbusExceptionsEventArgs : EventArgs
    {
        public ModbusExceptionsEventArgs()
        {
        }

        public ModbusExceptionsEventArgs(string dateTime, string exception)
        {
            DateTime = dateTime; // ?? throw new ArgumentNullException(dateTime);
            Exception = exception; // ?? throw new ArgumentNullException(dateTime);
        }

        public string? DateTime { get; set; } = null;
        public string? Exception { get; set; } = null;
    }
}
