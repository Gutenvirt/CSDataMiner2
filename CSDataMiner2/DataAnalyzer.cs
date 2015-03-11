﻿//
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
	public static class DataAnalyzer
	{
		const int Precision = 6;

		public static double[] GetRawScores (ref byte[,] data)
		{
			var result = new double[data.GetLength (1)];
			for (int i = 0; i < data.GetLength (1); i++)
				result [i] = Sum (GetRow (i, ref data));
			return result;
		}

		public static double[] GetPercentScores (ref byte[,] data)
		{
			var result = new double[data.GetLength (1)];
			for (int i = 0; i < data.GetLength (1); i++)
				result [i] = Average (GetRow (i, ref data));
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
				variance += pvalues [i] * (1 - pvalues [i]);
			}
			return Math.Round ((1 / (pvalues.GetLength (0) - 1) + 1) * (1 - variance / Math.Pow (stddev, 2)), Precision);
		}

		public static double GetStandardErrorOfMeasure (double stddev, double alpha)
		{
			return Math.Round (stddev * Math.Sqrt (1 - alpha), Precision);
		}

		public static double[] GetPointBiSerial (double stddev, double[] scores, ref byte[,] data)
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
				result [i] = Math.Round ((meanCorrect [i] - meanWrong [i]) / stddev * Math.Sqrt (numStuCorrect [i] / (double)data.GetLength (1) * numStuWrong [i] / (double)data.GetLength (1)), Precision);
			}
			return result;
		}

		public static double[] GetDescriptiveStats (double[] scores)
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

		public static double GetStandardDeviation (ref byte[,] data)
		{
			return GetStandardDeviation (-1, ref data);
		}

		public static double GetStandardDeviation (int colskip, ref byte[,] data)
		{
			double variance = 0;
			double[] rawScores = GetRawScores (ref data);
			double testMean = Average (rawScores);

			for (int i = 0; i < rawScores.GetLength (0); i++) {
				if (i == colskip)
					continue;
				variance += Math.Pow (rawScores [i] - testMean, 2);
			}
			return (double)Math.Round (Math.Sqrt (variance / rawScores.GetLength (0)), Precision);
		}

		public static double[] GetPValues (ref byte[,] data)
		{
			return GetPValues (-1, ref data);
		}

		public static double[] GetPValues (int colskip, ref byte[,] data)
		{
			var result = new double[data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				if (i == colskip)
					continue;
				result [i] = Average (GetColumn (i, ref data));
			}
			return result;
		}


		public static double[] GetAlphaIfDropped (ref byte[,] data)
		{
			var result = new double[data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				result [i] = GetAlpha (GetPValues (i, ref data), GetStandardDeviation (i, ref data), i);
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
			double result = (double)Math.Round (sum / list.GetLength (0), Precision);
			return result;
		}

		public static double[] GetRow (int index, ref byte[,] data)
		{
			int offset = 0;
			var source = new byte [data.GetLength (0)];
			for (int i = 0; i < data.GetLength (0); i++) {
				source [i] = data [i, index];
				if (data [i, index] == null)
					offset += 1;
			}
			var result = new double[source.GetLength (0) - offset];
			int k = 0;
			for (int i = 0; i < source.GetLength (0); i++) {
				if (source [i] == null)
					continue;
				result [k] = source [i];
				k += 1;
			}
			return result;
		}

		public static double[] GetColumn (int index, ref byte[,] data)
		{
			int offset = 0;
			var source = new byte [data.GetLength (1)];
			for (int i = 0; i < data.GetLength (1); i++) {
				source [i] = data [index, i];
				if (data [index, i] == null)
					offset += 1;
			}
			var result = new double[source.GetLength (0) - offset];
			int k = 0;
			for (int i = 0; i < source.GetLength (0); i++) {
				if (source [i] == null)
					continue;
				result [k] = source [i];
				k += 1;
			}
			return result;
		}
	}
}
