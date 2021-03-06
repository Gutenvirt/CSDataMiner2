//
//  BinDataOps.cs
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
using System.Linq;

namespace CSDataMiner2
{
	public static class BinDataOps
	{
		const int Precision = 6;
		const string NullValue = "NaN"; 
        
		public static double[] GetRawScores (byte[,] data)
		{
			var result = new double[data.GetLength (1)];

			for (int i = 0; i < data.GetLength (1); i++) {
				result [i] = Sum (GetRow (i, data));
			}
			return result;
		}

		public static double[] GetPercentScores (byte[,] data)
		{
			var result = new double[data.GetLength (1)];
			for (int i = 0; i < data.GetLength (1); i++) {
				result [i] = Average (GetRow (i, data));
			}
			return result;
		}

		public static double GetAlpha (double[] pvalues, double stddev)
		{
			return GetAlpha (pvalues, stddev, -1); 
		}

		public static double GetAlpha (double[] pvalues, double stddev, int colskip)
		{
			double variance = 0;
			for (int i = 0; i < pvalues.GetLength (0); i++) {
				if (i == colskip)
					continue;
				variance += pvalues [i] * (1.0 - pvalues [i]);
			}
			return (1 / (pvalues.GetLength (0) - 1.0) + 1.0) * (1.0 - variance / Math.Pow (stddev, 2));
		}

		public static double GetStandardErrorOfMeasure (double stddev, double alpha)
		{
			return stddev * Math.Sqrt (1.0 - alpha);
		}

		public static double[] GetPointBiSerial (double stddev, double[] scores, byte[,] data)
		{
			var meanCorrect = new double[data.GetLength (0)];
			var meanWrong = new double[data.GetLength (0)];
			var numStuCorrect = new double[data.GetLength (0)];
			var numStuWrong = new double[data.GetLength (0)];

			for (int i = 0; i < data.GetLength (0); i++) {
				for (int j = 0; j < data.GetLength (1); j++) {
					if (data [i, j] == 1) {
						meanCorrect [i] += scores [j];
						numStuCorrect [i] += 1;
					} else {
						meanWrong [i] += scores [j];
						numStuWrong [i] += 1;
					}
				}
			}
			var result = new double[data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				meanCorrect [i] = meanCorrect [i] / numStuCorrect [i];
				meanWrong [i] = meanWrong [i] / numStuWrong [i];
                result[i] = (meanCorrect[i] - meanWrong[i]) / stddev * Math.Sqrt(numStuCorrect[i] / data.GetLength(1) * numStuWrong[i] / data.GetLength(1));
			}
			return result;
		}

		public static double[] GetDescriptiveStats (double[] scores)
		{
            if (scores.Length < 5) { return new double[] { 0, 0, 0, 0, 0, 0 }; }
            Array.Sort (scores);
			int size = scores.GetLength (0);

			return new double[6] {
				scores [0],
				(size % 2 != 0) ? (double)scores [size / 4] : ((double)scores [size / 4] + (double)scores [size / 4 - 1]) / 2,
				Average (scores),
				(size % 2 != 0) ? (double)scores [size / 2] : ((double)scores [size / 2] + (double)scores [size / 2 - 1]) / 2,
				(size % 2 != 0) ? (double)scores [size / 4 * 3] : ((double)scores [size / 4 * 3] + (double)scores [size / 4 * 3 - 1]) / 2,
				scores [size - 1]
			};
		}

		public static double GetStandardDeviation (byte[,] data)
		{
			return GetStandardDeviation (-1, data);
		}

		public static double GetStandardDeviation (int colskip, byte[,] data)
		{
			double variance = 0;
			double[] rawScores = GetRawScores (data);
			double testMean = Average (rawScores);

			for (int i = 0; i < rawScores.GetLength (0); i++) {
				if (i == colskip)
					continue;
				variance += Math.Pow (rawScores [i] - testMean, 2);
			}
			return Math.Round (Math.Sqrt (variance / rawScores.GetLength (0)), Precision);
		}

        public static double[] GetZScore(double[] data)
        {
            var result = new double[data.GetLength(0)];
            double variance = 0;
            double testMean = Average(data);

            for (int i = 0; i < data.GetLength(0); i++)
            {
                variance += Math.Pow(data[i] - testMean, 2);
            }
            double stddev = Math.Sqrt(variance / data.GetLength(0));

            for (int i = 0; i < data.GetLength(0); i++)
            {
                result[i] = (data[i]-testMean )/stddev ;
            }
            return result;
        }

		public static double[] GetPValues (byte[,] data)
		{
			return GetPValues (-1, data);
		}

		public static double[] GetPValues (int colskip, byte[,] data)
		{
			var result = new double[data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				if (i == colskip)
					continue;
				result [i] = Average (GetColumn (i, data));
			}
			return result;
		}

		public static double GetTestSkew (double testMean, double testMedian, double testSTDev)
		{
			return 3 * (testMean - testMedian) / testSTDev;
		}

		public static double[] GetAlphaIfDropped (byte[,] data)
		{
			var result = new double[data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				result [i] = GetAlpha (GetPValues (i, data), GetStandardDeviation (i, data), i);
			}
			return result;
		}


		public static double Sum (double[] list)
		{
			double result = 0;
			for (int i = 0; i < list.GetLength (0); i++) {
				result += list [i];
			}
			return result;
		}

		public static double Average (double[] list)
		{
			double sum = Sum (list);
			double result = sum / list.GetLength (0);
			return result;
		}

		public static double[] GetRow (int index, byte[,] data)
		{
			int offset = 0;
			var source = new byte [data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				source [i] = data [i, index];
				if (data [i, index] > 1)
					offset += 1;
			}
			var result = new double[source.GetLength (0) - offset];
			int k = 0;
			for (int i = 0; i < source.GetLength (0); i++) {
				if (source [i] > 1)
					continue;
				result [k] = source [i];
				k += 1;
			}
			return result;
		}

		public static double[] GetColumn (int index, byte[,] data)
		{
			int offset = 0;
			var source = new byte [data.GetLength (1)];
			for (int i = 0; i < data.GetLength (1); i++) {
				source [i] = data [index, i];
				if (data [index, i] > 1)
					offset += 1;
			}
			var result = new double[source.GetLength (0) - offset];
			int k = 0;
			for (int i = 0; i < source.GetLength (0); i++) {
				if (source [i] > 1)
					continue;
				result [k] = source [i];
				k += 1;
			}
			return result;
		}
	}
}