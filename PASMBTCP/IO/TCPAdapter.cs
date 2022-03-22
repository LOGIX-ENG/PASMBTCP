using PASMBTCP.Device;
using PASMBTCP.Events;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace PASMBTCP.IO
{
    public class TCPAdapter : IDisposable
    {

        /// <summary>
        /// Private Variables
        /// </summary>
        private static Socket? _socket = null;
        private static ProtocolType _protocolType;
        private static SocketType _socketType;
        private static IPAddress? _systemIPAddress = null;
        private static IPEndPoint? _ipEndPoint = null;
        private bool _disposedValue;
        public static event EventHandler<GeneralExceptionEventArgs>? RaiseGeneralExceptionEvent;
        private static GeneralExceptionEventArgs? _generalEventArgs;
        private static readonly byte[]? _internalBuffer = new byte[14] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// Constructor
        /// </summary>
        public TCPAdapter()
        {
        }

        /// <summary>
        /// Properties
        /// </summary>
        public static string IpAddress { get; set; } = "127.0.0.1"; // Default Value.
        public static int Port { get; set; } = 502; // Default Value.
        public static int ConnectTimeout { get; set; } = 60000; // Default Value.
        public static int ReadWriteTimeout { get; set; } = 60000; // Default Value.
        public static Client? Client { get; set; }

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
            _socketType = SocketType.Stream;
            _protocolType = ProtocolType.Tcp;

            try
            {
                // 
                _systemIPAddress = IPAddress.Parse(Client.IPAddress);
                _ipEndPoint = new IPEndPoint(_systemIPAddress, Client.Port);
                _socket = new Socket(_ipEndPoint.AddressFamily, _socketType, _protocolType);

                using CancellationTokenSource cts = new(Client.ConnectTimeout);
                using (cts.Token.Register(() => _socket.Close()))
                {
                    await SocketTaskExtensions.ConnectAsync(_socket, _ipEndPoint, cts.Token);

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
                    using CancellationTokenSource cts = new(Client.ReadWriteTimeout);
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
            if (_socket != null)
            {
                try
                {
                    using CancellationTokenSource cts = new(Client.ReadWriteTimeout);
                    using (cts.Token.Register(() => _socket.Close()))
                    {
                        Memory<byte> buffer = new(_internalBuffer);
                        int len = await SocketTaskExtensions.ReceiveAsync(_socket, buffer, SocketFlags.None, cts.Token);
                        return buffer[..len].ToArray();
                    }
                }
                catch (Exception ex)
                {
                    _generalEventArgs = new(GetDateTime(), new Exception(ex.Message, ex.InnerException).ToString());
                    RaiseGeneralExceptionEvent?.Invoke(this, _generalEventArgs);
                    return _internalBuffer;
                }
            }
            else { return _internalBuffer; }
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
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _socket?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
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
