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
    class Player
    {
        public Paddel Paddel;
        public int PlayerScore;
        public Goal Goal;
        public string Username;


        public Player(Paddel Paddel, Goal Goal, string Username)
        {
            this.Paddel = Paddel;
            PlayerScore = 0;
            this.Goal = Goal;
            this.Username = Username;
        }

        public void UpdateScore(Vector2 puckPosition, float puckRadius)
        {
            if (Goal.isGoalScored(puckPosition, puckRadius))
                PlayerScore++;
        }
        public void MovePaddel(Vector2 direction)
        {

        }

    }
}