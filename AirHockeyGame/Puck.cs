using SlimDX;
using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using Vector2 = SlimDX.Vector2;

namespace AirHockeyGame
{
    class Puck : GameObject
    {
        public float BouncingFactor;
        public Shape PuckDrawingShape;

        public Puck(Shape puckDrawingShape)
        {
            Position = new Vector2(130, 280); ; // Start position of table
            BouncingFactor = 0.7f;
            PuckDrawingShape = puckDrawingShape;
            Mass = 0.077f;
        }

        public override void FaceOff()
        {
            // Reset position to the start of the table
            Position = new Vector2(130, 280);
            Canvas.SetTop(PuckDrawingShape, Position.Y);
            Canvas.SetLeft(PuckDrawingShape, Position.X);
        }

        public void UpdatePosition(float deltaTime)
        {
            // Update position based on velocity and deltaTime
            Position += Velocity * deltaTime;

            // Update the UI representation of the puck
            Canvas.SetTop(PuckDrawingShape, Position.Y);
            Canvas.SetLeft(PuckDrawingShape, Position.X);
        }

        public void BoundaryCollision(float canvasHeight, float canvasWidth)
        {
            // Check top boundary collision
            if (Position.Y - Radius <= 0)
            {
                Position = new Vector2(Position.X, Radius);
                Velocity = new Vector2(Velocity.X, -Velocity.Y * BouncingFactor);
            }
            // Check bottom boundary collision
            else if (Position.Y + Radius >= canvasHeight)
            {
                Position = new Vector2(Position.X, canvasHeight - Radius);
                Velocity = new Vector2(Velocity.X, -Velocity.Y * BouncingFactor);

                // Stop puck if velocity is low
                if (Math.Abs(Velocity.Y) < 1 && Math.Abs(Velocity.X) < 1)
                {
                    IsMoving = false;
                    Velocity = Vector2.Zero;
                }
            }

            // Check left boundary collision
            if (Position.X - Radius <= 0)
            {
                Position = new Vector2(Radius, Position.Y);
                Velocity = new Vector2(-Velocity.X * BouncingFactor, Velocity.Y);
            }
            // Check right boundary collision
            else if (Position.X + Radius >= canvasWidth)
            {
                Position = new Vector2(canvasWidth - Radius, Position.Y);
                Velocity = new Vector2(-Velocity.X * BouncingFactor, Velocity.Y);
            }
        }

        public bool CheckPadelCollision(Paddel paddel)
        {
            if (paddel == null) return false;

            // Compute the center of the puck and the paddle
            float XCenterPuck = this.Position.X;
            float YCenterPuck = this.Position.Y;
            float XCenterPaddle = paddel.Position.X;
            float YCenterPaddle = paddel.Position.Y;

            float diffX = XCenterPuck - XCenterPaddle;
            float diffY = YCenterPuck - YCenterPaddle;
            float distance = (float)Math.Sqrt(diffX * diffX + diffY * diffY);
            float r1 = this.Radius;
            float r2 = paddel.Radius;

            return distance <= (r1 + r2);
        }

        public void ResolveCollision(Paddel paddel)
        {
            // Calculate normal and tangent vectors
            Vector2 normal = new Vector2(this.Position.X - paddel.Position.X, this.Position.Y - paddel.Position.Y);
            float normNormal = normal.Length();
            Vector2 unitNormal = normal / normNormal;
            Vector2 unitTangent = new Vector2(-unitNormal.Y, unitNormal.X);

            // Calculate normal and tangential velocities
            float vPaddleNormal = Vector2.Dot(unitNormal, paddel.Velocity);
            float vPaddleTangent = Vector2.Dot(unitTangent, paddel.Velocity);
            float vPuckNormal = Vector2.Dot(unitNormal, this.Velocity);
            float vPuckTangent = Vector2.Dot(unitTangent, this.Velocity);

            // Resolve velocities after the collision
            float massSum = this.Mass + paddel.Mass;
            float massDiff = paddel.Mass - this.Mass;

            float vPaddleNormalAfter = (vPaddleNormal * massDiff + 2 * this.Mass * vPuckNormal) / massSum;
            float vPuckNormalAfter = (vPuckNormal * -massDiff + 2 * paddel.Mass * vPaddleNormal) / massSum;

            // Convert scalar velocities back to vectors
            Vector2 vPaddleNormalAfterVector = unitNormal * vPaddleNormalAfter;
            Vector2 vPuckNormalAfterVector = unitNormal * vPuckNormalAfter;
            Vector2 vPaddleTangentVector = unitTangent * vPaddleTangent;
            Vector2 vPuckTangentVector = unitTangent * vPuckTangent;

            // Update velocities
            this.Velocity = vPuckNormalAfterVector + vPuckTangentVector;
            paddel.Velocity = vPaddleNormalAfterVector + vPaddleTangentVector;
        }
    }
}