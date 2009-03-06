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
                    sConnectionNodes.Append( "<Connection>" + 
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
        public void GenerateInserts(List<String> lTables)
        {            
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
            StreamWriter sw_insert = new StreamWriter(Path.Combine(_OutputPath, GenerateFileName(ScriptTypes.InsertScript)));
            sw_insert.AutoFlush = true;
            Table t;
            Boolean bEndInsertionDependant = false, bIncludeSeparator = true;
            Int32 MaxData, LinesPerBlock = 0;

            sw_insert.WriteLine("-- Generated by Suru Insert Generator - " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
            sw_insert.WriteLine();

            #region Beggining of transactional script (if apply)
            
            if (_GenOpts.TransacionalScript)
            {
                sw_insert.WriteLine("Declare @TransactionName VarChar(20)");
                sw_insert.WriteLine("Set @TransactionName = 'SuruInsert'");
                sw_insert.WriteLine("Begin Tran @TransactionName");
                sw_insert.WriteLine();
            }

            #endregion

            if (_GenOpts.LinesPerBlock == 0)
                _GenOpts.LinesPerBlock = Int32.Parse(ConfigurationManager.AppSettings.Get("LinesPerBlock"));

            lTables = SortTablesByDependancy(lTables, _DBConnection);

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
                            sw_insert.WriteLine();
                            sw_insert.WriteLine("Set Identity_Insert " + TableName + " On;");
                            sw_insert.WriteLine();
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
                        sw_insert.WriteLine();

                        //Disable Identity Insert
                        if (bEndInsertionDependant)
                        {
                            sw_insert.WriteLine("Set Identity_Insert " + TableName + " Off;");
                            sw_insert.WriteLine();
                        }

                        //Go Statement - This sentence completes the batch and the query's memory consupmtion.
                        //Remember: "Go" does not have semicolon.
                        sw_insert.WriteLine("Go");
                        sw_insert.WriteLine();

                        //Re-enable Identity Insert
                        if (bEndInsertionDependant)
                        {
                            sw_insert.WriteLine("Set Identity_Insert " + TableName + " On;");
                            sw_insert.WriteLine();
                        }

                        //Resets the line counter.
                        LinesPerBlock = 0;
                    }
                    #endregion

                    sb = new StringBuilder();                    
                    sb.Append(Header);

                    foreach (Columns c in t.Columns)
                    {
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

                    //Now I have the insertion string. I should write it to a text file.
                    sw_insert.WriteLine(sb.ToString());

                    //Increments the line counter
                    LinesPerBlock++;
                }

                #endregion

                if (bEndInsertionDependant)
                {
                    sw_insert.WriteLine();
                    sw_insert.WriteLine("Set Identity_Insert " + TableName + " Off;");
                    sw_insert.WriteLine();
                }
            }

            #region Finalizing transactional script (if apply)

            //Script is transanctional. Commit if OK. Rollback otherwise.
            if (_GenOpts.TransacionalScript)
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

            #endregion
            
            sw_insert.WriteLine("-- Generated by Suru Insert Generator - " + DateTime.Now.ToString("yyyyMMdd HH:mm:ss"));
            sw_insert.WriteLine();
            sw_insert.WriteLine("Go");


            sw_insert.Close();
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

            if (bIsInsert)
            {
                sb.Append("Insert Into ");
                sb.Append(TableName);
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
                    throw new ApplicationException("Table " + TableName + " has a column of Data Type " + c.Type + ", which is not supported currently by application");

                //Ignored and Not included data types won't be selected.
                if (dDataTypes[c.Type] != DataTypes.NotIncluded && dDataTypes[c.Type] != DataTypes.Ignored && !c.OmitColumn && dDataTypes[c.Type] != DataTypes.Dates)
                {
                    sb.Append(c.Name);
                    sb.Append(",");
                }

                //Dates must be converted to ISO format before processing.
                if (dDataTypes[c.Type] == DataTypes.Dates)
                {
                    if (bIsInsert)
                    {
                        sb.Append(c.Name);
                        sb.Append(",");
                    }
                    else
                    {
                        sb.Append("Convert(VarChar, ");
                        sb.Append(c.Name);
                        sb.Append(", ");
                        sb.Append(DateFormatStyle);
                        sb.Append(") As ");
                        sb.Append(c.Name);
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
                sb.Append(TableName);
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
        
        private List<String> SortTablesByDependancy(List<String> lTables, Connection DBConnection)
        {
            //Tables must be sorted by dependancy.
            //E.g.: less or none dependant table first. Most dependant table last.

            #region Local Variables

            List<DependancyList> Dependancy = new List<DependancyList>();
            DependancyList TableDependancy = null;

            //This matrix will tell dependency count and dependency graph
            Int16[,] TableOrder = new Int16[lTables.Count, lTables.Count];
            Int16[] ReferenceCountArray = new Int16[lTables.Count];
            Int16[] RowOrderArray = new Int16[lTables.Count];

            String tablename = null;
            Int16 ReferenceCount;
            Boolean IncrementI = true;

            #endregion

            #region Generate Dependancy Table Graph and PreOrdering arrays

            for (Int16 ii = 0; ii < lTables.Count; ii++)
            {
                ReferenceCount = 0;

                tablename = lTables[ii];
                TableDependancy = new DependancyList();
                TableDependancy.TableName = tablename;
                TableDependancy.TableDependancy = DBConnection.ListTableDependancy(tablename);

                Dependancy.Add(TableDependancy);                                

                if (TableDependancy.TableDependancy != null)
                    //Only check selected tables, not all of them
                    for (Int16 jj = 0; jj < lTables.Count; jj++)
                        //Not checking itself
                        if (ii != jj && TableDependancy.TableDependancy.Contains(lTables[jj]))
                        {
                            TableOrder[ii, jj] = 1;
                            ReferenceCount++;
                        }
                
                //Store results in arrays (to be sorted later)
                ReferenceCountArray[ii] = ReferenceCount;
                RowOrderArray[ii] = ii;
            }

            #endregion

            #region Ordering Arrays and Solving conflicts

            //RowOrder must be sorted by Reference Count. Less reference count, less dependancy.
            //Equal Reference Count checked by reference between tied ones.
            Array.Sort(ReferenceCountArray, RowOrderArray);

            //--> Row Order Array now contains the Ordered sequence of values by Reference Count.

            List<String> OrderedList = new List<String>();

            //Order the list.
            Int16 i = 0, j = 0;           

            //This procedure is a mess... If someone find a simple way email it to me --> fabrosell@gmail.com
            //The biggest problem is solving ties on reference count...
            while (i < lTables.Count)
            {
                IncrementI = true;

                //Check for same dependency level
                if (i < lTables.Count - 1)
                {
                    //More than one with same dependancy. Problem!
                    if (ReferenceCountArray[i] == ReferenceCountArray[i + 1])
                    {
                        #region Code to Solve a Tie

                        //Get the last position which has the same Reference Count
                        j = i;
                        while (ReferenceCountArray[j] == ReferenceCountArray[i] && j < lTables.Count - 1)
                            j++;

                        Int16[] SubGroupReference = new Int16[j - i + 1];
                        Int16[] SubGroupOrder = new Int16[j - i + 1];

                        //Interval of same Reference Counts starts on i and ends on j.
                        //k goes by the reference counts
                        for (Int16 k = i; k <= j; k++)
                        {
                            SubGroupReference[k - i] = 0;
                            SubGroupOrder[k - i] = k;
                            
                            for (Int16 z = i; z <= j; z++)
                            {
                                SubGroupReference[k - i] += TableOrder[RowOrderArray[k], RowOrderArray[z]];
                            }
                        }

                        //Ordering arrays
                        Array.Sort(SubGroupReference, SubGroupOrder);

                        //Now we have solved the conflict and we have the right dependancy order.
                        for (Int16 y = 0; y < SubGroupOrder.Length; y++)
                        {
                            OrderedList.Add(lTables[RowOrderArray[SubGroupOrder[y]]]);
                            i++;
                        }

                        IncrementI = false;

                        #endregion
                    }
                    else
                        OrderedList.Add(lTables[RowOrderArray[i]]);
                }

                if (IncrementI)
                    i++;
            }

            #endregion

            return OrderedList;
        }

    }
}
