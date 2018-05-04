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

namespace UCSTest
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
        }

        // Loggar meddelanden för strängar (exempelvis vid felaktiga kommentarer på avtal)
        public void ErrorMessage(string msg)
        {

            logger.Error(msg, "Error discovered");
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
                counter++;
            }
        }
    }
}
