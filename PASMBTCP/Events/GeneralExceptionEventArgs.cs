namespace PASMBTCP.Events
{
    public class GeneralExceptionEventArgs : EventArgs
    {
        public GeneralExceptionEventArgs()
        {
        }

        public GeneralExceptionEventArgs(string dateTime, string exception)
        {
            DateTime = dateTime;
            Exception = exception;
        }

        public string? DateTime { get; set; } = null;
        public string? Exception { get; set; } = null;
    }
}
