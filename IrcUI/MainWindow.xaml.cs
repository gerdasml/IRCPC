using IRCPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IrcUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IrcClient _ircClient;
        public MainWindow()
        {
            InitializeComponent();
            InitializeIrc();
        }

        private void InitializeIrc()
        {
            _ircClient = new IrcClient("irc.freenode.net", 6667);
            _ircClient.MessageReceived += (sender, message) =>
            {
                Dispatcher.Invoke(() => 
                {
                    if(message.Host != null)
                    showMessages.AppendLine(FormatMessage(message), "Green");
                });
                //Console.WriteLine(message);
            };
            _ircClient.Connect("gerda", "Gerda");
           //irc.SendPrivateMessage("vstrimaitis", "this is Gerda from VS");
           _ircClient.JoinGroupChat("strymas");
           // _ircClient.SendGroupMessage("strymas", "i love you too");
           // _ircClient.SendGroupMessage("strymas", "i hope this works");
           // _ircClient.LeaveGroupChat("strymas", "i have to pee");
           // Thread.Sleep(2000000);
           // _ircClient.Disconnect();

        }

        private string FormatMessage(IrcMessage msg)
        {
            if (msg.Command == "JOIN") return string.Format("<{0}> has joined", msg.Nick);
            if (msg.Command == "PART") return string.Format("<{0}> has left the conversation", msg.Nick);
            if (msg.Command == "PRIVMSG") return string.Format("[{0}]<{1}> : {2}", DateTime.Now.ToString("HH:mm"), msg.Nick, msg.Message);
            if (msg.Command == "NICK") return string.Format("<{0}> has changed nickname to <{1}>", msg.Nick, msg.Message);
            if (msg.Command == "QUIT") return string.Format("<{0}> has quit ({1})", msg.Nick, msg.Message);
            return "--parsing error--";
        }
    }
}
