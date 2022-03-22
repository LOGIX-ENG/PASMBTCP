namespace PASMBTCP.Events
{
    public class ModbusExceptionsEventArgs : EventArgs
    {
        public ModbusExceptionsEventArgs()
        {
        }

        public ModbusExceptionsEventArgs(string dateTime, string exception)
        {
            DateTime = dateTime;
            Exception = exception;
        }

        public string? DateTime { get; set; } = null;
        public string? Exception { get; set; } = null;
    }
}
