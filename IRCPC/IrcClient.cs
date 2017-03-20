using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IRCPC
{
    public class IrcClient
    {
        private string _server;
        private int _port;
        private Socket _socket;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private Task _listeningTask;
        public event EventHandler<IrcMessage> MessageReceived;
        private StreamWriter _logger = new StreamWriter(new FileStream("log.txt", FileMode.Append));
        public string MyNick { get; private set; }
        public IrcClient(string server, int port)
        {
            _logger.AutoFlush = true;

            _server = server;
            _port = port;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_server, _port);

            _stream = new NetworkStream(_socket);
            _stream.ReadTimeout = int.MaxValue;

            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);

            _listeningTask = new Task(() => Listen());
            _listeningTask.Start();

        }

        private async void Listen()
        {
            while (true)
            {
                try
                {

                    var message = await _reader.ReadLineAsync();
                    _logger.WriteLine(string.Format("[{0}] s:\t{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message));
                    if (message.StartsWith("PING", StringComparison.InvariantCultureIgnoreCase))
                    {
                        StringBuilder sb = new StringBuilder(message);
                        sb[1] = 'O';
                        Console.WriteLine("\t" + message);
                        Console.WriteLine("\t" + sb.ToString());
                        SendMessage(sb.ToString());
                    }
                    else
                    {
                        if (MessageReceived != null)
                        {
                            MessageReceived(this, new IrcMessage(message));
                        }
                    }
                }
                catch(Exception )
                {
                    break;
                }
            }
        }

        private void SendMessage(string message)
        {
            _writer.WriteLine(message);
            _writer.Flush();
            _logger.WriteLine(string.Format("[{0}] c:\t{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), message));
        }

        public void Connect(string nickname, string realName)
        {
            MyNick = nickname;
            SendMessage("Nick " + nickname);
            SendMessage("User " + nickname + " 0 * : " + realName);
        }

        public void Disconnect()
        {
            SendMessage("Quit");
            _reader.Close();
            _writer.Close();
            _stream.Close();
            _socket.Close();
        }

        public void SendMessage(string friendNick, string privMessage)
        {
            SendMessage("privmsg " + friendNick + " : " + privMessage);
        }

        public void JoinGroupChat(string chatName)
        {
            SendMessage("join " + chatName);
        }

        public void LeaveGroupChat(string chatName)
        {
            SendMessage("part " + chatName);
        }
    }
}
