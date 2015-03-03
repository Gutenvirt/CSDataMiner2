//
//  DataAnalyzer.cs
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

namespace CSDataMiner2
{
	public class DataAnalyzer
	{

		public string NullValue { get; set; }

		public DataAnalyzer (string nullvalue, string[,] inpData)
		{
			NullValue = nullvalue;
		}

		public double[] GetRawScores (string[,] data)
		{
			var result = new double[data.GetLength (1)];
			for (int i = 0; i < data.GetLength (1); i++) {
				result [i] = Sum (GetRow (i, data));
			}
			return result;
		}

		public double[] GetPercentScores (string[,] data)
		{
			var result = new double[data.GetLength (1)];
			for (int i = 0; i < data.GetLength (1); i++) {
				result [i] = Average (GetRow (i, data));
			}
			return result;
		}

		public double GetAlpha (double[] pvalues, double stddev)
		{
			double variance = 0;
			for (int i = 0; i < pvalues.GetLength (0); i++) {
				variance += pvalues [i] * (1 - pvalues [i]);
			}
			return (1 / (pvalues.GetLength (0) - 1) + 1) * (1 - variance / Math.Pow (stddev, 2));
		}

		public double GetStandardErrorOfMeasure (double stddev, double alpha)
		{
			return stddev * Math.Sqrt (1 - alpha);
		}

		public double[] GetPointBiSerial (double stddev, double[] scores, string[,] data)
		{
			var meanCorrect = new double[data.GetLength (0)];
			var meanWrong = new double[data.GetLength (0)];
			var numStuCorrect = new double[data.GetLength (0)];
			var numStuWrong = new double[data.GetLength (0)];

			for (int i = 0; i < data.GetLength (0); i++) {
				for (int j = 0; j < data.GetLength (1); j++) {
					if (data [i, j] == "1") {
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
				result [i] = (meanCorrect [i] - meanWrong [i]) / stddev * Math.Sqrt (numStuCorrect [i] / (double)data.GetLength (1) * numStuWrong [i] / (double)data.GetLength (1));
			}
			return result;
		}

		public double[] GetDescriptiveStats (double[] scores)
		{
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

		public double GetStandardDeviation (string[,] data)
		{
			double variance = 0;
			double[] rawScores = GetRawScores (data);
			double testMean = Average (rawScores);

			for (int i = 0; i < rawScores.GetLength (0); i++) {
				variance += Math.Pow (rawScores [i] - testMean, 2);
			}
			return (double)Math.Sqrt (variance / rawScores.GetLength (0));
		}

		public double[] GetPValues (string[,] data)
		{
			var result = new double[data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				result [i] = Average (GetColumn (i, data));
			}
			return result;
		}

		public double Sum (double[] list)
		{
			double result = 0;
			for (int i = 0; i < list.GetLength (0); i++) {
				result += list [i];
			}
			return result;
		}

		public double Average (double[] list)
		{
			double sum = Sum (list);
			double result = (double)sum / list.GetLength (0);
			return result;
		}

		public double[] GetRow (int index, string[,] data)
		{
			int offset = 0;
			var source = new string [data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				source [i] = data [i, index];
				if (data [i, index] == NullValue)
					offset += 1;
			}
			var result = new double[source.GetLength (0) - offset];
			int k = 0;
			for (int i = 0; i < source.GetLength (0); i++) {
				if (source [i] == NullValue)
					continue;
				result [k] = double.Parse (source [i]);
				k += 1;
			}
			return result;
		}

		public double[] GetColumn (int index, string[,] data)
		{
			int offset = 0;
			var source = new string [data.GetLength (1)];
			for (int i = 0; i < data.GetLength (1); i++) {
				source [i] = data [index, i];
				if (data [index, i] == NullValue)
					offset += 1;
			}
			var result = new double[source.GetLength (0) - offset];
			int k = 0;
			for (int i = 0; i < source.GetLength (0); i++) {
				if (source [i] == NullValue)
					continue;
				result [k] = double.Parse (source [i]);
				k += 1;
			}
			return result;
		}
	}
}
