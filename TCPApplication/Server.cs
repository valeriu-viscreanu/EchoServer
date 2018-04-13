using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpDrivers;

namespace Logic
{
    public class Server : IEchoApp
    {
        private readonly TCPManager tcpManager = new TCPManager();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private const int PortNumber = 55001; //TODO move 
        private readonly TcpListener tcpListener = new TcpListener(GetServerIpAddress(), PortNumber);
        private List<TcpClient> tcpClients = new List<TcpClient>();

        public void Start(RunSettings settings)
        {

            this.tcpListener.Start();
            // scheduele this on the thread pool not to block excuting this
            Task.Run(async () =>
            {
                try
                {
                    MessageHandler?.Invoke($"Starting on {this.tcpListener.LocalEndpoint}"); 
                    // wait for a client connection
                    while (!this.cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var client = await tcpListener.AcceptTcpClientAsync();
                        tcpClients.Add(client);
                        MessageHandler?.Invoke($"A client  {client.Client.RemoteEndPoint} is connected");
                        // listen for messages from this client  using an awaitable(Task) for better scalability
                        Task.Run(async () =>
                        {
                            var disconnectionMessage = $"Client {client.Client.RemoteEndPoint} disconnected";
                            try
                            {
                                await tcpManager.EchoMessageAsync(client, tcpClients, MessageHandler);
                                if (!client.Connected)
                                {
                                    MessageHandler?.Invoke(disconnectionMessage);
                                }
                            }
                            catch
                            {
                                MessageHandler?.Invoke(disconnectionMessage);
                                throw;
                            }
                        });
                    }
                }
                catch
                {
                    // we reached this point probably means something went wrong or we are not reading anymore (half-open connection)
                    Stop();
                    throw;
                }
            }, cancellationTokenSource.Token);


        }

        private static IPAddress GetServerIpAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            IPAddress ipAddress = null;
            foreach (var address in ipHostInfo.AddressList)
            {
                if (address.AddressFamily ==
                    AddressFamily.InterNetwork)
                {
                    ipAddress = address;
                    break;
                }
            }

            if (ipAddress == null)
            {
                throw new Exception("No IPv4 address for server");
            }

            return ipAddress;
        }

        public bool SendMessage(string message)
        {
            bool isSentSuccessfully;
            try
            {
                var buffer = Encoding.ASCII.GetBytes(message);
                tcpClients.ForEach(client => client.GetStream().WriteAsync(buffer, 0, buffer.Length));
                isSentSuccessfully = true;
            }
            catch
            {
                isSentSuccessfully = false;
            }

            return isSentSuccessfully;
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            tcpListener.Stop();
        }

        public Action<string> MessageHandler { get; set; }
    }
}