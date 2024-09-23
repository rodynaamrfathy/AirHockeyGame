using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace AirHockeyGame
{
    class Display
    {
        public Canvas HockeyTable;

        public void UpdatePaddleDisplay(Paddel paddle)
        {
            // Update player one paddle position
            Canvas.SetLeft(paddle.PaddelDrawingShape, paddle.Position.X);
            Canvas.SetTop(paddle.PaddelDrawingShape, paddle.Position.Y);
        }

        public void UpdatePuckDisplay(Vector2 puckPos)
        {
            // Update puck position
            //Canvas.SetLeft();
            //Canvas.SetTop();
        }

        public void UpdateGameCanvas(Status gameStatus, Canvas paddleTwoDrawingShape, Ellipse puckShape)
        {
            // Update player two paddle position
            Canvas.SetRight(paddleTwoDrawingShape, gameStatus.PaddlePosition.X);
            Canvas.SetBottom(paddleTwoDrawingShape, gameStatus.PaddlePosition.Y);

            // Update puck position
            Canvas.SetRight(puckShape, gameStatus.PuckPosition.X);
            Canvas.SetBottom(puckShape, gameStatus.PuckPosition.Y);
        }

        public void DisplayScore()
        {

        }

    }
}