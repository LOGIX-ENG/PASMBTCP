namespace PASMBTCP.Events
{
    public class GeneralExceptionEventArgs : EventArgs
    {
        public GeneralExceptionEventArgs()
        {
        }

        public GeneralExceptionEventArgs(string dateTime, string exception)
        {
            DateTime = dateTime; // ?? throw new ArgumentNullException(dateTime);
            Exception = exception; // ?? throw new ArgumentNullException(exception);
        }

        public string? DateTime { get; set; } = null;
        public string? Exception { get; set; } = null;
    }
}
