using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Xml;

namespace Suru.InsertGenerator.BusinessLogic
{
    public class Connection
    {
        #region Local Variables and Constants

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

        #endregion

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
            List<Connection> lConnections = new List<Connection>();

            //Get the list of connections and his logins
            try
            {
                StreamReader sr = new StreamReader(ConfigurationManager.AppSettings.Get("ConfigurationFile"));
                Connection c;

                /* Configuration File Structure
                 * <Connections>
                 *  <Connection>
                 *    <Host>HostName</Host>
                 *    <User>UserName</User>
                 *    <Pass>Password</Pass>
                 *    <Authentication></Authentication>
                 *    <DataBase>Database</DataBase>
                 *    <IsLastSuccessful>Boolean</IsLastSuccessful>
                 *    <PasswordSaved>Boolean</PasswordSaved>
                 *  </Connection>
                 * </Connections>
                 * 
                 * Note: Inside 'Connection' tags the data will be encrypted. */

                try
                {
                    XmlDocument xmlConnectionFile = new XmlDocument();
                    xmlConnectionFile.LoadXml(sr.ReadToEnd());
                    XmlNodeList xmlConnectionNodeList = xmlConnectionFile.DocumentElement.SelectNodes("//Connection");
                    XmlNodeList xmlTempNodes;

                    foreach (XmlNode xmlConnection in xmlConnectionNodeList)
                    {
                        c = new Connection();

                        xmlTempNodes = xmlConnection.SelectNodes("descendant::Host");
                        c.HostName = xmlTempNodes[0].InnerText;

                        xmlTempNodes = xmlConnection.SelectNodes("descendant::User");
                        c.UserName = xmlTempNodes[0].InnerText;

                        xmlTempNodes = xmlConnection.SelectNodes("descendant::Pass");
                        c._Password = xmlTempNodes[0].InnerText;

                        xmlTempNodes = xmlConnection.SelectNodes("descendant::Authentication");

                        //Authentication method by default.
                        c._Authentication = AuthenticationMethods.Windows;

                        if (xmlTempNodes[0].InnerText == AuthenticationMethods.SqlServer.ToString())
                            c._Authentication = AuthenticationMethods.SqlServer;

                        if (xmlTempNodes[0].InnerText == AuthenticationMethods.Windows.ToString())
                            c._Authentication = AuthenticationMethods.Windows;

                        xmlTempNodes = xmlConnection.SelectNodes("descendant::DataBase");
                        c.Last_DataBase = xmlTempNodes[0].InnerText;

                        xmlTempNodes = xmlConnection.SelectNodes("descendant::IsLastSuccessful");

                        if (bool.Parse(xmlTempNodes[0].InnerText))
                            c.IsLastSucessfulLogin = true;
                        else
                            c.IsLastSucessfulLogin = false;

                        xmlTempNodes = xmlConnection.SelectNodes("descendant::PasswordSaved");

                        if (bool.Parse(xmlTempNodes[0].InnerText))
                            c.SavePassword = true;
                        else
                            c.SavePassword = false;
                    }
                }
                catch (Exception ex)
                {
                    throw new XmlException("XML data not properly formed");
                }
                finally
                {
                    sr.Close();
                }
            }
            catch (XmlException xmlex)
            {
                throw xmlex;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Can't load configuration file " + ConfigurationManager.AppSettings.Get("ConfigurationFile"));
            }
            
            return lConnections;
        }

        /// <summary>
        /// Save the current conection to the XML
        /// </summary>
        public void SaveConnection(List<Connection> lConnections)
        {
            /* Configuration File Structure
             * <Connections>
             *  <Connection>
             *    <Host>HostName</Host>
             *    <User>UserName</User>
             *    <Pass>Password</Pass>
             *    <Authentication></Authentication>
             *    <DataBase>Database</DataBase>
             *    <IsLastSuccessful>Boolean</IsLastSuccessful>
             *    <PasswordSaved>Boolean</PasswordSaved>
             *  </Connection>
             * </Connections>
             * 
             * Note: Inside 'Connection' tags the data will be encrypted. */

            try
            {
                StreamReader sr = new StreamReader(ConfigurationManager.AppSettings.Get("ConfigurationFile"));
                String sXmlContent = null;

                try
                {                                        
                    sXmlContent = sr.ReadToEnd();
                    sr.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sr.Close();   
                }

                try
                {
                    Connection OverwritingConnection = null;

                    //For one connection, several logins may exists.            
                    //Criteria for overwriting connections:
                    //  -- Hostname must be the same
                    //  -- Username must be the same
                    //  -- Authentication Method must be the same

                    Predicate<Connection> Find_Connection = delegate(Connection cCon)
                    {
                        return cCon._Authentication == _Authentication &&
                               cCon.HostName == _HostName &&
                               cCon.UserName == _UserName;
                    };

                    OverwritingConnection = lConnections.Find(Find_Connection);

                    XmlDocument xmlConnectionFile = new XmlDocument();
                    xmlConnectionFile.LoadXml(sXmlContent);
                    XmlNodeList xmlConnectionNodeList = xmlConnectionFile.DocumentElement.SelectNodes("//Connection");
                    XmlNodeList xmlTempNodes;

                    //The connection exists and must be overwritten
                    if (OverwritingConnection != null)
                    {
                        foreach (XmlNode xmlConnection in xmlConnectionNodeList)
                        {
                        }
                    }
                    else
                    {
                        String sUserName = "";
                        String sPassword = "";

                        if (_UserName != null)
                            sUserName = _UserName;

                        if (_Password != null)
                            sPassword = _Password;

                        //The connection does not exist and must be appended
                        String sConnection = "<Connection>" + 
                                                "<Host>" + _HostName + "</Host>" +
                                                "<User>" + sUserName + "</User>" +
                                                "<Pass>" + sPassword + "</Pass>" +
                                                "<Authentication>" + _Authentication.ToString() + "</Authentication>" +
                                                "<DataBase>" + _Last_DataBase + "</DataBase>" +
                                                "<IsLastSuccessful>True</IsLastSuccessful>" +
                                                "<PasswordSaved>" + _SavePassword.ToString()  + "<PasswordSaved>" +
                                             "</Connection>";

                    //I have to write all nodes to disk...
                        
                        
                    }

                    //Writes the file
                    //xmlConnectionFile.Save(ConfigurationManager.AppSettings.Get("ConfigurationFile"));
                }
                catch (Exception ex)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Can't load or write configuration file " + ConfigurationManager.AppSettings.Get("ConfigurationFile")); throw new ApplicationException("Can't load configuration file " + ConfigurationManager.AppSettings.Get("ConfigurationFile"));
            }
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
