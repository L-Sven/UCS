using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DragiFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            GetDataFromDB = Functionality.FillUwpNameList;
            InitializeComponent();
            Functionality.AddNameToList += AddNameToList;
            GetDataFromDB();
        }

        private string _nameToReplace;

        #region ===== Delegates =====

        private readonly StartGettingData GetDataFromDB;
        private readonly ReplaceNameAndReferenceDelegate _replaceNameAndReference = Functionality.ReplaceNameAndReference;
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
            _replaceNameAndReference(_nameToReplace, listViewNamesToChange);

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
    }
}
