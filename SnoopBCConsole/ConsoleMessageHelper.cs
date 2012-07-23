using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrumbLib;

namespace SnoopBCConsole
{
    class ConsoleMessageHelper : IMessageHandler
    {
        public void AddMessage(string message, bool isStatus = true, bool isDetail = true)
        {
            Console.WriteLine(message);
        }
    }
}
