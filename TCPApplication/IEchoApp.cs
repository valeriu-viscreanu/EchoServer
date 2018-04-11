namespace TCPApplication
{
    public interface IEchoApp
    {
        /// <summary>
        /// Start the app
        /// </summary>
        /// <param name="settings"></param>
        void Start(RunSettings settings);
        /// <summary>
        /// Send a message
        /// </summary>
        bool SendMessage(string message);
        /// <summary>
        /// stop everything and dispose
        /// </summary>
        void Stop();
    }
}