using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using adk = AdkNetWrapper;

namespace DragiFormsApp
{
    public delegate void SendDataToDatabaseDelegate(string newName, string newNameShort, ListView oldName);
    public delegate List<string> GetDataFromDatabaseDelegate();

    public class SqlMetoder
    {
        private static readonly DisplayMessageBoxDelegate DisplayMessageBox = Functionality.DisplayMessageBox;
        private static adk.Api.ADKERROR error;
        static string ftg = @"C:\ProgramData\SPCS\SPCS Administration\Företag\Ovnbol2000";
        static string sys = @"C:\ProgramData\SPCS\SPCS Administration\Gemensamma filer";
        private static int pData;
        private static int sData;
        private static bool userExists;

        public static void SendDataToDatabase(string newName, string newNameShort, ListView oldNames)
        {
            userExists = false; //Nollställa userExists till defaultläge
            //Öppnar upp företaget
            error = adk.Api.AdkOpen(ref sys, ref ftg);

            if (error.lRc != adk.Api.ADKE_OK)
            {
                string errortext = new string(' ', 200);
                int errtype = (int)adk.Api.ADK_ERROR_TEXT_TYPE.elRc;
                adk.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                DisplayMessageBox(errortext);
            }

            sData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_CODE_OF_SELLER);
            error = AdkNetWrapper.Api.AdkFirst(sData);

            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                DisplayMessageBox(errortext);
            }

            // Vi loopar igenom Databasen om personen redan finns där, om det inte är fallet skapas en person.
            while (error.lRc == adk.Api.ADKE_OK)
            {
                String namn = new string(' ', 25);
                AdkNetWrapper.Api.AdkGetStr(sData, AdkNetWrapper.Api.ADK_CODE_OF_SELLER_NAME, ref namn, 25);
                if (namn == newName)
                {
                    userExists = true;
                    break;
                }

                error = AdkNetWrapper.Api.AdkNext(sData);
            }

            if (!userExists)
            {
                adk.Api.AdkSetStr(sData, AdkNetWrapper.Api.ADK_CODE_OF_SELLER_NAME, ref newName);
                adk.Api.AdkSetStr(sData, AdkNetWrapper.Api.ADK_CODE_OF_SELLER_SIGN, ref newNameShort);
                adk.Api.AdkSetBool(sData, AdkNetWrapper.Api.ADK_CODE_OF_SELLER_SELLER, 1);
                adk.Api.AdkSetBool(sData, AdkNetWrapper.Api.ADK_CODE_OF_SELLER_REF, 1);

                adk.Api.AdkAdd(sData);
            }
            //sData Done!!
            AdkNetWrapper.Api.AdkClose();

            //Öppna företaget igen.
            error = adk.Api.AdkOpen(ref sys, ref ftg);

            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_AGREEMENT_HEAD);
            error = AdkNetWrapper.Api.AdkFirst(pData);


            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                DisplayMessageBox(errortext);
            }
            
            
            // Vi loopar igenom alla avtal och byter ut det existerande namnet mot det nya namnet om jämförelsen
            //är sann
            while (error.lRc == adk.Api.ADKE_OK)
            {
                string nameThatGetsReplaced = new string(' ', 26);
                adk.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_OUR_REFERENCE_NAME, ref nameThatGetsReplaced, 26);
                for (int i = 0; i < oldNames.Items.Count; i++)
                {
                    if (nameThatGetsReplaced == oldNames.Items[i].Text)
                    {
                        adk.Api.AdkSetStr(pData, adk.Api.ADK_AGREEMENT_HEAD_OUR_REFERENCE_NAME, ref newName);
                        adk.Api.AdkSetStr(pData, adk.Api.ADK_AGREEMENT_HEAD_SELLER_CODE, ref newNameShort);

                        adk.Api.AdkUpdate(pData);
                    }
                }

                

                // Sätter vidare pekaren till nästa artikelgrupp
                error = AdkNetWrapper.Api.AdkNext(pData);
            }
            
            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();
        }



        //Metoden hämtar alla personer som är registrerad som seller!
        public static List<string> GetDataFromDatabase()
        {
            error = AdkNetWrapper.Api.AdkOpen(ref sys, ref ftg);

            if (error.lRc != adk.Api.ADKE_OK)
            {
                string errortext = new string(' ', 200);
                int errtype = (int)adk.Api.ADK_ERROR_TEXT_TYPE.elRc;
                adk.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                DisplayMessageBox(errortext);
            }

            sData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_CODE_OF_SELLER);
            error = AdkNetWrapper.Api.AdkFirst(sData);

            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                DisplayMessageBox(errortext);
            }

            List<string> listOfNames = new List<string>();

            // loopar igenom seller namnen och plockar ut dem, lägger dem i en lista och fortsätter.
            while (error.lRc == adk.Api.ADKE_OK)
            {
                String namn = new string(' ', 25);
                AdkNetWrapper.Api.AdkGetStr(sData, AdkNetWrapper.Api.ADK_CODE_OF_SELLER_NAME, ref namn, 25);
                listOfNames.Add(namn);
                error = AdkNetWrapper.Api.AdkNext(sData);
                Thread.Sleep(500);
            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();

            return listOfNames;
        }
    }
}
