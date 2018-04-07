using System.Collections.Generic;

namespace Application
{
    internal interface IProgram
    {
        void Run();
        IEchoApp EchoApp{ get; set; }

        /// <summary>
        /// Parses command line argumets
        /// Due to brevety reasons a direct approach it is used instead of a more flexbile parsing system
        /// and also arguments need to be in a fixed order
        /// </summary>
        /// <param name="arguments"></param>
        void ParseCommandLineArguments(IReadOnlyList<string> arguments);
    }
}