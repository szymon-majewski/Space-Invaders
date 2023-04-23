using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Space_Invaders
{
    public class PlayerController
    {
        Spaceship player;
        int rightBorder;
        int leftBorder;

        public PlayerController(Spaceship spaceship, int leftBorder, int rightBorder)
        {
            player = spaceship;
            this.rightBorder = rightBorder;
            this.leftBorder = leftBorder;
        }

        public int HandleMoveInput()
        {
            if (!player.ControllingEnabled)
            {
                return 0;
            }

            player.MoveDirection = Direction.None;

            if (Keyboard.IsKeyDown(Key.Left) && !(player.XAfterMove(Direction.Left) < leftBorder))
            {
                player.MoveDirection += (int)Direction.Left;
            }

            if (Keyboard.IsKeyDown(Key.Right) && !(player.XAfterMove(Direction.Right) > rightBorder))
            {
                player.MoveDirection += (int)Direction.Right;
            }

            return (int)player.MoveDirection * player.Speed;
        }

        public delegate void EventHandler(object sender, NewBulletEventArgs e);
        public event EventHandler PlayerShot;

        public bool HandleShootingInput()
        {
            if (!player.ControllingEnabled)
            {
                return false; ;
            }

            // Add cooldown
            if (Keyboard.IsKeyDown(Key.Space) && player.CanShoot)
            {
                player.CanShoot = false;

                // Reseting timer
                player.shootingCooldownTimer.Stop();
                player.shootingCooldownTimer.Start();

                PlayerShot.Invoke(this, new NewBulletEventArgs(new Bullet(player.X + player.Width / 2,
                                                               player.Y,
                                                               Bullet.Source.Spaceship
                                                               )));
                return true;
            }

            return false;
        }
    }
}
