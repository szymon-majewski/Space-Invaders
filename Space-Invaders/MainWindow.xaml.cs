using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Space_Invaders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int WINDOW_WIDTH = 650;
        public const int WINDOW_HEIGHT = 800;

        private static SpaceInvaders _spaceInvaders;

        private static Dictionary<Alien.Type, List<ImageBrush>> _aliensImages = new Dictionary<Alien.Type, List<ImageBrush>>()
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

        public Rectangle[,] AliensRectangles = new Rectangle[SpaceInvaders.ALIENS_IN_COLUMN_COUNT, SpaceInvaders.ALIENS_IN_ROW_COUNT];

        public MainWindow()
        {
            InitializeComponent();

            Height = WINDOW_HEIGHT;
            Width = WINDOW_WIDTH;

            _spaceInvaders = new SpaceInvaders(new Board());
            _spaceInvaders.Setup();

            Setup();
        }

        public void Setup()
        {
            for (int y = 0; y < SpaceInvaders.ALIENS_IN_COLUMN_COUNT; ++y)
            {
                for (int x = 0; x < SpaceInvaders.ALIENS_IN_ROW_COUNT; ++x)
                {
                    AliensRectangles[y, x] = new Rectangle
                    {
                        Height = _spaceInvaders.Board.Aliens[y][x].Height,
                        Width = _spaceInvaders.Board.Aliens[y][x].Width,
                        Fill = _aliensImages[_spaceInvaders.Board.Aliens[y][x].TypeSize][0]
                    };

                    Canvas.SetTop(AliensRectangles[y, x], _spaceInvaders.Board.Aliens[y][x].Y);
                    Canvas.SetLeft(AliensRectangles[y, x], _spaceInvaders.Board.Aliens[y][x].X);

                    BoardPanel.Children.Add(AliensRectangles[y, x]);
                }
            }
        }
    }
}
