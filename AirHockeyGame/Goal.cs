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

        public Goal(Vector3 max, Vector3 min)
        {
            Max = max; // ex (100,0)
            Min = min; // ex (200, 20)
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