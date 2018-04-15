using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TcpDrivers
{
    public interface ITCPManager
    {
        string ClientEndPointName { get; }
        string ServertEndPointName { get; }
        bool IsClientConnected { get; }
        void Close();
        bool Connect(string endPointString);
        IEnumerable<IPEndPoint> GetIpEndPoints(IEnumerable<string> endPoints);
        Task EchoMessageAsync(TcpClient clientSender, IEnumerable<TcpClient> connectedClients, Action<string> messageHandler);
        Task ReceiveMessageAsync(Func<string, Task> messageHandler, TcpClient clientSender = null);
        bool SendMessage(string messageToSend);
    }
}