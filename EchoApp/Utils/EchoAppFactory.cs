using System;
using Application;
using Ninject;

namespace Application
{
    public class EchoAppFactory : IEchoAppFactory
    {
        private readonly IEchoApp clientApp;
        private readonly IEchoApp serverApp;

        public EchoAppFactory([Named("clientApp")]IEchoApp clientApp, [Named("serverApp")]IEchoApp serverApp)
        {
            this.clientApp = clientApp;
            this.serverApp = serverApp;
        }

        public IEchoApp GetEchoApp(RunMode mode)
        {
            IEchoApp app;
            switch (mode)
            {
                case RunMode.Server:
                    app = this.serverApp;
                    break;
                case RunMode.Client:
                    app = this.clientApp;
                    break;
                default:
                    throw new ArgumentException("No mode set");
            }

            return app;
        }
    }
}
