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
            _ircClient = new IrcClient("irc.freenode.net", 6667); // sito reik Login'e
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nick.Text) || string.IsNullOrWhiteSpace(user.Text))
            {
                MessageBox.Show("Invalid input");
                return;
            }
            _ircClient.Connect(nick.Text, user.Text); // connectas jau yra
            MainWindow mw = new MainWindow(_ircClient); // cia perduodam ta pati objekta main langui. sitas klientas jau pasijunges, tai ten nebereiks
            mw.Show();
            this.Close();
        }
    }
}
