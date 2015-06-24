//
//  frmMain.cs
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
using System.IO;
using System.Windows.Forms;

namespace CSDataMiner2
{
    public partial class frmMain : Form
    {

        public Assessment test;

        public frmMain()
        {
            InitializeComponent();
        }

        public void SetupChoices()
        {
            if (rbZeroReplace.Checked == true)
                GlobalSettings.DeleteOption = MethodOfDelete.ZeroReplace;
            else
                if (rbListWise.Checked == true)
                    GlobalSettings.DeleteOption = MethodOfDelete.Listwise;
                else
                    if (rbPairWise.Checked == true)
                        GlobalSettings.DeleteOption = MethodOfDelete.Pairwise;

            if (bcConvertCR.Checked)
                GlobalSettings.ReplaceCR = true;
            else { GlobalSettings.ReplaceCR = false; }

            if (cbOutCSV.Checked)
                GlobalSettings.GenerateCSV = true;
            else { GlobalSettings.GenerateCSV = false; }

            if (cbOutHTML.Checked)
                GlobalSettings.GenerateHTML = true;
            else { GlobalSettings.GenerateHTML = false; }

            if (cbReferences.Checked)
            {
                GlobalSettings.GenerateReferences = true;
                cbOutHTML.Checked = true;
            }
            else { GlobalSettings.GenerateReferences = false; }

            if (cbZScores.Checked)
                GlobalSettings.GenerateZScores = true;
            else { GlobalSettings.GenerateZScores = false; }

            if (oFD.FileName.ToLower().EndsWith("*.xlsx"))
                GlobalSettings.FileFIlter = "*.xlsx";
            else { GlobalSettings.FileFIlter = "*.dtxt"; }
        }

        private void cmdSingle_Click(object sender, EventArgs e)
        {
            oFD.ShowDialog();
            if (oFD.FileName != null && (oFD.FileName.EndsWith(".xlsx") | oFD.FileName.EndsWith(".dtxt")))
            {
                SetupChoices();
                test = new Assessment(oFD.FileName);
                textBox1.Text = FileIO.DiagnosticOutput (test);
                if (GlobalSettings.GenerateCSV) {FileIO.WriteCSV(test);}
                if (GlobalSettings.GenerateHTML) {FileIO.HTMLOut (test);}
                if (GlobalSettings.GenerateZScores) { FileIO.GenerateZScores(test); }
            }
            else { textBox1.Text += "ERROR: Bad file...\n"; }
        }

        private void cmdBatch_Click(object sender, EventArgs e)
        {
            fBD.ShowDialog();
            if (fBD.SelectedPath != null)
            {
                SetupChoices();
                GlobalSettings.DeleteOption = MethodOfDelete.Pairwise;
                rbPairWise.Checked = true;

                int x = 1;
                foreach (string file in Directory.GetFiles(fBD.SelectedPath, GlobalSettings.FileFIlter))
                {
                    if (file.EndsWith(".xlsx") | file.EndsWith(".dtxt"))
                    {
                        test = new Assessment(file);
                        textBox1.Text = "File " + x + " of " + Directory.GetFiles(fBD.SelectedPath).GetLength(0);
                        textBox1.Text = FileIO.DiagnosticOutput(test);
                        if (GlobalSettings.GenerateCSV) { FileIO.WriteCSV(test); }
                        if (GlobalSettings.GenerateHTML) { FileIO.HTMLOut(test); }
                        if (GlobalSettings.GenerateZScores) { FileIO.GenerateZScores(test); }
                        x++;
                        Application.DoEvents();
                    }
                }
            }
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void RbListWiseCheckedChanged(object sender, EventArgs e)
        {
            rbListWise.Checked = true;
            rbPairWise.Checked = false;
            rbZeroReplace.Checked = false;
        }

        void RbZeroReplaceCheckedChanged(object sender, EventArgs e)
        {
            rbListWise.Checked = false;
            rbPairWise.Checked = false;
            rbZeroReplace.Checked = true;
        }

        void RbPairWiseCheckedChanged(object sender, EventArgs e)
        {
            rbListWise.Checked = false;
            rbPairWise.Checked = true;
            rbZeroReplace.Checked = false;
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                textBox2.Text = textBox2.Text.ToLower();
                string[] cmd = textBox2.Text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                textBox2.Text = "";

                for (int i = 0; i < cmd.GetLength(0); i++)
                {
                    string _c = cmd[0];

                    switch (_c)
                    {
                        case "hist":
                            HistogramGen.Parse(cmd[1]);
                            textBox1.Text = HistogramGen.MsgQueue;
                            cmd = new string[] { };
                            break;
                    }
                }
            }
        }

        private void cbZScores_CheckedChanged(object sender, EventArgs e)
        {
            GlobalSettings.GenerateZScores = true;
        }
    }
}
