using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Space_Invaders
{
    internal class PlayerController
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

        public void HandleMoveInput()
        {
            player.MoveDirection = Direction.None;

            if (Keyboard.IsKeyDown(Key.Left) && !(player.XAfterMove(Direction.Left) < leftBorder))
            {
                player.MoveDirection += (int)Direction.Left;
            }

            if (Keyboard.IsKeyDown(Key.Right) && !(player.XAfterMove(Direction.Right) > rightBorder))
            {
                player.MoveDirection += (int)Direction.Right;
            }

            player.X += (int)player.MoveDirection * player.Speed;
        }

        public delegate void EventHandler(object sender, NewBulletEventArgs e);
        public event EventHandler PlayerShot;

        public void HandleShootingInput()
        {
            // Add cooldown
            if (Keyboard.IsKeyDown(Key.Space))
            {
                PlayerShot.Invoke(this, new NewBulletEventArgs(new Bullet(player.X + player.Width / 2,
                               player.Y,
                               Bullet.Source.Spaceship
                               )));
            }
        }
    }
}
