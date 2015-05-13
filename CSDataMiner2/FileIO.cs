//
//  FileIO.cs
//
//  Author:
//       Christopher Stefancik <gutenvirt@gmail.com>
//
//  Copyright (c) 2015 2015 CD Stefancik
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
using System.IO;

namespace CSDataMiner2
{
    public static class FileIO
    {

        public static void WriteCSV(string filename, byte[,] data)
        {
            if (filename != null)
            {
                filename = filename.Replace(".xlsx", ".csv");
            }
            else { return; }

            string[] strBuffer = new string[data.GetLength(1) + 1];

            for (int i = 0; i < data.GetLength(0); i++)
            {
                strBuffer[0] += "Q" + (i + 1).ToString() +",";
            }

            for (int j = 0; j < data.GetLength(1); j++)
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    if (data[i, j] == 255)
                    {
                        strBuffer[j + 1] += ".,";
                    }
                    else
                    {
                        strBuffer[j + 1] += data[i, j].ToString() + ',';
                    }
                }
            }

            System.IO.File.WriteAllLines(filename, strBuffer);
        }

    }
}

