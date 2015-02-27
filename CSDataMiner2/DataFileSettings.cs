/*
VBDataMiner - Extract and analyze data from MS Excel(c) files.
Copyright (C) 2015 Chris Stefancik gutenvirt@gmail.com

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 */

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

        public override string ToString()
        {
            return X.ToString() + "," + Y.ToString();
        }
    }
}
