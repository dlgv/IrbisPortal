using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BotUAC
{
    public class TException : Exception
    {
            public TException()
            {
            }
            public TException(string message)
                : base(message)
            {
            }
            public TException(string message, Exception inner)
                : base(message, inner)
            {
            }
     } // class TException
}
