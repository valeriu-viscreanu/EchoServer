using System;
using Application;
using Ninject;
using TCPApplication;

namespace EchoApp.Utils
{
    public static class ContainerUtils
    {
        public static void RegisterDependencies(IKernel container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container.Bind<IProgram>().To<Program>().InSingletonScope();
            container.Bind<IEchoApp>().To<TCPServer>().Named("serverApp");;
            container.Bind<IEchoApp>().To<TCPClient>().Named("clientApp");
            container.Bind<IEchoAppFactory>().To<EchoAppFactory>();
        }

    }
}