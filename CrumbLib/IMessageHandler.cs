using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrumbLib
{
    public interface IMessageHandler
    {
        void AddMessage(string message, bool isStatus = true, bool isDetail = true);
    }
}
