using System;
using System.Collections.Generic;
using System.Text;

namespace BattleshipsApp.Classes
{
    internal class ConnectionException : Exception
    {
        public override string ToString()
        {
            return base.Message;
        }
    }
}
