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
        private ITCPManager tcpManager; // TODO solve this with DI
        private IEnumerable<IPEndPoint> endPoints = new List<IPEndPoint>();

        public Client(ITCPManager tcpManager)
        {
            this.tcpManager = tcpManager;
        }

        /// <summary>
        /// try to connect to one of the servers
        /// if all have failed returns false
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            foreach (var ipEndpoint in endPoints)
            {
                try
                {
                    tcpManager.Connect(ipEndpoint);
                    // if no exception we assume to be connected successfully
                    return true;
                }
                catch
                {
                    continue;
                }
            }

            return false;
        }

        public void Start(RunSettings settings)
        {
            endPoints = tcpManager.CreateIpEndPoints(settings.ServerList);
            var connected = Connect();
            try
            {
                if (connected)
                {
                    StartListening();
                }
                else
                {
                    // force a reconection
                    throw  new Exception();
                }
            }
            catch
            {
                var reconnected = Connect();
                if (reconnected)
                {
                    StartListening();
                }
                else
                {
                    // we reached this point probably means something went wrong or we are not reading anymore (half-open connection)
                    Stop();
                    throw;
                }
            }

        }

        /// <summary>
        /// Start listening for Tcp messages from the server
        /// </summary>
        public void StartListening()
        {
            MessageHandler?.Invoke($"Client connected with server  {tcpManager.ClientEndPoint}");
            // run this on the thread pool not to block the normal execution
            Task.Run(async () =>
            {
                await tcpManager.ReceiveMessageAsync(async message => { MessageHandler?.Invoke($"Received {message}"); });
            }, cancellationTokenSource.Token);
        }

        public bool SendMessage(string messageToSend)
        {
            var isSentSuccessfully = false;
            if (!tcpManager.IsClientConnected)
            {
                return isSentSuccessfully;
            }
            isSentSuccessfully = tcpManager.SendMessage(messageToSend);
            return isSentSuccessfully;
        }

        public void Stop()
        {
            MessageHandler?.Invoke($"Stopping client {tcpManager.ClientEndPoint}");
            cancellationTokenSource.Cancel();
            tcpManager.Close();
        }

        public Action<string> MessageHandler { private get; set; }
    }
}

