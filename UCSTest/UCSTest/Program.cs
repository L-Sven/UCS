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
            Console.Title = "Power Bi Datafetcher";
            string ftg = ConfigurationManager.AppSettings["ftgPath"];
            string sys = ConfigurationManager.AppSettings["sysPath"];
            string startDatum = ConfigurationManager.AppSettings["datum"];
            
            //VismaTidData vismaTid = new VismaTidData(ftg, sys);
            VismaData go = new VismaData(ftg, sys, startDatum);
        }
    }
}
