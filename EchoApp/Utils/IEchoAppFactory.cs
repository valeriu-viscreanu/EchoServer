using Application;

namespace Application
{
    public interface IEchoAppFactory
    {
        IEchoApp GetEchoApp(RunMode mode);
    }
}