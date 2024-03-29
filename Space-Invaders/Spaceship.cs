﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Space_Invaders
{
    public class Spaceship : GameEntity, MovingEntity, ShootingEntity, Controllable
    {
        public int Speed { get; set; }
        public Direction MoveDirection { get; set; }

        public Direction DirectionOfProjectile { get; }

        public PlayerController PlayerController { get; }

        public int ShootingCooldownMiliseconds { get; set; }
        public bool CanShoot { get; set; }
        public Timer shootingCooldownTimer { get; set; }
        public bool ControllingEnabled { get; set; }

        private const int INITIAL_SPEED = 5;
        private const int SHOOTING_COOLDOWN_MILISECONDS = 500;

        public Spaceship(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = INITIAL_SPEED;
            DirectionOfProjectile = Direction.Up;

            ShootingCooldownMiliseconds = SHOOTING_COOLDOWN_MILISECONDS;

            shootingCooldownTimer = new Timer()
            {
                Interval = ShootingCooldownMiliseconds
            };
            shootingCooldownTimer.Elapsed += OnShootingCooldownTimerElapsed;
            CanShoot = true;


            // Temporary hard coded fixed value + 9
            PlayerController = new PlayerController(this, 0, MainWindow.WINDOW_WIDTH - Width - MainWindow.WEIRD_WINDOW_RIGHT_BORDER_DISPLACEMENT_AMOUNT + 9);
        }

        public void Move()
        {
            X += PlayerController.HandleMoveInput();
        }

        public int XAfterMove(Direction direction)
        {
            return X + Speed * (int)direction;
        }

        public bool Shoot()
        {
            return PlayerController.HandleShootingInput();
        }

        public void OnShootingCooldownTimerElapsed(object sender, EventArgs e)
        {
            CanShoot = true;
        }
    }
}
