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
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CSDataMiner2
{
	class DataParser
	{
		const string NullValue = "NaN";
		//unique enough not to get confused with other datapoints in the file <Not a Number>

		public string[,] BinaryData { get; set; }

		public string[,] ChoiceData { get; set; }

		public string[] Standards { get; set; }

		public string[] AnswerKey { get; set; }

		public string[] ItemType { get; set; }

		public string TestName { get; set; }

		public DataFileLocations dfLoc = new DataFileLocations (5, 6, 5);
		//General to target datasource, FirstDataRow x FirstDataCol x LastDataCol offset

		public DataParser (DataTable InpData, MethodOfDelete ParseOption)
		{
			TestName = InpData.Rows [0].ItemArray [6].ToString ();

			//Extract the data and store in a 2D array; although the initial import of data is to a database, operations are much too slow to be useful.  Some overhead
			//is lost initially, but in the long run, array ops are quick and efficient and overall speed up the processing.

			ChoiceData = new string[InpData.Columns.Count - dfLoc.LastDataCol, InpData.Rows.Count - dfLoc.FirstDataRow];
			BinaryData = new string[ChoiceData.GetLength (0), ChoiceData.GetLength (1)];
			Standards = new string[BinaryData.GetLength (0)];
			AnswerKey = new string[BinaryData.GetLength (0)];

			//Pull the answer choice out
			for (int i = 0; i < InpData.Columns.Count - dfLoc.FirstDataCol - dfLoc.LastDataCol; i++) {
				Standards [i] = InpData.Rows [4].ItemArray [i + dfLoc.FirstDataCol].ToString ();
				//the standards are hardcoded @ row 4.

				for (int j = 0; j < InpData.Rows.Count - dfLoc.FirstDataRow; j++) {

					ChoiceData [i, j] = InpData.Rows [j + dfLoc.FirstDataRow].ItemArray [i + dfLoc.FirstDataCol].ToString ();

					if (ChoiceData [i, j].IndexOf ("+") > -1) {
						BinaryData [i, j] = "1";
						AnswerKey [i] = ChoiceData [i, j].Replace ("+", "");
					} else {
						if (ChoiceData [i, j].Trim ().Length < 1) {
							ChoiceData [i, j] = NullValue;
							BinaryData [i, j] = ParseOption == MethodOfDelete.ZeroReplace ? "0" : NullValue;
						} else {
							BinaryData [i, j] = "0";
						}
					}
				}
			}
		}
	}

	enum MethodOfDelete
	{
		Listwise,
		Pairwise,
		ZeroReplace
	}
}