using System.Windows.Shapes;

namespace AirHockeyGame
{
    class GameEngine
    {
        public Puck puck;
        public Player player;
        public bool GameOver = false;

        public GameEngine(Paddel paddel, Goal goal, Shape puckShape, string username)
        {
            player = new Player(paddel, goal, username);
            puck = new Puck(puckShape);
        }

        public void UpdateGame()
        {
            puck.UpdatePosition(1 / 60f); // Assuming a 60 FPS update, adjust accordingly
            CheckCollision();
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
                player.PlayerScore
            );
        }

        public void UpdateGame(Status gameStatus)
        {
            puck.Position = gameStatus.PuckPosition;

            // Update visual positions if needed
        }
    }
}