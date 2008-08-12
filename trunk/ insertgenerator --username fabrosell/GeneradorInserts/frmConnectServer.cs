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

        //Handler of Load event
        private void frmConnectServer_Load(object sender, EventArgs e)
        {
            Load_AuthenticationMethods();

            Load_Logins();
        }

        //Handler of Connect button. 
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
        private void Load_Logins()
        {
            lConnections = Connection.GetConnections();

            cmbServerName.Items.Clear();
            cmbLogin.Items.Clear();

            lConnections = Connection.GetConnections();

            if (lConnections.Count != 0)
            {
                //First, all server names are loaded.
                foreach (Connection c in lConnections)
                {
                    cmbServerName.Items.Add(c.HostName);                        

                    //Show the last sucessful login
                    if (c.IsLastSucessfulLogin)
                        CurrentConnection = c;
                }

                if (CurrentConnection == null)
                    CurrentConnection = lConnections[0];

                Predicate<Connection> Connection_Find = delegate(Connection c) { return c.HostName == CurrentConnection.HostName; };

                List<Connection> MatchingServerNameConnections = lConnections.FindAll(Connection_Find);

                //Second, all logins are loaded.
                foreach (Connection c in MatchingServerNameConnections)
                    cmbLogin.Items.Add(c.UserName);

                //Last, the last successful connection is shown
                cmbServerName.SelectedIndex = cmbServerName.FindString(CurrentConnection.HostName);
                cmbLogin.SelectedIndex = cmbLogin.FindString(CurrentConnection.UserName);
                if (CurrentConnection.SavePassword)
                {
                    txtPassword.Text = FoobarMessage;
                    chkRememberPassword.Checked = true;
                }
                else
                {
                    txtPassword.Text = "";
                    chkRememberPassword.Checked = false;
                }

                cmbAuthentication.SelectedIndex = cmbAuthentication.FindString(CurrentConnection.Authentication.ToString());
            }
        }

        //Handler of Authentication Method change
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

        //Handler of Server change
        private void cmbServerName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }  
}