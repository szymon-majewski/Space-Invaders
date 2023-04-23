using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders
{
    public class NewBulletEventArgs : EventArgs
    {
        public Bullet NewBullet { get; set; }

        public NewBulletEventArgs(Bullet newBullet)
        {
            NewBullet = newBullet;
        }
    }
}
