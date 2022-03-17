using PASMBTCP.Events;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace PASMBTCP.IO
{
    public class SocketAdapter : IDisposable
    {

        /// <summary>
        /// Private Variables
        /// </summary>
        private static Socket? _socket = null;
        private static ProtocolType protocolType;
        private static SocketType socketType;
        private static IPAddress? systemIPAddress = null;
        private static IPEndPoint? ipEndPoint = null;
        private bool disposedValue;
        public static event EventHandler<GeneralExceptionEventArgs>? RaiseGeneralExceptionEvent;
        private static GeneralExceptionEventArgs? _generalEventArgs;

        /// <summary>
        /// Constructor
        /// </summary>
        public SocketAdapter()
        {
        }

        /// <summary>
        /// Properties
        /// </summary>
        public static string IpAddress { get; set; } = "127.0.0.1"; // Default Value.
        public static int Port { get; set; }
        public static int ConnectTImeout { get; set; }
        public static int ReadWriteTimeout { get; set; }

        /// <summary>
        /// Formats Date Time With Culture Info
        /// </summary>
        /// <returns>Date Time Formated String</returns>
        private static string GetDateTime()
        {
            DateTime dateTime = DateTime.Now;
            CultureInfo cultureInfo = new("en-US");
            string formatspecifier = "dd/MMMM/yyyy, hh:mm:ss tt";
            return dateTime.ToString(formatspecifier, cultureInfo);
        }

        /// <summary>
        /// Connect To Remote Client Async
        /// </summary>
        /// <returns>Value Task</returns>
        public async ValueTask ConnectAsync()
        {
            // Set Socket And Protocol Type
            socketType = SocketType.Stream;
            protocolType = ProtocolType.Tcp;

            try
            {
                // 
                systemIPAddress = IPAddress.Parse(IpAddress);
                ipEndPoint = new IPEndPoint(systemIPAddress, Port);
                _socket = new Socket(ipEndPoint.AddressFamily, socketType, protocolType);

                using CancellationTokenSource cts = new(ConnectTImeout);
                using (cts.Token.Register(() => _socket.Close()))
                {
                    await SocketTaskExtensions.ConnectAsync(_socket, ipEndPoint, cts.Token);

                }
            }
            catch (Exception ex)
            {
                _generalEventArgs = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                RaiseGeneralExceptionEvent?.Invoke(this, _generalEventArgs);
                //throw;
            }
        }

        /// <summary>
        /// Sends Data Request To Remote Host
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public async ValueTask SendDataAsync(ReadOnlyMemory<byte> buffer)
        {

            if (_socket != null)
            {
                try
                {
                    using CancellationTokenSource cts = new(ReadWriteTimeout);
                    using (cts.Token.Register(() => _socket.Close()))
                    {
                        await SocketTaskExtensions.SendAsync(_socket, buffer, SocketFlags.None, cts.Token);
                    }

                }
                catch (Exception ex)
                {
                    _generalEventArgs = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                    RaiseGeneralExceptionEvent?.Invoke(this, _generalEventArgs);
                }
            }
            else { return; }
        }

        /// <summary>
        /// Receives Data From Remote Host
        /// </summary>
        /// <returns>ValueTask byte[]</returns>
        public async ValueTask<byte[]> ReceiveDataAsync()
        {
            byte[] dummyBuffer = { 0,0,0,0,0,0,0,0,0,0,0,0 };
            if (_socket != null)
            {
                try
                {
                    using CancellationTokenSource cts = new(ReadWriteTimeout);
                    using (cts.Token.Register(() => _socket.Close()))
                    {
                        byte[] internalBuffer = new byte[1024];
                        Memory<byte> buffer = new(internalBuffer);
                        int len = await SocketTaskExtensions.ReceiveAsync(_socket, buffer, SocketFlags.None, cts.Token);
                        return buffer.Slice(0, len).ToArray();
                    }
                }
                catch (Exception ex)
                {
                    _generalEventArgs = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                    RaiseGeneralExceptionEvent?.Invoke(this, _generalEventArgs);
                    return dummyBuffer;
                }
            }
            else { return dummyBuffer; }
        }

        /// <summary>
        /// Disconnect From Remote Host
        /// </summary>
        public static void Disconnect()
        {
            _socket?.Disconnect(false);
            _socket?.Close();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _socket?.Dispose();
                    Console.WriteLine("Socket Disposed");
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SocketAdapter()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
