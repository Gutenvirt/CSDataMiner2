//
//  FileIO.cs
//
//  Author:
//       Christopher Stefancik <gutenvirt@gmail.com>
//
//  Copyright (c) 2015 2015 CD Stefancik
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
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace CSDataMiner2
{
    public static class FileIO
    {
        public static void WriteCSV(Assessment selTest)
        {
            string[] strBuffer = new string[selTest.fileParser.BinaryData.GetLength(1) + 1];

            for (int i = 0; i < selTest.fileParser.BinaryData.GetLength(0); i++)
            {
                strBuffer[0] += i == selTest.fileParser.BinaryData.GetLength(0) - 1 ? "Q" + (i + 1).ToString() : "Q" + (i + 1).ToString() + ",";
            }

            for (int j = 0; j < selTest.fileParser.BinaryData.GetLength(1); j++)
            {
                for (int i = 0; i < selTest.fileParser.BinaryData.GetLength(0); i++)
                {
                    if (selTest.fileParser.BinaryData[i, j] == 255)
                        strBuffer[j + 1] += i == selTest.fileParser.BinaryData.GetLength(0) - 1 ? "." : ".,";
                    else
                        strBuffer[j + 1] += i == selTest.fileParser.BinaryData.GetLength(0) - 1 ? selTest.fileParser.BinaryData[i, j].ToString() : selTest.fileParser.BinaryData[i, j].ToString() + ',';
                }
            }
            
            if (selTest.xlsxLoader == null)
                System.IO.File.WriteAllLines(selTest.txtLoader.WorkingDirectory + '\\' + selTest.fileParser.TestName + ".csv", strBuffer);
            else
                System.IO.File.WriteAllLines(selTest.xlsxLoader.WorkingDirectory + '\\' + selTest.fileParser.TestName + ".csv", strBuffer);
        }

        public static string DiagnosticOutput(Assessment selTest)
        {
            var sbOutput = new StringBuilder();
            sbOutput.Append(selTest.fileParser.TestName + Environment.NewLine + Environment.NewLine);

            sbOutput.Append(Environment.NewLine + "Descriptive Statistics" + Environment.NewLine);
            sbOutput.Append("──────────────┬──────────" + Environment.NewLine);
            sbOutput.Append("Obsvations    │ " + selTest.studentLength + Environment.NewLine);
            sbOutput.Append("Variables     │ " + selTest.testLength + Environment.NewLine);
            sbOutput.Append("──────────────┼──────────" + Environment.NewLine);
            sbOutput.Append("Standard Dev. │ " + selTest.testStdDev.ToString("0.00") + Environment.NewLine);
            sbOutput.Append("SEM           │ " + selTest.testSEM.ToString("0.00") + Environment.NewLine);
            sbOutput.Append("Test Alpha    │ " + selTest.testAlpha.ToString("0.00") + Environment.NewLine);
            sbOutput.Append("──────────────┼──────────" + Environment.NewLine);
            sbOutput.Append("Minimum       │ " + selTest.testStatistics[0].ToString("0.00") + Environment.NewLine);
            sbOutput.Append("Quartile 1    │ " + selTest.testStatistics[1].ToString("0.00") + Environment.NewLine);
            sbOutput.Append("Mean          │ " + selTest.testStatistics[2].ToString("0.00") + Environment.NewLine);
            sbOutput.Append("Median        │ " + selTest.testStatistics[3].ToString("0.00") + Environment.NewLine);
            sbOutput.Append("Qquartile 3   │ " + selTest.testStatistics[4].ToString("0.00") + Environment.NewLine);
            sbOutput.Append("Maximum       │ " + selTest.testStatistics[5].ToString("0.00") + Environment.NewLine);
            sbOutput.Append("──────────────┴──────────" + Environment.NewLine);
            sbOutput.Append(Environment.NewLine);

            sbOutput.Append("No.\tType\tStandard\t\tAnswer\tPValue\t\tPBS\tAifD" + Environment.NewLine);
            sbOutput.Append("──────────────────────────────────────────────────────────────────────────────" + Environment.NewLine);
            for (int i = 0; i < selTest.fileParser.BinaryData.GetLength(0); i++)
            {
                sbOutput.Append((i + 1) +
                    "\t" + selTest.itemType[i] + "\t" + selTest.itemStandards[i] + "\t\t" + selTest.itemAnswers[i] + "\t│" + selTest.itemPvalues[i].ToString("0.00") + "\t" + selTest.itemPBS[i].ToString("0.00") + "\t" + selTest.testAifD[i].ToString("0.00") + Environment.NewLine);
            }
            return sbOutput.ToString();
        }

        public static void HTMLOut(Assessment selTest)
        {
            var strHTML = new StringBuilder();

            double recTL = 1.0 / (double)selTest.testLength;
            int _tableWidth = 660;
            int _gDivHeight = 250;
            int _gDivWidth = 515;
            int _barWidth = 45;
            int _medianLeft = 11 + (int)(selTest.testMean * recTL * _gDivWidth);
            int _medianHeight = 231;
            int _stdLeft = 11 + (int)((selTest.testMean - selTest.testStdDev) * recTL * _gDivWidth);
            int _stdWidth = (int)(selTest.testStdDev * recTL * 2 * _gDivWidth);
            int _stdTop = 234;
            int p100Top = 0;
            int p75Top = (int)(_gDivHeight * .25);
            int p50Top = (int)(_gDivHeight * .5);
            int p25Top = (int)(_gDivHeight * .75);

            strHTML.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01//EN\"><HTML><HEAD><meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\"><meta name=\"author\" content=\"Chris Stefancik 2015\"><TITLE>" + selTest.fileParser.TestName + "</TITLE>");

            strHTML.Append("<STYLE  type=\"text/css\">");

            strHTML.Append("table { border-collapse: collapse; border-color: #c1c1c1; border-spacing: 0; border-style: solid; border-width: 1px 0 0 1px; vertical-align: middle; width: " + _tableWidth + "px; }");
            strHTML.Append("th { background-color: #edf2f9; border-color: #b0b7bb; border-style: solid; border-width: 0 1px 1px 0; color: #112277; font-family: Arial, Helvetica, Helv; font-size: small; font-style: normal; font-weight: bold; padding: 3px 6px; text-align: center; vertical-align: middle; }");
            strHTML.Append("td { background-color: #FFFFFF; border-color: #c1c1c1; border-style: solid; border-width: 0 1px 1px 0; font-family: Arial, Helvetica, Helv; font-size: small; font-style: normal; font-weight: normal; padding: 3px 6px; text-align: right; vertical-align: middle; }");
            strHTML.Append(".graph { height: " + _gDivHeight + "px; position: relative; width: " + _gDivWidth + "px; }");
            strHTML.Append(".bar { background-color: #edf2f9; border: 1px solid #c1c1c1; display: inline-block; margin: 1px; position: relative; vertical-align: baseline; width: " + _barWidth + "px; }");
            strHTML.Append(".median { background-color: #FBE2E0; border: 1px solid #9F9F9F; display: inline-block; height: " + _medianHeight + "px; left: " + _medianLeft + "px; margin: 0px; position: absolute; top: 1px; vertical-align: baseline; width: 0px; }");
            strHTML.Append(".std { border: 1px solid #9F9F9F; display: inline-block; left: " + _stdLeft + "px; margin: 0px; position: absolute; top: " + _stdTop + "px; vertical-align: baseline; width: " + _stdWidth + "px; }");
            strHTML.Append(".xlabel { border: 1px solid #FFFFFF; display: inline-block; font-family: Arial, Helvetica, Helv; font-size: x-small; font-style: normal; font-weight: normal; margin: 1px; position: relative; text-align: center; vertical-align: baseline; width: " + _barWidth + "px; }");
            strHTML.Append(".ylabel { display: inline-block; font-family: Arial, Helvetica, Helv; font-size: x-small; font-style: normal; font-weight: normal; left: 0px; position: absolute; text-align: left; }");
            strHTML.Append(".center { text-align: center; }");
            strHTML.Append(".left { text-align: left; }");
            strHTML.Append(".warning { background-color: #FBE2E0; }");

            strHTML.Append("</STYLE>");

            strHTML.Append("</HEAD><BODY><TABLE><tr><th colspan=\"6\"><p>" + selTest.fileParser.TestName + "</p></th><th colspan=\"4\"><p>Test Analysis Report</p></th></tr><tr><td class=\"center\" colspan=\"3\">Date: " + DateTime.Today + "</td><td class=\"center\" colspan=\"3\">User: " + Environment.UserName  + "</td><td class=\"center\" colspan=\"4\">CDS 2015</td></tr>");

            strHTML.Append("</table><p></p><table>");
            strHTML.Append("<tr><th colspan=\"2\">Statistics</th><th colspan=\"8\">Percent Score Distribution</th></tr>");

            if (selTest.testLength < 3)
                strHTML.Append("<tr><td>Items</td><td class=\"warning\">" + selTest.testLength + "</td>");
            else
                strHTML.Append("<tr><td>Items</td><td>" + selTest.testLength + "</td>");
            strHTML.Append("<td ROWSPAN=\"12\" colspan=\"8\"><div><div class=\"graph\">");

            double multiplier = 1 / selTest.studentHistogram.Max() * 230;
            for (int i = 0; i < 10; i++)
            {
                strHTML.Append("<div style=\"height: " + (int)(selTest.studentHistogram[i] * multiplier) + "px\" class=\"bar\"></div>");
            }
            strHTML.Append("<div class=\"std\"></div>");
            strHTML.Append("<div class=\"median\"></div>");
            strHTML.Append("<div class=\"ylabel\" style=\"top: " + p100Top + "px\">" + (selTest.studentHistogram.Max() / selTest.studentLength * 100).ToString("0.0") + "%</div>");
            strHTML.Append("<div class=\"ylabel\" style=\"top: " + p75Top + "px\">" + (selTest.studentHistogram.Max() / selTest.studentLength * 75).ToString("0.0") + "%</div>");
            strHTML.Append("<div class=\"ylabel\" style=\"top: " + p50Top + "px\">" + (selTest.studentHistogram.Max() / selTest.studentLength * 50).ToString("0.0") + "%</div>");
            strHTML.Append("<div class=\"ylabel\" style=\"top: " + p25Top + "px\">" + (selTest.studentHistogram.Max() / selTest.studentLength * 25).ToString("0.0") + "%</div>");

            strHTML.Append("<div class=\"xlabel\">0-10</div>" +
                        "<div class=\"xlabel\">11-20</div>" +
                        "<div class=\"xlabel\">21-30</div>" +
                        "<div class=\"xlabel\">31-40</div>" +
                        "<div class=\"xlabel\">41-50</div>" +
                        "<div class=\"xlabel\">51-60</div>" +
                        "<div class=\"xlabel\">61-70</div>" +
                        "<div class=\"xlabel\">71-80</div>" +
                        "<div class=\"xlabel\">81-90</div>" +
                        "<div class=\"xlabel\">91-100</div>");
            strHTML.Append("</div></div></tr>");

            if (selTest.studentLength < 25)
                strHTML.Append("<tr><td>Students</td><td class=\"warning\">" + selTest.studentLength + "</td></tr>");
            else
                strHTML.Append("<tr><td>Students</td><td>" + selTest.studentLength + "</td></tr>");

            double divFactor = 1.0 / selTest.testLength;

            strHTML.Append("<tr><td>Min</td><td>" + (selTest.studentRawScores.Min() * divFactor).ToString("P1") + "</td></tr>");
            strHTML.Append("<tr><td>Q1</td><td>" + (selTest.testStatistics[1] * divFactor).ToString("P1") + "</td></tr>");
            strHTML.Append("<tr><td>Mean</td><td>" + (selTest.testMean * divFactor).ToString("P1") + "</td></tr>");
            strHTML.Append("<tr><td>Median</td><td>" + (selTest.testStatistics[3] * divFactor).ToString("P1") + "</td></tr>");
            strHTML.Append("<tr><td>Q3</td><td>" + (selTest.testStatistics[4] * divFactor).ToString("P1") + "</td></tr>");
            strHTML.Append("<tr><td>Max</td><td>" + (selTest.studentRawScores.Max() * divFactor).ToString("P1") + "</td></tr>");
            strHTML.Append("<tr><td>Std Dev</td><td>" + (selTest.testStdDev * divFactor * 100).ToString("0.00") + "</td></tr>");
            strHTML.Append("<tr><td>SEM</td><td>" + (selTest.testSEM * divFactor * 100).ToString("0.00") + "</td></tr>");
            if (selTest.testSkew > 0)
                strHTML.Append("<tr><td>Skew</td><td>&#8592; " + selTest.testSkew.ToString("0.00") + "</td></tr>");
            else
                strHTML.Append("<tr><td>Skew</td><td>&#8594; " + selTest.testSkew.ToString("0.00") + "</td></tr>");
            if (selTest.testAlpha < .7 | selTest.testAlpha > 1)
                strHTML.Append("<tr><td>Alpha</td><td class=\"warning\">" + selTest.testAlpha.ToString("0.00") + "</td></tr>");
            else
                strHTML.Append("<tr><td>Alpha</td><td>" + selTest.testAlpha.ToString("0.00") + "</td></tr>");

            //Notes section
            
            if (GlobalSettings.HasCR ==true && GlobalSettings.ReplaceCR==true)
            {
                strHTML.Append("<tr><td class=\"left\" colspan=\"11\">");
                strHTML.Append("Note: Constructed response questions were converted a score of 0 or 1.");
            }
            strHTML.Append("</table><p></p>");

            //Test Design Section

            strHTML.Append("<table>");

            strHTML.Append("<tr><th colspan=\"4\">Item Difficulty</th>" +
                    "<th>% of Items</th>" +
                    "<th colspan=\"4\">Item Discrimination</th>" +
                    "<th colspan=\"1\">% of Items</th></tr>");

            strHTML.Append("<tr><td colspan=\"4\" class=\"left\">Easy (Higher than 70%)</td>" +
                "<td>" + (selTest.ItemDifficulty[2] * 100).ToString("0.0") + "</td>" +
                "<td colspan=\"4\" class=\"left\">Good (Higher than 0.3)</td>" +
                "<td colspan=\"1\">" + (selTest.ItemDiscrimination[2] * 100).ToString("0.0") + "</td></tr>");

            strHTML.Append("<tr><td colspan=\"4\" class=\"left\">Moderate (40% to 70%)</td>" +
                "<td>" + (selTest.ItemDifficulty[1] * 100).ToString("0.0") + "</td>" +
                "<td colspan=\"4\" class=\"left\">Acceptable (0.2 to 0.3)</td>" +
                "<td colspan=\"1\">" + (selTest.ItemDiscrimination[1] * 100).ToString("0.0") + "</td></tr>");

            strHTML.Append("<tr><td colspan=\"4\" class=\"left\">Hard (Less than 40%)</td>" +
                "<td>" + (selTest.ItemDifficulty[0] * 100).ToString("0.0") + "</td>" +
                "<td colspan=\"4\" class=\"left\">Needs Review (Less than 0.2)</td>" +
                "<td colspan=\"1\">" + (selTest.ItemDiscrimination[0] * 100).ToString("0.0") + "</td></tr>");

            strHTML.Append("</table><p></p>");

            //Item Review Section

            //Multiple Choice
            if (GlobalSettings.HasMC == true)
            {
                strHTML.Append("<table>");
                strHTML.Append("<tr><th>Item</th><th>P-Value</th><th>PBS</th><th>Alpha IfD</th><th>Answer</th><th>% C1</th><th>% C2</th><th>% C3</th><th>% C4</th><th>% Om</th></tr>");

                for (int i = 0; i < selTest.testLength; i++)
                {
                    if (selTest.itemType[i] == "MC")
                    {
                        strHTML.Append("<tr>");
                        strHTML.Append("<td>" + (i + 1) + " " + selTest.itemType[i] + "</td>");
                        if (selTest.itemPvalues[i] < .2 | selTest.itemPvalues[i] > .9)
                            strHTML.Append("<td class=\"warning\">" + selTest.itemPvalues[i].ToString("0.00") + "</td>");
                        else
                            strHTML.Append("<td>" + selTest.itemPvalues[i].ToString("0.00") + "</td>");
                        if (selTest.itemPBS[i] < .2)
                            strHTML.Append("<td class=\"warning\">" + selTest.itemPBS[i].ToString("0.00") + "</td>");
                        else
                            strHTML.Append("<td>" + selTest.itemPBS[i].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + selTest.testAifD[i].ToString("0.00") + "</td>");
                        strHTML.Append("<td class=\"center\">" + selTest.itemAnswers[i] + "</td>");
                        strHTML.Append("<td>" + selTest.MCFreq[i, 0].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + selTest.MCFreq[i, 1].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + selTest.MCFreq[i, 2].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + selTest.MCFreq[i, 3].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + selTest.MCFreq[i, 4].ToString("0.00") + "</td>");
                        strHTML.Append("</tr>");
                    }
                }
                strHTML.Append("</table>");
                strHTML.Append("<p></p>");
            }

            //Multiple Choice
            if (GlobalSettings.HasMS == true)
            {
                strHTML.Append("<table>");
                strHTML.Append("<tr><th>Item</th><th>P-Value</th><th>PBS</th><th>Alpha IfD</th><th>Answer</th><th>% C1</th><th>% C2</th><th>% C3</th><th>% C4</th><th>% Om</th></tr>");

                for (int i = 0; i < selTest.testLength; i++)
                {
                    if (selTest.itemType[i] == "MS")
                    {
                        strHTML.Append("<tr>");
                        strHTML.Append("<td>" + (i + 1) + " " + selTest.itemType[i] + "</td>");
                        if (selTest.itemPvalues[i] < .2 | selTest.itemPvalues[i] > .9)
                            strHTML.Append("<td class=\"warning\">" + selTest.itemPvalues[i].ToString("0.00") + "</td>");
                        else
                            strHTML.Append("<td>" + selTest.itemPvalues[i].ToString("0.00") + "</td>");
                        if (selTest.itemPBS[i] < .2)
                            strHTML.Append("<td class=\"warning\">" + selTest.itemPBS[i].ToString("0.00") + "</td>");
                        else
                            strHTML.Append("<td>" + selTest.itemPBS[i].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + selTest.testAifD[i].ToString("0.00") + "</td>");
                        strHTML.Append("<td class=\"center\">" + selTest.itemAnswers[i] + "</td>");
                        strHTML.Append("<td>" + selTest.MCFreq[i, 0].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + selTest.MCFreq[i, 1].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + selTest.MCFreq[i, 2].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + selTest.MCFreq[i, 3].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + selTest.MCFreq[i, 4].ToString("0.00") + "</td>");
                        strHTML.Append("</tr>");
                    }
                }
                strHTML.Append("</table>");
                strHTML.Append("<p></p>");
            }

            //Gridded Response
            if (GlobalSettings.HasGR == true)
            {
                strHTML.Append("<table>");
                strHTML.Append("<tr><th>Item</th><th>P-Value</th><th>PBS</th><th>Alpha IfD</th><th>Answer</th><th>Num. Unique Answers</th><th>% Om</th></tr>");

                for (int i = 0; i < selTest.testLength; i++)
                {
                    if (selTest.itemType[i] == "GR")
                    {
                        strHTML.Append("<tr>");
                        strHTML.Append("<td>" + (i + 1) + " " + selTest.itemType[i] + "</td>");
                        if (selTest.itemPvalues[i] < .2 | selTest.itemPvalues[i] > .9)
                            strHTML.Append("<td class=\"warning\">" + selTest.itemPvalues[i].ToString("0.00") + "</td>");
                        else
                            strHTML.Append("<td>" + selTest.itemPvalues[i].ToString("0.00") + "</td>");

                        if (selTest.itemPBS[i] < .2)
                            strHTML.Append("<td class=\"warning\">" + selTest.itemPBS[i].ToString("0.00") + "</td>");
                        else
                            strHTML.Append("<td>" + selTest.itemPBS[i].ToString("0.00") + "</td>");

                        strHTML.Append("<td>" + selTest.testAifD[i].ToString("0.00") + "</td>");
                        strHTML.Append("<td class=\"center\">" + selTest.itemAnswers[i] + "</td>");

                        strHTML.Append("<td>" + selTest.uniqueResponse[i] + "</td>");

                        strHTML.Append("<td>" + selTest.MCFreq[i, 4].ToString("0.00") + "</td>");
                        strHTML.Append("</tr>");
                    }
                }
                strHTML.Append("</table>");
                strHTML.Append("<p></p>");
            }

            if (GlobalSettings.HasCR == true)
            {
                strHTML.Append("<table>");
                strHTML.Append("<tr><th>Item</th><th>P-Value</th><th>PBS</th><th>Alpha IfD</th><th>Range</th><th>Mean Score</th><th>Percent Below Mean</th><th>% Om</th></tr>");

                for (int i = 0; i < selTest.testLength; i++)
                {
                    if (selTest.itemType[i] == "CR")
                    {
                        strHTML.Append("<tr>");
                        strHTML.Append("<td>" + (i + 1) + " " + selTest.itemType[i] + "</td>");
                        if (selTest.itemPvalues[i] < .2 | selTest.itemPvalues[i] > .9)
                            strHTML.Append("<td class=\"warning\">" + selTest.itemPvalues[i].ToString("0.00") + "</td>");
                        else
                            strHTML.Append("<td>" + selTest.itemPvalues[i].ToString("0.00") + "</td>");

                        if (selTest.itemPBS[i] < .2)
                            strHTML.Append("<td class=\"warning\">" + selTest.itemPBS[i].ToString("0.00") + "</td>");
                        else
                            strHTML.Append("<td>" + selTest.itemPBS[i].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + selTest.testAifD[i].ToString("0.00") + "</td>");
                        strHTML.Append("<td class=\"center\">" + selTest.fileParser.AnswerKey[i] + "</td>");

                        strHTML.Append("<td>" + selTest.fileParser.CRAverages[i].ToString("0.00") + "</td>");
                        strHTML.Append("<td>" + (100 - selTest.itemPvalues[i] * 100).ToString("0.00") + "*</td>");

                        strHTML.Append("<td>" + selTest.MCFreq[i, 4].ToString("0.00") + "</td>");
                        strHTML.Append("</tr>");
                    }
                }
                strHTML.Append("</table>");
                strHTML.Append("<p></p>");
            }
            //References Section

            if (GlobalSettings.GenerateReferences)
            {
                strHTML.Append("<table><tr><th colspan=\"10\">Citations</th></tr><tr><td colspan=\"10\" class=\"left\">");
                strHTML.Append("<p>Afifi, A. A., & Elashoff, R. M. (1966). Missing observations in multivariate statistics I. Review of the literature. <em>Journal of the American Statistical Association, </em> 61(315), 595-604.<br></br>");
                strHTML.Append("Brown, J. D. (2001). Point-biserial correlation coefficients. <em>JALT Testing + Evaluation SIG Newsletter, </em> 5(3), 12-15.<br></br>");
                strHTML.Append("Brown, S. (2011). Measures of shape: Skewness and Kurtosis. Retrieved on December, 31, 2014.<br></br>");
                strHTML.Append("Ebel, R. L. (1950). Construction and validation of educational tests. <em>Review of Educational Research,</em> 87-97.<br></br>");
                strHTML.Append("Ebel, R. L. (1965). Confidence Weighting and Test Reliability. <em>Journal of Educational Measurement,</em> 2(1), 49-57.<br></br>");
                strHTML.Append("Kelley, T., Ebel, R., & Linacre, J. M. (2002). Item discrimination indices. <em>Rasch Measurement Transactions,</em> 16(3), 883-884.<br></br>");
                strHTML.Append("Krishnan, V. (2013). The Early Child Development Instrument (EDI): An item analysis using Classical Test Theory (CTT) on Alberta\'s data. <em>Early Child Mapping (ECMap) Project Alberta, Community-University Partnership (CUP), Faculty of Extension, University of Alberta, Edmonton, Alberta.</em><br></br>");
                strHTML.Append("Matlock-Hetzel, S. (1997). Basic Concepts in Item and Test Analysis.<br></br>Pearson, K. (1895). Contributions to the mathematical theory of evolution. II. Skew variation in homogeneous material. <em>Philosophical Transactions of the Royal Society of London. A, </em>343-414.<br></br>");
                strHTML.Append("Richardson, M. W., & Stalnaker, J. M. (1933). A note on the use of bi-serial r in test research. <em>The Journal of General Psychology,</em> 8(2), 463-465.<br></br>");
                strHTML.Append("Yu, C. H., & Ds, P. (2012). A Simple Guide to the Item Response Theory (IRT) and Rasch Modeling.<br></br>Zeng, J., + Wyse, A. (2009). Introduction to Classical Test Theory. <em>Michigan, Washington, US.</em>");
                strHTML.Append("</p></td></tr></table>");
            }
            strHTML.Append("</table><p></p>");

            strHTML.Append("</HTML>");

            if (selTest.xlsxLoader == null)
                File.WriteAllText(selTest.txtLoader.WorkingDirectory + '\\' + selTest.fileParser.TestName.Trim() + ".htm", strHTML.ToString());
            else
                File.WriteAllText(selTest.xlsxLoader.WorkingDirectory + '\\' + selTest.fileParser.TestName.Trim() + ".htm", strHTML.ToString());
        }

        public static void GenerateZScores(Assessment selTest)
        {
            string[] strBuffer = new string[selTest.fileParser.StudentZScore.GetLength(0)];

            for (int i = 0; i < selTest.fileParser.StudentZScore.GetLength(0); i++)
            {
                strBuffer[i] = selTest.fileParser.StudentAlpha[i] + "," + selTest.fileParser.StudentZScore[i];
            }

            if (selTest.xlsxLoader == null)
                System.IO.File.WriteAllLines(selTest.txtLoader.WorkingDirectory + '\\' + selTest.fileParser.TestName + "-ZScore.csv", strBuffer);
            else
                System.IO.File.WriteAllLines(selTest.xlsxLoader.WorkingDirectory + '\\' + selTest.fileParser.TestName + "-ZScore.csv", strBuffer);
        }
    }
}