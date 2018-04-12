
namespace TCPApplication
{
    public interface IEchoAppFactory
    {
        IEchoApp GetEchoApp(RunMode runMode);
    }
}