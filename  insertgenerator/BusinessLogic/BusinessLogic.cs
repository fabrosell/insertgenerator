using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Xml;
using System.Security;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Suru.Common.EncryptionLibrary;

namespace Suru.InsertGenerator.BusinessLogic
{
    /// <summary>
    /// Class modeling a database connection
    /// </summary>
    public class Connection
    {
        #region Local Variables and Constants

        //Defining and defaulting parameters
        private String _HostName = null;
        private String _UserName = null;
        private SecureString _Password = null;
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
        public Connection(SecureString Password)
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
                 */
                try
                {
                    XmlDocument xmlConnectionFile = new XmlDocument();
                    String FileData = null;

                    try
                    {
                        FileData = Encryption.SymetricDecrypt(sr.ReadToEnd());
                        sr.Close();
                    }
                    catch
                    {
                        //Exception is thrown. May happen two things:
                        // 1º Data is unproperly formed due to an application error.
                        // 2º Data can't be read because was originated with differents Keys
                        // 3º Encryption keys differ because is first run.
                        //In all cases, file will be reseted.
                        try
                        {
                            sr.Close();
                            ResetStoredConnectionsFile();
                        }
                        catch
                        {
                            Encryption.ResetCryptoKeys();
                            ResetStoredConnectionsFile();
                        }
                        finally
                        {
                            //This is the data in file after resetion
                            FileData = "<Connections></Connections>";
                        }
                    }

                    xmlConnectionFile.LoadXml(FileData);
                    XmlNodeList xmlConnectionNodeList = xmlConnectionFile.DocumentElement.SelectNodes("//Connection");
                    XmlNodeList xmlTempNodes;

                    foreach (XmlNode xmlConnection in xmlConnectionNodeList)
                    {
                        c = new Connection();

                        xmlTempNodes = xmlConnection.SelectNodes("descendant::Host");
                        c.HostName = xmlTempNodes[0].InnerText;

                        xmlTempNodes = xmlConnection.SelectNodes("descendant::User");
                        c.UserName = xmlTempNodes[0].InnerText;

                        //Processing password
                        xmlTempNodes = xmlConnection.SelectNodes("descendant::Pass");

                        if (xmlTempNodes[0].InnerText.Length != 0)
                            c._Password = new SecureString();

                        foreach (Char cPiece in xmlTempNodes[0].InnerText)
                            c._Password.AppendChar(cPiece);

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

                        lConnections.Add(c);
                    }
                }
                catch (Exception ex)
                {
                    sr.Close();
                    throw new XmlException("XML data not properly formed");
                }
            }
            catch (XmlException xmlex)
            {
                throw xmlex;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Can't load configuration file " + ConfigurationManager.AppSettings.Get("ConfigurationFile") + " --> " + ex.Message);
            }

            return lConnections;
        }

        /// <summary>
        /// Resets the Stored connection file.
        /// </summary>
        public static void ResetStoredConnectionsFile()
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(ConfigurationManager.AppSettings.Get("ConfigurationFile"));

                StringBuilder sb = new StringBuilder();

                sb.Append("<Connections>" + "</Connections>");

                sw.Write(Encryption.SymetricEncrypt(sb.ToString()));

                sw.Close();
            }
            catch (Exception ex)
            {
                if (sw != null)
                    sw.Close();

                throw ex;
            }
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
                StreamWriter sw = null;
                String sXmlContent = null;

                try
                {
                    sXmlContent = Encryption.SymetricDecrypt(sr.ReadToEnd());
                    sr.Close();
                }
                catch (Exception ex)
                {
                    sr.Close();
                    throw ex;
                }

                try
                {
                    //For one connection, several logins may exists.            
                    //Criteria for overwriting connections:
                    //  -- Hostname must be the same
                    //  -- Username must be the same
                    //  -- Authentication Method must be the same

                    XmlDocument xmlConnectionFile = new XmlDocument();
                    xmlConnectionFile.LoadXml(sXmlContent);
                    XmlNodeList xmlConnectionNodeList = xmlConnectionFile.DocumentElement.SelectNodes("//Connection");
                    XmlNode tempNode;
                    Boolean NodeIsFound = false;

                    StringBuilder sConnectionNodes = new StringBuilder();
                    sConnectionNodes.Append("<Connections>" + Environment.NewLine);

                    String sUserName = "";
                    String sPassword = "";

                    if (_UserName != null)
                        sUserName = _UserName;

                    if (_Password != null)
                    {
                        IntPtr passwordBSTR = Marshal.SecureStringToBSTR(_Password);
                        sPassword = Marshal.PtrToStringBSTR(passwordBSTR);
                    }

                    //It's always easier to rewrite the connection.
                    sConnectionNodes.Append("<Connection>" +
                                                "<Host>" + _HostName + "</Host>" +
                                                "<User>" + sUserName + "</User>" +
                                                "<Pass>" + sPassword + "</Pass>" +
                                                "<Authentication>" + _Authentication.ToString() + "</Authentication>" +
                                                "<DataBase>" + _Last_DataBase + "</DataBase>" +
                                                "<IsLastSuccessful>True</IsLastSuccessful>" +
                                                "<PasswordSaved>" + _SavePassword.ToString() + "</PasswordSaved>"
                                                 +
                                              "</Connection>" + Environment.NewLine);

                    foreach (XmlNode xmlSavedConnection in xmlConnectionNodeList)
                    {
                        NodeIsFound = false;

                        tempNode = xmlSavedConnection.SelectNodes("descendant::Host")[0];

                        if (tempNode.InnerText == _HostName)
                        {
                            tempNode = xmlSavedConnection.SelectNodes("descendant::User")[0];

                            if (tempNode.InnerText == _UserName)
                            {
                                tempNode = xmlSavedConnection.SelectNodes("descendant::Authentication")[0];

                                if (tempNode.InnerText == _Authentication.ToString())
                                    NodeIsFound = true;
                            }
                        }

                        tempNode = xmlSavedConnection.SelectNodes("descendant::IsLastSuccessful")[0];
                        tempNode.InnerText = "False";

                        //NodeIsFound = True --> I found the Node
                        if (!NodeIsFound)
                            sConnectionNodes.Append("<Connection>" + xmlSavedConnection.InnerXml.ToString() + "</Connection>" + Environment.NewLine);
                    }

                    //The Node List is complete. It must be written to the connection file.
                    sConnectionNodes.Append("</Connections>");

                    //Writes the encrypted file
                    //Rewrites the file
                    sw = new StreamWriter(ConfigurationManager.AppSettings.Get("ConfigurationFile"));

                    sw.Write(Encryption.SymetricEncrypt(sConnectionNodes.ToString()));
                    sw.Close();
                }
                catch (Exception ex)
                {
                    if (sw != null)
                        sw.Close();

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Can't load or write configuration file " + ConfigurationManager.AppSettings.Get("ConfigurationFile") + " --> " + ex.Message);
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

                    String _InsecurePassword = null;
                    if (_Password != null)
                    {
                        IntPtr passwordBSTR = Marshal.SecureStringToBSTR(_Password);
                        _InsecurePassword = Marshal.PtrToStringBSTR(passwordBSTR);
                    }
                    else
                        _InsecurePassword = "";

                    ConnectionString = ConnectionString.Replace(PasswordPrefix, _InsecurePassword);
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

        /// <summary>
        /// Gets the table ID for the specified Table.
        /// </summary>
        /// <param name="TableName">A Valid Table</param>
        /// <returns>ID table (sysobjects data)</returns>
        public Int64 GetTableID(String TableName)
        {
            Int64 TableID = -1;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();

                    SqlCommand sqlCommand = new SqlCommand("Use " + _Last_DataBase + "; Select IsNull(ID, -1) As ID From SysObjects Where Name = '" + TableName + "';");
                    sqlCommand.Connection = sqlConn;

                    SqlDataReader dr = sqlCommand.ExecuteReader();

                    //Read the Table ID.
                    if (dr.Read())
                        TableID = Int64.Parse(dr["ID"].ToString());

                    dr.Close();
                }

            }
            catch (Exception ex)
            {
                //Connection Fails
                _ErrorMessage = ex.Message;
            }

            return TableID;
        }

        /// <summary>
        /// Returns the table's columns.
        /// </summary>
        /// <param name="ID">Table ID.</param>
        /// <returns>List of Column objects</returns>
        public Table GetTableColumns(Int64 ID)
        {
            Table t = new Table();

            t.HasIdentityColumn = false;
            t.Columns = new List<Columns>();

            Columns Column;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();

                    SqlCommand sqlCommand = new SqlCommand("Use " + _Last_DataBase + "; Select C.name as ColumnName, T.name as TypeName, C.iscomputed As IsComputed, C.status As Status From SysColumns C Left Join SysTypes T On C.xtype = T.xusertype Where id = " + ID + " Order By colorder;");

                    sqlCommand.Connection = sqlConn;

                    SqlDataReader dr = sqlCommand.ExecuteReader();
                    Int32 Status;

                    //Process each column list
                    while (dr.Read())
                    {
                        Column = new Columns();

                        Column.Name = dr["ColumnName"].ToString();
                        Column.Type = dr["TypeName"].ToString();
                        if (Int32.Parse(dr["IsComputed"].ToString()) == 0)
                            Column.IsCalculated = false;
                        else
                            Column.IsCalculated = true;

                        Status = Int32.Parse(dr["Status"].ToString());

                        //Timestamp columns are omitted
                        if (Column.Type.ToLower() == "timestamp")
                            Column.OmitColumn = true;

                        //Information taken out from http://msdn.microsoft.com/en-us/library/aa260398(SQL.80).aspx

                        //Bitmap status: 128 --> Column is identity type
                        if (Status >= 128)
                        {
                            Column.IdentityColumn = true;
                            t.HasIdentityColumn = true;
                            Status -= 128;
                        }

                        //Bitmap status: 64 --> Column is output parameter.
                        if (Status >= 64)
                        {
                            Column.OutputParameter = true;
                            Status -= 64;
                        }

                        //Bitmap status: 16 --> Column has Ansi Padding on.
                        if (Status >= 16)
                        {
                            Column.AnsiPaddingOn = true;
                            Status -= 16;
                        }

                        //Bitmap status: 8 --> Column allow nulls.
                        if (Status >= 8)
                            Column.AllowNulls = true;

                        t.Columns.Add(Column);
                    }

                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                //Connection Fails
                _ErrorMessage = ex.Message;
            }

            return t;
        }

        /// <summary>
        /// Get table's data.
        /// </summary>
        /// <param name="lTableColumns">Table's columns.</param>
        /// <param name="SelectStatement">Table's select statement.</param>
        /// <returns>The same column list passed as parameter, with data loaded on it.</returns>
        public List<Columns> GetTableData(List<Columns> lTableColumns, String SelectStatement)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();

                    SqlCommand sqlCommand = new SqlCommand("Use " + _Last_DataBase + ";" + SelectStatement);

                    sqlCommand.Connection = sqlConn;

                    SqlDataReader dr = sqlCommand.ExecuteReader();

                    Columns c;

                    //Process each column list
                    while (dr.Read())
                    {
                        for (Int32 i = 0; i < lTableColumns.Count; i++)
                        {
                            c = lTableColumns[i];

                            if (!c.OmitColumn)
                            {
                                if (c.Data == null)
                                    c.Data = new List<String>();

                                if (dr[c.Name] == DBNull.Value)
                                    c.Data.Add(null);
                                else
                                    c.Data.Add(dr[c.Name].ToString().Replace("'", "''"));
                            }
                        }
                    }

                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                //Connection Fails
                _ErrorMessage = ex.Message;
                throw ex;
            }

            return lTableColumns;
        }

        /// <summary>
        /// Get a list of tables in which table name parameter depends on.
        /// </summary>
        /// <param name="TableName">Table to get dependancies.</param>
        /// <returns>List of tables in which the parameter table depends on.</returns>
        public List<String> ListTableDependancy(String TableName)
        {
            List<String> TableList = null;

            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    sqlConn.Open();

                    SqlCommand sqlCommand = new SqlCommand("Use " + _Last_DataBase + "; Select O2.name As ReferredTable From sysobjects O Inner Join sysforeignkeys FK On FK.fkeyid = O.id Inner Join sysobjects O2 On Fk.rkeyid = O2.id Where O.name = '" + TableName + "' Order By O2.Name;");
                    sqlCommand.Connection = sqlConn;

                    SqlDataReader dr = sqlCommand.ExecuteReader();

                    TableList = new List<String>();

                    //Loads the referred table list into memory
                    while (dr.Read())
                        TableList.Add(dr["ReferredTable"].ToString());

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

    /// <summary>
    /// Struct with Generation Options
    /// </summary>
    public struct GenerationOptions
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _TransacionalScript;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _IncludeTableScripts;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _DerivedTableScript;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _CorrelatedDataTablesScript;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IdentityGenerationOptions _IdentityOptions;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Nullable<Int32> _TopRows;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Int32 _LinesPerBlock;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Encoding _FileEncoding;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _SQL2000Compatible;

        #region Attribute Encapsulation

        public Boolean TransacionalScript
        {
            get { return _TransacionalScript; }
            set { _TransacionalScript = value; }
        }

        public Boolean IncludeTableScripts
        {
            get { return _IncludeTableScripts; }
            set { _IncludeTableScripts = value; }
        }

        public Boolean DerivedTableScript
        {
            get { return _DerivedTableScript; }
            set { _DerivedTableScript = value; }
        }

        public Boolean CorrelatedDataTablesScript
        {
            get { return _CorrelatedDataTablesScript; }
            set { _CorrelatedDataTablesScript = value; }
        }

        public IdentityGenerationOptions IdentityOptions
        {
            get { return _IdentityOptions; }
            set { _IdentityOptions = value; }
        }

        public Nullable<Int32> TopRows
        {
            get { return _TopRows; }
            set { _TopRows = value; }
        }

        public Int32 LinesPerBlock
        {
            get { return _LinesPerBlock; }
            set { _LinesPerBlock = value; }
        }

        public Encoding FileEncoding
        {
            get { return _FileEncoding; }
            set { _FileEncoding = value; }
        }

        public Boolean SQL2000Compatible
        {
            get { return _SQL2000Compatible; }
            set { _SQL2000Compatible = value; }
        }

        #endregion
    }

    public enum AuthenticationMethods { SqlServer, Windows };

    public enum IdentityGenerationOptions { IdentityInsert, InsertionDependant, OmitIdentityColumns };

    public enum DataTypes { Integers, Decimals, Binarys, Strings, SpecificTypes, Ignored, NotIncluded, NotSupported, Dates };

    public enum ScriptTypes { TableScript, InsertScript };

    /// <summary>
    /// This class will hold all Columns' data relevant to generation.
    /// </summary>
    public class Columns
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private String _Name;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private String _Type;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _IsCalculated;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<String> _Data;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _AllowNulls;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _AnsiPaddingOn;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _OutputParameter;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _IdentityColumn;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _OmitColumn;

        #region Attribute Encapsulation

        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public String Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public Boolean IsCalculated
        {
            get { return _IsCalculated; }
            set { _IsCalculated = value; }
        }

        public List<String> Data
        {
            get { return _Data; }
            set { _Data = value; }
        }

        public Boolean AllowNulls
        {
            get { return _AllowNulls; }
            set { _AllowNulls = value; }
        }

        public Boolean AnsiPaddingOn
        {
            get { return _AnsiPaddingOn; }
            set { _AnsiPaddingOn = value; }
        }

        public Boolean OutputParameter
        {
            get { return _OutputParameter; }
            set { _OutputParameter = value; }
        }

        public Boolean IdentityColumn
        {
            get { return _IdentityColumn; }
            set { _IdentityColumn = value; }
        }

        public Boolean OmitColumn
        {
            get { return _OmitColumn; }
            set { _OmitColumn = value; }
        }

        #endregion

        public Columns()
        {
            _AllowNulls = false;
            _AnsiPaddingOn = false;
            _OutputParameter = false;
            _IdentityColumn = false;
            _OmitColumn = false;
        }
    }

    /// <summary>
    /// Represents a Table. 
    /// </summary>
    public class Table
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<Columns> _Columns;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Boolean _HasIdentityColumn;

        #region Attribute Encapsulation

        public List<Columns> Columns
        {
            get { return _Columns; }
            set { _Columns = value; }
        }

        public Boolean HasIdentityColumn
        {
            get { return _HasIdentityColumn; }
            set { _HasIdentityColumn = value; }
        }

        #endregion
    }

    /// <summary>
    /// Struct to represent Dependancy list. This will be useful to order tables by dependency.
    /// </summary>
    class DependancyList
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private String _TableName;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private List<String> _TableDependancy;

        #region Attribute encapsulation

        public String TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }

        public List<String> TableDependancy
        {
            get { return _TableDependancy; }
            set { _TableDependancy = value; }
        }

        #endregion
    }

    /// <summary>
    /// This class generates the Inserts' Scripts.
    /// </summary>
    public class SQLGeneration
    {
        private Connection _DBConnection;
        private GenerationOptions _GenOpts;
        private String _OutputPath;
        private Dictionary<String, DataTypes> dDataTypes = null;
        private Int32 _Script_Number = 1;

        //Date format: yyyy-mm-dd hh:mi:ss.mmm(24h)
        private const Int32 DateFormatStyle = 121;

        /// <summary>
        /// Class' constructor.
        /// </summary>
        /// <param name="DBConn">Database connection to use.</param>
        /// <param name="GenOpts">Insert Generation Options</param>
        public SQLGeneration(Connection DBConn, GenerationOptions GenOpts, String OutputPath)
        {
            _DBConnection = DBConn;
            _GenOpts = GenOpts;
            _OutputPath = OutputPath;

            //Load the Data types
            dDataTypes = new Dictionary<String, DataTypes>();

            dDataTypes.Add("bigint", DataTypes.Integers);
            dDataTypes.Add("int", DataTypes.Integers);
            dDataTypes.Add("smallint", DataTypes.Integers);
            dDataTypes.Add("tinyint", DataTypes.Integers);

            dDataTypes.Add("binary", DataTypes.Binarys);
            dDataTypes.Add("uniqueidentifier", DataTypes.Binarys);
            dDataTypes.Add("varbinary", DataTypes.Binarys);
            dDataTypes.Add("image", DataTypes.Binarys);

            dDataTypes.Add("char", DataTypes.Strings);
            dDataTypes.Add("nchar", DataTypes.Strings);
            dDataTypes.Add("ntext", DataTypes.Strings);
            dDataTypes.Add("nvarchar", DataTypes.Strings);
            dDataTypes.Add("text", DataTypes.Strings);
            dDataTypes.Add("varchar", DataTypes.Strings);

            dDataTypes.Add("xml", DataTypes.SpecificTypes);
            dDataTypes.Add("bit", DataTypes.SpecificTypes);

            dDataTypes.Add("datetime", DataTypes.Dates);
            dDataTypes.Add("smalldatetime", DataTypes.Dates);

            dDataTypes.Add("decimal", DataTypes.Decimals);
            dDataTypes.Add("float", DataTypes.Decimals);
            dDataTypes.Add("money", DataTypes.Decimals);
            dDataTypes.Add("numeric", DataTypes.Decimals);
            dDataTypes.Add("real", DataTypes.Decimals);
            dDataTypes.Add("smallmoney", DataTypes.Decimals);

            dDataTypes.Add("timestamp", DataTypes.NotIncluded);

            dDataTypes.Add("sql_variant", DataTypes.NotSupported);

            dDataTypes.Add("sysname", DataTypes.Ignored);
        }

        /// <summary>
        /// Generates the Inserts' Script.
        /// </summary>
        /// <param name="lTables">Tables to Generate Inserts.</param>
        public Boolean GenerateInserts(List<String> lTables)
        {
            #region File Generation (full size)

            Int64 TableID;

            //This list will hold all tables already generated.
            List<String> GeneratedTables = new List<String>();

            #region Type aclaration info

            //Integers --> These types don't need any special consideration on insert clause.
            //Decimals --> Care must be taken when generating insert (remember decimal dot).
            //Binarys  --> Binary types. They're handled like integers, without special considerations.
            //Strings  --> These types must have "'" enclosing his values.
            //SpecificTypes --> Types which must have a 'Convert' clause and values must be enclosed with "'"
            //DateTimes     --> Care must be taken when converting. Problems were reported...
            //Ignored       --> These types are not included on inserts.
            //NotIncluded   --> These types are not included on inserts, because it's impossible to insert them.
            //NotSupported  --> These types are not supported yet. An exception must be thrown.

            #endregion

            StringBuilder sb;
            String Header;
            Table t;
            Boolean bEndInsertionDependant = false, bIncludeSeparator = true;
            Int32 MaxData, LinesPerBlock = 0;

            StreamWriter sw_insert = null;
            FileStream fs_insert = null;
            Byte[] Data;

            String TempFileName = Path.GetTempFileName();

            if (!_GenOpts.SQL2000Compatible)
            {
                //sw_insert = new StreamWriter(Path.Combine(_OutputPath, GenerateFileName(ScriptTypes.InsertScript)), false, _GenOpts.FileEncoding);
                sw_insert = new StreamWriter(TempFileName, false, _GenOpts.FileEncoding);
                sw_insert.AutoFlush = true;

                sw_insert.WriteLine("-- Generated by Suru Insert Generator - " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                sw_insert.WriteLine();
                sw_insert.WriteLine("-- Please test this script before delivering it!");
                sw_insert.WriteLine();
            }
            else
            {
                //fs_insert = new FileStream(Path.Combine(_OutputPath, GenerateFileName(ScriptTypes.InsertScript)), FileMode.OpenOrCreate);
                fs_insert = new FileStream(TempFileName, FileMode.OpenOrCreate);

                Data = SQL2000Encoding("-- Generated by Suru Insert Generator - " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + Environment.NewLine + Environment.NewLine);
                fs_insert.Write(Data, 0, Data.Length);

                Data = SQL2000Encoding("-- Please test this script before delivering it!" + Environment.NewLine + Environment.NewLine);
                fs_insert.Write(Data, 0, Data.Length);
            }


            #region Beggining of transactional script (if apply)

            if (_GenOpts.TransacionalScript)
            {
                if (!_GenOpts.SQL2000Compatible)
                {
                    sw_insert.WriteLine("Declare @TransactionName VarChar(20)");
                    sw_insert.WriteLine("Set @TransactionName = 'SuruInsert'");
                    sw_insert.WriteLine("Begin Tran @TransactionName");
                    sw_insert.WriteLine();
                }
                else
                {
                    Data = SQL2000Encoding("Declare @TransactionName VarChar(20)" + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                    Data = SQL2000Encoding("Set @TransactionName = 'SuruInsert'" + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                    Data = SQL2000Encoding("Begin Tran @TransactionName" + Environment.NewLine + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                }
            }

            #endregion

            if (_GenOpts.LinesPerBlock == 0)
                _GenOpts.LinesPerBlock = Int32.Parse(ConfigurationManager.AppSettings.Get("LinesPerBlock"));

            //Heaviest method to order tables. Execution with 705 tables took 0.48 seconds average.
            Boolean GraphHasCycles = false;

            lTables = SortTablesByDependancy(lTables, _DBConnection, ref GraphHasCycles);

            if (GraphHasCycles)
            {
                if (!_GenOpts.SQL2000Compatible)
                {
                    sw_insert.WriteLine("-- Warning: data tables has cycles between them. Those tables are not in order.");
                    sw_insert.WriteLine();
                }
                else
                {
                    Data = SQL2000Encoding("-- Warning: data tables has cycles between them. Those tables are not in order." + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                }
            }

            foreach (String TableName in lTables)
            {
                TableID = _DBConnection.GetTableID(TableName);

                t = _DBConnection.GetTableColumns(TableID);

                #region Check for Identity Columns

                if (t.HasIdentityColumn)
                {
                    switch (_GenOpts.IdentityOptions)
                    {
                        case IdentityGenerationOptions.IdentityInsert:
                            //Set Identity Insert Table On / Off
                            if (!_GenOpts.SQL2000Compatible)
                            {
                                sw_insert.WriteLine();
                                sw_insert.WriteLine("Set Identity_Insert " + TableName + " On;");
                                sw_insert.WriteLine();
                            }
                            else
                            {
                                Data = SQL2000Encoding(Environment.NewLine + "Set Identity_Insert " + TableName + " On;" + Environment.NewLine);
                                fs_insert.Write(Data, 0, Data.Length);
                            }

                            bEndInsertionDependant = true;
                            break;
                        case IdentityGenerationOptions.InsertionDependant:
                            //Insertion dependant is the hardest thing to do here
                            break;
                        case IdentityGenerationOptions.OmitIdentityColumns:
                            foreach (Columns c in t.Columns)
                            {
                                //If column is identity, type is not included
                                if (c.IdentityColumn)
                                    c.OmitColumn = true;
                            }
                            break;
                    }
                }

                #endregion

                Header = GenerateHeaderSentence(t.Columns, TableName, null, true);

                t.Columns = _DBConnection.GetTableData(t.Columns, GenerateHeaderSentence(t.Columns, TableName, _GenOpts.TopRows, false));

                MaxData = 0;

                for (Int32 i = 0; i < t.Columns.Count; i++)
                {
                    if (t.Columns[i] != null && t.Columns[i].Data != null)
                    {
                        MaxData = t.Columns[i].Data.Count;
                        break;
                    }
                }

                #region Row processing

                for (Int32 i = 0; i < MaxData; i++)
                {
                    #region Check and Breaks script is LinesPerBlock reached
                    //Lines per block reached.
                    if (LinesPerBlock == _GenOpts.LinesPerBlock && !_GenOpts.TransacionalScript)
                    {
                        if (!_GenOpts.SQL2000Compatible)
                        {
                            sw_insert.WriteLine();

                            //Disable Identity Insert
                            if (bEndInsertionDependant && t.HasIdentityColumn)
                            {
                                sw_insert.WriteLine("Set Identity_Insert " + TableName + " Off;");
                            }

                            sw_insert.WriteLine();

                            //Go Statement - This sentence completes the batch and the query's memory consupmtion.
                            //Remember: "Go" does not have semicolon.
                            sw_insert.WriteLine("Go");
                            sw_insert.WriteLine();

                            //Re-enable Identity Insert
                            if (bEndInsertionDependant && t.HasIdentityColumn)
                            {
                                sw_insert.WriteLine("Set Identity_Insert " + TableName + " On;");
                            }

                            sw_insert.WriteLine();
                        }
                        else
                        {
                            //Disable Identity Insert
                            if (bEndInsertionDependant && t.HasIdentityColumn)
                            {
                                Data = SQL2000Encoding(Environment.NewLine + "Set Identity_Insert " + TableName + " Off;" + Environment.NewLine + "Go" + Environment.NewLine + "Set Identity_Insert " + TableName + " On;" + Environment.NewLine);
                                fs_insert.Write(Data, 0, Data.Length);
                            }
                            else
                            {
                                Data = SQL2000Encoding(Environment.NewLine + Environment.NewLine + "Go" + Environment.NewLine + Environment.NewLine);
                                fs_insert.Write(Data, 0, Data.Length);
                            }
                        }

                        //Resets the line counter.
                        LinesPerBlock = 0;
                    }
                    #endregion

                    sb = new StringBuilder();
                    sb.Append(Header);

                    foreach (Columns c in t.Columns)
                    {
                        #region Process Column Data

                        bIncludeSeparator = true;

                        if (c.Data != null && i < c.Data.Count)
                        {
                            if (c.Data[i] == null)
                                sb.Append("NULL");
                            else
                            {
                                switch (dDataTypes[c.Type])
                                {
                                    case DataTypes.Binarys:
                                        //My tests conclude that this types must be included like integers (no ', no convert)
                                        sb.Append(c.Data[i]);
                                        break;
                                    case DataTypes.Decimals:
                                        sb.Append(c.Data[i].Replace(',', '.'));
                                        break;
                                    case DataTypes.Ignored:
                                        //Nothing to append.
                                        break;
                                    case DataTypes.Integers:
                                        sb.Append(c.Data[i]);
                                        break;
                                    case DataTypes.NotIncluded:
                                        //Nothing to append. No include separator
                                        bIncludeSeparator = false;
                                        break;
                                    case DataTypes.NotSupported:
                                        throw new ApplicationException("The data type " + c.Type + " is not supported.");
                                        break;
                                    case DataTypes.SpecificTypes:
                                        sb.Append("Convert(");
                                        sb.Append(c.Type);
                                        sb.Append(", '");
                                        sb.Append(c.Data[i]);
                                        sb.Append("')");
                                        break;
                                    case DataTypes.Strings:
                                        sb.Append("'");
                                        sb.Append(c.Data[i]);
                                        sb.Append("'");
                                        break;
                                    case DataTypes.Dates:
                                        sb.Append("Convert(");
                                        sb.Append(c.Type);
                                        sb.Append(", '");
                                        sb.Append(c.Data[i]);
                                        sb.Append("', ");
                                        sb.Append(DateFormatStyle);
                                        sb.Append(")");
                                        break;
                                }
                            }

                            if (bIncludeSeparator)
                                sb.Append(",");
                        }
                    }

                    if (bIncludeSeparator)
                        sb.Remove(sb.Length - 1, 1);

                    sb.Append(");");

                        #endregion

                    //Now I have the insertion string. I should write it to a text file.
                    if (!_GenOpts.SQL2000Compatible)
                    {
                        sw_insert.WriteLine(sb.ToString());
                    }
                    else
                    {
                        Data = SQL2000Encoding(sb.ToString() + Environment.NewLine);
                        fs_insert.Write(Data, 0, Data.Length);
                        fs_insert.Flush();
                    }

                    //Increments the line counter
                    LinesPerBlock++;
                }

                #endregion

                if (bEndInsertionDependant)
                {
                    if (!_GenOpts.SQL2000Compatible)
                    {
                        sw_insert.WriteLine();
                        sw_insert.WriteLine("Set Identity_Insert " + TableName + " Off;");
                    }
                    else
                    {
                        Data = SQL2000Encoding(Environment.NewLine + "Set Identity_Insert " + TableName + " Off;");
                        fs_insert.Write(Data, 0, Data.Length);
                    }
                }

                if (!_GenOpts.SQL2000Compatible)
                    sw_insert.WriteLine();
                else
                {
                    Data = SQL2000Encoding(Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                }
            }

            #region Finalizing transactional script (if apply)

            //Script is transanctional. Commit if OK. Rollback otherwise.
            if (_GenOpts.TransacionalScript)
            {
                if (!_GenOpts.SQL2000Compatible)
                {
                    sw_insert.WriteLine();
                    sw_insert.WriteLine("If @@Error = 0");
                    sw_insert.WriteLine("   Begin");
                    sw_insert.WriteLine("     Commit Tran @TransactionName");
                    sw_insert.WriteLine("     Print 'Transaction ' + @TransactionName + ' was succesfully commited.'");
                    sw_insert.WriteLine("   End");
                    sw_insert.WriteLine("Else");
                    sw_insert.WriteLine("   Begin");
                    sw_insert.WriteLine("     RollBack Tran @TransactionName");
                    sw_insert.WriteLine("     Print 'Transaction ' + @TransactionName + ' could not be commited and was succesfully rollbacked.'");
                    sw_insert.WriteLine("");
                    sw_insert.WriteLine("   End");
                    sw_insert.WriteLine();
                }
                else
                {
                    Data = SQL2000Encoding(Environment.NewLine + "If @@Error = 0" + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);

                    Data = SQL2000Encoding("   Begin" + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                    Data = SQL2000Encoding("     Commit Tran @TransactionName" + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                    Data = SQL2000Encoding("     Print 'Transaction ' + @TransactionName + ' was succesfully commited.'" + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                    Data = SQL2000Encoding("   End" + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                    Data = SQL2000Encoding("Else" + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                    Data = SQL2000Encoding("   Begin" + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                    Data = SQL2000Encoding("     RollBack Tran @TransactionName" + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                    Data = SQL2000Encoding("     Print 'Transaction ' + @TransactionName + ' could not be commited and was succesfully rollbacked.'" + Environment.NewLine + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                    Data = SQL2000Encoding("   End" + Environment.NewLine);
                    fs_insert.Write(Data, 0, Data.Length);
                }
            }

            #endregion

            #region Closing file...

            if (!_GenOpts.SQL2000Compatible)
            {
                sw_insert.WriteLine("-- Generated by Suru Insert Generator - " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
                sw_insert.WriteLine();

                if (GraphHasCycles)
                {
                    sw_insert.WriteLine("-- Warning: data tables has cycles between them. Those tables are not in order.");
                    sw_insert.WriteLine();
                }

                sw_insert.WriteLine("Go");

                sw_insert.Close();
            }
            else
            {
                Data = SQL2000Encoding("-- Generated by Suru Insert Generator - " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + Environment.NewLine);
                fs_insert.Write(Data, 0, Data.Length);

                if (GraphHasCycles)
                {
                    Data = SQL2000Encoding("-- Warning: data tables has cycles between them. Those tables are not in order.");
                    fs_insert.Write(Data, 0, Data.Length);
                }

                Data = SQL2000Encoding("Go" + Environment.NewLine);
                fs_insert.Write(Data, 0, Data.Length);

                fs_insert.Close();
            }

            #endregion

            #endregion

            #region Size Checking and Split File if neccesary

            //File Size Cheking
            //Setting: MaximumFileSize (in megabytes)
            Int64 MaximumFileSize = Int32.Parse(ConfigurationManager.AppSettings.Get("MaximumFileSize")) * 1024 * 1024;

            FileInfo fi = new FileInfo(TempFileName);
            String FinalFileName = Path.Combine(_OutputPath, GenerateFileName(ScriptTypes.InsertScript));

            if (fi.Length > MaximumFileSize)
            {
                //File is longer than it should be. Split file.
                StreamReader sr = new StreamReader(TempFileName);
                FileStream fs = null;

                Int64 CurrentSize = 0;
                Int16 CurrentFileNumber = 1;
                String PartName;

                PartName = Path.Combine(Path.GetDirectoryName(FinalFileName), Path.GetFileNameWithoutExtension(FinalFileName) + "_" + CurrentFileNumber.ToString() + Path.GetExtension(FinalFileName));
                fs = new FileStream(PartName, FileMode.OpenOrCreate);

                while (!sr.EndOfStream)
                {
                    Data = SQL2000Encoding(sr.ReadLine() + Environment.NewLine);

                    if (CurrentSize + Data.Length > MaximumFileSize)
                    {
                        CurrentFileNumber++;
                        CurrentSize = 0;

                        //Create next part number
                        fs.Close();
                        PartName = Path.Combine(Path.GetDirectoryName(FinalFileName), Path.GetFileNameWithoutExtension(FinalFileName) + "_" + CurrentFileNumber.ToString() + Path.GetExtension(FinalFileName));
                        fs = new FileStream(PartName, FileMode.OpenOrCreate);                        
                    }

                    fs.Write(Data, 0, Data.Length);
                    fs.Flush();

                    CurrentSize += Data.Length;
                }

                fs.Close();
                sr.Close();

                //Deletes the big file
                File.Delete(TempFileName);
            }
            else
            {
                //File is below maximum allowed size.
                if (File.Exists(FinalFileName))
                    File.Delete(FinalFileName);

                File.Move(TempFileName, FinalFileName);
            }

            #endregion

            return GraphHasCycles;
        }

        /// <summary>
        /// Encodes to support old SQL2000 characters.
        /// </summary>
        /// <param name="txt">Text to Encode</param>
        /// <returns>Byte Array</returns>
        private Byte[] SQL2000Encoding(String txt)
        {
            Byte[] Des = new Byte[txt.Length];

            for (Int32 i = 0; i < txt.Length; i++)
                Des[i] = (Byte)(txt[i]);

            return Des;
        }

        /// <summary>
        /// This method generates the first part of the Insert sentence (Insert Into Table( columns...) Values (
        /// </summary>
        /// <param name="lColumns">List of table's columns.</param>
        /// <param name="TableName">Table's name.</param>
        /// <param name="bIsInsert">If true, generate Insert statement. Otherwise, generate select sentence.</param>
        /// <returns>Insert sentence (first part).</returns>
        private String GenerateHeaderSentence(List<Columns> lColumns, String TableName, Nullable<Int32> Rows, Boolean bIsInsert)
        {
            StringBuilder sb = new StringBuilder();
            Int16 NotIncludedColumns = 0;

            if (bIsInsert)
            {
                sb.Append("Insert Into ");
                //sb.Append("\"");
                sb.Append("[");
                sb.Append(TableName);
                //sb.Append("\"");
                sb.Append("]");
                sb.Append("(");
            }
            else
            {
                sb.Append("Select ");

                if (Rows != null)
                {
                    sb.Append(" Top ");
                    sb.Append(Rows);
                    sb.Append(" ");
                }
            }

            foreach (Columns c in lColumns)
            {
                //Cannot select not supported data types
                if (dDataTypes[c.Type] == DataTypes.NotSupported)
                    throw new ApplicationException("Table [" + TableName + "] has a column of Data Type " + c.Type + ", which is not supported currently by application");

                //Ignored and Not included data types won't be selected.
                if (dDataTypes[c.Type] != DataTypes.NotIncluded && dDataTypes[c.Type] != DataTypes.Ignored && !c.OmitColumn && dDataTypes[c.Type] != DataTypes.Dates)
                {
                    //sb.Append("\"");
                    sb.Append("[");
                    sb.Append(c.Name);
                    sb.Append("]");
                    //sb.Append("\"");
                    sb.Append(",");
                }
                else
                {
                    if (dDataTypes[c.Type] == DataTypes.NotIncluded || dDataTypes[c.Type] == DataTypes.Ignored || c.OmitColumn)
                        NotIncludedColumns++;
                }

                //Dates must be converted to ISO format before processing.
                if (dDataTypes[c.Type] == DataTypes.Dates)
                {
                    if (bIsInsert)
                    {
                        //sb.Append("\"");
                        sb.Append("[");
                        sb.Append(c.Name);
                        //sb.Append("\"");
                        sb.Append("]");
                        sb.Append(",");
                    }
                    else
                    {
                        sb.Append("Convert(VarChar, ");
                        //sb.Append("\"");
                        sb.Append("[");
                        sb.Append(c.Name);
                        sb.Append("]");
                        //sb.Append("\"");
                        sb.Append(", ");
                        sb.Append(DateFormatStyle);
                        sb.Append(") As ");
                        sb.Append("[");
                        //sb.Append("\"");
                        sb.Append(c.Name);
                        sb.Append("]");
                        //sb.Append("\"");
                        sb.Append(",");
                    }
                }
            }

            //Remove the last comma separator
            sb.Remove(sb.Length - 1, 1);

            if (bIsInsert)
                sb.Append(") Values (");
            else
            {
                sb.Append(" From ");
                sb.Append("[");
                //sb.Append("\"");
                sb.Append(TableName);
                //sb.Append("\"");
                sb.Append("]");
                sb.Append(";");
            }

            //If amount of not included columns is equal to number of columns return nothing.
            if (NotIncludedColumns == lColumns.Count)
            {
                sb = new StringBuilder();
                sb.Append(";");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates the Script File name, according with app.config snippets.
        /// </summary>
        /// <param name="Type">Type of script generated. Currently, only table creation and insert creations are available.</param>
        /// <returns>File name to use.</returns>
        private String GenerateFileName(ScriptTypes Type)
        {
            String CrudeName = "#XX#_No_File_Name.sql";

            if (Type == ScriptTypes.InsertScript)
                CrudeName = ConfigurationManager.AppSettings.Get("PoblateTablesSnippet");

            if (Type == ScriptTypes.TableScript)
                CrudeName = ConfigurationManager.AppSettings.Get("CreateTableSnippet");

            StringBuilder sb = new StringBuilder();
            sb.Append(CrudeName);

            //Replace the #DB# snippet
            sb.Replace("#DB#", _DBConnection.Last_DataBase);

            //Replace the #XX# snippet
            sb.Replace("#XX#", _Script_Number.ToString("00"));

            _Script_Number++;

            return sb.ToString();
        }

        /// <summary>
        /// Sort de tables by its dependancy. Less dependant ones first, most dependants last.
        /// </summary>
        /// <param name="lTables">Table list to order.</param>
        /// <param name="DBConnection">Connection object.</param>
        /// <returns>Ordered list of tables by its dependancy.</returns>
        private List<String> SortTablesByDependancy(List<String> lTables, Connection DBConnection, ref Boolean GraphHasCycles)
        {
            //Tables must be sorted by dependancy.
            //E.g.: less or none dependant table first. Most dependant table last.

            #region Local Variables

            List<DependancyList> Dependancy = new List<DependancyList>();
            DependancyList TableDependancy = null;

            //This matrix will tell dependency count and dependency graph
            Int16[,] TableOrder = new Int16[lTables.Count, lTables.Count];

            String tablename = null;

            #endregion

            #region Generate Dependancy Table Graph

            for (Int16 i = 0; i < lTables.Count; i++)
            {
                tablename = lTables[i];
                TableDependancy = new DependancyList();
                TableDependancy.TableName = tablename;
                TableDependancy.TableDependancy = DBConnection.ListTableDependancy(tablename);

                Dependancy.Add(TableDependancy);

                if (TableDependancy.TableDependancy != null)
                    //Only check selected tables, not all of them
                    for (Int16 j = 0; j < lTables.Count; j++)
                        //Not checking itself
                        if (i != j && TableDependancy.TableDependancy.Contains(lTables[j]))
                            TableOrder[i, j] = 1;
            }

            /*
             
            //This code is for dumping the table content into a file
             
            //Writing table into a file
            StreamWriter sw = new StreamWriter(@"c:\matrix.txt");

            for (Int16 i = 0; i < lTables.Count; i++)
            {
                sw.Write(lTables[i]);
                sw.Write('\t');
              
              
                for (Int16 j = 0; j < lTables.Count; j++)
                {
                    sw.Write(TableOrder[i, j].ToString());
                    sw.Write('\t');
                }

                sw.WriteLine();
            }
            
            sw.Close();
              
            */



            #endregion

            #region Ordering Arrays and Solving conflicts

            Int16[] CorrectOrder = OrderGraph(TableOrder, (Int16)lTables.Count, ref GraphHasCycles);

            List<String> OrderedList = new List<String>();

            //Reorganize the list to reflect the right order
            for (Int16 i = 0; i < lTables.Count; i++)
                OrderedList.Add(lTables[CorrectOrder[i]]);

            #endregion

            return OrderedList;
        }

        /// <summary>
        /// Order a graph. Return results in an array (indirect index to right one).
        /// </summary>
        /// <param name="TableOrder">Dependancy matrix.</param>
        /// <param name="TableNumber">Number of tables.</param>
        /// <returns>Ordered array.</returns>
        private Int16[] OrderGraph(Int16[,] TableOrder, Int16 TableNumber, ref Boolean GraphHasCycles)
        {
            //This method will process the graph and order the tables
            //The ordering method here working is the "topological sort"

            GraphHasCycles = false;

            Int16[] CorrectOrder = new Int16[TableNumber];
            List<Int16> NotProcessed = new List<Int16>();
            Int16 PredecessorCount, SuccessorCount;

            //This arrays will be the criteria to order
            Int16[,] LinkTable = new Int16[TableNumber, 2];

            Int16[] OrderedRank = new Int16[TableNumber];
            Int16[] PredecessorRank = new Int16[TableNumber];

            //Calculates predeccesor and succesor for each table
            for (Int16 i = 0; i < TableNumber; i++)
            {
                NotProcessed.Add(i);
                PredecessorCount = 0;
                SuccessorCount = 0;

                for (Int16 j = 0; j < TableNumber; j++)
                {
                    PredecessorCount += TableOrder[i, j];
                    SuccessorCount += TableOrder[j, i];
                }

                OrderedRank[i] = i;
                PredecessorRank[i] = PredecessorCount;

                LinkTable[i, 0] = PredecessorCount;
                LinkTable[i, 1] = SuccessorCount;
            }

            Array.Sort(PredecessorRank, OrderedRank);
            Int16 /*Index = 0,*/ LastOrderedNode = 0;

            //Orders the graph
            for (Int16 i = 0; i < TableNumber; i++)
            {
                if (NotProcessed.Contains(OrderedRank[i]))
                    ProcessNode(OrderedRank[i], ref LastOrderedNode, ref TableOrder, ref LinkTable, ref TableNumber, ref NotProcessed, ref CorrectOrder);
            }

            //When graph has horrible cycles, the algorithm won't process the cycling nodes. 
            if (NotProcessed.Count != 0)
            {
                GraphHasCycles = true;

                foreach (Int16 i in NotProcessed)
                {
                    CorrectOrder[LastOrderedNode] = i;
                    LastOrderedNode++;
                }
            }

            return CorrectOrder;
        }

        /// <summary>
        /// Process the graph nodes, to list them in right order.
        /// </summary>
        /// <param name="Index">Node to analize.</param>
        /// <param name="LastOrderedNode">Last correctly ordered node.</param>
        /// <param name="TableOrder">Table Order matrix.</param>
        /// <param name="LinkTable">Precedessors and succesors count.</param>
        /// <param name="TableNumber">Number of tables.</param>
        /// <param name="NotProcessed">Unprocessed node list.</param>
        /// <param name="CorrectOrder">Array ordered to the moment of the method call.</param>
        private void ProcessNode(Int16 Index, ref Int16 LastOrderedNode, ref Int16[,] TableOrder, ref Int16[,] LinkTable, ref Int16 TableNumber, ref List<Int16> NotProcessed, ref Int16[] CorrectOrder)
        {
            if (NotProcessed.Contains(Index) && NotProcessed.Count != 0)
            {
                Boolean OrderNode = false;

                if (LinkTable[Index, 0] == 0)
                    OrderNode = true;
                else
                {
                    //Let's assume order node is the right one.
                    OrderNode = true;

                    //Check its dependencies
                    for (Int16 i = 0; i < TableNumber; i++)
                        if (TableOrder[Index, i] == 1)
                            //If dependant node is not processed, abort checking
                            if (NotProcessed.Contains(i))
                            {
                                //Ops! A required predeccessor is not present in list...
                                OrderNode = false;
                                break;
                            }

                }

                //Process node and child nodes if node is to order
                if (OrderNode)
                {
                    CorrectOrder[LastOrderedNode] = Index;
                    LastOrderedNode++;
                    NotProcessed.Remove(Index);

                    //Process the childs recursively
                    for (Int16 i = 0; i < TableNumber; i++)
                        if (TableOrder[i, Index] == 1 && NotProcessed.Contains(i))
                            ProcessNode(i, ref LastOrderedNode, ref TableOrder, ref LinkTable, ref TableNumber, ref NotProcessed, ref CorrectOrder);
                }
            }
        }
    }
}
