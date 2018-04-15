using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpDrivers
{
    public class TcpServerManager : TcpManagerBase, ITcpServerManager
    {

        private const int PortNumber = 55001;

        /// <summary>
        /// This handles the echo logic
        /// </summary>
        /// <param name="clientSender">the sender</param>
        /// <param name="tcpClients">all clients</param>
        /// <param name="messageHandler">an action in order to use the message to be echoed</param>
        /// <returns></returns>
        public async Task EchoMessageAsync(TcpClient clientSender, IEnumerable<TcpClient> tcpClients, Action<string> messageHandler)
        {
            if (!tcpClients.Any())
            {
                return;
            }
            await ReceiveMessageAsync(async message =>
            {
                var messageToSend = $"Echo -> {message}";
                await SendMessage(tcpClients, messageHandler, messageToSend);
            }, clientSender);
        }

        public async Task SendMessage(IEnumerable<TcpClient> tcpClients, Action<string> messageHandler, string messageToSend)
        {
            var buffer = Encoding.ASCII.GetBytes(messageToSend);
            foreach (var client in tcpClients)
            {
                if (!client.Connected) continue;
                messageHandler?.Invoke($"Sending {messageToSend} to {client.Client.RemoteEndPoint}");
                await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}
