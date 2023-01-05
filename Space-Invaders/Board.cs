using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders
{
    internal class Board
    {
        public List<List<Alien>> Aliens { get; set; }
        public Spaceship Spaceship { get; set; }
        public List<Bullet> Bullets { get; set; }
    }
}
