using NLog;
using System;
using System.Collections.Generic;
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
        }

        // Loggar meddelanden för strängar (exempelvis vid felaktiga kommentarer på avtal)
        public void ErrorMessage(string msg)
        {

            logger.Error("Error discovered" + msg);
            counter++;
        }

        public void ErrorMessage(string msg, string msg2)
        {

            logger.Error(msg + msg2);
            counter++;
        }
    }
}
