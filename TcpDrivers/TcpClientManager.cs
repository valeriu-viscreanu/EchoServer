using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpDrivers
{
    public class TcpClientManager : TcpManagerBase, ITcpClientManager
    {

        private readonly TcpClient localTcpClient = new TcpClient();


        /// <summary>
        /// try to connect to a server
        /// </summary>
        /// <returns>connected successfully</returns>
        public bool Connect(string endPointString)
        {
            var ipEndpoint = GetIpEndPoint(endPointString);
            try
            {
                Connect(ipEndpoint);
            }
            catch
            {
                return false;
            }
            // if no exception we assume to be connected successfully
            return true;
        }

        public async Task ReceiveMessageAsync(Func<string, Task> messageHandler)
        {
           await ReceiveMessageAsync(messageHandler, localTcpClient);
        }

        /// <summary>
        /// get the client connection state
        /// </summary>
        /// <returns></returns>
        public static TcpState GetTcpState(TcpClient tcpClient)
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties()
                .GetActiveTcpConnections()
                .SingleOrDefault(x => x.LocalEndPoint.Equals(tcpClient?.Client?.LocalEndPoint));
            return ipProperties?.State ?? TcpState.Unknown;
        }
        
        public  IEnumerable<IPEndPoint> GetIpEndPoints(IEnumerable<string> endPoints)
        { 
            foreach (var endPoint in endPoints)
            {
                yield return GetIpEndPoint(endPoint);
            }
        }
        

        public void Connect(IPEndPoint endpoint)
        {
            if (!IsClientConnected)
            {
                localTcpClient?.Connect(endpoint);
            }
        }

        public bool IsClientConnected
        {
            get
            {
                return  GetTcpState(localTcpClient) == TcpState.Established;
            }
        }

        public bool SendMessage(string messageToSend)
        {
            if (IsClientConnected)
            {
                var buffer = Encoding.ASCII.GetBytes(messageToSend);
                localTcpClient.GetStream().Write(buffer, 0, buffer.Length);
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ClientEndPointName
        {
            get
            {
                return localTcpClient?.Client?.LocalEndPoint?.ToString();
            }
        }
        public string ServertEndPointName
        {
            get
            {
                return localTcpClient?.Client?.RemoteEndPoint.ToString();
            }
        }

        public void Close()
        {
            localTcpClient.Close();
        }
    }
}
