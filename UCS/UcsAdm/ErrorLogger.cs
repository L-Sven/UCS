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
using Adk = AdkNetWrapper;

namespace UcsAdm
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
            ErrorlogTillDatabas(ex.ToString());
            counter++;
        }

        // Loggar meddelanden för strängar (exempelvis vid felaktiga kommentarer på avtal)
        public void ErrorMessage(string msg)
        {
            logger.Error("Error discovered" + msg);
            ErrorlogTillDatabas(msg);
            counter++;
        }

        public void ErrorMessage(string msg, string msg2)
        {
            logger.Error(msg + msg2);
            ErrorlogTillDatabas(msg+msg2);
            counter++;
        }

        // Loggar errors vid anrop via API
        public void ErrorMessage(Adk.Api.ADKERROR error)
        {

            if (error.lRc != Adk.Api.ADKE_OK)
            {
                string errortext = new string(' ', 200);
                int errtype = (int)Adk.Api.ADK_ERROR_TEXT_TYPE.elRc;
                Adk.Api.AdkGetErrorText(ref error, errtype,
                    ref errortext, 200);
                logger.Error(errortext, "ADK error!");
                ErrorlogTillDatabas(errortext);
                counter++;
            }
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
