using System;
using System.Collections.Generic;
using System.Linq;
using Logic;

namespace Application
{
    public class Utils
    {
        /// <summary>
        /// Parses command line argumets
        /// Due to brevety reasons a direct approach it is used instead of a more flexbile parsing system
        /// and also arguments need to be in a fixed order
        /// </summary>
        /// <param name="arguments"></param>
        /// <exception cref="ArgumentException"></exception>
        public static RunSettings ParseCommandLineArguments(IReadOnlyList<string> arguments)
        {
            if (arguments.Count == 0)
            {
                throw new ArgumentException("You must specify mode of running please check the readme");
            }
            RunMode mode;
            var parsedOk = Enum.TryParse(arguments[0], true, out mode);
            if(!parsedOk) throw new ArgumentException("You must specify mode of running please check the readme");
            var serverLists = new List<string>();
            if (mode == RunMode.Client)
            {
                // add the rest of the command arguments as server 
                serverLists.AddRange(arguments.Skip(1));
            }

            return  new RunSettings { Mode = mode, ServerList = serverLists };
        }
    }
}