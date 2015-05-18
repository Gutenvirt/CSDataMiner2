using System;
using System.Linq;
using System.Text;


namespace CSDataMiner2
{
    public static class HistogramGen
    {

        public static string MsgQueue = "";

        public static void Parse(string options)
        {
            var _c = new double[]{};
            var _d = new double[]{};
            var _t = hType.Frequency;

            string[] optList = options.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            MsgQueue += "BEGIN//" + Environment.NewLine;

            foreach (string op in optList)
            {
                switch (op.Substring(0, 4))
                {
                    case "bins":
                        _c = CommandParser.ConvertStrA2Dbl(CommandParser.ReturnInBrackets(op).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                        break;

                    case "freq":
                        _t = hType.Frequency;
                        break;

                    case "dens":
                        _t = hType.Density;
                        break;

                    case "perc":
                        _t = hType.Percent;
                        break;

                    case "data":
                        _d = CommandParser.ConvertStrA2Dbl(CommandParser.ReturnInBrackets(op).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                        break;
                }          
            }
            double[] outm = GetValues(_t, _c, _d);

            for (int i = 0; i < outm.GetLength(0);i++)
            {
                MsgQueue += _c[i]  + ": " + outm[i] + Environment.NewLine;
            }
            MsgQueue += "END//";
        }

        public static string Star(double frac)
        {
            string result = "";

            for (int i = 0 ; i < frac*10; i++)
            {
                result += ">";
            }
            return result;
        }

        public static double[] GetValues(hType type, double[] cutoffs, double[] data)
        {
            Array.Sort(cutoffs);

            var result = new double[cutoffs.GetLength(0)];

            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < cutoffs.GetLength(0); j++)
                {
                    if (j == 0)
                    {
                        if (data[i] <= cutoffs[j])
                            result[j] += 1;
                    }
                    else
                    {
                        if (data[i] > cutoffs[j - 1] && data[i] <= cutoffs[j])
                            result[j] += 1;
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