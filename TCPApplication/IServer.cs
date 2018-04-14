namespace Logic
{
    public interface IServer
    {
        void Start(RunSettings settings);
        bool SendMessage(string message);
        void Stop();
    }
}