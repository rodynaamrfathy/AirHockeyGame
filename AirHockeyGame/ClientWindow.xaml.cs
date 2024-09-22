using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AirHockeyGame
{
    public partial class ClientWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;
        private UdpClient udpClient;
        private const int BroadcastPort = 34534;
        private string clientName;

        public ClientWindow()
        {
            InitializeComponent();
            StartListeningForBroadcasts();
        }

        private void StartListeningForBroadcasts()
        {
            try
            {
                udpClient = new UdpClient(BroadcastPort);
                Task.Run(() =>
                {
                    while (true)
                    {
                        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, BroadcastPort);
                        byte[] data = udpClient.Receive(ref remoteEndPoint);
                        string message = Encoding.UTF8.GetString(data);

                        // Check if the received message does not already exist in the HostListBox
                        bool messageExists = false;

                        // Using Dispatcher to access the UI element from a non-UI thread
                        Dispatcher.Invoke(() =>
                        {
                            foreach (var item in HostListBox.Items)
                            {
                                if (item.ToString() == message)
                                {
                                    messageExists = true;
                                    break;
                                }
                            }

                            // If the message doesn't exist, add it to the HostListBox
                            if (!messageExists)
                            {
                                HostListBox.Items.Add(message);
                            }
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void HostListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayHosts.Visibility = Visibility.Hidden;
            GetClientName.Visibility = Visibility.Visible;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            clientName = ClientNameBox.Text;
            if (string.IsNullOrEmpty(clientName))
                return;

            string selectedHost = HostListBox.SelectedItem.ToString();
            string hostAddress = selectedHost.Split(new string[] { "Available at " }, StringSplitOptions.None)[1].Trim();

            client = new TcpClient();
            client.Connect(hostAddress, 5000);
            stream = client.GetStream();

            // Transition to the Game window and pass client and stream
            Game game = new Game(client, stream, clientName, "AES");
            game.Show();
            this.Close();
        }

        // Ensure cleanup when the client window is closed
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            udpClient?.Close(); // Close the UDP client for receiving broadcasts
            //ensures that any existing connection between the host and client is properly closed.
            client?.Close();     // Close the TCP client connection if open
        }
    }
}