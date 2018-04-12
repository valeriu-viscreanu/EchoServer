using System;
using Ninject;

namespace TCPApplication
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

        public IEchoApp GetEchoApp(RunMode runMode)
        {
            IEchoApp app;
            switch (runMode)
            {
                case RunMode.Server:
                    app = this.serverApp;
                    break;
                case RunMode.Client:
                    app = this.clientApp;
                    break;
                default:
                    throw new ArgumentException($"No {nameof(runMode)} set");
            }

            return app;
        }
    }
}
