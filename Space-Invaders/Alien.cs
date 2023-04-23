using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Space_Invaders
{
    public class Alien : GameEntity, MovingEntity, ShootingEntity
    {
        public int Speed { get; set; }
        public Direction MoveDirection { get; set; }

        public Direction DirectionOfProjectile { get; }
        public int ShootingCooldownMiliseconds { get; set; }

        public bool CanShoot { get; set; }
        public Timer shootingCooldownTimer { get; set; }

        public const int SHOOTING_COOLDOWN_LOW_BORDER = 5000;
        public const int SHOOTING_COOLDOWN_UP_BORDER = 25000;

        private static Random random = new Random();

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

            ShootingCooldownMiliseconds = random.Next(SHOOTING_COOLDOWN_LOW_BORDER, SHOOTING_COOLDOWN_UP_BORDER);

            shootingCooldownTimer = new Timer()
            {
                Interval = ShootingCooldownMiliseconds
            };
            shootingCooldownTimer.Elapsed += OnShootingCooldownTimerElapsed;
            shootingCooldownTimer.Start();
            CanShoot = false;
        }

        // Incerases Y
        public void GetCloserToEarth(int amount)
        {
            Y += amount;
        }

        internal delegate void EventHandler(object sender, NewBulletEventArgs e);
        internal event EventHandler AlienShot;

        public bool Shoot()
        {
            if (!CanShoot)
            {
                return false;
            }

            CanShoot = false;

            shootingCooldownTimer.Stop();
            shootingCooldownTimer.Start();

            AlienShot.Invoke(this, new NewBulletEventArgs(new Bullet(X + Width / 2,
                                                          Y + Height,
                                                          Bullet.Source.Alien
                                                          )));
            return true;
        }

        private void OnShootingCooldownTimerElapsed(object sender, EventArgs e)
        {
            CanShoot = true;
        }
    }
}
