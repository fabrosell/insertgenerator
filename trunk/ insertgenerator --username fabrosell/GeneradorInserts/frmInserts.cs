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
        }

        /// <summary>
        /// This method updates DB Combobox and Status Bar Host and User Name
        /// </summary>
        private void UpdateForm()
        {            
            throw new Exception("This method has not been implemented.");
        }
    }
}