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

			var dcDataGet = new DataConnection (openFileDialog1.FileName, Source.Eduphoria);
			var dpDataFormat = new DataParser (dcDataGet.RawData, MethodOfDelete.Pairwise);
			var daDataDesc = new DataAnalyzer (dpDataFormat.NullValue, dpDataFormat.BinaryData);
			//ListWise -> Removes the student from database if there is ANY omission found.
			//Pairwise -> (DEFAULT) Replaces any omission with NaN (not a number) but still allows present data to be analyzed.
			//ZeroReplace -> Same as above but NaN is a replaced with a zero.

			string testName = dpDataFormat.TestName;
			string[] itemType = dpDataFormat.ItemType;
			string[] itemStandards = dpDataFormat.Standards;
			string[] itemAnswers = dpDataFormat.AnswerKey;
			double[] itemPvalues = daDataDesc.GetPValues (dpDataFormat.BinaryData);
			double testStdDev = daDataDesc.GetStandardDeviation (dpDataFormat.BinaryData);
			double testAlpha = daDataDesc.GetAlpha (itemPvalues, testStdDev);
			double testSEM = daDataDesc.GetStandardErrorOfMeasure (testStdDev, testAlpha);

			double[] studentRawScores = daDataDesc.GetRawScores (dpDataFormat.BinaryData);
			double[] studentPercentScores = daDataDesc.GetPercentScores (dpDataFormat.BinaryData);

			double[] itemPBS = daDataDesc.GetPointBiSerial (testStdDev, studentRawScores, dpDataFormat.BinaryData);

			textBox1.Text += testName + Environment.NewLine;
			for (int i = 0; i < dpDataFormat.BinaryData.GetLength (0); i++) {
				textBox1.Text += (i + 1) + "  :  " + itemType [i] + "  :  " + itemStandards [i] + " :\t\t" +
				itemAnswers [i] + " :\t" + itemPvalues [i] + " :\t" + itemPBS [i] + " :\t" + Environment.NewLine;
			}
			textBox1.Text += "STD: " + testStdDev + Environment.NewLine;
			textBox1.Text += "SEM: " + testSEM + Environment.NewLine;
			textBox1.Text += "Alp: " + testAlpha + Environment.NewLine;
			foreach (double n in daDataDesc.GetDescriptiveStats (daDataDesc.GetRawScores (dpDataFormat.BinaryData ) )) {
				textBox1.Text += n + Environment.NewLine;
			}
		}
	}
}
