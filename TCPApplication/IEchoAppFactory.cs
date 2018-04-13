
namespace Logic
{
    public interface IEchoAppFactory
    {
        IEchoApp GetEchoApp(RunMode runMode);
    }
}