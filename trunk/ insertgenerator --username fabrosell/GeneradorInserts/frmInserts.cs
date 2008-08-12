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
    public partial class frmInserts : Form
    {
        private Connection DBConnection;
        private List<String> TableList = null;
        private const String CheckBoxColumnName = "[Selected]";
        private const String TableColumnName = "[Table]";
        private frmConnectServer Parent;

        /// <summary>
        /// Class' constructor.
        /// </summary>
        /// <param name="conn">Connection to work with.</param>
        public frmInserts(Connection conn, frmConnectServer fParent)
        {
            DBConnection = conn;

            Parent = fParent;

            InitializeComponent();
        }

        private void frmInserts_Load(object sender, EventArgs e)
        {
            this.Text = "Suru SQL Insert Generator - v." + Application.ProductVersion;            

            //Load DataBases in ComboBox
            foreach (String s in DBConnection.DataBases)
                cmbDataBase.Items.Add(s);

            cmbDataBase.SelectedIndex = cmbDataBase.FindString(DBConnection.Last_DataBase);

            //Load the database tables
            Load_DataBase_Tables();
        }

        /// <summary>
        /// This method updates DB Combobox and Status Bar Host and User Name
        /// </summary>
        private void UpdateForm()
        {            
            throw new Exception("This method has not been implemented.");
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
                //TableList is null: user has not enough privileges or some error happened.
            }
        }

        //When the Selected Database changes, Table DataGrid must be reloaded.
        private void cmbDataBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            Load_DataBase_Tables();
            dgvTables.Focus();
        }

        //When form is closing, parent and current form must be disposed
        private void frmInserts_FormClosing(object sender, FormClosingEventArgs e)
        {
            Parent.Dispose();
            this.Dispose();
        }
    }
}