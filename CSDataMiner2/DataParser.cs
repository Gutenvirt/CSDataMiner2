/*
VBDataMiner - Extract and analyze data from MS Excel(c) files.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CSDataMiner2
{
    class DataParser
    {
        //public string[,] RawData { get; set; }
        public string[,] BinaryData {get; set;}
        public string[] Standards { get; set; }
        public string Status { get; set; }
        public string TestName {get; set;}
        public string NullValue = "NaN";
        public int NumStudentsDropped { get; set; }

        public DataFileSettings EdViewLocations = new DataFileSettings(new Corner(5, 6), new Corner(5, 5));

        public DataParser (DataTable RawData)
        {
        try
            {
                TestName = RawData.Rows[0].ItemArray[6].ToString ();

                for (int i = 0; i < RawData.Columns.Count; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        switch (RawData.Rows[j].ItemArray[i].ToString())
                        {
                            case "Ethnicity":
                                EdViewLocations.UpperLeft = new Corner(i, j);
                                break;

                            case "Raw Score":
                                EdViewLocations.UpperRight = new Corner(i, j);
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Status = e.ToString();
            }


            //**********************************
            //parser to remove extra rows;


            BinaryData = new string[RawData.Columns.Count, RawData.Rows.Count];
            for (int i = 0; i < RawData.Columns.Count; i++)
            {
                for (int j = 0; j < RawData.Columns.Count; j++)
                {
                    try
                    {
                        if (DBNull.Value.Equals(RawData.Rows[i].ItemArray[j]) || RawData.Rows[i].ItemArray[j].ToString() == "")
                        {
                            BinaryData[i, j] = NullValue;
                        }
                        else
                        {
                            BinaryData[i, j] = RawData.Rows[i].ItemArray[j].ToString();
                        }
                    }
                    catch
                    {
                        BinaryData[i, j] = NullValue;
                    }
                }

            }
        }
        }
    }
