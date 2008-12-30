using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
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

            _Parent.gGenOptions.IdentityOptions = IdentityGenOpts;
        }
    }
}