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
    public class Server : IEchoApp, IServer
    {
        private readonly ITcpServerManager tcpServerManager;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private const int PortNumber = 55001;
        private readonly TcpListener tcpListener = new TcpListener(GetServerIpAddress(), PortNumber);
        private readonly List<TcpClient> tcpClients = new List<TcpClient>();

        public Server(ITcpServerManager tcpServerManager)
        {
            this.tcpServerManager = tcpServerManager;
        }
        public void Start(RunSettings settings)
        {

            tcpListener.Start();

            // scheduele this on the thread pool not to block excuting this
            Task.Run(async () =>
            {
                try
                {
                    MessageHandler?.Invoke($"Starting on {tcpListener.LocalEndpoint}");
                    // wait for a client connection
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var client = await tcpListener.AcceptTcpClientAsync();
                        StartListeningFrom(client);
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

        private void StartListeningFrom(TcpClient client)
        {
            tcpClients.Add(client);
            MessageHandler?.Invoke($"A client  {client.Client.RemoteEndPoint} is connected");
            // listen for messages from this client  using an awaitable(Task) for better scalability
            Task.Run(async () =>
            {
                var disconnectionMessage = $"Client {client.Client.RemoteEndPoint} disconnected";
                try
                {
                    await tcpServerManager.EchoMessageAsync(client, tcpClients, MessageHandler);
                    HandleDisconnection(client, disconnectionMessage);
                }
                catch
                {
                    HandleDisconnection(client, disconnectionMessage);
                    throw;

                }
            });
        }

        private void HandleDisconnection(TcpClient client, string disconnectionMessage)
        {
            MessageHandler?.Invoke(disconnectionMessage);
            this.tcpClients.Remove(client);
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
                tcpServerManager.SendMessage(tcpClients, MessageHandler, message);
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