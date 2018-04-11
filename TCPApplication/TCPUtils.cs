using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPApplication
{
    public class TCPUtils
    {
        public async Task ReceiveDataAsync(TcpClient client, Action<string> dataProcessor)
        {
            var networkStream = client.GetStream();
            var buffer = new byte[4096];

            int bytesReceived;
            while ((bytesReceived = await networkStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                var data = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                dataProcessor(data); 
            }

        }
    }
}