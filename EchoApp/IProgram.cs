using System.Collections.Generic;
using Logic;

namespace Application
{
    public interface IProgram
    {
        void Run(RunSettings runSettings);
    }
}