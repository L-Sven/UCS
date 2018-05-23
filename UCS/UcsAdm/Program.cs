using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace UcsAdm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Power Bi Datafetcher";

            // Hämtar sökväg till företaget från App.config
            string ftg = ConfigurationManager.AppSettings["ftgPath"];

            // Hämtar sökväg till gemensamma systemfiler från App.config
            string sys = ConfigurationManager.AppSettings["sysPath"];

            // Hämtar datum från App.config från vilket startdatum information hämtas från
            string startDatum = ConfigurationManager.AppSettings["datum"];
            
            VismaData go = new VismaData(ftg, sys, startDatum);
        }
    }
}
