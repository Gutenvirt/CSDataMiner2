﻿//
//  Assessment.cs
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
	public class Assessment
	{
		public string Output { get; set; }

		private string[,] BinData;

		public Assessment (string filename)
		{
			var dcDataGet = new DataConnection (filename);
			var dpDataFormat = new DataParser (dcDataGet.RawData, MethodOfDelete.Pairwise, true);
			BinData = dpDataFormat.BinaryData;

			//ListWise -> Removes the student from database if there is ANY omission found.
			//Pairwise -> (DEFAULT) Replaces any omission with NaN (not a number) but still allows present data to be analyzed.
			//ZeroReplace -> Same as above but NaN is a replaced with a zero.

			string testName = dpDataFormat.TestName;
			string[] itemType = dpDataFormat.ItemType;
			string[] itemStandards = dpDataFormat.Standards;
			string[] itemAnswers = dpDataFormat.AnswerKey;

			double[] studentRawScores = DataAnalyzer.GetRawScores (ref BinData);
			double[] studentPercentScores = DataAnalyzer.GetPercentScores (ref BinData);

			int testLength = itemAnswers.GetLength (0);
			int studentLength = studentRawScores.GetLength (0);

			double[] itemPvalues = DataAnalyzer.GetPValues (ref BinData);
			double testStdDev = DataAnalyzer.GetStandardDeviation (ref BinData);
			double testAlpha = DataAnalyzer.GetAlpha (itemPvalues, testStdDev);

			double testSEM = DataAnalyzer.GetStandardErrorOfMeasure (testStdDev, testAlpha);

			double[] itemPBS = DataAnalyzer.GetPointBiSerial (testStdDev, studentRawScores, ref BinData);
			double[] testStatistics = DataAnalyzer.GetDescriptiveStats (studentRawScores);
			double[,] MCFreq = dpDataFormat.GetFrequencies (itemType);

			double[] testAifD = DataAnalyzer.GetAlphaIfDropped (ref BinData);

			Output += testName + Environment.NewLine;
			Output += "No.\tType\tStandard\t\tAnswer\tPValue\t\tPBS\tAifD" + Environment.NewLine;
			for (int i = 0; i < dpDataFormat.BinaryData.GetLength (0); i++) {
				Output += (i + 1) + "\t" + itemType [i] + "\t" + itemStandards [i] + "\t\t" +
				itemAnswers [i] + "\t" + itemPvalues [i] + "\t" + itemPBS [i] + "\t" + testAifD [i] + Environment.NewLine;
			}

			Output += Environment.NewLine + "Descriptive Statistics" + Environment.NewLine;
			Output += "STD: " + testStdDev + Environment.NewLine;
			Output += "SEM: " + testSEM + Environment.NewLine;
			Output += "Alp: " + testAlpha + Environment.NewLine;
			Output += "Min: " + testStatistics [0] + Environment.NewLine;
			Output += "Q1 : " + testStatistics [1] + Environment.NewLine;
			Output += "Mea: " + testStatistics [2] + Environment.NewLine;
			Output += "Med: " + testStatistics [3] + Environment.NewLine;
			Output += "Q3 : " + testStatistics [4] + Environment.NewLine;
			Output += "Max: " + testStatistics [5] + Environment.NewLine;
			Output += Environment.NewLine;

			for (int i = 0; i < dpDataFormat.BinaryData.GetLength (0); i++) {

				switch (itemType [i]) {
				case "MC":
					Output += MCFreq [i, 0] + "\t" + MCFreq [i, 1] + "\t" + MCFreq [i, 2] + "\t" + MCFreq [i, 3] + "\t" + MCFreq [i, 4] + Environment.NewLine;
					break;
				case "CR":
					Output += dpDataFormat.CRAverages [i] + Environment.NewLine;
					break;
				}
			}
		}
	}
}

