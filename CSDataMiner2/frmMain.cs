﻿using System;
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

            for (int i = 0; i< DataGetter.RawData.Columns.Count-1; i++)
            {
                for (int j = 0; j < DataGetter.RawData.Rows.Count-1; j++)
                {
                    textBox1.Text += DataFormatter.BinaryData[j, i] + "    ";
                    if (j==DataGetter.RawData.Columns.Count-1)
                    {
                        textBox1.Text+= Environment.NewLine ;
                    }
                }
            }
        }
    }
}
