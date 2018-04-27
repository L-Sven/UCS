using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADK = AdkNetWrapper;

namespace DragiFormsApp
{
    public delegate void SendDataToDatabaseDelegate(string newName, string newNameShort, List<string> oldNames);
    public delegate int AmountAgreements(ListView s);
    public delegate List<string> GetDataFromDatabaseDelegate();
    public delegate void ProgressBarEvent(int s);

    public class ApiMethods
    {
        static ErrorLogger _logger = new ErrorLogger();
        private static readonly DisplayMessageBoxDelegate DisplayMessageBox = Functionality.DisplayMessageBox;
        private static ADK.Api.ADKERROR error;
        private static string ftg = ConfigurationManager.AppSettings["ftgPath"]; //@"C:\ProgramData\SPCS\SPCS Administration\Företag\Ovnbol2000";
        private static string sys = ConfigurationManager.AppSettings["sysPath"]; //@"C:\ProgramData\SPCS\SPCS Administration\Gemensamma filer";
        private static int pData;
        private static int sData;
        private static bool userExists;
        public static event ProgressBarEvent progressBarEvent;
        public static ManualResetEvent progressMRE = new ManualResetEvent(false);

        public static void SendDataToDatabase(string newName, string newNameShort, List<string> oldNames)
        {
            int amountAgreements = NumOfAgreements(oldNames);

            //Vi måste aktivera eventet i en egen tråd för att det ska fungera asynkront.
            Task.Run(() => progressBarEvent?.Invoke(amountAgreements)); 

            userExists = false; //Nollställa userExists till defaultläge
            //Öppnar upp företaget
            error = ADK.Api.AdkOpen(ref sys, ref ftg);
            _logger.ErrorMessage(error);

            sData = ADK.Api.AdkCreateData(ADK.Api.ADK_DB_CODE_OF_SELLER);
            error = ADK.Api.AdkFirst(sData);
            _logger.ErrorMessage(error);

            // Vi loopar igenom Databasen om personen redan finns där, om det inte är fallet skapas en person.
            while (error.lRc == ADK.Api.ADKE_OK)
            {
                String namn = new string(' ', 25);
                ADK.Api.AdkGetStr(sData, ADK.Api.ADK_CODE_OF_SELLER_NAME, ref namn, 25);
                if (namn == newName)
                {
                    userExists = true;
                    break;
                }

                error = ADK.Api.AdkNext(sData);
                _logger.ErrorMessage(error);
            }

            if (!userExists)
            {
                ADK.Api.AdkSetStr(sData, ADK.Api.ADK_CODE_OF_SELLER_NAME, ref newName);
                ADK.Api.AdkSetStr(sData, ADK.Api.ADK_CODE_OF_SELLER_SIGN, ref newNameShort);
                ADK.Api.AdkSetBool(sData, ADK.Api.ADK_CODE_OF_SELLER_SELLER, 1);
                ADK.Api.AdkSetBool(sData, ADK.Api.ADK_CODE_OF_SELLER_REF, 1);

                ADK.Api.AdkAdd(sData);
            }
            //sData Done!!
            ADK.Api.AdkClose();

            //Öppna företaget igen.
            error = ADK.Api.AdkOpen(ref sys, ref ftg);
            _logger.ErrorMessage(error);

            pData = ADK.Api.AdkCreateData(ADK.Api.ADK_DB_AGREEMENT_HEAD);
            error = ADK.Api.AdkFirst(pData);
            _logger.ErrorMessage(error);

            // Vi loopar igenom alla avtal och byter ut det existerande namnet mot det nya namnet om jämförelsen
            //är sann
            try
            {
                int c = 0;
                while (error.lRc == ADK.Api.ADKE_OK)
                {
                    string nameThatGetsReplaced = new string(' ', 26);
                    ADK.Api.AdkGetStr(pData, ADK.Api.ADK_AGREEMENT_HEAD_OUR_REFERENCE_NAME,
                        ref nameThatGetsReplaced, 26);
                    foreach(var item in oldNames)
                    {
                        if (nameThatGetsReplaced == item)
                        {

                            ADK.Api.AdkSetStr(pData, ADK.Api.ADK_AGREEMENT_HEAD_OUR_REFERENCE_NAME, ref newName);
                            ADK.Api.AdkSetStr(pData, ADK.Api.ADK_AGREEMENT_HEAD_SELLER_CODE, ref newNameShort);
                            progressMRE.Set();
                            Form1.formProgressMRE.WaitOne();
                            ADK.Api.AdkUpdate(pData);
                            c++;
                        }
                    }



                    // Sätter vidare pekaren till nästa artikelgrupp
                    error = ADK.Api.AdkNext(pData);
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorMessage(ex);
            }

            // Stänger företaget
            ADK.Api.AdkClose();
        }



        //Metoden hämtar alla personer som är registrerad som seller!
        public static List<string> GetDataFromDatabase()
        {
            error = ADK.Api.AdkOpen(ref sys, ref ftg);
            _logger.ErrorMessage(error);

            sData = ADK.Api.AdkCreateData(ADK.Api.ADK_DB_CODE_OF_SELLER);
            error = ADK.Api.AdkFirst(sData);
            _logger.ErrorMessage(error);

            List<string> listOfNames = new List<string>();

            // loopar igenom seller namnen och plockar ut dem, lägger dem i en lista och fortsätter.
            while (error.lRc == ADK.Api.ADKE_OK)
            {
                String namn = new string(' ', 25);
                ADK.Api.AdkGetStr(sData, ADK.Api.ADK_CODE_OF_SELLER_NAME, ref namn, 25);
                listOfNames.Add(namn);
                error = ADK.Api.AdkNext(sData);
                _logger.ErrorMessage(error);
            }

            // Stänger företaget
            ADK.Api.AdkClose();

            return listOfNames;
        }

        //Metod för att räkna ut hur många avtal som uppfyller kraven för namnändring.
        public static int NumOfAgreements(List<string> oldNames)
        {
            sData = new int();
            int amountAgreements = 0;

            error = ADK.Api.AdkOpen(ref sys, ref ftg);
            _logger.ErrorMessage(error);

            sData = ADK.Api.AdkCreateData(ADK.Api.ADK_DB_AGREEMENT_HEAD);
            error = ADK.Api.AdkFirst(sData);
            _logger.ErrorMessage(error);

            // loopar igenom seller namnen och plockar ut dem, lägger dem i en lista och fortsätter.
            while (error.lRc == ADK.Api.ADKE_OK)
            {
                String namn = new string(' ', 25);
                ADK.Api.AdkGetStr(sData, ADK.Api.ADK_AGREEMENT_HEAD_OUR_REFERENCE_NAME, ref namn, 25);

                foreach (var item in oldNames)
                {
                    if (namn == item)
                    {
                        amountAgreements++;
                    }
                }
                
                //Pekar mot nästa avtal
                error = ADK.Api.AdkNext(sData);
                _logger.ErrorMessage(error);
            }

            // Stänger företaget
            ADK.Api.AdkClose();

            return amountAgreements;
        }
    }
}
