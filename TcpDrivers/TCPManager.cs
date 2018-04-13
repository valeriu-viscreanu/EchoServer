using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpDrivers
{
    public class TCPManager : ITCPManager
    {

        private readonly TcpClient tcpClient = new TcpClient();
        /// <summary>
        /// process the messages from a TCP client in an async mode
        /// it needs to know the client sender if used with the server
        /// TODO rework the server logic and change this
        /// </summary>
        /// <param name="clientSender"></param>
        /// <param name="messageHandler"></param>
        /// <returns></returns>
        public async Task ReceiveMessageAsync (Func<string, Task> messageHandler, TcpClient clientSender = null)
        {
            if (clientSender == null)
            {
                 clientSender = this.tcpClient;
            }

            var networkStream = clientSender.GetStream();
            var buffer = new byte[4096];

            int bytesReceived;
            while ((bytesReceived = await networkStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                var data = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                await messageHandler(data);
            }

        }

        /// <summary>
        /// This handles the echo logic
        /// </summary>
        /// <param name="clientSender">the sender</param>
        /// <param name="connectedClients">all clients</param>
        /// <param name="messageHandler">an action in order to use the message to be echoed</param>
        /// <returns></returns>
        public  async Task EchoMessageAsync(TcpClient clientSender, IEnumerable<TcpClient> connectedClients, Action<string> messageHandler)
        {
            if (clientSender == null || !connectedClients.Any())
            {
                return;
            }

           await ReceiveMessageAsync(async message =>
           {
               var messageToSend = $"Echo -> {message}";
               var buffer = Encoding.ASCII.GetBytes(messageToSend);
               foreach (var client in connectedClients)
               {
                   if (!client.Connected) continue;
                   messageHandler?.Invoke($"Sending {message} to {client.Client.RemoteEndPoint}");
                   await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
               }
           }, clientSender);
        }

        /// <summary>
        /// get the client connection state
        /// </summary>
        /// <returns></returns>
        public static TcpState GetState(TcpClient tcpClient)
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpConnections()
                .SingleOrDefault(x => x.LocalEndPoint.Equals(tcpClient?.Client.LocalEndPoint));
            return ipProperties?.State ?? TcpState.Unknown;
        }

        /// <summary>
        /// reads the ip endpoint from a string 
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        private static IPEndPoint CreateIpEndPoint(string endPoint)
        {
            const int minPortNumber = 0;
            const int maxPortNumber = 65535;

            string[] ep = endPoint.Split(':');
            if (ep.Length != 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if (!IPAddress.TryParse(ep[0], out ip))
            {
                throw new FormatException("Invalid ip-adress");
            }
            int port;
            if (!int.TryParse(ep[1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port) && port > minPortNumber && port < maxPortNumber)
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }


        public  IEnumerable<IPEndPoint> CreateIpEndPoints(IEnumerable<string> endPoints)
        { 
            foreach (var endPoint in endPoints)
            {
                yield return CreateIpEndPoint(endPoint);
            }
        }
        

        public void Connect(IPEndPoint endpoint)
        {
            if (!IsClientConnected)
            {
                tcpClient.Connect(endpoint);
            }
        }

        public bool IsClientConnected
        {
            get
            {
                return  GetState(tcpClient) == TcpState.Established;
            }
        }

        public bool SendMessage(string messageToSend)
        {
            if (IsClientConnected)
            {
                var buffer = Encoding.ASCII.GetBytes(messageToSend);
                tcpClient.GetStream().Write(buffer, 0, buffer.Length);
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ClientEndPoint
        {
            get
            {
                return tcpClient?.Client?.LocalEndPoint.ToString();
            }
        }

        public void Close()
        {
            tcpClient.Close();
        }
    }
}
