using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders
{
    internal class Alien : Renderable, MovingEntity
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; }
        public int Height { get; }
        public int Speed { get; set; }
        public int? MoveCooldown { get; set; }
        public Direction MoveDirection { get; set; }

        public void Render()
        {
            throw new NotImplementedException();
        }

        public void Move()
        {
            X += Speed * (int)MoveDirection;
        }

        public int XAfterMove()
        {
            return X + Speed * (int)MoveDirection;
        }

        public enum Type
        {
            Small,
            Medium,
            Large
        }

        public static Dictionary<Type, int> AlienTypeWidth = new Dictionary<Type, int>
        {
            { Type.Small, 24 },
            { Type.Medium, 33 },
            { Type.Large, 36 }
        };

        public static int AlienHeight = 24;
        public static int InitialSpeed = 20;

        public Type TypeSize;

        public Alien (int x, int y, Type type)
        {
            X = x;
            Y = y;
            TypeSize = type;
            Width = AlienTypeWidth[TypeSize];
            Height = AlienHeight;
            Speed = InitialSpeed;
            MoveDirection = Direction.Right;
        }

        // Incerases Y. Returns true if an alien is on earth level.
        public void GetCloserToEarth(int amount)
        {
            Y += amount;

            // Check game over
        }
    }
}
