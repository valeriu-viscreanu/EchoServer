using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TcpDrivers;

namespace Logic
{
    public class Client : IEchoApp
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ITcpClientManager tcpClientManager;
        private readonly List<string> serverList = new List<string>();

        public Client(ITcpClientManager tcpClientManager)
        {
            this.tcpClientManager = tcpClientManager;
        }

     

        public void Start(RunSettings settings)
        {
            serverList.AddRange(settings.ServerList);
            var isConnected = Connect();
            if (isConnected)
            {
                StartListening();
            }
            else
            {
                Stop();
            }
        }

        private bool Connect()
        {
            var isConnected = false;
            foreach (var serverEndpointString in serverList)
            {
                isConnected = this.tcpClientManager.Connect(serverEndpointString);
                if (isConnected)
                {
                    break;
                }
                MessageHandler?.Invoke($"Failed to connect to server {serverEndpointString}");
            }
            return isConnected;
        }


        /// <summary>
        /// Start listening for Tcp messages from the server
        /// </summary>
        public void StartListening()
        {
            MessageHandler?.Invoke($"Client connected with server  {this.tcpClientManager.ServertEndPointName}");
            // run this on the thread pool not to block the normal execution
            Task.Run(async () =>
            {
                await this.tcpClientManager.ReceiveMessageAsync(async message => { MessageHandler?.Invoke($"Received {message}"); });
            }, cancellationTokenSource.Token);
        }

        public bool SendMessage(string messageToSend)
        {
            if (!tcpClientManager.IsClientConnected)
            {
                bool isReconnected = Connect();
                if (!isReconnected)
                {
                    return false;
                }
                else
                {
                    MessageHandler?.Invoke($"Client reconnected with server  {this.tcpClientManager.ServertEndPointName}");
                }
            }
            return tcpClientManager.SendMessage(messageToSend);
        }

        public void Stop()
        {
            MessageHandler?.Invoke($"Stopping client {this.tcpClientManager.ClientEndPointName}");
            cancellationTokenSource.Cancel();
            this.tcpClientManager.Close();
        }

        public Action<string> MessageHandler { private get; set; }
    }
}

