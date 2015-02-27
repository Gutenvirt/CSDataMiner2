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
        public string[,] BinaryData {get; set;}
        public string[] Standards { get; set; }
        public string InportStatus { get; set; }
        public string TestName {get; set;}
        public int NumStudentsDropped { get; set; }

        public string NullValue = "NaN"; //unique enough not to get confused with other datapoints in the file <Not a Number>
        
        public DataFileLocations dflDataFileLoc = new DataFileLocations(5, 6, 5); //5,6,4

        public DataParser (DataTable InpData)
        {
            TestName = InpData.Rows[0].ItemArray[6].ToString (); 



            //Extract the data and store in a 2D array; although the initial import of data is a database, operations are much to slow.  We lose some overhead
            //initially, but in the long run array ops are quick and efficient and overall speed up the processing.

            BinaryData = new string[InpData.Columns.Count - dflDataFileLoc.LastDataCol, InpData.Rows.Count - dflDataFileLoc.FirstDataRow];
            
            for (int i = dflDataFileLoc.FirstDataCol; i < InpData.Columns.Count - dflDataFileLoc.LastDataCol; i++)
            {
                for (int j = dflDataFileLoc.FirstDataRow ; j < InpData.Rows.Count; j++)
                {
                    BinaryData[i - dflDataFileLoc.FirstDataCol, j - dflDataFileLoc.FirstDataRow] = InpData.Rows[j].ItemArray[i].ToString();
                    if (BinaryData[i - dflDataFileLoc.FirstDataCol, j - dflDataFileLoc.FirstDataRow].Trim().Length < 1)
                    {
                        BinaryData[i - dflDataFileLoc.FirstDataCol, j - dflDataFileLoc.FirstDataRow] = NullValue;
                    }
                }
            }
        }
        }
    }
