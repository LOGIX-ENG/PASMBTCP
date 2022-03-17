namespace PASMBTCP.Events
{
    public class DatabaseExceptionEventArgs : EventArgs
    {
        public DatabaseExceptionEventArgs()
        {
        }
        public DatabaseExceptionEventArgs(string dateTime, string exception)
        {
            DateTime = dateTime; //?? throw new ArgumentNullException(dateTime);
            Exception = exception; //?? throw new ArgumentNullException(dateTime);
        }

        public string? DateTime { get; set; } = null;
        public string? Exception { get; set; } = null;
    }
}
