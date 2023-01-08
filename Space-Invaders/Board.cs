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
        public List<Bullet> SpaceshipBullets { get; set; }
        public List<Bullet> AlienBullets { get; set; }

        public void Setup()
        {
            Spaceship.PlayerController.PlayerShot += NewSpaceshipBullet;

        }

        public void NewSpaceshipBullet(object sender, NewBulletEventArgs e)
        {
            SpaceshipBullets.Add(e.NewBullet);
        }
    }
}
