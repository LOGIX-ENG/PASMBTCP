namespace PASMBTCP.Events
{
    public class DatabaseExceptionEventArgs : EventArgs
    {
        public DatabaseExceptionEventArgs()
        {
        }
        public DatabaseExceptionEventArgs(string dateTime, string exception)
        {
            DateTime = dateTime;
            Exception = exception;
        }

        public string? DateTime { get; set; } = null;
        public string? Exception { get; set; } = null;
    }
}
