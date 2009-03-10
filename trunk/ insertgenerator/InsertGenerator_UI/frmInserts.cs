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
        private Boolean LoadingForm;

        //Worker thread
        private Thread WorkThread = null;

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

        /// <summary>
        /// This method initializes the database / tables form
        /// </summary>
        private void Load_Form()
        {
            this.Invoke((MethodInvoker)delegate()
            {
                LoadingForm = true;
                BlockControls(true);

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

                BlockControls(false);
            }
            );

            //Load the database tables
            Load_DataBase_Tables();

            UpdateForm();

            LoadingForm = false;

            if (WorkThread != null)
                WorkThread = null;
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
            this.Invoke((MethodInvoker)delegate()
            {
                BlockControls(true);

                DBConnection.Last_DataBase = cmbDataBase.Text;
            }
            );

            TableList = DBConnection.ListDatabaseTables();

            if (TableList != null)
            {
                //If there is any table, load the Results
                if (TableList.Count != 0)
                {
                    this.Invoke((MethodInvoker)delegate()
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
                    );
                }
                else
                    tssErrorMessage.Text = DBConnection.ErrorMessage;

                this.Invoke((MethodInvoker)delegate()
                {
                    dgvTables.Rows.Clear();
                }
                );

                DataGridViewRow dr;

                //Loads table on DataGrid
                Int32 iNumFila = 0;
                foreach (String s in TableList)
                {
                    this.Invoke((MethodInvoker)delegate()
                    {
                        dr = new DataGridViewRow();
                        dgvTables.Rows.Add(dr);

                        dgvTables.Rows[iNumFila].Cells[CheckBoxColumnName] = new DataGridViewCheckBoxCell();
                        dgvTables.Rows[iNumFila].Cells[CheckBoxColumnName].Value = false;

                        dgvTables.Rows[iNumFila].Cells[TableColumnName] = new DataGridViewTextBoxCell();
                        dgvTables.Rows[iNumFila].Cells[TableColumnName].Value = s;                        
                    }
                    );

                    iNumFila++;
                }
            }
            else
            {
                this.Invoke((MethodInvoker)delegate()
                {
                    dgvTables.Rows.Clear();
                    UpdateForm();
                }
                );
            }

            this.Invoke((MethodInvoker)delegate()
            {
                BlockControls(false);
            }
            );

            if (WorkThread != null)
                WorkThread = null;
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

        /// <summary>
        /// Block and release the form controls.
        /// </summary>
        /// <param name="bLock">True: disable controls. False: enable controls.</param>
        private void BlockControls(Boolean bLock)
        {
            cmbDataBase.Enabled = !bLock;
            btnChangePath.Enabled = !bLock;
            btnChangeConnection.Enabled = !bLock;
            chkSelectAll.Enabled = !bLock;
            btnGenerateInserts.Enabled = !bLock;
            btnOptions.Enabled = !bLock;

            //Cannot block grid for some reason...
            //dgvTables.Enabled = !bLock;
        }

        /// <summary>
        /// Generates the inserts
        /// </summary>
        private void Generate_Inserts()
        {
            List<String> lTablesToGenerate = new List<String>();
            String OutputPath = null;

            this.Invoke((MethodInvoker)delegate()
            {
                BlockControls(true);

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

                OutputPath = lblOutputPath.Text;
            }
            );

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
            SQLGeneration ScriptGenerator = new SQLGeneration(DBConnection, gGenOptions, OutputPath);

            //Method will return true if graph has cycles
            if (!ScriptGenerator.GenerateInserts(lTablesToGenerate))
                MessageBox.Show("Scripts Generated succesfully!", "Generation End", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Data tables has cycles. Table order could not be the right one.", "Generation End with Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);


            BlockControls(false);

            if (WorkThread != null)
                WorkThread = null;
        }

        /// <summary>
        /// Selects or unselects all tables listed in tables' data grid view.
        /// </summary>
        private void SelectAllTables()
        {
            //Cannot block controls due to graphical issue...
            //Cell is correctly checked on / off when "select all" changed...
            //but system selected cell won't show it.

            //BlockControls(true);

            DataGridViewCheckBoxCell dchk;

            foreach (DataGridViewRow dr in dgvTables.Rows)
            {
                dchk = (DataGridViewCheckBoxCell)dr.Cells[CheckBoxColumnName];
                dchk.Value = chkSelectAll.Checked;
            }            

            //BlockControls(false);
        }

        #endregion

        #region Events

        //Load Event Handler
        private void frmInserts_Load(object sender, EventArgs e)
        {
            this.Text = "Suru SQL Insert Generator - v." + Application.ProductVersion;

            ThreadStart ts = new ThreadStart(Load_Form);
            WorkThread = new Thread(ts);
            WorkThread.Start();

            //Load_Form();
        }

        //When the Selected Database changes, Table DataGrid must be reloaded.
        private void cmbDataBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!LoadingForm)
            {
                ThreadStart ts = new ThreadStart(Load_DataBase_Tables);
                WorkThread = new Thread(ts);
                WorkThread.Start();

                //Load_DataBase_Tables();
                dgvTables.Focus();

                chkSelectAll.Checked = false;
            }
        }

        //When form is closing, parent and current form must be disposed
        private void frmInserts_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WorkThread == null)
            {
                OriginalParent.Dispose();
                this.Dispose();
            }
            else
            {
                MessageBox.Show("Cannot close application while an operation is in progress.", "Please wait for completion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }
        }

        //Generate Insert Button Event Handler
        private void btnGenerateInserts_Click(object sender, EventArgs e)
        {
            ThreadStart ts = new ThreadStart(Generate_Inserts);
            WorkThread = new Thread(ts);
            WorkThread.Start();
        }

        //Select All Event Handler
        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            SelectAllTables();
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

            ThreadStart ts = new ThreadStart(Load_Form);
            WorkThread = new Thread(ts);
            WorkThread.Start();

            //Load_Form();
        }

        //Enter and select over selected row
        private void dgvTables_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                if (dgvTables.SelectedRows != null)
                {
                    DataGridViewCheckBoxCell dchk;

                    foreach (DataGridViewRow dr in dgvTables.SelectedRows)
                    {
                        dchk = (DataGridViewCheckBoxCell)dr.Cells[CheckBoxColumnName];
                        dchk.Value = !(Boolean)dchk.Value;
                    }
                }
        }       

        #endregion
    }
}