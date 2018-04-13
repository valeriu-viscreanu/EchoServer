using System;

namespace Logic
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
        /// Stop everything and dispose
        /// </summary>
        void Stop();

        /// <summary>
        ///  This will handle messages and will be used by the UI
        /// </summary>
        Action<string> MessageHandler {set; }
    }
}