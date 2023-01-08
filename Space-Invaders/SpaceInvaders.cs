using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders
{
    internal class SpaceInvaders
    {
        public const int ALIENS_IN_ROW_COUNT = 11;
        public const int ALIENS_IN_COLUMN_COUNT = 5;

        public const int BOARD_HORIZONTAL_MARGIN = 46;
        public const int BOARD_VERTICAL_MARGIN = 20;

        public const int SPACE_BETWEEN_ROWS = 30;

        public const int SPACESHIP_INITIAL_X = MainWindow.WINDOW_WIDTH / 2 - MainWindow.WEIRD_WINDOW_RIGHT_BORDER_DISPLACEMENT_AMOUNT;
        public const int SPACESHIP_INITIAL_Y = 600;
        public const int SPACESHIP_WIDTH = 39;
        public const int SPACESHIP_HEIGHT = 24;

        public Alien.Type[] AlienTypesInRows = new Alien.Type[ALIENS_IN_COLUMN_COUNT]
        { 
            Alien.Type.Small,
            Alien.Type.Medium,
            Alien.Type.Medium,
            Alien.Type.Large,
            Alien.Type.Large
        };

        public static Dictionary<Alien.Type, int> SpacesBetweenAliens = new Dictionary<Alien.Type, int>()
        {
            { Alien.Type.Small, 24 },
            { Alien.Type.Medium, 15 },
            { Alien.Type.Large, 12 }
        };

        public Board Board;

        public SpaceInvaders(Board board)
        {
            Board = board; 
        }

        public void Setup()
        {
            // Create aliens
            Board.Aliens = new List<List<Alien>>(ALIENS_IN_COLUMN_COUNT);

            int leftOffset;
            int topOffset = BOARD_VERTICAL_MARGIN;
            int currSpaceBetweenAliens;

            for (int y = 0; y < ALIENS_IN_COLUMN_COUNT; ++y)
            {
                Board.Aliens.Add(new List<Alien>(ALIENS_IN_ROW_COUNT));

                leftOffset = BOARD_HORIZONTAL_MARGIN;
                topOffset = BOARD_VERTICAL_MARGIN + y * (SPACE_BETWEEN_ROWS + Alien.AlienHeight);
                currSpaceBetweenAliens = SpacesBetweenAliens[AlienTypesInRows[y]];

                switch (AlienTypesInRows[y])
                {
                    case Alien.Type.Medium: { leftOffset += 1; break; }
                    case Alien.Type.Small: { leftOffset += 2; break; }
                }

                for (int x = 0; x < ALIENS_IN_ROW_COUNT; ++x)
                {
                    Board.Aliens[y].Add(new Alien(leftOffset, topOffset, AlienTypesInRows[y]));

                    leftOffset += currSpaceBetweenAliens + Alien.AlienTypeWidth[AlienTypesInRows[y]];
                }
            }

            // Create spaceship
            Board.Spaceship = new Spaceship(SPACESHIP_INITIAL_X, SPACESHIP_INITIAL_Y, SPACESHIP_WIDTH, SPACESHIP_HEIGHT);

            // Create bullets lists
            Board.SpaceshipBullets = new List<Bullet>();
            Board.AlienBullets = new List<Bullet>();

            Board.Setup();
        }
    }
}
