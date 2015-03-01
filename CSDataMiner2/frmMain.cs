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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSDataMiner2
{
	public partial class frmMain : Form
	{

		public frmMain ()
		{
			InitializeComponent ();
			openFileDialog1.ShowDialog ();

			DataConnection dcDataGet = new DataConnection (openFileDialog1.FileName);
			DataParser dpDataFormat = new DataParser (dcDataGet.RawData, MethodOfDelete.ZeroReplace);
			//ListWise -> Removes the student from database if there is ANY omission found.
			//Pairwise -> (DEFAULT) Replaces any omission with NaN (not a number) but still allows present data to be analyzed.
			//ZeroReplace -> Same as above but NaN is a replaced with a zero.


			//********************
			//testing.......

			textBox1.Text += dpDataFormat.TestName;
			textBox1.Text += Environment.NewLine;

			for (int i = 0; i < dpDataFormat.BinaryData.GetLength (1); i++) {
				for (int j = 0; j < dpDataFormat.BinaryData.GetLength (0); j++) {
					textBox1.Text += dpDataFormat.BinaryData [j, i] + " ";
				}
				textBox1.Text += Environment.NewLine;
			}

			textBox1.Text += Environment.NewLine;

			for (int i = 0; i < dpDataFormat.AnswerKey.GetLength (0); i++) {
				textBox1.Text += dpDataFormat.Standards [i];
				textBox1.Text += Environment.NewLine;
			}
			for (int i = 0; i < dpDataFormat.AnswerKey.GetLength (0); i++) {
				textBox1.Text += " " + dpDataFormat.AnswerKey [i];
			}
		}
	}
}
