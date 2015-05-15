using System;
using System.Linq;
using System.Text;


namespace CSDataMiner2
{
    public static class HistogramGen
    {

        public static double[] GetValues(hType type, double[] cutoffs, double[] data)
        {
            Array.Sort(cutoffs);

            var result = new double[cutoffs.GetLength(0)];

            for (int i = 0; i < cutoffs.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(0); j++)
                {
                    if (i == 0)
                    {
                        if (data[j] < cutoffs[i])
                        {
                            result[i] += 1;
                        }
                    }
                    else
                    {
                        if (data[j] >= cutoffs[i - 1] & data[j] < cutoffs[i])
                        {
                            result[i] += 1;
                        }
                    }
                }
            }
            if (type == hType.Density)
            {
                double max = 1 / result.Max();
                for (int i = 0; i < result.GetLength(0); i++)
                {
                    result[i] = result[i] * max;
                }
            }
            if (type == hType.Percent)
                {
                    double total = 1 / (double)data.GetLength(0);
                    for (int i = 0; i < result.GetLength(0); i++)
                    {
                        result[i] = result[i] * total;
                    }
                }
        return result;
        }
    }

    public enum hType
    {
        Density,
        Frequency,
        Percent
    }
}