//
//  DataParser.cs
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
using System.Data;

namespace CSDataMiner2
{
	class DataParser
	{
	
		public int?[,] BinaryData { get; set; }

		public string[,] ChoiceData { get; set; }

		public string[] Standards { get; set; }

		public string[] AnswerKey { get; set; }

		public double[] CRAverages { get; set; }

		public string[] ItemType { get; set; }

		public string TestName { get; set; }

		public string StatusReport { get; set; }

		public DataFileLocations dfLoc = new DataFileLocations (5, 6, 5);
		//General to target datasource, FirstDataRow x FirstDataCol x LastDataCol offset

		public DataParser (DataTable InpData, MethodOfDelete parseOption, bool replaceConstructedResponse)
		{
			switch (parseOption) {
			case MethodOfDelete.Listwise:
				StatusReport += "Listwise Deletion -> students with omitted data are removed before analyses.";
				break;
			case MethodOfDelete.Pairwise:
				StatusReport += "Pairwise Deletion -> students with omitted data are included where possible in the analyses.";
				break;
			case MethodOfDelete.ZeroReplace:
				StatusReport += "Replace Deletion -> students with ommitted data are given a zero, where appropriate, for the analyses.";
				break;
			}
		}

		public void DataClip (DataTable InpData)
		{
			// NEW SUB CLIPTABLE*****************************************************************
			char[] INVALID_CHARACTERS = "!@#$%NY".ToCharArray ();
			int NumberDroppedStudents = 0;

			for (int i = InpData.Rows.Count - 1; i >= dfLoc.FirstDataRow; i--) {
				for (int j = InpData.Columns.Count - dfLoc.LastDataCol - 1; j > dfLoc.FirstDataCol; j--) {
					if ((InpData.Rows [i].ItemArray [j] == DBNull.Value) | (InpData.Rows [i].ItemArray [j].ToString ().IndexOfAny (INVALID_CHARACTERS) > -1)) {
						InpData.Rows [i].Delete ();
						NumberDroppedStudents += 1;
						break; //no need to continue iterating through a deleted row, break this loop and move to next one.
					}
				}
			}
			InpData.AcceptChanges (); 

			if (NumberDroppedStudents > 0)
				StatusReport += "There were " + NumberDroppedStudents + " student records dropped because of improper record format or selected missing data option.";
		}

		public void Parse ()
		{
			ChoiceData = new string[InpData.Columns.Count - dfLoc.LastDataCol - dfLoc.FirstDataCol, InpData.Rows.Count - dfLoc.FirstDataRow];
			BinaryData = new int?[ChoiceData.GetLength (0), ChoiceData.GetLength (1)];
			Standards = new string[BinaryData.GetLength (0)];
			AnswerKey = new string[Standards.GetLength (0)];

			TestName = InpData.Rows [0].ItemArray [6].ToString ();

			for (int i = 0; i < InpData.Columns.Count - dfLoc.FirstDataCol - dfLoc.LastDataCol; i++) {
				Standards [i] = InpData.Rows [4].ItemArray [i + dfLoc.FirstDataCol].ToString ();
				//the standards are hardcoded @ row 4.

				for (int j = 0; j < InpData.Rows.Count - dfLoc.FirstDataRow; j++) {

					ChoiceData [i, j] = InpData.Rows [j + dfLoc.FirstDataRow].ItemArray [i + dfLoc.FirstDataCol].ToString ().Trim ();

					if (ChoiceData [i, j].IndexOf ("+") == 0) {
						BinaryData [i, j] = 1;
						AnswerKey [i] = ChoiceData [i, j].Replace ("+", "");
					} else {
						if (ChoiceData [i, j].Length < 1) {
							ChoiceData [i, j] = null;
							if (parseOption == MethodOfDelete.ZeroReplace) {
								BinaryData [i, j] = 0;
							} else {
								BinaryData [i, j] = null;
							}
						} else {
							BinaryData [i, j] = 0;
						}
					}
				}
				if (string.IsNullOrEmpty (AnswerKey [i]))
					AnswerKey [i] = "";
			}

		}

		public void GetItemType ()
		{
			// NEW SUB GETITEMTYPE*****************************************************************
			char[] VALID_NUMERICS = "0123456789-.".ToCharArray ();
			char[] VALID_ALPHAS = "ABCDEFGHIJ".ToCharArray ();
			ItemType = new string[AnswerKey.GetLength (0)]; 

			for (int i = 0; i < AnswerKey.GetLength (0); i++) {
				string s = AnswerKey [i];
				string iType = "CR";
				if (s.IndexOf (",") == 1) {
					iType = "MS";
				} else {
					if (s.Length == 1 && s.IndexOfAny (VALID_ALPHAS) == 0) {
						iType = "MC";
					} else {
						if (s.IndexOf ("-") > -1 | s.IndexOf (".") > -1 | Standards [i].StartsWith ("MAFS")) {
							iType = "GR";
						}
					}
				}
				ItemType [i] = AnswerKey [i] == "NA" ? "MC" : iType;
			}
			StatusReport += "Item types are guessed from available data, gridded response and constructed response may be incorrectly marked.";
			if (replaceConstructedResponse)
				CRAverages = ConvertConstructedResponse ();
		}


		public double[] ConvertConstructedResponse ()
		{
			var result = new double[ChoiceData.GetLength (0)];
			for (int i = 0; i < ChoiceData.GetLength (0); i++) {
				if (ItemType [i] != "CR")
					continue;
				for (int j = 0; j < ChoiceData.GetLength (1); j++) {
					string s = ChoiceData [i, j].Replace ("+", "");
					if (s == null)
						continue;
					result [i] += double.Parse (s);
				}
				result [i] = Math.Round (result [i] / ChoiceData.GetLength (1), 2);
				for (int j = 0; j < ChoiceData.GetLength (1); j++) {
					string s = ChoiceData [i, j].Replace ("+", "");
					if (s == null)
						continue;
					BinaryData [i, j] = (int)(double.Parse (s) >= result [i] ? 1 : 0);
				}
			}
			return result;
		}

		public double [,] GetFrequencies (string[] type)
		{
			var result = new double[ChoiceData.GetLength (0), 5];
			for (int i = 0; i < ChoiceData.GetLength (0); i++) {
				if (type [i] != "MC")
					continue;
				for (int j = 0; j < ChoiceData.GetLength (1); j++) {
					string s = ChoiceData [i, j].Replace ("+", "");
					if (s == "A" | s == "F") {
						result [i, 0] += 1;
						continue;
					}
					if (s == "B" | s == "G") {
						result [i, 1] += 1;
						continue;
					}
					if (s == "C" | s == "H") {
						result [i, 2] += 1;
						continue;
					}
					if (s == "D" | s == "J") {
						result [i, 3] += 1;
						continue;
					}
					if (s == null) {
						result [i, 4] += 1;
						continue;
					}
				}
				result [i, 0] = Math.Round (result [i, 0] / ChoiceData.GetLength (1) * 100, 1);
				result [i, 1] = Math.Round (result [i, 1] / ChoiceData.GetLength (1) * 100, 1);
				result [i, 2] = Math.Round (result [i, 2] / ChoiceData.GetLength (1) * 100, 1);
				result [i, 3] = Math.Round (result [i, 3] / ChoiceData.GetLength (1) * 100, 1);
				result [i, 4] = Math.Round (result [i, 4] / ChoiceData.GetLength (1) * 100, 1);
			}
			return result;
		}
	}

	enum MethodOfDelete
	{
		Listwise,
		Pairwise,
		ZeroReplace
	}
}