using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Ninject;

namespace Application
{
    public class TCPClient : IEchoApp
    {
        public void Start(RunSettings settings)
        {
            Console.WriteLine("Staring");
        }

        public void SendMessage(string message)
        {
            Console.WriteLine($"Sending message {message}");
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

    internal class Program : IProgram
    {
        #region StaticMembers
        private static readonly IProgram ApplicationInstance = new Program();
        private static readonly IKernel Container = new StandardKernel();
        #endregion

        #region PrivateMembers
        private RunSettings runSettings;
        private IEchoApp echoApp;

        /// <summary>
        /// Exposed for 
        /// </summary>
        public IEchoApp EchoApp
        {
            get
            {
                return echoApp;
            }

            set
            {
                echoApp = value;
            }
        }
        #endregion

        #region InstanceMethods

        public void Run()
        {
            Console.WriteLine($"Starting in {runSettings.Mode} mode...");
            this.echoApp = CreateApp();
            echoApp.Start(runSettings);
            while (true)
            {
                Console.WriteLine("Enter a message to send and press enter or enter exit to stop the the program");
                var input = Console.ReadLine();
                if (input == "exit")
                {
                    echoApp.Stop();
                    Console.WriteLine("Stopping ...");
                    break;
                }

                echoApp.SendMessage(input);
            }
        }

        private IEchoApp CreateApp()
        {
            IEchoApp app;
            switch (runSettings.Mode)
            {
                case RunMode.Server:
                    app = new TCPServer();
                    break;
                case RunMode.Client:
                    app = new TCPClient(); 
                    break;
                default:
                    throw new ArgumentException("No mode set");
            }

            return app;
        }



        /// <summary>
        /// Parses command line argumets
        /// Due to brevety reasons a direct approach it is used instead of a more flexbile parsing system
        /// and also arguments need to be in a fixed order
        /// </summary>
        /// <param name="arguments"></param>
        public void ParseCommandLineArguments(IReadOnlyList<string> arguments)
        {
            if (arguments.Count == 0)
            {
                throw new ArgumentException("You must specify mode of running please check the readme");
            }
            RunMode mode;
            Enum.TryParse(arguments[0], true, out mode); //TODO  handle parse failure

            var serverLists = new List<string>();
            if (mode == RunMode.Client)
            {
                // add the rest of the command arguments as server 
                serverLists.AddRange(arguments.Skip(1));
            }

            runSettings = new RunSettings { Mode = mode, ServerList = serverLists };
        }

        #endregion

        static void Main(string[] args)
        {
            try
            {
                Container.Bind<IProgram>().To<Program>().InSingletonScope();

                ApplicationInstance.ParseCommandLineArguments(args);
                ApplicationInstance.Run();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Application failed due to wrong arguments message : {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application failed with unexpected exception message : {ex.Message} stack trace  : {ex.StackTrace} and inner exception : {ex.InnerException?.Message}");
            }
            finally
            {
                Console.ReadKey();
            }

        }
    }
}
