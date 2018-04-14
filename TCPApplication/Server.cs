using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpDrivers;

namespace Logic
{
    /// <summary>
    /// todo extract interface and DI!
    /// 
    /// </summary>
    public class Server : IEchoApp, IServer
    {
        private readonly ITCPManager tcpManager;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private const int PortNumber = 55001; //TODO move 
        private readonly TcpListener tcpListener = new TcpListener(GetServerIpAddress(), PortNumber);
        private List<TcpClient> tcpClients = new List<TcpClient>();

        public Server(ITCPManager tcpManager)
        {
            this.tcpManager = tcpManager;
        }
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
                                if (TCPManager.GetState(client) != TcpState.Established)
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
            var hostName = Dns.GetHostName();
            var ipHostInfo = Dns.GetHostEntry(hostName);
            var ipAddress = ipHostInfo.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);
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