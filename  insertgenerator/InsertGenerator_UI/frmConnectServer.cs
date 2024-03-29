using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Suru.Common.EncryptionLibrary;
using Suru.InsertGenerator.BusinessLogic;
using System.Security;

namespace Suru.InsertGenerator.GeneradorUI
{
    public partial class frmConnectServer : Form
    {
        #region Atributtes

        public String FoobarMessage = "[reveal-proof]";                                       
        private Connection DBConnection;
        List<Connection> lConnections = null;        
        Connection CurrentConnection = null;

        frmInserts _Parent = null;

        #endregion

        #region Class Methods

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

        /// <summary>
        /// Load all logins, set the last one which had a sucessful connection as the default.
        /// </summary>
        private void Load_Logins()
        {
            cmbServerName.Items.Clear();
            cmbLogin.Items.Clear();

            lConnections = Connection.GetConnections();

            if (lConnections.Count != 0)
            {
                //First, all server names are loaded.
                foreach (Connection c in lConnections)
                {
                    if (cmbServerName.FindString(c.HostName) == -1)
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
                    //Omit windows authentication Logins
                    if (c.UserName != "")
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

                SelectAuthenticationType(CurrentConnection.Authentication);
            }
        }

        /// <summary>
        /// Maps the current authetication method that is present in authenticacion's comobox.
        /// </summary>
        /// <returns>Authentication currently selected</returns>
        public AuthenticationMethods MapAuthenticationType()
        {
            if (cmbAuthentication.SelectedIndex == 0)
                return AuthenticationMethods.Windows;
            else
                return AuthenticationMethods.SqlServer;
        }

        /// <summary>
        /// Sets the authentication method with the value passed on
        /// </summary>
        /// <param name="am">Authentication method to be selected in combobox.</param>
        public void SelectAuthenticationType(AuthenticationMethods am)
        {
            switch (am)
            {
                case AuthenticationMethods.SqlServer:
                    cmbAuthentication.SelectedIndex = 1;
                    break;
                case AuthenticationMethods.Windows:
                    cmbAuthentication.SelectedIndex = 0;
                    break;
                default:
                    cmbAuthentication.SelectedIndex = 0;
                    break;
            }
        }

        #endregion

        #region Events & Constructors

        public frmConnectServer()
        {
            InitializeComponent();
        }

        public frmConnectServer(frmInserts Parent)
        {
            InitializeComponent();
            _Parent = Parent;
        }

        //Cancel button event handler
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
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

            if (CurrentConnection != null)
            {
                //If selected connection differs from current connection, set it null
                if (CurrentConnection.HostName != cmbServerName.Text.Trim() ||
                    CurrentConnection.UserName != cmbLogin.Text.Trim() ||
                    CurrentConnection.Authentication != MapAuthenticationType())
                {
                    CurrentConnection = null;
                }
                else
                    DBConnection = CurrentConnection;
            }

            switch (MapAuthenticationType())
            {
                case AuthenticationMethods.Windows: //Windows Authentication                    
                    if (CurrentConnection == null)
                    {
                        //Only Server Name is required for Windows Authentication
                        DBConnection = new Connection();
                        DBConnection.HostName = cmbServerName.Text.Trim();
                    }

                    break;
                case AuthenticationMethods.SqlServer: //SQL Authentication
                    if (CurrentConnection == null)
                    {
                        SecureString sPassword = new SecureString();
                        foreach (Char c in txtPassword.Text.Trim())
                            sPassword.AppendChar(c);

                        //Replace the password by the foobar message, so it can't be seen with passwords revealers
                        txtPassword.Text = FoobarMessage;

                        //Create a Connection object
                        DBConnection = new Connection(sPassword);

                        //Complete the other paramethers
                        DBConnection.HostName = cmbServerName.Text.Trim();
                        DBConnection.UserName = cmbLogin.Text.Trim();
                        DBConnection.SavePassword = chkRememberPassword.Checked;
                    }

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

                if (_Parent == null)
                {
                    frmInserts InsertGeneratorForm = new frmInserts(DBConnection, this);

                    InsertGeneratorForm.Show();
                }
                else
                {
                    //It's being called from 'change connection', not first time.
                    _Parent.DBConnection = DBConnection;
                    this.Dispose();
                }
            }
            else
            {
                //Show Error Message
                String ErrorMessage = "Can't connect to " + DBConnection.HostName + System.Environment.NewLine +
                                      "Additional Information:" + System.Environment.NewLine +
                                      DBConnection.ErrorMessage;

                MessageBox.Show(ErrorMessage, "Connect to Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Enable disabled controls by Authentication Method
            btnConnect.Enabled = true;
            btnCancel.Enabled = true;
            cmbServerName.Enabled = true;
            cmbAuthentication.Enabled = true;

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
                                             
        //Handler of Authentication Method change
        private void cmbAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (MapAuthenticationType())
            {
                case AuthenticationMethods.Windows: //Windows Authentication
                    cmbLogin.Text = SystemInformation.UserDomainName + @"\" + SystemInformation.UserName;
                    cmbLogin.Enabled = false;
                    txtPassword.Text = "";
                    txtPassword.Enabled = false;
                    chkRememberPassword.Checked = false;
                    chkRememberPassword.Enabled = false;
                    break;
                case AuthenticationMethods.SqlServer: //SQL Server Authentication
                    cmbLogin.Enabled = true;
                    txtPassword.Enabled = true;
                    chkRememberPassword.Enabled = true;

                    //Connection has changed. I need to update CurrentConnection.
                    Predicate<Connection> Find_Connection = delegate(Connection c) { return c.HostName == cmbServerName.Text.Trim() && c.Authentication == MapAuthenticationType(); };
                    CurrentConnection = lConnections.Find(Find_Connection);

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
            //When server name changes, logins must be updated
            List<Connection> lServer_Connections = null;

            Predicate<Connection> Find_Connections = delegate(Connection c) { return c.HostName == cmbServerName.Text.Trim(); };

            lServer_Connections = lConnections.FindAll(Find_Connections);

            cmbLogin.Items.Clear();

            CurrentConnection = null;

            foreach (Connection c in lServer_Connections)
            {
                cmbLogin.Items.Add(c.UserName);

                if (c.IsLastSucessfulLogin)
                    CurrentConnection = c;
            }

            if (CurrentConnection == null && lServer_Connections.Count != 0)
                CurrentConnection = lServer_Connections[0];

            SelectAuthenticationType(CurrentConnection.Authentication);
        }

        //Handler of command keys pressed
        private void frmConnectServer_KeyDown(object sender, KeyEventArgs e)
        {
            //F12 code: Resets connection file & Encryption Keys
            if (e.KeyCode == Keys.F12)
            {
                if (MessageBox.Show("Are you sure you want to delete stored connections and reset encryption keys?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {                    
                    Connection.ResetStoredConnectionsFile();

                    Encryption.ResetCryptoKeys();

                    MessageBox.Show("Stored connections and Crypto keys reseted succesfully.", "Reset OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Load_Logins();
                }
            }
        }

        #endregion
    }  
}