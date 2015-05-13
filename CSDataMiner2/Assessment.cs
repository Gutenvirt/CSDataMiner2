//
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
using System.Linq;

namespace CSDataMiner2
{
	public class Assessment
	{
		public string Output { get; set; }

		string testName;
		string[] itemType;
		string[] itemStandards;
		string[] itemAnswers;

		float[] studentRawScores;
		float[] studentPercentScores;
		int[] studentHistogram;

		int testLength;
		int studentLength;

		float[] itemPvalues;
		float testStdDev;
		float testAlpha;
		float testMean;
		float testSkew;

		float testSEM;

		float[] itemPBS;
		float[] testStatistics;
		float[,] MCFreq;

		float[] testAifD;


		public Assessment (string filename)
		{
			var dcDataGet = new DataConnection (filename);
			var dpDataFormat = new DataParser (dcDataGet.RawData);
			//ListWise -> Removes the student from database if there is ANY omission found.
			//Pairwise -> (DEFAULT) Replaces any omission with NaN (not a number) but still allows present data to be analyzed.
			//ZeroReplace -> Same as above but NaN is a replaced with a zero.

			testName = dpDataFormat.TestName;
			itemType = dpDataFormat.ItemType;
			itemStandards = dpDataFormat.Standards;
			itemAnswers = dpDataFormat.AnswerKey;

			studentRawScores = DataAnalyzer.GetRawScores (dpDataFormat.BinaryData);
			studentPercentScores = DataAnalyzer.GetPercentScores (dpDataFormat.BinaryData);

			testLength = itemAnswers.GetLength (0);
			studentLength = studentRawScores.GetLength (0);

			itemPvalues = DataAnalyzer.GetPValues (dpDataFormat.BinaryData);
			testStdDev = DataAnalyzer.GetStandardDeviation (dpDataFormat.BinaryData);
			testAlpha = DataAnalyzer.GetAlpha (itemPvalues, testStdDev);

			testSEM = DataAnalyzer.GetStandardErrorOfMeasure (testStdDev, testAlpha);

			itemPBS = DataAnalyzer.GetPointBiSerial (testStdDev, studentRawScores, dpDataFormat.BinaryData);
			testStatistics = DataAnalyzer.GetDescriptiveStats (studentRawScores);
			testMean = testStatistics [2];
			studentHistogram = DataAnalyzer.GetStudentHistogramValues (studentRawScores, testLength);

			testSkew = DataAnalyzer.GetTestSkew (testMean, testStatistics [3], testStdDev);

			MCFreq = dpDataFormat.GetFrequencies (itemType);

			testAifD = DataAnalyzer.GetAlphaIfDropped (dpDataFormat.BinaryData);

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
			Output += Environment.NewLine;
			Output += "Histogram (Max:" + studentHistogram.Max () + ")" + Environment.NewLine;
			for (int i = 0; i < studentHistogram.GetLength (0); i++) {
				Output += i + ": " + ((double)studentHistogram [i] / studentHistogram.Max ()) + Environment.NewLine;
			}

			Output += HTMLOut ();
		}

		public string HTMLOut ()
		{
			string strHTML = "";
			int _tableWidth = 660;
			int _gDivHeight = 250;
			int _gDivWidth = 515;
			int _barWidth = 45;
			int _medianLeft = 11 + (int)(testMean / testLength * _gDivWidth);
			int _medianHeight = 231;
			int _stdLeft = 11 + (int)((testMean - testStdDev) / testLength * _gDivWidth);
			int _stdWidth = (int)(testStdDev / testLength * 2 * _gDivWidth);
			int _stdTop = 234;
			int p100Top = 0;
			int p75Top = (int)(_gDivHeight / 4);
			int p50Top = (int)(_gDivWidth / 2);
			int p25Top = (int)(_gDivWidth / 4 * 3);

			strHTML = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01//EN\"><HTML><HEAD><meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\"><meta name=\"author\" content=\"Chris Stefancik 2015\"><TITLE>" + testName + "</TITLE>";

			strHTML += "<STYLE  type=\"text/css\">";

			strHTML += "\ntable { border-collapse: collapse; border-color: #c1c1c1; border-spacing: 0; border-style: solid; border-width: 1px 0 0 1px; vertical-align: middle; width: " + _tableWidth + "px; }";
			strHTML += "\nth { background-color: #edf2f9; border-color: #b0b7bb; border-style: solid; border-width: 0 1px 1px 0; color: #112277; font-family: Arial, Helvetica, Helv; font-size: small; font-style: normal; font-weight: bold; padding: 3px 6px; text-align: center; vertical-align: middle; }";
			strHTML += "\ntd { background-color: #FFFFFF; border-color: #c1c1c1; border-style: solid; border-width: 0 1px 1px 0; font-family: Arial, Helvetica, Helv; font-size: small; font-style: normal; font-weight: normal; padding: 3px 6px; text-align: right; vertical-align: middle; }";
			strHTML += "\n.graph { height: " + _gDivHeight + "px; position: relative; width: " + _gDivWidth + "px; }";
			strHTML += "\n.bar { background-color: #edf2f9; border: 1px solid #c1c1c1; display: inline-block; margin: 1px; position: relative; vertical-align: baseline; width: " + _barWidth + "px; }";
			strHTML += "\n.median { background-color: #FBE2E0; border: 1px solid #9F9F9F; display: inline-block; height: " + _medianHeight + "px; left: " + _medianLeft + "px; margin: 0px; position: absolute; top: 1px; vertical-align: baseline; width: 0px; }";
			strHTML += "\n.std { border: 1px solid #9F9F9F; display: inline-block; left: " + _stdLeft + "px; margin: 0px; position: absolute; top: " + _stdTop + "px; vertical-align: baseline; width: " + _stdWidth + "px; }";
			strHTML += "\n.xlabel { border: 1px solid #FFFFFF; display: inline-block; font-family: Arial, Helvetica, Helv; font-size: x-small; font-style: normal; font-weight: normal; margin: 1px; position: relative; text-align: center; vertical-align: baseline; width: " + _barWidth + "px; }";
			strHTML += "\n.ylabel { display: inline-block; font-family: Arial, Helvetica, Helv; font-size: x-small; font-style: normal; font-weight: normal; left: 0px; position: absolute; text-align: left; }";
			strHTML += "\n.center { text-align: center; }";
			strHTML += "\n.left { text-align: left; }";
			strHTML += "\n.warning { background-color: #FBE2E0; }";

			strHTML += "</STYLE>";

			strHTML += "</HEAD>\n<BODY><TABLE><tr><th colspan=\"6\"><p>" + testName + "</p></th><th colspan=\"4\"><p>Test Analysis Report</p></th></tr><tr><td class=\"center\" colspan=\"3\">Date: " + DateTime.Today + "</td><td class=\"center\" colspan=\"3\">User: " + "USERNAME" + "</td><td class=\"center\" colspan=\"4\">CDS 2015</td></tr>";

			strHTML += "</table><p></p><table>";
			strHTML += "<tr><th colspan=\"2\">Raw Score</th><th colspan=\"8\">Percent Score Distribution</th></tr>";

			if (testLength < 3) {
				strHTML += "<tr><td>Items</td><td class=\"warning\">" + testLength + "</td>";
			} else {
				strHTML += "<tr><td>Items</td><td>" + testLength + "</td>";
			}
			strHTML += "<td ROWSPAN=\"12\" colspan=\"8\"><div><div class=\"graph\">";
			for (int i = 0; i < 10; i++) {
				strHTML += "<div style=\"height: " + ((double)studentHistogram [i] / studentHistogram.Max () * 230) + "px\" class=\"bar\"></div>";
			}
			strHTML += "<div class=\"std\"></div>";
			strHTML += "<div class=\"median\"></div>";
			strHTML += "<div class=\"ylabel\" style=\"top: " + p100Top + "px\">" + (double)(studentHistogram.Max () / studentLength * 100) + "%</div>";
			strHTML += "<div class=\"ylabel\" style=\"top: " + p75Top + "px\">" + (double)(studentHistogram.Max () / studentLength * 75) + "%</div>";
			strHTML += "<div class=\"ylabel\" style=\"top: " + p50Top + "px\">" + (double)(studentHistogram.Max () / studentLength * 50) + "%</div>";
			strHTML += "<div class=\"ylabel\" style=\"top: " + p25Top + "px\">" + (int)(studentHistogram.Max () / studentLength * 25) + "%</div>";
		
			for (int i = 0; i < 10; i++) {
				if (i == 9)
					strHTML += "<div class=\"xlabel\">" + (i * 10) + "-100</div>";
				else
					strHTML += "<div class=\"xlabel\">" + (i * 10) + "-" + (i * 10 + 9) + "</div>";
				strHTML += "</div></div></tr>";
			}

			if (studentLength < 25)
				strHTML += "<tr><td>Students</td><td class=\"warning\">" + studentLength + "</td></tr>";
			else
				strHTML += "<tr><td>Students</td><td>" + studentLength + "</td></tr>";

			strHTML += "<tr><td>Min</td><td>" + studentRawScores.Min () + "</td></tr>";
			strHTML += "<tr><td>Q1</td><td>" + testStatistics [1] + "</td></tr>";
			strHTML += "<tr><td>Mean</td><td>" + testMean.ToString ("0.00") + "</td></tr>";
			strHTML += "<tr><td>Median</td><td>" + testStatistics [3] + "</td></tr>";
			strHTML += "<tr><td>Q3</td><td>" + testStatistics [4] + "</td></tr>";
			strHTML += "<tr><td>Max</td><td>" + studentRawScores.Max () + "</td></tr>";
			strHTML += "<tr><td>Std Dev</td><td>" + testStdDev.ToString ("0.00") + "</td></tr>";

			if (testSkew > 0)
				strHTML += "<tr><td>Skew</td><td>&#8592; " + testSkew.ToString ("0.00") + "</td></tr>";
			else
				strHTML += "<tr><td>Skew</td><td>&#8594; " + testSkew.ToString ("0.00") + "</td></tr>";
			if (testAlpha < .7 | testAlpha > 1)
				strHTML += "<tr><td>Alpha</td><td class=\"warning\">" + testAlpha.ToString ("0.00") + "</td></tr>";
			else
				strHTML += "<tr><td>Alpha</td><td>" + testAlpha.ToString ("0.00") + "</td></tr>";
			strHTML += "<tr><td>SEM</td><td>" + testSEM.ToString ("0.00") + "</td></tr>";

			/*Notes section
			if (nNumberStudentsDrop > 0) {
				strHTML += "<tr><td class=\"left\" colspan=\"11\">";
				strHTML += "Note: Some students (" + nNumberStudentsDrop + " total) were dropped because of complete test omission or an error in the data record; consider rescoring the test and re-running this analysis tool.";
			} 
			*/

			strHTML += "</table><p></p>";


            /* Test Design Section

            strHTML += "<table>";
                    

            strHTML += "<tr><th colspan=\"4\">Item Difficulty</th>" +
                "<th>% of Items</th>" +
                "<th colspan=\"4\">Item Discrimination</th>" +
                "<th colspan=\"1\">% of Items</th></tr>";

            strHTML += "<tr><td colspan=\"4\" class=\"left\">Easy (Higher than 70%)</td>" +
				"<td>" + Math.Round(itemDifficulty(2) / nNumberColumns * 100, 1) + "</td>" +
				"<td colspan=\"4\" class=\"left\">Good (Higher than 0.3)</td>" +
				"<td colspan=\"1\">" + Math.Round(itemDiscrimination(2) / nNumberColumns * 100, 1) + "</td></tr>";

				strHTML += "<tr><td colspan=\"4\" class=\"left\">Moderate (40% to 70%)</td>" +
				"<td>" + Math.Round(itemDifficulty(1) / nNumberColumns * 100, 1) + "</td>" +
				"<td colspan=\"4\" class=\"left\">Acceptable (0.2 to 0.3)</td>" +
				"<td colspan=\"1\">" + Math.Round(itemDiscrimination(1) / nNumberColumns * 100, 1) + "</td></tr>";

				strHTML += "<tr><td colspan=\"4\" class=\"left\">Hard (Less than 40%)</td>" +
				"<td>" + Math.Round(itemDifficulty(0) / nNumberColumns * 100, 1) + "</td>" +
				"<td colspan=\"4\" class=\"left\">Needs Review (Less than 0.2)</td>" +
				"<td colspan=\"1\">" + Math.Round(itemDiscrimination(0) / nNumberColumns * 100, 1) + "</td></tr>";

                strHTML += "</table><p></p>";
			*/
				
			//Item Review Section

				//Multiple Choice
				if (hasMC)
            {
				strHTML += "<table>";
				strHTML += "<tr><th>Item</th><th>P-Value</th><th>PBS</th><th>Alpha IfD</th><th>Answer</th><th>% C1</th><th>% C2</th><th>% C3</th><th>% C4</th><th>% Om</th></tr>";
            }

				For _a = 0 To nNumberColumns - 1
				If itemStrData(_a, StrData.Type) = "MC" Then
				strHTML += "<tr>"
				strHTML += "<td>" +a + 1 + " " + itemStrData(_a, StrData.Type) + "</td>"
				If ItemDblData(_a, DblData.PV) < 0.2 Or ItemDblData(_a, DblData.PV) > 0.9 Then
				strHTML += "<td class=\"warning\">" + ItemDblData(_a, DblData.PV).ToString("0.00") + "</td>"
				Else
				strHTML += "<td>" + ItemDblData(_a, DblData.PV).ToString("0.00") + "</td>"
				End If
				If ItemDblData(_a, DblData.PBS) < 0.2 Then
				strHTML += "<td class=\"warning\">" + ItemDblData(_a, DblData.PBS).ToString("0.00") + "</td>"
				Else
				strHTML += "<td>" + ItemDblData(_a, DblData.PBS).ToString("0.00") + "</td>"
				End If

				strHTML += "<td>" + ItemDblData(_a, DblData.AIfD).ToString("0.00") + "</td>"
				strHTML += "<td class=\"center\">" + itemStrData(_a, StrData.Answer) + "</td>"
				strHTML += "<td>" + Math.Round(ItemIntData(_a, IntData.MC1) / nNumberRows * 100, 0) + "</td>"
				strHTML += "<td>" + Math.Round(ItemIntData(_a, IntData.MC2) / nNumberRows * 100, 0) + "</td>"
				strHTML += "<td>" + Math.Round(ItemIntData(_a, IntData.MC3) / nNumberRows * 100, 0) + "</td>"
				strHTML += "<td>" + Math.Round(ItemIntData(_a, IntData.MC4) / nNumberRows * 100, 0) + "</td>"
				strHTML += "<td>" + Math.Round(ItemIntData(_a, IntData.Omissions) / nNumberRows * 100, 2) + "</td>"
				strHTML += "</tr>"
				End If
				Next
				strHTML += "</table>"
				strHTML += "<p></p>"
				End If


				'Multiple Select
				If hasMS = True Then
				strHTML += "<table>"
				strHTML += "<tr><th>Item</th><th>P-Value</th><th>PBS</th><th>Alpha IfD</th><th>Answer</th><th>% C1</th><th>% C2</th><th>% C3</th><th>% C4</th><th>% Om</th></tr>"

				For _a = 0 To nNumberColumns - 1
				If itemStrData(_a, StrData.Type) = "MS" Then
				strHTML += "<tr>"
				strHTML += "<td>" +a + 1 + " " + itemStrData(_a, StrData.Type) + "</td>"
				If ItemDblData(_a, DblData.PV) < 0.2 Or ItemDblData(_a, DblData.PV) > 0.9 Then
				strHTML += "<td class=\"warning\">" + ItemDblData(_a, DblData.PV).ToString("0.00") + "</td>"
				Else
				strHTML += "<td>" + ItemDblData(_a, DblData.PV).ToString("0.00") + "</td>"
				End If
				If ItemDblData(_a, DblData.PBS) < 0.2 Then
				strHTML += "<td class=\"warning\">" + ItemDblData(_a, DblData.PBS).ToString("0.00") + "</td>"
				Else
				strHTML += "<td>" + ItemDblData(_a, DblData.PBS).ToString("0.00") + "</td>"
				End If

				strHTML += "<td>" + ItemDblData(_a, DblData.AIfD).ToString("0.00") + "</td>"
				strHTML += "<td class=\"center\">" + itemStrData(_a, StrData.Answer) + "</td><td colspan=\"4\"></td>"
				strHTML += "<td>" + Math.Round(ItemIntData(_a, IntData.Omissions) / nNumberRows * 100, 2) + "</td>"
				strHTML += "</tr>"
				End If
				Next
				strHTML += "</table>"
				strHTML += "<p></p>"
				End If

				'Gridded Response

				If hasGR = True Then
				strHTML += "<table>"
				strHTML += "<tr><th>Item</th><th>P-Value</th><th>PBS</th><th>Alpha IfD</th><th>Answer</th><th>Percent Below</th><th>Percent Above</th><th>% Om</th></tr>"

				For _a = 0 To nNumberColumns - 1
				If itemStrData(_a, StrData.Type) = "GR" Then
				strHTML += "<tr>"
				strHTML += "<td>" +a + 1 + " " + itemStrData(_a, StrData.Type) + "</td>"
				If ItemDblData(_a, DblData.PV) < 0.2 Or ItemDblData(_a, DblData.PV) > 0.9 Then
				strHTML += "<td class=\"warning\">" + ItemDblData(_a, DblData.PV).ToString("0.00") + "</td>"
				Else
				strHTML += "<td>" + ItemDblData(_a, DblData.PV).ToString("0.00") + "</td>"
				End If
				If ItemDblData(_a, DblData.PBS) < 0.2 Then
				strHTML += "<td class=\"warning\">" + ItemDblData(_a, DblData.PBS).ToString("0.00") + "</td>"
				Else
				strHTML += "<td>" + ItemDblData(_a, DblData.PBS).ToString("0.00") + "</td>"
				End If

				strHTML += "<td>" + ItemDblData(_a, DblData.AIfD).ToString() + "</td>"
				strHTML += "<td class=\"center\">" + itemStrData(_a, StrData.Answer) + "</td>"
				strHTML += "<td>" + (100 - ItemDblData(_a, DblData.PV) * 100 - c_CRPassRate(_a) - ItemIntData(_a, IntData.Omissions) / nNumberRows * 100).ToString("0.00") + "</td>"
				strHTML += "<td>" + c_CRPassRate(_a).ToString("0.00") + "</td>"
				strHTML += "<td>" + Math.Round(ItemIntData(_a, IntData.Omissions) / nNumberRows * 100, 2) + "</td>"
				strHTML += "</tr>"
				End If
				Next
				strHTML += "</table>"
				strHTML += "<p></p>"
				End If

				'Constructed Response
				If hasCR = True Then
				strHTML += "<table>"
				strHTML += "<tr><th>Item</th><th>P-Value</th><th>PBS</th><th>Alpha IfD</th><th>Mean</th><th>Percent Below</th><th>Percent at or Above</th><th>% Om</th></tr>"

				For _a = 0 To nNumberColumns - 1
				If itemStrData(_a, StrData.Type) = "CR" Then
				strHTML += "<tr>"
				strHTML += "<td>" +a + 1 + " " + itemStrData(_a, StrData.Type) + "</td>"
				If ItemDblData(_a, DblData.PV) < 0.2 Or ItemDblData(_a, DblData.PV) > 0.9 Then
				strHTML += "<td class=\"warning\">" + ItemDblData(_a, DblData.PV).ToString("0.00") + "</td>"
				Else
				strHTML += "<td>" + ItemDblData(_a, DblData.PV).ToString("0.00") + "</td>"
				End If
				If ItemDblData(_a, DblData.PBS) < 0.2 Then
				strHTML += "<td class=\"warning\">" + ItemDblData(_a, DblData.PBS).ToString("0.00") + "</td>"
				Else
				strHTML += "<td>" + ItemDblData(_a, DblData.PBS).ToString("0.00") + "</td>"
				End If

				strHTML += "<td>" + ItemDblData(_a, DblData.AIfD).ToString("0.00") + "</td>"
				strHTML += "<td class=\"center\">" + itemStrData(_a, StrData.Answer).Substring(0, itemStrData(_a, StrData.Answer).IndexOf(".") + 3) + "</td>"
				strHTML += "<td>" + (100 - c_CRPassRate(_a) - (ItemIntData(_a, IntData.Omissions) / nNumberRows * 100)).ToString("0.00") + "</td>"
				strHTML += "<td>" + c_CRPassRate(_a).ToString("0.00") + "</td>"
				strHTML += "<td>" + Math.Round(ItemIntData(_a, IntData.Omissions) / nNumberRows * 100, 2) + "</td>"
				strHTML += "</tr>"
				End If
				Next
				strHTML += "</table>"
				strHTML += "<p></p>"
				End If
				*/

			//References Section

				
			strHTML += "<table><tr><th colspan=\"10\">Citations</th></tr><tr><td colspan=\"10\" class=\"left\">";
			strHTML += "<p>Afifi, A. A., + Elashoff, R. M. (1966). Missing observations in multivariate statistics I. Review of the literature. <em>Journal of the American Statistical Association, </em> 61(315), 595-604.<br></br>";
			strHTML += "Brown, J. D. (2001). Point-biserial correlation coefficients. <em>JALT Testing + Evaluation SIG Newsletter, </em> 5(3), 12-15.<br></br>";
			strHTML += "Brown, S. (2011). Measures of shape: Skewness and Kurtosis. Retrieved on December, 31, 2014.<br></br>";
			strHTML += "Ebel, R. L. (1950). Construction and validation of educational tests. <em>Review of Educational Research,</em> 87-97.<br></br>";
			strHTML += "Ebel, R. L. (1965). Confidence Weighting and Test Reliability. <em>Journal of Educational Measurement,</em> 2(1), 49-57.<br></br>";
			strHTML += "Kelley, T., Ebel, R., + Linacre, J. M. (2002). Item discrimination indices. <em>Rasch Measurement Transactions,</em> 16(3), 883-884.<br></br>";
			strHTML += "Krishnan, V. (2013). The Early Child Development Instrument (EDI): An item analysis using Classical Test Theory (CTT) on Alberta\'s data. <em>Early Child Mapping (ECMap) Project Alberta, Community-University Partnership (CUP), Faculty of Extension, University of Alberta, Edmonton, Alberta.</em><br></br>";
			strHTML += "Matlock-Hetzel, S. (1997). Basic Concepts in Item and Test Analysis.<br></br>Pearson, K. (1895). Contributions to the mathematical theory of evolution. II. Skew variation in homogeneous material. <em>Philosophical Transactions of the Royal Society of London. A, </em>343-414.<br></br>";
			strHTML += "Richardson, M. W., + Stalnaker, J. M. (1933). A note on the use of bi-serial r in test research. <em>The Journal of General Psychology,</em> 8(2), 463-465.<br></br>";
			strHTML += "Yu, C. H., + Ds, P. (2012). A Simple Guide to the Item Response Theory (IRT) and Rasch Modeling.<br></br>Zeng, J., + Wyse, A. (2009). Introduction to Classical Test Theory. <em>Michigan, Washington, US.</em>";
			strHTML += "</p></td></tr></table>";
				
			strHTML += "</table><p></p>";
			
			strHTML += "</HTML>";
			return strHTML;
		}
	}
}
