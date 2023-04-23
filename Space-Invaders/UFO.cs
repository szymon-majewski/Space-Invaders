using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders
{
    public class UFO : GameEntity, MovingEntity
    {
        public int Speed { get; set; }
        public Direction MoveDirection { get; set; }

        private const int INITIAL_SPEED = 2;

        public UFO(int x, int y, int width, int height, Direction moveDirection)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            MoveDirection = moveDirection;
            Speed = INITIAL_SPEED;
        }

        public void Move()
        {
            X += Speed * (int)MoveDirection;
        }
    }
}
