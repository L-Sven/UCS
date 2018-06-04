using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace UcsVismaTid
{
    
    class ErrorLogger
    {

        private Logger logger;
        
        public int counter { get; set; }

        public ErrorLogger()
        {
            logger = LogManager.GetCurrentClassLogger();
            counter = 0;
        }

        // Loggar meddelanden för identifierade exceptions
        public void ErrorMessage(Exception ex)
        {
            logger.Error(ex, "Exception discovered");
            counter++;
            ErrorlogTillDatabas(ex.ToString());
        }

        // Loggar meddelanden för strängar (exempelvis vid felaktiga kommentarer på avtal)
        public void ErrorMessage(string msg)
        {

            logger.Error("Error discovered" + msg);
            counter++;
            ErrorlogTillDatabas(msg);
        }

        public void ErrorMessage(string msg, string msg2)
        {

            logger.Error(msg + msg2);
            counter++;
            ErrorlogTillDatabas(msg+msg2);
        }

        public void ErrorlogTillDatabas(string error)
        {
            var sqlCon2 = new SqlConnection(@ConfigurationManager.AppSettings["dbPath"]);
            SqlCommand cmdAddErrorlog = new SqlCommand("sp_add_errorlog", sqlCon2);

            cmdAddErrorlog.CommandType = CommandType.StoredProcedure;

            try
            {
                sqlCon2.Open();
                cmdAddErrorlog.Parameters.Add(new SqlParameter("@errortext", error));
                cmdAddErrorlog.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this.ErrorMessage(ex);
            }
            finally
            {
                sqlCon2.Close();
            }
        }
    }
}
