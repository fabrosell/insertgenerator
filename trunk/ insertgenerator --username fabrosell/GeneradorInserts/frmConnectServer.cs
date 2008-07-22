using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Suru.InsertGenerator.BusinessLogic;

namespace GeneradorInserts
{
    public partial class frmConnectServer : Form
    {
        public String FoobarMessage = "[I won't show it!]";
        private Connection DBConnection;

        public frmConnectServer()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        /// <summary>
        /// This method loads the Authentications' combobox.
        /// </summary>
        private void Load_AuthenticationMethods()
        {
            cmbAuthentication.Items.Clear();            

            cmbAuthentication.Items.Add("Windows Authentication");
            cmbAuthentication.Items.Add("SQL Server Authentication");

            if (DBConnection != null)
            {
                switch (DBConnection.Authentication)
                {
                    case AuthenticationMethods.Windows:
                        cmbAuthentication.SelectedIndex = 0;
                        break;
                    case AuthenticationMethods.SqlServer:
                        cmbAuthentication.SelectedIndex = 1;
                        break;
                }
            }
            else
                //Windows authentication is the default authentication
                cmbAuthentication.SelectedIndex = 0;
        }

        private void frmConnectServer_Load(object sender, EventArgs e)
        {
            Load_AuthenticationMethods();
        }
    }  
}