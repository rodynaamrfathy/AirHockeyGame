using SlimDX.XAudio2;
using System;
using System.Linq;
using System.Numerics;
using System.Windows.Controls;
using System.Windows;
using Vector2 = SlimDX.Vector2;

namespace AirHockeyGame
{
    class Paddel : GameObject
    {
        public bool IsMoving = false;
        public Canvas PaddelDrawingShape;

        public Paddel(Canvas paddelShape)
        {
            PaddelDrawingShape = paddelShape;
            Mass = 0.2f; // 200 grams, converted to kg
        }

        public override void FaceOff()
        {
            Canvas.SetTop(PaddelDrawingShape, 345);  // Y position
            Canvas.SetLeft(PaddelDrawingShape, 142); // X position
        }

        // ApplyForce - Calculates the velocity of the paddle based on its movement and time taken
        public void ApplyForce(Point initialPaddelPosition, Point finalPaddelPosition, TimeSpan timeTaken)
        {
            // Calculate velocity based on position change and time taken
            Velocity.X = (float)(finalPaddelPosition.X - initialPaddelPosition.X) / (float)timeTaken.TotalSeconds;
            Velocity.Y = (float)(finalPaddelPosition.Y - initialPaddelPosition.Y) / (float)timeTaken.TotalSeconds;

            Velocity *= 0.95f;
        }

        public void RestrictMovement(float canvasHeight, float canvasWidth)
        {
            if (Position.Y - Radius <= 280)
            {
                Position = new Vector2(Position.X, 280 + Radius);
            }
            else if (Position.Y + Radius >= canvasHeight)
            {
                Position = new Vector2(Position.X, canvasHeight - Radius);
            }
            // Restrict movement to the left boundary
            if (Position.X - Radius <= 0)
            {
                Position = new Vector2(Radius, Position.Y);
            }

            // Restrict movement to the right boundary
            else if (Position.X + Radius >= canvasWidth)
            {
                Position = new Vector2(canvasWidth - Radius, Position.Y);
            }
        }
    }
}