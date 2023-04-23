using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders
{
    public interface ShootingEntity
    {
        // Direction of projectile
        public Direction DirectionOfProjectile { get; }

        public int ShootingCooldownMiliseconds { get; set; }
        public bool CanShoot { get; set; }
        public Timer shootingCooldownTimer { get; set; }

        public bool Shoot();
    }
}
