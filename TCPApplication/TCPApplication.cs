using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPApplication
{
    public class TCPClient : IEchoApp
    {
        public void Start(RunSettings settings)
        {
            Console.WriteLine("Start");
        }

        public void SendMessage(string message)
        {
            Console.WriteLine($"Sending {message}");
        }

        public void Stop()
        {

            Console.WriteLine("Stop");
        }
    }



    public class TCPServer : IEchoApp
    {
        public void Start(RunSettings settings)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string message)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}

