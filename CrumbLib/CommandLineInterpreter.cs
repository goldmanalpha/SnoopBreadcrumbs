using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrumbLib
{
    public class CommandLineInterpreter
    {

        public CommandLineInterpreter()
        {
            Ignores = new List<string>();

        }

        public bool Parse(string[] args)
        {
            if (args == null || args.Length == 0)
                return false;

            Path = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                Ignores.Add(args[i]);                
            }

            return true;
        }


        public List<string> Ignores { get; private set; }
        public string Path { get; private set; }
    }
}
