using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using Vector2 = SlimDX.Vector2;


namespace AirHockeyGame
{
    class GameObject
    {
        public bool IsMoving;
        public Vector2 Position;
        public Vector2 Velocity = new Vector2(0, 0);
        public float Mass;
        public float Radius;

        public virtual void FaceOff()
        {

        }
    }
}