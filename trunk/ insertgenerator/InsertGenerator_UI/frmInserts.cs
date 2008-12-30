using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using Suru.InsertGenerator.BusinessLogic;

namespace Suru.InsertGenerator.GeneradorUI
{
    public partial class frmInserts : Form
    {
        #region Variables

        public Connection DBConnection;
        private List<String> TableList = null;
        private const String CheckBoxColumnName = "[Selected]";
        private const String TableColumnName = "[Table]";
        private frmConnectServer OriginalParent;
        public GenerationOptions gGenOptions = new GenerationOptions();

        #endregion

        #region Class Methods

        /// <summary>
        /// Class' constructor.
        /// </summary>
        /// <param name="conn">Connection to work with.</param>
        public frmInserts(Connection conn, frmConnectServer fParent)
        {
            DBConnection = conn;

            OriginalParent = fParent;

            InitializeComponent();
        }

        //Load Event Handler
        private void frmInserts_Load(object sender, EventArgs e)
        {
            this.Text = "Suru SQL Insert Generator - v." + Application.ProductVersion;

            Load_Form();
        }

        /// <summary>
        /// This method initializes the database / tables form
        /// </summary>
        private void Load_Form()
        {
            if (ConfigurationManager.AppSettings.Get("LastDirectoryUsed") == "")
            {
                lblOutputPath.Text = Application.StartupPath;
                Update_Last_Path_Used(Application.StartupPath);
            }
            else
                lblOutputPath.Text = ConfigurationManager.AppSettings.Get("LastDirectoryUsed");

            cmbDataBase.Items.Clear();

            //Load DataBases in ComboBox
            foreach (String s in DBConnection.DataBases)
                cmbDataBase.Items.Add(s);

            cmbDataBase.SelectedIndex = cmbDataBase.FindString(DBConnection.Last_DataBase);

            //Load the database tables
            Load_DataBase_Tables();

            UpdateForm();            
        }

        /// <summary>
        /// This method updates DB Combobox and Status Bar Host and User Name
        /// </summary>
        private void UpdateForm()
        {            
            tssServerName.Text = DBConnection.HostName;

            if (DBConnection.UserName == "")
                tssUserName.Text = SystemInformation.UserName;
            else
                tssUserName.Text = DBConnection.UserName;

            tssArrowKey.Text = "-->";

            tssErrorMessage.Text = DBConnection.ErrorMessage;
        }

        /// <summary>
        /// This method loads the current database tables into datagridview.
        /// </summary>
        private void Load_DataBase_Tables()
        {
            DBConnection.Last_DataBase = cmbDataBase.Text;

            TableList = DBConnection.ListDatabaseTables();

            if (TableList != null)
            {
                //If there is any table, load the Results
                if (TableList.Count != 0)
                {
                    if (dgvTables.Columns.Count == 0)
                    {
                        DataGridViewCheckBoxColumn dchkColumn = new DataGridViewCheckBoxColumn();
                        dchkColumn.Name = CheckBoxColumnName;
                        dchkColumn.HeaderText = "Select";
                        dchkColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        dgvTables.Columns.Add(dchkColumn);

                        DataGridViewTextBoxColumn dtxtColumn = new DataGridViewTextBoxColumn();
                        dtxtColumn.Name = TableColumnName;
                        dtxtColumn.ReadOnly = true;
                        dtxtColumn.HeaderText = "Table";                        
                        dgvTables.Columns.Add(dtxtColumn);

                        dgvTables.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
                else
                    tssErrorMessage.Text = DBConnection.ErrorMessage;

                dgvTables.Rows.Clear();

                DataGridViewRow dr;

                //Loads table on DataGrid
                Int32 iNumFila = 0;
                foreach (String s in TableList)
                {
                    dr = new DataGridViewRow();
                    dgvTables.Rows.Add(dr);                    
                    
                    dgvTables.Rows[iNumFila].Cells[CheckBoxColumnName] = new DataGridViewCheckBoxCell();
                    dgvTables.Rows[iNumFila].Cells[CheckBoxColumnName].Value = false;

                    dgvTables.Rows[iNumFila].Cells[TableColumnName] = new DataGridViewTextBoxCell();
                    dgvTables.Rows[iNumFila].Cells[TableColumnName].Value = s;

                    iNumFila++;
               }
            }
            else
            {
                dgvTables.Rows.Clear();
                UpdateForm();
            }

        }

        /// <summary>
        /// Saves into configuration file last path used by User.
        /// </summary>
        /// <param name="Path">Path used.</param>
        private void Update_Last_Path_Used(String Path)
        {
            Configuration ConfigurationFile = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            ConfigurationManager.AppSettings.Set("LastDirectoryUsed", Path);
            ConfigurationFile.AppSettings.Settings["LastDirectoryUsed"].Value = Path;

            ConfigurationFile.Save();
        }

        #endregion

        #region Events

        //When the Selected Database changes, Table DataGrid must be reloaded.
        private void cmbDataBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            Load_DataBase_Tables();
            dgvTables.Focus();
        }

        //When form is closing, parent and current form must be disposed
        private void frmInserts_FormClosing(object sender, FormClosingEventArgs e)
        {
            OriginalParent.Dispose();
            this.Dispose();
        }

        //Generate Insert Button Event Handler
        private void btnGenerateInserts_Click(object sender, EventArgs e)
        {
            List<String> lTablesToGenerate = new List<String>();

            //Getting checked rows from table
            DataGridViewCheckBoxCell dchk;
            foreach (DataGridViewRow dr in dgvTables.Rows)
            {
                dchk = (DataGridViewCheckBoxCell)dr.Cells[CheckBoxColumnName];

                if ((Boolean)dchk.Value)
                    lTablesToGenerate.Add((String)dr.Cells[TableColumnName].Value);
            }

            //You must select tables message.
            if (lTablesToGenerate.Count == 0)
            {
                MessageBox.Show("You must select one or more tables to Generate Inserts", "No tables selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvTables.Focus();
                return;
            }  
          
            /*
             *  Options to generation:
             * 
             *  Basic Ones
             *  -/ Posibility of generating inserts of correlated tables.
             *  -/ Posibility of generating inserts of correlated and derived tables
             *  -/ Posibility of mapping identity columns but not forcing identity insert (identities table-dependant).
             *  -/ Generating with tables and data creation scripts
             *  -/ Transactional creation script
             *  -/ Identity columns might be overriden or not (identity insert)
             *
             *  Future Addition
             *  - Generate Test Script (so data can be deleted and inserting to test the script)
             *  - Change destination table name
             *  - Including timestamp/rowversion insert statement
             *  - Top rows
             *  - Ommiting computed columns
             *  - Ommit image columns
             *  - Owner? (when you aren't)
             *  - Filtering Rows with a clause.
             *  - Exclude some columns
             *  - Include some columns
             */

            //Generate the Inserts
            SQLGeneration ScriptGenerator = new SQLGeneration(DBConnection, gGenOptions, lblOutputPath.Text);

            ScriptGenerator.GenerateInserts(lTablesToGenerate);

            MessageBox.Show("Scripts Generated succesfully!", "Generation End", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Select All Event Handler
        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            DataGridViewCheckBoxCell dchk;

            foreach (DataGridViewRow dr in dgvTables.Rows)
            {
                dchk = (DataGridViewCheckBoxCell)dr.Cells[CheckBoxColumnName];
                dchk.Value = chkSelectAll.Checked;
            }
        }

        //Generating Options
        private void btnOptions_Click(object sender, EventArgs e)
        {
            frmScriptOptions Dialog = new frmScriptOptions(this);

            Dialog.ShowDialog();
        }

        //Changing current Directory
        private void btnChangePath_Click(object sender, EventArgs e)
        {
            if (fbdBrowseDirectory.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    String DummyFileName = Path.Combine(fbdBrowseDirectory.SelectedPath, "____test_file_delete_please.txt");

                    //Test if app has writing permission
                    StreamWriter sw = new StreamWriter(DummyFileName);
                    sw.Close();

                    if (File.Exists(DummyFileName))
                        File.Delete(DummyFileName);
                }
                catch
                {
                    MessageBox.Show("Can't change folder to selected one because it's read only.", "Selected folder is Read Only", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                lblOutputPath.Text = fbdBrowseDirectory.SelectedPath;
                Update_Last_Path_Used(fbdBrowseDirectory.SelectedPath);
            }
        }

        //Changing current Connection
        private void btnChangeConnection_Click(object sender, EventArgs e)
        {
            frmConnectServer frmChangeConnection = new frmConnectServer(this);            
            frmChangeConnection.ShowDialog();
            Load_Form();
        }

        #endregion
    }
}