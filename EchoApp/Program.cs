using System;
using System.Net;
using System.Net.Sockets;
using EchoApp.Utils;
using TCPApplication;
using Ninject;


namespace Application
{
    public class Program : IProgram
    {
        static void Main(string[] args)
        {
            try
            {
                ContainerUtils.RegisterDependencies(Container);
                Application = Container.Get<IProgram>();
                var runSettings = Utils.ParseCommandLineArguments(args);
                Application.Run(runSettings);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Application failed due to wrong arguments message : {ex.Message}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Application connection failed due to : {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Application failed with an unexpected exception message : {ex.Message} stack trace  : {ex.StackTrace} and inner exception : {ex.InnerException?.Message}");
            }
            finally
            {
                Console.ReadKey();
            }
        }


        #region StaticMembers
        private static IProgram Application;
        private static readonly IKernel Container = new StandardKernel();
        #endregion

        #region PrivateMembers
        private IEchoApp echoApp;
        private IEchoAppFactory echoAppFactory;
        #endregion

        #region InstanceMethods

        public Program(IEchoAppFactory echoAppFactory)
        {
            this.echoAppFactory = echoAppFactory;
        }

        /// <summary>
        ///  Main app run point
        ///  Due to depency with static Console this proved to be untestable
        ///  To be fixed
        /// </summary>
        public void Run(RunSettings runSettings)
        {
            Console.WriteLine($"Starting in {runSettings.Mode} mode...");
            this.echoApp = this.echoAppFactory.GetEchoApp(runSettings.Mode);
            Console.WriteLine("Attempting to connect...");
            echoApp.Start(runSettings);
            while (true)
            {
                Console.WriteLine("Enter a message to send and press enter or enter exit to stop the the program");
                var input = Console.ReadLine();
                if (input == "exit")
                {
                    echoApp.Stop();
                    Console.WriteLine("Stopping ... Press any key to exit");
                    break;
                }
                var result = echoApp.SendMessage(input);
                if (!result)
                {
                    Console.WriteLine("Could not send message");
                }
            }
        }
        #endregion
    }
}
