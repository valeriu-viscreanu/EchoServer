using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPApplication
{
    public class TCPManager
    {
        public async Task ReceiveMessageAsync(TcpClient client, Func<string, Task> messageProcessor)
        {
            var networkStream = client.GetStream();
            var buffer = new byte[4096]; 

            int bytesReceived;
            while ((bytesReceived = await networkStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                var data = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                await messageProcessor(data);
            }

        }

        public async Task EchoMessageAsync(TcpClient clientSender, IEnumerable<TcpClient> connectedClients)
        {
            await ReceiveMessageAsync(clientSender, async message =>
           {
               var messageToSend = $"Echo -> {message}";
               var buffer = Encoding.ASCII.GetBytes(messageToSend);
               foreach (var client in connectedClients)
               {
                   if (!client.Connected) continue;
                   Console.WriteLine($"Sending {messageToSend} to {client.Client.RemoteEndPoint}");
                   await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
               }
           });
        }
    }
}
