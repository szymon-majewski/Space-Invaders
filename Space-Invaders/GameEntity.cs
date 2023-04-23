using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders
{
    public abstract class GameEntity
    {
        // Position on board
        public int X { get; set; }
        public int Y { get; set; }

        // Size
        public int Width { get; set; }
        public int Height { get; set;  }
    }
}
