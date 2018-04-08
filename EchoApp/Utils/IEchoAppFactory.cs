using Application;
using TCPApplication;

namespace Application
{
    public interface IEchoAppFactory
    {
        IEchoApp GetEchoApp(RunMode runMode);
    }
}