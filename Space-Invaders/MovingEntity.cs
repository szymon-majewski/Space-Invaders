using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders
{
    public interface MovingEntity
    {
        // Amount of units entity moves at once
        public int Speed { get; set; }

        // Move direction
        public Direction MoveDirection { get; set; }

        // Change entity position.
        public void Move();
    }
}
