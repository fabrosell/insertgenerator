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
                StreamWriter sw = null;
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
                    sConnectionNodes.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + Environment.NewLine + "<Connections>" + Environment.NewLine);

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
                    sConnectionNodes.Append( "<Connection>" + Encryption.Encrypt(                                                    
                                                "<Host>" + _HostName + "</Host>" +
                                                "<User>" + sUserName + "</User>" +
                                                "<Pass>" + sPassword + "</Pass>" +
                                                "<Authentication>" + _Authentication.ToString() + "</Authentication>" +
                                                "<DataBase>" + _Last_DataBase + "</DataBase>" +
                                                "<IsLastSuccessful>True</IsLastSuccessful>" +
                                                "<PasswordSaved>" + _SavePassword.ToString() + "</PasswordSaved>"
                                                ) +
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
                            sConnectionNodes.Append("<Connection>" + Encryption.Encrypt(xmlSavedConnection.InnerXml.ToString()) + "</Connection>" + Environment.NewLine);
                    }

                    //The Node List is complete. It must be written to the connection file.
                    sConnectionNodes.Append("</Connections>");

                    //Writes the file
                    //Rewrites the file
                    sw = new StreamWriter(ConfigurationManager.AppSettings.Get("ConfigurationFile"));

                    sw.Write(sConnectionNodes);
                }
                catch (Exception ex)
                {
                    throw new Exception();
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Can't load or write configuration file " + ConfigurationManager.AppSettings.Get("ConfigurationFile"));
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
        public List<Columns> GetTableColumns(Int64 ID)
        {
            List<Columns> lTableColumns = new List<Columns>();
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

                        lTableColumns.Add(Column);
                    }

                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                //Connection Fails
                _ErrorMessage = ex.Message;
            }

            return lTableColumns;
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

                            if (c.Data == null)
                                c.Data = new List<String>();

                            if (dr[c.Name] == DBNull.Value)
                                c.Data.Add(null);
                            else
                                c.Data.Add(dr[c.Name].ToString().Replace("'", "''"));
                        }
                    }

                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                //Connection Fails
                _ErrorMessage = ex.Message;
            }

            return lTableColumns;
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

        #endregion
    }

    public enum AuthenticationMethods { SqlServer, Windows };

    public enum IdentityGenerationOptions { IdentityInsert, InsertionDependant, OmitIdentityColumns };

    public enum DataTypes { Integers, Decimals, Binarys, Strings, SpecificTypes, Ignored, NotIncluded, NotSupported };

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

        #endregion

        public Columns()
        {
            _AllowNulls = false;
            _AnsiPaddingOn = false;
            _OutputParameter = false;
            _IdentityColumn = false;
        }
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

            //Data Type Not Supported Yet
            //dDataTypes.Add("binary", DataTypes.Binarys);
            dDataTypes.Add("binary", DataTypes.NotSupported);

            //Data Type Not Supported Yet
            //dDataTypes.Add("varbinary", DataTypes.Binarys);
            dDataTypes.Add("varbinary", DataTypes.NotSupported);

            //Data Type Not Supported Yet
            //dDataTypes.Add("image", DataTypes.Binarys);
            dDataTypes.Add("image", DataTypes.NotSupported);

            dDataTypes.Add("char", DataTypes.Strings);
            dDataTypes.Add("nchar", DataTypes.Strings);
            dDataTypes.Add("ntext", DataTypes.Strings);
            dDataTypes.Add("nvarchar", DataTypes.Strings);
            dDataTypes.Add("text", DataTypes.Strings);
            dDataTypes.Add("varchar", DataTypes.Strings);

            dDataTypes.Add("xml", DataTypes.SpecificTypes);
            dDataTypes.Add("bit", DataTypes.SpecificTypes);
            dDataTypes.Add("datetime", DataTypes.SpecificTypes);
            dDataTypes.Add("smalldatetime", DataTypes.SpecificTypes);

            dDataTypes.Add("decimal", DataTypes.Decimals);
            dDataTypes.Add("float", DataTypes.Decimals);
            dDataTypes.Add("money", DataTypes.Decimals);
            dDataTypes.Add("numeric", DataTypes.Decimals);
            dDataTypes.Add("real", DataTypes.Decimals);
            dDataTypes.Add("smallmoney", DataTypes.Decimals);

            dDataTypes.Add("timestamp", DataTypes.NotIncluded);

            dDataTypes.Add("uniqueidentifier", DataTypes.NotSupported);
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
            List<Columns> TableColumns;

            //This list will hold all tables already generated.
            List<String> GeneratedTables = new List<String>();

            //Integers --> These types don't need any special consideration on insert clause.
            //Decimals --> Care must be taken when generating insert (remember decimal dot).
            //Binarys  --> Binary types. Don't know how to handle them now.
            //Strings  --> These types must have "'" enclosing his values.
            //SpecificTypes --> Types which must have a 'Convert' clause and values must be enclosed with "'"
            //Ignored       --> These types are not included on inserts.
            //NotIncluded   --> These types are not included on inserts, because it's impossible to insert them.
            //NotSupported  --> These types are not supported yet. An exception must be thrown.

            StringBuilder sb;
            String Header;
            StreamWriter sw_insert = new StreamWriter(Path.Combine(_OutputPath, GenerateFileName(ScriptTypes.InsertScript)));
            sw_insert.AutoFlush = true;

            foreach (String TableName in lTables)
            {
                TableID = _DBConnection.GetTableID(TableName);

                TableColumns = _DBConnection.GetTableColumns(TableID);

                Header = GenerateHeaderSentence(TableColumns, TableName, true);

                TableColumns = _DBConnection.GetTableData(TableColumns, GenerateHeaderSentence(TableColumns, TableName, false));
                
                //All rows...
                for (Int32 i = 0; i < TableColumns[0].Data.Count; i++)
                {
                    sb = new StringBuilder();
                    sb.Append(Header);

                    foreach (Columns c in TableColumns)
                    {
                        if (c.Data[i] == null)
                            sb.Append("NULL");
                        else
                        {                            
                            switch (dDataTypes[c.Type])
                            {
                                case DataTypes.Binarys:
                                    //I don't know how to process them.
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
                                    //Nothing to append
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
                            }
                        }

                        sb.Append(",");
                    }

                    sb.Remove(sb.Length - 1, 1);
                    sb.Append(");");

                    //Now I have the insertion string. I should write it to a text file.
                    sw_insert.WriteLine(sb.ToString());
                }                
            }

            sw_insert.Close();
        }

        /// <summary>
        /// This method generates the first part of the Insert sentence (Insert Into Table( columns...) Values (
        /// </summary>
        /// <param name="lColumns">List of table's columns.</param>
        /// <param name="TableName">Table's name.</param>
        /// <param name="bIsInsert">If true, generate Insert statement. Otherwise, generate select sentence.</param>
        /// <returns>Insert sentence (first part).</returns>
        private String GenerateHeaderSentence(List<Columns> lColumns, String TableName, Boolean bIsInsert)
        {
            StringBuilder sb = new StringBuilder();

            if (bIsInsert)
            {
                sb.Append("Insert Into ");
                sb.Append(TableName);
                sb.Append("(");
            }
            else
                sb.Append("Select ");

            foreach (Columns c in lColumns)
            {             
                if (dDataTypes[c.Type] == DataTypes.NotSupported)
                    throw new ApplicationException("Table " + TableName + " has a column of Data Type " + c.Type + ", which is not supported currently by application");

                if (dDataTypes[c.Type] != DataTypes.NotIncluded && dDataTypes[c.Type] != DataTypes.Ignored)
                {
                    sb.Append(c.Name);
                    sb.Append(",");
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
    }
}
