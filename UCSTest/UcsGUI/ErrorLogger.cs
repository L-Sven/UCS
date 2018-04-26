using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Adk = AdkNetWrapper;

namespace UcsGui
{
    
    class ErrorLogger
    {

        private Logger logger;
        public int counter = 0;

        public ErrorLogger()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        public void ErrorMessage(Exception ex)
        {
            logger.Error(ex, "Exception discovered");
            counter++;
        }
        public void ErrorMessage(string msg)
        {
            logger.Error(msg, "Error discovered");
            counter++;
        }

        public void ErrorMessage(Adk.Api.ADKERROR error)
        {
            if (error.lRc != Adk.Api.ADKE_OK)
            {
                string errortext = new string(' ', 200);
                int errtype = (int)Adk.Api.ADK_ERROR_TEXT_TYPE.elRc;
                Adk.Api.AdkGetErrorText(ref error, errtype,
                    ref errortext, 200);
                logger.Error(errortext, "ADK error!");
                counter++;
            }
        }
    }
}
