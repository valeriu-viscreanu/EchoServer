using System;
using Application;
using Ninject;
using Logic;
using TcpDrivers;

namespace EchoApp.Utils
{
    public static class ContainerUtils
    {
        public static void RegisterDependencies(IKernel container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container.Bind<IProgram>().To<Program>().InSingletonScope();
            container.Bind<IEchoApp>().To<Server>().Named("serverApp");;
            container.Bind<IEchoApp>().To<Client>().Named("clientApp");
            container.Bind<IEchoAppFactory>().To<EchoAppFactory>();
            container.Bind<ITCPManager>().To<TCPManager>();
        }

    }
}