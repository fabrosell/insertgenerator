using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Suru.InsertGenerator.BusinessLogic;

namespace Suru.InsertGenerator.GeneradorUI
{
    public partial class frmScriptOptions : Form
    {
        private frmInserts _Parent = null;

        public frmScriptOptions(frmInserts Parent)
        {
            _Parent = Parent;
            InitializeComponent();
        }

        private void frmScriptOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Transactional Script Option Enabled
            if (chkGenerateTransactional.Checked)
                _Parent.gGenOptions.TransacionalScript = true;
            else
                _Parent.gGenOptions.TransacionalScript = false;

            //Generate Table Script Option Enabled
            if (chkGenerateTables.Checked)
                _Parent.gGenOptions.IncludeTableScripts = true;
            else
                _Parent.gGenOptions.IncludeTableScripts = false;

            //Generate Derived Tables Script Option Enabled
            if (chkGenerateDerived.Checked)
                _Parent.gGenOptions.DerivedTableScript = true;
            else
                _Parent.gGenOptions.DerivedTableScript = false;

            //Generate Correlated Data Script Option Enabled
            if (chkGenerateCorrelated.Checked)
                _Parent.gGenOptions.CorrelatedDataTablesScript = true;
            else
                _Parent.gGenOptions.CorrelatedDataTablesScript = false;

            IdentityGenerationOptions IdentityGenOpts = IdentityGenerationOptions.OmitIdentityColumns;

            if (rbIdentitiesFromTable.Checked)
                IdentityGenOpts = IdentityGenerationOptions.IdentityInsert;

            if (rbInsertionDependant.Checked)
                IdentityGenOpts = IdentityGenerationOptions.InsertionDependant;

            //Top Rows, if apply
            if (nudTopRows.Value != 0)
                _Parent.gGenOptions.TopRows = (Int32)nudTopRows.Value;
            else
                _Parent.gGenOptions.TopRows = null;

            //Max lines per block
            _Parent.gGenOptions.LinesPerBlock = (Int32)nudLinesPerSQLBlock.Value;

            _Parent.gGenOptions.IdentityOptions = IdentityGenOpts;
        }

        private void frmScriptOptions_Load(object sender, EventArgs e)
        {
            try
            {
                InitializarNudLinesPerSQLBlock();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Lines per Block setting!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }
        }

        private void InitializarNudLinesPerSQLBlock()
        {
            nudLinesPerSQLBlock.Maximum = Int32.Parse(ConfigurationManager.AppSettings.Get("LinesPerBlock"));
            nudLinesPerSQLBlock.Minimum = 1;
            nudLinesPerSQLBlock.Increment = 100;
            nudLinesPerSQLBlock.Value = nudLinesPerSQLBlock.Maximum;
            nudLinesPerSQLBlock.Enabled = true;
        }

        private void chkGenerateTransactional_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGenerateTransactional.Checked)
            {
                if (DialogResult.No == MessageBox.Show("Warning: you won't we allowed to break the script after " + nudLinesPerSQLBlock.Value.ToString() + " lines. Do you want to continue?", "Break script disabled", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                {
                    nudLinesPerSQLBlock.Enabled = true;
                    chkGenerateTransactional.Checked = false;
                }
                else
                {                    
                    nudLinesPerSQLBlock.Enabled = false;
                }
            }
            else
            {
                nudLinesPerSQLBlock.Enabled = true;
            }
        }
    }
}