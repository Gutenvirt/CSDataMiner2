/*
CSDataMiner - Extract and analyze data from MS Excel(c) files.
Copyright (C) 2015 Chris Stefancik gutenvirt@gmail.com

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 */

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
			var dpDataFormat = new DataParser (dcDataGet.RawData, MethodOfDelete.Pairwise);
			var daDataDesc = new DataAnalyzer (dpDataFormat.BinaryData);
			//ListWise -> Removes the student from database if there is ANY omission found.
			//Pairwise -> (DEFAULT) Replaces any omission with NaN (not a number) but still allows present data to be analyzed.
			//ZeroReplace -> Same as above but NaN is a replaced with a zero.


			//********************
			//testing.......

			textBox1.Text += dpDataFormat.TestName;
			textBox1.Text += Environment.NewLine;

			/*  Add feature to display random 10 rows of data**********************
			for (int i = 0; i < dpDataFormat.BinaryData.GetLength (1); i++) {
				for (int j = 0; j < dpDataFormat.BinaryData.GetLength (0); j++) {
					textBox1.Text += dpDataFormat.BinaryData [j, i] + " ";
				}
				textBox1.Text += Environment.NewLine;
			}
			*/

			textBox1.Text += Environment.NewLine;

			for (int i = 0; i < dpDataFormat.AnswerKey.GetLength (0); i++) {
				textBox1.Text += (i + 1) + " :\t" + dpDataFormat.AnswerKey [i] + "\t-> " + dpDataFormat.ItemType [i] + "\t @ " + dpDataFormat.Standards [i] + "\t\t :: " + daDataDesc.GetPValues () [i].ToString ();
				textBox1.Text += Environment.NewLine;
			}

			textBox1.Text += Environment.NewLine;

			foreach (string s in dpDataFormat.StatusReport.Split ('.'))
				textBox1.Text += s + Environment.NewLine;
		}
	}
}
