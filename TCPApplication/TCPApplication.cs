using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPApplication
{
    public class Client : IEchoApp
    {
        private  readonly TcpClient tcpClient = new TcpClient();
        private  readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public void Start(RunSettings settings)
        {
            var ipEndPointString = settings.ServerList.First(); // in case of non existing or invalid IP we throw for now...
            var ipEndpoint = Utils.CreateIPEndPoint(ipEndPointString);

            tcpClient.Connect(ipEndpoint.Address, ipEndpoint.Port);
            
            // run this on the thread pool not to block The UI
            Task.Run(async () =>
            {
                var networkStream = tcpClient.GetStream();
                var bytes = new byte[4096];

                int bytesReceived;
                while ((bytesReceived = await networkStream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    var data = Encoding.ASCII.GetString(bytes, 0, bytesReceived);
                    Console.WriteLine($"Received: {data}"); // TODO add event to remove Console dependency
                }

                // we reached this point probably means we are not reading anymore (half-open connection)
                Stop();
            }, cancellationTokenSource.Token);

        }

        public bool SendMessage(string messageToSend)
        {
            var sentSuccessfully = false;
            var buffer = Encoding.ASCII.GetBytes(messageToSend);
            if (tcpClient.Connected)
            {
                tcpClient.GetStream().Write(buffer, 0, buffer.Length);
                sentSuccessfully = true;
            }
            return sentSuccessfully;
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            tcpClient.Close();
        }
    }


    
    public class Server : IEchoApp
    {
        public void Start(RunSettings settings)
        {
            throw new NotImplementedException();
        }

        public bool SendMessage(string message)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}

