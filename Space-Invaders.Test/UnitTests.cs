using System;
using Space_Invaders;
using NUnit.Framework;

namespace Space_Invaders.Test
{
    [TestFixture]
    public class UnitTests
    {
        [TestCase(1, Direction.Left, ExpectedResult = -1)]
        [TestCase(5, Direction.Right, ExpectedResult = 5)]
        [TestCase(-5, Direction.Right, ExpectedResult = -5)]
        [TestCase(0, Direction.Right, ExpectedResult = 0)]
        public int AlienHorizontalMoveTest(int speed, Direction dir)
        {
            Alien alien = new Alien(0, 0, Alien.Type.Large);
            alien.Speed = speed;
            alien.MoveDirection = dir;

            alien.Move();

            return alien.X;
        }

        [TestCase(1, Direction.Left, ExpectedResult = -1)]
        [TestCase(5, Direction.Right, ExpectedResult = 5)]
        [TestCase(-5, Direction.Right, ExpectedResult = -5)]
        [TestCase(0, Direction.Right, ExpectedResult = 0)]
        public int SpacesihpHorizontalMoveTest(int speed, Direction dir)
        {
            Spaceship spaceship = new Spaceship(0, 0, 0, 0);
            spaceship.Speed = speed;

            return spaceship.XAfterMove(dir);
        }

        [TestCase(100, Direction.Down, Bullet.Source.Alien, ExpectedResult = 100)]
        [TestCase(10, Direction.Up, Bullet.Source.Spaceship, ExpectedResult = -10)]
        [TestCase(0, Direction.Up, Bullet.Source.Spaceship, ExpectedResult = 0)]
        [TestCase(-10, Direction.Up, Bullet.Source.Spaceship, ExpectedResult = 10)]
        public int BulletVerticalMoveTest(int speed, Direction dir, Bullet.Source source)
        {
            Bullet bullet = new Bullet(0, 0, source);
            bullet.Speed = speed;
            bullet.MoveDirection = dir;

            bullet.Move();

            return bullet.Y;
        }

        [Test, Combinatorial]
        public void AlienYAfterAdvancingToNextRow(
            [Values(100, 4, 0, -90)] int amount,
            [Values(Alien.Type.Large, Alien.Type.Medium, Alien.Type.Small)] Alien.Type type)
        {
            Alien alien = new Alien(0, 0, type);
            int initialY = alien.Y;

            alien.GetCloserToEarth(amount);

            Assert.AreEqual(initialY + amount, alien.Y);
        }

        [Test]
        public void UfoNullAtStart()
        {
            SpaceInvaders spaceInvaders = new SpaceInvaders(new Board());
            spaceInvaders.Setup();

            Assert.Null(spaceInvaders.Board.UFO);
        }
    }
}
