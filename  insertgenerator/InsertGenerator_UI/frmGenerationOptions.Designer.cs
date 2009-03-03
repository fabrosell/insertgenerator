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
            this.chkGenerateTransactional = new System.Windows.Forms.CheckBox();
            this.chkGenerateTables = new System.Windows.Forms.CheckBox();
            this.chkGenerateDerived = new System.Windows.Forms.CheckBox();
            this.chkGenerateCorrelated = new System.Windows.Forms.CheckBox();
            this.rbIdentitiesFromTable = new System.Windows.Forms.RadioButton();
            this.rbInsertionDependant = new System.Windows.Forms.RadioButton();
            this.rbNoIdentity = new System.Windows.Forms.RadioButton();
            this.gbIdentityOptions = new System.Windows.Forms.GroupBox();
            this.gbGeneralOptions = new System.Windows.Forms.GroupBox();
            this.lblRows = new System.Windows.Forms.Label();
            this.nudTopRows = new System.Windows.Forms.NumericUpDown();
            this.lblLPSB = new System.Windows.Forms.Label();
            this.nudLinesPerSQLBlock = new System.Windows.Forms.NumericUpDown();
            this.gbIdentityOptions.SuspendLayout();
            this.gbGeneralOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTopRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLinesPerSQLBlock)).BeginInit();
            this.SuspendLayout();
            // 
            // chkGenerateTransactional
            // 
            this.chkGenerateTransactional.AutoSize = true;
            this.chkGenerateTransactional.Enabled = false;
            this.chkGenerateTransactional.Location = new System.Drawing.Point(6, 19);
            this.chkGenerateTransactional.Name = "chkGenerateTransactional";
            this.chkGenerateTransactional.Size = new System.Drawing.Size(120, 17);
            this.chkGenerateTransactional.TabIndex = 1;
            this.chkGenerateTransactional.Text = "Transactional Script";
            this.chkGenerateTransactional.UseVisualStyleBackColor = true;
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
            this.gbIdentityOptions.Location = new System.Drawing.Point(12, 188);
            this.gbIdentityOptions.Name = "gbIdentityOptions";
            this.gbIdentityOptions.Size = new System.Drawing.Size(231, 95);
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
            this.gbGeneralOptions.Size = new System.Drawing.Size(236, 170);
            this.gbGeneralOptions.TabIndex = 9;
            this.gbGeneralOptions.TabStop = false;
            this.gbGeneralOptions.Text = "General Options";
            // 
            // lblRows
            // 
            this.lblRows.AutoSize = true;
            this.lblRows.Location = new System.Drawing.Point(106, 113);
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
            this.nudTopRows.Location = new System.Drawing.Point(6, 111);
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
            // lblLPSB
            // 
            this.lblLPSB.AutoSize = true;
            this.lblLPSB.Location = new System.Drawing.Point(106, 139);
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
            this.nudLinesPerSQLBlock.Location = new System.Drawing.Point(6, 137);
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
            // frmScriptOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(260, 294);
            this.Controls.Add(this.gbGeneralOptions);
            this.Controls.Add(this.gbIdentityOptions);
            this.Name = "frmScriptOptions";
            this.ShowInTaskbar = false;
            this.Text = "Script Options";
            this.Load += new System.EventHandler(this.frmScriptOptions_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmScriptOptions_FormClosing);
            this.gbIdentityOptions.ResumeLayout(false);
            this.gbIdentityOptions.PerformLayout();
            this.gbGeneralOptions.ResumeLayout(false);
            this.gbGeneralOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTopRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLinesPerSQLBlock)).EndInit();
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
    }
}