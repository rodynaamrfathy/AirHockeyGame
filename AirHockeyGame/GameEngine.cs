using System.Windows.Controls;
using System.Windows.Shapes;
using SlimDX;

namespace AirHockeyGame
{
    class GameEngine
    {
        public Puck puck;
        public Player player;

        public GameEngine(Paddel paddel, Goal goal, Shape puckShape, string username)
        {
            player = new Player(paddel, goal, username);
            puck = new Puck(puckShape);

            // Start the game loop
            //Task.Run(() => GameLoop()); // Run the game loop
        }

        public void UpdateGame()
        {
            // Update game logic, check for collisions, etc.
            CheckCollision();
            // Further game update logic if needed
        }

        public void CheckCollision()
        {
            //puck.BoundaryCollision(/* canvasHeight */, /* canvasWidth */);
            if (puck.CheckPadelCollision(player.Paddel))
                puck.ResolveCollision(player.Paddel);
        }

        public Status GenerateStatus()
        {
            return new Status(
                puck.Position,
                player.Paddel.Position,
                player.PlayerScore, // Adjust if using multiple players
                0 // Placeholder for second player score if needed
            );
        }

        public void UpdateGame(Status gameStatus)
        {
            puck.Position = gameStatus.PuckPosition;
            player.Paddel.Position = gameStatus.PuckPosition;

            // Update visual positions if needed
        }
    }
}