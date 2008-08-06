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

        /// <summary>
        /// Class' constructor.
        /// </summary>
        /// <param name="conn">Connection to work with.</param>
        public frmInserts(Connection conn)
        {
            DBConnection = conn;

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
                        dgvTables.Columns.Add(dchkColumn);

                        DataGridViewTextBoxColumn dtxtColumn = new DataGridViewTextBoxColumn();
                        dtxtColumn.Name = TableColumnName;
                        dtxtColumn.ReadOnly = true;
                        dtxtColumn.HeaderText = "Table";
                    }
                }
                else
                    tssErrorMessage.Text = DBConnection.ErrorMessage;

                dgvTables.Rows.Clear();

                DataGridViewCheckBoxCell dchkcell;
                DataGridViewTextBoxCell dtxtCell;
                DataGridViewRow dr;

                //Loads table on DataGrid
                foreach (String s in TableList)
                {
                    dr = new DataGridViewRow();

                    dchkcell = new DataGridViewCheckBoxCell();
                    dchkcell.Value = false;
                    dr.Cells.Add(dchkcell);

                    dtxtCell = new DataGridViewTextBoxCell();
                    dtxtCell.Value = s;
                    dr.Cells.Add(dtxtCell);

                    dgvTables.Rows.Add(dr);
                }
            }
            else
            {
                //TableList is null: user has not enough privileges or some error happened.
            }
        }
    }
}