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
using System.Linq;

namespace CSDataMiner2
{
	public static class DataAnalyzer
	{
		const int Precision = 6;
		const string NullValue = "NaN";

		public static float[] GetRawScores (byte[,] data)
		{
			var result = new float[data.GetLength (1)];
			for (int i = 0; i < data.GetLength (1); i++) {
				result [i] = Sum (GetRow (i, data));
			}
			return result;
		}

		public static float[] GetPercentScores (byte[,] data)
		{
			var result = new float[data.GetLength (1)];
			for (int i = 0; i < data.GetLength (1); i++) {
				result [i] = Average (GetRow (i, data));
			}
			return result;
		}

		public static float GetAlpha (float[] pvalues, float stddev)
		{
			return GetAlpha (pvalues, stddev, -1); 
		}

		public static float GetAlpha (float[] pvalues, float stddev, int colskip)
		{
			float variance = 0;
			for (int i = 0; i < pvalues.GetLength (0); i++) {
				if (i == colskip)
					continue;
				variance += pvalues [i] * (1 - pvalues [i]);
			}
			return (float)Math.Round ((1 / (pvalues.GetLength (0) - 1) + 1) * (1 - variance / Math.Pow (stddev, 2)), Precision);
		}

		public static float GetStandardErrorOfMeasure (float stddev, float alpha)
		{
			return (float)Math.Round (stddev * Math.Sqrt (1 - alpha), Precision);
		}

		public static float[] GetPointBiSerial (float stddev, float[] scores, byte[,] data)
		{
			var meanCorrect = new float[data.GetLength (0)];
			var meanWrong = new float[data.GetLength (0)];
			var numStuCorrect = new float[data.GetLength (0)];
			var numStuWrong = new float[data.GetLength (0)];

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
			var result = new float[data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				meanCorrect [i] = meanCorrect [i] / numStuCorrect [i];
				meanWrong [i] = meanWrong [i] / numStuWrong [i];
				result [i] = (float)Math.Round ((meanCorrect [i] - meanWrong [i]) / stddev * Math.Sqrt (numStuCorrect [i] / (float)data.GetLength (1) * numStuWrong [i] / (float)data.GetLength (1)), Precision);
			}
			return result;
		}

		public static float[] GetDescriptiveStats (float[] scores)
		{
			Array.Sort (scores);
			int size = scores.GetLength (0);

			return new float[6] {
				scores [0],
				(size % 2 != 0) ? (float)scores [size / 4] : ((float)scores [size / 4] + (float)scores [size / 4 - 1]) / 2,
				Average (scores),
				(size % 2 != 0) ? (float)scores [size / 2] : ((float)scores [size / 2] + (float)scores [size / 2 - 1]) / 2,
				(size % 2 != 0) ? (float)scores [size / 4 * 3] : ((float)scores [size / 4 * 3] + (float)scores [size / 4 * 3 - 1]) / 2,
				scores [size - 1]
			};
		}

		public static int[] GetStudentHistogramValues (float[] scores, int nColumns)
		{ 
			var result = new int[10];

			for (int i = 0; i < scores.GetLength (0); i++) {
				double _s = scores [i] / nColumns;
				if (_s < 0.1)
					result [0] += 1;
				if (_s >= 0.1 & _s < 0.2)
					result [1] += 1;
				if (_s >= 0.2 & _s < 0.3)
					result [2] += 1;
				if (_s >= 0.3 & _s < 0.4)
					result [3] += 1;
				if (_s >= 0.4 & _s < 0.5)
					result [4] += 1;
				if (_s >= 0.5 & _s < 0.6)
					result [5] += 1;
				if (_s >= 0.6 & _s < 0.7)
					result [6] += 1;
				if (_s >= 0.7 & _s < 0.8)
					result [7] += 1;
				if (_s >= 0.8 & _s < 0.9)
					result [8] += 1;
				if (_s >= 0.9)
					result [9] += 1;
			}
			return result;
		}

		public static float GetStandardDeviation (byte[,] data)
		{
			return GetStandardDeviation (-1, data);
		}

		public static float GetStandardDeviation (int colskip, byte[,] data)
		{
			double variance = 0;
			float[] rawScores = GetRawScores (data);
			float testMean = Average (rawScores);

			for (int i = 0; i < rawScores.GetLength (0); i++) {
				if (i == colskip)
					continue;
				variance += Math.Pow (rawScores [i] - testMean, 2);
			}
			return (float)Math.Round (Math.Sqrt (variance / rawScores.GetLength (0)), Precision);
		}

		public static float[] GetPValues (byte[,] data)
		{
			return GetPValues (-1, data);
		}

		public static float[] GetPValues (int colskip, byte[,] data)
		{
			var result = new float[data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				if (i == colskip)
					continue;
				result [i] = Average (GetColumn (i, data));
			}
			return result;
		}


		public static float[] GetAlphaIfDropped (byte[,] data)
		{
			var result = new float[data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				result [i] = GetAlpha (GetPValues (i, data), GetStandardDeviation (i, data), i);
			}
			return result;
		}


		public static float Sum (float[] list)
		{
			float result = 0;
			for (int i = 0; i < list.GetLength (0); i++) {
				result += list [i];
			}
			return result;
		}

		public static float Average (float[] list)
		{
			float sum = Sum (list);
			float result = (float)Math.Round (sum / list.GetLength (0), Precision);
			return result;
		}

		public static float[] GetRow (int index, byte[,] data)
		{
			int offset = 0;
			var source = new byte [data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				source [i] = data [i, index];
				if (data [i, index] > 1)
					offset += 1;
			}
			var result = new float[source.GetLength (0) - offset];
			int k = 0;
			for (int i = 0; i < source.GetLength (0); i++) {
				if (source [i] > 1)
					continue;
				result [k] = (float)source [i];
				k += 1;
			}
			return result;
		}

		public static float[] GetColumn (int index, byte[,] data)
		{
			int offset = 0;
			var source = new byte [data.GetLength (1)];
			for (int i = 0; i < data.GetLength (1); i++) {
				source [i] = data [index, i];
				if (data [index, i] > 1)
					offset += 1;
			}
			var result = new float[source.GetLength (0) - offset];
			int k = 0;
			for (int i = 0; i < source.GetLength (0); i++) {
				if (source [i] > 1)
					continue;
				result [k] = (float)source [i];
				k += 1;
			}
			return result;
		}
	}
}