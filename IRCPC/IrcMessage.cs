using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCPC
{
    public class IrcMessage
    {
        public string Prefix { get; private set; }
        public string Message { get; private set; }
        public IEnumerable<string> Arguments { get; private set; }
        public string Command { get; private set; }
        public string Nick { get; private set; }
        public string Host { get; private set; }

        public IrcMessage(string s)
        {
            var parts = s.Split(' ');
            Prefix = parts[0].Substring(1);
            s = string.Join(" ", parts.Skip(1));
            parts = s.Split(':');
            if (parts.Length >= 2) Message = string.Join(":", parts.Skip(1));
            s = parts[0];
            parts = s.Split(' ');
            Command = parts[0];
            Arguments = parts.Skip(1);

            parts = Prefix.Split('!');
            Nick = parts[0];
            if (parts.Length >= 2) Host = parts[1];
        }
    }
}
