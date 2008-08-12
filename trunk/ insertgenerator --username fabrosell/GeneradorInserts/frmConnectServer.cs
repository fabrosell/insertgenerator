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
        List<Connection> lConnections = null;
        Connection CurrentConnection = null;

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

            Cargar_Logins();
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
                case 0: //Windows Authentication
                    //Only Server Name is required for Windows Authentication
                    DBConnection = new Connection();
                    DBConnection.HostName = cmbServerName.Text.Trim();

                    break;
                case 1: //SQL Authentication
                    //Create a Connection object
                    DBConnection = new Connection(txtPassword.Text.Trim());

                    //Replace the password by the foobar message, so it can't be seen with passwords revealers
                    txtPassword.Text = FoobarMessage;                    

                    //Complete the other paramethers
                    DBConnection.HostName = cmbServerName.Text.Trim();
                    DBConnection.UserName = cmbLogin.Text.Trim();
                    DBConnection.SavePassword = chkRememberPassword.Checked;

                    break;
                default:
                    MessageBox.Show("Unknown Authentication Method", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            }

            //If connecting is succesful, save the connection
            if (DBConnection.TestConnection())
            {
                this.Hide();

                DBConnection.SaveConnection(lConnections);                

                frmInserts InsertGeneratorForm = new frmInserts(DBConnection, this);

                InsertGeneratorForm.Show();
            }
            else
            {
                //Show Error Message
                String ErrorMessage = "Can't connect to " + DBConnection.HostName + System.Environment.NewLine +
                                      "Additional Information:" + System.Environment.NewLine +
                                      DBConnection.ErrorMessage;

                MessageBox.Show(ErrorMessage, "Connect to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);


                //Enable disabled controls by Authentication Method
                switch (cmbAuthentication.SelectedIndex)
                {
                    case 0: //Windows Authentication
                        cmbLogin.Enabled = false;
                        txtPassword.Enabled = false;
                        chkRememberPassword.Checked = false;
                        chkRememberPassword.Enabled = false;
                        break;
                    case 1: //SQL Server Authentication
                        cmbLogin.Enabled = true;
                        txtPassword.Enabled = true;
                        chkRememberPassword.Enabled = true;
                        break;
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Load all logins, set the last one which had a sucessful connection as the default.
        /// </summary>
        private void Cargar_Logins()
        {
            lConnections = Connection.GetConnections();

            foreach (Connection c in lConnections)
            {

            }
        }

        private void cmbAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbAuthentication.SelectedIndex)
            {
                case 0: //Windows Authentication
                    cmbLogin.Text = SystemInformation.UserDomainName + @"\" + SystemInformation.UserName;
                    cmbLogin.Enabled = false;
                    txtPassword.Text = "";
                    txtPassword.Enabled = false;
                    chkRememberPassword.Checked = false;
                    chkRememberPassword.Enabled = false;
                    break;
                case 1: //SQL Server Authentication
                    cmbLogin.Enabled = true;
                    txtPassword.Enabled = true;
                    chkRememberPassword.Enabled = true;

                    if (CurrentConnection != null)
                    {
                        cmbLogin.Text = CurrentConnection.UserName;
                        txtPassword.Text = FoobarMessage;
                        chkRememberPassword.Checked = CurrentConnection.SavePassword;
                    }
                    else
                    {
                        cmbLogin.Text = "";
                        txtPassword.Text = "";
                        chkRememberPassword.Checked = false;
                    }
                    break;
                default:
                    MessageBox.Show("Unknown Authentication Method", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Dispose();
                    break;
            }
        }
    }  
}