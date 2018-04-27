using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DragiFormsApp
{
    //public delegate void ReplaceNameAndReferenceDelegate(string s, string y);
    public delegate void ReplaceNameAndReferenceDelegate(string s, List<string> names);
    public delegate void DisplayMessageBoxDelegate(string s);
    public delegate void FillUwpNameListEventDelegate(string s);
    public delegate void StartGettingData();

    public class Functionality
    {
        private static readonly SendDataToDatabaseDelegate _sendDataToDatabase = ApiMethods.SendDataToDatabase;
        private readonly GetDataFromDatabaseDelegate _getDataFromDatabase = ApiMethods.GetDataFromDatabase;
        //private static readonly AmountAgreements _amountAgreements = ApiMethods.NumOfAgreements;

        public event FillUwpNameListEventDelegate AddNameToList;
        
        private static readonly ErrorLogger _logger = new ErrorLogger();

        public static void ReplaceNameAndReference(string newName, List<string> names)
        {
            //Vi plockar ut initialerna från det nya namnet med hjälp av en foreach som
            //hämtar ut första elementet i strängen för varje array element.
            try
            {
                string[] tempData = newName.Split(' ');
                string newNameShort = string.Empty;
                foreach (string t in tempData)
                {
                    newNameShort += t[0];
                }
                Task.Run(() =>_sendDataToDatabase(newName, newNameShort, names));
            }
            catch (Exception ex)
            {
                _logger.ErrorMessage(ex);
            }
        }

        public async void FillUwpNameList()
        {
            try
            {
                //Fyller på en lista med namn från Databasen, och signalerar eventet AddNameToList
                //som fyller på listan.
                List<string> listOfNames = await Task.Run(() => _getDataFromDatabase());
                foreach (var el in listOfNames)
                {
                    //Notis till själv, ? är en ifsats som vid true utför Invoke(ex); I detta fall om eventet har prenumeranter
                    AddNameToList?.Invoke(el);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorMessage(ex);
            }
        }

        public static void DisplayMessageBox(string meddelande)
        {
            MessageBox.Show(meddelande);
        }
    }
}
