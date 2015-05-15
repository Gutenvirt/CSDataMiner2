using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDataMiner2
{

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
