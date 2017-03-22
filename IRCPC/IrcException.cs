using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCPC
{
    class IrcException : Exception
    {
        public IrcException() : base()
        { }

        public IrcException(string message) : base(message)
        { }

        public IrcException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
