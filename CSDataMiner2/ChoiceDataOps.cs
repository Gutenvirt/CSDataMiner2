//
//  ChoiceDataOps.cs
//
//  Author:
//       Christopher Stefancik <gutenvirt@gmail.com>
//
//  Copyright (c) 2015 CD Stefancik
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CSDataMiner2
{
    public static class ChoiceDataOps
    {

        public static int[] GetUniqueResponse(string[,] data, string[] type)
        {
            var result = new int[data.GetLength(0)];
            var tmpCol = new string[data.GetLength(1)];
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (type[i] != "GR")
                {
                    result[i] = 0;
                    continue;
                }
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    tmpCol[j] = data[i, j];
                }
                result[i] = tmpCol.Distinct().Count();
            }
            return result;
        }

        public static double[,] GetFrequencies(string[] type, string[,] data)
        {
            var result = new double[data.GetLength(0), 5];

            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (type[i] != "MC" && type[i] != "MS")
                    continue;
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    string s = data[i, j].Replace("+", "");
                    if (s == "A" | s == "F")
                    {
                        result[i, 0] += 1;
                        continue;
                    }
                    if (s == "B" | s == "G")
                    {
                        result[i, 1] += 1;
                        continue;
                    }
                    if (s == "C" | s == "H")
                    {
                        result[i, 2] += 1;
                        continue;
                    }
                    if (s == "D" | s == "J")
                    {
                        result[i, 3] += 1;
                        continue;
                    }
                    if (s == "NaN" | s.Trim() == string.Empty)
                    {
                        result[i, 4] += 1;
                        continue;
                    }
                }

                double multiplier = 1.0 / (double)data.GetLength(1) * 100.0;

                result[i, 0] = result[i, 0] * multiplier;
                result[i, 1] = result[i, 1] * multiplier;
                result[i, 2] = result[i, 2] * multiplier;
                result[i, 3] = result[i, 3] * multiplier;
                result[i, 4] = result[i, 4] * multiplier;
            }
            return result;
        }


    }
}
