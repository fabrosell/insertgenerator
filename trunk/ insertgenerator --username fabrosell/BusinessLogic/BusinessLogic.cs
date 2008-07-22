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

        #region Encapsulamiento de Atributos

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

        #endregion

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
    }
}
