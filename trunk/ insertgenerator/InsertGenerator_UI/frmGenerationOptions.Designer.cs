namespace Suru.InsertGenerator.GeneradorUI
{
    partial class frmScriptOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScriptOptions));
            this.chkGenerateTransactional = new System.Windows.Forms.CheckBox();
            this.chkGenerateTables = new System.Windows.Forms.CheckBox();
            this.chkGenerateDerived = new System.Windows.Forms.CheckBox();
            this.chkGenerateCorrelated = new System.Windows.Forms.CheckBox();
            this.rbIdentitiesFromTable = new System.Windows.Forms.RadioButton();
            this.rbInsertionDependant = new System.Windows.Forms.RadioButton();
            this.rbNoIdentity = new System.Windows.Forms.RadioButton();
            this.gbIdentityOptions = new System.Windows.Forms.GroupBox();
            this.gbGeneralOptions = new System.Windows.Forms.GroupBox();
            this.chkSQL2000Comp = new System.Windows.Forms.CheckBox();
            this.lblLPSB = new System.Windows.Forms.Label();
            this.nudLinesPerSQLBlock = new System.Windows.Forms.NumericUpDown();
            this.lblRows = new System.Windows.Forms.Label();
            this.nudTopRows = new System.Windows.Forms.NumericUpDown();
            this.gbFileOptions = new System.Windows.Forms.GroupBox();
            this.cmbEncodings = new System.Windows.Forms.ComboBox();
            this.lblEncoding = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.gbIdentityOptions.SuspendLayout();
            this.gbGeneralOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLinesPerSQLBlock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTopRows)).BeginInit();
            this.gbFileOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkGenerateTransactional
            // 
            this.chkGenerateTransactional.AutoSize = true;
            this.chkGenerateTransactional.Location = new System.Drawing.Point(6, 19);
            this.chkGenerateTransactional.Name = "chkGenerateTransactional";
            this.chkGenerateTransactional.Size = new System.Drawing.Size(120, 17);
            this.chkGenerateTransactional.TabIndex = 1;
            this.chkGenerateTransactional.Text = "Transactional Script";
            this.chkGenerateTransactional.UseVisualStyleBackColor = true;
            this.chkGenerateTransactional.CheckedChanged += new System.EventHandler(this.chkGenerateTransactional_CheckedChanged);
            // 
            // chkGenerateTables
            // 
            this.chkGenerateTables.AutoSize = true;
            this.chkGenerateTables.Enabled = false;
            this.chkGenerateTables.Location = new System.Drawing.Point(6, 42);
            this.chkGenerateTables.Name = "chkGenerateTables";
            this.chkGenerateTables.Size = new System.Drawing.Size(177, 17);
            this.chkGenerateTables.TabIndex = 2;
            this.chkGenerateTables.Text = "Generate Table Creation Scripts";
            this.chkGenerateTables.UseVisualStyleBackColor = true;
            // 
            // chkGenerateDerived
            // 
            this.chkGenerateDerived.AutoSize = true;
            this.chkGenerateDerived.Enabled = false;
            this.chkGenerateDerived.Location = new System.Drawing.Point(6, 65);
            this.chkGenerateDerived.Name = "chkGenerateDerived";
            this.chkGenerateDerived.Size = new System.Drawing.Size(191, 17);
            this.chkGenerateDerived.TabIndex = 3;
            this.chkGenerateDerived.Text = "Generate Inserts of Derived Tables";
            this.chkGenerateDerived.UseVisualStyleBackColor = true;
            // 
            // chkGenerateCorrelated
            // 
            this.chkGenerateCorrelated.AutoSize = true;
            this.chkGenerateCorrelated.Enabled = false;
            this.chkGenerateCorrelated.Location = new System.Drawing.Point(6, 88);
            this.chkGenerateCorrelated.Name = "chkGenerateCorrelated";
            this.chkGenerateCorrelated.Size = new System.Drawing.Size(228, 17);
            this.chkGenerateCorrelated.TabIndex = 4;
            this.chkGenerateCorrelated.Text = "Generate Inserts of Correlated Data Tables";
            this.chkGenerateCorrelated.UseVisualStyleBackColor = true;
            // 
            // rbIdentitiesFromTable
            // 
            this.rbIdentitiesFromTable.AutoSize = true;
            this.rbIdentitiesFromTable.Location = new System.Drawing.Point(6, 19);
            this.rbIdentitiesFromTable.Name = "rbIdentitiesFromTable";
            this.rbIdentitiesFromTable.Size = new System.Drawing.Size(222, 17);
            this.rbIdentitiesFromTable.TabIndex = 6;
            this.rbIdentitiesFromTable.TabStop = true;
            this.rbIdentitiesFromTable.Text = "Enable Identity Insert (values from Tables)";
            this.rbIdentitiesFromTable.UseVisualStyleBackColor = true;
            // 
            // rbInsertionDependant
            // 
            this.rbInsertionDependant.AutoSize = true;
            this.rbInsertionDependant.Enabled = false;
            this.rbInsertionDependant.Location = new System.Drawing.Point(6, 42);
            this.rbInsertionDependant.Name = "rbInsertionDependant";
            this.rbInsertionDependant.Size = new System.Drawing.Size(158, 17);
            this.rbInsertionDependant.TabIndex = 7;
            this.rbInsertionDependant.TabStop = true;
            this.rbInsertionDependant.Text = "Insertion-Dependant Identity";
            this.rbInsertionDependant.UseVisualStyleBackColor = true;
            // 
            // rbNoIdentity
            // 
            this.rbNoIdentity.AutoSize = true;
            this.rbNoIdentity.Location = new System.Drawing.Point(6, 65);
            this.rbNoIdentity.Name = "rbNoIdentity";
            this.rbNoIdentity.Size = new System.Drawing.Size(180, 17);
            this.rbNoIdentity.TabIndex = 8;
            this.rbNoIdentity.TabStop = true;
            this.rbNoIdentity.Text = "No Identity Inserts (omit columns)";
            this.rbNoIdentity.UseVisualStyleBackColor = true;
            // 
            // gbIdentityOptions
            // 
            this.gbIdentityOptions.Controls.Add(this.rbIdentitiesFromTable);
            this.gbIdentityOptions.Controls.Add(this.rbNoIdentity);
            this.gbIdentityOptions.Controls.Add(this.rbInsertionDependant);
            this.gbIdentityOptions.Location = new System.Drawing.Point(10, 178);
            this.gbIdentityOptions.Name = "gbIdentityOptions";
            this.gbIdentityOptions.Size = new System.Drawing.Size(236, 95);
            this.gbIdentityOptions.TabIndex = 8;
            this.gbIdentityOptions.TabStop = false;
            this.gbIdentityOptions.Text = "Identity Options";
            // 
            // gbGeneralOptions
            // 
            this.gbGeneralOptions.Controls.Add(this.lblLPSB);
            this.gbGeneralOptions.Controls.Add(this.nudLinesPerSQLBlock);
            this.gbGeneralOptions.Controls.Add(this.lblRows);
            this.gbGeneralOptions.Controls.Add(this.nudTopRows);
            this.gbGeneralOptions.Controls.Add(this.chkGenerateTransactional);
            this.gbGeneralOptions.Controls.Add(this.chkGenerateTables);
            this.gbGeneralOptions.Controls.Add(this.chkGenerateCorrelated);
            this.gbGeneralOptions.Controls.Add(this.chkGenerateDerived);
            this.gbGeneralOptions.Location = new System.Drawing.Point(12, 12);
            this.gbGeneralOptions.Name = "gbGeneralOptions";
            this.gbGeneralOptions.Size = new System.Drawing.Size(236, 160);
            this.gbGeneralOptions.TabIndex = 9;
            this.gbGeneralOptions.TabStop = false;
            this.gbGeneralOptions.Text = "General Options";
            // 
            // chkSQL2000Comp
            // 
            this.chkSQL2000Comp.AutoSize = true;
            this.chkSQL2000Comp.Location = new System.Drawing.Point(9, 42);
            this.chkSQL2000Comp.Name = "chkSQL2000Comp";
            this.chkSQL2000Comp.Size = new System.Drawing.Size(159, 17);
            this.chkSQL2000Comp.TabIndex = 10;
            this.chkSQL2000Comp.Text = "SQL 2000 Compatible Script";
            this.chkSQL2000Comp.UseVisualStyleBackColor = true;
            this.chkSQL2000Comp.CheckedChanged += new System.EventHandler(this.chkSQL2000Comp_CheckedChanged);
            // 
            // lblLPSB
            // 
            this.lblLPSB.AutoSize = true;
            this.lblLPSB.Location = new System.Drawing.Point(106, 134);
            this.lblLPSB.Name = "lblLPSB";
            this.lblLPSB.Size = new System.Drawing.Size(107, 13);
            this.lblLPSB.TabIndex = 9;
            this.lblLPSB.Text = "Lines per SQL Block.";
            // 
            // nudLinesPerSQLBlock
            // 
            this.nudLinesPerSQLBlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudLinesPerSQLBlock.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudLinesPerSQLBlock.Location = new System.Drawing.Point(6, 132);
            this.nudLinesPerSQLBlock.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudLinesPerSQLBlock.Name = "nudLinesPerSQLBlock";
            this.nudLinesPerSQLBlock.Size = new System.Drawing.Size(91, 20);
            this.nudLinesPerSQLBlock.TabIndex = 8;
            this.nudLinesPerSQLBlock.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblRows
            // 
            this.lblRows.AutoSize = true;
            this.lblRows.Location = new System.Drawing.Point(106, 108);
            this.lblRows.Name = "lblRows";
            this.lblRows.Size = new System.Drawing.Size(91, 13);
            this.lblRows.TabIndex = 7;
            this.lblRows.Text = "Top rows (0 = all).";
            // 
            // nudTopRows
            // 
            this.nudTopRows.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudTopRows.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudTopRows.Location = new System.Drawing.Point(6, 106);
            this.nudTopRows.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudTopRows.Name = "nudTopRows";
            this.nudTopRows.Size = new System.Drawing.Size(91, 20);
            this.nudTopRows.TabIndex = 5;
            this.nudTopRows.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // gbFileOptions
            // 
            this.gbFileOptions.Controls.Add(this.chkSQL2000Comp);
            this.gbFileOptions.Controls.Add(this.cmbEncodings);
            this.gbFileOptions.Controls.Add(this.lblEncoding);
            this.gbFileOptions.Location = new System.Drawing.Point(12, 279);
            this.gbFileOptions.Name = "gbFileOptions";
            this.gbFileOptions.Size = new System.Drawing.Size(234, 70);
            this.gbFileOptions.TabIndex = 10;
            this.gbFileOptions.TabStop = false;
            this.gbFileOptions.Text = "File Options";
            // 
            // cmbEncodings
            // 
            this.cmbEncodings.FormattingEnabled = true;
            this.cmbEncodings.Items.AddRange(new object[] {
            "ASCII",
            "UTF-7",
            "UTF-8",
            "UTF-16",
            "UTF-32"});
            this.cmbEncodings.Location = new System.Drawing.Point(107, 15);
            this.cmbEncodings.Name = "cmbEncodings";
            this.cmbEncodings.Size = new System.Drawing.Size(121, 21);
            this.cmbEncodings.TabIndex = 1;
            // 
            // lblEncoding
            // 
            this.lblEncoding.AutoSize = true;
            this.lblEncoding.Location = new System.Drawing.Point(6, 18);
            this.lblEncoding.Name = "lblEncoding";
            this.lblEncoding.Size = new System.Drawing.Size(52, 13);
            this.lblEncoding.TabIndex = 0;
            this.lblEncoding.Text = "Encoding";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(132, 355);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(114, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Location = new System.Drawing.Point(10, 355);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(114, 23);
            this.btnSaveChanges.TabIndex = 12;
            this.btnSaveChanges.Text = "&Save Changes";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // frmScriptOptions
            // 
            this.AcceptButton = this.btnSaveChanges;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(260, 385);
            this.Controls.Add(this.btnSaveChanges);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbFileOptions);
            this.Controls.Add(this.gbGeneralOptions);
            this.Controls.Add(this.gbIdentityOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmScriptOptions";
            this.ShowInTaskbar = false;
            this.Text = "Script Options";
            this.Load += new System.EventHandler(this.frmScriptOptions_Load);
            this.gbIdentityOptions.ResumeLayout(false);
            this.gbIdentityOptions.PerformLayout();
            this.gbGeneralOptions.ResumeLayout(false);
            this.gbGeneralOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLinesPerSQLBlock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTopRows)).EndInit();
            this.gbFileOptions.ResumeLayout(false);
            this.gbFileOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkGenerateTransactional;
        private System.Windows.Forms.CheckBox chkGenerateTables;
        private System.Windows.Forms.CheckBox chkGenerateDerived;
        private System.Windows.Forms.CheckBox chkGenerateCorrelated;
        private System.Windows.Forms.RadioButton rbIdentitiesFromTable;
        private System.Windows.Forms.RadioButton rbInsertionDependant;
        private System.Windows.Forms.RadioButton rbNoIdentity;
        private System.Windows.Forms.GroupBox gbIdentityOptions;
        private System.Windows.Forms.GroupBox gbGeneralOptions;
        private System.Windows.Forms.Label lblRows;
        private System.Windows.Forms.NumericUpDown nudTopRows;
        private System.Windows.Forms.Label lblLPSB;
        private System.Windows.Forms.NumericUpDown nudLinesPerSQLBlock;
        private System.Windows.Forms.GroupBox gbFileOptions;
        private System.Windows.Forms.ComboBox cmbEncodings;
        private System.Windows.Forms.Label lblEncoding;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSaveChanges;
        private System.Windows.Forms.CheckBox chkSQL2000Comp;
    }
}