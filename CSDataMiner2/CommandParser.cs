//
//  CommandParser.cs
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

namespace CSDataMiner2
{

    //Reserved for commandline interface 

    public static class CommandParser
    {

        public static List<string> cmdList = new List<string>();

        public static void Parse(string comBuffer)
        {
            string[] cmd = comBuffer.Split(new char[] { ' ', ',', ':' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < cmd.GetLength(0); i++)
            {
                cmdList.Add(cmd[i]);
            }
        }

        public static string ReturnInBrackets(string s)
        {
            return s.Substring(s.IndexOf('{') + 1, s.IndexOf('}') - s.IndexOf('{') - 1);

        }

        public static double[] ConvertStrA2Dbl(string[] s)
        {
            var result = new double[s.GetLength(0)];

            for (int i = 0; i < s.GetLength(0); i++)
            {
                double.TryParse(s[i], out result[i]);
            }
            return result;
        }
    }
}
