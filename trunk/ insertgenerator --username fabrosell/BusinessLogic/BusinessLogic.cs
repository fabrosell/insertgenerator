using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Suru.InsertGenerator.BusinessLogic
{
    public class Connection
    {
        //Defining and defaulting parameters
        private String _HostName = null;
        private String _UserName = null;
        private String _Password = null;
        private AuthenticationMethods _Authentication = AuthenticationMethods.Windows;
        private String _ErrorMessage = null;
        private List<String> _DataBases = null;
        private Boolean _SavePassword = false;
        private Boolean _IsLastSucessfulLogin = false;
        private String _Last_DataBase = DefaultDatabase;

        //This variable must be all times hided (neither read only access)
        private String ConnectionString;

        //Identifiers for connections string replacement
        private const String ServerPrefix = "[Server]";
        private const String DatabasePrefix = "[Catalog]";
        private const String UserPrefix = "[User]";
        private const String PasswordPrefix = "[Password]";

        //Identifier for Default Database
        private const String DefaultDatabase = "master";

        //Connections Strings. I currently know only two authentication methods.
        private const String SQL_Authenticacion_ConnectionString = "Data Source=" + ServerPrefix + ";Initial Catalog=" + DatabasePrefix + ";User Id=" + UserPrefix + ";Password=" + PasswordPrefix + ";";
        private const String Windows_Authenticacion_ConnectionString = "Server=" + ServerPrefix + ";Database=" + DatabasePrefix + ";Trusted_Connection=True;";        

        #region Attribute Encapsulation

        public String HostName
        {
            get { return _HostName; }
            set { _HostName = value; }
        }

        public String UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        public AuthenticationMethods Authentication
        {
            get { return _Authentication; }
        }

        public String ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }


        public List<String> DataBases
        {
            get { return _DataBases; }
            set { _DataBases = value; }
        }


        public Boolean SavePassword
        {
            get { return _SavePassword; }
            set { _SavePassword = value; }
        }

        public Boolean IsLastSucessfulLogin
        {
            get { return _IsLastSucessfulLogin; }
            set { _IsLastSucessfulLogin = value; }
        }

        public String Last_DataBase
        {
            get { return _Last_DataBase; }
            set { _Last_DataBase = value; }
        }

        #endregion

        /// <summary>
        /// Class constructor, for SQL Authentication Method.
        /// </summary>
        /// <param name="Password">Connection's password.</param>
        public Connection(String Password)
        {
            _Password = Password;
            _Authentication = AuthenticationMethods.SqlServer;
            DataBases = new List<String>();
        }

        /// <summary>
        /// Class constructor, for Windows Authentication Method
        /// </summary>
        public Connection()
        {
            _Authentication = AuthenticationMethods.Windows;
            DataBases = new List<String>();
        }

        /// <summary>
        /// Get the list of connections.
        /// </summary>
        /// <returns>List of Connections</returns>
        public static List<Connection> GetConnections()
        {
            //Get the list of connections and his logins
            //throw new Exception("This method has not been implemented.");
            return new List<Connection>();
        }

        /// <summary>
        /// Save the current conection to the XML
        /// </summary>
        public void SaveConnection()
        {
            //For one connection, several logins may exists.
            //throw new Exception("This method has not been implemented.");
        }

        /// <summary>
        /// Test if the connection is working or not.
        /// </summary>
        /// <returns>True if it can connect. False otherwise.</returns>
        public Boolean TestConnection()
        {
            //Test if connection parameters are valid...            
            //This is done trying to get Database List

            switch (_Authentication)
            {
                case AuthenticationMethods.Windows:
                    ConnectionString = Windows_Authenticacion_ConnectionString;
                    ConnectionString = ConnectionString.Replace(ServerPrefix, _HostName);
                    ConnectionString = ConnectionString.Replace(DatabasePrefix, DefaultDatabase);
                    break;
                case AuthenticationMethods.SqlServer:
                    ConnectionString = SQL_Authenticacion_ConnectionString;
                    ConnectionString = ConnectionString.Replace(ServerPrefix, _HostName);
                    ConnectionString = ConnectionString.Replace(DatabasePrefix, DefaultDatabase);
                    ConnectionString = ConnectionString.Replace(UserPrefix, _UserName);
                    ConnectionString = ConnectionString.Replace(PasswordPrefix, _Password);
                    break;
                default:
                    throw new Exception("Unknown authentication method.");
                    break;
            }

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();

                    SqlCommand sqlCommand = new SqlCommand("Select Name From SysDataBases Where Name != 'tempdb' Order By Name;");
                    sqlCommand.Connection = sqlConn;

                    SqlDataReader dr = sqlCommand.ExecuteReader();

                    //Loads the Database list into memory
                    while (dr.Read())
                        DataBases.Add(dr["Name"].ToString());

                    dr.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                //Connection Fails
                _ErrorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// List the Database tables for the current connection.
        /// </summary>
        /// <returns>List of tables.</returns>
        public List<String> ListDatabaseTables()
        {
            List<String> TableList = null;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();

                    SqlCommand sqlCommand = new SqlCommand("Use " + _Last_DataBase + "; Select Name From SysObjects Where Type = 'U' Order By Name;");
                    sqlCommand.Connection = sqlConn;

                    SqlDataReader dr = sqlCommand.ExecuteReader();

                    TableList = new List<String>();

                    //Loads the Database list into memory
                    while (dr.Read())
                        TableList.Add(dr["Name"].ToString());

                    dr.Close();
                }

            }
            catch (Exception ex)
            {
                //Connection Fails
                _ErrorMessage = ex.Message;
            }

            return TableList;
        }
    }

    public enum AuthenticationMethods { SqlServer, Windows };
}
