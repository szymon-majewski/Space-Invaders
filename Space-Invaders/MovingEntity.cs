using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders
{
    internal interface MovingEntity : GameEntity
    {
        // Amount of units entity moves at once
        public int Speed { get; set; }

        // Time in miliseconds between each move. Null if entity can move all the time.
        public int? MoveCooldown { get; set; }

        // Move direction
        public Direction MoveDirection { get; set; }

        // Change entity position
        public void Move();
    }
}
