using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AirHockeyGame
{
    // renders 
    class Display
    {
        public Canvas HockeyTable;

        public void UpdateGameCanvas(Vector2 paddlePos, Vector2 puckPos)
        {
            // Update player one paddle position
            Canvas.SetLeft();
            Canvas.SetTop();

            // Update puck position
            Canvas.SetLeft();
            Canvas.SetTop();
        }

        public void UpdateGameCanvas(Status gameStatus)
        {
            // Update player two paddle position
            Canvas.SetLeft();
            Canvas.SetTop();

            // Update puck position
            Canvas.SetLeft();
            Canvas.SetTop();
        }

        public void DisplayScore()
        {

        }

    }
}