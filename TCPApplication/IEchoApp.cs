namespace TCPApplication
{
    public interface IEchoApp
    {
        void Start(RunSettings settings);
        bool SendMessage(string message);
        void Stop();
    }
}