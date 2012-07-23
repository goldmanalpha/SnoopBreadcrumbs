using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrumbLib;

namespace SnoopBCConsole
{
    class Program
    {
        static int Main(string[] args)
        {
            var interpreter = new CommandLineInterpreter();

            if (!interpreter.Parse(args))
            {
                Console.WriteLine("Usage: SnoopBCConsole RootPath [ignoreControl1] [ignoreControl2] [ignoreControl3] [...]");
                return 1;
            }
            else
            {

                var helper = new XmlHelper();

                //helper.TagXmlElements(interpreter.
            
            }

            return 0;
        }
    }
}
