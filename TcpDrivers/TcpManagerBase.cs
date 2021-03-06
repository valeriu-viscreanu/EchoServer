﻿using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpDrivers
{
    public abstract class TcpManagerBase
    {

        private const int maxPortNumber = 65535;
        private const int minPortNumber = 0;
        private const int bufferSize = 4096;
        /// <summary>
        /// reads the ip endpoint from a string 
        /// </summary>
        /// <param name="e  ndPoint"></param>
        /// <returns></returns>
        protected static IPEndPoint GetIpEndPoint(string endPoint)
        {
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
                throw new FormatException("Invalsid port");
            }
            return new IPEndPoint(ip, port);
        }

        /// <summary>
        /// process the messages from a TCP client in an async mode
        /// it needs to know the client sender if used with the server
        /// TODO rework the server logic and change this
        /// </summary>
        /// <param name="clientSender"></param>
        /// <param name="messageHandler"></param>
        /// <returns></returns>
        public async Task ReceiveMessageAsync(Func<string, Task> messageHandler, TcpClient clientSender)
        {
            var networkStream = clientSender.GetStream();
            var buffer = new byte[bufferSize];
            var bytesReceived = 0;
            var receivedMessage = string.Empty;
            while ((bytesReceived = await networkStream.ReadAsync(buffer, 0, bufferSize)) != 0)
            {
                // Translate data bytes to a ASCII string.
                var data = Encoding.ASCII.GetString(buffer, 0, bytesReceived);

                if (bytesReceived >= bufferSize)
                {
                    receivedMessage += data; // string builder
                }
                else
                {
                    if (string.IsNullOrEmpty(receivedMessage))
                    {
                        receivedMessage = data;
                    }

                    await messageHandler(receivedMessage);
                    receivedMessage = string.Empty;
                }
            }
        }


    }
}
