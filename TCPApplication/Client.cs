using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TcpDrivers;

namespace Logic
{
    public class Client : IEchoApp
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ITCPManager tcpManager; 
        private readonly IEnumerable<IPEndPoint> endPoints = new List<IPEndPoint>();
        private readonly List<string> serverList = new List<string>();

        public Client(ITCPManager tcpManager)
        {
            this.tcpManager = tcpManager;
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
                isConnected = tcpManager.Connect(serverEndpointString);
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
            MessageHandler?.Invoke($"Client connected with server  {tcpManager.ServertEndPointName}");
            // run this on the thread pool not to block the normal execution
            Task.Run(async () =>
            {
                await tcpManager.ReceiveMessageAsync(async message => { MessageHandler?.Invoke($"Received {message}"); });
            }, cancellationTokenSource.Token);
        }

        public bool SendMessage(string messageToSend)
        {
            if (!tcpManager.IsClientConnected)
            {
                bool isReconnected = Connect();
                if (!isReconnected)
                {
                    return false;
                }
                else
                {
                    MessageHandler?.Invoke($"Client reconnected with server  {tcpManager.ServertEndPointName}");
                }
            }
            return tcpManager.SendMessage(messageToSend);
        }

        public void Stop()
        {
            MessageHandler?.Invoke($"Stopping client {tcpManager.ClientEndPointName}");
            cancellationTokenSource.Cancel();
            tcpManager.Close();
        }

        public Action<string> MessageHandler { private get; set; }
    }
}

