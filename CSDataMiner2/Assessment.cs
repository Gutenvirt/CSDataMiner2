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
using System.Text;


namespace CSDataMiner2
{
    sealed public class Assessment
    {
        public string Output { get; set; }

        public FileParser fileParser;
        public LoadXLSX xlsxLoader;
        public LoadTxt txtLoader;

        public string[] itemType;
        public string[] itemStandards;
        public string[] itemAnswers;

        public double[] studentRawScores;
        public double[] studentPercentScores;
        public double[] studentHistogram;

        public double[] ItemDiscrimination;
        public double[] ItemDifficulty;

        public int testLength;
        public int studentLength;
        public int[] uniqueResponse;

        public double[] itemPvalues;
        public double testStdDev;
        public double testAlpha;
        public double testMean;
        public double testSkew;

        public double testSEM;

        public double[] itemPBS;
        public double[] testStatistics;
        public double[,] MCFreq;

        public double[] testAifD;

        public Assessment(string filename)
        {
            if (filename.EndsWith(".dtxt"))
            {
                txtLoader =new LoadTxt (filename);
                fileParser = new FileParser(txtLoader.strRawData);
            }

            if (filename.EndsWith (".xlsx"))
            {
                xlsxLoader = new LoadXLSX(filename);
                fileParser = new FileParser(xlsxLoader.RawData); 
            }

            //ListWise -> Removes the student from database if there is ANY omission found.
            //Pairwise -> (DEFAULT) Replaces any omission with NaN (not a number) but still allows present data to be analyzed.
            //ZeroReplace -> Same as above but NaN is a replaced with a zero.

            itemType = fileParser.ItemType;
            itemStandards = fileParser.Standards;
            itemAnswers = fileParser.AnswerKey;

            studentRawScores = BinDataOps.GetRawScores(fileParser.BinaryData);
            studentPercentScores = BinDataOps.GetPercentScores(fileParser.BinaryData);

            testLength = itemAnswers.GetLength(0);
            studentLength = studentRawScores.GetLength(0);

            if (GlobalSettings.HasGR == true )
                uniqueResponse = ChoiceDataOps.GetUniqueResponse(fileParser.ChoiceData, fileParser.ItemType); 

            itemPvalues = BinDataOps.GetPValues(fileParser.BinaryData);
            testStdDev = BinDataOps.GetStandardDeviation(fileParser.BinaryData);
            testAlpha = BinDataOps.GetAlpha(itemPvalues, testStdDev);

            testSEM = BinDataOps.GetStandardErrorOfMeasure(testStdDev, testAlpha);

            itemPBS = BinDataOps.GetPointBiSerial(testStdDev, studentRawScores, fileParser.BinaryData);
            testStatistics = BinDataOps.GetDescriptiveStats(studentRawScores);
            testMean = testStatistics[2];
            studentHistogram = HistogramGen.GetValues(hType.Frequency, new double[10] { .1, .2, .3, .4, .5, .6, .7, .8, .9, 1 }, studentPercentScores);

            testSkew = BinDataOps.GetTestSkew(testMean, testStatistics[3], testStdDev);

            MCFreq = ChoiceDataOps.GetFrequencies(itemType, fileParser.ChoiceData);

            testAifD = BinDataOps.GetAlphaIfDropped(fileParser.BinaryData);

            ItemDifficulty = HistogramGen.GetValues(hType.Percent, new double[3] { .4, .7, 1 }, itemPvalues);
            ItemDiscrimination = HistogramGen.GetValues(hType.Percent, new double[3] { .2, .3, 1 }, itemPBS);
        }
    }
}
