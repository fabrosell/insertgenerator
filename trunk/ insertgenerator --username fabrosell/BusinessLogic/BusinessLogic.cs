using System;
using System.Collections.Generic;
using System.Text;

namespace Suru.InsertGenerator.BusinessLogic
{
    public class Connection
    {
        private String _HostName;
        private String _UserName;
        private String _Password;
        private AuthenticationMethods _Authentication;
        private String _ErrorMessage;
        private Boolean _SavePassword;

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
            set 
            { 
                 _Authentication = value;
                if (value == AuthenticationMethods.Windows)
                {
                    //Make Local User and Password unavailable for storing
                    _UserName = null;
                    _Password = null;
                }
            }
        }

        public String ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; }
        }

        public Boolean SavePassword
        {
            get { return _SavePassword; }
            set { _SavePassword = value; }
        }

        #endregion

        /// <summary>
        /// Class constructor.
        /// </summary>
        /// <param name="Password">Connection's password.</param>
        public Connection(String Password)
        {
            _Password = Password;
        }

        /// <summary>
        /// Get the list of connections.
        /// </summary>
        /// <returns>List of Connections</returns>
        public static List<Connection> GetConnections()
        {
            //Get the list of connections and his logins
            throw new Exception("This method has not been implemented.");
        }

        /// <summary>
        /// Save the current conection to the XML
        /// </summary>
        public void SaveConnection()
        {
            //For one connection, several logins may exists.
            throw new Exception("This method has not been implemented.");
        }

        /// <summary>
        /// Test if the connection is working or not.
        /// </summary>
        /// <returns>True if it can connect. False otherwise.</returns>
        public Boolean TestConnection()
        {
            //Test if connection parameters are valid.
            throw new Exception("This method has not been implemented.");
        }

        /// <summary>
        /// Get the list of Server's DataBases
        /// </summary>
        /// <returns>List of Dabatabes</returns>
        public List<String> GetDataBases()
        {
            //Get the list of DataBases
            throw new Exception("This method has not been implemented.");
        }

    }

    public enum AuthenticationMethods { SqlServer, Windows };
}
