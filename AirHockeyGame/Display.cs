using SlimDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        public void UpdateGameCanvas(Status gameStatus, Canvas paddel2DrawingShape)
        {
            // Update player two paddle position
            Canvas.SetRight(paddel2DrawingShape, gameStatus.PaddlePosition.X);
            Canvas.SetBottom(paddel2DrawingShape, gameStatus.PaddlePosition.Y);

            // Update puck position
            //Canvas.SetLeft();
            //Canvas.SetTop();
        }

        public void DisplayScore()
        {

        }

    }
}