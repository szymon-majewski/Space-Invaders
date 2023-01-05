using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Space_Invaders
{
    internal class Alien : Renderable
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; }
        public int Height { get; }

        public void Render()
        {
            throw new NotImplementedException();
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

        public Type TypeSize;

        public Alien (int x, int y, Type type)
        {
            X = x;
            Y = y;
            TypeSize = type;
            Width = AlienTypeWidth[TypeSize];
            Height = AlienHeight;
        }
    }
}
