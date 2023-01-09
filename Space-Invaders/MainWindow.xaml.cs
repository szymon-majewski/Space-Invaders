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
        public const int ALIEN_POST_DEATH_EXPLOSION_TIME = 200;
        public const int TIME_BETWEEN_WAVES = 500;
        public const double WAVE_ALIEN_FPS_MULTIPLIER = 1.5;

        public int Score = 0;

        internal readonly Dictionary<Alien.Type, int> ALIEN_SCORES = new Dictionary<Alien.Type, int>()
        {
            { Alien.Type.Small, 30 },
            { Alien.Type.Medium, 20 },
            { Alien.Type.Large, 10 }
        };
        public readonly List<int> UFO_SCORES = new List<int>() { 50, 100, 150, 200, 250, 300 };

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
        internal static ImageBrush AlienExplosionImage = new ImageBrush() { ImageSource = new BitmapImage(new Uri("pack://application:,,,/res/img/AlienExplosion.png")) };

        public List<List<Rectangle>> AliensRectangles = new List<List<Rectangle>>(SpaceInvaders.ALIENS_IN_COLUMN_COUNT);
        public Rectangle SpaceshipRectangle;
        public List<Rectangle> BulletsRectangles = new List<Rectangle>();

        private DispatcherTimer gameTimerAliens;
        private const double INITIAL_ALIENS_UPDATES_PER_SECOND = 0.8;

        private DispatcherTimer gameTimerStandard;
        private const int STANDARD_UPDATES_PER_SECOND = 300;

        private DispatcherTimer alienDeathTimer;

        private int currentWaveIndex = 0;
        private double currentAliensUpdatesPerSecond = INITIAL_ALIENS_UPDATES_PER_SECOND;
        private double aliensUpdatesPerSecondBonus = 0.1;

        private int currentAlienImageIndex;

        private List<Alien> leftmostAliens;
        private List<Alien> rightmostAliens;

        private bool previousTickAliensWentDown;

        public const int WEIRD_WINDOW_RIGHT_BORDER_DISPLACEMENT_AMOUNT = 24;
        public const int UPPER_BORDER_Y = 10;

        public const int ALIEN_EXPLOSION_WIDTH = 36;

        public MainWindow()
        {
            InitializeComponent();

            Height = WINDOW_HEIGHT;
            Width = WINDOW_WIDTH;

            SpaceInvaders = new SpaceInvaders(new Board());

            Setup();
        }

        private void GameTimerStandardTick(object sender, EventArgs e)
        {
            //Delete later
            HighScoreLabel.Content = currentAliensUpdatesPerSecond;

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
                    RemoveBullet(i);

                    continue;
                }

                Canvas.SetTop(BulletsRectangles[i], currentBullet.Y);
                Canvas.SetLeft(BulletsRectangles[i], currentBullet.X);

                Rect bulletHitBox = new Rect(Canvas.GetLeft(BulletsRectangles[i]), Canvas.GetTop(BulletsRectangles[i]), BulletsRectangles[i].Width, BulletsRectangles[i].Height);
                bool bulletHit = false;

                // Collisions
                for (int y = 0; y < SpaceInvaders.Board.Aliens.Count && !bulletHit; ++y)
                {
                    for (int x = 0; x < SpaceInvaders.Board.Aliens[y].Count; ++x)
                    {
                        Rectangle currAlienRectangle = AliensRectangles[y][x];

                        Rect alienHitBox = new Rect(Canvas.GetLeft(currAlienRectangle), Canvas.GetTop(currAlienRectangle), currAlienRectangle.Width, currAlienRectangle.Height);
                        
                        if (alienHitBox.IntersectsWith(bulletHitBox))
                        {
                            bulletHit = true;
                            RemoveBullet(i);
                            AlienDeath(y, x);

                            break;
                        }
                    }
                }
            }
        }

        private void AlienDeath(int y, int x)
        {
            currentAliensUpdatesPerSecond += aliensUpdatesPerSecondBonus;

            Rectangle deadAlienRectangle = AliensRectangles[y][x];
            Alien deadAlien = SpaceInvaders.Board.Aliens[y][x];

            Score += ALIEN_SCORES[deadAlien.TypeSize];
            ScoreLabel.Content = "Score: " + Score;

            SpaceInvaders.Board.Aliens[y].RemoveAt(x);
            AliensRectangles[y].RemoveAt(x);

            if (SpaceInvaders.Board.Aliens[y].Count == 0)
            {
                SpaceInvaders.Board.Aliens.RemoveAt(y);
                AliensRectangles.RemoveAt(y);
            }

            RemoveAlienFromListsWithUtmostPositions(deadAlien);

            deadAlienRectangle.Width = ALIEN_EXPLOSION_WIDTH;
            deadAlienRectangle.Fill = AlienExplosionImage;
            alienDeathTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(ALIEN_POST_DEATH_EXPLOSION_TIME)
            };
            alienDeathTimer.Tick += (sender, e) => { RemoveAlienRectangle(sender, e, deadAlienRectangle); };
            alienDeathTimer.Start();

            if (SpaceInvaders.Board.Aliens.Count == 0)
            {
                gameTimerAliens.Stop();
                gameTimerStandard.Stop();

                ++currentWaveIndex;

                DispatcherTimer waveEndedTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(TIME_BETWEEN_WAVES)
                };
                waveEndedTimer.Tick += (sender, e) => { (sender as DispatcherTimer).Stop(); Setup(); };
                waveEndedTimer.Start();
            }
        }

        private void RemoveAlienRectangle(object sender, EventArgs e, Rectangle deadAlienRectangle)
        {
            BoardPanel.Children.Remove(deadAlienRectangle);
            (sender as DispatcherTimer).Stop();
        }

        private void RemoveBullet(int index)
        {
            SpaceInvaders.Board.SpaceshipBullets.RemoveAt(index);
            BoardPanel.Children.Remove(BulletsRectangles[index]);
            BulletsRectangles.RemoveAt(index);
        }

        private void GameTimerAliensTick(object sender, EventArgs e)
        {
            // Update aliens timer interval
            gameTimerAliens.Interval = TimeSpan.FromMilliseconds(1000.0 / currentAliensUpdatesPerSecond);

            bool thisTickAliensGoDown = 
                !previousTickAliensWentDown &&
                (rightmostAliens[0].XAfterMove() + rightmostAliens[0].Width + WEIRD_WINDOW_RIGHT_BORDER_DISPLACEMENT_AMOUNT > ActualWidth ||
                leftmostAliens[0].XAfterMove() < 0);

            Alien currentAlien;
            int alienImageIndex = currentAlienImageIndex++ % 2 == 1 ? 0 : 1;

            for (int y = 0; y < SpaceInvaders.Board.Aliens.Count; ++y)
            {
                for (int x = 0; x < SpaceInvaders.Board.Aliens[y].Count; ++x)
                {
                    currentAlien = SpaceInvaders.Board.Aliens[y][x];

                    // Animate
                    AliensRectangles[y][x].Fill = AliensImages[currentAlien.TypeSize][alienImageIndex];

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

                    Canvas.SetTop(AliensRectangles[y][x], currentAlien.Y);
                    Canvas.SetLeft(AliensRectangles[y][x], currentAlien.X);
                }
            }

            previousTickAliensWentDown = thisTickAliensGoDown;

            Alien alienFromBottommostRow = SpaceInvaders.Board.Aliens[SpaceInvaders.Board.Aliens.Count - 1][0];

            if (alienFromBottommostRow.Y + alienFromBottommostRow.Height >= SpaceInvaders.Board.Spaceship.Y)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            gameTimerStandard.Stop();
            gameTimerAliens.Stop();
            alienDeathTimer.Stop();
            SpaceInvaders.Board.Spaceship.ControllingEnabled = false;
            GameOverLabel.Content = "GAME OVER. SCORE: " + Score;
            GameOverLabel.Visibility = Visibility.Visible;
        }

        public void Setup()
        {
            SpaceInvaders.Setup();

            ClearBoard();

            //foreach (object boardPanelChild in BoardPanel.Children)
            //{
            //    if (boardPanelChild is Rectangle)
            //    {
            //        BoardPanel.Children.Remove(boardPanelChild as UIElement);
            //    }
            //}

            //BoardPanel.Children.Clear();

            // Aliens rectanles
            for (int y = 0; y < SpaceInvaders.ALIENS_IN_COLUMN_COUNT; ++y)
            {
                AliensRectangles.Add(new List<Rectangle>(SpaceInvaders.ALIENS_IN_ROW_COUNT));

                for (int x = 0; x < SpaceInvaders.ALIENS_IN_ROW_COUNT; ++x)
                {
                    AliensRectangles[y].Add(new Rectangle
                    {
                        Height = SpaceInvaders.Board.Aliens[y][x].Height,
                        Width = SpaceInvaders.Board.Aliens[y][x].Width,
                        Fill = AliensImages[SpaceInvaders.Board.Aliens[y][x].TypeSize][0]
                    });

                    Canvas.SetTop(AliensRectangles[y][x], SpaceInvaders.Board.Aliens[y][x].Y);
                    Canvas.SetLeft(AliensRectangles[y][x], SpaceInvaders.Board.Aliens[y][x].X);

                    BoardPanel.Children.Add(AliensRectangles[y][x]);
                }
            }

            GetAliensWithRightmostPositions();
            GetAliensWithLeftmostPositions();

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

            // This variable is for alien animation purposes
            currentAlienImageIndex = 0;

            previousTickAliensWentDown = false;

            Canvas.SetLeft(GameOverLabel, 120);
            Canvas.SetTop(GameOverLabel, 120);

            // Setup aliens game timers intervals
            currentAliensUpdatesPerSecond = INITIAL_ALIENS_UPDATES_PER_SECOND + (currentWaveIndex * WAVE_ALIEN_FPS_MULTIPLIER);

            // Setup game timers
            gameTimerAliens = new DispatcherTimer();
            gameTimerAliens.Interval = TimeSpan.FromMilliseconds(1000.0 / currentAliensUpdatesPerSecond);
            gameTimerAliens.Tick += GameTimerAliensTick;

            gameTimerStandard = new DispatcherTimer();
            gameTimerStandard.Interval = TimeSpan.FromMilliseconds(1000.0 / STANDARD_UPDATES_PER_SECOND);
            gameTimerStandard.Tick += GameTimerStandardTick;

            gameTimerAliens.Start();
            gameTimerStandard.Start();
        }

        private void GetAliensWithRightmostPositions()
        {
            // LINQ MinBy, MaxBy, SelectMany, Where
            rightmostAliens = SpaceInvaders.Board.Aliens.SelectMany(a => a).Where(a => a.X == SpaceInvaders.Board.Aliens.SelectMany(a => a).MaxBy(a => a.X).X).ToList();
        }

        private void GetAliensWithLeftmostPositions()
        {
            // LINQ MinBy, MaxBy, SelectMany, Where
            leftmostAliens = SpaceInvaders.Board.Aliens.SelectMany(a => a).Where(a => a.X == SpaceInvaders.Board.Aliens.SelectMany(a => a).MinBy(a => a.X).X).ToList();
        }

        private void RemoveAlienFromListsWithUtmostPositions(Alien alien)
        {
            if (rightmostAliens.Remove(alien) && rightmostAliens.Count == 0)
            {
                GetAliensWithRightmostPositions();
            }
            if (leftmostAliens.Remove(alien) && leftmostAliens.Count == 0)
            {
                GetAliensWithLeftmostPositions();
            }
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

        private void ClearBoard()
        {
            foreach (Rectangle bulletRectangle in BulletsRectangles)
            {
                BoardPanel.Children.Remove(bulletRectangle);
            }

            // LINQ
            foreach (Rectangle alienRectangle in AliensRectangles.SelectMany(a => a).ToList())
            {
                BoardPanel.Children.Remove(alienRectangle);
            }

            BoardPanel.Children.Remove(SpaceshipRectangle);
        }
    }
}
