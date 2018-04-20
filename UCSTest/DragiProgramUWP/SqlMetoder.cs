using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using adk = AdkNetWrapper;

namespace DragiProgramUWP
{
    public delegate void SendDataToDatabaseDelegate();
    public delegate void GetDataFromDatabaseDelegate();

    public class SqlMetoder
    {
        private static readonly DisplayMessageBoxDelegate _displayMessageBox = Functionality.DisplayMessageBox;
        private static adk.Api.ADKERROR error;
        static String ftg = @"C:\ProgramData\SPCS\SPCS Administration\Företag\Ovnbol2000";
        static String sys = @"C:\ProgramData\SPCS\SPCS Administration\Gemensamma filer";
        private static int pData;
        private static int sData;
        private static bool userExists;


        public static void SendDataToDatabase(string newName, string oldName)
        {
            string newNameShort = "DS"; //Remove later, used for testing

            error = adk.Api.AdkOpen(ref sys, ref ftg);

            if (error.lRc != adk.Api.ADKE_OK)
            {
                string errortext = new string(' ', 200);
                int errtype = (int)adk.Api.ADK_ERROR_TEXT_TYPE.elRc;
                adk.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                _displayMessageBox(errortext);
            }

            sData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_CODE_OF_SELLER);
            error = AdkNetWrapper.Api.AdkFirst(sData);

            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                _displayMessageBox(errortext);
            }

            // Snurra som gör att sdata refererar till dragi!
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

                adk.Api.AdkUpdate(sData);
            }
            //sData Done!!

            pData = AdkNetWrapper.Api.AdkCreateData(AdkNetWrapper.Api.ADK_DB_AGREEMENT_HEAD);
            error = AdkNetWrapper.Api.AdkFirst(pData);


            if (error.lRc != AdkNetWrapper.Api.ADKE_OK)
            {
                String errortext = new String(' ', 200);
                int errtype = (int)AdkNetWrapper.Api.ADK_ERROR_TEXT_TYPE.elRc;
                AdkNetWrapper.Api.AdkGetErrorText(ref error, errtype, ref errortext, 200);
                _displayMessageBox(errortext);
            }



            // Snurra som loopar igenom alla avtal och gör dragi till säljare
            while (error.lRc == adk.Api.ADKE_OK)
            {
                string nameThatGetsReplaced = new string(' ', 26);
                adk.Api.AdkGetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_OUR_REFERENCE_NAME, ref nameThatGetsReplaced, 26);
                if (nameThatGetsReplaced == oldName)
                {
                    adk.Api.AdkSetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_OUR_REFERENCE_NAME, ref newName);
                    adk.Api.AdkSetStr(pData, AdkNetWrapper.Api.ADK_AGREEMENT_HEAD_SELLER_CODE, ref newNameShort);

                    adk.Api.AdkUpdate(pData);
                }

                // Sätter vidare pekaren till nästa artikelgrupp
                error = AdkNetWrapper.Api.AdkNext(pData);
            }

            // Stänger företaget
            AdkNetWrapper.Api.AdkClose();
        }

        public static void GetDataFromDatabase()
        {
            
        }
    }
}
