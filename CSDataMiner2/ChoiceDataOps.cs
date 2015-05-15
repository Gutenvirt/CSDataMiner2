using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDataMiner2
{
    public static class ChoiceDataOps
    {

        public static double[,] GetFrequencies(string[] type, string[,] data)
        {
            var result = new double[data.GetLength(0), 5];

            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (type[i] != "MC")
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
                    if (s == "NaN")
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
