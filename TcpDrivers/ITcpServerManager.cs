using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TcpDrivers
{
    public interface ITcpServerManager
    {
        Task EchoMessageAsync(TcpClient clientSender, IEnumerable<TcpClient> tcpClients, Action<string> messageHandler);
        Task SendMessage(IEnumerable<TcpClient> tcpClients, Action<string> messageHandler, string messageToSend);
    }
}
