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
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UcsGui;

namespace UcsGUI
{
    public partial class radBtnHämtaAllData : Form
    {
        private VismaData _go = null;
        private static CancellationTokenSource Tokensource;
        private Thread _localThread = null;

        private readonly string sys;
        private string _ftg;
        private string _datum = null;

        public radBtnHämtaAllData()
        {
            InitializeComponent();

            datePickStartDatum.Enabled = false;
            btnÖppnaErrorLog.Visible = false;

            VismaData.AddInfoToTextBoxEvent += AddInfo;
            
            _ftg = ConfigurationManager.AppSettings["ftgPath"];
            sys = ConfigurationManager.AppSettings["sysPath"];
        }

        #region ===== Buttons =====

        private void btnVäljFöretag_Click(object sender, EventArgs e)
        {
            ChangeFtg();
        }

        private void btnAvsluta_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void btnKörProgrammet_Click(object sender, EventArgs e)
        {
            //Instantierar tokensource här, då det ska vid varje knapptryckning skapas en ny fräsch tokensource som inte har fått sitt värde ändrat.
            Tokensource = new CancellationTokenSource();
            CancellationToken _token = Tokensource.Token;

            btnAvsluta.Enabled = false;
            btnStopTask.Enabled = true;
            btnÖppnaErrorLog.Visible = false;
            btnKörProgrammet.Visible = false;

            txtBoxInfo.Clear();
            int counter = 0;

            _go = new VismaData(_ftg, sys, _datum);
            
            txtBoxInfo.Visible = true;
            //För att inte blockera main thread så kör vi det i en task, och väntar på att Task är färdig innan metoden är färdig.
            Task t = Task.Run(() =>
            {
                _localThread = Thread.CurrentThread;
                counter = _go.StartFetchingData(_token);

            }, _token);
            await t;
            
            btnKörProgrammet.Visible = true;

            if (counter > 0)
            {
                btnÖppnaErrorLog.Visible = true;
            }

            btnAvsluta.Enabled = true;
            btnStopTask.Enabled = false;
        }

        private void btnÖppnaErrorLog_Click(object sender, EventArgs e)
        {
            string path = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Process.Start(path + @"\Errorlog " + DateTime.Today.ToString("yyyy-MM-dd") + ".txt");
        }

        private void btnStopTask_Click(object sender, EventArgs e)
        {
            Tokensource.Cancel();

            btnStopTask.Enabled = false;
            btnAvsluta.Enabled = false;
        }

        #endregion


        #region ===== Radio Buttons =====

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            switch (radBtnFrånVissDatum.Checked)
            {
                case true:
                    radBtnFrånVissDatum.Checked = false;
                    radBtnAllData.Checked = true;
                    datePickStartDatum.Enabled = false;
                    _datum = null;
                    break;
            }
        }

        private void radBtnFrånVissDatum_CheckedChanged(object sender, EventArgs e)
        {
            switch (radBtnAllData.Checked)
            {
                case true:
                    radBtnAllData.Checked = false;
                    radBtnFrånVissDatum.Checked = true;
                    datePickStartDatum.Enabled = true;
                    _datum = datePickStartDatum.Value.Date.ToString("yyyy-MM-dd");
                    break;
            }
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
            _ftg = "";
                if (selectFolder.ShowDialog() == DialogResult.OK)
                {
                    _ftg = selectFolder.SelectedPath;
                    if (_ftg.Contains(@"\SPCS\SPCS Administration\"))
                    {
                        
                        btnKörProgrammet.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Error, ej korrekt sökväg!");
                        btnKörProgrammet.Enabled = false;
                    }
                
                }
                
            selectFolder.Dispose();
        }

        private void datePickStartDatum_ValueChanged(object sender, EventArgs e)
        {
            _datum = datePickStartDatum.Value.Date.ToString("yyyy-MM-dd");
        }

        
    }
}
