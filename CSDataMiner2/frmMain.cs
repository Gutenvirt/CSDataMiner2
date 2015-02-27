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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSDataMiner2
{
    public partial class frmMain : Form
    {

        public frmMain()
        {
            InitializeComponent();
            openFileDialog1.ShowDialog();

            DataConnection DataGetter = new DataConnection (openFileDialog1.FileName);
            DataParser DataFormatter = new DataParser(DataGetter.RawData);

            Console.WriteLine(DataFormatter.BinaryData.GetUpperBound(0));
            Console.WriteLine(DataFormatter.BinaryData.GetUpperBound(1));

            for (int i = 0; i < DataFormatter.BinaryData.GetUpperBound(0); i++)
            {
                for (int j = 0; j < DataFormatter.BinaryData.GetUpperBound(1); j++)
                {
                    textBox1.Text += DataFormatter.BinaryData[i, j].ToString() + "   ";
                    if (i == DataFormatter.BinaryData.GetUpperBound(0))
                    {
                        textBox1.Text += Environment.NewLine;
                    }
                }
            }
        }
    }
}
