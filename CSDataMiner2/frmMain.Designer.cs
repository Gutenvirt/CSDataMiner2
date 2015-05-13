namespace CSDataMiner2
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.oFD = new System.Windows.Forms.OpenFileDialog ();
			this.textBox1 = new System.Windows.Forms.TextBox ();
			this.fBD = new System.Windows.Forms.FolderBrowserDialog ();
			this.rbListWise = new System.Windows.Forms.RadioButton ();
			this.rbPairWise = new System.Windows.Forms.RadioButton ();
			this.rbZeroReplace = new System.Windows.Forms.RadioButton ();
			this.bcConvertCR = new System.Windows.Forms.CheckBox ();
			this.cbOutCSV = new System.Windows.Forms.CheckBox ();
			this.cbOutHTML = new System.Windows.Forms.CheckBox ();
			this.cbZScores = new System.Windows.Forms.CheckBox ();
			this.cbReferences = new System.Windows.Forms.CheckBox ();
			this.cmdSingle = new System.Windows.Forms.Button ();
			this.cmdBatch = new System.Windows.Forms.Button ();
			this.cbEduphoria = new System.Windows.Forms.CheckBox ();
			this.cbPerformMatter = new System.Windows.Forms.CheckBox ();
			this.cmdExit = new System.Windows.Forms.Button ();
			this.SuspendLayout ();
			// 
			// oFD
			// 
			this.oFD.Filter = "All Files|*.*";
			// 
			// textBox1
			// 
			this.textBox1.Font = new System.Drawing.Font ("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox1.Location = new System.Drawing.Point (12, 131);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox1.Size = new System.Drawing.Size (877, 177);
			this.textBox1.TabIndex = 0;
			// 
			// rbListWise
			// 
			this.rbListWise.AutoSize = true;
			this.rbListWise.Location = new System.Drawing.Point (15, 14);
			this.rbListWise.Name = "rbListWise";
			this.rbListWise.Size = new System.Drawing.Size (104, 17);
			this.rbListWise.TabIndex = 1;
			this.rbListWise.Text = "Listwise Deletion";
			this.rbListWise.UseVisualStyleBackColor = true;
			this.rbListWise.Click += new System.EventHandler (this.RbListWiseCheckedChanged);
			// 
			// rbPairWise
			// 
			this.rbPairWise.AutoSize = true;
			this.rbPairWise.Checked = true;
			this.rbPairWise.Location = new System.Drawing.Point (15, 37);
			this.rbPairWise.Name = "rbPairWise";
			this.rbPairWise.Size = new System.Drawing.Size (106, 17);
			this.rbPairWise.TabIndex = 2;
			this.rbPairWise.TabStop = true;
			this.rbPairWise.Text = "Pairwise Deletion";
			this.rbPairWise.UseVisualStyleBackColor = true;
			this.rbPairWise.Click += new System.EventHandler (this.RbPairWiseCheckedChanged);
			// 
			// rbZeroReplace
			// 
			this.rbZeroReplace.AutoSize = true;
			this.rbZeroReplace.Location = new System.Drawing.Point (15, 60);
			this.rbZeroReplace.Name = "rbZeroReplace";
			this.rbZeroReplace.Size = new System.Drawing.Size (98, 17);
			this.rbZeroReplace.TabIndex = 3;
			this.rbZeroReplace.Text = "Omitted to Zero";
			this.rbZeroReplace.UseVisualStyleBackColor = true;
			this.rbZeroReplace.Click += new System.EventHandler (this.RbZeroReplaceCheckedChanged);
			// 
			// bcConvertCR
			// 
			this.bcConvertCR.AutoSize = true;
			this.bcConvertCR.Checked = true;
			this.bcConvertCR.CheckState = System.Windows.Forms.CheckState.Checked;
			this.bcConvertCR.Location = new System.Drawing.Point (15, 95);
			this.bcConvertCR.Name = "bcConvertCR";
			this.bcConvertCR.Size = new System.Drawing.Size (146, 17);
			this.bcConvertCR.TabIndex = 4;
			this.bcConvertCR.Text = "Convert CR to Dichotomy";
			this.bcConvertCR.UseVisualStyleBackColor = true;
			this.bcConvertCR.CheckedChanged += new System.EventHandler (this.BcConvertCRCheckedChanged);
			// 
			// cbOutCSV
			// 
			this.cbOutCSV.AutoSize = true;
			this.cbOutCSV.Checked = true;
			this.cbOutCSV.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbOutCSV.Location = new System.Drawing.Point (242, 15);
			this.cbOutCSV.Name = "cbOutCSV";
			this.cbOutCSV.Size = new System.Drawing.Size (124, 17);
			this.cbOutCSV.TabIndex = 5;
			this.cbOutCSV.Text = "Ouput CSV Data File";
			this.cbOutCSV.UseVisualStyleBackColor = true;
			this.cbOutCSV.CheckedChanged += new System.EventHandler (this.CbOutCSVCheckedChanged);
			// 
			// cbOutHTML
			// 
			this.cbOutHTML.AutoSize = true;
			this.cbOutHTML.Checked = true;
			this.cbOutHTML.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbOutHTML.Location = new System.Drawing.Point (242, 38);
			this.cbOutHTML.Name = "cbOutHTML";
			this.cbOutHTML.Size = new System.Drawing.Size (126, 17);
			this.cbOutHTML.TabIndex = 6;
			this.cbOutHTML.Text = "Output HTML Report";
			this.cbOutHTML.UseVisualStyleBackColor = true;
			this.cbOutHTML.CheckedChanged += new System.EventHandler (this.CbOutHTMLCheckedChanged);
			// 
			// cbZScores
			// 
			this.cbZScores.AutoSize = true;
			this.cbZScores.Checked = true;
			this.cbZScores.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbZScores.Location = new System.Drawing.Point (242, 95);
			this.cbZScores.Name = "cbZScores";
			this.cbZScores.Size = new System.Drawing.Size (128, 17);
			this.cbZScores.TabIndex = 7;
			this.cbZScores.Text = "Output Z-Scores CSV";
			this.cbZScores.UseVisualStyleBackColor = true;
			this.cbZScores.CheckedChanged += new System.EventHandler (this.CbZScoresCheckedChanged);
			// 
			// cbReferences
			// 
			this.cbReferences.AutoSize = true;
			this.cbReferences.Location = new System.Drawing.Point (262, 61);
			this.cbReferences.Name = "cbReferences";
			this.cbReferences.Size = new System.Drawing.Size (121, 17);
			this.cbReferences.TabIndex = 8;
			this.cbReferences.Text = "Append References";
			this.cbReferences.UseVisualStyleBackColor = true;
			this.cbReferences.CheckedChanged += new System.EventHandler (this.CbReferencesCheckedChanged);
			// 
			// cmdSingle
			// 
			this.cmdSingle.Location = new System.Drawing.Point (610, 14);
			this.cmdSingle.Name = "cmdSingle";
			this.cmdSingle.Size = new System.Drawing.Size (118, 48);
			this.cmdSingle.TabIndex = 9;
			this.cmdSingle.Text = "Run Single Report";
			this.cmdSingle.UseVisualStyleBackColor = true;
			this.cmdSingle.Click += new System.EventHandler (this.cmdSingle_Click);
			// 
			// cmdBatch
			// 
			this.cmdBatch.Location = new System.Drawing.Point (610, 68);
			this.cmdBatch.Name = "cmdBatch";
			this.cmdBatch.Size = new System.Drawing.Size (118, 48);
			this.cmdBatch.TabIndex = 10;
			this.cmdBatch.Text = "Run Batch Reports";
			this.cmdBatch.UseVisualStyleBackColor = true;
			this.cmdBatch.Click += new System.EventHandler (this.cmdBatch_Click);
			// 
			// cbEduphoria
			// 
			this.cbEduphoria.AutoSize = true;
			this.cbEduphoria.Checked = true;
			this.cbEduphoria.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbEduphoria.Location = new System.Drawing.Point (463, 14);
			this.cbEduphoria.Name = "cbEduphoria";
			this.cbEduphoria.Size = new System.Drawing.Size (111, 17);
			this.cbEduphoria.TabIndex = 11;
			this.cbEduphoria.Text = "Eduphoria Source";
			this.cbEduphoria.UseVisualStyleBackColor = true;
			this.cbEduphoria.CheckedChanged += new System.EventHandler (this.CbEduphoriaCheckedChanged);
			// 
			// cbPerformMatter
			// 
			this.cbPerformMatter.AutoSize = true;
			this.cbPerformMatter.Location = new System.Drawing.Point (463, 36);
			this.cbPerformMatter.Name = "cbPerformMatter";
			this.cbPerformMatter.Size = new System.Drawing.Size (89, 17);
			this.cbPerformMatter.TabIndex = 12;
			this.cbPerformMatter.Text = "Other Source";
			this.cbPerformMatter.UseVisualStyleBackColor = true;
			this.cbPerformMatter.CheckedChanged += new System.EventHandler (this.CbPerformMatterCheckedChanged);
			// 
			// cmdExit
			// 
			this.cmdExit.Location = new System.Drawing.Point (771, 15);
			this.cmdExit.Name = "cmdExit";
			this.cmdExit.Size = new System.Drawing.Size (118, 48);
			this.cmdExit.TabIndex = 13;
			this.cmdExit.Text = "Exit";
			this.cmdExit.UseVisualStyleBackColor = true;
			this.cmdExit.Click += new System.EventHandler (this.cmdExit_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size (901, 321);
			this.Controls.Add (this.cmdExit);
			this.Controls.Add (this.cbPerformMatter);
			this.Controls.Add (this.cbEduphoria);
			this.Controls.Add (this.cmdBatch);
			this.Controls.Add (this.cmdSingle);
			this.Controls.Add (this.cbReferences);
			this.Controls.Add (this.cbZScores);
			this.Controls.Add (this.cbOutHTML);
			this.Controls.Add (this.cbOutCSV);
			this.Controls.Add (this.bcConvertCR);
			this.Controls.Add (this.rbZeroReplace);
			this.Controls.Add (this.rbPairWise);
			this.Controls.Add (this.rbListWise);
			this.Controls.Add (this.textBox1);
			this.Name = "frmMain";
			this.Text = "CSDataMiner 2.0";
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.OpenFileDialog oFD;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.FolderBrowserDialog fBD;
		private System.Windows.Forms.RadioButton rbListWise;
		private System.Windows.Forms.RadioButton rbPairWise;
		private System.Windows.Forms.RadioButton rbZeroReplace;
		private System.Windows.Forms.CheckBox bcConvertCR;
		private System.Windows.Forms.CheckBox cbOutCSV;
		private System.Windows.Forms.CheckBox cbOutHTML;
		private System.Windows.Forms.CheckBox cbZScores;
		private System.Windows.Forms.CheckBox cbReferences;
		private System.Windows.Forms.Button cmdSingle;
		private System.Windows.Forms.Button cmdBatch;
		private System.Windows.Forms.CheckBox cbEduphoria;
		private System.Windows.Forms.CheckBox cbPerformMatter;
		private System.Windows.Forms.Button cmdExit;
	}
}

