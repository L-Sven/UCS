using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adk = AdkNetWrapper;
using System.Windows;
using System.Configuration;

namespace UCSTest
{
    class Program
    {
        
        

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            // Test
            Console.Title = "Power Bi Datafetcher";

            // Hämtar sökväg till företaget från App.config
            string ftg = ConfigurationManager.AppSettings["ftgPath"];

            // Hämtar sökväg till gemensamma systemfiler från App.config
            string sys = ConfigurationManager.AppSettings["sysPath"];

            // Hämtar datum från App.config från vilket startdatum information hämtas från
            string startDatum = ConfigurationManager.AppSettings["datum"];
            
            //VismaTidData vismaTid = new VismaTidData(ftg, sys);
            VismaData go = new VismaData(ftg, sys, startDatum);
            
        }

    }
}
