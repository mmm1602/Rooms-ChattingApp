using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.MVVM.View;
using ChatClient.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<ContactModel> Contacts { get; set; }


        private ContactModel _selectedContact;

        public ContactModel SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                _selectedContact = value;
                OnPropertyChanged();
            }
        }


        private string _messageInBox;

        public string Message
        {
            get { return _messageInBox; }
            set
            {
                _messageInBox = value;
                OnPropertyChanged();
            }
        }

        private string _username;

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string PFP_Source { get; set; }



        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand SendCommand { get; set; }
        public RelayCommand SetPictureCommand { get; set; }
        public RelayCommand AddContactCommand { get; set; }


        private readonly Server _server;
        public MainViewModel()
        {

            PFP_Source = "https://cdn.mos.cms.futurecdn.net/yCPyoZDQBBcXikqxkeW2jJ-1024-80.jpg";

            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<MessageModel>();
            Contacts = new ObservableCollection<ContactModel>();

            _server = new Server();
            _server.ConnectedEvent += UserConnected;
            _server.MsgRecievedEvent += MsgReceieved;
            _server.DisconnectedEvent += UserDisconnect;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));


            Contacts.Add(
                new ContactModel
                {
                    Username = "TestContact",
                    ImageSource = "https://cdn.mos.cms.futurecdn.net/yCPyoZDQBBcXikqxkeW2jJ-1024-80.jpg"
                }
                );
            Messages.Add(
                new MessageModel
                {
                    Username = "TestContact",
                    ImageSource = "https://cdn.mos.cms.futurecdn.net/yCPyoZDQBBcXikqxkeW2jJ-1024-80.jpg",
                    Message = "Whats Up?",
                    UsernameColor = "#00FF00",
                    IsNative = false,
                    FirstMessage = true,
                    Time = DateTime.Now
                }
                );
            SendCommand = new RelayCommand(o =>
            {
                SendMessageCommand = new RelayCommand(o => _server.SendMsgToServer(Message), o => !string.IsNullOrEmpty(Message));

                Message = "";
            });

            
        }

        private void UserDisconnect()
        {
            var UID = _server.PackReader.ReadMsg();
            var user = Users.Where(x => x.UID == UID).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MsgReceieved()
        {
            var msg = _server.PackReader.ReadMsg();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(
                new MessageModel
                {
                    Username = "DAMN",
                    ImageSource = "https://avatarfiles.alphacoders.com/293/293215.jpg",
                    Message = "Hello",
                    UsernameColor = "#00FF00",
                    IsNative = false,
                    FirstMessage = true,
                    Time = DateTime.Now
                }
                ));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PackReader.ReadMsg(),
                UID = _server.PackReader.ReadMsg(),
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
