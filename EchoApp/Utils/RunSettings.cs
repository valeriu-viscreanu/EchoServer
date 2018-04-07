using System.Collections.Generic;
using Application;

namespace Application
{
    public class RunSettings
    {
        public RunMode Mode { get; set; }
        public IEnumerable<string> ServerList { get; set; }
    }

    public enum RunMode
    {
        Error = 0,
        Server = 1,
        Client = 2
    }
}