using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Space_Invaders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int WINDOW_WIDTH = 650;
        public const int WINDOW_HEIGHT = 800;

        public const int ALIEN_ADVANCEMENT_RATE = 10;

        internal SpaceInvaders SpaceInvaders;

        internal static Dictionary<Alien.Type, List<ImageBrush>> AliensImages { get; set; } = new Dictionary<Alien.Type, List<ImageBrush>>()
        {
            {
                Alien.Type.Large, new List<ImageBrush>()
                {  
                    new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/AlienLarge1.png")) },
                    new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/AlienLarge2.png")) }
                }
            },
            {
                Alien.Type.Medium, new List<ImageBrush>()
                {
                    new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/AlienMedium1.png")) },
                    new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/AlienMedium2.png")) }
                }
            },
            {
                Alien.Type.Small, new List<ImageBrush>()
                {
                    new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/AlienSmall1.png")) },
                    new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/AlienSmall2.png")) }
                }
            },
        };
        internal static ImageBrush SpaceshipImage = new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/Spaceship.png")) };
        internal static ImageBrush SpaceshipBulletImage = new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/SpaceshipMissile.png")) };
        internal static List<ImageBrush> AlienBulletImages = new List<ImageBrush>()
        {
            new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/AlienMissile1.png")) },
            new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/AlienMissile2.png")) },
            new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/AlienMissile3.png")) },
            new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/AlienMissile4.png")) }

        };

        public Rectangle[,] AliensRectangles = new Rectangle[SpaceInvaders.ALIENS_IN_COLUMN_COUNT, SpaceInvaders.ALIENS_IN_ROW_COUNT];
        public Rectangle SpaceshipRectangle;
        public List<Rectangle> BulletsRectangles = new List<Rectangle>();

        private DispatcherTimer gameTimerAliens;
        private const int aliensUpdatesPerSecond = 2;
        private DispatcherTimer gameTimerStandard;
        private const int standardUpdatesPerSecond = 300;

        private int currentAlienImageIndex;

        private List<Alien> leftmostAliens;
        private List<Alien> rightmostAliens;

        private bool previousTickAliensWentDown;

        public const int WEIRD_WINDOW_RIGHT_BORDER_DISPLACEMENT_AMOUNT = 24;

        public const int UPPER_BORDER_Y = 50;

        public MainWindow()
        {
            InitializeComponent();

            Height = WINDOW_HEIGHT;
            Width = WINDOW_WIDTH;

            SpaceInvaders = new SpaceInvaders(new Board());
            SpaceInvaders.Setup();

            Setup();

            // This variable is for alien animation purposes
            currentAlienImageIndex = 0;

            previousTickAliensWentDown = false;

            // Setup game timers
            gameTimerAliens = new DispatcherTimer();
            gameTimerAliens.Interval = TimeSpan.FromMilliseconds(1000.0 / aliensUpdatesPerSecond);
            gameTimerAliens.Tick += GameTimerAliensTick;

            gameTimerStandard = new DispatcherTimer();
            gameTimerStandard.Interval = TimeSpan.FromMilliseconds(1000.0 / standardUpdatesPerSecond);
            gameTimerStandard.Tick += GameTimerStandardTick;

            gameTimerAliens.Start();
            gameTimerStandard.Start();
        }

        private void GameTimerStandardTick(object sender, EventArgs e)
        {
            Spaceship spaceship = SpaceInvaders.Board.Spaceship;

            // Player move
            spaceship.Move();

            Canvas.SetTop(SpaceshipRectangle, spaceship.Y);
            Canvas.SetLeft(SpaceshipRectangle, spaceship.X);

            // Bullets shooting and movement
            spaceship.Shoot();

            for (int i = 0; i < SpaceInvaders.Board.SpaceshipBullets.Count; ++i)
            {
                Bullet currentBullet = SpaceInvaders.Board.SpaceshipBullets[i];
                currentBullet.Move();

                // Bullet hit the ceiling
                if (currentBullet.Y <= UPPER_BORDER_Y)
                {
                    SpaceInvaders.Board.SpaceshipBullets.Remove(currentBullet);
                    BoardPanel.Children.Remove(BulletsRectangles[i]);
                    BulletsRectangles.RemoveAt(i);

                    continue;
                }

                Canvas.SetTop(BulletsRectangles[i], currentBullet.Y);
                Canvas.SetLeft(BulletsRectangles[i], currentBullet.X);
            }

            // Collisions
        }

        private void GameTimerAliensTick(object sender, EventArgs e)
        {
            bool thisTickAliensGoDown = 
                !previousTickAliensWentDown &&
                (rightmostAliens[0].XAfterMove() + rightmostAliens[0].Width + WEIRD_WINDOW_RIGHT_BORDER_DISPLACEMENT_AMOUNT > ActualWidth ||
                leftmostAliens[0].XAfterMove() < 0);

            Alien currentAlien;
            int alienImageIndex = currentAlienImageIndex++ % 2 == 1 ? 0 : 1;

            for (int y = 0; y < SpaceInvaders.ALIENS_IN_COLUMN_COUNT; ++y)
            {
                for (int x = 0; x < SpaceInvaders.ALIENS_IN_ROW_COUNT; ++x)
                {
                    currentAlien = SpaceInvaders.Board.Aliens[y][x];

                    // Animate
                    AliensRectangles[y, x].Fill = AliensImages[currentAlien.TypeSize][alienImageIndex];

                    // Move down or sideways
                    if (thisTickAliensGoDown)
                    {
                        currentAlien.MoveDirection = (Direction)((int)currentAlien.MoveDirection * -1);
                        currentAlien.GetCloserToEarth(ALIEN_ADVANCEMENT_RATE);
                    }
                    else
                    {
                        currentAlien.Move();
                    }

                    Canvas.SetTop(AliensRectangles[y, x], currentAlien.Y);
                    Canvas.SetLeft(AliensRectangles[y, x], currentAlien.X);
                }
            }

            previousTickAliensWentDown = thisTickAliensGoDown;
        }

        public void Setup()
        {
            // Aliens rectanles
            for (int y = 0; y < SpaceInvaders.ALIENS_IN_COLUMN_COUNT; ++y)
            {
                for (int x = 0; x < SpaceInvaders.ALIENS_IN_ROW_COUNT; ++x)
                {
                    AliensRectangles[y, x] = new Rectangle
                    {
                        Height = SpaceInvaders.Board.Aliens[y][x].Height,
                        Width = SpaceInvaders.Board.Aliens[y][x].Width,
                        Fill = AliensImages[SpaceInvaders.Board.Aliens[y][x].TypeSize][0]
                    };

                    Canvas.SetTop(AliensRectangles[y, x], SpaceInvaders.Board.Aliens[y][x].Y);
                    Canvas.SetLeft(AliensRectangles[y, x], SpaceInvaders.Board.Aliens[y][x].X);

                    BoardPanel.Children.Add(AliensRectangles[y, x]);
                }
            }

            GetAliensWithUtmostPositions();

            // Player rectangle
            SpaceshipRectangle = new Rectangle
            {
                Height = SpaceInvaders.Board.Spaceship.Height,
                Width = SpaceInvaders.Board.Spaceship.Width,
                Fill = SpaceshipImage
            };

            Canvas.SetTop(SpaceshipRectangle, SpaceInvaders.Board.Spaceship.Y);
            Canvas.SetLeft(SpaceshipRectangle, SpaceInvaders.Board.Spaceship.X);

            BoardPanel.Children.Add(SpaceshipRectangle);

            // Add bullet events
            SpaceInvaders.Board.Spaceship.PlayerController.PlayerShot += AddBulletToBoard;
        }

        private void GetAliensWithUtmostPositions()
        {
            // LINQ MinBy, MaxBy, SelectMany, Where
            rightmostAliens = SpaceInvaders.Board.Aliens.SelectMany(a => a).Where(a => a.X == SpaceInvaders.Board.Aliens.SelectMany(a => a).MaxBy(a => a.X).X).ToList();
            leftmostAliens = SpaceInvaders.Board.Aliens.SelectMany(a => a).Where(a => a.X == SpaceInvaders.Board.Aliens.SelectMany(a => a).MinBy(a => a.X).X).ToList();
        }

        private void AddBulletToBoard(object sender, NewBulletEventArgs e)
        {
            Rectangle NewBulletRectangle = new Rectangle
            {
                Width = e.NewBullet.Width,
                Height = e.NewBullet.Height,
                Fill = e.NewBullet.BulletSource == Bullet.Source.Spaceship ? SpaceshipBulletImage : AlienBulletImages[0]
            };

            BulletsRectangles.Add(NewBulletRectangle);

            Canvas.SetTop(NewBulletRectangle, e.NewBullet.Y);
            Canvas.SetLeft(NewBulletRectangle, e.NewBullet.X);

            BoardPanel.Children.Add(NewBulletRectangle);
        }
    }
}
