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
        public MainWindow(IrcClient ircClient)
        {
            _ircClient = ircClient;
            InitializeComponent();
            InitializeIrc();
        }

        public void InitializeIrc()
        {
            _ircClient.MessageReceived += (sender, message) => // sito reik cia
            {
                Dispatcher.Invoke(() => 
                {
                    if(message.Host != null)
                    showMessages.AppendLine(FormatMessage(message), "Green");
                });
                //Console.WriteLine(message);
            }; // sita paliekam, nes cia pasakyta kaip atvaizduot zinutes or sth

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

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            string name = textBox.Text;
            if (string.IsNullOrWhiteSpace(name)) return;
            if (Keyboard.IsKeyDown(Key.Enter))
            {
                if (name[0] == '#') _ircClient.JoinGroupChat(name);
                chatName.Items.Add(name);
            }
        }

        private void sendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (chatName.SelectedItem == null)
            {
                MessageBox.Show("No selected item");
                return;
            }
            string nameOfChat = chatName.SelectedItem.ToString();
            if (nameOfChat[0] == '#') _ircClient.SendMessage(nameOfChat, writeMessages.Text);
            else _ircClient.SendMessage(nameOfChat, writeMessages.Text);
            showMessages.AppendLine(string.Format("[{0}]<gerda>: {1}", DateTime.Now.ToString("HH:mm"), writeMessages.Text), "red");
        }
    }
}
