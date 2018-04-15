using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TcpDrivers
{
    public interface ITcpClientManager
    {
        string ClientEndPointName { get; }
        string ServertEndPointName { get; }
        bool IsClientConnected { get; }
        void Close();
        bool Connect(string endPointString);
        Task ReceiveMessageAsync(Func<string, Task> messageHandler);
        bool SendMessage(string messageToSend);
    }
}