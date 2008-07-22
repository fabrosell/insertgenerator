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

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //Disable Controls
            cmbServerName.Enabled = false;
            cmbAuthentication.Enabled = false;
            cmbLogin.Enabled = false;
            txtPassword.Enabled = false;
            btnConnect.Enabled = false;
            btnCancel.Enabled = false;

            switch (cmbAuthentication.SelectedIndex)
            {
                case 0:
                    MessageBox.Show("This Feature has not been implemented yet", "Sorry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case 1:
                    //Create a Connection object
                    DBConnection = new Connection(txtPassword.Text.Trim());

                    //Replace the password by the foobar message, so it can't be seen with passwords revealers
                    txtPassword.Text = FoobarMessage;                    

                    //Complete the other paramethers
                    DBConnection.Authentication = AuthenticationMethods.SqlServer;
                    DBConnection.HostName = cmbServerName.SelectedItem.ToString().Trim();
                    DBConnection.UserName = cmbLogin.SelectedItem.ToString().Trim();
                    DBConnection.SavePassword = chkRememberPassword.Checked;

                    break;
                default:
                    MessageBox.Show("Unknown Authentication Method", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            }

            //If connecting is succesful, save the connection
            if (DBConnection.TestConnection())
            {
                DBConnection.SaveConnection();

                frmInserts InsertGeneratorForm = new frmInserts(DBConnection);

                InsertGeneratorForm.Show();

                this.Dispose();
            }
            else
            {
                //Show Error Message
                String ErrorMessage = "Can't connect to " + DBConnection.HostName + System.Environment.NewLine +
                                      "Additional Information:" + System.Environment.NewLine +
                                      DBConnection.ErrorMessage;

                MessageBox.Show(ErrorMessage, "Connect to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //Enable disabled Controls.                
                cmbServerName.Enabled = true;
                cmbAuthentication.Enabled = true;
                cmbLogin.Enabled = true;
                txtPassword.Enabled = true;
                txtPassword.Text = "";
                btnConnect.Enabled = true;
                btnCancel.Enabled = true;
            }

        }
    }  
}