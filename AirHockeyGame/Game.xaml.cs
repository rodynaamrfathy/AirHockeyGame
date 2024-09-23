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
        private Stopwatch stopwatch = new Stopwatch();

        public Game(TcpClient client, NetworkStream stream, string username, string encryptionType)
        {
            InitializeComponent();
            this.client = client;
            this.stream = stream;

            encryption = EncryptionFactory.GetEncryptionAlgorithm(encryptionType);
            InitializeGameComponents(username);

            // Start the networking tasks
            Task.Run(() => StartGameNetworking());
            Task.Run(() => SendGameUpdates());

            // Start the game loop
            Task.Run(() => GameLoop());
        }

        private void StartGameNetworking()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // Connection has been closed
                    string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.WriteLine($"Received Encrypted Message: {encryptedMessage}");

                    // Decrypt the received message
                    string receivedMessage = encryption.Decrypt(encryptedMessage);
                    Debug.WriteLine($"Decrypted Message: {receivedMessage}");

                    // Handle the received message
                    Status gameStatus = Status.FromJson(receivedMessage);
                    if (gameStatus != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            gameDisplay.UpdateGameCanvas(gameStatus, PaddleTwoCanvas, Puck);
                        });
                    }
                    else
                    {
                        Debug.WriteLine("Game status is null after deserialization.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in StartGameNetworking: {ex.Message}");
                    break; // Exit loop on error
                }
            }
        }

        private void SendGameUpdates()
        {
            while (true)
            {
                try
                {
                    Status status = gameEngine.GenerateStatus();
                    string statusJson = status.ToJson();

                    // Encrypt the status before sending
                    string encryptedStatus = encryption.Encrypt(statusJson);
                    byte[] statusBytes = Encoding.UTF8.GetBytes(encryptedStatus);
                    stream.Write(statusBytes, 0, statusBytes.Length);

                    // Delay between updates
                    Task.Delay(50).Wait();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in SendGameUpdates: {ex.Message}");
                    break; // Exit loop on error
                }
            }
        }


        private void InitializeGameComponents(string username)
        {
            Paddel paddle = new Paddel(PaddleOneCanvas);
            Goal goal = new Goal();
            var puckShape = Puck;

            gameDisplay = new Display { HockeyTable = HockeyCanvas };
            gameEngine = new GameEngine(paddle, goal, puckShape, username);
            gameDisplay.HockeyTable.MouseMove += PaddelCanvas_MouseMove;
            gameEngine.player.Paddel.PaddelDrawingShape.MouseLeftButtonDown += Paddle_MouseLeftButtonDown;
            gameEngine.player.Paddel.PaddelDrawingShape.MouseLeftButtonUp += paddle_mouseleftbuttonup;
        }

        private void Paddle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
            gameEngine.player.Paddel.PaddelDrawingShape.CaptureMouse();
            initialMousePosition = e.GetPosition(HockeyCanvas);
            initialMouseDownTime = DateTime.Now;
            stopwatch.Reset();
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
                    (float)(gameEngine.player.Paddel.Position.X + deltaX),
                    (float)(gameEngine.player.Paddel.Position.Y + deltaY)
                );

                // Restrict movement to stay within canvas bounds
                gameEngine.player.Paddel.RestrictMovement((float) HockeyCanvas.ActualHeight, (float)HockeyCanvas.ActualWidth);

                // Update the visual representation of the paddle
                gameDisplay.UpdatePaddleDisplay(gameEngine.player.Paddel);

                // Update the initial mouse position for the next move event
                initialMousePosition = mousePos;
            }
        }


        public void GameLoop()
        {
            Stopwatch frameStopwatch = new Stopwatch();
            frameStopwatch.Start();
            gameEngine.puck.FaceOff();
            while (!gameEngine.GameOver)
            {
                // Update the game state
                gameEngine.UpdateGame((float) HockeyCanvas.ActualHeight, (float) HockeyCanvas.ActualWidth); // Call to update game logic and check for collisions
                gameDisplay.UpdatePuckDisplay(gameEngine.puck);
                // Render the game display
                Dispatcher.Invoke(() =>
                {
                    gameDisplay.UpdateGameCanvas(gameEngine.GenerateStatus(), PaddleTwoCanvas, Puck);
                });

                // Control frame rate (e.g., 60 FPS)
                while (frameStopwatch.ElapsedMilliseconds < 16) // ~60 FPS
                {
                    // Wait
                }
                frameStopwatch.Restart();
            }
        }
public void GameLoop()
{
    Stopwatch frameStopwatch = new Stopwatch();
    frameStopwatch.Start();
    gameEngine.puck.FaceOff();
    
    while (!gameEngine.GameOver)
    {
        float canvasHeight = (float)HockeyCanvas.ActualHeight;
        float canvasWidth = (float)HockeyCanvas.ActualWidth;

        // Update the game state
        gameEngine.UpdateGame(canvasHeight, canvasWidth);

        // Check for collisions with the paddle
        if (gameEngine.puck.CheckPaddleCollision(gameEngine.PlayerPaddle))
        {
            gameEngine.puck.ResolveCollision(gameEngine.PlayerPaddle);
        }

        // Update puck position
        gameEngine.puck.UpdatePosition(0.016f); // 60 FPS -> deltaTime ~ 16ms

        // Update UI on the main thread
        Dispatcher.Invoke(() =>
        {
            gameDisplay.UpdateGameCanvas(gameEngine.GenerateStatus(), PaddleTwoCanvas, Puck);
        });

        // Control frame rate (~60 FPS)
        Task.Delay(16).Wait();
    }
}

    }
}