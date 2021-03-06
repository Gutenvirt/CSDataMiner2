//
//  FileParser.cs
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
using System.Diagnostics;

namespace CSDataMiner2
{
    sealed public class FileParser
    {
        public string NullValue = "NaN";

        public byte[,] BinaryData { get; set; }

        public string[,] ChoiceData { get; set; }

        public string[] Standards { get; set; }

        public string[] AnswerKey { get; set; }

        public double[] CRAverages { get; set; }

        public string[] ItemType { get; set; }

        public string TestName { get; set; }

        public int NumberDroppedStudents { get; set; }

        public string StatusReport { get; set; }

        public string[] StudentAlpha { get; set; }
        public double[] StudentZScore { get; set; }

        public DataFileSettings dfLoc = new DataFileSettings(5, 6, 5);
        //General to target datasource, FirstDataRow x FirstDataCol x LastDataCol offset

        public FileParser(string[] InpData)
        {
            int NumberOfFields = 9;

            char delimit = '\t';
            TestName = InpData[0].Split(delimit)[1];

            if (TestName.IndexOf('/') > -1) { TestName = TestName.Replace("/", string.Empty); }
            if (TestName.IndexOf(':') > -1) { TestName = TestName.Replace(":", string.Empty); }
            if (TestName.IndexOf('?') > -1) { TestName = TestName.Replace("?", string.Empty); }
            if (TestName.IndexOf('!') > -1) { TestName = TestName.Replace("!", string.Empty); }
            if (TestName.IndexOf(',') > -1) { TestName = TestName.Replace(",", string.Empty); }
            if (TestName.IndexOf(';') > -1) { TestName = TestName.Replace(";", string.Empty); }
            TestName = TestName.Replace("*", "(V1 Test)");

            //1 subject
            //2 school year
            int nVars = int.Parse(InpData[3].Split(delimit)[1]);
            string[] tmpItemType = InpData[4].Split(delimit);
            //5 max points possible
            string[] tmpStndards = InpData[6].Split(delimit);
            string[] tmpAnswers = InpData[7].Split(delimit);
            //8 headings

            int nObs = InpData.GetLength(0) - NumberOfFields;

            AnswerKey = new string[nVars];
            Array.Copy(tmpAnswers, 1, AnswerKey, 0, nVars);

            ItemType = new string[nVars];
            Array.Copy(tmpItemType, 1, ItemType, 0, nVars);

            Standards = new string[nVars];
            Array.Copy(tmpStndards, 1, Standards, 0, nVars);

            ChoiceData = new string[nVars, nObs];
            BinaryData = new byte[nVars, nObs];

            StudentZScore = new double[nObs];
            StudentAlpha = new string[nObs];


            for (int i = NumberOfFields; i < InpData.GetLength(0); i++)
            {
                string[] strBuffer = InpData[i].Split(new char[] { delimit }, StringSplitOptions.None);
                if (GlobalSettings.GenerateZScores == true)
                {
                    StudentAlpha[i - NumberOfFields] = strBuffer[0];
                    double.TryParse(strBuffer[102], out StudentZScore[i-NumberOfFields]);
                }
                for (int j = 1; j < nVars + 1; j++)
                {
                    ChoiceData[j - 1, i - NumberOfFields] = strBuffer[j];
                    if (strBuffer[j] == string.Empty | strBuffer[j] == null)
                    {
                        ChoiceData[j - 1, i - NumberOfFields] = NullValue;
                        BinaryData[j - 1, i - NumberOfFields] = GlobalSettings.DeleteOption == MethodOfDelete.ZeroReplace ? (byte)0 : (byte)255;
                    }
                    else
                    {
                        BinaryData[j - 1, i - NumberOfFields] = strBuffer[j] == AnswerKey[j - 1] ? (byte)1 : (byte)0;
                    }
                }
            }

            if (GlobalSettings.GenerateZScores == true)
                StudentZScore = BinDataOps.GetZScore (StudentZScore);

            GlobalSettings.HasMC = false;
            GlobalSettings.HasMS = false;
            GlobalSettings.HasGR = false;
            GlobalSettings.HasCR = false;

            for (int i = 0; i < nVars; i++)
            {
                if (ItemType[i] == "MC")
                    GlobalSettings.HasMC = true;
                if (ItemType[i] == "MS")
                    GlobalSettings.HasMS = true;
                if (ItemType[i] == "GR")
                    GlobalSettings.HasGR = true;
                if (ItemType[i] == "CR")
                    GlobalSettings.HasCR = true;
            }
            if (GlobalSettings.ReplaceCR)
                CRAverages = ConvertConstructedResponse();
        }

        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        public FileParser(DataTable InpData)
        {
            switch (GlobalSettings.DeleteOption)
            {
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

            TestName = InpData.Rows[0].ItemArray[6].ToString().Replace(",", "").Replace(":", "");

            //Depending on the options, listwise deletions must be completed first since it changes the size of the datatable and
            //ultimately the bounds of the CHOCIE and BINARY 2D array created later.
            char[] INVALID_CHARACTERS = "!@#$%NY".ToCharArray();
            NumberDroppedStudents = 0;

            for (int i = InpData.Rows.Count - 1; i >= dfLoc.FirstDataRow; i--)
            {
                for (int j = InpData.Columns.Count - dfLoc.LastDataCol - 1; j > dfLoc.FirstDataCol; j--)
                {
                    if ((GlobalSettings.DeleteOption == MethodOfDelete.Listwise && InpData.Rows[i].ItemArray[j] == DBNull.Value) | (InpData.Rows[i].ItemArray[j].ToString().IndexOfAny(INVALID_CHARACTERS) > -1))
                    {
                        InpData.Rows[i].Delete();
                        NumberDroppedStudents += 1;
                        break; //no need to continue iterating through a deleted row, break this loop and move to next one.
                    }
                }
            }
            InpData.AcceptChanges();

            if (NumberDroppedStudents > 0)
                StatusReport += "There were " + NumberDroppedStudents + " student records dropped because of improper record format or selected missing data option.";

            //Extract the data and store in a 2D array; although the initial import of data is to a database, operations are much too slow to be useful.  Some overhead
            //is lost initially, but in the long run, array ops are quick and efficient and overall speed up the processing.

            ChoiceData = new string[InpData.Columns.Count - dfLoc.LastDataCol - dfLoc.FirstDataCol, InpData.Rows.Count - dfLoc.FirstDataRow];
            BinaryData = new byte[ChoiceData.GetLength(0), ChoiceData.GetLength(1)];
            Standards = new string[BinaryData.GetLength(0)];
            AnswerKey = new string[Standards.GetLength(0)];

            //Pull the answer choice out
            for (int i = 0; i < InpData.Columns.Count - dfLoc.FirstDataCol - dfLoc.LastDataCol; i++)
            {
                Standards[i] = InpData.Rows[4].ItemArray[i + dfLoc.FirstDataCol].ToString();
                //the standards are hardcoded @ row 4.

                for (int j = 0; j < InpData.Rows.Count - dfLoc.FirstDataRow; j++)
                {

                    ChoiceData[i, j] = InpData.Rows[j + dfLoc.FirstDataRow].ItemArray[i + dfLoc.FirstDataCol].ToString().Trim();

                    if (ChoiceData[i, j].IndexOf("+") == 0)
                    {
                        BinaryData[i, j] = 1;
                        AnswerKey[i] = ChoiceData[i, j].Replace("+", "");
                    }
                    else
                    {
                        if (ChoiceData[i, j].Length < 1)
                        {
                            ChoiceData[i, j] = NullValue;
                            BinaryData[i, j] = (byte)(GlobalSettings.DeleteOption == MethodOfDelete.ZeroReplace ? 0 : 255);
                        }
                        else
                            BinaryData[i, j] = 0;
                    }
                }
                if (string.IsNullOrEmpty(AnswerKey[i]))
                    AnswerKey[i] = "NA";
            }

            char[] VALID_NUMERICS = "0123456789-.".ToCharArray();
            char[] VALID_ALPHAS = "ABCDEFGHIJ".ToCharArray();
            ItemType = new string[AnswerKey.GetLength(0)];

            GlobalSettings.HasMC = false;
            GlobalSettings.HasMS = false;
            GlobalSettings.HasGR = false;
            GlobalSettings.HasCR = false;

            for (int i = 0; i < AnswerKey.GetLength(0); i++)
            {
                string s = AnswerKey[i];
                string iType = "CR";
                if (s.IndexOf(",") == 1)
                    iType = "MS";
                else
                {
                    if (s.Length == 1 && s.IndexOfAny(VALID_ALPHAS) == 0)
                        iType = "MC";
                    else
                    {
                        if (s.IndexOf("-") > -1 | s.IndexOf(".") > -1 | Standards[i].StartsWith("MAFS"))
                            iType = "GR";
                    }
                }
                ItemType[i] = AnswerKey[i] == "NA" ? "MC" : iType;

                if (ItemType[i] == "MC")
                    GlobalSettings.HasMC = true;
                if (ItemType[i] == "MS")
                    GlobalSettings.HasMS = true;
                if (ItemType[i] == "GR")
                    GlobalSettings.HasGR = true;
                if (ItemType[i] == "CR")
                    GlobalSettings.HasCR = true;
            }
            StatusReport += "Item types are guessed from available data, gridded response and constructed response may be incorrectly marked.";
            if (GlobalSettings.ReplaceCR)
                CRAverages = ConvertConstructedResponse();
        }

        public double[] ConvertConstructedResponse()
        {
            var result = new double[ChoiceData.GetLength(0)];
            for (int i = 0; i < ChoiceData.GetLength(0); i++)
            {
                if (ItemType[i] != "CR")
                    continue;
                for (int j = 0; j < ChoiceData.GetLength(1); j++)
                {
                    string s = ChoiceData[i, j].Replace("+", "");
                    if (s == NullValue | s == string.Empty)
                        continue;
                    result[i] += double.Parse(s);
                }
                result[i] = result[i] / ChoiceData.GetLength(1);
                for (int j = 0; j < ChoiceData.GetLength(1); j++)
                {
                    string s = ChoiceData[i, j].Replace("+", "");
                    if (s == NullValue | s == string.Empty)
                        continue;
                    BinaryData[i, j] = (byte)(double.Parse(s) >= result[i] ? 1 : 0);
                }
            }
            return result;
        }
    }
}