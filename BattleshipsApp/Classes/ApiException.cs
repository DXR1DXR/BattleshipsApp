using System;
using System.Collections.Generic;
using System.Text;

namespace BattleshipsApp.Classes
{
    internal class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }
        public override string ToString()
        {
            return base.Message;
        }
    }
}
