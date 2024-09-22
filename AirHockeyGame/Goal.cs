using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using Vector2 = SlimDX.Vector2;


namespace AirHockeyGame
{
    class Goal
    {
        public Vector2 Min;
        public Vector2 Max;

        public Goal()
        {
            Min = new Vector2(100, 565); // Bottom-left corner
            Max = new Vector2(200, 585); // Top-right corner
        }

        public bool isGoalScored(Vector2 puckPosition, float puckRadius)
        {
            // check if the puck is inside the area of the goal
            bool isPuckXInGoal = puckPosition.X - puckRadius <= Max.X && puckPosition.X + puckRadius >= Min.X;
            bool isPuckYInGoal = puckPosition.Y >= Min.Y && puckPosition.Y <= Max.Y;

            return isPuckXInGoal && isPuckYInGoal;
        }
    }
}