//
//  DataFileLocations.cs
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

namespace CSDataMiner2
{
	//This is a data container becuase there needs to be a place to house locations within the datafile itself.

	sealed public class DataFileSettings
	{
		public int FirstDataRow { get; set; }

		public int FirstDataCol { get; set; }

		public int LastDataCol { get; set; }

		public DataFileSettings (int firstDataRow, int firstDataCol, int lastDataCol)
		{
			FirstDataCol = firstDataCol;
			LastDataCol = lastDataCol;
			FirstDataRow = firstDataRow;
			//This is misleading, LastDataCol is actually a number that is subtracted from the final column since tests have a different number of question.
			//TotalCol - LastDataCol = real data column.
		}
	}
}