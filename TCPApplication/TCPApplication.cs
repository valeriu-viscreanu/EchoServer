using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPApplication
{
    public class Client : IEchoApp
    {
        private readonly TcpClient tcpClient = new TcpClient();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly TCPManager tcpManager = new TCPManager();


        public void Start(RunSettings settings)
        {
            // in case of non existing or invalid IP we throw for now...TODO change
            var ipEndPointString = settings.ServerList.First(); 
            var ipEndpoint = Utils.CreateIPEndPoint(ipEndPointString);

            try
            {
                tcpClient.Connect(ipEndpoint.Address, ipEndpoint.Port);
                Console.WriteLine($"Client connected on {tcpClient.Client.LocalEndPoint}");
                // run this on the thread pool not to block The UI
                Task.Run(async () => { await this.tcpManager.ReceiveMessageAsync(tcpClient, async message => { Console.WriteLine($"Received {message}"); }); }, cancellationTokenSource.Token);
            }
            catch
            {
                // we reached this point probably means something went wrong or we are not reading anymore (half-open connection)
                Stop();
                throw;
            }

        }

        public bool SendMessage(string messageToSend)
        {
            var isSentSuccessfully = false;
            if (tcpClient.Client == null)
            {
                return isSentSuccessfully;
            }
            var buffer = Encoding.ASCII.GetBytes(messageToSend);
            if (tcpClient.Connected)
            {
                tcpClient.GetStream().Write(buffer, 0, buffer.Length);
                isSentSuccessfully = true;
            }
            return isSentSuccessfully;
        }

        public void Stop()
        {
            Console.WriteLine("Stopping client");
            cancellationTokenSource.Cancel();
            tcpClient.Close();
        }
    }



    public class Server : IEchoApp
    {
        private readonly TCPManager tcpManager = new TCPManager();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private const int portNumber = 55001; //TODO move 
        private readonly TcpListener tcpListener = new TcpListener(GetServerIpAddress(), portNumber);
        private List<TcpClient> tcpClients = new List<TcpClient>();

        public void Start(RunSettings settings)
        {

            tcpListener.Start();
            // scheduele this on the thread pool not to block excuting this
            Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine($"Starting on {tcpListener.LocalEndpoint}"); // TODO add event to remove Console dependency
                    // wait for a client connection
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var client = await tcpListener.AcceptTcpClientAsync();
                        tcpClients.Add(client);
                        Console.WriteLine($"A client  {client.Client.RemoteEndPoint} is connected");
                        // listen for messages from this client  using an awaitable(Task) for better scalability
                        Task.Run(async () =>
                        {
                            var disconnectionMessage = $"Client {client.Client.RemoteEndPoint} disconnected";
                            try
                            {
                                await tcpManager.EchoMessageAsync(client, this.tcpClients);
                                if (!client.Connected)
                                {
                                    Console.WriteLine(disconnectionMessage);
                                }
                            }
                            catch
                            {
                                Console.WriteLine(disconnectionMessage);
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
    }
}

