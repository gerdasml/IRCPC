using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        public event EventHandler<IrcMessage> ErrorOccurred;
        private StreamWriter _logger = new StreamWriter(new FileStream("log.txt", FileMode.Append));

        ManualResetEvent loginFinished = new ManualResetEvent(false);
        bool loginSuccess = false;
        public string MyNick { get; private set; }

        private List<string> _errorList = new List<string>
        {
            "400",
            "401",
            "402",
            "403",
            "404",
            "405",
            "406",
            "407",
            "408",
            "408",
            "409",
            "411",
            "412",
            "413",
            "414",
            "415",
            "416",
            "416",
            "419",
            "421",
            "422",
            "423",
            "424",
            "425",
            "429",
            "430",
            "431",
            "432",
            "433",
            "434",
            "434",
            "435",
            "435",
            "436",
            "437",
            "437",
            "438",
            "438",
            "439",
            "440",
            "441",
            "442",
            "443",
            "444",
            "445",
            "446",
            "447",
            "449",
            "451",
            "452",
            "453",
            "455",
            "456",
            "457",
            "458",
            "459",
            "460",
            "461",
            "462",
            "463",
            "464",
            "465",
            "466",
            "467",
            "468",
            "468",
            "469",
            "470",
            "470",
            "471",
            "472",
            "473",
            "474",
            "475",
            "476",
            "477",
            "477",
            "478",
            "479",
            "479",
            "480",
            "480",
            "481",
            "482",
            "483",
            "484",
            "484",
            "484",
            "484",
            "485",
            "485",
            "485",
            "485",
            "486",
            "486",
            "486",
            "487",
            "487",
            "488",
            "489",
            "489",
            "491",
            "492",
            "493",
            "494",
            "495",
            "496",
            "497",
            "498",
            "499",
            "501",
            "502",
            "503",
            "503",
            "504",
            "511",
            "512",
            "513",
            "514",
            "514",
            "515",
            "516",
            "517",
            "518",
            "518",
            "519",
            "519",
            "520",
            "520",
            "520",
            "521",
            "522",
            "523",
            "524",
            "524",
            "525",
            "526",
            "550",
            "551",
            "552",
            "553",
            "972",
            "973",
            "974",
            "975",
            "976",
            "977",
            "979",
            "980",
            "981",
            "982",
            "983",
            "999"
        };

        public IrcClient(string server, int port)
        {
            _logger.AutoFlush = true;
            //cia reiktu to try catch apie kuri kalbejai?
            _server = server;
            _port = port;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(_server, _port);

            _stream = new NetworkStream(_socket);
            _stream.ReadTimeout = int.MaxValue;

            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);

           

        }
        public void Listen()
        {
            _listeningTask = new Task(() => Listener());
            _listeningTask.Start();
        }
        private async void Listener()
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
                        var smth = new IrcMessage(message);

                        if (smth.Command == "001")
                        {
                            loginSuccess = true;
                            loginFinished.Set();
                        }
                        if (smth.Host == null && _errorList.Contains(smth.Command) && ErrorOccurred != null)
                        {
                            loginFinished.Set();
                            ErrorOccurred(this, smth);
                        } 

                        if (MessageReceived != null)
                        {
                            MessageReceived(this, smth);
                        }
                    }
                }
                catch(Exception)
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

        public bool Connect(string nickname, string realName)
        {
            loginFinished.Reset();
            MyNick = nickname;
            SendMessage("User " + nickname + " 0 * : " + realName);
            SendMessage("Nick " + nickname);

            loginFinished.WaitOne();
            return loginSuccess;
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

        public void CheckIfUserExists(string nick)
        {
            SendMessage("WHOIS "+ nick);
        }
    }
}
