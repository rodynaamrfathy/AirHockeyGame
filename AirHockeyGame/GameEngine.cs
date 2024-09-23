using System.Diagnostics;
using System.Windows.Automation;
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

        public void UpdateGame(float canvasHeight, float canvasWidth)
        {
            CheckCollision(canvasHeight, canvasWidth);
        }


        public void CheckCollision(float canvasHeight, float canvasWidth)
        {
            puck.BoundaryCollision(canvasHeight, canvasWidth);
            if (puck.CheckPadelCollision(player.Paddel))
            {
                Debug.WriteLine("collided!!!!!!");
                puck.ResolveCollision(player.Paddel);
            }
            else
            {
                Debug.WriteLine("did not collide!!!!!!");

            }
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
        public void CheckForGoal()
        {
            player.UpdateScore(puck.Position, puck.Radius);
            ResetGameAfterGoal();
        }

        private void ResetGameAfterGoal()
        {
            puck.FaceOff();
            // Reset paddles to starting positions 
        }
    }
}