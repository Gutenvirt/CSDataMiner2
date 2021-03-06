//
//  FileLoader.cs
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
using System.Data.OleDb;
using System.Data;
using System.IO;

namespace CSDataMiner2
{
	sealed public class LoadXLSX
	{
		public DataTable RawData = new DataTable (Guid.NewGuid ().ToString ());

		public string ErrorString { get; set; }
        public string FileName { get; set; }
        public string WorkingDirectory { get; set; }

        public LoadXLSX(string dbFilename)
		{
			try {
				//IMEX=1 poses a problem, but there are little options available.  It treats all data pulled as a string, so extra parsing is needed later on.
				//Using the Office InterOps protocols are inherently dangerous and full of ambiguity when dealing with data, OLEDB is the only alternative.
				//once the source data files are available in CSV, most of this will be legacy support for VERY specific cases.

				var _oleAdapter = new OleDbDataAdapter ("SELECT * FROM [Sheet1$]", "provider=Microsoft.ACE.OLEDB.12.0; Data Source='" + dbFilename + "'; Extended Properties='Excel 12.0;IMEX=1;HDR=NO'");
				_oleAdapter.Fill (RawData);
			} catch (IOException e) {
				ErrorString = e.ToString ();
			}
            FileName = dbFilename;
            WorkingDirectory = dbFilename.Substring(0, dbFilename.LastIndexOf('\\'));
        }
    }

    public sealed class LoadTxt
    {
        public string[] strRawData;

        public string FileName { get; set; }
        public string WorkingDirectory { get; set; }

        public LoadTxt(string fFilename)
        {
            strRawData = File.ReadAllLines(fFilename);
            FileName = fFilename;
            WorkingDirectory = fFilename.Substring(0, fFilename.LastIndexOf('\\'));
        }
    }
}