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
using System.Windows.Forms;

namespace CSDataMiner2
{
	public partial class frmMain : Form
	{
		public frmMain ()
		{
			InitializeComponent ();
		}

		public void SetupChoices ()
		{
			if (rbZeroReplace.Checked == true) {
				GlobalSettings.DeleteOption = MethodOfDelete.ZeroReplace;
			} else {
				if (rbListWise.Checked == true) {
					GlobalSettings.DeleteOption = MethodOfDelete.Listwise;
				} else {
					if (rbPairWise.Checked == true) {
						GlobalSettings.DeleteOption = MethodOfDelete.Pairwise;
					}
				}
			}
			if (bcConvertCR.Checked == true)
				GlobalSettings.ReplaceCR = true;
			else {
				GlobalSettings.ReplaceCR = false;
			}
		}

		private void cmdSingle_Click (object sender, EventArgs e)
		{
			oFD.ShowDialog ();
			if (oFD.FileName != null) {
				SetupChoices (); 
				var test = new Assessment (oFD.FileName);
				textBox1.Text = test.Output;
			}
		}

		private void cmdBatch_Click (object sender, EventArgs e)
		{
			textBox1.Text = "Doh!";
		}

		private void cmdExit_Click (object sender, EventArgs e)
		{
			Application.Exit ();
		}

		void RbListWiseCheckedChanged (object sender, EventArgs e)
		{
			rbListWise.Checked = true;
			rbPairWise.Checked = false;
			rbZeroReplace.Checked = false;
		}

		void RbZeroReplaceCheckedChanged (object sender, EventArgs e)
		{
			rbListWise.Checked = false;
			rbPairWise.Checked = false;
			rbZeroReplace.Checked = true;
		}

		void RbPairWiseCheckedChanged (object sender, EventArgs e)
		{
			rbListWise.Checked = false;
			rbPairWise.Checked = true;
			rbZeroReplace.Checked = false;
		}

		void BcConvertCRCheckedChanged (object sender, EventArgs e)
		{
	
		}

		void CbOutCSVCheckedChanged (object sender, EventArgs e)
		{
	
		}

		void CbOutHTMLCheckedChanged (object sender, EventArgs e)
		{
	
		}

		void CbReferencesCheckedChanged (object sender, EventArgs e)
		{
	
		}

		void CbZScoresCheckedChanged (object sender, EventArgs e)
		{
	
		}

		void CbEduphoriaCheckedChanged (object sender, EventArgs e)
		{
	
		}

		void CbPerformMatterCheckedChanged (object sender, EventArgs e)
		{
	
		}
	}
}
