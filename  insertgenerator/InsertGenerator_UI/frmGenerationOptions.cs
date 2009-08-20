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

        private void frmScriptOptions_Load(object sender, EventArgs e)
        {
            try
            {
                InitializarNudLinesPerSQLBlock();

                LoadEncodingOptions();

                #region Initializes settings from parent form

                chkGenerateCorrelated.Checked = _Parent.gGenOptions.CorrelatedDataTablesScript;
                chkGenerateDerived.Checked = _Parent.gGenOptions.DerivedTableScript;

                if (_Parent.gGenOptions.FileEncoding == Encoding.UTF7)
                    cmbEncodings.SelectedIndex = 0;

                if (_Parent.gGenOptions.FileEncoding == Encoding.UTF8)
                    cmbEncodings.SelectedIndex = 1;
                
                if (_Parent.gGenOptions.FileEncoding == Encoding.Unicode)
                    cmbEncodings.SelectedIndex = 2;
                
                if (_Parent.gGenOptions.FileEncoding == Encoding.UTF32)
                    cmbEncodings.SelectedIndex = 1;

                switch (_Parent.gGenOptions.IdentityOptions)
                {
                    case IdentityGenerationOptions.IdentityInsert:
                        rbIdentitiesFromTable.Checked = true;
                        rbInsertionDependant.Checked = false;
                        rbNoIdentity.Checked = false;
                        break;
                    case IdentityGenerationOptions.InsertionDependant:
                        rbIdentitiesFromTable.Checked = false;
                        rbInsertionDependant.Checked = true;
                        rbNoIdentity.Checked = false;
                        break;
                    case IdentityGenerationOptions.OmitIdentityColumns:
                        rbIdentitiesFromTable.Checked = false;
                        rbInsertionDependant.Checked = false;
                        rbNoIdentity.Checked = true;
                        break;
                }

                chkGenerateTables.Checked = _Parent.gGenOptions.IncludeTableScripts;
                nudLinesPerSQLBlock.Value = _Parent.gGenOptions.LinesPerBlock;
                chkSQL2000Comp.Checked = _Parent.gGenOptions.SQL2000Compatible;

                if (_Parent.gGenOptions.TopRows != null)
                    nudTopRows.Value = (Int32)_Parent.gGenOptions.TopRows;
                else
                    nudTopRows.Value = 0;

                chkGenerateTransactional.Checked = _Parent.gGenOptions.TransacionalScript;  
              
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid Lines per Block setting!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }
        }

        private void LoadEncodingOptions()
        {
            cmbEncodings.Items.Clear();
            cmbEncodings.Items.Add("ASCII");
            cmbEncodings.Items.Add("UTF-8");
            cmbEncodings.Items.Add("UTF-16");

            //UTF-8 by default
            cmbEncodings.SelectedIndex = 1;
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

        private void btnSaveChanges_Click(object sender, EventArgs e)
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


            //SQL 2000 compatible Script
            if (chkSQL2000Comp.Checked)
                _Parent.gGenOptions.SQL2000Compatible = true;
            else
                _Parent.gGenOptions.SQL2000Compatible = false;

            //Encoding Types
            Encoding eEncoding;

            switch ((String)cmbEncodings.SelectedItem)
            {
                case "ASCII":
                    eEncoding = Encoding.ASCII;
                    break;
                case "UTF-7":
                    eEncoding = Encoding.UTF7;
                    break;
                case "UTF-16":
                    eEncoding = Encoding.BigEndianUnicode;
                    break;
                case "UTF-32":
                    eEncoding = Encoding.UTF32;
                    break;
                default:
                    eEncoding = Encoding.UTF8;
                    break;
            }

            _Parent.gGenOptions.FileEncoding = eEncoding;

            _Parent.gGenOptions.IdentityOptions = IdentityGenOpts;

            this.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void chkSQL2000Comp_CheckedChanged(object sender, EventArgs e)
        {
            cmbEncodings.Enabled = !chkSQL2000Comp.Checked;
        }        
    }
}