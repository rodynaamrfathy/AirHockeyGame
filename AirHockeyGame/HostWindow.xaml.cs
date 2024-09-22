using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace AirHockeyGame
{
    public partial class HostWindow : Window
    {
        public TcpListener server = null;
        public TcpClient client;
        public NetworkStream stream;
        public string hostName;
        public UdpClient udpClient;
        public const int bufferSize = 1024;
        public byte[] Buffer = new byte[bufferSize];
        private int PORT_NUM = 5000;
        private bool IsConnected = false;
        private const int BroadcastInterval = 5000;
        private bool shouldBroadcast = true;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public HostWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            hostName = HostNameBox.Text;
            if (string.IsNullOrEmpty(hostName))
            {
                hostName = "Unknown";
                return;
            }
            WelcomeText.Text = $"Welcome, {hostName}";
            StartHosting();
            GetHostName.Visibility = Visibility.Hidden;
            StatusTextBlock.Visibility = Visibility.Visible;
        }

        private void StartHosting()
        {
            server = new TcpListener(IPAddress.Any, PORT_NUM);
            server.Start();
            server.BeginAcceptTcpClient(new AsyncCallback(OnClientConnected), null);
            StartBroadcasting();
        }

        private void StartBroadcasting()
        {
            udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;
            IPEndPoint broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, 34534);
            string message = $"Host: {hostName}, Available at {GetLocalIPAddress()}";
            byte[] data = Encoding.UTF8.GetBytes(message);

            Task.Run(() =>
            {
                while (shouldBroadcast)
                {
                    udpClient.Send(data, data.Length, broadcastEndPoint);
                    Thread.Sleep(BroadcastInterval); // Wait for the interval before broadcasting again
                }
            }, cts.Token);
        }

        private void OnClientConnected(IAsyncResult ar)
        {
            IsConnected = true;
            client = server.EndAcceptTcpClient(ar);
            stream = client.GetStream();
            // Cancel broadcasting
            cts.Cancel();

            Dispatcher.Invoke(() =>
            {
                StatusTextBlock.Text = "Client is connected";

                // Pass the client and stream to the Game window
                Game game = new Game(client, stream, hostName);
                game.Show();

                // Close the host window
                this.Close();
            });
        }

        private string GetLocalIPAddress()
        {
            string hostIP = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostIP);

            foreach (IPAddress address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(address))
                {
                    return address.ToString();
                }
            }

            return "No valid IP address found.";
        }

        // Ensure cleanup when the host window is closed
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            udpClient?.Close(); // Stop broadcasting
            //ensures the TCP server stops listening for new connections.
            server?.Stop();     // Stop the TCP server
            //ensures that any existing connection between the host and client is properly closed.
            client?.Close();     // Close the TCP client connection if open
        }
    }
}