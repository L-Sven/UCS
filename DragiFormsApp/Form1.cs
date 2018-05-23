using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

namespace DragiFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            _func = new Functionality();
            _getDataFromDb =_func.FillUwpNameList;
            _getDataFromDb();   //Hämtar  referenser från Db och visar i listview
            _func.AddNameToList += AddNameToList;
            ApiMethods.progressBarEvent += ProgressBarHandler;
        }

        private static Functionality _func;
        public static ManualResetEvent formProgressMRE = new ManualResetEvent(false);

        private string _nameToReplace;

        #region ===== Delegates =====

        private readonly StartGettingData _getDataFromDb;
        private readonly ReplaceNameAndReferenceDelegate _replaceNameAndReference = Functionality.ReplaceNameAndReference;//_func.ReplaceNameAndReference;
        private readonly DisplayMessageBoxDelegate _displayMessageBox = Functionality.DisplayMessageBox;


        #endregion

        #region ===== Buttons =====

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnAcceptChanges_Click(object sender, EventArgs e)
        {
            //Denna switch används för att kontrollera om det nya namnet för avtalen ska tas från 
            //textbox eller från den vänstra listview. Vi kollar om användaren har kryssat för det.
            switch (radioBtnUseExistingUserName.Checked)
            {
                case true:
                    {
                        //Vi loopar igenom alla SelectedItems (vi tillåter bara att en markeras)
                        //och ger värdet av det till variablen _nameToReplace.
                        for (int i = 0; i < listViewExistingNames.SelectedItems.Count; i++)
                        {
                            _nameToReplace = listViewExistingNames.SelectedItems[i].SubItems[i].Text;
                        }
                    }
                    break;
                case false:
                    {
                        //Är radiobtn inte kryssat för, så kollar vi om ett namn har angetts och ett namn har lagds till i högra listview.
                        if (listViewNamesToChange.Items.Count < 1 || txtBoxNewName.Text == "" || txtBoxNewName.Text.Length < 2)
                        {
                            _displayMessageBox(
                            "Inget namn har angivits eller så har du inte valt ett namn som ska ersättas");
                            return;
                        }
                        _nameToReplace = txtBoxNewName.Text;
                    };
                    break;
            }
            List<string> names = new List<string>();
            foreach (ListViewItem item in listViewNamesToChange.Items)
            {
                names.Add(item.Text);
            }

            _replaceNameAndReference(_nameToReplace, names);

        }


        private void btnAddToChangeList_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selected in listViewExistingNames.SelectedItems)
            {
                listViewExistingNames.Items.Remove(selected);
                listViewNamesToChange.Items.Add(selected);
            }
        }

        private void BtnRemoveFromChangeList_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectedItem in listViewNamesToChange.SelectedItems)
            {
                listViewNamesToChange.Items.Remove(selectedItem);
                listViewExistingNames.Items.Add(selectedItem);
            }
        }

        #endregion

        //Denna funktion är kopplat till ett Event som utförs i Functionality filen och tar emot
        //varenda namn och lägger det i listview för befintliga seller namn
        public void AddNameToList(string s)
        {
            listViewExistingNames.Items.Add(s);
        }

        //Uppdaterar och ändrar progressbar
        public void ProgressBarHandler(int amountOfAgreements)
        {
            
            //progressBar.Value = 0;
            //progressBar.Step = 1; //Notis till mig själv, step betyder med hur mycket det ska öka varje steg.
            
                this.BeginInvoke((Action)delegate   //DÅ vi anropar metoden från en annan tråd måste vi säkerställa att den körs på UI tråden.
                {
                    progressBar.Maximum = amountOfAgreements;
                    using (Graphics gr = progressBar.CreateGraphics())
                    {
                        progressBar.Visible = true;
                        this.Cursor = Cursors.WaitCursor;

                        for (int i = 0; i < amountOfAgreements; i++)
                        {
                            ApiMethods.progressMRE.WaitOne();   //Vi väntar på besked från ApiMethods att få fortsätta.
                            progressBar.Value = i;  //Detta steg gör progressBar.Step överflödig. Value måste sättas innan gr.DrawString utförs!!!!

                            gr.DrawString(progressBar.Value + " av " + amountOfAgreements,  //Ett helvete av information om hur datan ska visas i ProgressBar.
                                SystemFonts.DefaultFont, Brushes.Black,
                                new PointF( //PointF är i grunden uträknaren för vart texten ska vara placerad i progressBar
                                    progressBar.Width / 2.3F -
                                    (gr.MeasureString(progressBar.Value.ToString(), SystemFonts.DefaultFont).Width / 2.0F),
                                    progressBar.Height / 2F -
                                    (gr.MeasureString(progressBar.Value.ToString(), SystemFonts.DefaultFont).Height / 2.0F)));

                            Thread.Sleep(300); //Används för att testa ProgressBar pga. kort tidsintervall
                            formProgressMRE.Set();  //Signalisera ApiMethods tråden att den kan fortsätta.
                        }
                        progressBar.Visible = false;    //Allt färdig, gör progressbar osynlig.
                        this.Cursor = Cursors.Default;
                    }
                });
        }
    }
}
