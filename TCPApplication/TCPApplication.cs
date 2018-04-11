using System;
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
        private readonly TCPUtils tcpUtils = new TCPUtils();


        public void Start(RunSettings settings)
        {
            // in case of non existing or invalid IP we throw for now...TODO change
            var ipEndPointString = settings.ServerList.First(); 
            var ipEndpoint = Utils.CreateIPEndPoint(ipEndPointString);

            try
            {
                tcpClient.Connect(ipEndpoint.Address, ipEndpoint.Port);
                // run this on the thread pool not to block The UI
                Task.Run(async () => { await tcpUtils.ReceiveDataAsync(tcpClient, message => { Console.WriteLine($"Received {message}"); }); }, cancellationTokenSource.Token);
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
            cancellationTokenSource.Cancel();
            tcpClient.Close();
        }
    }



    public class Server : IEchoApp
    {
        private readonly TCPUtils tcpUtils = new TCPUtils();
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private const int portNumber = 55001; //TODO move 
        private readonly TcpListener tcpListener = new TcpListener(IPAddress.Parse("172.31.15.199"),portNumber);

        public void Start(RunSettings settings)
        {
           
            tcpListener.Start();
            Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine($"Starting ip {tcpListener.LocalEndpoint}"); // TODO add event to remove Console dependency
                    // wait for a client connection
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var client = await tcpListener.AcceptTcpClientAsync();
                        // TODO add  var message = await tcpUtils.ReceiveDataAsync(client); Change implentation of method
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

        private IPAddress GetServerIpAddress()
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
            // TODO check for clients .... 
            var isSentSuccessfully = false;
            // no connection means nobody to send the message to
            if (!tcpListener.Server.Connected)
            {
                return isSentSuccessfully;
            }
            var buffer = Encoding.ASCII.GetBytes(message);

            
            return isSentSuccessfully;
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            tcpListener.Stop();
        }
    }
}

