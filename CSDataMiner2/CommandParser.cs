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

        public static void Parse (string comBuffer)  
        {
            string[] cmd = comBuffer.Split(new char[]{' ', ',', ':'}, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < cmd.GetLength(0); i++)
            {
                cmdList.Add(cmd[i]);
            }
        }
    }
}
