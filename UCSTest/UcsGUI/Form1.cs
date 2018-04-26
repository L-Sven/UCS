using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UcsGui;

namespace UcsGUI
{
    public partial class Form1 : Form
    {
        private VismaData _go = null;
        private static CancellationTokenSource tokensource = new CancellationTokenSource();
        CancellationToken token = tokensource.Token;
        private readonly string sys;
        private string _ftg;
        private string _datum;

        public Form1()
        {
            InitializeComponent();
            
            VismaData.AddInfoToTextBoxEvent += AddInfo;

            _ftg = ConfigurationManager.AppSettings["ftgPath"];
            sys = ConfigurationManager.AppSettings["sysPath"];
            _datum = ConfigurationManager.AppSettings["datum"];
        }

        #region ===== Buttons =====

        private void btnVäljFöretag_Click(object sender, EventArgs e)
        {
            ChangeFtg();
        }

        private void btnAvsluta_Click(object sender, EventArgs e)
        {
            //Vi avbryter vår Task innan vi avslutar programmet.
            tokensource.Cancel();
            Application.Exit();
        }

        private async void btnKörProgrammet_Click(object sender, EventArgs e)
        {
            txtBoxInfo.Clear();
            
            _go = new VismaData(_ftg, sys, _datum);
            
            //Vi gömmer kör knappen medans programmet arbetar.
            btnKörProgrammet.Visible = false;

            txtBoxInfo.Visible = true;
            //För att inte blockera main thread så kör vi det i en task, och väntar på att Task är färdig innan metoden är färdig.
            Task t = Task.Run(() =>
            {
                //tokensource påverkas av avsluta knappen som avbryter vår Task i så fall.
                while (!tokensource.IsCancellationRequested)
                {
                    _go.StartFetchingData();
                }
            }, token);
            await t;

            btnKörProgrammet.Visible = true;
        }

        #endregion

        public void AddInfo(string s)
        {
            //Evil black magic, nä för att undvika en cross-thread exception så anropar vi
            //en anonym delegate som utför funktionen åt oss genom main threaden.
            this.BeginInvoke((Action)delegate { txtBoxInfo.Text += "\r\n" + s; });
        }

        private void ChangeFtg()
        {
            //Vi sätter ftg till det företaget vi vill ha. Det finns också ett default värde sparat i App.Config som kan ändras.
            FolderBrowserDialog selectFolder = new FolderBrowserDialog();
            selectFolder.SelectedPath = @"C:\ProgramData\SPCS\SPCS Administration";
            selectFolder.Description = "Vänligen ange sökvägen till Företaget som ska öppnas.";
            if (selectFolder.ShowDialog() == DialogResult.OK)
            {
                _ftg = selectFolder.SelectedPath;
            }
            selectFolder.Dispose();
        }

        private void datePickStartDatum_ValueChanged(object sender, EventArgs e)
        {
            _datum = datePickStartDatum.Value.Date.ToString("yyyy-MM-dd");
        }

        private void btnÖppnaErrorLog_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Users\sven_\OneDrive\Dokument\Sourcetree\UCS\UCSTest\UcsGUI\bin\Debug\testlogs\Errorlog " + DateTime.Today.ToString("yyyy-MM-dd") + ".txt");
        }
    }
}
