using IRCPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IrcUI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    /// 

   
    public partial class LoginWindow : Window
    {
        private IrcClient _ircClient;
        public LoginWindow()
        {
            InitializeComponent();
            try
            {
                _ircClient = new IrcClient("irc.freenode.net", 6667); // sito reik Login'e
                _ircClient.ErrorOccurred += (s, msg) =>
                {
                    MessageBox.Show(msg.Message);
                };
                _ircClient.Listen();
            }
            catch (SocketException)
            {
                MessageBox.Show("Failed to connect.");
                this.Close();
            }
            
            catch (Exception)
            {
                MessageBox.Show("Something went wrong.");
                this.Close();
            }
            
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nick.Text) || string.IsNullOrWhiteSpace(user.Text))
            {
                MessageBox.Show("Invalid input");
                return;
            }
            
            if(!_ircClient.Connect(nick.Text, user.Text))
            {
                return;
            }
            // tarkim cia nepavyksta prisijungt (pvz nickas jau naudojamas)
            // tada issimes IrcExcpetion
            // betdel to sustos tas Listener metodas, vadinasi nustos musu visas zinuciu gaudymas is serverio
            // todel jau toliau bandant tarkim is naujo jungtis nieks neveiks, nes tiesiog neturesim zinuciu is servo
            MainWindow mw = new MainWindow(_ircClient); // cia perduodam ta pati objekta main langui. sitas klientas jau pasijunges, tai ten nebereiks
            mw.Show();
            this.Close();
            
        }
    }
}
