using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDataMiner2
{
    public class DataFileSettings
    {
        public Corner UpperRight { get; set; }
        public Corner UpperLeft { get; set; }

        public DataFileSettings (Corner UpperRightCorner, Corner UpperLeftCorner)
        {
            UpperRight = UpperRightCorner;

            UpperLeft = UpperLeftCorner; 
        }
    }

    public struct Corner
    {
        public readonly int X;
        public readonly int Y;

        public Corner(int xInt, int yInt)
        {
            this.X = xInt;
            this.Y = yInt;
        }
    }
}
