using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plotter
{
    class Coordinate
    {
        //a custom class that holds the x and y coordinates of a pixel, useful for lists of pixels
        //lists of pixels can be used in searching to ignore certain pixels to speed up the searching process
        //or can be useful in sending or saving serialized data
        private int x;
        private int y;
        public Coordinate() { x = 0; y = 0; }
        public Coordinate(int xx, int yy)
        {
            x = xx;
            y = yy;
        }
        public int X()
        {
            return x;
        }

        public int Y()
        {
            return y;
        }

        public void X(int xx)
        {
            x = xx;
        }

        public void Y(int yy)
        {
            y = yy;
        }

        public bool compare (Coordinate pixelToCompare)
        {
            if (pixelToCompare.X() == this.x && pixelToCompare.Y() == this.y)
            {
                return true;
            }
            else
                return false;
        }
    }
}
