namespace Application
{
    public interface IEchoApp
    {
        void Start(RunSettings settings);
        void SendMessage(string message);
        void Stop();
    }
}