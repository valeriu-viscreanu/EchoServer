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
        private  readonly TcpClient tcpClient = new TcpClient();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public void Start(RunSettings settings)
        {
            var ipEndPointString = settings.ServerList.First(); // in case of non existing or invalid IP we throw for now...
            var ipEndpoint = Utils.CreateIPEndPoint(ipEndPointString); // Fixed port for now 

            tcpClient.Connect(ipEndpoint);
            
            // run this on the thread pool not to block The UI
            Task.Run(() =>
            {
                int bytesReceived;
                var stream = tcpClient.GetStream();
                var bytes = new byte[4096];

                while ((bytesReceived = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    var data = Encoding.ASCII.GetString(bytes, 0, bytesReceived);
                    Console.WriteLine($"Received: {data}");
                }
            }, cancellationTokenSource.Token);

        }

        public void SendMessage(string messageToSend)
        {
            var buffer = Encoding.ASCII.GetBytes(messageToSend);
            tcpClient.GetStream().Write(buffer, 0, buffer.Length);
        }

        public void Stop()
        {
            this.cancellationTokenSource.Cancel();
            tcpClient.Close();
        }
    }



    public class Server : IEchoApp
    {
        public void Start(RunSettings settings)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string message)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}

