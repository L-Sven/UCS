using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adk = AdkNetWrapper;
using System.Windows;
using System.Windows.Forms;
using System.Configuration;

namespace UCSTest
{
    class Program
    {

        public static String ftg = String.Empty;
        public static String sys = string.Empty;

        [STAThreadAttribute]
        static void Main(string[] args)
        {
            ftg = ConfigurationManager.AppSettings["ftgPath"];
            sys = ConfigurationManager.AppSettings["sysPath"];
            ChooseVismaFilePath();
            //VismaTidData vismaTid = new VismaTidData(ftg, sys);
            VismaData go = new VismaData();
            // C:\users\sijoh0500\Work Folders\Documents\Github\UCSTest\UCSTest\fakturaDB.mdf
        }

        private static void ChooseVismaFilePath()
        {
            while (ftg.Length == 0 || sys.Length == 0)
            {
                FolderBrowserDialog selectFolder = new FolderBrowserDialog();
                selectFolder.SelectedPath = @"C:\ProgramData\SPCS\SPCS Administration";
                selectFolder.Description = "Vänligen ange sökvägen till Företaget som ska öppnas.";
                if (selectFolder.ShowDialog() == DialogResult.OK)
                {
                    ftg = selectFolder.SelectedPath;
                }
                selectFolder.Dispose();

                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                folderBrowser.SelectedPath = @"C:\ProgramData\SPCS\SPCS Administration";
                folderBrowser.Description = "Vänligen ange sökvägen till Visma Administrations gemensamma filer.";
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    sys = folderBrowser.SelectedPath;
                }

                folderBrowser.Dispose();
            }
            



        }

    }
}
