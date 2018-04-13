using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TcpDrivers
{
    public interface ITCPManager
    {
        string ClientEndPoint { get; }
        bool IsClientConnected { get; }

        void Close();
        void Connect(IPEndPoint endpoint);
        IEnumerable<IPEndPoint> CreateIpEndPoints(IEnumerable<string> endPoints);
        Task EchoMessageAsync(TcpClient clientSender, IEnumerable<TcpClient> connectedClients, Action<string> messageHandler);
        Task ReceiveMessageAsync(Func<string, Task> messageHandler, TcpClient clientSender = null);
        bool SendMessage(string messageToSend);
    }
}