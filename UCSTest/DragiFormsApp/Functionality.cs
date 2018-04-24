using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DragiFormsApp
{
    //public delegate void ReplaceNameAndReferenceDelegate(string s, string y);
    public delegate void ReplaceNameAndReferenceDelegate(string s, ListView x);
    public delegate void DisplayMessageBoxDelegate(string s);
    public delegate void FillUwpNameListEventDelegate(string s);
    public delegate void StartGettingData();

    public class Functionality
    {
        private static readonly SendDataToDatabaseDelegate SendDataToDatabase = SqlMetoder.SendDataToDatabase;
        private static readonly GetDataFromDatabaseDelegate GetDataFromDatabase = SqlMetoder.GetDataFromDatabase;

        public static event FillUwpNameListEventDelegate AddNameToList;

        public static void ReplaceNameAndReference(string newName, ListView oldNames)
        {
            //Vi plockar ut initialerna från det nya namnet med hjälp av en foreach som
            //hämtar ut först elementet (0) i strängen.
            string[] tempData = newName.Split(' ');
            string newNameShort = string.Empty;
            foreach (string t in tempData)
            {
                newNameShort += t[0];
            }
            SendDataToDatabase(newName, newNameShort, oldNames);
        }

        public static async void FillUwpNameList()
        {
            List<string> listOfNames = await Task.Run(() => GetDataFromDatabase());

            foreach (var el in listOfNames)
            {
                if (AddNameToList != null)
                {
                    AddNameToList(el);
                }
            }


        }

        public static void DisplayMessageBox(string meddelande)
        {
            MessageBox.Show(meddelande);
        }
    }
}
