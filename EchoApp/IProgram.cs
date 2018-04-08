using System.Collections.Generic;
using TCPApplication;

namespace Application
{
    public interface IProgram
    {
        void Run(RunSettings runSettings);
    }
}