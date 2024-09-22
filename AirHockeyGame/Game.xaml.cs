using SlimDX.XACT3;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SlimDX;

namespace AirHockeyGame
{
    public partial class Game : Window
    {
        private TcpClient client;
        private NetworkStream stream;
        private GameEngine gameEngine;
        private Display gameDisplay;
        private bool isMouseDown;
        private IEncryption encryption;
        private Point initialMousePosition;
        private DateTime initialMouseDownTime;
        private Stopwatch stopwatch;

        public Game(TcpClient client, NetworkStream stream, string username, string encryptionType)
        {
            InitializeComponent();
            this.client = client;
            this.stream = stream;

            // Select the encryption type (AES, DES, RSA)
            encryption = EncryptionFactory.GetEncryptionAlgorithm(encryptionType);

            // Initialize the game components
            InitializeGameComponents(username);

            // Start the networking tasks
            StartGameNetworking();
        }

        private void InitializeGameComponents(string username)
        {
            Paddel paddle = new Paddel(PaddleOneCanvas);
            Goal goal = new Goal();
            var puckShape = Puck;

            gameDisplay = new Display { HockeyTable = HockeyCanvas };
            gameEngine = new GameEngine(paddle, goal, puckShape, username);
        }

        private void StartGameNetworking()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Decrypt the received message
                    string receivedMessage = encryption.Decrypt(encryptedMessage);

                    // Handle the received message (like player movements, game state updates, etc.)
                    Status gameStatus = Status.FromJson(receivedMessage);
                    Dispatcher.Invoke(() =>
                    {
                        gameDisplay.UpdateGameCanvas(gameStatus);
                    });
                }
            });
        }

        private void Paddle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
            gameEngine.player.Paddel.PaddelDrawingShape.CaptureMouse();
            initialMousePosition = e.GetPosition(HockeyCanvas);
            initialMouseDownTime = DateTime.Now;
            stopwatch.Reset();
            Task.Run(() => SendGameUpdates());
        }

        private void paddle_mouseleftbuttonup(object sender, MouseButtonEventArgs e)
        {
            if (isMouseDown)
            {
                isMouseDown = false;
                gameEngine.player.Paddel.PaddelDrawingShape.ReleaseMouseCapture();
            }
        }

        private void PaddelCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                var mousePos = e.GetPosition(HockeyCanvas);

                // Calculate the new position of the paddle
                double deltaX = mousePos.X - initialMousePosition.X;
                double deltaY = mousePos.Y - initialMousePosition.Y;

                // Update paddle position based on mouse movement
                gameEngine.player.Paddel.Position = new Vector2(
                    (float) (gameEngine.player.Paddel.Position.X + deltaX),
                    (float)(gameEngine.player.Paddel.Position.Y + deltaY)
                );

                // Update the visual representation of the paddle
                gameDisplay.UpdatePaddleDisplay(gameEngine.player.Paddel.Position);

                // Update the initial mouse position for the next move event
                initialMousePosition = mousePos;
            }
        }

        private void SendGameUpdates()
        {
            while (isMouseDown)
            {
                // Send the updated status to the server
                Status status = gameEngine.GenerateStatus();
                string statusJson = status.ToJson();

                // Encrypt the status before sending
                string encryptedStatus = encryption.Encrypt(statusJson);
                byte[] statusBytes = Encoding.UTF8.GetBytes(encryptedStatus);
                stream.Write(statusBytes, 0, statusBytes.Length);

                // Delay between updates (e.g., 50 milliseconds for smooth updates)
                Task.Delay(50).Wait();
            }
        }

        public void GameLoop()
        {
            //each 60fps it should recheck for the position of
            //the paddel and the force applied on it by the mouse
            //and then update on the screen display
            //and send game update

            //3yza el send update w el display y7slo parrallel lb3d msh wara b3d

            //Start sending updates continuously while the mouse is down
        }
    }
}