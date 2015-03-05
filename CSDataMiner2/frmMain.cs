//
//  frmMain.cs
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
using System.Windows.Forms;

namespace CSDataMiner2
{
	public partial class frmMain : Form
	{

		public frmMain ()
		{
			InitializeComponent ();
			openFileDialog1.ShowDialog ();

			var dcDataGet = new DataConnection (openFileDialog1.FileName);
			var dpDataFormat = new DataParser (dcDataGet.RawData, MethodOfDelete.Pairwise, true);
			var daDataDesc = new DataAnalyzer (dpDataFormat.NullValue, dpDataFormat.BinaryData);
			//ListWise -> Removes the student from database if there is ANY omission found.
			//Pairwise -> (DEFAULT) Replaces any omission with NaN (not a number) but still allows present data to be analyzed.
			//ZeroReplace -> Same as above but NaN is a replaced with a zero.

			string testName = dpDataFormat.TestName;
			string[] itemType = dpDataFormat.ItemType;
			string[] itemStandards = dpDataFormat.Standards;
			string[] itemAnswers = dpDataFormat.AnswerKey;

			double[] studentRawScores = daDataDesc.GetRawScores (dpDataFormat.BinaryData);
			double[] studentPercentScores = daDataDesc.GetPercentScores (dpDataFormat.BinaryData);

			int testLength = itemAnswers.GetLength (0);
			int studentLength = studentRawScores.GetLength (0);

			double[] itemPvalues = daDataDesc.GetPValues (dpDataFormat.BinaryData);
			double testStdDev = daDataDesc.GetStandardDeviation (dpDataFormat.BinaryData);
			double testAlpha = daDataDesc.GetAlpha (itemPvalues, testStdDev);

			double testSEM = daDataDesc.GetStandardErrorOfMeasure (testStdDev, testAlpha);

			double[] itemPBS = daDataDesc.GetPointBiSerial (testStdDev, studentRawScores, dpDataFormat.BinaryData);
			double[] testStatistics = daDataDesc.GetDescriptiveStats (studentRawScores);
			double[,] MCFreq = dpDataFormat.GetFrequencies (itemType);

			double[] testAifD = daDataDesc.GetAlphaIfDropped (dpDataFormat.BinaryData);

			textBox1.Text += testName + Environment.NewLine;
			textBox1.Text += "No.\tType\tStandard\t\tAnswer\tPValue\t\tPBS\tAifD" + Environment.NewLine;
			for (int i = 0; i < dpDataFormat.BinaryData.GetLength (0); i++) {
				textBox1.Text += (i + 1) + "\t" + itemType [i] + "\t" + itemStandards [i] + "\t\t" +
				itemAnswers [i] + "\t" + itemPvalues [i] + "\t" + itemPBS [i] + "\t" + testAifD [i] + Environment.NewLine;
			}

			textBox1.Text += Environment.NewLine + "Descriptive Statistics" + Environment.NewLine;
			textBox1.Text += "STD: " + testStdDev + Environment.NewLine;
			textBox1.Text += "SEM: " + testSEM + Environment.NewLine;
			textBox1.Text += "Alp: " + testAlpha + Environment.NewLine;
			textBox1.Text += "Min: " + testStatistics [0] + Environment.NewLine;
			textBox1.Text += "Q1 : " + testStatistics [1] + Environment.NewLine;
			textBox1.Text += "Mea: " + testStatistics [2] + Environment.NewLine;
			textBox1.Text += "Med: " + testStatistics [3] + Environment.NewLine;
			textBox1.Text += "Q3 : " + testStatistics [4] + Environment.NewLine;
			textBox1.Text += "Max: " + testStatistics [5] + Environment.NewLine;
			textBox1.Text += Environment.NewLine;

			for (int i = 0; i < dpDataFormat.BinaryData.GetLength (0); i++) {

				switch (itemType [i]) {
				case "MC":
					textBox1.Text += MCFreq [i, 0] + "\t" + MCFreq [i, 1] + "\t" + MCFreq [i, 2] + "\t" + MCFreq [i, 3] + "\t" + MCFreq [i, 4] + Environment.NewLine;
					break;
				case "CR":
					textBox1.Text += dpDataFormat.CRAverages [i] + Environment.NewLine;
					break;
				}
			}
		}
	}
}
