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
        private int X;
        private int Y;
        public Coordinate() { X = 0; Y = 0; }
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int GetX()
        {
            return X;
        }

        public int GetY()
        {
            return Y;
        }

        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Compare (Coordinate pixelToCompare)
        {
            if (pixelToCompare.GetX() == this.X && pixelToCompare.GetY() == this.Y)
            {
                return true;
            }
            else
                return false;
        }
    }
}
