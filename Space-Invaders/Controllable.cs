using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders
{
    internal interface Controllable
    {
        public PlayerController PlayerController { get; }

        public int XAfterMove(Direction direction);
    }
}
