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
            var msgHelper = new ConsoleMessageHelper();
                

            if (!interpreter.Parse(args))
            {
                msgHelper.AddMessage("Usage: SnoopBCConsole RootPath [ignoreControl1] [ignoreControl2] [ignoreControl3] [...]");
                return 1;
            }
            else
            {

                var processor = new XamlFilesProcessor();

                var xamls = processor.FindFiles(interpreter.Path, 
                    (s, s1) => msgHelper.AddMessage(s + ": " + s1), msgHelper);

                if (xamls.Count == 0)
                {
                    msgHelper.AddMessage("No Xaml files found");
                    return 2;
                }
                else
                {
                    var exceptions = processor.ProcessXamls(xamls, 
                        (s, s1) => msgHelper.AddMessage(s + ": " + s1), i => {}, 
                        msgHelper);

                    if (exceptions > 0)
                    {
                        msgHelper.AddMessage("Exception count: " + exceptions);
                        return 3;
                    }
                }
            }

            return 0;
        }

        
    }


}
